using System.Text.Json.Serialization;

namespace RightToAskClient.Models
{
    public class NewQuestionCommand
    {
        [JsonPropertyName("question_text")]
        public string question_text { get; set; }

        //TODO: Add the QuestionNonDefiningFields here.
    }
}