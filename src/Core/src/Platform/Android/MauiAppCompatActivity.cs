using Android.OS;
using AndroidX.AppCompat.App;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Platform;

namespace Microsoft.Maui
{
	public partial class MauiAppCompatActivity : AppCompatActivity
	{
		// Override this if you want to handle the default Android behavior of restoring fragments on an application restart
		protected virtual bool AllowFragmentRestore => false;

		protected override void OnCreate(Bundle? savedInstanceState)
		{
			if (!AllowFragmentRestore)
			{
				// Remove the automatically persisted fragment structure; we don't need them
				// because we're rebuilding everything from scratch. This saves a bit of memory
				// and prevents loading errors from child fragment managers
				savedInstanceState?.Remove("android:support:fragments");
				savedInstanceState?.Remove("androidx.lifecycle.BundlableSavedStateRegistry.key");
			}

			// If the theme has the maui_splash attribute, change the theme
			if (Theme.TryResolveAttribute(Resource.Attribute.maui_splash))
			{
				SetTheme(Resource.Style.Maui_MainTheme_NoActionBar);
			}

			base.OnCreate(savedInstanceState);

			this.CreateNativeWindow(MauiApplication.Current.Application, savedInstanceState);
		}

		void SendPointToAdornerService(float x, float y)
		{
			var statusBarHeight = GetStatusBarHeight();
			int GetStatusBarHeight()
			{
				int result = 0;
				if (Resources == null)
					return result;
				int resourceId = Resources.GetIdentifier("status_bar_height", "dimen", "android");
				if (resourceId > 0)
				{
					result = Resources.GetDimensionPixelSize(resourceId);
				}
				return result;
			}
			var adornerService = MauiApplication.Current.Services.GetService<IAdornerService>();
			if (adornerService != null)
			{
				var point = new Point(this.FromPixels(x), this.FromPixels(y - statusBarHeight));
				System.Diagnostics.Debug.WriteLine($"touch at point {point.X} {point.Y}");
				(adornerService as AdornerService)?.ExecuteTouchEventDelegate(point);
			}
		}
	}
}