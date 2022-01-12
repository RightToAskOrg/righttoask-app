using System;
using RightToAskClient.Models;
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
		private Question q = new Question();
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
		
		/* This should never be called with a BindingContext that isn't a 
		** Question, but we fall back to q just in case.
		 */
        private void IncrementOrDecrement(object sender, EventArgs eventArgs)
		{
			this.q = BindingContext as Question ?? q;
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