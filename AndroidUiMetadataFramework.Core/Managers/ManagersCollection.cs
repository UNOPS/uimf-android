using App.Style;
namespace AndroidUiMetadataFramework.Core.Managers
{
    public class ManagersCollection
    {
        public StyleRegister StyleRegister { get; set; } = new StyleRegister();
        public InputManagerCollection InputManagerCollection { get; set; } = new InputManagerCollection();
        public OutputManagerCollection OutputManagerCollection { get; set; } = new OutputManagerCollection();
        public EventHandlerManagerCollection EventHandlerManagerCollection { get; set; } = new EventHandlerManagerCollection();
    }
}