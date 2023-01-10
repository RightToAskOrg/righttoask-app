using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using RightToAskClient.Models.ServerCommsData;
using Xamarin.Forms;

namespace RightToAskClient.Models
{
	public class FilterChoices : INotifyPropertyChanged
	{
		private string _searchKeyword = "";
		
		private SelectableList<Person> _questionWriterLists
			= new SelectableList<Person>(new List<Person>(), new List<Person>());

		// The writer of questions, for pages where we want to see all the questions written by a certain person.
		// Needs a public setter because the list of options is taken from what the user searches for.
		public SelectableList<Person> QuestionWriterLists
		{
			get => _questionWriterLists;
			set
			{
				_questionWriterLists = value;
				OnPropertyChanged();
			}
		}
		
		// Express each FilterChoice as a pair of lists: the whole list from which things are seleced,
		// and the list of selections.

		public SelectableList<Committee> CommitteeLists { get; private set; } = new SelectableList<Committee>(new List<Committee>(), new List<Committee>());

		public List<Committee> SelectedCommittees => CommitteeLists.SelectedEntities as List<Committee> ?? new List<Committee>();

		public SelectableList<Authority> AuthorityLists { get; private set; } = new SelectableList<Authority>(new List<Authority>(), new List<Authority>());

		public List<Authority> SelectedAuthorities => AuthorityLists.SelectedEntities as List<Authority> ?? new List<Authority>();

		public SelectableList<MP> AnsweringMPsListsMine { get; } = new SelectableList<MP>(new List<MP>(), new List<MP>());

		public List<MP> SelectedAnsweringMPsMine
		{
			get => AnsweringMPsListsMine.SelectedEntities as List<MP> ?? new List<MP>();
			set
			{
				AnsweringMPsListsMine.SelectedEntities = value;
				OnPropertyChanged();
			}
		}

		public SelectableList<MP> AskingMPsListsMine { get; } = new SelectableList<MP>(new List<MP>(), new List<MP>());

		public List<MP> SelectedAskingMPsMine
		{
			get => AskingMPsListsMine.SelectedEntities as List<MP> ?? new List<MP>();
			set
			{
				AskingMPsListsMine.SelectedEntities = value;
				OnPropertyChanged();
			}
		}

		public SelectableList<MP> AnsweringMPsListsNotMine { get; private set; } = new SelectableList<MP>(new List<MP>(), new List<MP>());

		public List<MP> SelectedAnsweringMPsNotMine
		{
			get => AnsweringMPsListsNotMine.SelectedEntities as List<MP> ?? new List<MP>();
			set
			{
				AnsweringMPsListsNotMine.SelectedEntities = value;
				OnPropertyChanged();
			}
		}

		public SelectableList<MP> AskingMPsListsNotMine { get; private set; } = new SelectableList<MP>(new List<MP>(), new List<MP>());

		public List<MP> SelectedAskingMPsNotMine
		{
			get =>  AskingMPsListsNotMine.SelectedEntities as List<MP> ?? new List<MP>();
			set
			{
				AskingMPsListsNotMine.SelectedEntities = value;
				OnPropertyChanged();
			}
		}

		public string SearchKeyword
		{
			get => _searchKeyword;
			set
			{
				_searchKeyword = value;
				OnPropertyChanged();
			}
		}


		/* Sometimes, FilterChoices may be initialised before the other complete lists are.
		 * This is most likely to happen for MyMPs, but may also happen for Committees or AllMPs,
		 * if server communication is slower than app load. In these cases, we need to re-initialise
		 * after getting a message from the respective server-communicating data structures.
		 * The other instances are related to specific questions as they are
		 * downloaded - in these cases, the Init in the constructor is all that's needed
		 * because the other information is (almost certainly) already set up.
		 */
		public FilterChoices()
		{
			InitSelectableLists();
			MessagingCenter.Subscribe<object> (
				this, 
				Constants.NeedToUpdateMyMpLists, 
				(sender) =>
				{
					UpdateMyMPLists();
				});
			MessagingCenter.Subscribe<object> (
				this, 
				Constants.InitAllMPsLists, 
				(sender) =>
				{
					UpdateAllMPsLists();
				});
			MessagingCenter.Subscribe<object> (
				this, 
				Constants.InitCommitteeLists, 
				(sender) =>
				{
					UpdateAllCommitteesLists();
				});
		}

		public event PropertyChangedEventHandler? PropertyChanged;
        
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // This is necessary on startup because of the need to update the AllMPs list.
        // It's still helpful for them to use the constructor because if this data structure
        // is initialized after the MP read-in, the constructor should suffice.
        private void InitSelectableLists()
        {
	        // Note: No init for question writers, because it needs to be initialised when the user searches for names.
	        AnsweringMPsListsNotMine = new SelectableList<MP>(ParliamentData.AllMPs, new List<MP>());
	        AskingMPsListsNotMine =  new SelectableList<MP>(ParliamentData.AllMPs, new List<MP>());
	        AuthorityLists = new SelectableList<Authority>(ParliamentData.AllAuthorities, new List<Authority>());
	        CommitteeLists =
		        new SelectableList<Committee>(CommitteesAndHearingsData.AllCommittees, new List<Committee>());
	        UpdateMyMPLists();
        }

