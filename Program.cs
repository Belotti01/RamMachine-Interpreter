using System.Diagnostics;
using MudBlazor.Services;

namespace RamMachineInterpreter;

public class Program {

	public static void Main(string[] args)
	{
		Debug.WriteLine("> Configuring the application builder...");
		var builder = WebApplication.CreateBuilder(args);

		// Add services to the container.
		builder.Services.AddRazorPages();
		builder.Services.AddServerSideBlazor();
		builder.Services.AddMudServices();
		Debug.WriteLine("> Building the application...");
		var app = builder.Build();

		Debug.WriteLine("> Configuring the application...");
		// Configure the HTTP request pipeline.
		if(!app.Environment.IsDevelopment())
		{
			app.UseExceptionHandler("/Error");
			// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
			app.UseHsts();
		}

		app.UseHttpsRedirection();

		app.UseStaticFiles();

		app.UseRouting();

		app.MapBlazorHub();
		app.MapFallbackToPage("/_Host");

		Debug.WriteLine("> Starting the application...");
		app.Run();
	}
}