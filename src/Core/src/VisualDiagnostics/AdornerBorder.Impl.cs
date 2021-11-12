﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui
{
	/// <summary>
	/// Adorner Border.
	/// Used to set up the initial Androer Border drawable.
	/// By itself, it does nothing. Implement on top of it to draw shapes.
	/// </summary>
	public partial class AdornerBorder : IAdornerBorder, IDrawable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="AdornerBorder"/> class.
		/// </summary>
		/// <param name="view">An <see cref="IView"/> to create the Adorner Border around.</param>
		/// <param name="dpi">Override DPI setting. Default: 1</param>
		/// <param name="offset">Offset point used for positioning drawable object. Default: null</param>
		/// <param name="fillColor">Canvas Fill Color.</param>
		/// <param name="strokeColor">Canvas Stroke Color.</param>
		public AdornerBorder(IView view, float dpi = 1, Point? offset = null, Color? fillColor = null, Color? strokeColor = null)
		{
			if (fillColor != null)
				this.FillColor = fillColor;

			if (strokeColor != null)
				this.StrokeColor = strokeColor;

			if (offset == null)
				this.Offset = new Point();
			else
				this.Offset = offset.Value;

			this.VisualView = view;
			this.DPI = dpi;
		}

		/// <inheritdoc/>
		public virtual void Draw(ICanvas canvas, RectangleF dirtyRect)
		{
			canvas.FillColor = this.FillColor;
			canvas.StrokeColor = this.StrokeColor;
		}

		/// <inheritdoc/>
		public float DPI { get; }

		/// <inheritdoc/>
		public IView VisualView { get; }

		/// <inheritdoc/>
		public Point Offset { get; }

		/// <inheritdoc/>
		public Color FillColor { get; } = Color.FromRgba(225, 0, 0, 125);

		/// <inheritdoc/>
		public Color StrokeColor { get; } = Color.FromRgba(225, 0, 0, 125);
	}
}
