using Prism.Commands;
using Prism.Mvvm;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WeigthIndicator.Domain.Models.Common;
using WeigthIndicator.Models.Infrastructure;

namespace WeigthIndicator.ViewModels.Common
{
    public class ItemPaginationViewModelBase<TModel> : ReactiveObject
        where TModel : BaseEntity
    {
        private ObservableCollection<TModel> _collection;
        public ObservableCollection<TModel> Collection
        {
            get => _collection;
            set => this.RaiseAndSetIfChanged(ref _collection, value);
        }
        private string _searchingStirng;

        public string SearchingString
        {
            get { return _searchingStirng; }
            set { this.RaiseAndSetIfChanged(ref _searchingStirng, value); }
        }
        public PaginationHelper PaginationHelper { get; set; } = new PaginationHelper();
        public IEnumerable<ItemPerPage> ItemPerPages => ItemPerPage.GetItemPerPages();
        public DelegateCommand NextPage { get; set; }
        public DelegateCommand PreviousPage { get; set; }
        public DelegateCommand SearchCommand { get; set; }
        public DelegateCommand<KeyEventArgs> SearchKeyDownCommand { get; set; }

        public ItemPaginationViewModelBase()
        {
            NextPage = new DelegateCommand(ExecuteNextPage).ObservesCanExecute(() => PaginationHelper.HasNext);
            PreviousPage = new DelegateCommand(ExecutePreviousPage).ObservesCanExecute(() => PaginationHelper.HasPrevious);
            SearchCommand = new DelegateCommand(ExecuteSearchCommand);
            SearchKeyDownCommand = new DelegateCommand<KeyEventArgs>(ExecuteSearchKeyDownCommand);
            PaginationHelper.ItemsPerPageChanged += PaginationPerPageItemsChanged;
        }
        private void ExecuteSearchKeyDownCommand(KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ExecuteSearchCommand();
            }
        }
        private void ExecuteSearchCommand()
        {
            PaginationHelper.CurrentPage = 1;
            Task.Run(async () =>
            {
                await Initialize();
            });
        }
        public void PaginationPerPageItemsChanged()
        {
            Task.Run(async () => await Initialize());
        }

        private async void ExecutePreviousPage()
        {
            PaginationHelper.CurrentPage--;
            await Initialize();
        }

        private async void ExecuteNextPage()
        {
            PaginationHelper.CurrentPage++;
            await Initialize();
        }

        public virtual Task Initialize()
        {
            return Task.CompletedTask;
            //var dto = await _itemApiService.Get(PaginationHelper.CurrentPage, PaginationHelper.ItemsPerPage, SearchingString);
            //PaginationHelper.ItemsCount = dto.ItemsCount;
            //Collection = new ObservableCollection<TModel>(dto.Items);
            //await OnInitializeCompleted();
        }
        //public override void Dispose()
        //{
        //    PaginationHelper.ItemsPerPageChanged -= PaginationPerPageItemsChanged;
        //    base.Dispose();
        //}


    }
}
