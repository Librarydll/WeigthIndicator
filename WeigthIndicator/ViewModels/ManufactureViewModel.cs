using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Core.Print;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Factory;

namespace WeigthIndicator.ViewModels
{
    public class ManufactureViewModel : ReactiveObject
    {
        private IManufactureDataService _manufactureDataService;

        [Reactive] public Manufacture Manufacture { get; set; } = new Manufacture();
        [Reactive] public ObservableCollection<Manufacture> ManufacturesCollection { get; set; }
        public ReactiveCommand<Unit, Manufacture> CreateManufacture { get; }
        public ReactiveCommand<Manufacture, Unit> PrintCommand { get; set; }
        [Reactive] public Manufacture SelectedManufacture { get; set; }

        public ManufactureViewModel(IManufactureDataService manufactureDataService)
        {
            _manufactureDataService = manufactureDataService;

            CreateManufacture = ReactiveCommand.CreateFromTask(ExecuteCreateCreateManufacture);
            CreateManufacture.Subscribe(x => AddManufacture(x));

            var canPrint = this
                .WhenAnyValue(x => x.SelectedManufacture)
                .Select(x => x != null);

            PrintCommand = ReactiveCommand.Create<Manufacture>(ExecutePrintCommand, canPrint);
            Task.Run(Initialize);

        }

        private void AddManufacture(Manufacture manufacture)
        {
            ManufacturesCollection.Add(new Manufacture
            {
                Id = manufacture.Id,
                AddressKz = manufacture.AddressKz,
                Index = manufacture.Index,
                AddressRu = manufacture.AddressRu,
                Name = manufacture.Name
            });
            Manufacture = null;
            Manufacture = new Manufacture();
        }

        private async Task Initialize()
        {
            var manufactures = await _manufactureDataService.GetManufactures();
            ManufacturesCollection = new ObservableCollection<Manufacture>(manufactures);

        }

        private void ExecutePrintCommand(Manufacture manufacture)
        {
            //var printInitialize = PrintPreviewFactory.GetPrintView(PrintViewType.BuyerInformation);
            //var flowDoc = printInitialize.InitializeFlow(new Models.ViewModels.ReestrObject(new Reestr() { Customer = manufacture }));
            //PrintHelper.Prints(flowDoc, manufacture.ShortName);
        }
       

        private async Task<Manufacture> ExecuteCreateCreateManufacture()
        {
            await _manufactureDataService.CreateManufacture(Manufacture);

            return Manufacture;
        }
    }
}
