using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace OutDiskReadService.ebThread
{
    public class EBThread : IEBRunnable
    {
        protected IEBCommand command;
        protected Thread thread;

        public EBThread()
        {
            thread = new Thread(this.run);
        }

        public EBThread(IEBCommand target_)
        {
            command = target_;
            thread = new Thread(this.run);
        }

        public virtual void run()
        {
            if (command != null)
            {
                command.execute(null);
            }
        }

        public void start()
        {
            thread.Start();
        }

        public void stop()
        {
            thread.Abort();
        }
    }
}