        public void RemoveAllSelections()
        {
	        AnsweringMPsListsNotMine.SelectedEntities = new List<MP>();
	        AnsweringMPsListsMine.SelectedEntities = new List<MP>();
	        AskingMPsListsNotMine.SelectedEntities = new List<MP>();
	        AskingMPsListsMine.SelectedEntities = new List<MP>();
	        AuthorityLists.SelectedEntities = new List<Authority>();
	        CommitteeLists.SelectedEntities = new List<Committee>();
	        _questionWriterLists.SelectedEntities = new List<Person>();
	        SearchKeyword = "";
        }

        static public void NeedToUpdateMyMpLists(object sender)
        {
	        MessagingCenter.Send<object>(sender, Constants.NeedToUpdateMyMpLists);
        }

        static public void NeedToInitAllMPsLists(object sender)
        {
	        MessagingCenter.Send<object>(sender, Constants.InitAllMPsLists);
        }
        static public void NeedToInitCommitteeLists(object sender)
        {
	        MessagingCenter.Send<object>(sender, Constants.InitCommitteeLists);
        }

        public List<PersonID> TranscribeQuestionAnswerersForUpload()
        {
            // We take the (duplicate-removing) union of selected MPs, because at the moment the UI doesn't remove 
            // your MPs from the 'other MPs' list and the user may have selected the same MP in both categories.
            var MPAnswerers = SelectedAnsweringMPsNotMine.Union(SelectedAnsweringMPsMine);
            var MPanswerersServerData = MPAnswerers.Select(mp => new PersonID(new MPId(mp)));
            
            // Add authorities, guaranteed not to be duplicates.
            return MPanswerersServerData.
                Concat(SelectedAuthorities.Select(a => new PersonID(a))).ToList();
        }

        public List<PersonID> TranscribeQuestionAskersForUpload()
        {
	        // Entities who should raise the question - currently just MPs and committees.
	        var MPAskers = SelectedAskingMPsNotMine.Union(SelectedAskingMPsMine);
	        var MPAskersServerData = MPAskers.Select(mp => new PersonID(new MPId(mp)));

	        // Add committees, guaranteed not to be duplicates.
	        return MPAskersServerData.
		        Concat(SelectedCommittees.Select(c => new PersonID(new CommitteeInfo(c)))).ToList();
        }

        // Update the list of my MPs. Called when the user changes their electorates (including choosing some
        // for the first time).
        // Note that it's a tiny bit unclear whether we should remove
        // any selected MPs who are (now) not yours. At the moment, I have simply left it as it is,
        // which means that if a person starts drafting a question, then changes their electorate details,
        // it's still possible for the question to go to 'my MP' when that person is their previous MP.
        private void UpdateMyMPLists()
        {
	        AnsweringMPsListsMine.AllEntities = ParliamentData.FindAllMPsGivenElectorates(IndividualParticipant.getInstance().ProfileData.RegistrationInfo.Electorates.ToList());
	        AskingMPsListsMine.AllEntities = AnsweringMPsListsMine.AllEntities;
        }
        
        // Called during initialization phase that reads all MP lists from server. The ...MPsListsMine will be non-empty if
        // We already have stored electorates for this person. 
		private void UpdateAllMPsLists()
		{
	        AnsweringMPsListsNotMine = new SelectableList<MP>(ParliamentData.AllMPs, new List<MP>());
	        AskingMPsListsNotMine =  new SelectableList<MP>(ParliamentData.AllMPs, new List<MP>());
		}

		private void UpdateAllCommitteesLists()
		{
	        CommitteeLists =
		        new SelectableList<Committee>(CommitteesAndHearingsData.AllCommittees, new List<Committee>());
		}


        public bool Validate()
        {
            var isValid = false;
            var hasInvalidData = false;
            if (SelectedAnsweringMPsMine.Any())
            {
                foreach (var mp in SelectedAnsweringMPsMine)
                {
                    if (!mp.Validate())
                    {
                        hasInvalidData = true;
                    }
                }
            }
            if (SelectedAskingMPsMine.Any())
            {
                foreach (var mp in SelectedAskingMPsMine)
                {
                    if (!mp.Validate())
                    {
                        hasInvalidData = true;
                    }
                }
            }
            if (SelectedAuthorities.Any())
            {
                foreach (var auth in SelectedAuthorities)
                {
                    if (!auth.Validate())
                    {
                        hasInvalidData = true;
                    }
                }
            }
            if (SelectedCommittees.Any())
            {
                foreach (var com in SelectedCommittees)
                {
                    if (string.IsNullOrEmpty(com.ShortestName))
                    {
                        hasInvalidData = true;
                    }
                }
            }
            
            isValid = !hasInvalidData;
            return isValid;
        }
    }
}
