using System;
using System.Collections.Generic;
using System.Threading;

namespace Foyerry.Core.Lucene
{
    public class LuceneIndexWriter : IDisposable
    {
        private Queue<WriteIndexTask> _queueTask;
        private static object _lockObj = new object();

        private static LuceneIndexWriter _luceneIndexWriter;

        private bool _state;
        private readonly Semaphore _semaphore;

        public static LuceneIndexWriter Instance
        {
            get
            {
                lock (_lockObj)
                {
                    return _luceneIndexWriter ?? (_luceneIndexWriter = new LuceneIndexWriter());
                }
            }
        }

        public void InsertIndexTask(WriteIndexTask task)
        {
            if (task != null)
            {
                lock (_lockObj)
                {
                    _queueTask.Enqueue(task);
                    _semaphore.Release();
                }
            }
        }

        private LuceneIndexWriter()
        {
            lock (_lockObj)
            {
                try
                {
                    _state = true;
                    _semaphore = new Semaphore(0, int.MaxValue);
                    _queueTask = new Queue<WriteIndexTask>();
                    var thread = new Thread(Work) { IsBackground = true };
                    thread.Start();
                }
                catch
                {
                }
            }
        }

        private void Work()
        {
            while (true)
            {
                if (_queueTask.Count > 0)
                {
                    WriteIndex();
                }
                else
                {
                    if (Wait()) break;
                }
            }
        }

        private void WriteIndex()
        {
            WriteIndexTask task = null;
            lock (_lockObj)
            {
                if (_queueTask.Count > 0)
                {
                    task = _queueTask.Dequeue();
                }
            }
            if (task != null)
            {
                WriteIndexToDirectory(task);
            }
        }

        private void WriteIndexToDirectory(WriteIndexTask task)
        {
            //TODO:
            //1根据taskId从数据库查找文档
            //2构建Document
            //3 根据task.Type 判断是update还是add
            //4 写入
        }

        private bool Wait()
        {
            //determine log life time is true or false
            if (_state)
            {
                WaitHandle.WaitAny(new WaitHandle[] { _semaphore }, -1, false);
                return false;
            }
            //FileClose();
            return true;
        }

        public void Dispose()
        {
            _state = false;
        }
    }

    public class WriteIndexTask
    {
        public int TaskId { get; set; }
        /// <summary>
        /// Type=1时表示insert
        /// Type=2是表示update
        /// </summary>
        public IndexOperateType Type { get; set; }
    }

    public enum IndexOperateType
    {
        Add = 1,
        Update = 2,
        DeleteOne = 3,
        DeleteAll = 4
    }

}
