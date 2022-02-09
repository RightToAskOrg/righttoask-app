using RightToAskClient.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace RightToAskClient.ViewModels
{
    public class QuestionViewModel : BaseViewModel
    {
        static private QuestionViewModel _instance;
        static public QuestionViewModel Instance { get => _instance ??= new QuestionViewModel(); }

        private Question _question;
        public Question Question
        {
            get => _question;
            set => SetProperty(ref _question, value);
        }

        private bool _isNewQuestion;
        public bool IsNewQuestion
        {
            get => _isNewQuestion;
            set => SetProperty(ref _isNewQuestion, value);
        }

        public QuestionViewModel()
        {
            // set defaults
            Question = new Question();
            IsNewQuestion = true;
        }
    }
}
