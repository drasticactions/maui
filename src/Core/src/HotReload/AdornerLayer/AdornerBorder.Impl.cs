using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui
{
	public partial class AdornerBorder : IAdornerBorder
	{
		public float DPI { get; set; } = 1;

		public Rectangle Offset { get; set; } = new Rectangle();

		public Color Color { get; set; } = Color.FromRgba(225, 0, 0, 125);

		public IView? VisualView { get; set; }

		public void Draw(ICanvas canvas, RectangleF dirtyRect)
		{
			if (this.VisualView == null)
				return;

			if (this.DPI <= 0)
				this.DPI = 1;

			canvas.FillColor = Color;
			canvas.StrokeColor = Color;

			var rect = this.GetNativeViewBounds(this.VisualView);
			var x = (rect.X / this.DPI) + (Offset.X);
			var y = (rect.Y / this.DPI) + (Offset.Y);
			var width = (rect.Width / this.DPI) + Offset.Width;
			var height = (rect.Height / this.DPI) + Offset.Height;
			canvas.FillRectangle(new Rectangle(x, y, width, height));
			canvas.DrawLine(0, (float)y, (float)5000, (float)y);
			canvas.DrawLine(0, (float)(y + height), (float)5000, (float)(y + height));
			canvas.DrawLine((float)x, 0, (float)x, (float)5000);
			canvas.DrawLine((float)(x + width), 0, (float)(x + width), (float)5000);
		}

#if NETSTANDARD
		public Rectangle GetNativeViewBounds(IView view)
		{
			System.Diagnostics.Debug.WriteLine("Not native view, using IView frame bounds");
			return view.Frame;
		}
#endif
	}
}
