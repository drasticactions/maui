using System;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui
{
	public interface IAdornerLayer
	{
		bool IsLayerEnabled { get; set; }

		public ICanvas Canvas { get; set; }

		event EventHandler<AdornerHitEvent> OnTouch;

		void InitializeNativeLayer(IMauiContext context, object nativeLayer);

		void AddAdorner(IVisualTreeElement visualElement);

		void RemoveAdorner();
	}

	public class AdornerHitEvent
	{
		public AdornerHitEvent(Point point, IList<IVisualTreeElement> elements)
		{
			this.Point = point;
			this.VisualTreeElements = elements;
		}

		public IList<IVisualTreeElement> VisualTreeElements { get; }

		public Point Point { get; }
	}
}
