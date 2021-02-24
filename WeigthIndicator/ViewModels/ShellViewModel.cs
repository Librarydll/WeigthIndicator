using Prism.Services.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Services;
using WeigthIndicator.Views;

namespace WeigthIndicator.ViewModels
{
    public class ShellViewModel:ReactiveObject
    {
        private readonly IComPortProvider _comPortProvider;
        private readonly IRecipeDataService _recipeDataService;
        private readonly IReestrDataService _reestrDataService;
        private readonly IReestrSettingProvider _reestrSettingProvider;
        private readonly IDialogService _dialogService;
        public ReactiveCommand<Unit,Unit> OpenReestrSettingCommand { get; }

        private ObservableCollection<Reestr> _reestrsCollection;

        public ObservableCollection<Reestr> ReestrsCollection
        {
            get { return _reestrsCollection; }
            set { _reestrsCollection = value; }
        }

        //private readonly ObservableAsPropertyHelper<ObservableCollection<Reestr>> _reestrsCollection;
        //public ObservableCollection<Reestr> ReestrsCollection => _reestrsCollection.Value;
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
            this.WhenAnyValue(x => x._comPortProvider.ComPortConnector.IsAvailable,x=>x._comPortProvider.ComPortConnector.ParsedValue)
                .Where(x=>x.Item1 && x.Item2>0)
                .SelectMany(InsertToDataBase)
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(InsertToCollection);
            ExecuteOpenReestrSettingCommand();
        }

   
        private async Task<(bool,Reestr)> InsertToDataBase((bool ,double ) args)
        {
            var net = args.Item2 - _reestrSettingProvider.ReestrSetting.TaraBarrel;
            var reestr = CreateReestr(net);
            if (reestr.RecipeId > 0)
            {
                await _reestrDataService.CreateReestr(reestr);
                return (true, reestr);
            }
            return (false, null);
        }

        private void InsertToCollection((bool,Reestr) args)
        {
            if (args.Item1)
            {
                ReestrsCollection.Add(args.Item2);
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
                TareBarrel =_reestrSettingProvider.ReestrSetting.TaraBarrelWithLid,
                TareBarrelWithLid =_reestrSettingProvider.ReestrSetting.TaraBarrelWithLid,
                PackingDate = DateTime.Now,
                ReestrState =true,
                Net = net,
            };
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
