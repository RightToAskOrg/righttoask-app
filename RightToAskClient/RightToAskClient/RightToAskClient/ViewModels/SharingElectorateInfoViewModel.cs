using RightToAskClient.Models;

namespace RightToAskClient.ViewModels
{
    public class SharingElectorateInfoViewModel : BaseViewModel
    {
        private readonly Registration _registration;
        
        private string _showUserName;
        public string ShowUserName
        {
            get => _showUserName;
            set => SetProperty(ref _showUserName, value);
        }
        
        public SharingElectorateInfoViewModel(Registration registration)
        {
            _registration = registration;
            ShowUserName = registration.uid;
        }

        public SharingElectorateInfoViewModel()
        {
            
        }
        
    }
}