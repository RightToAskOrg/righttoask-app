using RightToAskClient.Maui.ViewModels;
using RightToAskClient.Maui.Views;
using RightToAskClient.Maui.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using RightToAskClient.Maui.Models.ServerCommsData;
using RightToAskClient.Maui.Resx;
using RightToAskClient.Maui.Views.Popups;
using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RightToAskClient.Maui.Models
{
    public enum QuestionDetailsStatus
    {
        NewQuestion,
        OtherUserQuestion,
        UpdateMyQuestion
    }

    public class Question : ObservableObject
    {
        public QuestionDetailsStatus Status { get; set; }

        private string _questionText = "";
        public string QuestionText
        {
            get => _questionText;
            set
            {
                SetProperty(ref _questionText, value);
        //         Updates.question_text = _questionText;
            }
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
        private string _questionId = "";
        public string QuestionId
        {
            get => _questionId;
            set => SetProperty(ref _questionId, value);
        }
        private string _version = "";
        public string Version
        {
            get => _version;
            set => SetProperty(ref _version, value);
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
            }
        }


        // Whether the person writing the question allows other users to add QuestionAnswerers
        private RTAPermissions _whoShouldAskTheQuestionPermissions;
        public RTAPermissions WhoShouldAskTheQuestionPermissions
        {
            get => _whoShouldAskTheQuestionPermissions;
            set 
            {
                SetProperty(ref _whoShouldAskTheQuestionPermissions, value);
            }
        }

        // A list of existing answers, specifying who gave the answer in the role of representing which MP.
        public List<Answer> _answers { get; set; } = new List<Answer>();
        
        public List<Answer> Answers => _answers;


        private List<Uri> _hansardLink = new List<Uri>();

        public List<Uri> HansardLink
        {
            get => _hansardLink;
            private set => SetProperty(ref _hansardLink, value);
        }

        private bool _alreadyDownvoted;
        public bool AlreadyDownvoted 
        {
            get => _alreadyDownvoted;
            private set => SetProperty(ref _alreadyDownvoted, value);
        }
        
        private bool _alreadyUpvoted;
        public bool AlreadyUpvoted 
        {
            get => _alreadyUpvoted;
            private set => SetProperty(ref _alreadyUpvoted, value);
        }

        private bool _alreadyReported;
        public bool AlreadyReported
        {
            get => _alreadyReported;
            set => SetProperty(ref _alreadyReported, value);
        }

        public bool HasAnswer => Answers.Any();

        // Explicit empty constructor, for use in the case we're generating our own question.
        public Question()
        {
        } 
        
        // For questions we read off the server - check whether we've previously up-voted them or
        // reported them.
        public Question(in QuestionResponseRecords questionResponses) : base()
        {
        }

        // Call empty constructor to initialize commands etc.
        // Then convert data downloaded from server into a displayable form.
        public Question(QuestionReceiveFromServer serverQuestion, QuestionResponseRecords questionResponses) 
        {
            
            // question-defining fields
            QuestionSuggester = serverQuestion.author ?? "";
            _questionText = serverQuestion.question_text ?? "";
            Timestamp =  serverQuestion.timestamp ?? 0;
            
            // bookkeeping fields
            _questionId = serverQuestion.question_id ?? "";
            _version = serverQuestion.version ?? "";
            
            LastModified = serverQuestion.last_modified ?? 0;
            
            // vote-tally fields
            TotalVotes = serverQuestion.total_votes ?? 0;
            NetVotes = serverQuestion.net_votes ?? 0;
            
            // question non-defining fields
            // Note this needs to set the private field (_background), not the public one,
            // because the latter updates the Updates, which is not what we want at this point.
            _background = serverQuestion.background ?? "";

            // Check whether the user has already responded to this question.
            AlreadyUpvoted = questionResponses.IsAlreadyUpvoted(QuestionId);
            AlreadyDownvoted = questionResponses.IsAlreadyDownvoted(QuestionId);
            AlreadyReported = questionResponses.IsAlreadyReported(QuestionId);
            
            interpretFilters(serverQuestion);

            _whoShouldAnswerTheQuestionPermissions = serverQuestion.who_should_answer_the_question_permissions;
            _whoShouldAskTheQuestionPermissions = serverQuestion.who_should_ask_the_question_permissions;

            // Again, needs to be private _answers so that updates aren't set.
            _answers = serverQuestion.answers ?.Select(ans => new Answer(ans)).ToList() ?? new List<Answer>();
            
            AnswerAccepted = serverQuestion.answer_accepted ?? false;
            HansardLink = new List<Uri>();
            if (serverQuestion.hansard_link != null)
            {
                foreach (var link in serverQuestion.hansard_link)
                {
                    var possibleUrl = ParliamentaryURICreator.StringToValidParliamentaryUrl(link?.url ?? "");
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
                        var myMPs = ParliamentData.FindAllMPsGivenElectorates(IndividualParticipant.getInstance().ProfileData.RegistrationInfo.Electorates.ToList());
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
                        var myMPs = ParliamentData.FindAllMPsGivenElectorates(IndividualParticipant.getInstance().ProfileData.RegistrationInfo.Electorates.ToList());
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
            QuestionViewModel.Instance.Question.HansardLink.Add(newHansardLink);
            OnPropertyChanged("HansardLink");
        }



        public void ToggleUpvotedStatus()
        {
                AlreadyUpvoted = !AlreadyUpvoted;
        }
        public void ToggleDownvotedStatus()
        {
                AlreadyDownvoted = !AlreadyDownvoted;
        }

        public void ToggleReportStatus()
        {
                AlreadyReported = !AlreadyReported;
        }
        
        //validation
        public bool ValidateNewQuestion()
        {
            // just needs question text for new questions
            return !string.IsNullOrEmpty(QuestionText);
        }

        public bool ValidateUpdateQuestion()
        {
            return !string.IsNullOrEmpty(QuestionId) &&
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