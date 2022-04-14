using RightToAskClient.ViewModels;
using RightToAskClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using RightToAskClient.Models.ServerCommsData;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RightToAskClient.Models
{
    public class Question : ObservableObject
    {
        private int _upVotes;
        private int _downVotes;

        private string _questionText = "";
        public string QuestionText
        {
            get => _questionText;
            set
            {
                SetProperty(ref _questionText, value);
                QuestionViewModel.Instance._serverQuestionUpdates.question_text = _questionText;
            }
        }
        // VT: As an Australian, I am suspicious of anything except Unix time. Let's just store 
        // milliseconds, as the server does, and then display them according to the local timezone.
        // I am reliably advised that anything else is too clever to actually work.
        // Likewise for expiry date.
        public DateTime UploadTimestamp { get; set; }
        private string _background = "";
        public string Background
        {
            get => _background;
            set
            {
                SetProperty(ref _background, value);
                QuestionViewModel.Instance._serverQuestionUpdates.background = _background;
            }
        }
        public bool AnswerAccepted { get; set; }
        public string IsFollowupTo { get; set; } = "";
        // public List<string> Keywords { get; set; } = new List<string>();
        // public List<string> Category { get; set; } = new List<string>();
        // public DateTime ExpiryDate { get; set; }
        private string _questionId = "";
        public string QuestionId
        {
            get => _questionId;
            set
            {
                SetProperty(ref _questionId, value);
                QuestionViewModel.Instance._serverQuestionUpdates.question_id = _questionId;
            }
        }
        private string _version = "";
        public string Version
        {
            get => _version;
            set
            {
                SetProperty(ref _version, value);
                QuestionViewModel.Instance._serverQuestionUpdates.version = _version;
            }
        }

        // The person who suggested the question
        private string _questionSuggester = "";
        public string QuestionSuggester 
        { 
            get => _questionSuggester; 
            set => SetProperty(ref _questionSuggester, value); 
        }

        // The Authority, department, MPs, who are meant to answer 
        public ObservableCollection<Entity> QuestionAnswerers { get; set; } = new ObservableCollection<Entity>();

        // Whether the person writing the question allows other users to add QuestionAnswerers
        // false = RTAPermissions.WriterOnly  (default)
        // true  = RTAPermissions.Others
        private bool _othersCanAddAnswerers = false;
        public bool OthersCanAddAnswerers
        {
            get => _othersCanAddAnswerers;
            set 
            {
                SetProperty(ref _othersCanAddAnswerers, value);
                QuestionViewModel.Instance.WhoShouldAnswerItPermissions = _othersCanAddAnswerers ? RTAPermissions.Others : RTAPermissions.WriterOnly;
            }
        }

        // The MPs, committee or other Right To Ask user who are meant to ask the question

        public string QuestionAnswerer => QuestionAnswerers.FirstOrDefault().ToString() ?? "";
        
        // The MPs or committee who are meant to ask the question
        public string QuestionAsker { get; set; } = "";

        // Whether the person writing the question allows other users to add QuestionAnswerers
        // false = RTAPermissions.WriterOnly  (default)
        // true  = RTAPermissions.Others
        private bool _othersCanAddAskers = false;
        public bool OthersCanAddAskers
        {
            get => _othersCanAddAskers;
            set 
            {
                SetProperty(ref _othersCanAddAskers, value);
                QuestionViewModel.Instance.WhoShouldAskItPermissions = _othersCanAddAskers ? RTAPermissions.Others : RTAPermissions.WriterOnly;
            }
        }

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
        private bool _alreadyUpvoted;
        public bool AlreadyUpvoted 
        {
            get => _alreadyUpvoted;
            set => SetProperty(ref _alreadyUpvoted, value);
        }

        private bool _alreadyReported;
        public bool AlreadyReported
        {
            get => _alreadyReported;
            set => SetProperty(ref _alreadyReported, value);
        }

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

        // constructor needed for command creation
        public Question()
        {
            UpvoteCommand = new Command(() => 
            {
                if (!AlreadyUpvoted)
                {
                    UpVotes += 1;
                    AlreadyUpvoted = true;
                }
                else
                {
                    UpVotes -= 1;
                    AlreadyUpvoted = false;
                }
            });
            QuestionDetailsCommand = new Command(() =>
            {
                //QuestionViewModel.Instance.SelectedQuestion = this;
                QuestionViewModel.Instance.Question = this;
                QuestionViewModel.Instance.IsNewQuestion = false;
                _ = Shell.Current.GoToAsync($"{nameof(QuestionDetailPage)}");
            });
            ShareCommand = new AsyncCommand(async() =>
            {
                await Share.RequestAsync(new ShareTextRequest 
                {
                    Text = QuestionText,
                    Title = "Share Text"
                });
            });
            ReportCommand = new Command(() =>
            {
                AlreadyReported = !AlreadyReported;
            });
        }

        // command
        public Command UpvoteCommand { get; }
        public Command ReportCommand { get; }
        public Command QuestionDetailsCommand { get; }
        public IAsyncCommand ShareCommand { get; }
    }
}