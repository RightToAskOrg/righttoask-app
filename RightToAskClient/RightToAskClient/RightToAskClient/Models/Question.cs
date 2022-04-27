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
        private NewQuestionSendToServer _updates = new NewQuestionSendToServer();

        private string _questionText = "";
        public string QuestionText
        {
            get => _questionText;
            set
            {
                SetProperty(ref _questionText, value);
                QuestionViewModel.Instance._serverQuestionUpdates.question_text = _questionText;
                _updates.question_text = _questionText;
            }
        }
        
        // Lists the updates that have occurred since construction.
        public NewQuestionSendToServer Updates => _updates; 
        
        public void ReinitQuestionUpdates()
        {
            _updates = new NewQuestionSendToServer();
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
                _updates.background = _background;
            }
        }

        private FilterChoices _filters = new FilterChoices();

        // These are all the metadata for the question, including who should
        // answer and ask it. This is part of the question because it may be completely
        // different from the filters that the user has put in place to find the 
        // question.
        // TODO Think about how to record updates appropriately.
        public FilterChoices Filters
        {
            get => _filters;
            set
            {
                SetProperty(ref _filters, value);
            }
        }
        
        // TODO do updates.
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
        // Note that this is never part of the updates - only the defining features - 
        // so doesn't need an update set.
        private string _questionSuggester = "";
        public string QuestionSuggester 
        { 
            get => _questionSuggester;
            set
            {
                SetProperty(ref _questionSuggester, value);
            }
        }

        // Whether the person writing the question allows other users to add QuestionAnswerers
        // false = RTAPermissions.WriterOnly  (default)
        // true  = RTAPermissions.Others
        // Note that this does not need an explicit _update fix because the ViewModel takes
        // care of the translation from 2 to 3 values.
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

        public string QuestionAnswerers => "" 
            + String.Join(", ",Filters.SelectedAnsweringMPs.Select(mp => mp.ShortestName))
            + String.Join(", ",Filters.SelectedAnsweringMPsMine.Select(mp => mp.ShortestName))
            + String.Join(", ",Filters.SelectedAuthorities.Select(a => a.ShortestName));

        // The MPs or committee who are meant to ask the question
        public string QuestionAskers => ""
            + String.Join(", ", Filters.SelectedAskingMPs.Select(mp => mp.ShortestName))
            + String.Join(", ", Filters.SelectedAskingMPsMine.Select(mp => mp.ShortestName));
        // TODO add:
            // + String.Join(",",Filters.SelectedAskingCommittee.Select(... ))
            // + String.Join(",",Filters.SelectedAskingUsers.Select(....));
            
        

        // Whether the person writing the question allows other users to add QuestionAnswerers
        // false = RTAPermissions.WriterOnly  (default)
        // true  = RTAPermissions.Others
        // Note that this does not need an explicit _update fix because the ViewModel takes
        // care of the translation from 2 to 3 values.
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

        private string _linkOrAnswer = "";

        public string LinkOrAnswer
        {
            get => _linkOrAnswer;
            set
            {
                SetProperty(ref _background, value);
                QuestionViewModel.Instance._serverQuestionUpdates.background = _background;
                // TODO Deal with types for vectors of answers.
                // _updates.answers = _linkOrAnswer;
            }
        }

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


        // constructor needed for command creation
        public Question()
        {
            UpvoteCommand = new Command(() => 
            {
                if (!AlreadyUpvoted)
                {
                    UpVotes += 1;
                    AlreadyUpvoted = true;
                    App.ReadingContext.ThisParticipant.UpvotedQuestionIDs.Add(QuestionId);
                }
                else
                {
                    UpVotes -= 1;
                    AlreadyUpvoted = false;
                    App.ReadingContext.ThisParticipant.UpvotedQuestionIDs.Remove(QuestionId);
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
                if (AlreadyReported)
                {
                    App.ReadingContext.ThisParticipant.ReportedQuestionIDs.Add(QuestionId);
                }
                else
                {
                    App.ReadingContext.ThisParticipant.ReportedQuestionIDs.Remove(QuestionId);
                }
            });
        }

        // commands
        public Command UpvoteCommand { get; }
        public Command ReportCommand { get; }
        public Command QuestionDetailsCommand { get; }
        public IAsyncCommand ShareCommand { get; }

    }
}