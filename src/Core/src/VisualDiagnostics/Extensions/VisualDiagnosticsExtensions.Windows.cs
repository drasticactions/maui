﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;

namespace Microsoft.Maui
{
	public static class VisualDiagnosticsWindowsExtensions
	{
		public static ViewTransform? GetViewTransform(this IView view)
		{
			var nativeView = view.GetNative(true);
			if (nativeView == null)
				return null;
			return GetViewTransform(nativeView);
		}

		public static ViewTransform? GetViewTransform(FrameworkElement element)
		{
			var root = element.XamlRoot;
			var offset = element.TransformToVisual(root.Content) as MatrixTransform;
			if (offset == null)
				return null;
			Matrix matrix = offset.Matrix;
			return new ViewTransform()
			{
				M11 = matrix.M11,
				M12 = matrix.M12,
				M21 = matrix.M21,
				M22 = matrix.M22,
				OffsetX = matrix.OffsetX,
				OffsetY = matrix.OffsetY,
			};
		}

		public static async Task<byte[]?> RenderAsPng(this IView view)
		{
			var nativeView = view.GetNative(true);
			if (nativeView == null)
				return null;

			if (nativeView is not UIElement uiElement)
				return null;

			try
			{
				// HACK: Should be set to actual DPI.
				double logicalDpi = 1;
				var bitmap = new RenderTargetBitmap();
				await bitmap.RenderAsync(uiElement);
				var pixelBuffer = await bitmap.GetPixelsAsync();
				using var memoryStream = new InMemoryRandomAccessStream();
				BitmapEncoder enc = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, memoryStream);
				enc.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied, (uint)bitmap.PixelWidth, (uint)bitmap.PixelHeight, logicalDpi, logicalDpi, pixelBuffer.ToArray());
				await enc.FlushAsync();
				await memoryStream.FlushAsync();
				var stream = memoryStream.AsStream();
				byte[] result = new byte[stream.Length];
				stream.Read(result, 0, result.Length);
				return result;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
