using System.Diagnostics;

namespace Ratbuddyssey;

public class DefaultExceptionHandler : IObserver<Exception>
{
    private static async void OnException(Exception value)
    {
        if (Debugger.IsAttached)
        {
            Debugger.Break();
        }

        await MessageInteractions.Exception.Handle(value);
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
