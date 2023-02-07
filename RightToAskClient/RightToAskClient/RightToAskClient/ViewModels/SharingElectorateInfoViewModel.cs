using System.Diagnostics;
using System.Threading.Tasks;
using RightToAskClient.CryptoUtils;
using RightToAskClient.Helpers;
using RightToAskClient.HttpClients;
using RightToAskClient.Models;
using RightToAskClient.Resx;
using RightToAskClient.Views.Popups;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace RightToAskClient.ViewModels
{
    public class SharingElectorateInfoViewModel : BaseViewModel
    {
        private readonly Registration? _registration;

        public IAsyncCommand BackCommand { get; }
        public IAsyncCommand SaveAndFinishCommand { get; }

        private bool _ableToFinish;

        public bool AbleToFinish
        {
            get => _ableToFinish;
            set => SetProperty(ref _ableToFinish, value);
        }
        
        public SharingElectorateInfoViewModel(Registration registration) : this()
        {
            _registration = registration;
            AbleToFinish = true;
        }

        public SharingElectorateInfoViewModel()
        {
            BackCommand = new AsyncCommand(async () => { await Application.Current.MainPage.Navigation.PopAsync(); });

            SaveAndFinishCommand = new AsyncCommand(async () =>
            {
                RegistrationViewModel.SaveRegistrationToPreferences(_registration);
                Debug.Assert(_registration.registrationStatus == RegistrationStatus.NotRegistered);

                _registration.public_key = ClientSignatureGenerationService.MyPublicKey;

                // Check that the registration info we're about to send to the server is valid.
                var regTest = _registration.IsValid();
                if (regTest.Success)
                {
                    await SendNewUserToServer();
                }
                // If registration info is invalid, prompt the user to fix it up.
                else
                {
                    var errorMessage = AppResources.InvalidRegistration;
                    if (regTest is ErrorResult errorResult)
                    {
                        errorMessage = errorResult.Message;
                    }

                    var popup = new OneButtonPopup(errorMessage, AppResources.OKText);
                    await Application.Current.MainPage.Navigation.ShowPopupAsync(popup);
                }
            });
        }

        private async Task SendNewUserToServer()
        {
            var httpResponse = await RTAClient.RegisterNewUser(_registration);
            var httpValidation = RTAClient.ValidateHttpResponse(httpResponse, "Server Signature Verification");
            ReportLabelText = httpValidation.errorMessage;
            if (httpValidation.isValid)
            {
                _registration.registrationStatus = RegistrationStatus.Registered;
                XamarinPreferences.shared.Set(Constants.IsRegistered, _registration.IsRegistered);

                // if the response seemed successful, put it in more common terms for the user.
                if (ReportLabelText.Contains("Success"))
                {
                    ReportLabelText = AppResources.AccountCreationSuccessResponseText;
                }

                // Now we're registered, we can't change our UID - we can only update the other fields.
                Title = AppResources.EditYourAccountTitle;
                PopupLabelText = AppResources.EditAccountPopupText;
                // pop back to the QuestionDetailsPage after the account is created
                var navigation = Application.Current.MainPage.Navigation;

                // remove 3 pages
                // current - sharing electorate page
                // registration view
                // code of conduct view
                navigation.RemovePage(navigation.NavigationStack[navigation.NavigationStack.Count - 2]);
                navigation.RemovePage(navigation.NavigationStack[navigation.NavigationStack.Count - 2]);
                navigation.RemovePage(navigation.NavigationStack[navigation.NavigationStack.Count - 2]);
                await navigation.PopAsync();
            }
        }
    }
}