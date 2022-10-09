using RightToAskClient.Models.ServerCommsData;

namespace RightToAskClient.Models
{
    public class Answer
    {
        private string _userAnsweredBy = "";

        // uid.
        public string UserAnsweredBy   
        {
            get => _userAnsweredBy;
            private set => _userAnsweredBy = value;
        } 

        private MP _mpAnsweredBy = new MP();
        public MP MPAnsweredBy
        {
            get => _mpAnsweredBy;
            private set => _mpAnsweredBy = value;
        }

        private string _answerText = "";
        public string AnswerText
        {
            get => _answerText;
            private set => _answerText = value;
        }

        // For generating app Answer data structure from server data structure (possible nulls etc).
        public Answer(QuestionAnswer ans)
        {
            AnswerText = ans.answer ?? "";
            UserAnsweredBy = ans.answered_by ?? "";
            MPAnsweredBy = ParliamentData.FindMPOrMakeNewOne(ans.mp);
        }
    }
}