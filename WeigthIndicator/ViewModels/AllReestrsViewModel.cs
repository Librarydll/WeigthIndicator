using Prism.Commands;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using WeigthIndicator.Dialog;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Models;
using WeigthIndicator.Store;

namespace WeigthIndicator.ViewModels
{
    public class AllReestrsViewModel : ItemDialogViewModelBase<IEnumerable<Reestr>>
    {
        public const ReestrLocationState InWarehouse = ReestrLocationState.RetrunedBack | ReestrLocationState.InWarehouse;
        private Dictionary<int, Reestr> _selectedReestrs =new Dictionary<int, Reestr>();
        private readonly IReestrDataService _reestrDataService;
        private IEnumerable<Reestr> _reestrs;
        //0-filterbydate;1-filterbystring
        private int _lastSelectedFilter = -1;
        public FilterModel FilterModel { get; set; }
        private IEnumerable<string> _allBatches;
        private bool ignore = false;
        private ObservableCollection<SelectableItem<Reestr>> _reestrsCollection;

        public ObservableCollection<SelectableItem<Reestr>> ReestrsCollection
        {
            get { return _reestrsCollection; }
            set { this.RaiseAndSetIfChanged(ref _reestrsCollection, value); }
        }

        private ObservableCollection<string> _materials;
        public ObservableCollection<string> Materials
        {
            get { return _materials; }
            set { this.RaiseAndSetIfChanged(ref _materials, value); }
        }

        private ObservableCollection<string> _batches;
        public ObservableCollection<string> Batches
        {
            get { return _batches; }
            set { this.RaiseAndSetIfChanged(ref _batches, value); }
        }

        private string _selectedBatch;
        public string SelectedBatch
        {
            get { return _selectedBatch; }
            set
            {
                _selectedBatch = value;
                if (!ignore)
                    SelectedBatchChanged();
            }
        }


        private string _selectedMaterial;
        public string SelectedMaterial
        {
            get { return _selectedMaterial; }
            set
            {
                _selectedMaterial = value;
                if (!ignore)
                    SelectedMaterialChanged();
            }
        }

        [Reactive] public int SelectedPrintViewType { get; set; }
        [Reactive] public int SelectedBarrelType { get; set; }
        public int BarrelCode => SelectedBarrelType - 1;

        public ReactiveCommand<Unit, Unit> FilterCommad { get; }
        public ReactiveCommand<Unit, Unit> FilterBySearchQueryCommand { get; }
        public ReactiveCommand<Reestr, Unit> PrintCommand { get; }
        public ReactiveCommand<Reestr, Unit> EditCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportExcelCommand { get; }
        public ReactiveCommand<Unit, Unit> SelectAllReestr { get; }

        [Reactive] public int ReestrCount { get; set; }

        [Reactive] public double NetTotal { get; set; }
        public double Height { get;  }
        public double Width { get; }
        public AllReestrsViewModel(ModalNavigationStore modalNavigationStore,
            IReestrDataService reestrDataService)
            : base(modalNavigationStore)
        {
            FilterModel = new FilterModel();
            _reestrDataService = reestrDataService;
            FilterCommad = ReactiveCommand.CreateFromTask(ExecuteFilterCommand);

            var canFilter =
                this.WhenAnyValue(x => x.FilterModel.SearchQuery,
                x => !string.IsNullOrWhiteSpace(x));
      
            this.WhenAnyValue(x => x.SelectedBarrelType)
                .Skip(1)
                .SelectMany(FilterCollectionByBarrelType)
                .Subscribe();

            FilterBySearchQueryCommand = ReactiveCommand.CreateFromTask(ExecuteFilterBySearchQueryCommand, canFilter);
            SelectAllReestr = ReactiveCommand.Create(ExecuteSelectAllReestr);
            Height = Application.Current.MainWindow.ActualHeight - 50;
            Width = Application.Current.MainWindow.ActualWidth - 80;
        }

        private void ExecuteSelectAllReestr()
        {
            if (ReestrsCollection != null)
            {
                foreach (var item in ReestrsCollection)
                {
                    item.IsSelected = !item.IsSelected;
                }
            }
        }

        private async Task<Unit> FilterCollectionByBarrelType(int key)
        {
            if (_lastSelectedFilter == 0)
            {
                await ExecuteFilterCommand();
            }
            else
            {
                await ExecuteFilterBySearchQueryCommand();
            }
            return Unit.Default;
        }


