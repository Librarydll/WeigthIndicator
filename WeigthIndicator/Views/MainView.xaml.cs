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

                //this.OneWayBind(ViewModel,vm => vm.SelectedItem,v => v.NavigationView.SelectedItem)
                //    .DisposeWith(disposables);
                //this.OneWayBind(ViewModel, vm => vm.Router, v => v.RoutedViewHost.Router)
                //    .DisposeWith(disposables);

                //this.Bind(ViewModel, vm => vm.SelectedItem, view => view.NavigationView.SelectedItem)
                //    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.RecipeView, view => view.RecipeView.IsSelected)
                    .DisposeWith(disposables);


                this.Bind(ViewModel, vm => vm.MainView, view => view.SecondView.IsSelected)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.SettingView, view => view.SettingView.IsSelected)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.BarellView, view => view.BarellView.IsSelected)
                    .DisposeWith(disposables);

                this.WhenAnyValue(x => x.ViewModel)
                   .Subscribe(x => x.GoToStatusView());

            });
        }
    }
}
