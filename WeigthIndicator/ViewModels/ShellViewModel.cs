using Prism.Services.Dialogs;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Views;

namespace WeigthIndicator.ViewModels
{
    public class ShellViewModel:ReactiveObject
    {
        private readonly IRecipeDataService _recipeDataService;
        private readonly IDialogService _dialogService;
        public ReactiveCommand<Unit,Unit> OpenReestrSettingCommand { get;  }

        public ShellViewModel(
            IRecipeDataService recipeDataService,
            IDialogService dialogService)
        {
            _recipeDataService = recipeDataService;
            _dialogService = dialogService;
            OpenReestrSettingCommand = ReactiveCommand.Create(ExecuteOpenReestrSettingCommand);
        }
     

        private void ExecuteOpenReestrSettingCommand()
        {
            _dialogService.ShowDialog("ReestrSettingView", x =>
            {

            });
        }

      
    }
}