        private async Task ExecuteFilterCommand()
        {

            if (FilterModel.IsToDateInclude)
            {
                _reestrs = await _reestrDataService.GetReestrsByDates(FilterModel.FromDate, FilterModel.ToDate, BarrelCode, InWarehouse);
            }
            else
            {
                _reestrs = await _reestrDataService.GetReestrsByDateAndType(FilterModel.FromDate, BarrelCode, InWarehouse);
            }

            IntializeReestr(_reestrs);
            FillCombo();
            _lastSelectedFilter = 0;
        }

        private async Task ExecuteFilterBySearchQueryCommand()
        {
            var regex = new Regex(FilterModel.pattern);
            var match = regex.Match(FilterModel.SearchQuery);
            if (match.Success)
            {
                var from = int.Parse(match.Groups[1].Value);
                var to = int.Parse(match.Groups[3].Value);

                _reestrs = await _reestrDataService.GetReestrsByBarrelNumbers(FilterModel.FromDate, FilterModel.ToDate, from, to, BarrelCode, InWarehouse);
            }
            else
            {
                _reestrs = await _reestrDataService.GetReestrsByBatchNumber(FilterModel.FromDate, FilterModel.ToDate, FilterModel.SearchQuery, BarrelCode, InWarehouse);
            }
            IntializeReestr(_reestrs);
            _lastSelectedFilter = 1;
            FillCombo();
        }

        private void SelectedBatchChanged()
        {
            IEnumerable<Reestr> reestrs = _reestrs;
            if (!string.IsNullOrWhiteSpace(SelectedMaterial))
            {
                reestrs = _reestrs.Where(x => x.Recipe.ShortName == SelectedMaterial);

            }
            if (!string.IsNullOrWhiteSpace(SelectedBatch))
            {
                reestrs = reestrs.Where(x => x.BatchNumber == SelectedBatch);
            }
            IntializeReestr(reestrs);
        }
        private void SelectedMaterialChanged()
        {
            IEnumerable<Reestr> reestrs = _reestrs;
            if (!string.IsNullOrWhiteSpace(SelectedMaterial))
            {
                reestrs = _reestrs.Where(x => x.Recipe.ShortName == SelectedMaterial);
            }
            ignore = true;
            Batches = new ObservableCollection<string>(reestrs.Select(x => x.BatchNumber).Distinct());
            ignore = false;
            IntializeReestr(reestrs);
        }

        public void FillCombo()
        {
            string all = "";
            _allBatches = ReestrsCollection.Select(x => x.Item.BatchNumber).Distinct();
            Batches = new ObservableCollection<string>
            {
                all
            };
            Batches.AddRange(_allBatches);
            var materials = ReestrsCollection.Select(x => x.Item.Recipe.ShortName).Distinct();
            Materials = new ObservableCollection<string>
            {
                all
            };
            Materials.AddRange(materials);

        }

        private void IntializeReestr(IEnumerable<Reestr> reestrs)
        {

            if (ReestrsCollection != null)
            {
                foreach (var item in ReestrsCollection)
                {
                    item.SelectedChanged -= ItemSelectedChanged;
                }

            }
            ReestrsCollection?.Clear();
            ReestrsCollection = ReestrsCollection ?? new ObservableCollection<SelectableItem<Reestr>>();

            foreach (var item in reestrs)
            {
                var it = new SelectableItem<Reestr> { Item = item };
                ReestrsCollection.Add(it);
                it.SelectedChanged += ItemSelectedChanged;
            }
        }

        private void ItemSelectedChanged(SelectableItem<Reestr> selectableItem)
        {
            if (selectableItem.IsSelected)
            {
                if (!_selectedReestrs.ContainsKey(selectableItem.Item.Id))
                {
                    _selectedReestrs.Add(selectableItem.Item.Id, selectableItem.Item);
                }
            }
            else
            {
                _selectedReestrs.Remove(selectableItem.Item.Id);
            }

            NetTotal = _selectedReestrs.Values.Sum(x => x.Net);
            ReestrCount = _selectedReestrs.Count;
        }

        public override Task ExecuteConfirmCommand()
        {
            CloseDialogOnSuccess(_selectedReestrs.Values);
            Dispose();
            return Task.CompletedTask;
        }

        public override void ExecuteCancelCommand()
        {
            Dispose();
            base.ExecuteCancelCommand();
        }

        public void Dispose()
        {
            if (ReestrsCollection != null)
            {
                foreach (var item in ReestrsCollection)
                {
                    item.SelectedChanged -= ItemSelectedChanged;
                }
            }
        }
    }
}
