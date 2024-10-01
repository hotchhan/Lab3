/**
Name: Hannah Hotchkiss, Carissa Engebose
Date: 10/1/24
Description: Lab 2, but now with a remote database and data persistence
Bugs: None known.
**/

using Microsoft.Extensions.Logging;

namespace Lab3;

public static class MauiProgram
{
	// method creates a MauiApp instance
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular"); // set custom fonts for the app
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

#if DEBUG
		builder.Logging.AddDebug(); // enable logging during the debugging process
#endif

		return builder.Build(); // build and return the MAUI app
	}
}
