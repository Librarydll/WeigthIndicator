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
    public class MainViewModel : ReactiveObject, IScreen
    {
        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _unityContainer;
        private ShellView _shellView;
        private RecipeView _recipeView;
        private SettingView _settingView;
        public RoutingState Router { get; private set; }
        [Reactive] public bool RecipeView { get; set; }
        [Reactive] public bool MainView { get; set; }
        [Reactive] public bool SettingView { get; set; }

        public MainViewModel(IRegionManager regionManager,
            IUnityContainer unityContainer)
        {
            _regionManager = regionManager;
            _unityContainer = unityContainer;
            this.WhenAnyValue(x => x.RecipeView)
                .Where(x => RecipeView)
                .Subscribe(x=> GoToRecipeView());

            this.WhenAnyValue(x => x.MainView)
                .Where(x => MainView)
                .Subscribe(x => GoToMainView());

            this.WhenAnyValue(x => x.SettingView)
                .Where(x => SettingView)
                .Subscribe(x => GoToSettingView());
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


        public  void NavigateToView(object view, string viewName, string regionName = "MainRegion")
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
