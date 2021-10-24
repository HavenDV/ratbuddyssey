using Ratbuddyssey;
using Uno.UI;
using Windows.UI.Xaml;

#if DEBUG
FeatureConfiguration.UIElement.AssignDOMXamlName = true;
#endif

Application.Start(callback =>
{
    _ = new App();
});
