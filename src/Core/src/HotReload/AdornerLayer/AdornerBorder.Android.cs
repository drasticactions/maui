using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui
{
	public partial class AdornerBorder : IAdornerBorder
	{
		public Rectangle GetNativeViewBounds(IView view)
		{
			var nativeView = view.GetNative(true);
			if (nativeView == null || nativeView.Context == null)
			{
				return new Rectangle();
			}

			var location = new int[2];
			nativeView.GetLocationOnScreen(location);
			return new Rectangle(
				location[0],
				location[1],
				nativeView.Context.ToPixels(view.Frame.Width),
				nativeView.Context.ToPixels(view.Frame.Height));
		}
	}
}
