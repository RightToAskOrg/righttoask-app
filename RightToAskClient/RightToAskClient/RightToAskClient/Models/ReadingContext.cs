using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using RightToAskClient.Models;
using Xamarin.Forms;

namespace RightToAskClient
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
		public IndividualParticipant ThisParticipant { get; set; }

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
		
		public ObservableCollection<Tag> Departments { get; set; }

		// public ObservableCollection<Tag> SelectableAuthorities { get; set; }

		public ObservableCollection<Tag> StateElectorates { get; set; }
		public ObservableCollection<Tag> FederalElectorates { get; set; }
		


		// At the moment, this simply populates the reading context with a
		// hardcoded set of "existing" questions.
		private void InitializeDefaultSetup()
		{
			ThisParticipant = new IndividualParticipant();
			Filters = new FilterChoices();

			Departments = new ObservableCollection<Tag>();
			Departments.Add(new Tag { TagEntity = new Entity { EntityName = "Environment" }, Selected = false });
			Departments.Add(new Tag { TagEntity = new Entity { EntityName = "Home Affairs" }, Selected = false });
			Departments.Add(new Tag { TagEntity = new Entity { EntityName = "Defence" }, Selected = false });
			Departments.Add(new Tag { TagEntity = new Entity { EntityName = "Health" }, Selected = false });
			Departments.Add(new Tag { TagEntity = new Entity { EntityName = "Treasury" }, Selected = false });
			Departments.Add(new Tag { TagEntity = new Entity { EntityName = "Human Services" }, Selected = false });
			Departments.Add(new Tag
				{ TagEntity = new Entity { EntityName = "Innovation, Industry and Science" }, Selected = false });
			Departments.Add(new Tag { TagEntity = new Entity { EntityName = "Communications" }, Selected = false });

			/*
			MyMPs = new ObservableCollection<Tag>();
			MyMPs.Add(new Tag { TagEntity = new Entity { EntityName = "Janet Rice" }, Selected = false });
			MyMPs.Add(new Tag { TagEntity = new Entity { EntityName = "Danny O'Brien" }, Selected = false });
			MyMPs.Add(new Tag { TagEntity = new Entity { EntityName = "Peter Dutton" }, Selected = false });
			MyMPs.Add(new Tag { TagEntity = new Entity { EntityName = "Penny Wong" }, Selected = false });
			MyMPs.Add(new Tag { TagEntity = new Entity { EntityName = "Daniel Andrews" }, Selected = false });
			MyMPs.Add(new Tag { TagEntity = new Entity { EntityName = "Ged Kearney" }, Selected = false });
			MyMPs.Add(new Tag { TagEntity = new Entity { EntityName = "Michael McCormack" }, Selected = false });
			MyMPs.Add(new Tag { TagEntity = new Entity { EntityName = "Mark Dreyfus" }, Selected = false });
			MyMPs.Add(new Tag { TagEntity = new Entity { EntityName = "Michaelia Cash" }, Selected = false });
			*/

			TestCurrentMPs = new ObservableCollection<Entity>();
			TestCurrentMPs.Add(new Entity { EntityName = "Janet Rice" });
			TestCurrentMPs.Add(new Entity { EntityName = "Danny O'Brien" });
			TestCurrentMPs.Add(new Entity { EntityName = "Peter Dutton" });
			TestCurrentMPs.Add(new Entity { EntityName = "Penny Wong" });
			TestCurrentMPs.Add(new Entity { EntityName = "Daniel Andrews" });
			TestCurrentMPs.Add(new Entity { EntityName = "Ged Kearney" });
			TestCurrentMPs.Add(new Entity { EntityName = "Michael McCormack" });
			TestCurrentMPs.Add(new Entity { EntityName = "Mark Dreyfus" });
			
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

			
			StateElectorates = new ObservableCollection<Tag>();
			StateElectorates.Add(new Tag
			{
				TagEntity = new Entity { EntityName = "Gembrook"}, 
				Selected = false 
			});
			StateElectorates.Add(new Tag
			{
				TagEntity = new Entity{EntityName = "Nepean"}, 
				Selected = false
			});
			StateElectorates.Add(new Tag
			{
				TagEntity = new Entity { EntityName = "Sunbury"}, 
				Selected = false 
			});
			StateElectorates.Add(new Tag
			{
				TagEntity = new Entity{EntityName = "Brighton"}, 
				Selected = false
			});
			StateElectorates.Add(new Tag
			{
				TagEntity = new Entity { EntityName = "Eildon"}, 
				Selected = false 
			});
			StateElectorates.Add(new Tag
			{
				TagEntity = new Entity{EntityName = "Ovens Valley"}, 
				Selected = false
			});
			StateElectorates.Add(new Tag
			{
				TagEntity = new Entity { EntityName = "Malvern"}, 
				Selected = false
			});
			StateElectorates.Add(new Tag
			{
				TagEntity = new Entity{EntityName = "Northcote"}, 
				Selected = false
			});
			StateElectorates.Add(new Tag
			{
				TagEntity = new Entity { EntityName = "Gippsland South"}, 
				Selected = false
			});

			FederalElectorates = new ObservableCollection<Tag>();
			FederalElectorates.Add(new Tag
			{
				TagEntity = new Entity { EntityName = "Cooper" },
				Selected = false
			});
			FederalElectorates.Add( new Tag
			{
				TagEntity = new Entity { EntityName = "Higgins" } ,
				Selected = false
			});
			FederalElectorates.Add ( new Tag
			{
				TagEntity = new Entity { EntityName = "Flinders" },
				Selected = false 
			});
			FederalElectorates.Add ( new Tag
			{
				TagEntity = new Entity { EntityName = "Isaacs" } ,
				Selected = false
			});
			FederalElectorates.Add ( new Tag
			{
				TagEntity = new Entity { EntityName = "Melbourne" },
				Selected = false
			});
			FederalElectorates.Add ( new Tag
			{
				TagEntity = new Entity { EntityName = "Mallee"} ,
				Selected = false
			});
			FederalElectorates . Add ( new Tag
			{
				TagEntity = new Entity { EntityName = "Indi"} ,
				Selected = false
			});
			FederalElectorates . Add ( new Tag
			{
				TagEntity = new Entity { EntityName = "Monash"} ,
				Selected = false
			});
			FederalElectorates . Add ( new Tag
			{
				TagEntity = new Entity { EntityName = "Wills"},
				Selected = false
			});
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
			       "Question: " + DraftQuestion + '\n' +
			       "Departments: " + Departments + '\n' ;
		}

	}
}
