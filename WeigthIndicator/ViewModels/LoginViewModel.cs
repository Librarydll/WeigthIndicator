using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Store;

namespace WeigthIndicator.ViewModels
{
    public class LoginViewModel : ReactiveObject
    {
        private readonly IUserDataService _userDataService;
        private readonly IContainerProvider _containerProvider;
        private readonly UserStore _userStore;
        [Reactive] public string Message { get; set; }
        public bool HasError => !string.IsNullOrWhiteSpace(Message);
        public string Password { get; set; }
        public string UserName { get; set; }
        public DelegateCommand LoginCommand { get; set; }
        public DelegateCommand<KeyEventArgs> PwdKeyDownCommand { get; set; }
        public LoginViewModel(
            IUserDataService userDataService,
            IContainerProvider containerProvider,
            UserStore userStore)
        {
            _userDataService = userDataService;
            _containerProvider = containerProvider;
            _userStore = userStore;
            LoginCommand = new DelegateCommand(async () => await ExecuteLoginCommand());
            PwdKeyDownCommand = new DelegateCommand<KeyEventArgs>(ExecutePwdKeyDownCommand);

            this.WhenAnyValue(x => x.Message)
                .Subscribe(z => this.RaisePropertyChanged(nameof(HasError)));
        }


        private void ExecutePwdKeyDownCommand(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Task.Run(async () => await ExecuteLoginCommand()).ConfigureAwait(false);
            }
        }

        private async Task ExecuteLoginCommand()
        {
            Message = null;
            try
            {
                var authenticationResult = await _userDataService.Login(UserName, Password);

                if (authenticationResult.IsSuccess)
                {
                    _userStore.CurrentUser = authenticationResult.User;
                    Navigate();
                }
                else
                {
                    Message = authenticationResult.Error;
                }
            }
            catch (HttpRequestException)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    Message = "Нет подключение с сервером";
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void Navigate()
        {
            var vm = _containerProvider.Resolve<NavigationViewModel>();
            var store = _containerProvider.Resolve<NavigationStore>();
            store.CurrentViewModel = vm;
        }
    }
}
