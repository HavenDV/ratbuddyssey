using System;
using System.Diagnostics;
using System.Reactive.Linq;

namespace Ratbuddyssey.Utilities
{
    public class DefaultExceptionHandler : IObserver<Exception>
    {
        private static async void OnException(Exception value)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            await Interactions.Exception.Handle(value);
        }

        public void OnNext(Exception value)
        {
            OnException(value);
        }

        public void OnError(Exception error)
        {
            OnException(error);
        }

        public void OnCompleted()
        {
        }
    }
}