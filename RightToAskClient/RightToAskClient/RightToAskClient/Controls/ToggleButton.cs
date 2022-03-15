using System;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

/* A button intended for upvoting. It can be pressed once,
** at which point it shows the 'UndoText'.  If clicked again,
 * it reverts to its initial state.
 * The binding value is the thing to be incremented.
*/
namespace RightToAskClient.Controls
{
    
    public class ToggleButton : Button 
    {
		private Question _q = new Question();
        private string _undoMessage = "Undo upvote";
        private string _initialText = "+1";
		private bool _upVoteMode;

		public ToggleButton()
		{
			Text = _initialText;
			Clicked += IncrementOrDecrement;
		}
		
		public static readonly BindableProperty ToBeIncrementedProperty 
			= BindableProperty.Create(nameof(ToBeIncremented), typeof(Question), typeof(ToggleButton));  
		public Question ToBeIncremented 
		{  
			get 
			{  
			 	return (Question) GetValue(ToBeIncrementedProperty);  
			}  
			set 
			{  
				SetValue(ToBeIncrementedProperty, value);  
			}  
		}  
		
		/* This should never be called with a BindingContext that isn't a 
		** Question, but we fall back to q just in case.
		 */
        private void IncrementOrDecrement(object sender, EventArgs eventArgs)
		{
			_q = BindingContext as Question ?? _q;
			_upVoteMode = Text.Equals(_initialText);

			if (_upVoteMode)
			{
			    _q.UpVotes++;
			    Text = _undoMessage;
			}
			else
			{
				_q.UpVotes--;
			    Text = _initialText;
			}
		}
    }
}