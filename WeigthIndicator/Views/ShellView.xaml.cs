using ReactiveUI;
using System.Reactive.Disposables;
using WeigthIndicator.ViewModels;

namespace WeigthIndicator.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : ReactiveUserControl<ShellViewModel>
    {
        public ShellView()
        {
            InitializeComponent();

            this.WhenActivated(disposables =>
            {
                this.WhenAnyValue(x => x.DataContext)
                    .BindTo(this, x => x.ViewModel);

                //this.OneWayBind(ViewModel, vm => vm.RecipesCollection, v => v.RecipeCmb.ItemsSource)
                //    .DisposeWith(disposabled);

                //this.Bind(ViewModel, vm => vm.SelectedRecipe, v => v.RecipeCmb.SelectedItem)
                //    .DisposeWith(disposabled);
                this.BindCommand(ViewModel, vm => vm.OpenReestrSettingCommand, v => v.ReestrSetting);
            });

        }
    }
}
