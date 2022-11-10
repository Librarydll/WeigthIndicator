using Prism.Commands;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using WeigthIndicator.Dialog;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Models.ViewModels;
using WeigthIndicator.Store;

namespace WeigthIndicator.ViewModels
{
    public class CreateUpdateOutcomeViewModel : ReactiveObject
    {
        private readonly IDialogNavigationService _dialogNavigationService;
        private readonly IOutcomeDataService _outcomeDataService;
        private readonly NavigationStore _navigationStore;
        private readonly IUnityContainer _unityContainer;

        [Reactive] public Outcome Outcome { get; set; }
        public int UserId { get; set; }
        [Reactive] public DateTime Created { get; set; } = DateTime.Now;
        public ObservableCollection<OutcomeItemViewModel> OutcomeItems { get; set; }
        public DelegateCommand SelecteMultipleReestrCommand { get; set; }
        public DelegateCommand BackCommand { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand<OutcomeItemViewModel> DeleteCommand { get; set; }
        public ReactiveObject ParentViewModel { get; set; }
        public CreateUpdateOutcomeViewModel(
            NavigationStore navigationStore,
            UserStore userStore,
            IUnityContainer unityContainer,
            IDialogNavigationService dialogNavigationService,
            IOutcomeDataService  outcomeDataService)
        {
            SelecteMultipleReestrCommand = new DelegateCommand(ExecuteSelecteMultipleReestrCommand);
            BackCommand = new DelegateCommand(ExecuteBackCommand);
            SaveCommand = new DelegateCommand(async () => await ExecuteSaveCommand());
            DeleteCommand = new DelegateCommand<OutcomeItemViewModel>(ExecuteDeleteCommand);
            _dialogNavigationService = dialogNavigationService;
            _outcomeDataService = outcomeDataService;
            _navigationStore = navigationStore;
            _unityContainer = unityContainer;
            OutcomeItems = new ObservableCollection<OutcomeItemViewModel>();
            UserId = userStore.CurrentUser.Id;
        }

        private void ExecuteDeleteCommand(OutcomeItemViewModel outcomeItemViewModel)
        {
            if (outcomeItemViewModel.Id == 0) OutcomeItems.Remove(outcomeItemViewModel);

            outcomeItemViewModel.Count = -1;
        }


        private async Task ExecuteSaveCommand()
        {
            PrepareOutcome();
            var outcomeItems = OutcomeItemViewModel.BuildOut(OutcomeItems);
            Outcome.OutcomeItems = outcomeItems.ToList();
            if (Outcome.Id == 0)
            {
                Outcome.UserId = UserId;
                await _outcomeDataService.CreateOutcome(Outcome);
            }
            else
            {
                await _outcomeDataService.UpdateOutcome(Outcome);
            }

            ExecuteBackCommand();
        }

        private void ExecuteBackCommand()
        {
            _unityContainer.Resolve<NavigationViewModel>().ContentViewModel = ParentViewModel;
        }

        public void OnNavigated(ReactiveObject reactiveObject,Outcome outcome)
        {
            Outcome = outcome ?? new Outcome() { Created = DateTime.Now };
            Created = Outcome.Created;
            ParentViewModel = reactiveObject;
            if(Outcome.Id > 0)
            {
                Task.Run(async () =>
                {
                    Outcome = await _outcomeDataService.GetOutcome(outcome.Id);
                    Created = Outcome.Created;
                    var items = OutcomeItemViewModel.BuildIn(Outcome.OutcomeItems);
                    OutcomeItems = new ObservableCollection<OutcomeItemViewModel>(items);

                });
            }
           
        }

        private void ExecuteSelecteMultipleReestrCommand()
        {
            _dialogNavigationService.ShowDialog<AllReestrsViewModel, IEnumerable<Reestr>>(callback =>
            {
                if (callback.IsSuccess)
                {
                    foreach (var reestr in callback.Value)
                    {
                        var existed = OutcomeItems.FirstOrDefault(x => x.ReestrId == reestr.Id);
                        if (existed == null)
                        {
                            OutcomeItems.Add(new OutcomeItemViewModel
                            {
                                Count = reestr.Net,
                                Comment = reestr.Note,
                                OutcomeId = Outcome.Id,
                                ReestrId = reestr.Id,
                                Reestr = reestr,
                            });
                        }
                    }
                }
            });
        }

        private void PrepareOutcome()
        {
            if(Created.Date.TimeOfDay == new TimeSpan())
            {
                var now = DateTime.Now;
                Outcome.Created = new DateTime(Created.Year, Created.Month, Created.Day, now.TimeOfDay.Hours, now.TimeOfDay.Minutes, now.TimeOfDay.Seconds);
            }
        }
    }
}
