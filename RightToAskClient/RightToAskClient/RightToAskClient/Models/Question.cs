using RightToAskClient.ViewModels;
using RightToAskClient.Views;
using RightToAskClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Resx;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.CommunityToolkit.Extensions;

namespace RightToAskClient.Models
{
    public enum QuestionDetailsStatus
    {
        NewQuestion,
        OtherUserQuestion,
        UpdateMyQuestion
    }

    public class Question : ObservableObject
    {
        private int _upVotes;
        private int _downVotes;

        private QuestionSendToServer _updates = new QuestionSendToServer()
        {
            // Most updates are simply omitted when not changed, but the Permissions enum needs to send a specific
            // "no change" value. 
            who_should_ask_the_question_permissions = RTAPermissions.NoChange,
            who_should_answer_the_question_permissions = RTAPermissions.NoChange
        };

        public QuestionDetailsStatus Status { get; set; }

        private string _questionText = "";
        public string QuestionText
        {
            get => _questionText;
            set
            {
                SetProperty(ref _questionText, value);
                //** QuestionViewModel.Instance.ServerQuestionUpdates.question_text = _questionText;
                _updates.question_text = _questionText;
            }
        }

        // needed for getting a bool result back from the generic popups
        public bool PopupResponse { get; set; } = false;

        // Lists the updates that have occurred since construction.
        public QuestionSendToServer Updates => _updates; 
        
        public void ReinitQuestionUpdates()
        {
            _updates = new QuestionSendToServer()
            {
                // Init explicit 'no change' value for permissions.
                who_should_answer_the_question_permissions = RTAPermissions.NoChange,
                who_should_ask_the_question_permissions = RTAPermissions.NoChange
            };
        }
        public int Timestamp { get; set; }
        
