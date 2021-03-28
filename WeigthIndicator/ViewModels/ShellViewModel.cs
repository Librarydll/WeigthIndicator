using DynamicData;
using Prism.Services.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using WeigthIndicator.Core.Print;
using WeigthIndicator.Domain.Exceptions;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Events;
using WeigthIndicator.Factory;
using WeigthIndicator.Services;
using WeigthIndicator.Views;

namespace WeigthIndicator.ViewModels
{
    public class ShellViewModel : ReactiveObject
    {
        private readonly IComPortProvider _comPortProvider;
        private readonly IReestrSettingDataService _reestrSettingDataService;
        private readonly IRecipeDataService _recipeDataService;
        private readonly IReestrDataService _reestrDataService;
        private readonly IDialogService _dialogService;
        private bool _isValueDroppedToMinimum = true;
        public ReactiveCommand<Unit, Unit> OpenReestrSettingCommand => ReactiveCommand.Create(ExecuteOpenReestrSettingCommand);

        private ObservableCollection<Reestr> _reestrsCollection;

        public ObservableCollection<Reestr> ReestrsCollection
        {
            get { return _reestrsCollection; }
            set { this.RaiseAndSetIfChanged(ref _reestrsCollection, value); }
        }


        public ReestrSetting ReestrSetting { get; set; }

        [Reactive] public int ProgressValue { get; set; }
        [Reactive] public int MaxProgressValue { get; set; }
        [Reactive] public bool IsAutoMode { get; set; }
        [Reactive] public Reestr SelectedReestr { get; set; }
        [Reactive] public int SelectedPrintViewType { get; set; }

        private readonly ObservableAsPropertyHelper<double> _itemWeight;
        public double ItemWeigth => _itemWeight.Value;


        public ReactiveCommand<Unit, Reestr> SaveCommand { get; set; }
        public ReactiveCommand<Reestr, Unit> EditCommand { get; set; }
        public ReactiveCommand<Reestr, Unit> PrintCommand { get; set; }

        private readonly ObservableAsPropertyHelper<int> _reestrCount;
        public int ReestrCount => _reestrCount.Value;

        private readonly ObservableAsPropertyHelper<double> _netTotal;
        public double NetTotal => _netTotal.Value;

        public ShellViewModel(IComPortProvider comPortProvider,
            IReestrSettingDataService reestrSettingDataService,
            IRecipeDataService recipeDataService,
            IReestrDataService reestrDataService,
            IDialogService dialogService)
        {
            _comPortProvider = comPortProvider;
            _reestrSettingDataService = reestrSettingDataService;
            _recipeDataService = recipeDataService;
            _reestrDataService = reestrDataService;
            _dialogService = dialogService;
            MaxProgressValue = 3;
            IsAutoMode = true;

            var parsedValue = this.WhenAnyValue(x => x._comPortProvider.ComPortConnector.ParsedValue);
            parsedValue.Subscribe(ValueDropped);
            parsedValue.ObserveOnDispatcher()
                        .Subscribe(x => Proggress(x));

            Observable.Interval(TimeSpan.FromSeconds(1))
                .ObserveOnDispatcher()
                .Subscribe(Progress2);

            parsedValue.Throttle(TimeSpan.FromSeconds(3))
                        .Where(x => IsAutoMode && _isValueDroppedToMinimum)
                        .Where(ValidateParsedValue)
                        .SelectMany(InsertToDataBase)
                        .Catch((Func<Exception, IObservable<Reestr>>)HandleException)
                        .Where(x => x != null)
                        .Select(x => x)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(InsertToCollection);

            var reestrCount = this.WhenAnyValue(x => x.ReestrsCollection.Count);
          _reestrCount = reestrCount
                        .Select(x => x)
                        .ToProperty(this, x => x.ReestrCount);

            _netTotal = reestrCount
                .Select(x => ReestrsCollection.Sum(z => z.Net))
                .ToProperty(this, x => x.NetTotal);


            _itemWeight = parsedValue.Select(x => x)
                                     .ObserveOn(RxApp.MainThreadScheduler)
                                     .ToProperty(this, x => x.ItemWeigth);

            var canExecute = this
               .WhenAnyValue(x => x.IsAutoMode,
               isAutoMode => isAutoMode == false);

            SaveCommand = ReactiveCommand.CreateFromTask(ExecuteSaveCommand, canExecute);
            SaveCommand
                .Catch((Func<Exception, IObservable<Reestr>>)HandleException)
                .Where(x => x != null)
                .Select(x => x)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(InsertToCollection);


            var canEditAndCanPrint = this
                .WhenAnyValue(x => x.SelectedReestr)
                .Select(x => x != null);


            EditCommand = ReactiveCommand.Create<Reestr>(ExecuteEditViewCommand, canEditAndCanPrint);
            PrintCommand = ReactiveCommand.Create<Reestr>(ExecutePrintViewCommand, canEditAndCanPrint);
            ExecuteOpenReestrSettingCommand();

        }

