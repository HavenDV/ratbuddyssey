using Audyssey;
using System.Reactive;
using System.Windows;

namespace Ratbuddyssey
{
    public partial class App
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Interactions.Warning.RegisterHandler(static context =>
            {
                var message = context.Input;

                MessageBox.Show(message, "Warning:", MessageBoxButton.OK, MessageBoxImage.Warning);

                context.SetOutput(Unit.Default);
            });
            Interactions.Exception.RegisterHandler(static context =>
            {
                var exception = context.Input;

                MessageBox.Show($"{exception}", "Error:", MessageBoxButton.OK, MessageBoxImage.Error);

                context.SetOutput(Unit.Default);
            });
        }
    }
}
