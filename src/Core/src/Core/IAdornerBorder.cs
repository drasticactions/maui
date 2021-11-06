using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui
{
	public interface IAdornerBorder : IDrawable
	{
		IView VisualView { get; }

		Rectangle Offset { get; }
	}
}
