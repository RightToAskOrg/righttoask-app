using CommunityToolkit.Maui;
using static RightToAskClient.Maui.Views.Controls.Accessibility;

namespace RightToAskClient.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureEffects(effects =>
                {
                    //effects.Add<>();
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("DancingScript-VariableFont_wght.ttf", "DanceFont");
                    //  fonts.AddFont("Roboto-Black.ttf", "AppFont");
                });
         
            return builder.Build();
        }

    }
}