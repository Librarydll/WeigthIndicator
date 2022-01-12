using ReactiveUI;
using System.Reactive.Disposables;
using WeigthIndicator.ViewModels;
using System;
namespace WeigthIndicator.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : ReactiveWindow<MainViewModel>
    {
        public MainView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.DataContext)
                    .BindTo(this, x => x.ViewModel);

                this.Bind(ViewModel, vm => vm.RecipeView, view => view.RecipeView.IsSelected)
                    .DisposeWith(disposables);


                this.Bind(ViewModel, vm => vm.MainView, view => view.SecondView.IsSelected)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.SettingView, view => view.SettingView.IsSelected)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.BarrelView, view => view.BarrelView.IsSelected)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.CustomerView, view => view.CustomerView.IsSelected)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.ReestrView, view => view.ReestrView.IsSelected)
                   .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.GroupedReestrView, view => view.GropedReestrView.IsSelected)
                 .DisposeWith(disposables);

                this.WhenAnyValue(x => x.ViewModel)
                   .Subscribe(x => x.GoToStatusView());

            });
        }
    }
}
