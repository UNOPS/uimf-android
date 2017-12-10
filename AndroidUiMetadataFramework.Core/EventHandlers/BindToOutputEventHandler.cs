namespace AndroidUiMetadataFramework.Core.EventHandlers
{
    using System;
    using System.Collections.Generic;
    using AndroidUiMetadataFramework.Core.Attributes;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;
    using Newtonsoft.Json.Linq;
    using UiMetadataFramework.Basic.EventHandlers;
    using UiMetadataFramework.MediatR;

    [EventHandler(Type = "bind-to-output")]
    public class BindToOutputEventHandler : IEventHandlerManager
    {
        public void HandleEvent(IDictionary<string, object> inputEventCustomProperties, FormInputManager inputManager, InvokeForm.Response result)
        {
            var outputField = inputEventCustomProperties.GetCustomProperty<string>(nameof(BindToOutputAttribute.OutputFieldId));

            object value;
            if (result.Data.GetType() == typeof(JObject))
            {
                var jsonObj = (JObject)result.Data;
                value = jsonObj.GetValue(outputField, StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                var propertyInfo = result.Data.GetType().GetProperty(outputField);
                value = propertyInfo?.GetValue(result.Data, null);
            }

            inputManager.Manager.SetValue(value);
        }

    }
}