﻿using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Views;
using AndroidX.CoordinatorLayout.Widget;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Graphics.Native;

namespace Microsoft.Maui
{
	/// <summary>
	/// Visual Diagnostics Layer.
	/// </summary>
	public partial class VisualDiagnosticsLayer : IVisualDiagnosticsLayer, IDrawable
	{
		Activity? _nativeActivity;

		/// <inheritdoc/>
		public bool DisableUITouchEventPassthrough { get; set; }

		/// <inheritdoc/>
		public NativeGraphicsView? VisualDiagnosticsGraphicsView { get; internal set; }

		/// <inheritdoc/>
		public HashSet<Tuple<IScrollView, Android.Views.View>> ScrollViews { get; } = new HashSet<Tuple<IScrollView, View>>();

		public void AddScrollableElementHandler(IScrollView scrollBar)
		{
			var nativeScroll = scrollBar.GetNative(true);
			if (nativeScroll != null)
			{
				nativeScroll.ScrollChange += Scroll_ScrollChange;
				this.ScrollViews.Add(new Tuple<IScrollView, View>(scrollBar, nativeScroll));
			}
		}

		/// <inheritdoc/>
		public void RemoveScrollableElementHandler()
		{
			foreach (var scrollBar in this.ScrollViews)
			{
				if (!scrollBar.Item2.IsDisposed())
					scrollBar.Item2.ScrollChange -= Scroll_ScrollChange;
			}

			this.ScrollViews.Clear();
		}

		/// <inheritdoc/>
		public void InitializeNativeLayer(IMauiContext context, ViewGroup nativeLayer)
		{
			if (nativeLayer == null || nativeLayer.Context == null)
				return;

			if (nativeLayer.Context is Activity activity)
				_nativeActivity = activity;

			if (_nativeActivity == null || _nativeActivity.WindowManager == null || _nativeActivity.WindowManager.DefaultDisplay == null)
				return;

			var measuredHeight = nativeLayer.MeasuredHeight;

			if (_nativeActivity.Window != null)
				_nativeActivity.Window.DecorView.LayoutChange += DecorView_LayoutChange;

			if (_nativeActivity != null && _nativeActivity.Resources != null && _nativeActivity.Resources.DisplayMetrics != null)
				this.DPI = _nativeActivity.Resources.DisplayMetrics.Density;

			this.VisualDiagnosticsGraphicsView = new NativeGraphicsView(nativeLayer.Context, this);
			if (this.VisualDiagnosticsGraphicsView == null)
				return;

			this.VisualDiagnosticsGraphicsView.Touch += TouchLayer_Touch;
			nativeLayer.AddView(this.VisualDiagnosticsGraphicsView, 0, new CoordinatorLayout.LayoutParams(CoordinatorLayout.LayoutParams.MatchParent, CoordinatorLayout.LayoutParams.MatchParent));
			this.VisualDiagnosticsGraphicsView.BringToFront();
			this.IsNativeViewInitialized = true;
		}

		/// <inheritdoc/>
		public void Invalidate()
		{
			this.VisualDiagnosticsGraphicsView?.Invalidate();
		}

		private void Scroll_ScrollChange(object? sender, View.ScrollChangeEventArgs e)
		{
			this.Invalidate();
		}

		/// <summary>
		/// Generates the Adorner Offset.
		/// </summary>
		/// <param name="nativeActivity">Android Activity, <see cref="Activity"/>.</param>
		/// <param name="graphicsView"><see cref="NativeGraphicsView"/>.</param>
		/// <returns>Offset Rectangle.</returns>
		private Point GenerateAdornerOffset(Activity nativeActivity, NativeGraphicsView graphicsView)
		{
			if (nativeActivity.Resources == null || nativeActivity.Resources.DisplayMetrics == null)
				return new Point();

			if (graphicsView == null)
				return new Point();

			float dpi = nativeActivity.Resources.DisplayMetrics.Density;
			float heightPixels = nativeActivity.Resources.DisplayMetrics.HeightPixels;

			return new Point(0, -(heightPixels - graphicsView.MeasuredHeight) / dpi);
		}

		private void TouchLayer_Touch(object? sender, View.TouchEventArgs e)
		{
			if (e == null || e.Event == null)
				return;

			e.Handled = this.DisableUITouchEventPassthrough;
			var point = new Point(e.Event.RawX, e.Event.RawY);
			OnTouchInternal(point, true);
		}

		private void DecorView_LayoutChange(object? sender, View.LayoutChangeEventArgs e)
		{
			if (this.AdornerBorders.Any())
				this.RemoveAdorners();

			if (this.VisualDiagnosticsGraphicsView != null && this._nativeActivity != null)
				this.Offset = GenerateAdornerOffset(this._nativeActivity, this.VisualDiagnosticsGraphicsView);

			this.Invalidate();
		}
	}
}
