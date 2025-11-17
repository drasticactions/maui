#nullable enable
using System;

namespace Microsoft.Maui.Controls
{
	public interface IControlsElement : Maui.IElement
	{
		event EventHandler<HandlerChangingEventArgs>? HandlerChanging;
		event EventHandler? HandlerChanged;
	}
}
