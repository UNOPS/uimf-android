namespace AndroidUiMetadateFramework.Core.Models
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UiMetadataFramework.Core;

    public interface IFormWrapper
    {
        void UpdateView(MyFormHandler myFormHandler, FormMetadata metadata, IDictionary<string, object> inputFieldValues = null);
        Task UpdateViewAsync(MyFormHandler myFormHandler, FormMetadata metadata, IDictionary<string, object> inputFieldValues = null);
    }
}