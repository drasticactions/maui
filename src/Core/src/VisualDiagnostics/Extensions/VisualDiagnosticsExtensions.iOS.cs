﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using CoreAnimation;
using CoreGraphics;
using ObjCRuntime;
using UIKit;

namespace Microsoft.Maui
{
	public static class CoreAnimationExtensions
	{
		public static CATransform3D Prepend(this CATransform3D a, CATransform3D b) =>
			b.Concat(a);

		public static CATransform3D GetLocalTransform(this CALayer layer)
		{
			return CATransform3D.Identity
				.Translate(
					layer.Position.X,
					layer.Position.Y,
					layer.ZPosition)
				.Prepend(layer.Transform)
				.Translate(
					-layer.AnchorPoint.X * layer.Bounds.Width,
					-layer.AnchorPoint.Y * layer.Bounds.Height,
					-layer.AnchorPointZ);
		}

		public static CATransform3D GetChildTransform(this CALayer layer)
		{
			var childTransform = layer.SublayerTransform;

			if (childTransform.IsIdentity)
				return childTransform;

			return CATransform3D.Identity
				.Translate(
					layer.AnchorPoint.X * layer.Bounds.Width,
					layer.AnchorPoint.Y * layer.Bounds.Height,
					layer.AnchorPointZ)
				.Prepend(childTransform)
				.Translate(
					-layer.AnchorPoint.X * layer.Bounds.Width,
					-layer.AnchorPoint.Y * layer.Bounds.Height,
					-layer.AnchorPointZ);
		}

		public static CATransform3D TransformToAncestor(this CALayer fromLayer, CALayer toLayer)
		{
			var transform = CATransform3D.Identity;

			CALayer? current = fromLayer;
			while (current != toLayer)
			{
				transform = transform.Concat(current.GetLocalTransform());

				current = current.SuperLayer;
				if (current == null)
					break;

				transform = transform.Concat(current.GetChildTransform());
			}
			return transform;
		}

		public static ViewTransform ToViewTransform(this CATransform3D transform) =>
			new ViewTransform
			{
				M11 = transform.m11,
				M12 = transform.m12,
				M13 = transform.m13,
				M14 = transform.m14,
				M21 = transform.m21,
				M22 = transform.m22,
				M23 = transform.m23,
				M24 = transform.m24,
				M31 = transform.m31,
				M32 = transform.m32,
				M33 = transform.m33,
				M34 = transform.m34,
				OffsetX = transform.m41,
				OffsetY = transform.m42,
				OffsetZ = transform.m43,
				M44 = transform.m44
			};
	}

