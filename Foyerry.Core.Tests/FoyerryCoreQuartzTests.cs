using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Foyerry.Core.Quartz;

namespace Foyerry.Core.Tests
{
    public class FoyerryCoreQuartzTests
    {
        public static void Add()
        {
            new QuartzJob().RegisterJob(TestDemo.Start, TimeSpan.FromMinutes(5), 3, TimeSpan.FromSeconds(10));
        }
    }

    public class TestDemo
    {
        public static void Start()
        {
        }
    }
}