        private void ExecuteEditViewCommand(Reestr reestr)
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

        private void ExecutePrintViewCommand(Reestr reestr)
        {
            var printViewType = (PrintViewType)Enum.Parse(typeof(PrintViewType), SelectedPrintViewType.ToString());
            if (printViewType == PrintViewType.NoPrint) return;
            var printInitialize = PrintPreviewFactory.GetPrintView(printViewType);

            FlowDocument flowDoc = printInitialize.InitializeFlow(reestr);

            PrintHelper.Prints(flowDoc, reestr.PackingDate.ToString("dd.MM.yyyy"));
        }


        public async Task<IEnumerable<Reestr>> Initialize()
        {
            var reestrs = await _reestrDataService.GetReestrsByDate(DateTime.Now);
            ReestrSetting = await _reestrSettingDataService.GetReestrSetting();
            MessageBus.Current.SendMessage(ReestrSetting.CurrentRecipe);
            return reestrs;
        }

        public void FillCollection(IEnumerable<Reestr> reestrs)
        {
            if (ReestrsCollection == null)
                ReestrsCollection = new ObservableCollection<Reestr>(reestrs);
            else
            {
                ReestrsCollection.AddRange(reestrs);
            }
        }

        private void ValueDropped(double value)
        {
            if (ReestrSetting == null) return;

            if (value <= ReestrSetting.MinDefaultWeigth)
                _isValueDroppedToMinimum = true;

        }

        private async Task<Reestr> ExecuteSaveCommand()
        {
            if (_isValueDroppedToMinimum)
            {
                var net = _comPortProvider.ComPortConnector.ParsedValue - ReestrSetting.TaraBarrel;
                var reestr = CreateReestr(net);
                return await _reestrDataService.CreateReestrAndUpdateBarrelStorage(reestr);
            }

            return null;
        }

        private IObservable<Reestr> HandleException(Exception exception)
        {
            if (exception is BarrelStorageEmptyException ex)
            {
                MessageBox.Show("Test");
            }

            return Observable.Return<Reestr>(null);
        }

        private void Progress2(long obj)
        {
            if (MaxProgressValue != ProgressValue)
                ProgressValue += 1;

        }

        private double Proggress(double value)
        {
            ProgressValue = 0;
            return value;
        }

        private async Task<Reestr> InsertToDataBase(double value)
        {
            var net = value - ReestrSetting.TaraBarrel;
            var reestr = CreateReestr(net);
            if(reestr.BarrelNumber>0)
                await _reestrSettingDataService.UpdateReestrSettingBarrelColumn(ReestrSetting);
            return await _reestrDataService.CreateReestrAndUpdateBarrelStorage(reestr);
        }

        private void InsertToCollection(Reestr reestr)
        {
            if (reestr != null)
            {
                ReestrsCollection.Add(reestr);
                _isValueDroppedToMinimum = false;
                ExecutePrintViewCommand(reestr);
                MessageBus.Current.SendMessage(new ReestredAddedEvent { Reestr = reestr });
            }
        }


        private Reestr CreateReestr(double net) 
        {
            var reestr = new Reestr
            {
                BatchNumber = ReestrSetting.BatchNumber,
                Customer = ReestrSetting.Customer,
                CustomerId = ReestrSetting.CustomerId,
                RecipeId = ReestrSetting.CurrentRecipe.Id,
                Recipe = ReestrSetting.CurrentRecipe,
                TareBarrel = ReestrSetting.TaraBarrel,
                TareBarrelWithLid = ReestrSetting.TaraBarrelWithLid,
                PackingDate = DateTime.Now,
                ReestrState = true,
                Net = net,
            };

            reestr.BarrelNumber = ReestrSetting.GetBarrelNumberAndClearIt();
            return reestr;
        }

        private bool ValidateParsedValue(double value)
        {
            if (value <= 0)
                return false;

            if (value < ReestrSetting.MaxWeight && value > ReestrSetting.MinWeight)
                return true;
            return false;
        }

        private void ExecuteOpenReestrSettingCommand()
        {
            _dialogService.ShowDialog("ReestrSettingView", x =>
            {
                if (x.Result == ButtonResult.OK)
                {
                    ReestrSetting = x.Parameters.GetValue<ReestrSetting>("model");
                    MessageBus.Current.SendMessage(ReestrSetting.CurrentRecipe);

                }
                if (!_comPortProvider.ComPortConnector.IsOpen)
                {
                    OpenComPort();
                }

            });
        }

        private void OpenComPort()
        {
            if (!_comPortProvider.ComPortConnector.IsOpen)
            {
                try
                {
                    _comPortProvider.Start();
                }
                catch (Exception ex)
                {
                 MessageBox.Show(ex.Message);
                 MessageBox.Show(ex.InnerException?.Message);
                _comPortProvider.ComPortConnector.CloseComPort();
                _comPortProvider.Start();
                }
            }
        }
    }

}
