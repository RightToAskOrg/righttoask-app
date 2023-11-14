using System;
using RightToAskClient.Maui.Models;


namespace RightToAskClient.Maui.Views
{
    public partial class PersonProfilePage : ContentPage
    {
        private ReadingContext context;
        public PersonProfilePage (string name, ReadingContext context) 
        {
            InitializeComponent ();
            this.context = context;
            BindingContext = context;
            
            DMButton.Text = "Send Direct Message to " + name;
            SeeQuestionsButton.Text = "Read questions from " + name;
            FollowButton.Text = "Follow " + name;
        }
        
        private void FollowButton_OnClicked(object sender, EventArgs e)
        {
            ((Button) sender).Text = "Following not implemented";
            
        }

        private void DMButton_OnClicked(object sender, EventArgs e)
        {
            ((Button) sender).Text = "DMs not implemented";
        }
        
        // At the moment, this pushes a brand new question-reading page,
        // which is meant to have only questions from this person, but
        // at the moment just has everything.
        // 
        // Think a bit harder about how people will navigate or understand this:
        // Will they expect to be adding a new stack layer, or popping off old ones?
        private async void SeeQuestionsButton_OnClicked(object sender, EventArgs e)
        {
			var readingPage = new ReadingPage(true, context);
			await Navigation.PushAsync (readingPage);
        }
    }
}