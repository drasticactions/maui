using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Graphics;
using Android.Util;
using Android.Views;
using AndroidX.CoordinatorLayout.Widget;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Native;
using GL = Android.Opengl;

namespace Microsoft.Maui.Controls
{
	public partial class AdornerLayer : IAdornerLayer
	{
		NativeGraphicsView touchLayer;
		Android.Util.DisplayMetrics metric;
		public AdornerLayer(IWindow window)
		{
			this.Window = window;
		}

		public void InitializeNativeLayer(IMauiContext context, object view)
		{
			var nativeView = view as ViewGroup;
			this.Canvas = new NativeCanvas(nativeView.Context);
			//this.Canvas.DisplayScale = 1.5f;
			var act = ((Activity)nativeView.Context);
			this.metric = new Android.Util.DisplayMetrics();
#pragma warning disable CS0618 // Type or member is obsolete
			act.WindowManager.DefaultDisplay.GetMetrics(metric);
#pragma warning restore CS0618 // Type or member is obsolete
			touchLayer = new NativeGraphicsView(nativeView.Context, _border = new AdornerBorder(act.Window));
			touchLayer.Touch += TouchLayer_Touch;
			nativeView.AddView(touchLayer, 0, new CoordinatorLayout.LayoutParams(CoordinatorLayout.LayoutParams.MatchParent, CoordinatorLayout.LayoutParams.MatchParent));
			touchLayer.BringToFront();
		}

		public void AddAdorner(IVisualTreeElement visualElement)
		{
			_border.VisualElement = visualElement;
			this.touchLayer.Invalidate();
		}


		public void RemoveAdorner()
		{
			_border.VisualElement = null;
			this.touchLayer.Invalidate();
		}

		public Rectangle GetNativeViewBounds(IView visualElement)
		{
			var nativeView = visualElement.GetNative(true);

			var location = new int[2];
			nativeView.GetLocationInSurface(location);

			return new Rectangle(
				location[0],
				location[1],
				nativeView.Width,
				nativeView.Height);
		}

		private void TouchLayer_Touch(object sender, Android.Views.View.TouchEventArgs e)
		{
			if (this.IsLayerEnabled)
			{
				var list = new List<IVisualTreeElement>();
				this.GetVisualTreeElements(this.Window as IVisualTreeElement, new Graphics.Point(e.Event.RawX, e.Event.RawY), GetNativeViewBounds, list);
				list.Reverse();
				if (list.Any())
				{
					var item = list.First();
					System.Diagnostics.Debug.WriteLine(item.GetType().ToString());
					this.AddAdorner(list.First());
				}
				this.OnTouch?.Invoke(this, new AdornerHitEvent(new Graphics.Point(e.Event.RawX, e.Event.RawY), list));
				e.Handled = true;
				return;
			}
			e.Handled = false;
		}

		internal class AdornerBorder : IDrawable
		{
			Android.Views.Window androidWindow;

			public IVisualTreeElement VisualElement { get; set; }

			public ICanvas Canvas { get; internal set; }

			public AdornerBorder(Android.Views.Window win)
			{
				this.androidWindow = win;
			}

			Android.Util.DisplayMetrics metric = new Android.Util.DisplayMetrics();

			public Rectangle GetNativeViewBounds(IView visualElement)
			{
				var nativeView = visualElement.GetNative(true);
				var location = new int[2];
				nativeView.GetLocationInWindow(location);
				return new Rectangle(
					location[0],
					location[1],
					nativeView.Context.ToPixels(visualElement.Frame.Width),
					nativeView.Context.ToPixels(visualElement.Frame.Height));
			}

