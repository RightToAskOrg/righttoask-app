using System;
using Xamarin.Forms;

namespace RightToAskClient.Controls
{
    public class ClickableLabel : Label
    {
	    public event EventHandler? Tapped;

		public ClickableLabel()
		{
			FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
			
			TapGestureRecognizer tapped = new TapGestureRecognizer();
			tapped.Tapped += _tapped_Tapped;
			GestureRecognizers.Add(tapped);
		}

		private void _tapped_Tapped(object sender, EventArgs e)
		{
			Tapped?.Invoke(sender, e);	
		}
    }
}