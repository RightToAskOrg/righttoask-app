using CommunityToolkit.Maui;

namespace RightToAskClient;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiCommunityToolkit()
            .UseMauiApp<App>();

        return builder.Build();
    }
}
