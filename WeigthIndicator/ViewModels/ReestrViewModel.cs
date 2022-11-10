using Newtonsoft.Json;
using Prism.Services.Dialogs;
using QRCoder;
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
using System.Windows.Documents;
using System.Windows.Forms;
using WeigthIndicator.Core.Excel;
using WeigthIndicator.Core.Print;
using WeigthIndicator.Dialog;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Factory;
using WeigthIndicator.Mapper;
using WeigthIndicator.Models;
using WeigthIndicator.Shared.Services;
using WeigthIndicator.Views;

namespace WeigthIndicator.ViewModels
{
    public class ReestrViewModel : ReactiveObject
    {
        private string _filename = string.Empty;
        private readonly IReestrDataService _reestrDataService;
        private readonly IDialogNavigationService _dialogNavigationService;
        private IEnumerable<Reestr> _reestrs;
        //0-filterbydate;1-filterbystring
        private int _lastSelectedFilter = -1;
        public FilterModel FilterModel { get; set; }
        private IEnumerable<string> _allBatches;
        private bool ignore = false;
        private ObservableCollection<Reestr> _reestrsCollection;

        public ObservableCollection<Reestr> ReestrsCollection
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
                if(!ignore)
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

        private readonly ObservableAsPropertyHelper<int> _reestrCount;
        public int ReestrCount => _reestrCount.Value;

        private readonly ObservableAsPropertyHelper<double> _netTotal;
        public double NetTotal => _netTotal.Value;

        public ReestrViewModel(IReestrDataService reestrDataService,IDialogNavigationService dialogNavigationService)
        {
            FilterModel = new FilterModel();
            _reestrDataService = reestrDataService;
            _dialogNavigationService = dialogNavigationService;
            FilterCommad = ReactiveCommand.CreateFromTask(ExecuteFilterCommand);


            var canFilter =
                this.WhenAnyValue(x => x.FilterModel.SearchQuery,
                x => !string.IsNullOrWhiteSpace(x));


            var reestrCount = this.WhenAnyValue(x => x.ReestrsCollection.Count);
            _reestrCount = reestrCount
                          .Select(x => x)
                          .ToProperty(this, x => x.ReestrCount);

            _netTotal = reestrCount
                .Select(x => ReestrsCollection.Sum(z => z.Net))
                .ToProperty(this, x => x.NetTotal);
            this.WhenAnyValue(x => x.SelectedBarrelType)
                .Skip(1)
                .SelectMany(FilterCollectionByBarrelType)
                .Subscribe();

            FilterBySearchQueryCommand = ReactiveCommand.CreateFromTask(ExecuteFilterBySearchQueryCommand, canFilter);
            PrintCommand = ReactiveCommand.Create<Reestr>(ExecutePrintCommand);
            EditCommand = ReactiveCommand.Create<Reestr>(ExecuteEditCommand);
            ExportExcelCommand = ReactiveCommand.Create(ExecuteExportExcelCommand);
        }

        private async Task<Unit> FilterCollectionByBarrelType(int key)
        {
            if (_lastSelectedFilter == 0)
            {
                await  ExecuteFilterCommand();
            }
            else
            {
                await  ExecuteFilterBySearchQueryCommand();
            }
            return Unit.Default;
        }

        private void ExecuteExportExcelCommand()
        {
            var filename = _filename + ".xlsx";

            ExcelHelper.ExportExcel(ReestrsCollection, filename);
        }

        private void ExecuteEditCommand(Reestr reestr)
        {

            _dialogNavigationService.ShowDialog<ReestrEditViewModel, Reestr>(reestr, callback =>
            {
                if (callback.IsSuccess)
                {
                    Reestr updated = callback.Value;

                    Task.Run(async () =>
                    {
                        await _reestrDataService.UpdateReestr(updated);
                    })
                .ContinueWith(x =>
                {
                    if (x.Status == TaskStatus.RanToCompletion)
                    {
                        var r = ReestrsCollection.FirstOrDefault(z => z.Id == reestr.Id);
                        r.ReestrState = updated.ReestrState;
                        r.Customer = updated.Customer;
                        r.CustomerId = updated.CustomerId;
                        r.Net = updated.Net;
                        r.Note = updated.Note;
                    }
                });
                }
            });
        }

        private void ExecutePrintCommand(Reestr reestr)
        {
            var printViewType = (PrintViewType)Enum.Parse(typeof(PrintViewType), SelectedPrintViewType.ToString());
            var printInitialize = PrintPreviewFactory.GetPrintView(printViewType);
            var robj = new Models.ViewModels.ReestrObject(reestr);
            FlowDocument flowDoc = printInitialize.InitializeFlow(robj);
            PrintHelper.Prints(flowDoc, reestr.PackingDate.ToString("dd.MM.yyyy"));

        }

        private async Task ExecuteFilterCommand()
        {

            if (FilterModel.IsToDateInclude)
            {
                _reestrs = await _reestrDataService.GetReestrsByDates(FilterModel.FromDate, FilterModel.ToDate, BarrelCode);
                _filename = FilterModel.FromDate.ToString("dd.MM.yyyy") + "_" + FilterModel.ToDate.ToString("dd.MM.yyyy");
            }
            else
            {
                _reestrs = await _reestrDataService.GetReestrsByDateAndType(FilterModel.FromDate, BarrelCode);
                _filename = FilterModel.FromDate.ToString("dd.MM.yyyy");
            }

            ReestrsCollection = new ObservableCollection<Reestr>(_reestrs);
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

                _reestrs = await _reestrDataService.GetReestrsByBarrelNumbers(FilterModel.FromDate,FilterModel.ToDate, from, to, BarrelCode);
                _filename = from.ToString() + "_" + to.ToString();
            }
            else
            {
                _reestrs = await _reestrDataService.GetReestrsByBatchNumber(FilterModel.FromDate, FilterModel.ToDate, FilterModel.SearchQuery, BarrelCode);
                _filename = FilterModel.SearchQuery.Replace("/",".");
            }

            ReestrsCollection = new ObservableCollection<Reestr>(_reestrs);
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
            ReestrsCollection = new ObservableCollection<Reestr>(reestrs);
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
            ReestrsCollection = new ObservableCollection<Reestr>(reestrs);
        }

        public void FillCombo()
        {
            string all = "";
            _allBatches = ReestrsCollection.Select(x => x.BatchNumber).Distinct();
            Batches = new ObservableCollection<string>
            {
                all
            };
            Batches.AddRange(_allBatches);
            var materials = ReestrsCollection.Select(x => x.Recipe.ShortName).Distinct();
            Materials = new ObservableCollection<string>
            {
                all
            };
            Materials.AddRange(materials);

           // SelectedMaterial = Materials.FirstOrDefault();
           // SelectedBatch = Batches.FirstOrDefault();
        }

    }
}
