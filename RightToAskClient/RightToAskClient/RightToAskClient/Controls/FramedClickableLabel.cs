using System;
using Xamarin.Forms;

namespace RightToAskClient.Controls
{
    public class FramedClickableLabel : Frame
    {
	    private TapGestureRecognizer _tapped = new TapGestureRecognizer();
        private ClickableLabel _label = new ClickableLabel();
        public event EventHandler Tapped;

        public FramedClickableLabel()
        {
	        BorderColor = Color.Teal;
	        Content = _label;
	        _tapped.Tapped += _tapped_Tapped;
	        GestureRecognizers.Add(_tapped);
        }
            
		private void _tapped_Tapped(object sender, EventArgs e)
		{
			Tapped?.Invoke(sender, e);	
		}
        
    }
}