using Android.Content.Res;
using RightToAskClient.Maui.Resx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RightToAskClient.Maui.Helpers
{
    //https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/label?view=net-maui-8.0#create-a-hyperlink
    public class HyperlinkSpan : Span
    {
        public static readonly BindableProperty UrlProperty =
            BindableProperty.Create(nameof(Url), typeof(string), typeof(HyperlinkSpan), null);

        public string Url
        {
            get { return (string)GetValue(UrlProperty); }
            set { SetValue(UrlProperty, value); }
        }

        public HyperlinkSpan()
        {
            TextDecorations = TextDecorations.Underline;
            TextColor = (Color)Application.Current.Resources["UrlTextColorLightMode"];
            GestureRecognizers.Add(new TapGestureRecognizer
            {
                // Launcher.OpenAsync is provided by Essentials.
                Command = new Command(async () => await OpenUrl(Url))
            });
        }

        //TODO: test on real device
        public async Task OpenUrl(string url)
        {
            await Launcher.OpenAsync(url);
        }
    }
}
