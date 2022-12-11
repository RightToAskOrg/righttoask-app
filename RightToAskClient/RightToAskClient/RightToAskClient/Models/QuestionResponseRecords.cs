using System;
using System.Collections.Generic;

namespace RightToAskClient.Models
{
    /*
     * Stores information about this user's responses to questions: which ones
     * they've up-voted, down-voted, dismissed, flagged.
     */
    public class QuestionResponseRecords
    {
		private HashSet<string> UpvotedQuestionIDs { get; set; } = new HashSet<string>();
		private HashSet<string> DownvotedQuestionIDs { get; set; } = new HashSet<string>();
		private HashSet<string> ReportedQuestionIDs { get; set; } = new HashSet<string>();
		private HashSet<string> RemovedQuestionIDs { get; set; } = new HashSet<string>();

		public QuestionResponseRecords()
		{
			// Retrieve from preferences.
			// retrieve from set
			throw new NotImplementedException();
		}

		public void AddDownvotedQuestion(string questionID)
		{
			// Add to Set
			// Store in Preferences
			// retrieve from set
			throw new NotImplementedException();
		}

		public bool IsAlreadyDownvoted(string questionID)
		{
			// retrieve from set
			// retrieve from set
			throw new NotImplementedException();
		}
		
		public void AddUpvotedQuestion(string questionID)
		{
			// Add to Set
			// Store in Preferences
			// retrieve from set
			throw new NotImplementedException();
		}

		public bool IsAlreadyUpvoted(string questionID)
		{
			// retrieve from set
			// retrieve from set
			throw new NotImplementedException();
		}
		
		public void AddReportedQuestion(string questionID)
		{
			// Add to Set
			// Store in Preferences
			// retrieve from set
			throw new NotImplementedException();
		}

		public bool IsAlreadyReported(string questionID)
		{
			// retrieve from set
			// retrieve from set
			throw new NotImplementedException();
		}
		
		public void AddDismissedQuestion(string questionID)
		{
			// Add to Set
			// Store in Preferences
			// retrieve from set
			throw new NotImplementedException();
		}

		public bool IsAlreadyDismissed(string questionID)
		{
			// retrieve from set
			throw new NotImplementedException();
		}
    }
}