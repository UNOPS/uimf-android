namespace AndroidApp.Forms.Inputs
{
    using Android.App;
    using Android.OS;
    using Android.Support.V4.App;

    [Activity(Label = "FileUploaderActivity")]
    public class FileUploaderActivity : FragmentActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            this.SetContentView(Resource.Layout.FileChooser);
            // Create your application here
        }
    }
}