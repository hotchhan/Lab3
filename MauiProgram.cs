/*
* Name: Hannah Hotchkiss, Carissa Engebose
* Description: Lab 2 - App that allows you to input visited Airports to have a record of all visited and when you 
* visited them
* Date: 9/24/2024
* Bugs: None known
* Reflection: This app was a good excerise to understand the basics of creating a .NET MAUI app that includes a user 
* interface and small backend which stores the data the user enters. Overall we think it helped us know more about how to 
* use a UI and communicate with the overall structure of a C# app.
*/
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
