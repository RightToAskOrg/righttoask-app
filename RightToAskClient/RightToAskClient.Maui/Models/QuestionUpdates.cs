using System.Collections.Generic;
using System.Linq;
using RightToAskClient.Maui.Models.ServerCommsData;

namespace RightToAskClient.Maui.Models
{
    public class QuestionUpdates
    {
        public RTAPermissions WhoShouldAnswerPermissions = RTAPermissions.NoChange;
        public RTAPermissions WhoShouldAskPermissions = RTAPermissions.NoChange;
        public List<QuestionAnswer> NewAnswers = new List<QuestionAnswer>();
        public List<HansardLink> NewHansardLinks = new List<HansardLink>();
        public string NewBackground = "";

        public string QuestionID;
        public string Version;

        public QuestionUpdates(string questionID=null, string version=null)
        {
            QuestionID = questionID ?? "";
            Version = version ?? "";
        }

        public bool AnyUpdates =>
            !string.IsNullOrEmpty(NewBackground) 
            || WhoShouldAnswerPermissions != RTAPermissions.NoChange
            || WhoShouldAskPermissions != RTAPermissions.NoChange
            || NewHansardLinks.Any()
            || NewAnswers.Any();
    }
}