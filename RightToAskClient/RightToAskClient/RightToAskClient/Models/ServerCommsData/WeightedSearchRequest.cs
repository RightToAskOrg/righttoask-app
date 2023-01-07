using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

// Information necessary for a search request, including characteristics from QuestionSendToServer
// (question_text and who should answer or ask the question), plus 
// weights: relative importance of search/sort criteria,
// page: from .. to, indicating which part of the ordered list we are requesting.

namespace RightToAskClient.Models.ServerCommsData
{
    public class WeightedSearchRequest
    {
        [JsonPropertyName("question_text")] 
        public string? question_text { get; set; }

        [JsonPropertyName("mp_who_should_ask_the_question")]
        public List<PersonID>? mp_who_should_ask_the_question { get; set; }

        [JsonPropertyName("entity_who_should_answer_the_question")]
        public List<PersonID>? entity_who_should_answer_the_question { get; set; }
 
        [JsonPropertyName("page")]
        public QuestionListPage? page { get; set; }
        
        [JsonPropertyName("weights")]
        public Weights? weights { get; set; }
    }
}
