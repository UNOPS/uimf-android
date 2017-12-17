namespace AndroidUiMetadataFramework.Core.Outputs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Android.App;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Attributes;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;
    using Newtonsoft.Json.Linq;
    using UiMetadataFramework.Basic.Input;
    using UiMetadataFramework.Basic.Output;
    using UiMetadataFramework.Core;

    [Output(Type = "paginated-data")]
    public class PaginationOutput : IOutputManager
    {
        private IList<object> ItemList { get; set; }
        private LinearLayout OutputList { get; set; }
        private int PageIndex { get; set; } = 1;
        private int TotalCount { get; set; }

        public View GetView(OutputFieldMetadata outputField,
            object value,
            MyFormHandler myFormHandler,
            FormMetadata formMetadata,
            List<FormInputManager> inputsManager)
        {
            this.OutputList = new LinearLayout(Application.Context) { Orientation = Orientation.Vertical };
            if (!string.IsNullOrEmpty(outputField.Label))
            {
                var label = new TextView(Application.Context) { Text = outputField.Label + ": " };
                this.OutputList.AddView(label, this.OutputList.MatchParentWrapContent());
                myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("TextView", label);
            }

            var paginatedData = value.CastTObject<PaginatedData<object>>();
            this.ItemList = paginatedData.Results.ToList();
            this.TotalCount = paginatedData.TotalCount;

            var listView = this.ItemList.IntializeListView(outputField, myFormHandler);

            if (this.TotalCount > 10)
            {
                if (formMetadata != null)
                {
                    var btnLoadMore = this.CreateLoadMoreButton(myFormHandler, formMetadata, outputField, listView, inputsManager,
                        myFormHandler.AllFormsMetadata);
                    myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("Button Link LoadMore", btnLoadMore);
                    listView.AddFooterView(btnLoadMore);
                }
            }
            
            if (this.ItemList.Count == 0)
            {
                var noResult = new TextView(Application.Context)
                {
                    Text = "no data."
                };
                noResult.LayoutParameters = noResult.WrapContent();
                myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("TextView NoResult", noResult);
                this.OutputList.AddView(noResult);
            }
            listView.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, listView.GetListHeigth());
            myFormHandler.ManagersCollection.StyleRegister.ApplyStyle("ListView", listView);
            this.OutputList.AddView(listView);
            return this.OutputList;
        }

        private Button CreateLoadMoreButton(MyFormHandler myFormHandler,
            FormMetadata formMetadata,
            OutputFieldMetadata outputField,
            ListView listView,
            List<FormInputManager> inputsManager,
            Dictionary<string, FormMetadata> allFormsMetadata)
        {
            var btnLoadMore = new Button(Application.Context) { Text = "Load More" };
            btnLoadMore.Click += async (sender, args) =>
            {
                this.PageIndex++;
                var paginator = inputsManager.Find(a => a.Input.Type == "paginator");
                paginator.Manager.SetValue(new Paginator
                {
                    PageSize = 10,
                    PageIndex = this.PageIndex
                });
                var response = await myFormHandler.HandleFormAsync(formMetadata, inputsManager);
                object responsevalue;
                if (response.Data.GetType() == typeof(JObject))
                {
                    var jsonObj = (JObject)response.Data;
                    responsevalue = jsonObj.GetValue(outputField.Id, StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    var propertyInfo = response.Data.GetType().GetProperty(outputField.Id);
                    responsevalue = propertyInfo?.GetValue(response.Data, null);
                }

                var paginatedData = responsevalue.CastTObject<PaginatedData<object>>();
                var newList = paginatedData.Results.ToList();

                if (newList.Any())
                {
                    foreach (var item in newList)
                    {
                        this.ItemList.Add(item);
                    }
                }
                //// Appending new data to menuItems ArrayList
                listView.Refresh(this.ItemList, outputField, myFormHandler, allFormsMetadata);

                // Setting new scroll position
                listView.SetSelectionFromTop(10 * (this.PageIndex - 1), 0);
                listView.LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, listView.GetListHeigth());

                if (this.ItemList.Count == this.TotalCount)
                {
                    btnLoadMore.Visibility = ViewStates.Invisible;
                }
            };

            return btnLoadMore;
        }
    }
}