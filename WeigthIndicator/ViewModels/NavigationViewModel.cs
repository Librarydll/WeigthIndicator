using Prism.Regions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using WeigthIndicator.Store;
using WeigthIndicator.Views;

namespace WeigthIndicator.ViewModels
{
    public class NavigationViewModel : ReactiveObject
    {
        private readonly NavigationStore _navigationStore;
        private readonly IUnityContainer _unityContainer;


        [Reactive] public ReactiveObject StatusViewModel { get; set; }
        [Reactive] public ReactiveObject ContentViewModel { get; set; }


        [Reactive] public bool RecipeView { get; set; }
        [Reactive] public bool MainView { get; set; }
        [Reactive] public bool SettingView { get; set; }
        [Reactive] public bool BarrelView { get; set; }
        [Reactive] public bool CustomerView { get; set; }
        [Reactive] public bool ReestrView { get; set; }
        [Reactive] public bool GroupedReestrView { get; set; }
        [Reactive] public bool OutcomeView { get; set; }
        [Reactive] public bool ManufactureView { get; set; }
        public NavigationViewModel(
                    NavigationStore navigationStore,
                    IUnityContainer unityContainer)
        {
            _navigationStore = navigationStore;
            _unityContainer = unityContainer;
            this.WhenAnyValue(x => x.RecipeView)
                .Where(x => RecipeView)
                .Subscribe(x => GoToRecipeView());

            this.WhenAnyValue(x => x.MainView)
                .Where(x => MainView)
                .Subscribe(x => GoToMainView());

            this.WhenAnyValue(x => x.SettingView)
                .Where(x => SettingView)
                .Subscribe(x => GoToSettingView());

            this.WhenAnyValue(x => x.BarrelView)
                .Where(x => BarrelView)
                .Subscribe(x => GoToBarrelView());

            this.WhenAnyValue(x => x.CustomerView)
                .Where(x => CustomerView)
                .Subscribe(x => GoToCustomerView());


            this.WhenAnyValue(x => x.ReestrView)
                .Where(x => ReestrView)
                .Subscribe(x => GoToReestrView());

            this.WhenAnyValue(x => x.GroupedReestrView)
               .Where(x => GroupedReestrView)
               .Subscribe(x => GoToGropedReestrView());

            this.WhenAnyValue(x => x.OutcomeView)
                .Where(x => OutcomeView)
                .Subscribe(x => GoToOutcomeView());

            this.WhenAnyValue(x => x.ManufactureView)
            .Where(x => ManufactureView)
            .Subscribe(x => GoToManufactureView());

            GoToStatusView();
        }

        private void GoToManufactureView()
        {
            ContentViewModel = _unityContainer.Resolve<ManufactureViewModel>();
        }

        public void GoToStatusView()
        {
            StatusViewModel = _unityContainer.Resolve<StatusViewModel>();
        }

        private void GoToGropedReestrView()
        {
            ContentViewModel = _unityContainer.Resolve<GroupedReestrViewModel>();
        }

        private void GoToReestrView()
        {
            ContentViewModel = _unityContainer.Resolve<ReestrViewModel>();
        }

        private void GoToCustomerView()
        {
            ContentViewModel = _unityContainer.Resolve<CustomerViewModel>();
        }

        private void GoToBarrelView()
        {
            ContentViewModel = _unityContainer.Resolve<BarrelViewModel>();
        }
        private void GoToOutcomeView()
        {
            ContentViewModel = _unityContainer.Resolve<OutcomeViewModel>();
        }

        void GoToSettingView()
        {
            ContentViewModel = _unityContainer.Resolve<SettingViewModel>();
        }

        void GoToRecipeView()
        {
            ContentViewModel = _unityContainer.Resolve<RecipeViewModel>();
        }

        void GoToMainView()
        {
            ContentViewModel = _unityContainer.Resolve<ShellViewModel>();
        }

    }
}
