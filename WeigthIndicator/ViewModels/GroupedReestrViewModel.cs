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
using WeigthIndicator.Core.Excel;
using WeigthIndicator.Core.Print;
using WeigthIndicator.Domain.DTO;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Factory;
using WeigthIndicator.Models;

namespace WeigthIndicator.ViewModels
{
    public class GroupedReestrViewModel : ReactiveObject
    {
        private string _filename = string.Empty;
        private readonly IReestrDataService _reestrDataService;
        private IEnumerable<GroupedReestr> _reestrs;
        //0-filterbydate;1-filterbystring
        public FilterModel FilterModel { get; set; }

        private ObservableCollection<GroupedReestr> _reestrsCollection;

        public ObservableCollection<GroupedReestr> ReestrsCollection
        {
            get { return _reestrsCollection; }
            set { this.RaiseAndSetIfChanged(ref _reestrsCollection, value); }
        }

        public ReactiveCommand<Unit, Unit> FilterCommad { get; }
        public ReactiveCommand<Unit, Unit> ExportExcelCommand { get; }

        private readonly ObservableAsPropertyHelper<int> _reestrCount;
        public int ReestrCount => _reestrCount.Value;

        private readonly ObservableAsPropertyHelper<double> _netTotal;
        public double NetTotal => _netTotal.Value;

        public GroupedReestrViewModel(IReestrDataService reestrDataService)
        {
            FilterModel = new FilterModel();
            _reestrDataService = reestrDataService;
            FilterCommad = ReactiveCommand.CreateFromTask(ExecuteFilterCommand);


            var canFilter =
                this.WhenAnyValue(x => x.FilterModel.SearchQuery,
                x => !string.IsNullOrWhiteSpace(x));


            var reestrCount = this.WhenAnyValue(x => x.ReestrsCollection.Count);
            _reestrCount = reestrCount
                          .Select(x => ReestrsCollection.Sum(z=>z.BarrelsCount))
                          .ToProperty(this, x => x.ReestrCount);

            _netTotal = reestrCount
                .Select(x => ReestrsCollection.Sum(z => z.TotalNet))
                .ToProperty(this, x => x.NetTotal);
        

            ExportExcelCommand = ReactiveCommand.Create(ExecuteExportExcelCommand);
        }


        private void ExecuteExportExcelCommand()
        {
            var filename ="grouped_"+ _filename + ".xlsx";

            ExcelHelperForGrouped.ExportExcel(ReestrsCollection, filename);
        }

     
        private async Task ExecuteFilterCommand()
        {

            if (FilterModel.IsToDateInclude)
            {
                _reestrs = await _reestrDataService.GetGroupedReestrs(FilterModel.FromDate, FilterModel.ToDate);
                _filename = FilterModel.FromDate.ToString("dd.MM.yyyy") + "_" + FilterModel.ToDate.ToString("dd.MM.yyyy");
            }
            else
            {
                _reestrs = await _reestrDataService.GetGroupedReestrs(FilterModel.FromDate);
                _filename = FilterModel.FromDate.ToString("dd.MM.yyyy");
            }

            ReestrsCollection = new ObservableCollection<GroupedReestr>(_reestrs);
        }
      
    }

}
