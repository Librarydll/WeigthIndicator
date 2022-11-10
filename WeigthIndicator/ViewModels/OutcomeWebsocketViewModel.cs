using Prism.Commands;
using Prism.Mvvm;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Models.ViewModels;
using WeigthIndicator.Services;

namespace WeigthIndicator.ViewModels
{
    public class OutcomeWebsocketViewModel : ReactiveObject, IDisposable
    {
        private readonly IWebsocketMessageHandler _websocketMessageHandler;
        private readonly IUnityContainer _unityContainer;

        public DelegateCommand BackCommand { get; set; }
        public ObservableCollection<OutcomeItemViewModel> OutcomeItems { get; set; }
        public ReactiveObject ParentViewModel { get; set; }

        public OutcomeWebsocketViewModel(IWebsocketMessageHandler websocketMessageHandler,IUnityContainer unityContainer)
        {
            OutcomeItems = new ObservableCollection<OutcomeItemViewModel>();
            _websocketMessageHandler = websocketMessageHandler;
            _unityContainer = unityContainer;
            _websocketMessageHandler.ReestrAddedEvent += ReestrAddedEvent;
            _websocketMessageHandler.ReestrRemovedEvent += ReestrRemovedEvent;
            BackCommand = new DelegateCommand(ExecuteBackCommand);
        }

        private void ReestrRemovedEvent(Shared.Models.ReestrQrObject reestrQrObject)
        {
            var existed = OutcomeItems.FirstOrDefault(x => x.Id == reestrQrObject.Id);
            if(existed != null)
            {
                Application.Current.Dispatcher.Invoke(() => OutcomeItems.Remove(existed));
            }
        }

        private void ReestrAddedEvent(Shared.Models.ReestrQrObject reestrQrObject)
        {
            var existed = OutcomeItems.FirstOrDefault(x => x.Id == reestrQrObject.Id);
            if (existed == null)
            {
                var model = new OutcomeItemViewModel
                {
                    Count = reestrQrObject.Net,
                    ReestrId = reestrQrObject.Id,
                    Reestr = new Reestr
                    {
                        BarrelNumber = reestrQrObject.BarrelNumber,
                        BatchNumber = reestrQrObject.BatchNumber,
                        Net = reestrQrObject.Net,
                        Recipe = new Recipe { LongNameRu = reestrQrObject.ProductionTitle },
                        PackingDate = reestrQrObject.PackingDate,
                        BarrelStorage = new BarrelStorage { ProductionDate = reestrQrObject.ProductionDate }

                    }
                };
                Application.Current.Dispatcher.Invoke(() => OutcomeItems.Add(model));
            }
        }

        public void Dispose()
        {
            _websocketMessageHandler.ReestrAddedEvent -= ReestrAddedEvent;
            _websocketMessageHandler.ReestrRemovedEvent -= ReestrRemovedEvent;
        }
        private void ExecuteBackCommand()
        {
            _unityContainer.Resolve<NavigationViewModel>().ContentViewModel = ParentViewModel;
        }


        public void OnNavigated(ReactiveObject reactiveObject)
        {
            ParentViewModel = reactiveObject;
        }


    }
}
