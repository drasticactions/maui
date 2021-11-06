using System;
using System.Collections.Generic;
using Microsoft.Maui.Graphics;

namespace Microsoft.Maui
{
	public interface IAdornerLayer : IDrawable
	{
		/// <summary>
		/// Gets or sets indicating whether to not pass UI taps from the touch layer to the underlying app.
		/// Enable this setting if you want to disable user input, but still be able to allow them to tap to get point events.
		/// This could be used to allow a user to tap on screen, get the live visual tree from where they tapped, and draw Adorners.
		/// </summary>
		bool BlockUITapsFromTouchLayer { get; set; }

		/// <summary>
		/// Gets the default adorner border. Used when a user passed in a <see cref="IVisualTreeElement"/> without a <see cref="IAdornerBorder"/>.
		/// </summary>
		IAdornerBorder DefaultAdornerBorder { get; }

		/// <summary>
		/// Touch event for all taps processed on the adorner layer touch surface.
		/// </summary>
		event EventHandler<Point> OnTouch;

		/// <summary>
		/// Adds a new adorner to the adorner layer.
		/// </summary>
		/// <param name="adornerBorder"><see cref="IAdornerBorder"/>.</param>
		void AddAdorner(IAdornerBorder adornerBorder);

		/// <summary>
		/// Adds a new adorner to the Adorner Layer. Uses the default adorner border for drawing.
		/// </summary>
		/// <param name="visualElement"><see cref="IVisualTreeElement"/>.</param>
		void AddAdorner(IVisualTreeElement visualElement);

		/// <summary>
		/// Removes adorner from adorner layer.
		/// </summary>
		/// <param name="adornerBorder"><see cref="IAdornerBorder"/>.</param>
		void RemoveAdorner(IAdornerBorder adornerBorder);

		/// <summary>
		/// Removes all adorners from the adorner layer.
		/// </summary>
		void RemoveAdorners();

		/// <summary>
		/// Removes all adorners containing the inner <see cref="IVisualTreeElement"/>.
		/// </summary>
		/// <param name="visualElement"><see cref="IVisualTreeElement"/>.</param>
		void RemoveAdorners(IVisualTreeElement visualElement);

		/// <summary>
		/// Gets the native view bounds of a given <see cref="IView"/>.
		/// </summary>
		/// <param name="view"><see cref="IView"/></param>
		/// <returns><see cref="Rectangle"/> of the native bounds of the view.</returns>
		Rectangle GetNativeViewBounds(IView view);

		/// <summary>
		/// Gets a list of visual tree elements for a given point.
		/// </summary>
		/// <param name="point"><see cref="Point"/>.</param>
		/// <param name="elements">Optional list of existing <see cref="IVisualTreeElement"/>.</param>
		/// <returns>IList of <see cref="IVisualTreeElement"/>.</returns>
		IList<IVisualTreeElement> GetVisualTreeElements(Point point, IList<IVisualTreeElement>? elements);

		/// <summary>
		/// Gets a list of visual tree elements for a given x, y point.
		/// </summary>
		/// <param name="x">The X Value.</param>
		/// <param name="y">The Y Value.</param>
		/// <param name="elements">Optional list of existing <see cref="IVisualTreeElement"/>.</param>
		/// <returns>IList of <see cref="IVisualTreeElement"/>.</returns>
		IList<IVisualTreeElement> GetVisualTreeElements(double x, double y, IList<IVisualTreeElement>? elements);

		/// <summary>
		/// Gets a list of underlying Visual Tree Elements based off of an existing IVisualTreeElement and Point.
		/// </summary>
		/// <param name="visualElement"><see cref="IVisualTreeElement"/>.</param>
		/// <param name="point"><see cref="Point"/>.</param>
		/// <param name="getNativeViewBounds">Function for calculating native view bounds. Can use <see cref="GetNativeViewBounds(IView)"/> or override with your own.</param>
		/// <param name="elements">Optional list of existing <see cref="IVisualTreeElement"/>.</param>
		/// <returns>IList of <see cref="IVisualTreeElement"/>.</returns>
		IList<IVisualTreeElement> GetVisualTreeElements(IVisualTreeElement visualElement, Point point, Func<IView, Rectangle> getNativeViewBounds, IList<IVisualTreeElement>? elements);
	}
}
