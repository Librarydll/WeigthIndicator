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
        public RoutingState Router { get; private set; }

        public ReactiveCommand<Unit, Unit> GoRecipeView { get; }
        [Reactive] public bool RecipeView { get; set; }
        [Reactive] public bool MainView { get; set; }

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
            if (view == null) return;

            if (!_regionManager.Regions[regionName].Views.Contains(view))
            {
                _regionManager.Regions[regionName].Add(view, viewName);
            }
           _regionManager.Regions[regionName].Activate(view);
        }

    }
}
