using System;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui
{
	public interface IAdornerLayer
	{
		bool IsLayerEnabled { get; set; }

		event EventHandler<AdornerHitEvent> OnTouch;

		IAdornerBorder AdornerBorder { get; }

		void AddAdorner(IVisualTreeElement visualElement);

		void RemoveAdorner();

		void InitializeNativeLayer(IMauiContext context, object nativeLayer);

		bool ScrollToView(IView view);

		bool IsViewVisible(IView view);

		void Invalidate();

		IList<IVisualTreeElement> GetVisualTreeElements(Point point, IList<IVisualTreeElement>? elements);


		IList<IVisualTreeElement> GetVisualTreeElements(float x, float y, IList<IVisualTreeElement>? elements);

		void SendTouchEvent(Point point);

		IList<IVisualTreeElement> GetVisualTreeElements(IVisualTreeElement visualElement, Point point, Func<IView, Rectangle> getNativeViewBounds, IList<IVisualTreeElement>? elements);
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
