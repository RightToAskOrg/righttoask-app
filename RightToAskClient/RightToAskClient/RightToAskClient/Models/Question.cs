using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RightToAskClient.Models
{
    public class Question : INotifyPropertyChanged
    {
        private int _upVotes;
        private int _downVotes;

        public string QuestionText { get; set; } = "";
        
        // The citizen who suggested the question
        public string QuestionSuggester { get; set; } = "";
        
        // The Authority, department, MPs, who are meant to answer 
        public ObservableCollection<Entity> QuestionAnswerers { get; set; } = new ObservableCollection<Entity>();
        
        // The MPs or committee who are meant to ask the question
        public string QuestionAsker { get; set; } = "";

        public string LinkOrAnswer { get; set; } = "";

        public int UpVotes 
        {
            get
            {
                return _upVotes;
            }
            set
            {
                _upVotes = value;
                OnPropertyChanged();
            }
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
        
        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}