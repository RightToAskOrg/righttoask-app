using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RightToAskClient.Annotations;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace RightToAskClient.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InfoPopup : OneButtonPopup
    {
        public InfoPopup([NotNull] string message, [NotNull] string buttonText) : base(message, buttonText)
        {
        }

        public InfoPopup([NotNull] string title, [NotNull] string message, [NotNull] string buttonText) : base(title, message, buttonText)
        {
        }
    }
}