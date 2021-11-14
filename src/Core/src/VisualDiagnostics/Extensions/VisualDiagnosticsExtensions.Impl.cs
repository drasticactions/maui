using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Maui
{
	public static class VisualDiagnosticsExtensions
	{
#if NET6 || NETSTANDARD
		public static Task<byte[]?> RenderAsPng(this IView view)
		{
			return Task.FromResult<byte[]?>(null);
		}

		public static ViewTransform? GetViewTransform(this IView view)
		{
			return null;
		}
#endif
	}
}
