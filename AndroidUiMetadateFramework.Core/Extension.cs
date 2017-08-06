namespace AndroidUiMetadateFramework.Core
{
	using Android.App;
	using Android.Content;

	public static class Extension
	{
		public static int ConvertPixelsToDp(this int pixelValue)
		{
			var dp = (int)(pixelValue / Application.Context.Resources.DisplayMetrics.Density);
			return dp;
		}

		public static int ScreenWidthDp(this Context context)
		{
			var metrics = Application.Context.Resources.DisplayMetrics;
			var widthInDp = metrics.WidthPixels.ConvertPixelsToDp();
			return widthInDp.ConvertPixelsToDp();
		}

		public static int ScreenHeightDp(this Context context)
		{
			var metrics = Application.Context.Resources.DisplayMetrics;
			var heightInDp = metrics.HeightPixels.ConvertPixelsToDp();
			return heightInDp.ConvertPixelsToDp();
		}
	}
}