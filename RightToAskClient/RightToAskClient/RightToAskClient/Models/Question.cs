using RightToAskClient.ViewModels;
using RightToAskClient.Views;
using RightToAskClient.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using RightToAskClient.Models.ServerCommsData;
using RightToAskClient.Resx;
using RightToAskClient.Views.Popups;
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
        // Note these relate to whether this user up- or down-voted the question, not the global tally.
        private int _upVotesByThisUser;
        private int _downVotesByThisUser;

        public QuestionDetailsStatus Status { get; set; }

        private string _questionText = "";
        public string QuestionText
        {
            get => _questionText;
            set
            {
                SetProperty(ref _questionText, value);
                //** QuestionViewModel.Instance.ServerQuestionUpdates.question_text = _questionText;
                Updates.question_text = _questionText;
            }
        }

        // needed for getting a bool result back from the generic popups

        // Lists the updates that have occurred since construction.
        public QuestionSendToServer Updates { get; private set; } = new QuestionSendToServer()
        {
            // Most updates are simply omitted when not changed, but the Permissions enum needs to send a specific
            // "no change" value. 
            who_should_ask_the_question_permissions = RTAPermissions.NoChange,
            who_should_answer_the_question_permissions = RTAPermissions.NoChange
        };

        public void ReinitQuestionUpdates()
        {
            Updates = new QuestionSendToServer()
            {
                // Init explicit 'no change' value for permissions.
                who_should_answer_the_question_permissions = RTAPermissions.NoChange,
                who_should_ask_the_question_permissions = RTAPermissions.NoChange
            };
        }

        public int Timestamp { get; set; } = 0;
        public int LastModified { get; set; } = 0;
        public int TotalVotes { get; private set; } = 0;
        public int NetVotes { get; private set; } = 0;

        private string _background = "";
        public string Background
        {
            get => _background;
            set
            {
                SetProperty(ref _background, value);
                //** QuestionViewModel.Instance.ServerQuestionUpdates.background = _background;
                Updates.background = _background;
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
            set => SetProperty(ref _filters, value);
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
            set => SetProperty(ref _questionId, value);
            //** QuestionViewModel.Instance.ServerQuestionUpdates.question_id = _questionId;
        }
        private string _version = "";
        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
            //** QuestionViewModel.Instance.ServerQuestionUpdates.version = _version;
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
                Updates.who_should_answer_the_question_permissions = value;
            }
        }

        public string QuestionAnswerers =>  
            Extensions.JoinFilter(", ",
                string.Join(", ",Filters.SelectedAnsweringMPsNotMine.Select(mp => mp.ShortestName)),
                string.Join(", ",Filters.SelectedAnsweringMPsMine.Select(mp => mp.ShortestName)),
                string.Join(", ",Filters.SelectedAuthorities.Select(a => a.ShortestName)));

        // The MPs or committee who are meant to ask the question
        public string QuestionAskers =>
            Extensions.JoinFilter(", ",
                string.Join(", ", Filters.SelectedAskingMPsNotMine.Select(mp => mp.ShortestName)), 
                string.Join(", ", Filters.SelectedAskingMPsMine.Select(mp => mp.ShortestName)), 
                string.Join(",", Filters.SelectedCommittees.Select(com => com.ShortestName)));
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
                Updates.who_should_ask_the_question_permissions = value;
            }
        }

        // A list of existing answers, specifying who gave the answer in the role of representing which MP.
        public List<Answer>? _answers { get; set; } 
        
        public List<Answer> Answers => _answers;


        private List<Uri> _hansardLink = new List<Uri>();

        public List<Uri> HansardLink
        {
            get => _hansardLink;
            private set => SetProperty(ref _hansardLink, value);
            //** QuestionViewModel.Instance.ServerQuestionUpdates.hansard_link = _hansardLink;
        }

        public int UpVotesByThisUser
        {
            get => _upVotesByThisUser;
            set => SetProperty(ref _upVotesByThisUser, value);
        }
        public int DownVotesByThisUser 
        {
            get => _downVotesByThisUser;
            set
            {
                _downVotesByThisUser = value;
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

        // constructor needed for command creation
        public Question()
        {
            UpvoteCommand = new Command(async () => 

            {
                // can only upvote questions if you are registered
                if (IndividualParticipant.IsRegistered)
                {
                    if (!AlreadyUpvoted)
                    {
                        UpVotesByThisUser += 1;
                        AlreadyUpvoted = true;
                        IndividualParticipant.UpvotedQuestionIDs.Add(QuestionId);
                    }
                    else
                    {
                        UpVotesByThisUser -= 1;
                        AlreadyUpvoted = false;
                        IndividualParticipant.UpvotedQuestionIDs.Remove(QuestionId);
                    }
                }
                else
                {
                    var message = AppResources.CreateAccountPopUpText;
                    var popup = new TwoButtonPopup(AppResources.MakeAccountQuestionText, message, AppResources.NotNowAnswerText, AppResources.OKText, true); // this instance uses a model instead of a VM
                    var popupResult = await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
                    if (popup.HasApproved(popupResult))
                    {
                        await Shell.Current.GoToAsync($"{nameof(RegisterPage1)}");
                    }
                }
            });
            /*
            QuestionDetailsCommand = new Command(() =>
            {
                QuestionViewModel.Instance.Question = this;
                QuestionViewModel.Instance.IsNewQuestion = false;
                _ = Shell.Current.GoToAsync($"{nameof(QuestionDetailPage)}");
            });
            */
            
            /*
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
                    IndividualParticipant.ReportedQuestionIDs.Add(QuestionId);
                }
                else
                {
                    IndividualParticipant.ReportedQuestionIDs.Remove(QuestionId);
                }
            });
            */
        }

        // commands
        public Command UpvoteCommand { get; }
        // public Command ReportCommand { get; }
        // public Command QuestionDetailsCommand { get; }
        // public IAsyncCommand ShareCommand { get; }

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
            
            LastModified = serverQuestion.last_modified ?? 0;
            
            // vote-tally fields
            TotalVotes = serverQuestion.total_votes ?? 0;
            NetVotes = serverQuestion.net_votes ?? 0;
            
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
                foreach (var link in serverQuestion.hansard_link)
                {
                    var possibleUrl = ParliamentData.StringToValidParliamentaryUrl(link?.url ?? "");
                    if (possibleUrl.Success)
                    {
                        HansardLink.Add(possibleUrl.Data);
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
                        var myMPs = ParliamentData.FindAllMPsGivenElectorates(IndividualParticipant.ProfileData.RegistrationInfo.Electorates.ToList());
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
                        var myMPs = ParliamentData.FindAllMPsGivenElectorates(IndividualParticipant.ProfileData.RegistrationInfo.Electorates.ToList());
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
            var possibleItem = listB.ToList().Find(t => t != null && t.DataEquals(item));
            if (possibleItem is null)
            {
                return false;
            }
            
            listA.Add(possibleItem);
            return true;
        }

        public void AddHansardLink(Uri newHansardLink)
        {
            if (Updates.hansard_link is null)
            {
            Updates.hansard_link = new List<HansardLink>{new HansardLink(newHansardLink.OriginalString)};
            }
            // People may add multiple Hansard links at once.
            else
            {
                Updates.hansard_link.Add(new HansardLink(newHansardLink.OriginalString));
            }
            QuestionViewModel.Instance.Question.HansardLink.Add(newHansardLink);
            OnPropertyChanged("HansardLink");
        }

        public void AddAnswer(string answer)
        {
            Updates.answers = new List<QuestionAnswer>()
            {
                new QuestionAnswer()
                {
                    mp = new MPId(IndividualParticipant.MPRegisteredAs),
                    answer = answer
                }
            };
        }

        public void ToggleReportStatus()
        {
                AlreadyReported = !AlreadyReported;
                if (AlreadyReported)
                {
                    IndividualParticipant.ReportedQuestionIDs.Add(QuestionId);
                }
                else
                {
                    IndividualParticipant.ReportedQuestionIDs.Remove(QuestionId);
                }

        }
        
        //validation
        public bool ValidateNewQuestion()
        {
            // just needs question text for new questions
            return !string.IsNullOrEmpty(QuestionText);
        }

        public bool ValidateUpdateQuestion()
        {
            return !string.IsNullOrEmpty(QuestionText) && !string.IsNullOrEmpty(QuestionId) &&
                           !string.IsNullOrEmpty(Version);
            // needs more fields to update an existing question
        }

        public bool ValidateDownloadedQuestion()
        {
            return !string.IsNullOrEmpty(QuestionText) &&
                   !string.IsNullOrEmpty(QuestionId) &&
                   !string.IsNullOrEmpty(Version) &&
                   Timestamp != 0 &&
                   TotalVotes >= 0;
        }
    }
}