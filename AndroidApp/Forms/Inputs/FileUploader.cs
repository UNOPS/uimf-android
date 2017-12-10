namespace AndroidApp.Forms.Inputs
{
    using System.Collections.Generic;
    using Android.App;
    using Android.Content;
    using Android.Views;
    using Android.Widget;
    using AndroidUiMetadataFramework.Core.Attributes;
    using AndroidUiMetadataFramework.Core.Managers;
    using AndroidUiMetadataFramework.Core.Models;

    [Input(Type = "file-uploader")]
    public class FileUploader : IInputManager
    {
        private LinearLayout Layout { get; set; }

        public View GetView(IDictionary<string, object> inputCustomProperties, MyFormHandler myFormHandler)
        {
            this.Layout = new LinearLayout(Application.Context)
            {
                Orientation = Orientation.Horizontal
            };
            var button = new Button(Application.Context)
            {
                Text = "Choose file"
            };
            button.SetAllCaps(false);
            button.Click += (sender, args) =>
            {
                var intent = new Intent(Intent.ActionGetContent);
                intent.SetType("*/*");
                intent.AddCategory(Intent.CategoryOpenable);
                var receiver = new Intent(Application.Context, typeof(BroadcastTest));

                var pendingIntent = PendingIntent.GetBroadcast(Application.Context, 0, receiver, PendingIntentFlags.UpdateCurrent);
                try
                {
                    //Application.Context.StartActivityForResult(Intent.CreateChooser(intent, "Select a file", pendingIntent.IntentSender), 0);
                }
                catch (System.Exception exAct)
                {
                    System.Diagnostics.Debug.Write(exAct);
                }

                //var intent = new Intent(Application.Context, typeof(FileUploaderActivity));
                //intent.SetFlags(ActivityFlags.NewTask);
                //Application.Context.StartActivity(intent);
            };
            this.Layout.AddView(button);
            return this.Layout;
        }

        public object GetValue()
        {
            return null;
        }

        public void SetValue(object value)
        {
        }
    }

    [BroadcastReceiver]
    public class BroadcastTest : BroadcastReceiver
    {
        public override void OnReceive(Context context, Intent intent)
        {
            var message = intent.GetStringExtra("message");
            var title = intent.GetStringExtra("title");

            var resultIntent = new Intent(context, typeof(MainActivity));
            resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);

            var pending = PendingIntent.GetActivity(context, 0,
                resultIntent,
                PendingIntentFlags.CancelCurrent);

            var builder =
                new Notification.Builder(context)
                    .SetContentTitle(title)
                    .SetContentText(message)
                    .SetSmallIcon(Resource.Drawable.Icon)
                    .SetDefaults(NotificationDefaults.All);

            builder.SetContentIntent(pending);

            var notification = builder.Build();

            var manager = NotificationManager.FromContext(context);
            manager.Notify(1337, notification);
        }
    }
}