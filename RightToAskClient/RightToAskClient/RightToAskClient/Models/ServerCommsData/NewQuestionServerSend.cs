using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RightToAskClient.Models.ServerCommsData
{
    public class NewQuestionServerSend
    {
        [JsonPropertyName("question_text")]
        public string question_text { get; set; }
        //[JsonPropertyName("question_writer")]
        //public string question_writer { get; set; }
        //[JsonPropertyName("upload_timestamp")]
        //public DateTime upload_timestamp { get; set; }

        // non-defining fields
        [JsonPropertyName("background")]
        public string background { get; set; }
        //[JsonPropertyName("mp_who_should_ask_the_questions")]
        //public List<Entity> mp_who_should_ask_the_questions { get; set; }
        //[JsonPropertyName("entity_who_should_answer_the_quetions")]
        //public List<Entity> entity_who_should_answer_the_quetions { get; set; }
        //[JsonPropertyName("answer")]
        //public List<Tuple<string, Person, MP>> answer { get; set; } // List<string,answerer>
        //[JsonPropertyName("answer_accepted")]
        //public bool answer_accepted { get; set; }
        //[JsonPropertyName("hansard_link")]
        //public List<string> hansard_link { get; set; } // list<url>
        [JsonPropertyName("is_followup_to")]
        public string is_followup_to { get; set; } // questionID
        //[JsonPropertyName("keywords")]
        //public List<string> keywords { get; set; }
        //[JsonPropertyName("category")]
        //public List<string> category { get; set; }
        //[JsonPropertyName("expiry_date")]
        //public DateTime expiry_date { get; set; }

        // bookkeeping fields
        //[JsonPropertyName("question_id")]
        //public string question_id { get; set; }

    }
}