			public Rectangle TestFix(IView visualElement)
			{
				var nativeView = visualElement.GetNative(true);
				var location = new int[2];
				nativeView.GetLocationOnScreen(location);

				var X = location[0];
				// You are the worst, Android. Here we need to convert the location we get to dips,
				// because Xamarin.Forms Bounds are in dips, and then also subtract the status bar,
				// once again converting to dips.
				var yDips = nativeView.Context.FromPixels(location[1]);

				var rect = new Rect();
				androidWindow.DecorView.GetWindowVisibleDisplayFrame(rect);
				var statusBarHeight = rect.Top;
				var statusBarDips = nativeView.Context.FromPixels(statusBarHeight);

				var Y = yDips - statusBarDips;
				return new Rectangle(X, Y, nativeView.Width, nativeView.Height);
			}

			static ViewTransform GetViewTransform(Android.Views.View view)
			{
				if (view == null || view.Matrix.IsIdentity)
					return null;

				var m = new float[16];
				var v = new float[16];
				var r = new float[16];

				GL.Matrix.SetIdentityM(r, 0);
				GL.Matrix.SetIdentityM(v, 0);
				GL.Matrix.SetIdentityM(m, 0);

				GL.Matrix.TranslateM(v, 0, view.Left, view.Top, 0);
				GL.Matrix.TranslateM(v, 0, view.PivotX, view.PivotY, 0);
				GL.Matrix.TranslateM(v, 0, view.TranslationX, view.TranslationY, 0);
				GL.Matrix.ScaleM(v, 0, view.ScaleX, view.ScaleY, 1);
				GL.Matrix.RotateM(v, 0, view.RotationX, 1, 0, 0);
				GL.Matrix.RotateM(v, 0, view.RotationY, 0, 1, 0);
				GL.Matrix.RotateM(m, 0, view.Rotation, 0, 0, 1);

				GL.Matrix.MultiplyMM(r, 0, v, 0, m, 0);
				GL.Matrix.TranslateM(m, 0, r, 0, -view.PivotX, -view.PivotY, 0);

				return new ViewTransform
				{
					M11 = m[0],
					M12 = m[1],
					M13 = m[2],
					M14 = m[3],
					M21 = m[4],
					M22 = m[5],
					M23 = m[6],
					M24 = m[7],
					M31 = m[8],
					M32 = m[9],
					M33 = m[10],
					M34 = m[11],
					OffsetX = m[12],
					OffsetY = m[13],
					OffsetZ = m[14],
					M44 = m[15]
				};
			}

			public void Draw(ICanvas canvas, RectangleF dirtyRect)
			{
				this.Canvas = canvas;
				canvas.FillColor = Microsoft.Maui.Graphics.Color.FromRgba(225, 0, 0, 125);
				canvas.StrokeColor = Microsoft.Maui.Graphics.Color.FromRgba(225, 0, 0, 125);
				if (this.VisualElement != null)
				{
					canvas.FillColor = Microsoft.Maui.Graphics.Color.FromRgba(225, 0, 0, 125);
					var element = this.VisualElement as IView;
					if (element != null)
					{
						var native = element.GetNative(true);
						if (native != null)
						{
							var rect = GetNativeViewBounds(element);

							var x = rect.X / 1.5;
							var y = rect.Y / 1.5;
							var width = rect.Width / 1.5;
							var height = rect.Height / 1.5;
							var test2 = new Rectangle(x, y, width, height);
							canvas.FillRectangle(test2);
							canvas.DrawLine(0, (float)y, (float)5000, (float)y);
							canvas.DrawLine(0, (float)(y + height), (float)5000, (float)(y + height));
							canvas.DrawLine((float)x, 0, (float)x, (float)5000);
							canvas.DrawLine((float)(x + width), 0, (float)(x + width), (float)5000);
						}
					}
				}
			}
		}

		class ViewTransform
		{
			public double M11 = 1;
			public double M12;
			public double M13;
			public double M14;
			public double M21;
			public double M22 = 1;
			public double M23;
			public double M24;
			public double M31;
			public double M32;
			public double M33 = 1;
			public double M34;
			public double OffsetX;
			public double OffsetY;
			public double OffsetZ;
			public double M44 = 1;
		}
	}
}
