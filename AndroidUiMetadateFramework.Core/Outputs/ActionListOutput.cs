namespace AndroidUiMetadateFramework.Core.Outputs
{
	using System.Collections.Generic;
	using Android.App;
	using Android.Graphics;
	using Android.Views;
	using Android.Widget;
	using AndroidUiMetadateFramework.Core.Attributes;
	using AndroidUiMetadateFramework.Core.Managers;
	using AndroidUiMetadateFramework.Core.Models;
	using UiMetadataFramework.Basic.Output;
	using UiMetadataFramework.Core;

	[Output(Type = "action-list")]
	public class ActionListOutput : IOutputManager
	{
		private LinearLayout OutputView { get; set; }

		public View GetView(OutputFieldMetadata outputField,
			object value,
			MyFormHandler myFormHandler,
			FormMetadata formMetadata,
			List<FormInputManager> inputsManager)
		{
			this.OutputView = new LinearLayout(Application.Context) { Orientation = Orientation.Vertical };
			var actions = value.CastTObject<ActionList>();

			foreach (var btn in actions.Actions)
			{
				var button = this.InitializeActionButton(btn, myFormHandler);
				this.OutputView.AddView(button, this.OutputView.MatchParentWrapContent());
			}

			return this.OutputView;
		}

		public Button InitializeActionButton(FormLink btn, MyFormHandler myFormHandler)
		{
			var button = new Button(Application.Context) { Text = btn.Label };
			button.SetAllCaps(false);
			button.Click += async (sender, args) =>
			{
				var linearLayout = new LinearLayout(Application.Context)
				{
					Orientation = Orientation.Vertical
				};
				View layout;
				if (myFormHandler.AllFormsMetadata != null)
				{
					layout = myFormHandler.GetIForm(myFormHandler.AllFormsMetadata[btn.Form], btn.InputFieldValues);
				}
				else
				{
					layout = await myFormHandler.GetIFormAsync(btn.Form, btn.InputFieldValues);
				}
				if (layout != null)
				{
					linearLayout.SetBackgroundColor(Color.Black);
					var param = linearLayout.MatchParentWrapContent();
					var closeBtn = new Button(Application.Context) { Text = "Close" };
					closeBtn.SetBackgroundColor(Color.Black);
					linearLayout.AddView(layout, param);
					param.Weight = 1;
					linearLayout.AddView(closeBtn, param);
					var metrics = Application.Context.Resources.DisplayMetrics;
					PopupWindow popup = new PopupWindow(linearLayout, metrics.WidthPixels - 40, 3 * metrics.HeightPixels / 4);
					popup.ShowAtLocation(button, GravityFlags.Center, 5, 0);
					closeBtn.Click += (o, eventArgs) => { popup.Dismiss(); };
					popup.Focusable = true;
					popup.Update();
				}
			};
			return button;
		}
	}
}