using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WeigthIndicator.Scanner.Exceptions;
using WeigthIndicator.Scanner.Services;
using WeigthIndicator.Scanner.Store;
using WeigthIndicator.Scanner.Views;
using WeigthIndicator.Shared.Models;
using Xamarin.Forms;
using ZXing;

namespace WeigthIndicator.Scanner.ViewModels
{
    public class OutcomeViewModel : BaseViewModel
    {
        public int ReestrsCount => ReestrsStore.ReestrsCount;
    
        public double TotalNet => ReestrsStore.TotalNet;
        public ReestrsStore ReestrsStore { get; }
        public Command ResetCommand { get; set; }
        public Command CompleteCommand { get; set; }
        public Command<Result> ScanResultCommand { get; set; }
        public Command NavigateReestrsListPageCommand { get; set; }
        public Command ReconnectCommand { get; set; }
        public INavigation Navigation { get; }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }
        public OutcomeViewModel(INavigation navigation)
        {
            ResetCommand = new Command(ExecuteResetCommand);
            CompleteCommand = new Command(ExecuteCompleteCommand);
            ScanResultCommand = new Command<Result>(ExecuteScanResultCommand);
            NavigateReestrsListPageCommand = new Command(ExecuteNavigateReestrsListPageCommand);
            ReconnectCommand = new Command(ExecuteReconnectCommand);
            ReestrsStore = DependencyService.Resolve<ReestrsStore>();
            Navigation = navigation;
            IsConnected = DependencyService.Resolve<IWebsocketMessageHandler>().IsConnected;
            DependencyService.Resolve<IWebsocketMessageHandler>().ConnectionChanged += state =>
            {
                Device.BeginInvokeOnMainThread(() => IsConnected = state);
            };
        }

        private void ExecuteReconnectCommand(object obj)
        {
            DependencyService.Resolve<IWebsocketClientService>().Reconnect();
        }

        private void ExecuteNavigateReestrsListPageCommand(object obj)
        {
            Navigation.PushAsync(new ReestrsListPage());
        }

        private void ExecuteScanResultCommand(Result result)
        {
            try
            {
                if(ReestrsStore.TryAddReestr(result.Text,out ReestrQrObject reestrQrObject))
                {
                    OnPropertyChanged(nameof(TotalNet));
                    OnPropertyChanged(nameof(ReestrsCount));
                    var model = PrepareModelForScannedReestr(reestrQrObject);
                    DependencyService.Resolve<IWebsocketMessageHandler>().SendMessage(model);
                }                
            }
            catch (ReestrNetException)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Значение нетта", "У данной бочки нетто меньше еденицы", "Ок");

                });
            }
            catch (ReestrAlreadyExistException)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                   await Application.Current.MainPage.DisplayAlert("Дубликат", "Данная бочка уже отсканированна", "Ок");

                });
            }
        }

        private void ExecuteCompleteCommand(object obj)
        {
            DependencyService.Resolve<IWebsocketMessageHandler>().CreateOutcomeResult += OutcomeViewModel_CreateOutcomeResult;

            var webSocket = DependencyService.Resolve<IWebsocketMessageHandler>();
            var model = PrepareModelForOutcome();
            webSocket.SendMessage(model);
        }

        private void OutcomeViewModel_CreateOutcomeResult(CreateOutcomeResult result)
        {
            try
            {
                if (!result.IsSuccuess)
                {
                    var text = $"Не удалось выполнить операцию\n{result.Reestr.ProductionTitle} Номер бочки - {result.Reestr.BarrelNumber}\nСостояние - вне склада";
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Application.Current.MainPage.DisplayAlert("Ошибка", text, "Ок");

                    });
                }
                else
                {
                    Reset();
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                DependencyService.Resolve<IWebsocketMessageHandler>().CreateOutcomeResult -= OutcomeViewModel_CreateOutcomeResult;
            }
        }

        private async void ExecuteResetCommand(object obj)
        {
            var result = await Application.Current.MainPage.DisplayAlert("Сброс", "Вы действительно хотите сбросить?\nДанные будут утеряны", "Да", "Нет");
            if (!result) return;
            Reset();
        }

        private WebsocketRootModel PrepareModelForOutcome()
        {
            var userStore = DependencyService.Resolve<UserStore>();

            CreateOutcomeModel model = new CreateOutcomeModel
            {
                Reestrs = ReestrsStore.Reestrs,
                Comment = ReestrsStore.Comment,
                Title = ReestrsStore.Title,
                UserId = userStore.User.Id
            };
            return new WebsocketRootModel
            {
                MessageType = MessageType.CreateOutcomeModel,
                CreateOutcomeModel = model
            };
        }

        private WebsocketRootModel PrepareModelForScannedReestr(ReestrQrObject reestrQrObject)
        {
            return new WebsocketRootModel
            {
                MessageType = MessageType.ReestrAdded,
                Reestr = reestrQrObject
            };
        }
       
        private void Reset()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                ReestrsStore.Reset();
                OnPropertyChanged(nameof(TotalNet));
                OnPropertyChanged(nameof(ReestrsCount));
            });
        }
    }
}
