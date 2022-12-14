using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using RightToAskClient.Helpers;
using Xamarin.Forms;

namespace RightToAskClient.Models
{
    /*
     * Stores information about this user's responses to questions: which ones
     * they've up-voted, down-voted, dismissed, flagged.
     */
    public class QuestionResponseRecords
    {
	    private HashSet<string> _upvotedQuestionIDs = new HashSet<string>();
	    private HashSet<string> _downvotedQuestionIDs= new HashSet<string>();
	    private HashSet<string> _reportedQuestionIDs = new HashSet<string>();
	    private HashSet<string> _dismissedQuestionIDs = new HashSet<string>();
	    private bool _isInitialised;

		public void Init()
		{
			if (!_isInitialised)
			{
				// Retrieve from preferences.
				_upvotedQuestionIDs = retrieveHashSetFromPreferences(Constants.UpvotedQuestions);
				_downvotedQuestionIDs = retrieveHashSetFromPreferences(Constants.DownvotedQuestions);
				_dismissedQuestionIDs = retrieveHashSetFromPreferences(Constants.DismissedQuestions);
				_reportedQuestionIDs = retrieveHashSetFromPreferences(Constants.ReportedQuestions);
				_isInitialised = true;
			}
		}


		public void AddDownvotedQuestion(string questionID)
		{
			_downvotedQuestionIDs.Add(questionID);
			storeHashSetInPreferences(Constants.DownvotedQuestions, _downvotedQuestionIDs);
		}

		public bool IsAlreadyDownvoted(string questionID) => _downvotedQuestionIDs.Contains(questionID);
		
		public void AddUpvotedQuestion(string questionID)
		{
			_upvotedQuestionIDs.Add(questionID);
			storeHashSetInPreferences(Constants.UpvotedQuestions, _upvotedQuestionIDs);
		}

		public bool IsAlreadyUpvoted(string questionID) => _upvotedQuestionIDs.Contains(questionID);
		
		public void AddReportedQuestion(string questionID)
		{
			_reportedQuestionIDs.Add(questionID);
			storeHashSetInPreferences(Constants.ReportedQuestions, _reportedQuestionIDs);
		}

		public bool IsAlreadyReported(string questionID) => _reportedQuestionIDs.Contains(questionID);
		
		public void AddDismissedQuestion(string questionID)
		{
			_dismissedQuestionIDs.Add(questionID);
			storeHashSetInPreferences(Constants.DismissedQuestions, _dismissedQuestionIDs);
		}

		public bool IsAlreadyDismissed(string questionID) => _dismissedQuestionIDs.Contains(questionID);

		private void storeHashSetInPreferences(string key, HashSet<string> hashSet)
		{
			try
			{
				var hashSetString = JsonSerializer.Serialize(hashSet);
				XamarinPreferences.shared.Set(key, hashSetString);
			}
			catch (Exception e)
			{
				Debug.WriteLine("Error storing "+key+" to preferences: "+e.Message);
			}
		}
		private HashSet<string> retrieveHashSetFromPreferences(string key)
		{
			var retrievedSet = new HashSet<string>();
			
			var retrievedString = XamarinPreferences.shared.Get(key, "");
			if (!String.IsNullOrEmpty(retrievedString))
			{
				try
				{
					retrievedSet = JsonSerializer.Deserialize<HashSet<string>>(retrievedString);
				}
				catch (Exception e)
				{
					Debug.WriteLine("Error deserialising "+key+":"+e.Message);
				}
			}

			return retrievedSet ?? new HashSet<string>();;
		}
    }
}