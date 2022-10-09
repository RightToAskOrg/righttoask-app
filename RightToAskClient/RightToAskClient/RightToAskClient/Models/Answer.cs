using RightToAskClient.Models.ServerCommsData;

namespace RightToAskClient.Models
{
    public class Answer
    {
        // uid.
        public string UserAnsweredBy { get; private set; } = "";

        public MP MPAnsweredBy { get; private set; } = new MP();

        public string AnswerText { get; private set; } = "";

        // For generating app Answer data structure from server data structure (possible nulls etc).
        public Answer(QuestionAnswer ans)
        {
            AnswerText = ans.answer ?? "";
            UserAnsweredBy = ans.answered_by ?? "";
            MPAnsweredBy = ParliamentData.FindMPOrMakeNewOne(ans.mp);
        }
    }
}