        private string _background = "";
        public string Background
        {
            get => _background;
            set
            {
                SetProperty(ref _background, value);
                //** QuestionViewModel.Instance.ServerQuestionUpdates.background = _background;
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
                //** QuestionViewModel.Instance.ServerQuestionUpdates.question_id = _questionId;
            }
        }
        private string _version = "";
        public string Version
        {
            get => _version;
            set
            {
                SetProperty(ref _version, value);
                //** QuestionViewModel.Instance.ServerQuestionUpdates.version = _version;
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
        private RTAPermissions _whoShouldAnswerTheQuestionPermissions;
        public RTAPermissions WhoShouldAnswerTheQuestionPermissions
        {
            get => _whoShouldAnswerTheQuestionPermissions;
            set 
            {
                SetProperty(ref _whoShouldAnswerTheQuestionPermissions, value);
                _updates.who_should_answer_the_question_permissions = value;
            }
        }

        public string QuestionAnswerers =>  
            Extensions.JoinFilter(", ",
                String.Join(", ",Filters.SelectedAnsweringMPsNotMine.Select(mp => mp.ShortestName)),
                String.Join(", ",Filters.SelectedAnsweringMPsMine.Select(mp => mp.ShortestName)),
                String.Join(", ",Filters.SelectedAuthorities.Select(a => a.ShortestName)));

        // The MPs or committee who are meant to ask the question
        public string QuestionAskers =>
            Extensions.JoinFilter(", ",
                String.Join(", ", Filters.SelectedAskingMPsNotMine.Select(mp => mp.ShortestName)), 
                String.Join(", ", Filters.SelectedAskingMPsMine.Select(mp => mp.ShortestName)), 
                String.Join(",", Filters.SelectedCommittees.Select(com => com.ShortestName)));
            // TODO add:
            // + String.Join(",",Filters.SelectedAskingUsers.Select(....));
            
        

        // Whether the person writing the question allows other users to add QuestionAnswerers
        private RTAPermissions _whoShouldAskTheQuestionPermissions;
        public RTAPermissions WhoShouldAskTheQuestionPermissions
        {
            get => _whoShouldAskTheQuestionPermissions;
            set 
            {
                SetProperty(ref _whoShouldAskTheQuestionPermissions, value);
                _updates.who_should_ask_the_question_permissions = value;
            }
        }

        // A list of existing answers, specifying who gave the answer in the role of representing which MP.
        public List<Answer>? _answers { get; set; } 
        
        public List<Answer> Answers 
        { 
            get => _answers;
        }

        
        private List<Uri> _hansardLink = new List<Uri>();

        public List<Uri> HansardLink
        {
            get => _hansardLink;
            private set
            {
                SetProperty(ref _hansardLink, value);
                //** QuestionViewModel.Instance.ServerQuestionUpdates.hansard_link = _hansardLink;
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

        // chosen by route of option A or B to answer in app or raise in parliament
        private bool _answerInApp = false;
        public bool AnswerInApp
        {
            get => _answerInApp;
            set => SetProperty(ref _answerInApp, value);
        }

        // booleans stored for new style popups
        private bool _approveClicked = false;
        public bool ApproveClicked
        {
            get => _approveClicked;
            set => SetProperty(ref _approveClicked, value);
        }
        private bool _cancelClicked = false;
        public bool CancelClicked
        {
            get => _cancelClicked;
            set => SetProperty(ref _cancelClicked, value);
        }

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
                    var popup = new TwoButtonPopup(this, AppResources.MakeAccountQuestionText, message, AppResources.NotNowAnswerText, AppResources.OKText); // this instance uses a model instead of a VM
                    _ = await App.Current.MainPage.Navigation.ShowPopupAsync(popup);
                    if (ApproveClicked)
                    {
                        await Shell.Current.GoToAsync($"{nameof(RegisterPage1)}");
                    }
                }
            });
            QuestionDetailsCommand = new Command(() =>
            {
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
            PopupApproveCommand = new Command(() =>
            {
                ApproveClicked = true;
                CancelClicked = false;
            });
            PopupCancelCommand = new Command(() =>
            {
                CancelClicked = true;
                ApproveClicked = false;
            });
        }

        // commands
        public Command UpvoteCommand { get; }
        public Command ReportCommand { get; }
        public Command PopupApproveCommand { get; }
        public Command PopupCancelCommand { get; }
        public Command QuestionDetailsCommand { get; }
        public IAsyncCommand ShareCommand { get; }

        // Call empty constructor to initialize commands etc.
        // Then convert data downloaded from server into a displayable form.
        public Question(QuestionReceiveFromServer serverQuestion) : this()
        {
            // question-defining fields
            QuestionSuggester = serverQuestion.author ?? "";
            QuestionText = serverQuestion.question_text ?? "";
            Timestamp =  serverQuestion.timestamp ?? 0;
            
            // bookkeeping fields
            QuestionId = serverQuestion.question_id ?? "";
            Version = serverQuestion.version ?? "";
            
            // question non-defining fields
            Background = serverQuestion.background ?? "";

            interpretFilters(serverQuestion);

            WhoShouldAnswerTheQuestionPermissions = serverQuestion.who_should_answer_the_question_permissions;
            WhoShouldAskTheQuestionPermissions = serverQuestion.who_should_ask_the_question_permissions;

            _answers = serverQuestion.answers ?.Select(ans => new Answer(ans)).ToList() ?? new List<Answer>();
            AnswerAccepted = serverQuestion.answer_accepted ?? false;
            HansardLink = new List<Uri>();
            if (serverQuestion.hansard_link != null)
            {
                foreach (HansardLink? link in serverQuestion.hansard_link)
                {
                    var possibleUrl = ParliamentData.StringToValidParliamentaryUrl(link?.url ?? "");
                    if (String.IsNullOrEmpty(possibleUrl.Err))
                    {
                        HansardLink.Add(possibleUrl.Ok);
                    }
                }
            }

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
                        var myMPs = ParliamentData.FindAllMPsGivenElectorates(App.ReadingContext.ThisParticipant.RegistrationInfo.Electorates.ToList());
                        if (!CanFindInListBThenAddToListA<MP>(entity.AsMP, Filters.SelectedAnsweringMPsMine, myMPs))
                        {
                            // otherwise, try to find it in AllMPs
                            if (!CanFindInListBThenAddToListA<MP>(entity.AsMP, Filters.SelectedAnsweringMPsNotMine,
                                ParliamentData.AllMPs))
                            {
                                // If all else fails, add the bare-bones MP record we received.
                                Filters.SelectedAnsweringMPsNotMine.Add(entity.AsMP);
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
                    // SelectedAskingUsers isn't used anywhere yet.
                    /*
                    if (entity.AsRTAUser != null)
                    {
                        Filters.SelectedQuestionAsker.Add(entity.AsRTAUser);
                    } else
                    */
                    if (entity.AsMP != null)
                    {
                        // If the MP is one of mine, add it to AskingMPsMine
                        var myMPs = ParliamentData.FindAllMPsGivenElectorates(App.ReadingContext.ThisParticipant.RegistrationInfo.Electorates.ToList());
                        if (!CanFindInListBThenAddToListA<MP>(entity.AsMP, Filters.SelectedAskingMPsMine, myMPs))
                        {
                            // otherwise, try to find it in AllMPs
                            if (!CanFindInListBThenAddToListA<MP>(entity.AsMP, Filters.SelectedAskingMPsNotMine,
                                ParliamentData.AllMPs))
                            {
                                // If all else fails, add bare-bones MP data we got from the server.
                                Filters.SelectedAskingMPsNotMine.Add(entity.AsMP);
                            }
                        }
                    }
                    else if (entity.AsCommittee != null)
                    {
                        // Add the relevant committee from AllCommittees if we can find it.
                        if (!CanFindInListBThenAddToListA<Committee>(entity.AsCommittee, Filters.SelectedCommittees,
                            CommitteesAndHearingsData.AllCommittees))
                        {
                            // If all else fails, add bare-bones Committee data we got from the server.
                            Filters.SelectedCommittees.Add(entity.AsCommittee);
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
        private bool CanFindInListBThenAddToListA<T>(T item, List<T> listA, IEnumerable<T> listB)  where T: Entity
        {
            T possibleItem = listB.ToList().Find(t => t != null && t.DataEquals(item));
            if (possibleItem is null)
            {
                return false;
            }
            
            listA.Add(possibleItem);
            return true;
        }

        //validation
        public bool ValidateNewQuestion()
        {
            bool isValid = false;
            // just needs question text for new questions
            if (!string.IsNullOrEmpty(QuestionText))
            {
                isValid = true;
            }
            return isValid;
        }

        public bool ValidateUpdateQuestion()
        {
            bool isValid = false;
            // needs more fields to update an existing question
            if (!string.IsNullOrEmpty(QuestionText)
                && !string.IsNullOrEmpty(QuestionId)
                && !string.IsNullOrEmpty(Version))
            {
                isValid = true;
            }
            return isValid;
        }

        public void AddHansardLink(Uri newHansardLink)
        {
            if (_updates.hansard_link is null)
            {
            _updates.hansard_link = new List<HansardLink>{new HansardLink(newHansardLink.OriginalString)};
            }
            // People may add multiple Hansard links at once.
            else
            {
                _updates.hansard_link.Add(new HansardLink(newHansardLink.OriginalString));
            }
            QuestionViewModel.Instance.Question.HansardLink.Add(newHansardLink);
            OnPropertyChanged("HansardLink");
        }

        public void AddAnswer(string answer)
        {
            var me = App.ReadingContext.ThisParticipant;

            _updates.answers = new List<QuestionAnswer>()
            {
                new QuestionAnswer()
                {
                    mp = new MPId(me.MPRegisteredAs),
                    answer = answer
                }
            };
        }
    }
}