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
        public ReactiveCommand<Unit, Unit> OpenReestrSettingCommand { get; }

        private ObservableCollection<Reestr> _reestrsCollection;

        public ObservableCollection<Reestr> ReestrsCollection
        {
            get { return _reestrsCollection; }
            set { _reestrsCollection = value; }
        }
        public ReestrSetting ReestrSetting { get; set; }

        [Reactive] public int ProgressValue { get; set; }
        [Reactive] public int MaxProgressValue { get; set; }
        [Reactive] public bool IsAutoMode { get; set; }

        private readonly ObservableAsPropertyHelper<double> _itemWeight;
        public double ItemWeigth => _itemWeight.Value;


        public ReactiveCommand<Unit, Reestr> SaveCommand { get; set; }

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
            OpenReestrSettingCommand = ReactiveCommand.Create(ExecuteOpenReestrSettingCommand);
            ReestrsCollection = new ObservableCollection<Reestr>();
            var parsedValue = this.WhenAnyValue(x => x._comPortProvider.ComPortConnector.ParsedValue);
            parsedValue.Subscribe(ValueDropped);
            parsedValue.ObserveOnDispatcher()
                        .Subscribe(x => Proggress(x));

            Observable.Timer(TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(1))
                .ObserveOnDispatcher()
                .Subscribe(Progress2);


            parsedValue.Throttle(TimeSpan.FromSeconds(3))
                        .Where(x => { Console.WriteLine(IsAutoMode); return IsAutoMode; })
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

            MaxProgressValue = 3;
            IsAutoMode = true;

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
            ExecuteOpenReestrSettingCommand();
        }

        public async Task<ReestrSetting> InitializeReesterSetting()
        {
            ReestrSetting = await _reestrSettingDataService.GetReestrSetting();
            MessageBus.Current.SendMessage(ReestrSetting.CurrentRecipe);
            return ReestrSetting;
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
            Buyer = ReestrSetting.BuyerName,
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
