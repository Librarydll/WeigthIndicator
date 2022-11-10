using Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Unity;
using WeigthIndicator.Dialog;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Models;
using WeigthIndicator.Store;
using WeigthIndicator.ViewModels.Common;

namespace WeigthIndicator.ViewModels
{
    public class OutcomeViewModel : ItemPaginationViewModelBase<Outcome>
    {
        private readonly IUnityContainer _unityContainer;
        private readonly IOutcomeDataService _outcomeDataService;
        public DelegateCommand NavigateToCreateOutcomeCommand { get; set; }
        public FilterModel DateTimeFilter { get; set; }
        public DelegateCommand NavigateOutcomeWebsocketCommand { get; set; }

        public DelegateCommand<Outcome> UpdateOutcomeCommand { get; set; }
        public DelegateCommand<Outcome> DeleteOutcomeCommand { get; set; }
        public OutcomeViewModel(
            IDialogNavigationService dialogNavigationService,
            IUnityContainer unityContainer,
            IOutcomeDataService outcomeDataService)
        {
            NavigateToCreateOutcomeCommand = new DelegateCommand(ExecuteNavigateToCreateOutcomeCommand);
            UpdateOutcomeCommand = new DelegateCommand<Outcome>(ExecuteUpdateOutcomeCommand);
            DeleteOutcomeCommand = new DelegateCommand<Outcome>(async (m) => await ExecuteDeleteOutcomeCommand(m));
            NavigateOutcomeWebsocketCommand = new DelegateCommand(ExecuteNavigateOutcomeWebsocketCommand);
            _unityContainer = unityContainer;
            _outcomeDataService = outcomeDataService;
            DateTimeFilter = new FilterModel();
            Task.Run(async () => await Initialize());
        }

     

        private async Task ExecuteDeleteOutcomeCommand(Outcome outcome)
        {
            var mb = MessageBox.Show("Вы действительно хотите удалить данную отгрузку?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mb == MessageBoxResult.No) return;

            await _outcomeDataService.DeleteOutcome(outcome.Id);
            Application.Current.Dispatcher.Invoke(() => Collection.Remove(outcome));
        }

        private void ExecuteUpdateOutcomeCommand(Outcome outcome)
        {
            NavigateToView(outcome);
        }

        private void ExecuteNavigateToCreateOutcomeCommand()
        {
            NavigateToView(null);
        }

        private void NavigateToView(Outcome outcome)
        {
            var navigation = _unityContainer.Resolve<NavigationViewModel>();
            var vm = _unityContainer.Resolve<CreateUpdateOutcomeViewModel>();
            navigation.ContentViewModel = vm;
            vm.OnNavigated(this, outcome);
        }
        public override async Task Initialize()
        {
            var dto = await _outcomeDataService.GetOutcomes(DateTimeFilter.FromDate, DateTimeFilter.ToDate, PaginationHelper.CurrentPage, PaginationHelper.ItemsPerPage, SearchingString);
            PaginationHelper.ItemsCount = dto.ItemsCount;
            Collection = new ObservableCollection<Outcome>(dto.Collection);
        }

        private void ExecuteNavigateOutcomeWebsocketCommand()
        {
            var navigation = _unityContainer.Resolve<NavigationViewModel>();
            var vm = _unityContainer.Resolve<OutcomeWebsocketViewModel>();
            navigation.ContentViewModel = vm;
            vm.OnNavigated(this);
        }
    }
}
