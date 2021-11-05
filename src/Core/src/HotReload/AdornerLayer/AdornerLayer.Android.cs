using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Views;
using AndroidX.CoordinatorLayout.Widget;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Native;

namespace Microsoft.Maui
{
	public partial class AdornerLayer : IAdornerLayer
	{
		Android.Util.DisplayMetrics? metric;
		NativeGraphicsView? touchLayer;

		public void InitializeNativeLayer(IMauiContext context, object nativeLayer)
		{
			var nativeView = nativeLayer as ViewGroup;
			if (nativeView == null)
			{
				System.Diagnostics.Debug.WriteLine("AdornerLayer: Could not initialize native Android view as root layout.");
				return;
			}

			var nativeActivity = nativeView.Context as Activity;

			if (nativeActivity == null || nativeActivity.WindowManager == null || nativeActivity.WindowManager.DefaultDisplay == null)
			{
				System.Diagnostics.Debug.WriteLine("AdornerLayer: Could not cast native Android activity.");
				return;
			}

			var measuredHeight = nativeView.MeasuredHeight;

			if (nativeActivity.Window != null)
			{
				nativeActivity.Window.DecorView.LayoutChange += DecorView_LayoutChange;
			}


			this.metric = new Android.Util.DisplayMetrics();
#pragma warning disable CS0618 // Type or member is obsolete
			nativeActivity.WindowManager.DefaultDisplay.GetMetrics(metric);
			this.AdornerBorder.DPI = this.metric.Density;
#pragma warning restore CS0618 // Type or member is obsolete
			touchLayer = new NativeGraphicsView(nativeView.Context, this.AdornerBorder);
			if (touchLayer == null)
			{
				System.Diagnostics.Debug.WriteLine("AdornerLayer: Could not set up touch layer canvas.");
				return;
			}

			touchLayer.Touch += TouchLayer_Touch;
			nativeView.AddView(touchLayer, 0, new CoordinatorLayout.LayoutParams(CoordinatorLayout.LayoutParams.MatchParent, CoordinatorLayout.LayoutParams.MatchParent));
			touchLayer.BringToFront();
		}

		private void HandleScrolls()
		{
			if (this.window == null)
				return;
			var content = this.window.Content as IVisualTreeElement;
			if (content == null)
				return;
			var scrolls = GetScrollBars(content);
			foreach(var scroll in scrolls)
			{
				var realScroll = scroll as IScrollView;
				if (realScroll != null)
				{
					var realerscroll = realScroll.GetNative(true);
					if (realerscroll != null)
					realerscroll.ScrollChange += Realerscroll_ScrollChange;
				}
			}
		}

		private IList<IScrollView> GetScrollBars(IVisualTreeElement element, IList<IScrollView>? scrollViews = null)
		{
			if (scrollViews == null)
				scrollViews = new List<IScrollView>();
			if (element is IScrollView scroll)
				scrollViews.Add(scroll);
			var children = element.GetVisualChildren();
			foreach(var child in children)
			{
				GetScrollBars(child, scrollViews);
			}

			return scrollViews;
		}

		private void Realerscroll_ScrollChange(object? sender, View.ScrollChangeEventArgs e)
		{
			this.touchLayer?.Invalidate();
		}

		private void DecorView_LayoutChange(object? sender, View.LayoutChangeEventArgs e)
		{
			if (e == null)
				return;
			if (this.metric != null && this.touchLayer != null)
				this.AdornerBorder.Offset = new Rectangle(0, -(this.metric.HeightPixels - touchLayer.MeasuredHeight) / this.AdornerBorder.DPI, 0, 0);
			HandleScrolls();
			this.touchLayer?.Invalidate();
		}

		public void Invalidate()
		{
			this.touchLayer?.Invalidate();
		}

		public bool IsViewVisible(IView view)
		{
			var native = view.GetNative(true);
			if (native == null)
				return true;
			return native.GetGlobalVisibleRect(new Android.Graphics.Rect());
		}

		private void TouchLayer_Touch(object? sender, Android.Views.View.TouchEventArgs? e)
		{
			if (e == null || e.Event == null)
				return;

			System.Diagnostics.Debug.WriteLine(e.Event.ToString());
			this.touchLayer?.Invalidate();

			if (window == null || window is not IVisualTreeElement rootWindow || !this.IsLayerEnabled)
			{
				e.Handled = false;
				return;
			}

			var point = new Graphics.Point(e.Event.RawX, e.Event.RawY);

			var lvt = this.GetVisualTreeElements(rootWindow, point, this.AdornerBorder.GetNativeViewBounds);
			this.OnTouch?.Invoke(this, new AdornerHitEvent(point, lvt));
			if (lvt.Any())
				this.AddAdorner(lvt.First());
			e.Handled = true;
		}
	}
}
