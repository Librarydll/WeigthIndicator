using Prism.Commands;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WeigthIndicator.Dialog;
using WeigthIndicator.Domain.Models.Common;

namespace WeigthIndicator.ViewModels.Common
{
    public class ItemsListViewModelBase<TModel, DialogViewModel> : ItemPaginationViewModelBase<TModel>
        where TModel : BaseEntity
        where DialogViewModel : ReactiveObject, IModal<TModel>
    {
        protected readonly IDialogNavigationService _dialogNavigationService;

        public DelegateCommand CreateCommand { get; set; }
        public DelegateCommand<TModel> UpdateCommand { get; set; }
        public DelegateCommand<TModel> DeleteCommand { get; set; }

        public ItemsListViewModelBase(
            IDialogNavigationService dialogNavigationService) 
        {
            _dialogNavigationService = dialogNavigationService;
            Task.Run(async () => await Initialize());
            CreateCommand = new DelegateCommand(ExecuteCreateCommand);
            UpdateCommand = new DelegateCommand<TModel>(ExecuteUpdateCommand);
            DeleteCommand = new DelegateCommand<TModel>(async (c) => await ExecuteDeleteCommand(c));
        }

        public virtual void ExecuteCreateCommand()
        {
            _dialogNavigationService.ShowDialog<DialogViewModel, TModel>(default, OnModalClosed);

        }

        private void OnModalClosed(ModelDialogParameter<TModel> model)
        {
            if (model.IsSuccess)
            {
                Task.Run(async () =>
                {
                    await Initialize();
                });
            }
        }

        public virtual void ExecuteUpdateCommand(TModel model)
        {
            _dialogNavigationService.ShowDialog<DialogViewModel, TModel>(model, OnModalClosedUpdated);

        }

        protected void OnModalClosedUpdated(ModelDialogParameter<TModel> model)
        {
            if (model.IsSuccess)
            {
                Task.Run(async () =>
                {
                    await Initialize();
                });
            }
        }

        public virtual  Task ExecuteDeleteCommand(TModel model)
        {
            return Task.CompletedTask;
            //var msg = MessageBox.Show("Удалить", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Information);
            //if (msg == MessageBoxResult.No) return;
            //try
            //{
            //    var isSuccess = await _itemApiService.Delete(model);
            //    OnModelDeletedEventHandler(isSuccess);
            //    if (isSuccess)
            //        await Initialize();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);

            //}
        }
    
    }
}
