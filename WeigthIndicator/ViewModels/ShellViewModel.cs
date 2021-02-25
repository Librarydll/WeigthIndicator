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
using WeigthIndicator.Domain.Exceptions;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Services;
using WeigthIndicator.Views;

namespace WeigthIndicator.ViewModels
{
    public class ShellViewModel : ReactiveObject
    {
        private readonly IComPortProvider _comPortProvider;
        private readonly IRecipeDataService _recipeDataService;
        private readonly IReestrDataService _reestrDataService;
        private readonly IReestrSettingProvider _reestrSettingProvider;
        private readonly IDialogService _dialogService;
        public ReactiveCommand<Unit, Unit> OpenReestrSettingCommand { get; }

        private ObservableCollection<Reestr> _reestrsCollection;

        public ObservableCollection<Reestr> ReestrsCollection
        {
            get { return _reestrsCollection; }
            set { _reestrsCollection = value; }
        }

       [Reactive] public int ProgressValue { get; set; }
       [Reactive] public int MaxProgressValue { get; set; }
       [Reactive] public bool IsAutoMode { get; set; }


        private readonly ObservableAsPropertyHelper<double> _itemWeight;
        public double ItemWeigth => _itemWeight.Value;


        public ReactiveCommand<Unit,Reestr> SaveCommand { get; set; }

        public ShellViewModel(IComPortProvider comPortProvider,
            IRecipeDataService recipeDataService,
            IReestrDataService reestrDataService,
            IReestrSettingProvider reestrSettingProvider,
            IDialogService dialogService)
        {
            _comPortProvider = comPortProvider;
            _recipeDataService = recipeDataService;
            _reestrDataService = reestrDataService;
            _reestrSettingProvider = reestrSettingProvider;
            _dialogService = dialogService;
            OpenReestrSettingCommand = ReactiveCommand.Create(ExecuteOpenReestrSettingCommand);
            ReestrsCollection = new ObservableCollection<Reestr>();
            var parsedValue = this.WhenAnyValue(x => x._comPortProvider.ComPortConnector.ParsedValue);

            parsedValue.ObserveOnDispatcher()
                        .Subscribe(x=>Proggress(x));

            Observable.Timer(TimeSpan.FromSeconds(1.5), TimeSpan.FromSeconds(1))
                .ObserveOnDispatcher()
                .Subscribe(Progress2);

            parsedValue.Throttle(TimeSpan.FromSeconds(3))
                        .Where(ValidateParsedValue)
                        .Where(x=> IsAutoMode)
                        .SelectMany(InsertToDataBase)
                        .Catch((Func<Exception, IObservable<Reestr>>)HandleException)
                        .Where(x=>x!=null)
                        .Select(x=>x)
                        .ObserveOn(RxApp.MainThreadScheduler)
                        .Subscribe(InsertToCollection);

            _itemWeight = parsedValue.Select(x => x)
                                     .ObserveOn(RxApp.MainThreadScheduler)
                                     .ToProperty(this, x => x.ItemWeigth);

            MaxProgressValue = 3;
            IsAutoMode = true;

            var canExecute = this
               .WhenAnyValue(x => x.IsAutoMode,
               isAutoMode => isAutoMode  == false);

       
            SaveCommand = ReactiveCommand.CreateFromTask(ExecuteSaveCommand, canExecute);
            SaveCommand
                .Catch((Func<Exception, IObservable<Reestr>>)HandleException)
                .Where(x => x != null)
                .Select(x => x)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(InsertToCollection);
            ExecuteOpenReestrSettingCommand();
        }

        private async Task<Reestr> ExecuteSaveCommand()
        {
            if (ValidateParsedValue(_comPortProvider.ComPortConnector.ParsedValue))
            {
                var net = _comPortProvider.ComPortConnector.ParsedValue - _reestrSettingProvider.ReestrSetting.TaraBarrel;
                var reestr = CreateReestr(net);
                if (reestr.RecipeId > 0)
                {
                    await _reestrDataService.CreateReestrAndUpdateBarellStorage(reestr);
                    return reestr;
                }
            }
            return null;
        }

        private IObservable<Reestr> HandleException(Exception exception)
        {
            if(exception is BarellStorageEmptyException ex)
            {
               MessageBox.Show("Test");
            }

            return Observable.Return<Reestr>(null);
        }

        private void Progress2(long obj)
        {
            if(MaxProgressValue!=ProgressValue)
                ProgressValue += 1;

        }

        private double Proggress(double value)
        {
            ProgressValue = 0;

            return value;
        }

        private async Task<Reestr> InsertToDataBase(double value)
        {
            var net = value - _reestrSettingProvider.ReestrSetting.TaraBarrel;
            var reestr = CreateReestr(net);
            if (reestr.RecipeId > 0)
            {
                await _reestrDataService.CreateReestrAndUpdateBarellStorage(reestr);
                return reestr;
            }
            return null;
        }

        private void InsertToCollection(Reestr reestr)
        {
            if (reestr != null)
            {
                ReestrsCollection.Add(reestr);
            }
        }


        private Reestr CreateReestr(double net)
        {
            return new Reestr
            {
                BatchNumber = _reestrSettingProvider.ReestrSetting.BatchNumber,
                Buyer = _reestrSettingProvider.ReestrSetting.BuyerName,
                RecipeId = _reestrSettingProvider.ReestrSetting.CurrentRecipe.Id,
                Recipe = _reestrSettingProvider.ReestrSetting.CurrentRecipe,
                TareBarrel = _reestrSettingProvider.ReestrSetting.TaraBarrelWithLid,
                TareBarrelWithLid = _reestrSettingProvider.ReestrSetting.TaraBarrelWithLid,
                PackingDate = DateTime.Now,
                ReestrState = true,
                Net = net,
            };
        }

        private bool ValidateParsedValue(double value)
        {
            if (value <= 0)
                return false;

            if (value < _reestrSettingProvider.ReestrSetting.MaxWeight && value > _reestrSettingProvider.ReestrSetting.MinWeight)
                return true;
            return false;
        }

        private void ExecuteOpenReestrSettingCommand()
        {
            _dialogService.ShowDialog("ReestrSettingView", x =>
            {
                if (!_comPortProvider.ComPortConnector.IsOpen)
                {
                    _comPortProvider.Start();
                }
            });
        }
    }

}
