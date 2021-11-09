using System;
using Xamarin.Forms;

/* A button intended for upvoting. It can be pressed once,
** at which point it shows the 'UndoText'.  If clicked again,
 * it reverts to its initial state.
 * The binding value is the thing to be incremented.
*/
namespace RightToAskClient.Controls
{
    
    public class ToggleButton : Button
    {
        private string undoMessage = "Undo upvote";
        private string initialText = "+1";
		private bool upVoteMode;

		public ToggleButton()
		{
			Text = initialText;
			Clicked += IncrementOrDecrement;
			BackgroundColor = Color.Turquoise;
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
		
        private void IncrementOrDecrement(object sender, EventArgs eventArgs)
		{
			Question q;
			q = BindingContext as Question;

			upVoteMode = Text.Equals(initialText);

			if (upVoteMode)
			{
			    q.UpVotes++;
			    Text = undoMessage;
			}
			else
			{
				q.UpVotes--;
			    Text = initialText;
			}
		}
    }
}