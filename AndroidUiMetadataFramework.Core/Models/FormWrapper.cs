namespace AndroidUiMetadataFramework.Core.Models
{
    using UiMetadataFramework.Basic.Response;

    public interface IFormWrapper
    {
        void CloseForm();
        void ReloadView(MyFormHandler myFormHandler, ReloadResponse reloadResponse);
        void UpdateView(MyFormHandler myFormHandler,
            FormParameter formParameter,
            string submitAction = null);
    }
}