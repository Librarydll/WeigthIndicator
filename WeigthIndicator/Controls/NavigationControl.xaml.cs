using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WeigthIndicator.ViewModels;

namespace WeigthIndicator.Controls
{
    /// <summary>
    /// Interaction logic for NavigationControl.xaml
    /// </summary>
    public partial class NavigationControl : ReactiveUserControl<NavigationViewModel>
    {
        public NavigationControl()
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
