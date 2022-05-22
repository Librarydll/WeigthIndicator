using ReactiveUI;
using System;
using WeigthIndicator.Store;

namespace WeigthIndicator.Dialog
{
    public interface IDialogNavigationService
    {
        void ShowDialog<TViewModel>()
           where TViewModel : ReactiveObject;


        void ShowDialog<TViewModel, TModel>(Action<ModelDialogParameter<TModel>> callback)
            where TViewModel : ReactiveObject, IModal<TModel>;
        void ShowDialog<TViewModel, TModel>(TModel model)
            where TViewModel : ReactiveObject, IModal<TModel>;
        void ShowDialog<TViewModel, TModel>(TModel model, Action<ModelDialogParameter<TModel>> callback)
            where TViewModel : ReactiveObject, IModal<TModel>;

        void ShowDialog<TViewModel, TStore, TModel>(Action<ModelDialogParameter<TModel>> callback)
            where TViewModel : ReactiveObject, IModal<TModel>
            where TStore : ModalNavigationStore;
        void ShowDialog<TViewModel, TStore, TModel>(TModel model)
            where TViewModel : ReactiveObject, IModal<TModel>
            where TStore : ModalNavigationStore;
        void ShowDialog<TViewModel, TStore, TModel>(TModel model, Action<ModelDialogParameter<TModel>> callback)
            where TViewModel : ReactiveObject, IModal<TModel>
            where TStore : ModalNavigationStore;

        void CloseDialog<TStore>() where TStore : ModalNavigationStore;

        // void ShowDialog<TViewModel>() where TViewModel : ReactiveObject;
    }
}
