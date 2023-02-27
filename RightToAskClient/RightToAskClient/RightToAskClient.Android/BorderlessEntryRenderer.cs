using Android.Content;
using RightToAskClient.Droid.UI.Renderers;
using RightToAskClient.Views.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(BorderlessEntry), typeof(BorderlessEntryRenderer))]
namespace RightToAskClient.Droid.UI.Renderers
{
    public class BorderlessEntryRenderer : EntryRenderer
    {
        public BorderlessEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            //Configure native control (TextBox)
            if(Control != null)
            {
                Control.Background = null;
            }

            // Configure Entry properties
            if(e.NewElement != null)
            {

            }
        }
    }
}