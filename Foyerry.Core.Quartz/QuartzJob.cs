using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace Foyerry.Core.Quartz
{
    public class QuartzJob
    {
        private readonly IScheduler _iScheduler = StdSchedulerFactory.GetDefaultScheduler();

        public void Start()
        {
            _iScheduler.Start();
        }

        public void ShutDown()
        {
            _iScheduler.Shutdown();
        }
        /// <summary>
        /// 创建job任务
        /// </summary>
        /// <param name="action"></param>
        /// <param name="interval">job调度的间隔时间</param>
        /// <param name="repeatCount">任务执行的次数(默认重复执行)：0 表示执行一次；小于0 表示重复执行；大于0 表示执行的次数</param>
        /// <param name="delay">多长时间后开始执行</param>
        public void RegisterJob(Action action, TimeSpan interval, int repeatCount = -1, TimeSpan delay = new TimeSpan())
        {
            try
            {
                var type = DynamicJob.CreateType(action);
                IJobDetail jobDetail = JobBuilder.Create(type).Build();
                var trigger = TriggerBuilder.Create()
                    .StartAt(DateTimeOffset.Now.Add(delay))
                    .WithSimpleSchedule(x =>
                    {
                        x.WithInterval(interval);
                        if (repeatCount < 0)
                        {
                            x.RepeatForever();
                        }
                        else if (repeatCount > 0)
                        {
                            x.WithRepeatCount(repeatCount);
                        }
                    })
                    .Build();
                _iScheduler.ScheduleJob(jobDetail, trigger);
            }
            catch (Exception e)
            {

            }

        }

        public void RegisterJob(Action action, string cronExpression)
        {
            try
            {
                var type = DynamicCreateType(action);
                IJobDetail jobDetail = JobBuilder.Create(type).Build();
                var trigger = TriggerBuilder.Create()
                    .WithCronSchedule(cronExpression)
                    .Build();
                _iScheduler.ScheduleJob(jobDetail, trigger);
            }
            catch (Exception e)
            {
            }

        }

        public static Type DynamicCreateType(Action action)
        {
            //动态创建程序集
            AssemblyName assemblyName = new AssemblyName("");
            AssemblyBuilder assemblyBuilder =
                AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            //动态创建模块
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("");
            //动态创建类
            TypeBuilder typeBuilder = moduleBuilder.DefineType("");
            //添加父类(IJob)
            typeBuilder.SetParent(typeof(JobBase));
            return typeBuilder.CreateType();

        }

        private static class DynamicJob
        {
            public static readonly ConcurrentDictionary<string, JobAction> JobActions
                = new ConcurrentDictionary<string, JobAction>();

            public static Type CreateType(Action action)
            {
                //动态创建程序集
                AssemblyName assemblyName = new AssemblyName("Assembly_" + Guid.NewGuid().ToString());
                AssemblyBuilder assemblyBuilder =
                    AppDomain.CurrentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
                //动态创建模块
                ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("Module_" + Guid.NewGuid().ToString());
                //动态创建类
                var typeName = string.Format("Type_" + Guid.NewGuid().ToString());
                TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName);
                //添加父类(IJob)
                typeBuilder.SetParent(typeof(JobBase));
                var jobAction = new JobAction
                {
                    Action = action,
                };
                JobActions.TryAdd(typeName, jobAction);
                return typeBuilder.CreateType();


            }

            public static IJobDetail CreateJobDetail(Action action)
            {
                var type = CreateType(action);
                return JobBuilder.Create(type).Build();
            }
        }

        private class JobAction
        {
            public Action Action { get; set; }
        }

        public class JobBase : IJob
        {
            public JobBase()
            {
            }

            public void Execute(IJobExecutionContext context)
            {
                try
                {
                    var action = DynamicJob.JobActions[GetType().Name];
                    action.Action();
                    return;
                }
                catch (Exception e)
                {
                }
            }
        }
    }
}
