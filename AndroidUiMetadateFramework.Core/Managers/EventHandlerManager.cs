namespace AndroidUiMetadateFramework.Core.Managers
{
    using AndroidUiMetadateFramework.Core.Models;
    using UiMetadataFramework.MediatR;

    public interface IEventHandlerManager
    {
        void HandleEvent(object inputEventCustomProperties, FormInputManager inputManager, InvokeForm.Response result);
    }
}