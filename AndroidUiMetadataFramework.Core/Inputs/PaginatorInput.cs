namespace AndroidUiMetadataFramework.Core.Inputs
{
    using System;
    using System.Collections.Generic;
    using Android.App;
    using Android.Text;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Attributes;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;
    using UiMetadataFramework.Basic.Input;

    [Input(Type = "paginator")]
    public class PaginatorInput : IInputManager
    {
        private EditText Ascending { get; set; }
        private LinearLayout InputPaginator { get; set; }
        private EditText OrderBy { get; set; }
        private EditText PageIndex { get; set; }
        private EditText PageSize { get; set; }

        public View GetView(IDictionary<string, object> inputCustomProperties, MyFormHandler myFormHandler)
        {
            this.InputPaginator = new LinearLayout(Application.Context);
            this.Ascending = new EditText(Application.Context) { InputType = InputTypes.ClassText };
            this.OrderBy = new EditText(Application.Context) { InputType = InputTypes.ClassText };
            this.PageIndex = new EditText(Application.Context) { InputType = InputTypes.ClassText };
            this.PageSize = new EditText(Application.Context) { InputType = InputTypes.ClassText };
            this.InputPaginator.AddView(this.Ascending);
            this.InputPaginator.AddView(this.OrderBy);
            this.InputPaginator.AddView(this.PageIndex);
            this.InputPaginator.AddView(this.PageSize);
            return this.InputPaginator;
        }

        public object GetValue()
        {
            return new Paginator
            {
                Ascending = !string.IsNullOrEmpty(this.Ascending.Text) && Convert.ToBoolean(this.Ascending.Text),
                OrderBy = this.OrderBy.Text,
                PageIndex = !string.IsNullOrEmpty(this.PageIndex.Text) ? Convert.ToInt32(this.PageIndex.Text) : 1,
                PageSize = !string.IsNullOrEmpty(this.PageSize.Text) ? Convert.ToInt32(this.PageSize.Text) : 10
            };
        }

        public void SetValue(object value)
        {
            Paginator paginator = value.CastTObject<Paginator>();

            this.Ascending.Text = paginator.Ascending?.ToString();
            this.OrderBy.Text = paginator.OrderBy;
            this.PageIndex.Text = paginator.PageIndex?.ToString();
            this.PageSize.Text = paginator.PageSize?.ToString();
        }
    }
}