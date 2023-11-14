using Microsoft.Maui.Controls.Xaml;

namespace RightToAskClient.Maui.Views.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoPopup : OneButtonPopup
    {
        public InfoPopup([NotNull] string message, [NotNull] string buttonText) : base(message, buttonText, isInfoPopup:true)
        {
        }

        public InfoPopup([NotNull] string title, [NotNull] string message, [NotNull] string buttonText) : base(title, message, buttonText, isInfoPopup:true)
        {
        }
    }
}