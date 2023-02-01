using RightToAskClient.Resx;
using Xamarin.Forms;

namespace RightToAskClient.Views
{
    public partial class CodeOfConductPage : ContentPage
    {
        public CodeOfConductPage()
        {
            InitializeComponent();
            var mdView = new Xam.Forms.Markdown.MarkdownView();
            mdView.Markdown = AppResources.CodeOfConductCopy;
            mdView.RelativeUrlHost = "";
            this.Content = new ScrollView() { Content = mdView };
        }
    }
}