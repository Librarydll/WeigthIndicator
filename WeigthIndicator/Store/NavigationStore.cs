using Prism.Mvvm;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeigthIndicator.Store
{
    public class NavigationStore
    {
        private ReactiveObject _currentViewModel;
        public ReactiveObject CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
               // _currentViewModel?.Dispose();
                _currentViewModel = value;
                OnCurrentViewModelChanged();
            }
        }

        public event Action CurrentViewModelChanged;
        public event Action CurrentViewModelClosed;
        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
        public bool IsOpen => CurrentViewModel != null;

        public void Close()
        {
            CurrentViewModel = null;
            CurrentViewModelClosed?.Invoke();
        }
    }
}
