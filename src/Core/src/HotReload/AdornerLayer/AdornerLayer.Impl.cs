using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui
{
#if ANDROID
	public partial class AdornerLayer : IAdornerLayer
	{
		IWindow window { get; }

		public IAdornerBorder AdornerBorder { get; }

		public ICanvas? Canvas { get; set; }

		public AdornerLayer (IWindow window)
		{
			this.window = window;
			this.AdornerBorder = new AdornerBorder();
		}

		public bool ScrollToView(IView view)
		{
			if (view == null || view.Parent == null)
				return false;

			if (this.IsViewVisible(view))
				return false;

			var visualElement = view as IVisualTreeElement;
			if (visualElement != null)
			{
				var scrollView = ParentScrollView(visualElement);
				if (scrollView != null)
				{
					var nativeView = this.AdornerBorder.GetNativeViewBounds(view);
					scrollView.RequestScrollTo(nativeView.X, nativeView.Y, true);
					return true;
				}
			}

			return false;
		}

		private IScrollView? ParentScrollView(IVisualTreeElement element)
		{
			if (element is IScrollView scrollView)
				return scrollView;
			if (element == null)
				return null;
			var parent = element.GetVisualParent();
			if (parent != null) 
				return ParentScrollView(parent);
			return null;
		}

		public void AddAdorner(IVisualTreeElement visualElement)
		{

			if (visualElement is IView view)
			{
				if (this.AdornerBorder.VisualView == view)
					return;
				this.ScrollToView(view);
				this.AdornerBorder.VisualView = view;
			}

			this.Invalidate();
		}

		public void RemoveAdorner()
		{
			this.AdornerBorder.VisualView = null;
			this.Invalidate();
		}

		public IList<IVisualTreeElement> GetVisualTreeElements(float x, float y, IList<IVisualTreeElement>? elements)
			=> GetVisualTreeElements(new Point(x, y), elements);

		public IList<IVisualTreeElement> GetVisualTreeElements(Point point, IList<IVisualTreeElement>? elements)
		{
			if (elements == null)
				elements = new List<IVisualTreeElement>();

			if (this.window is not IVisualTreeElement element)
				return elements;

			return GetVisualTreeElements(element, point, this.AdornerBorder.GetNativeViewBounds, elements);
		}

		public void SendTouchEvent(Point point)
		{
			throw new NotImplementedException();
		}

		public IList<IVisualTreeElement> GetVisualTreeElements(IVisualTreeElement visualElement, Point point, Func<IView, Rectangle> getNativeViewBounds, IList<IVisualTreeElement>? elements = null)
		{
			if (elements == null)
				elements = new List<IVisualTreeElement>();

			if (visualElement is IView view)
			{
				var bounds = getNativeViewBounds(view);
				if (bounds.Contains(point))
				{
					elements.Add(visualElement);
				}
			}

			var children = visualElement.GetVisualChildren();

			if (!children.Any())
				return elements;

			foreach (var child in children)
			{
				GetVisualTreeElements(child, point, getNativeViewBounds, elements);
			}

			return elements.Reverse().ToList();
		}

		public bool IsLayerEnabled { get; set; } = false;


#pragma warning disable CS0067 // The event is never used
		public event EventHandler<AdornerHitEvent>? OnTouch;
#pragma warning restore CS0067
	}
#else
	public partial class AdornerLayer : IAdornerLayer
	{
		IWindow window { get; }

		public AdornerLayer(IWindow window)
		{
			this.window = window;
			this.AdornerBorder = new AdornerBorder();
		}

		public bool IsLayerEnabled { get; set; }

		public IAdornerBorder AdornerBorder { get; }


#pragma warning disable CS0067 // The event is never used
		public event EventHandler<AdornerHitEvent>? OnTouch;
#pragma warning restore CS0067

		public void InitializeNativeLayer(IMauiContext context, object nativeLayer)
		{
			throw new NotImplementedException();
		}

		public void AddAdorner(IVisualTreeElement visualElement)
		{
		}

		public void RemoveAdorner()
		{
		}

		public void Invalidate()
		{
		}

		public bool ScrollToView(IView view)
		{
			return false;
		}

		public IList<IVisualTreeElement> GetVisualTreeElements(float x, float y, IList<IVisualTreeElement>? elements)
			=> GetVisualTreeElements(new Point(x, y), elements);

		public IList<IVisualTreeElement> GetVisualTreeElements(IVisualTreeElement visualElement, Point point, Func<IView, Rectangle> getNativeViewBounds, IList<IVisualTreeElement>? elements = null)
		{
			return new List<IVisualTreeElement>();
		}

		public bool IsViewVisible(IView view)
		{
			throw new NotImplementedException();
		}

		public IList<IVisualTreeElement> GetVisualTreeElements(Point point, IList<IVisualTreeElement>? elements)
		{
			throw new NotImplementedException();
		}

		public void SendTouchEvent(Point point)
		{
			throw new NotImplementedException();
		}
	}

#endif
}
