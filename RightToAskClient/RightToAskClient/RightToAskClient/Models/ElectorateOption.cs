using System;

namespace RightToAskClient.Models
{
    public class ElectorateOption
    {
        private string _electorateTitle;
        private string _electorateValue;
        private bool _isPublic;

        public ElectorateOption()
        {
            _electorateTitle = "";
            _electorateValue = "";
            _isPublic = false;
        }

        public ElectorateOption(string electorateTitle, string electorateValue, bool isPublic)
        {
            _electorateTitle = electorateTitle;
            _electorateValue = electorateValue;
            _isPublic = isPublic;
        }

        public string ElectorateTitle
        {
            get => _electorateTitle;
            set => _electorateTitle = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string ElectorateValue
        {
            get => _electorateValue;
            set => _electorateValue = value ?? throw new ArgumentNullException(nameof(value));
        }

        public bool IsPublic
        {
            get => _isPublic;
            set => _isPublic = value;
        }
    }
}