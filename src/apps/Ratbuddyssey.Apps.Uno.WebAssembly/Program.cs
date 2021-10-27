using Ratbuddyssey;
using Uno.UI;

#if DEBUG
FeatureConfiguration.UIElement.AssignDOMXamlName = true;
#endif

Application.Start(callback =>
{
    _ = new App();
});
