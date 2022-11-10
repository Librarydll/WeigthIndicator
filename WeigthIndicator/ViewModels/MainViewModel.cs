using Prism.Commands;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using WebSocketSharp.Server;
using WeigthIndicator.Services;
using WeigthIndicator.Store;

namespace WeigthIndicator.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly NavigationStore _navigationStore;
        private readonly ModalNavigationStore _modalNavigationStore;
        private readonly IWebsocketServer _webscoketServer;

        public ReactiveObject CurrentViewModel => _navigationStore.CurrentViewModel;
        public ReactiveObject CurrentModalViewModel => _modalNavigationStore.CurrentViewModel;
        public bool IsOpen => _modalNavigationStore.IsOpen;

        public DelegateCommand WindowsLoaded { get; set; }
        public MainViewModel(
            NavigationStore navigationStore, 
            ModalNavigationStore modalNavigationStore,
            IWebsocketServer webscoketServer)
        {
            _navigationStore = navigationStore;
            _modalNavigationStore = modalNavigationStore;
            _webscoketServer = webscoketServer;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _modalNavigationStore.CurrentViewModelChanged += OnCurrentModalViewModelChanged;
            WindowsLoaded = new DelegateCommand(ExecuteWindowsLoaded);
        }

        private void ExecuteWindowsLoaded()
        {   
            _webscoketServer.Start();
        }

        private void OnCurrentModalViewModelChanged()
        {
            this.RaisePropertyChanged(nameof(CurrentModalViewModel));
            this.RaisePropertyChanged(nameof(IsOpen));
        }

        private void OnCurrentViewModelChanged()
        {
            this.RaisePropertyChanged(nameof(CurrentViewModel));
        }

    }
}
