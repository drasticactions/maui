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
			return view.Frame;
		}
	}
}
