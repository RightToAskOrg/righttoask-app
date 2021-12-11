using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RightToAskClient.Models
{
	public class ReadingContext : INotifyPropertyChanged
	{
        public event PropertyChangedEventHandler PropertyChanged;

        public ReadingContext()
        {
	        InitializeDefaultSetup();
        }
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        
		// Things about this user.
		// These selections are made at registration, or at 'complete registration.'
		public IndividualParticipant ThisParticipant { get; set; } = new IndividualParticipant();

		// Things about the current search, draft question or other action.
		public FilterChoices Filters { get; set; }
		public string DraftQuestion { get; set; }
		
		// Whether this is a 'top ten' search.
		public bool TopTen { get; set; }

		public string GoDirect_Committee { get; set; }
		
		public string GoDirect_MP { get; set; }

		// Existing things about the world.
		// TODO: at the moment, the list of 'my MPs' is the 
		// hardcoded and has the generic 'Entity' type. 
		
		public ObservableCollection<Entity> TestCurrentMPs { get; set; }

		public ObservableCollection<Question> ExistingQuestions { get; set; }
		
		//public ObservableCollection<Tag> Departments { get; set; }

		// public ObservableCollection<Tag> SelectableAuthorities { get; set; }

		


		// At the moment, this simply populates the reading context with a
		// hardcoded set of "existing" questions.
		private void InitializeDefaultSetup()
		{
			// ThisParticipant = new IndividualParticipant();
			Filters = new FilterChoices();

			/*
			Departments = new ObservableCollection<Tag>();
			Departments.Add(new Tag { TagEntity = new Authority { AuthorityName = "Environment" }, Selected = false });
			Departments.Add(new Tag { TagEntity = new Authority { AuthorityName = "Home Affairs" }, Selected = false });
			Departments.Add(new Tag { TagEntity = new Authority { AuthorityName = "Defence" }, Selected = false });
			Departments.Add(new Tag { TagEntity = new Authority { AuthorityName = "Health" }, Selected = false });
			Departments.Add(new Tag { TagEntity = new Authority { AuthorityName = "Treasury" }, Selected = false });
			Departments.Add(new Tag { TagEntity = new Authority { AuthorityName = "Human Services" }, Selected = false });
			Departments.Add(new Tag
				{ TagEntity = new Authority { AuthorityName = "Innovation, Industry and Science" }, Selected = false });
			Departments.Add(new Tag { TagEntity = new Authority { AuthorityName = "Communications" }, Selected = false });

			MyMPs = new ObservableCollection<Tag>();
			MyMPs.Add(new Tag { TagEntity = new Authority { EntityName = "Janet Rice" }, Selected = false });
			MyMPs.Add(new Tag { TagEntity = new Authority { EntityName = "Danny O'Brien" }, Selected = false });
			MyMPs.Add(new Tag { TagEntity = new Authority { EntityName = "Peter Dutton" }, Selected = false });
			MyMPs.Add(new Tag { TagEntity = new Authority { EntityName = "Penny Wong" }, Selected = false });
			MyMPs.Add(new Tag { TagEntity = new Authority { EntityName = "Daniel Andrews" }, Selected = false });
			MyMPs.Add(new Tag { TagEntity = new Authority { EntityName = "Ged Kearney" }, Selected = false });
			MyMPs.Add(new Tag { TagEntity = new Authority { EntityName = "Michael McCormack" }, Selected = false });
			MyMPs.Add(new Tag { TagEntity = new Authority { EntityName = "Mark Dreyfus" }, Selected = false });
			MyMPs.Add(new Tag { TagEntity = new Authority { EntityName = "Michaelia Cash" }, Selected = false });
			*/

			TestCurrentMPs = new ObservableCollection<Entity>();

			ExistingQuestions = new ObservableCollection<Question>();
			ExistingQuestions.Add(
				new Question
				{
					QuestionText = "What is the error rate of the Senate Scanning solution?",
					QuestionSuggester = "Alice",
					QuestionAsker = "",
					DownVotes = 1,
					UpVotes = 2
				});
			ExistingQuestions.Add(
				new Question
				{
					QuestionText = "What is the monthly payment to AWS for COVIDSafe?",
					QuestionSuggester = "Bob",
					QuestionAsker = "",
					DownVotes = 3,
					UpVotes = 1
				});
			ExistingQuestions.Add(
				new Question
				{
					QuestionText =
						"Why did the ABC decide against an opt-in consent model for data sharing with Facebook and Google?",
					QuestionSuggester = "Chloe",
					QuestionAsker = "",
					DownVotes = 1,
					UpVotes = 2
				});
			ExistingQuestions.Add(
				new Question
				{
					QuestionText =
						"What is the government's position on the right of school children to strike for climate?",
					QuestionSuggester = "Darius",
					DownVotes = 1,
					UpVotes = 2
				});


		/*
		StateElectorates = new ObservableCollection<Tag>();
		StateElectorates.Add(new Tag
		{
			TagEntity = new Authority { AuthorityName = "Gembrook"}, 
			Selected = false 
		});
		StateElectorates.Add(new Tag
		{
			TagEntity = new Authority{AuthorityName = "Nepean"}, 
			Selected = false
		});
		StateElectorates.Add(new Tag
		{
			TagEntity = new Authority { AuthorityName = "Sunbury"}, 
			Selected = false 
		});
		StateElectorates.Add(new Tag
		{
			TagEntity = new Authority{AuthorityName = "Brighton"}, 
			Selected = false
		});
		StateElectorates.Add(new Tag
		{
			TagEntity = new Authority { AuthorityName = "Eildon"}, 
			Selected = false 
		});
		StateElectorates.Add(new Tag
		{
			TagEntity = new Authority{AuthorityName = "Ovens Valley"}, 
			Selected = false
		});
		StateElectorates.Add(new Tag
		{
			TagEntity = new Authority { AuthorityName = "Malvern"}, 
			Selected = false
		});
		StateElectorates.Add(new Tag
		{
			TagEntity = new Authority{AuthorityName = "Northcote"}, 
			Selected = false
		});
		StateElectorates.Add(new Tag
		{
			TagEntity = new Authority { AuthorityName = "Gippsland South"}, 
			Selected = false
		});

		FederalElectorates = new ObservableCollection<Tag>();
		FederalElectorates.Add(new Tag
		{
			TagEntity = new Authority { AuthorityName = "Cooper" },
			Selected = false
		});
		FederalElectorates.Add( new Tag
		{
			TagEntity = new Authority { AuthorityName = "Higgins" } ,
			Selected = false
		});
		FederalElectorates.Add ( new Tag
		{
			TagEntity = new Authority { AuthorityName = "Flinders" },
			Selected = false 
		});
		FederalElectorates.Add ( new Tag
		{
			TagEntity = new Authority { AuthorityName = "Isaacs" } ,
			Selected = false
		});
		FederalElectorates.Add ( new Tag
		{
			TagEntity = new Authority { AuthorityName = "Melbourne" },
			Selected = false
		});
		FederalElectorates.Add ( new Tag
		{
			TagEntity = new Authority { AuthorityName = "Mallee"} ,
			Selected = false
		});
		FederalElectorates . Add ( new Tag
		{
			TagEntity = new Authority { AuthorityName = "Indi"} ,
			Selected = false
		});
		FederalElectorates . Add ( new Tag
		{
			TagEntity = new Authority { AuthorityName = "Monash"} ,
			Selected = false
		});
		FederalElectorates . Add ( new Tag
		{
			TagEntity = new Authority { AuthorityName = "Wills"},
			Selected = false
		});
	*/
	}


		// TODO This ToString doesn't really properly convey the state of
		// the ReadingContext, e.g. doesn't reflect registering or knowing your
		// MPs.
		// And it probably isn't necessary to write out all the unselected things.
		
		public override string ToString ()
		{
			return "Keyword: " + Filters.SearchKeyword + '\n' +
			       "TopTen: " + TopTen + '\n' +
			       "Direct Committee: " + GoDirect_Committee + '\n' +
			       "Direct MP: " + GoDirect_MP + '\n' +
			       "Question: " + DraftQuestion + '\n' ;
		}

	}
}
