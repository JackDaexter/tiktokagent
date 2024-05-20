using Material.Components.Maui.Extensions;
using Microsoft.Extensions.Logging;
using tiktokagent.ViewModel;

namespace tiktokagent;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        Environment.SetEnvironmentVariable("Type","Development");
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.Services.AddSingleton<MainPageVm>();
        builder.Services.AddTransient<MainPage>();
        builder
            .UseMaterialComponents();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

}