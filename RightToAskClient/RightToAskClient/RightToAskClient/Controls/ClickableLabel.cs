using System;
using System.Net.Mime;
using Xamarin.Forms;

namespace RightToAskClient.Controls
{
    public class ClickableLabel : Label
    {
	    private TapGestureRecognizer _tapped; 
		public event EventHandler Tapped;

		public ClickableLabel()
		{
			TextColor = Color.Teal;
			FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label));
			
			_tapped = new TapGestureRecognizer();
			_tapped.Tapped += _tapped_Tapped;
			GestureRecognizers.Add(_tapped);
		}

		private void _tapped_Tapped(object sender, EventArgs e)
		{
			Tapped?.Invoke(sender, e);	
		}
    }
}