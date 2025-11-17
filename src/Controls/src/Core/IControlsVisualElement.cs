#nullable enable
using System;

namespace Microsoft.Maui.Controls
{
	public interface IControlsVisualElement : IControlsElement, IView
	{
		event EventHandler? WindowChanged;
		Window? Window { get; }
		event EventHandler? PlatformContainerViewChanged;
	}
}
