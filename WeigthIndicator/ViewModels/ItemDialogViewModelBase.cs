using Prism.Commands;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Dialog;
using WeigthIndicator.Store;

namespace WeigthIndicator.ViewModels
{
    public class ItemDialogViewModelBase<TEntity> : ReactiveObject, IModal<TEntity>
    {
        protected readonly ModalNavigationStore _storeBase;

        public event Action<ModelDialogParameter<TEntity>> DialogClosed;

        [Reactive] public string Title { get; set; }
        public virtual bool CanConfirm => true;
        public DelegateCommand ConfirmCommand { get; }
        public DelegateCommand CancelCommand { get; }
        public ItemDialogViewModelBase(ModalNavigationStore storeBase)
        {
            ConfirmCommand = new DelegateCommand(async () => await ExecuteConfirmCommand()).ObservesCanExecute(() => CanConfirm);
            CancelCommand = new DelegateCommand(ExecuteCancelCommand);
            _storeBase = storeBase;
        }

        public virtual void ExecuteCancelCommand()
        {
            CloseDialogOnCancel();
        }

        public virtual Task ExecuteConfirmCommand()
        {
            return Task.CompletedTask;
        }
        public virtual void OnDialogOpen(TEntity model)
        {

        }

        public void CloseDialogOnSuccess(TEntity entity)
        {
            CloseDialog(ModelDialogParameter<TEntity>.Success(entity));
        }
        public void CloseDialogOnCancel()
        {
            CloseDialog(new ModelDialogParameter<TEntity> { IsSuccess = false });
        }
        public void CloseDialog(ModelDialogParameter<TEntity> parameter)
        {
            _storeBase.Close();
            DialogClosed?.Invoke(parameter);
        }

    }
}
