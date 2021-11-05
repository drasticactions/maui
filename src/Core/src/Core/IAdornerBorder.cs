using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui
{
	public interface IAdornerBorder : IDrawable
	{
		float DPI { get; set; }

		IView? VisualView { get; set; }

		Rectangle Offset { get; set; }

		Rectangle GetNativeViewBounds(IView view);
	}
}
