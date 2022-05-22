using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using WeigthIndicator.Store;

namespace WeigthIndicator.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly NavigationStore _navigationStore;
        private readonly ModalNavigationStore _modalNavigationStore;

        public ReactiveObject CurrentViewModel => _navigationStore.CurrentViewModel;
        public ReactiveObject CurrentModalViewModel => _modalNavigationStore.CurrentViewModel;
        public bool IsOpen => _modalNavigationStore.IsOpen;


        public MainViewModel(NavigationStore navigationStore, ModalNavigationStore modalNavigationStore)
        {
            _navigationStore = navigationStore;
            _modalNavigationStore = modalNavigationStore;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _modalNavigationStore.CurrentViewModelChanged += OnCurrentModalViewModelChanged;
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
