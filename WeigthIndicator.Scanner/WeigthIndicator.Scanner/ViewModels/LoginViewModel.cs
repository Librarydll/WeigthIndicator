using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Scanner.Helper;
using WeigthIndicator.Scanner.Models;
using WeigthIndicator.Scanner.Services;
using WeigthIndicator.Scanner.Store;
using WeigthIndicator.Scanner.Views;
using WeigthIndicator.Shared.Models;
using Xamarin.Forms;

namespace WeigthIndicator.Scanner.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        public Command LoginCommand { get; }
        public Command SaveServerCommand { get; }

        public string Login { get; set; } = "admin";
        public string Password { get; set; } = "123";
        public string Server { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
            SaveServerCommand = new Command(async ()=> await ExecuteSaveServerCommand());
            Server = AppSettingHelper.LoadApplicationProperty();

       

        }

        private async Task ExecuteSaveServerCommand()
        {
            if (!string.IsNullOrWhiteSpace(Server))
            {
                await AppSettingHelper.SaveApplicationProperty(Server);
                DependencyService.Resolve<IWebsocketClientService>().ChangeServerUrl(Server.CreateWebsocketAddress());
                DependencyService.Resolve<IWebsocketClientService>().Reconnect();

            }
        }

        private void OnLoginClicked(object obj)
        {
            var wb = DependencyService.Resolve<IWebsocketMessageHandler>();
            DependencyService.Resolve<IWebsocketClientService>().Connect();

            var store = DependencyService.Resolve<UserStore>();
            var root = new WebsocketRootModel
            {
                MessageType = MessageType.Authorize,
                AuthorizeModel = new AuthorizeModel
                {
                    Login = Login,
                    Password = Password
                }
            };

            store.User = new User
            {
                FullName ="",
                Id = 1
            };

            Navigate();

            //   wb.SendMessage(root);
            //wb.AuthorizationResult += auth =>
            //{
            //    if (auth.IsSuccess)
            //    {
            //        store.User = new User
            //        {
            //            FullName = auth.FIO,
            //            Id = auth.Id
            //        };

            //        Navigate();
            //    }
            //    else
            //    {
            //        ErrorMessage = auth.ErrorMessage;
            //        OnPropertyChanged(nameof(ErrorMessage));
            //        OnPropertyChanged(nameof(HasError));
            //    }
            //};
           
        }
        private void Navigate()
        {
            Device.BeginInvokeOnMainThread(async()=> await Shell.Current.GoToAsync($"//{nameof(OutcomePage)}"));
        }
    }
}
