﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

// Data structure for serialising question data to send to the server,
// either for new questions or for updating existing questions.
// Note that these fields should have the same names and types as the equivalent
// fields in QuestionReceiveFromServer, though the set of fields is not
// exactly the same.
namespace RightToAskClient.Models.ServerCommsData
{
    public class QuestionSendToServer
    {

        // Question-defining fields.
        // The only question-defining field we need is question_text.
        // Note that author isn't necessary because the server knows
        // which user uploaded it, and timestamp isn't sent by the client.
        [JsonPropertyName("question_text")]
        public string? question_text { get; set; }

        // bookkeeping fields
        [JsonPropertyName("question_id")]
        public string? question_id { get; set; }
        
        [JsonPropertyName("version")]
        public string? version { get; set; }
        
        // non-defining fields
        [JsonPropertyName("background")]
        public string? background { get; set; }
        
        // TODO Note that the entity that should ask the question might be a
        // committee or other RTA participant, not necessarily an MP.
        // Not clear whether we want a separate field for each of these possibilities
        // - possibly we do.
        [JsonPropertyName("mp_who_should_ask_the_question")]
        public List<PersonID>? mp_who_should_ask_the_question { get; set; }

        [JsonPropertyName("who_should_ask_the_question_permissions")]
        public RTAPermissions who_should_ask_the_question_permissions { get; set; }

        [JsonPropertyName("entity_who_should_answer_the_question")]
        public List<PersonID>? entity_who_should_answer_the_question { get; set; }

        [JsonPropertyName("who_should_answer_the_question_permissions")]
        public RTAPermissions who_should_answer_the_question_permissions { get; set; }

        // TODO: Check whether this data structure lines up with the 'QuestionAnswer' type 
        // - possibly we want to implement that explicitly.
        [JsonPropertyName("answers")]
        public List<Tuple<string, Person, MP>>? answers { get; set; } 
        
        // TODO This needs to be interpreted/stored carefully, because we need 'false'
        // to mean 'the user has seen it but didn't fully approve' and to distinguish this
        // from 'the user hasn't seen or hasn't rated the answer'
        // Consider whether it'd make more sense to have an enum.
        [JsonPropertyName("answer_accepted")]
        public bool? answer_accepted { get; set; }
        
        [JsonPropertyName("hansard_link")]
        public List<Uri> hansard_link { get; set; } 
        
        [JsonPropertyName("is_followup_to")]
        public string? is_followup_to { get; set; } 
        
        public QuestionSendToServer () {}
        public QuestionSendToServer(Question question)
        {
            if (!String.IsNullOrEmpty(question.QuestionText))
            {
                question_text = question.QuestionText;
            }

            if (!String.IsNullOrEmpty(question.Background))
            {
                background = question.Background;
            }

            TranscribeQuestionFiltersForUpload(question.Filters);
            
            //TODO: Add other structures.
        }
        
        // Returns true if there were any non-empty filters present.
        public bool TranscribeQuestionFiltersForUpload(FilterChoices filters)
        {
            // We take the (duplicate-removing) union of selected MPs, because at the moment the UI doesn't remove 
            // your MPs from the 'other MPs' list and the user may have selected the same MP in both categories.
            var MPAnswerers = filters.SelectedAnsweringMPsNotMine.Union(filters.SelectedAnsweringMPsMine);
            var MPanswerersServerData = MPAnswerers.Select(mp => new PersonID(new MPId(mp)));
            
            // Add authorities, guaranteed not to be duplicates.
            List<PersonID> answerers = MPanswerersServerData.
                Concat(filters.SelectedAuthorities.Select(a => new PersonID(a))).ToList();
            entity_who_should_answer_the_question = answerers;

            // Entities who should raise the question - currently just MPs and committees.
            // TODO add other users.
            var MPAskers = filters.SelectedAskingMPsNotMine.Union(filters.SelectedAskingMPsMine);
            var MPAskersServerData = MPAskers.Select(mp => new PersonID(new MPId(mp)));
            
            // Add committees, guaranteed not to be duplicates.
            var CommitteeAskers = filters.SelectedCommittees;
            // var CommitteeAskersServerData = CommitteeAskers.Select(c => new PersonID(new CommitteeInfo(c)));
            
            List<PersonID> askers = MPAskersServerData.
                Concat(CommitteeAskers.Select(c => new PersonID(new CommitteeInfo(c)))).ToList();
            mp_who_should_ask_the_question = askers;

            return askers.Any() || answerers.Any();
        }
        public bool ValidateNewQuestion()
        {
            bool isValid = false;
            // just needs question text for new questions
            if (!string.IsNullOrEmpty(question_text))
            {
                isValid = true;
            }
            return isValid;
        }

        public bool ValidateUpdateQuestion()
        {
            bool isValid = false;
            // needs more fields to update an existing question
            if (!string.IsNullOrEmpty(question_text)
                && !string.IsNullOrEmpty(question_id)
                && !string.IsNullOrEmpty(version))
            {
                isValid = true;
            }
            return isValid;
        }
    }
}
