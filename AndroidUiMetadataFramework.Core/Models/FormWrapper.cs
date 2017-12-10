namespace AndroidUiMetadataFramework.Core.Models
{
    using System.Collections.Generic;
    using UiMetadataFramework.Basic.Response;
    using UiMetadataFramework.Core;

    public interface IFormWrapper
    {
        void ReloadView(MyFormHandler myFormHandler, ReloadResponse reloadResponse);

        void UpdateView(MyFormHandler myFormHandler,
            FormMetadata metadata,
            IDictionary<string, object> inputFieldValues = null,
            string submitAction = null);

        void CloseForm();
    }
}