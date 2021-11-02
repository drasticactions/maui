using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui.Controls
{
#if __ANDROID__
	public partial class AdornerLayer : IAdornerLayer
	{
		private AdornerBorder _border { get; set; }

		public bool IsLayerEnabled { get; set; } = true;

		public ICanvas Canvas { get; set; }

		public IWindow Window { get; set; }

#pragma warning disable CS0067 // The event is never used
		public event EventHandler<AdornerHitEvent> OnTouch;
#pragma warning restore CS0067

		private void GetVisualTreeElements(IVisualTreeElement visualElement, Point point, Func<IView, Rectangle> getNativeViewBounds, IList<IVisualTreeElement> elements)
		{
			if (visualElement is IView view)
			{
				var bounds = getNativeViewBounds(view);
				if (!bounds.Contains(point))
				{
					return;
				}
				
				elements.Add(visualElement);
			}

			var children = visualElement.GetVisualChildren();
			foreach (var child in children)
			{
				GetVisualTreeElements(child, point, getNativeViewBounds, elements);
			}
		}
	}
#else

	public partial class AdornerLayer : IAdornerLayer
	{
		public bool IsLayerEnabled { get; set; }
		public ICanvas Canvas { get; set; }

#pragma warning disable CS0067 // The event is never used
		public event EventHandler<AdornerHitEvent> OnTouch;
#pragma warning restore CS0067

		public IWindow Window { get; set; }
		public AdornerLayer(IWindow window)
		{
			this.Window = window;
			//this.Drawable = _border = new AdornerBorder();
			this.OnTouch += AdornerLayer_OnTouch;	
		}

		private void AdornerLayer_OnTouch(object sender, AdornerHitEvent e)
		{
			throw new NotImplementedException();
		}

		public void AddAdorner(IVisualTreeElement visualElement)
		{
			throw new NotImplementedException();
		}


		public void RemoveAdorner()
		{
			throw new NotImplementedException();
		}

		public void InitializeNativeLayer(IMauiContext context, object nativeLayer)
		{
			throw new NotImplementedException();
		}


		private void GetVisualTreeElements(IVisualTreeElement visualElement, Point point, Func<IElement, Rectangle> getNativeViewBounds, IList<IVisualTreeElement> elements)
		{
			throw new NotImplementedException();
		}
	}
#endif
}
