using ReactiveUI;
using System;
using Unity;
using WeigthIndicator.Store;

namespace WeigthIndicator.Dialog
{
    public class DialogNavigationService : IDialogNavigationService
    {
        private readonly IUnityContainer _serviceProvider;

        public DialogNavigationService(IUnityContainer serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void ShowDialog<TViewModel, TModel>(Action<ModelDialogParameter<TModel>> callback)
            where TViewModel : ReactiveObject, IModal<TModel>
        {
            var vm = ShowDialogInner<TViewModel>();

            vm.DialogClosed += m =>
            {
                callback?.Invoke(m);
            };
        }
        public void ShowDialog<TViewModel, TStore, TModel>(Action<ModelDialogParameter<TModel>> callback)
            where TViewModel : ReactiveObject, IModal<TModel>
            where TStore : ModalNavigationStore
        {
            var vm = ShowDialogInner<TViewModel, TStore>();

            vm.DialogClosed += m =>
            {
                callback?.Invoke(m);
            };
        }
        public void ShowDialog<TViewModel, TModel>(TModel model, Action<ModelDialogParameter<TModel>> callback)
            where TViewModel : ReactiveObject, IModal<TModel>
        {
            var vm = ShowDialogInner<TViewModel>();

            vm.DialogClosed += m =>
            {
                callback?.Invoke(m);
            };
            vm.OnDialogOpen(model);
        }
        public void ShowDialog<TViewModel, TStore, TModel>(TModel model, Action<ModelDialogParameter<TModel>> callback)
            where TViewModel : ReactiveObject, IModal<TModel>
            where TStore : ModalNavigationStore
        {
            var vm = ShowDialogInner<TViewModel, TStore>();

            vm.DialogClosed += m =>
            {
                callback?.Invoke(m);
            };
            vm.OnDialogOpen(model);
        }
        public void ShowDialog<TViewModel, TModel>(TModel model)
          where TViewModel : ReactiveObject, IModal<TModel>
        {
            var vm = ShowDialogInner<TViewModel>();

            vm.OnDialogOpen(model);
        }
        public void ShowDialog<TViewModel, TStore, TModel>(TModel model)
          where TViewModel : ReactiveObject, IModal<TModel>
          where TStore : ModalNavigationStore
        {
            var vm = ShowDialogInner<TViewModel, TStore>();

            vm.OnDialogOpen(model);
        }



        private TViewModel ShowDialogInner<TViewModel>()
            where TViewModel : ReactiveObject
        {
            var vm = _serviceProvider.Resolve<TViewModel>();
            var store = _serviceProvider.Resolve<ModalNavigationStore>();
            store.CurrentViewModel = vm;
            return vm;
        }
        private TViewModel ShowDialogInner<TViewModel, TStore>()
          where TViewModel : ReactiveObject
          where TStore : ModalNavigationStore
        {
            var vm = _serviceProvider.Resolve<TViewModel>();
            var store = _serviceProvider.Resolve<TStore>();
            store.CurrentViewModel = vm;
            return vm;
        }

        public void ShowDialog<TViewModel>() where TViewModel : ReactiveObject
        {
            var vm = _serviceProvider.Resolve<TViewModel>();
            var store = _serviceProvider.Resolve<ModalNavigationStore>();
            store.CurrentViewModel = vm;
        }

        public void CloseDialog<TStore>() where TStore : ModalNavigationStore
        {
            var store = _serviceProvider.Resolve<TStore>();
            store.Close();
        }
    }
}
