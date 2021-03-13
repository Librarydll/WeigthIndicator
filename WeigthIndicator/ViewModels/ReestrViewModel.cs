using Prism.Services.Dialogs;
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
using System.Windows.Forms;
using WeigthIndicator.Core.Excel;
using WeigthIndicator.Core.Print;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Factory;
using WeigthIndicator.Models;
using WeigthIndicator.Views;

namespace WeigthIndicator.ViewModels
{
    public class ReestrViewModel:ReactiveObject
    {
        private string _filename = string.Empty;
        private readonly IReestrDataService _reestrDataService;
        private readonly IDialogService _dialogService;

        public FilterModel FilterModel { get; set; }


        private ObservableCollection<Reestr> _reestrsCollection;

        public ObservableCollection<Reestr> ReestrsCollection
        {
            get { return _reestrsCollection; }
            set { this.RaiseAndSetIfChanged(ref _reestrsCollection, value); }
        }

        [Reactive] public int SelectedPrintViewType { get; set; }

        public ReactiveCommand<Unit, Unit> FilterCommad { get; }
        public ReactiveCommand<Unit, Unit> FilterBySearchQueryCommand { get; }
        public ReactiveCommand<Reestr, Unit> PrintCommand { get; }
        public ReactiveCommand<Reestr, Unit> EditCommand { get; }
        public ReactiveCommand<Unit, Unit> ExportExcelCommand { get; }

        private readonly ObservableAsPropertyHelper<int> _reestrCount;
        public int ReestrCount => _reestrCount.Value;

        private readonly ObservableAsPropertyHelper<double> _netTotal;
        public double NetTotal => _netTotal.Value;

        public ReestrViewModel(IReestrDataService reestrDataService,IDialogService dialogService)
        {
            FilterModel = new FilterModel();
            _reestrDataService = reestrDataService;
            _dialogService = dialogService;
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

            FilterBySearchQueryCommand = ReactiveCommand.CreateFromTask(ExecuteFilterBySearchQueryCommand, canFilter);
            PrintCommand = ReactiveCommand.Create<Reestr>(ExecutePrintCommand);
            EditCommand = ReactiveCommand.Create<Reestr>(ExecuteEditCommand);
            ExportExcelCommand = ReactiveCommand.Create(ExecuteExportExcelCommand);
        }

        private void ExecuteExportExcelCommand()
        {
            var filename = _filename + ".xlsx";

            ExcelHelper.ExportExcel(ReestrsCollection, filename);
        }

        private void ExecuteEditCommand(Reestr reestr)
        {
            var param = new DialogParameters();
            param.Add("model", reestr);
            Reestr updated = null;
            _dialogService.ShowDialog("ReestrEditView", param, x =>
            {
                if (x.Result == ButtonResult.OK)
                {
                    updated = x.Parameters.GetValue<Reestr>("model");
                }
            });

            if (updated != null)
            {
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
        }

        private void ExecutePrintCommand(Reestr reestr)
        {
            var printViewType = (PrintViewType)Enum.Parse(typeof(PrintViewType), SelectedPrintViewType.ToString());
            var printInitialize = PrintPreviewFactory.GetPrintView(printViewType);

            var flowDoc = printInitialize.InitializeFlow(reestr);

            PrintHelper.Prints(flowDoc, reestr.PackingDate.ToString("dd.MM.yyyy"));
        }

        private async Task ExecuteFilterCommand()
        {
            IEnumerable<Reestr> reestrs;

            if (FilterModel.IsToDateInclude)
            {
                reestrs = await _reestrDataService.GetReestrsByDates(FilterModel.FromDate, FilterModel.ToDate);
                _filename = FilterModel.FromDate.ToString("dd.MM.yyyy") + "_" + FilterModel.ToDate.ToString("dd.MM.yyyy");
            }
            else
            {
                reestrs = await _reestrDataService.GetReestrsByDate(FilterModel.FromDate);
                _filename = FilterModel.FromDate.ToString("dd.MM.yyyy");
            }

            ReestrsCollection = new ObservableCollection<Reestr>(reestrs);

        }

        private async Task ExecuteFilterBySearchQueryCommand()
        {
            IEnumerable<Reestr> reestrs;
            var regex = new Regex(FilterModel.pattern);
            var match = regex.Match(FilterModel.SearchQuery);
            if (match.Success)
            {
                var from = int.Parse(match.Groups[1].Value);
                var to = int.Parse(match.Groups[3].Value);

                reestrs = await _reestrDataService.GetReestrsByBarrelNumbers(from, to);
                _filename = from.ToString() + "_" + to.ToString();
            }
            else
            {
                reestrs = await _reestrDataService.GetReestrsByBatchNumber(FilterModel.SearchQuery);
                _filename = FilterModel.SearchQuery.Replace("/",".");
            }

            ReestrsCollection = new ObservableCollection<Reestr>(reestrs);

        }

    }
}
