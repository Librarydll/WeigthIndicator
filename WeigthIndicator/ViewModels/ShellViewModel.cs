using DynamicData;
using DynamicData.Binding;
using Prism.Events;
using Prism.Services.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using WeigthIndicator.Core.Print;
using WeigthIndicator.Dapper.Services;
using WeigthIndicator.Domain.Exceptions;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Events;
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

        private readonly ObservableAsPropertyHelper<double> _itemWeight;
        public double ItemWeigth => _itemWeight.Value;


        public ReactiveCommand<Unit, Reestr> SaveCommand { get; set; }
        public ReactiveCommand<Reestr, Unit> EditCommand { get; set; }
        public ReactiveCommand<Reestr, Unit> PrintCommand { get; set; }

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

            Observable.Timer(TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(1))
                .ObserveOnDispatcher()
                .Subscribe(Progress2);


            parsedValue.Throttle(TimeSpan.FromSeconds(3))
                        .Where(x => IsAutoMode)
                        .Where(ValidateParsedValue)
                        .SelectMany(InsertToDataBase)
                        .Catch((Func<Exception, IObservable<Reestr>>)HandleException)
                        .Where(x => x != null)
                        .Select(x => x)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(InsertToCollection);

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

            EditCommand = ReactiveCommand.Create<Reestr>(ExecuteEditViewCommand);
            PrintCommand = ReactiveCommand.Create<Reestr>(ExecutePrintViewCommand);
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

            PrintPreviewView printPreviewView = new PrintPreviewView();

            FlowDocument flowDoc = printPreviewView.InitializeFlowDocument(reestr);

            PrintHelper.Prints(flowDoc);
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
            if (ValidateParsedValue(_comPortProvider.ComPortConnector.ParsedValue))
            {
                var net = _comPortProvider.ComPortConnector.ParsedValue - ReestrSetting.TaraBarrel;
                var reestr = CreateReestr(net);
                return await _reestrDataService.CreateReestrAndUpdateBarellStorage(reestr);
            }

            return null;
        }

        private IObservable<Reestr> HandleException(Exception exception)
        {
            if (exception is BarellStorageEmptyException ex)
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
            return await _reestrDataService.CreateReestrAndUpdateBarellStorage(reestr);
        }

        private void InsertToCollection(Reestr reestr)
        {
            if (reestr != null)
            {
                ReestrsCollection.Add(reestr);
                _isValueDroppedToMinimum = false;
            }
        }


        private Reestr CreateReestr(double net) => new Reestr
        {
            BatchNumber = ReestrSetting.BatchNumber,
            Customer = ReestrSetting.Customer,
            CustomerId = ReestrSetting.CustomerId,
            RecipeId = ReestrSetting.CurrentRecipe.Id,
            Recipe = ReestrSetting.CurrentRecipe,
            TareBarrel = ReestrSetting.TaraBarrelWithLid,
            TareBarrelWithLid = ReestrSetting.TaraBarrelWithLid,
            PackingDate = DateTime.Now,
            ReestrState = true,
            Net = net,
        };

        private bool ValidateParsedValue(double value)
        {
            if (!_isValueDroppedToMinimum)
                return false;
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
                    _comPortProvider.Start();
                }
            });
        }
    }

}
