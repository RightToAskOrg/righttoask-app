using RightToAskClient.Maui.Models.ServerCommsData;

namespace RightToAskClient.Maui.Models
{
    public class Answer
    {
        // uid.
        public string UserAnsweredBy { get; } = "";

        public MP MPAnsweredBy { get; } = new MP();

        public string AnswerText { get; } = "";

        // For generating app Answer data structure from server data structure (possible nulls etc).
        public Answer(QuestionAnswer ans)
        {
            AnswerText = ans.answer ?? "";
            UserAnsweredBy = ans.answered_by ?? "";
            MPAnsweredBy = ParliamentData.FindMPOrMakeNewOne(ans.mp);
        }
    }
}