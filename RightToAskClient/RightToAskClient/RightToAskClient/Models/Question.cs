using RightToAskClient.ViewModels;
using RightToAskClient.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Resx;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace RightToAskClient.Models
{
    public class Question : ObservableObject
    {
        private int _upVotes;
        private int _downVotes;
        private QuestionSendToServer _updates = new QuestionSendToServer();

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
        public QuestionSendToServer Updates => _updates; 
        
        public void ReinitQuestionUpdates()
        {
            _updates = new QuestionSendToServer();
        }
        // TODO: As an Australian, I am suspicious of anything except Unix time. Let's just store 
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
            set => SetProperty(ref _questionSuggester, value); 
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

        private List<Uri> _hansardLink = new List<Uri>();

        public List<Uri> HansardLink
        {
            get => _hansardLink;
            set
            {
                SetProperty(ref _hansardLink, value);
                QuestionViewModel.Instance._serverQuestionUpdates.hansard_link = _hansardLink;
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

        private bool _hasAnswer;
        public bool HasAnswer
        {
            get => _hasAnswer;
            set => SetProperty(ref _hasAnswer, value);
        }

        /*
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
        */

        // constructor needed for command creation
        public Question()
        {
            UpvoteCommand = new Command(async () => 

            {
                // can only upvote questions if you are registered
                if (App.ReadingContext.ThisParticipant.IsRegistered)
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
                }
                else
                {
                    string message = AppResources.CreateAccountPopUpText;
                    bool registerNow = await App.Current.MainPage.DisplayAlert(AppResources.MakeAccountQuestionText, message, AppResources.OKText, AppResources.NotNowAnswerText);
                    if (registerNow)
                    {
                        await Shell.Current.GoToAsync($"{nameof(RegisterPage1)}");
                    }
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
        
        // Call empty constructor to initialize commands etc.
        public Question(QuestionReceiveFromServer serverQuestion) : this()
        {
            // question-defining fields
            QuestionSuggester = serverQuestion.author ?? "";
            QuestionText = serverQuestion.question_text ?? "";
            // TODO Not clear whether we need this.
            // Timestamp = serverQuestion.timestamp ?? "";
            
            // bookkeeping fields
            QuestionId = serverQuestion.question_id ?? "";
            Version = serverQuestion.version ?? "";
            
            // question non-defining fields
            Background = serverQuestion.background ?? "";

            interpretFilters(serverQuestion);
            
            OthersCanAddAnswerers = serverQuestion.who_should_answer_the_question_permissions == RTAPermissions.Others;
            OthersCanAddAskers = serverQuestion.who_should_ask_the_question_permissions == RTAPermissions.Others;

            // TODO Add answers
            // Answers = serverQuestion.answers ?? new List();

            AnswerAccepted = serverQuestion.answer_accepted ?? false;
            HansardLink = serverQuestion.hansard_link ?? new List<Uri>();
            IsFollowupTo = serverQuestion.is_followup_to ?? "";
        }

        // At the moment, if this gets an entity that it can't match, it simply adds it to the 'selected' list anyway,
        // in the minimal form it receives from the server.
        // TODO it's possible that this may cause problems with uniqueness, and hence we may consider dropping it instead.
        private void interpretFilters(QuestionReceiveFromServer serverQuestion)
        {
            Filters = new FilterChoices();

            if (serverQuestion.entity_who_should_answer_the_question != null)
            {
                foreach (var entity in serverQuestion.entity_who_should_answer_the_question)
                {
                    if (entity.AsAuthority != null)
                    {
                        // If we can find it in our existing authority list, add that item to 'selected'
                        if(!CanFindInListBThenAddToListA(entity.AsAuthority, Filters.SelectedAuthorities, 
                            ParliamentData.AllAuthorities))
                        {
                            // otherwise, add the authority we just constructed/received
                            Filters.SelectedAuthorities.Add(entity.AsAuthority);
                        }
                    }
                    else if (entity.AsMP != null)
                    {
                        // If the MP is one of mine, add it to AnsweringMPsMine
                        if (!CanFindInListBThenAddToListA<MP>(entity.AsMP,
                            Filters.SelectedAnsweringMPsMine, App.ReadingContext.ThisParticipant.MyMPs))
                        {
                            // otherwise, try to find it in AllMPs
                            if (!CanFindInListBThenAddToListA<MP>(entity.AsMP, Filters.SelectedAnsweringMPs,
                                ParliamentData.AllMPs))
                            {
                                // If all else fails, add the bare-bones MP record we received.
                                Filters.SelectedAnsweringMPs.Add(entity.AsMP);
                            }
                        }
                    }
                }
            }

            // Exactly the same, but for asking rather than answering MPs 
            // except that at the moment, we have no authorities/orgs set up to be able to ask the question,
            // and also we may one day have other users set to ask questions.
            if (serverQuestion.mp_who_should_ask_the_question != null)
            {
                foreach (var entity in serverQuestion.mp_who_should_ask_the_question)
                {
                    // TODO SelectedAskingUsers isn't used anywhere yet.
                    if (entity.AsRTAUser != null)
                    {
                        Filters.SelectedAskingUsers.Add(entity.AsRTAUser);
                    }
                    else if (entity.AsMP != null)
                    {
                        // If the MP is one of mine, add it to AskingMPsMine
                        if (!CanFindInListBThenAddToListA<MP>(entity.AsMP,
                                Filters.SelectedAskingMPsMine, App.ReadingContext.ThisParticipant.MyMPs))
                        {
                            // otherwise, try to find it in AllMPs
                            if (!CanFindInListBThenAddToListA<MP>(entity.AsMP, Filters.SelectedAskingMPs,
                                ParliamentData.AllMPs))
                            {
                                // If all else fails, add bare-bones MP data we got from the server.
                                Filters.SelectedAskingMPs.Add(entity.AsMP);
                            }
                        }
                    }
                }
            }
        }

        // If an item equal to item is found in listB, that list element is added to listA.
        // Note that the ListB element is added, not the item - this is important for data structures
        // such as MP in which the equality operator is true if identifying fields (but not necessarily
        // all fields) are equal.
        // Returns true if the item was found
        private bool CanFindInListBThenAddToListA<T>(T item, IEnumerable<T> listA, IEnumerable<T> listB)
        {
            T possibleItem = listB.ToList().Find(t => t != null && t.Equals(item));
            if (possibleItem is null)
            {
                return false;
            }
            
            listA.ToList().Add(possibleItem);
            return true;
        }

        //validation
        public bool Validate()
        {
            bool isValid = false;
            // must have an Id string, and question text
            if (!string.IsNullOrEmpty(QuestionId)
                && !string.IsNullOrEmpty(QuestionText))
            {
                isValid = true;
            }
            return isValid;
        }
    }
}