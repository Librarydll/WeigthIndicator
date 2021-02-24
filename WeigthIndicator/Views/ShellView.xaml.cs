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

                this.OneWayBind(ViewModel,
                    vm => vm.ReestrsCollection,
                    v => v.ReestrsCollection.ItemsSource)
                    .DisposeWith(disposables);

                this.BindCommand(ViewModel, vm => vm.OpenReestrSettingCommand, v => v.ReestrSetting);
            });

        }
    }
}
