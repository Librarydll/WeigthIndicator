using ModernWpf.Controls;
using Prism.Regions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows;
using Unity;
using WeigthIndicator.Dapper;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Views;

namespace WeigthIndicator.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _unityContainer;
        private ShellView _shellView;
        private RecipeView _recipeView;
        private SettingView _settingView;
        private BarrelView _barrelView;
        private StatusView _statusView;
        private CustomerView _customerView;
        private ReestrView _reestrView;
        private GroupedReestrView _groupedReestrView;
        [Reactive] public bool RecipeView { get; set; }
        [Reactive] public bool MainView { get; set; }
        [Reactive] public bool SettingView { get; set; }
        [Reactive] public bool BarrelView { get; set; }
        [Reactive] public bool CustomerView { get; set; }
        [Reactive] public bool ReestrView { get; set; }
        [Reactive] public bool GroupedReestrView { get; set; }

        public MainViewModel(IRegionManager regionManager,
            IUnityContainer unityContainer)
        {
            _regionManager = regionManager;
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
        }



        public void GoToStatusView()
        {
            _statusView = _statusView ?? _unityContainer.Resolve<StatusView>();
            NavigateToView(_statusView, nameof(StatusView), "StatusBarRegion");

        }

        private void GoToGropedReestrView()
        {
            _groupedReestrView = _groupedReestrView ?? _unityContainer.Resolve<GroupedReestrView>();
            NavigateToView(_groupedReestrView, nameof(GroupedReestrView));
        }

        private void GoToReestrView()
        {
            _reestrView = _reestrView ?? _unityContainer.Resolve<ReestrView>();
            NavigateToView(_reestrView, nameof(ReestrView));
        }

        private void GoToCustomerView()
        {
            _customerView = _customerView ?? _unityContainer.Resolve<CustomerView>();
            NavigateToView(_customerView, nameof(CustomerView));
        }

        private void GoToBarrelView()
        {
            _barrelView = _barrelView ?? _unityContainer.Resolve<BarrelView>();
            NavigateToView(_barrelView, nameof(BarrelView));
        }

        void GoToSettingView()
        {
            _settingView = _settingView ?? _unityContainer.Resolve<SettingView>();
            NavigateToView(_settingView, nameof(SettingView));
        }

        void GoToRecipeView()
        {
            _recipeView = _recipeView ?? _unityContainer.Resolve<RecipeView>();
            NavigateToView(_recipeView, nameof(RecipeView));
        }

        void GoToMainView()
        {
            _shellView = _shellView ?? _unityContainer.Resolve<ShellView>();
            NavigateToView(_shellView, nameof(ShellView));
        }


        public void NavigateToView(object view, string viewName, string regionName = "MainRegion")
        {
            if (view == null)
                throw new NullReferenceException("view is null");

            if (!_regionManager.Regions[regionName].Views.Contains(view))
            {
                _regionManager.Regions[regionName].Add(view, viewName);
            }
            _regionManager.Regions[regionName].Activate(view);
        }

    }
}
