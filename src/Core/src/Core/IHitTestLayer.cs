#nullable enable
using System;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui
{
	public interface IHitTestLayer
	{
		public IWindow Window { get; }

		bool EnableTouchLayer { get; set; }

		bool IsNativeVisualElementContainedInPoint(IVisualTreeElement element, Point point);

		IList<IVisualTreeElement> GetVisualTreeElements(Point point);
	}

	public class WindowHitTestTappedEvent
	{
		public Point Point { get; set; }
	}
}