namespace AndroidUiMetadataFramework.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Android.App;
    using Android.Content;
    using Android.Views;
    using Android.Widget;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using UiMetadataFramework.Core;
    using UiMetadataFramework.MediatR;

    public static class Extension
    {
        public static T CastTObject<T>(this object obj)
        {
            if (obj.GetType() == typeof(JObject))
            {
                return JsonConvert.DeserializeObject<T>(obj.ToString());
            }
            if (obj.GetType() == typeof(JValue))
            {
                var jValue = (JValue)obj;
                return jValue.HasValues ? ((JValue)obj).ToObject<T>() : default(T);
            }
            if (obj.GetType() == typeof(JArray))
            {
                return ((JArray)obj).ToObject<T>();
            }
            return (T)obj;
        }

        public static int ConvertPixelsToDp(this int pixelValue)
        {
            var dp = (int)(pixelValue / Application.Context.Resources.DisplayMetrics.Density);
            return dp;
        }

        public static T GetCustomProperty<T>(this IDictionary<string, object> customProperties, string property)
        {
            var dictionary = new Dictionary<string, object>(customProperties, StringComparer.OrdinalIgnoreCase);
            object value;
            dictionary.TryGetValue(property, out value);
            return value != null ? value.CastTObject<T>() : default(T);
        }

        public static int GetListHeigth(this ListView listView)
        {
            var listAdapter = listView.Adapter;
            if (listAdapter == null)
            {
                return 0;
            }

            var totalHeight = 0;

            for (var i = 0; i < listAdapter.Count; i++)
            {
                var listItem = listAdapter.GetView(i, null, listView);
                listItem.Measure(0, 0);
                totalHeight += listItem.MeasuredHeight;
            }

            return totalHeight;
        }

        public static IEnumerable<object> GetTypeaheadSource(this object source, MyFormHandler myFormHandler)
        {
            if (source is string)
            {
                var list = new Dictionary<string, object> { { "query", "" } };
                var obj = JsonConvert.SerializeObject(list);

                var dataSource = source.ToString();
                var request = new InvokeForm.Request
                {
                    Form = dataSource,
                    InputFieldValues = obj
                };
                var result = Task.Run(
                    () => myFormHandler.InvokeFormAsync(new[] { request }, false));

                var response = result.Result;
                var typeahead = response[0].Data.CastTObject<TypeaheadResponse<object>>();
                if (typeahead != null)
                {
                    return typeahead.Items;
                }
            }
            else
            {
                return (IEnumerable<object>)source;
            }
            return new List<object>();
        }

        public static ListView IntializeListView(this IList<object> itemList, OutputFieldMetadata outputField, MyFormHandler myFormHandler)
        {
            var listView = new ListView(Application.Context);
            listView.SetPadding(10, 0, 10, 0);
            var outputFieldProperty = outputField.CustomProperties.GetCustomProperty<IEnumerable<OutputFieldMetadata>>("columns");
            listView.FastScrollEnabled = true;
            var adapter = new ListCustomAdapter<object>(itemList.ToList(), outputFieldProperty, myFormHandler);
            listView.Adapter = adapter;
            return listView;
        }

        public static LinearLayout.LayoutParams MatchParentWrapContent(this View view)
        {
            return new LinearLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent,
                ViewGroup.LayoutParams.WrapContent);
        }

        public static ListView Refresh(this ListView listView,
            IList<object> newItemList,
            OutputFieldMetadata outputField,
            MyFormHandler myFormHandler,
            Dictionary<string, FormMetadata> allFormsMetadata)
        {
            var outputFieldProperty = outputField.CustomProperties.GetCustomProperty<IEnumerable<OutputFieldMetadata>>("columns");
            var adapter = new ListCustomAdapter<object>(newItemList.ToList(), outputFieldProperty, myFormHandler);
            listView.Adapter = adapter;
            return listView;
        }

        public static int ScreenHeightDp(this Context context)
        {
            var metrics = Application.Context.Resources.DisplayMetrics;
            var heightInDp = metrics.HeightPixels.ConvertPixelsToDp();
            return heightInDp.ConvertPixelsToDp();
        }

        public static int ScreenWidthDp(this Context context)
        {
            var metrics = Application.Context.Resources.DisplayMetrics;
            var widthInDp = metrics.WidthPixels.ConvertPixelsToDp();
            return widthInDp.ConvertPixelsToDp();
        }
    }
}