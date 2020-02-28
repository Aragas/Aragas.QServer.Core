using Aragas.TupleEventSystem;

using System;
using System.Threading;

namespace Aragas.QServer.Core
{
    public abstract class BaseThreadSafeConnectionHandler
    {
        public BaseEventHandler<EventArgs> Ready { get; set; } = new WeakReferenceEventHandler<EventArgs>();
        public BaseEventHandler<EventArgs> Disconnected { get; set; } = new WeakReferenceEventHandler<EventArgs>();
        //public event EventHandler Ready;
        //public event EventHandler Disconnected;

        protected CancellationTokenSource UpdateToken { get; set; } = new CancellationTokenSource();
        protected ManualResetEventSlim UpdateLock { get; } = new ManualResetEventSlim(false);

        protected ManualResetEventSlim ConnectionLock { get; } = new ManualResetEventSlim(true); // Will cause deadlock if false. See Leave();

        private bool IsDisposing { get; set; }


        public void StartListening()
        {
            if (!UpdateLock.IsSet)
            {
                //UpdateToken = new CancellationTokenSource();
                new Thread(Update).Start();
            }
            else
            {
                throw new Exception("UpdateThread is already running!");
            }
        }

        protected void Join()
        {
            Ready?.Invoke(this, EventArgs.Empty);
        }

        protected void Leave()
        {
            ConnectionLock.Wait(); // this should ensure we will send every packet enqueued at the moment of calling Leave()

            if (UpdateToken?.IsCancellationRequested == false)
            {
                UpdateToken.Cancel();
                UpdateLock.Wait(); // Wait for the Update cycle to finish
            }

            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Will raise Disconnected event.
        /// </summary>
        /// <param name="reason"></param>
        public virtual void SendKick(string reason = "") { Leave(); }

        public abstract void Update();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposing)
            {
                if (disposing)
                {
                    if (UpdateToken?.IsCancellationRequested == false)
                    {
                        UpdateToken.Cancel();
                        UpdateLock.Wait();
                    }

                    UpdateLock.Dispose();
                    ConnectionLock.Dispose();

                    Ready?.Dispose();
                    Disconnected?.Dispose();
                }


                IsDisposing = true;
            }
        }
        ~BaseThreadSafeConnectionHandler()
        {
            Dispose(false);
        }
    }
}