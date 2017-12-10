namespace AndroidUiMetadataFramework.Core.Managers
{
    using System.Collections.Generic;
    using AndroidUiMetadataFramework.Core.Models;
    using UiMetadataFramework.MediatR;

    public interface IEventHandlerManager
    {
        void HandleEvent(IDictionary<string,object> inputEventCustomProperties, FormInputManager inputManager, InvokeForm.Response result);
    }
}