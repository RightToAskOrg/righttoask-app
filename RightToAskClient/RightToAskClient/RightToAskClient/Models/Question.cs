using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.CommunityToolkit.ObjectModel;

namespace RightToAskClient.Models
{
    public class Question : ObservableObject
    {
        private int _upVotes;
        private int _downVotes;

        public string QuestionText { get; set; } = "";
        public DateTime UploadTimestamp { get; set; }
        public string Background { get; set; } = "";
        public bool AnswerAccepted { get; set; }
        public string IsFollowupTo { get; set; } = "";
        public List<string> Keywords { get; set; } = new List<string>();
        public List<string> Category { get; set; } = new List<string>();
        public DateTime ExpiryDate { get; set; }
        public string QuestionId { get; set; } = "";
        public string Version { get; set; } = "";

        // The citizen who suggested the question
        private string _questionSuggester = "";
        public string QuestionSuggester 
        { 
            get => _questionSuggester; 
            set => SetProperty(ref _questionSuggester, value); 
        }

        // The Authority, department, MPs, who are meant to answer 
        public ObservableCollection<Entity> QuestionAnswerers { get; set; } = new ObservableCollection<Entity>();
        
        // The MPs or committee who are meant to ask the question
        public string QuestionAsker { get; set; } = "";

        public string LinkOrAnswer { get; set; } = "";

        public int UpVotes
        {
            get => _upVotes;
            set => SetProperty(ref _upVotes, value);
        }
        public int DownVotes 
        {
            get
            {
                return _downVotes;
            }
            set
            {
                _downVotes = value;
                OnPropertyChanged();
            }
        }
        
        public bool AlreadyUpvoted { get; set; }

        public override string ToString ()
        {
            
            List<string> questionAnswerersList 
                = QuestionAnswerers.Select(ans => ans.GetName()).ToList();
            // view.Select(f => return new { Food = f, Selected = selectedFood.Contains(f)});
            return QuestionText+ "\n" +
                   "Suggested by: " + QuestionSuggester + '\n' +
                   "To be asked by: " + QuestionAsker + '\n' +
                   // var readablePhrase = string.Join(" ", words); 
                   "To be answered by: " + string.Join(", ", questionAnswerersList) + '\n' +
                   "UpVotes: " + UpVotes+ '\n' +
                   // "DownVotes: " + DownVotes + '\n' +
                   "Link/Answer: " + LinkOrAnswer;
        }
    }
}