using System;
using System.Threading;
using System.Windows.Threading;

namespace CardCreator.Features.Threading
{
    public static class ThreadManager
    {
        public static void RunActionInNewThread(Action action)
        {
            var newThread = new Thread(new ThreadStart(() =>
            {
                // Create our context, and install it:
                SynchronizationContext.SetSynchronizationContext(
                    new DispatcherSynchronizationContext(
                        Dispatcher.CurrentDispatcher));

                action();

                // Start the Dispatcher Processing
                Dispatcher.Run();
            }));
            // Set the apartment state
            newThread.SetApartmentState(ApartmentState.STA);
            // Make the thread a background thread
            newThread.IsBackground = true;
            // Start the thread
            newThread.Start();
        }

        public static void RunActionWithDispatcher(Dispatcher dispatcher, Action action)
        {
            dispatcher.BeginInvoke(DispatcherPriority.Normal, new ThreadStart(() => action()));
        }
    }
}