	public static class VisualDiagnosticsiOSExtensions
	{
		[DllImport(Constants.ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
		static extern bool bool_objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);

		[DllImport(Constants.ObjectiveCLibrary, EntryPoint = "objc_msgSend")]
		static extern IntPtr IntPtr_objc_msgSend(IntPtr receiver, IntPtr selector);

		public static Task<byte[]?> RenderAsPng(this IView view)
		{
			var nativeView = view.GetNative(true);
			if (nativeView == null)
				return Task.FromResult<byte[]?>(null);
			var skipChildren = !(view is IView && !(view is ILayout));
			return Task.FromResult(RenderAsPng(nativeView.Window, nativeView.Layer, UIScreen.MainScreen.Scale, skipChildren));
		}

		public static ViewTransform? GetViewTransform(this IView view)
		{
			var nativeView = view.GetNative(true);
			if (nativeView == null)
				return null;
			return GetViewTransform(nativeView.Layer);
		}

		public static ViewTransform? GetViewTransform(CALayer layer)
		{
			if (layer == null)
				return null;

			var superLayer = layer.SuperLayer;
			if (layer.Transform.IsIdentity && (superLayer == null || superLayer.Transform.IsIdentity))
				return null;

			var superTransform = layer.SuperLayer?.GetChildTransform() ?? CATransform3D.Identity;

			return layer.GetLocalTransform()
				.Concat(superTransform)
					.ToViewTransform();
		}

		static byte[]? RenderAsPng(UIWindow window, object obj, nfloat scale, bool skipChildren = true)
		{
			using (var image = Render(window, obj, scale, skipChildren))
				return image != null ? AsPNGBytes(image) : null;
		}

		static byte[]? AsPNGBytes(this UIImage image)
		{
			if (image == null)
				return null;

			var data = image.AsPNG();
			if (data == null)
				return null;

			try
			{
				var result = new byte[data.Length];
				Marshal.Copy(data.Bytes, result, 0, (int)data.Length);
				return result;
			}
			finally
			{
				data.Dispose();
			}
		}


		static UIImage? Render(UIWindow window, object obj, nfloat scale, bool skipChildren = true)
		{
			CGContext? ctx = null;
			Exception? error = null;

			var viewController = obj as UIViewController;
			if (viewController != null)
			{
				// NOTE: We rely on the window frame having been set to the correct size when this method is invoked.
				UIGraphics.BeginImageContextWithOptions(window.Bounds.Size, false, scale);
				ctx = UIGraphics.GetCurrentContext();

				if (!TryRender(window, ctx, ref error))
				{
					//FIXME: test/handle this case
					// Log.Warning(TAG, $"TryRender failed on {window}");
				}

				// Render the status bar with the correct frame size
				UIApplication.SharedApplication.TryHideStatusClockView();
				var statusbarWindow = UIApplication.SharedApplication.GetStatusBarWindow();
				if (statusbarWindow != null/* && metrics.StatusBar != null*/)
				{
					statusbarWindow.Frame = window.Frame;
					statusbarWindow.Layer.RenderInContext(ctx);
				}
			}

			var view = obj as UIView;
			if (view != null)
			{
				UIGraphics.BeginImageContextWithOptions(view.Bounds.Size, false, scale);
				ctx = UIGraphics.GetCurrentContext();
				// ctx will be null if the width/height of the view is zero
				if (ctx != null)
					TryRender(view, ctx, ref error);
			}

			var layer = obj as CALayer;
			if (layer != null)
			{
				UIGraphics.BeginImageContextWithOptions(layer.Bounds.Size, false, scale);
				ctx = UIGraphics.GetCurrentContext();
				if (ctx != null)
					TryRender(layer, ctx, skipChildren, ref error);
			}

			if (ctx == null)
				return null;

			var image = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return image;
		}

		static bool TryRender(UIView view, CGContext ctx, ref Exception? error)
		{
			try
			{
				view.DrawViewHierarchy(view.Bounds, afterScreenUpdates: true);
				return true;
			}
			catch (Exception e)
			{
				error = e;
				return false;
			}
		}

		static bool TryRender(CALayer layer, CGContext ctx, bool skipChildren, ref Exception? error)
		{
			try
			{
				Dictionary<IntPtr, bool>? visibilitySnapshot = null;
				if (skipChildren)
					visibilitySnapshot = GetVisibilitySnapshotAndHideLayers(layer);
				layer.RenderInContext(ctx);
				if (skipChildren)
					ResetLayerVisibilitiesFromSnapshot(layer, visibilitySnapshot);
				return true;
			}
			catch (Exception e)
			{
				error = e;
				return false;
			}
		}

		static Dictionary<IntPtr, bool> GetVisibilitySnapshotAndHideLayers(CALayer layer)
		{
			var visibilitySnapshot = new Dictionary<IntPtr, bool>();
			layer.Sublayers?.ForEach(sublayer => {
				var subSnapshot = GetVisibilitySnapshotAndHideLayers(sublayer);
				foreach (var kvp in subSnapshot)
					visibilitySnapshot.Add(kvp.Key, kvp.Value);
				visibilitySnapshot.Add(sublayer.Handle, sublayer.Hidden);
				sublayer.Hidden = true;
			});
			return visibilitySnapshot;
		}

		static void ResetLayerVisibilitiesFromSnapshot(
			CALayer layer,
			Dictionary<IntPtr, bool>? visibilitySnapshot)
		{
			layer.Sublayers?.ForEach(sublayer => {
				ResetLayerVisibilitiesFromSnapshot(sublayer, visibilitySnapshot);
				if (visibilitySnapshot != null)
					sublayer.Hidden = visibilitySnapshot[sublayer.Handle];
			});
		}

		static void TryHideStatusClockView(this UIApplication app)
		{
			var statusBarWindow = app.GetStatusBarWindow();
			if (statusBarWindow == null)
				return;

			var clockView = statusBarWindow.FindSubview(
				"UIStatusBar",
				"UIStatusBarForegroundView",
				"UIStatusBarTimeItemView"
			);

			if (clockView != null)
				clockView.Hidden = true;
		}

		static UIWindow? GetStatusBarWindow(this UIApplication app)
		{
			if (!bool_objc_msgSend_IntPtr(app.Handle,
				Selectors.respondsToSelector.Handle,
				Selectors.statusBarWindow.Handle))
				return null;

			var ptr = IntPtr_objc_msgSend(app.Handle, Selectors.statusBarWindow.Handle);
			return ptr != IntPtr.Zero ? ObjCRuntime.Runtime.GetNSObject(ptr) as UIWindow : null;
		}

		static class Selectors
		{
			public static readonly Selector statusBarWindow = new Selector("statusBarWindow");
			public static readonly Selector respondsToSelector = new Selector("respondsToSelector:");
		}

		static UIView? FindSubview(this UIView view, params string[] classNames)
		{
			return FindSubview(view, ((IEnumerable<string>)classNames).GetEnumerator());
		}

		static UIView? FindSubview(UIView view, IEnumerator<string> classNames)
		{
			if (!classNames.MoveNext())
				return view;

			foreach (var subview in view.Subviews)
			{
				if (subview.ToString().StartsWith("<" + classNames.Current + ":", StringComparison.Ordinal))
					return FindSubview(subview, classNames);
			}

			return null;
		}
	}
}
