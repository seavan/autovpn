using System;
using System.Threading;

namespace autovpn_test
{
    public class ThreadWorker : IDisposable
    {
        public ThreadWorker()
        {
            StopRequested = false;
        }

        public int CycleTime { get; set; }

        public void Worker()
        {
            while (!StopRequested)
            {
                try
                {
                    DoJob();
                    Thread.Sleep(CycleTime);
                }
                catch (ThreadAbortException _te)
                {
                    //Tracer.I.Error(_te.Message);
                    break;
                }
                catch (Exception _e)
                {
                    //Tracer.I.Error(_e.Message);
                }
            }

            m_Thread = null;
        }

        public virtual void DoJob()
        {

        }

        private bool StopRequested { get; set; }

        public bool IsBackground { get; set; }

        public void Start()
        {
            if (IsAlive)
            {
                return;
            }

            m_Thread = new Thread(Worker);
            m_Thread.IsBackground = IsBackground;
            m_Thread.Start();
        }

        public bool IsAlive
        {
            get
            {
                // todo add IsAlive check with lock
                return (m_Thread != null);
            }
        }

        public void Stop()
        {
            if (IsAlive)
            {
                try
                {
                    m_Thread.Abort();
                }
                catch (Exception _e)
                {
                    //Tracer.I.Error(_e.Message);
                }
                m_Thread = null;
            }
        }

        public void StopGracefully()
        {
            StopRequested = true;
            int count = 50;
            int c = 0;
            int interval = 500;
            while (IsAlive)
            {
                if (c++ > count)
                {
                    Stop();
                    break;
                }
                Thread.Sleep(interval);
            }

        }

        private Thread m_Thread;

        #region IDisposable Members

        public void Dispose()
        {
            Stop();
        }

        #endregion
    }
}