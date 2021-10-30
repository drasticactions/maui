using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Maui.Hosting
{
	public static partial class AppHostBuilderExtensions
	{
		public static MauiAppBuilder ConfigureAdorners(this MauiAppBuilder builder)
		{
			builder.Services.TryAddSingleton<IAdornerService, AdornerService>();
			return builder;
		}
	}
}
