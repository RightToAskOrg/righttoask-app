using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RightToAskClient.Models
{
    public class Question : INotifyPropertyChanged
    {
        private int upVotes;
        private int downVotes;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public string QuestionText { get; set; }
        
        // The citizen who suggested the question
        public string QuestionSuggester { get; set; }
        
        // The Authority, department, MPs, who are meant to answer 
        public ObservableCollection<Entity> QuestionAnswerers { get; set; }
        
        // The MPs or committee who are meant to ask the question
        public string QuestionAsker { get; set; }

        public string LinkOrAnswer { get; set; }

        public int UpVotes 
        {
            get
            {
                return upVotes;
            }
            set
            {
                upVotes = value;
                OnPropertyChanged("UpVotes");
            }
        }        
        public int DownVotes 
        {
            get
            {
                return downVotes;
            }
            set
            {
                downVotes = value;
                OnPropertyChanged("DownVotes");
            }
        }
        
        public override string ToString ()
        {
            
            List<string> questionAnswerersList 
                = QuestionAnswerers != null ? QuestionAnswerers.Select(ans => ans.EntityName).ToList()
                                    : new List<string>();
            // view.Select(f => return new { Food = f, Selected = selectedFood.Contains(f)});
            return QuestionText+ "\n" +
                   "Suggested by: " + (QuestionSuggester ?? "") + '\n' +
                   "To be asked by: " + (QuestionAsker ?? "") + '\n' +
                   // var readablePhrase = string.Join(" ", words); 
                   "To be answered by: " + string.Join(", ", questionAnswerersList) + '\n' +
                   "UpVotes: " + UpVotes+ '\n' +
                   // "DownVotes: " + DownVotes + '\n' +
                   "Link/Answer: " + (LinkOrAnswer ?? "");
        }
        
    }
}