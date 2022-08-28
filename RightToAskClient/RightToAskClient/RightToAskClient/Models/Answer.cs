using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using RightToAskClient.Models.ServerCommsData;

namespace RightToAskClient.Models
{
    public class Answer
    {

        // Should be omitted on sending - filled in by server.
        public string UserAnsweredBy { get; set; } // uid.

        public MP MPAnsweredBy { get; set; }

        public string AnswerText { get; set; }
        
        // For generating app Answer data structure from server data structure (possible nulls etc).
        public Answer(QuestionAnswer ans)
        {
            AnswerText = ans.answer ?? "";
            UserAnsweredBy = ans.answered_by ?? "";
            MPAnsweredBy = new MP(ans.mp);
        }
    }
}