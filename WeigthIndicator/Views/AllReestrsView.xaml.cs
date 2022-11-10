using ReactiveUI;
using System.Reactive.Disposables;
using System.Windows.Controls;
using WeigthIndicator.ViewModels;

namespace WeigthIndicator.Views
{
    /// <summary>
    /// Interaction logic for AllReestrsView.xaml
    /// </summary>
    public partial class AllReestrsView : ReactiveUserControl<AllReestrsViewModel>
    {
        public AllReestrsView()
        {
            InitializeComponent();
            this.WhenActivated(disposables =>
            {
                ViewModel = DataContext as AllReestrsViewModel;


                this.OneWayBind(ViewModel,
                    vm => vm.ReestrsCollection,
                    v => v.ReestrsCollection.ItemsSource)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.FilterModel.FromDate, v => v.FromDate.SelectedDate);
                this.Bind(ViewModel, vm => vm.FilterModel.ToDate, v => v.ToDate.SelectedDate);
                this.Bind(ViewModel, vm => vm.FilterModel.IsToDateInclude, v => v.IncludeToDate.IsChecked);
                this.Bind(ViewModel, vm => vm.FilterModel.SearchQuery, v => v.SearchQuery.Text);

                this.Bind(ViewModel, vm => vm.SelectedPrintViewType, v => v.PrintViewTypeCmb.SelectedIndex)
                    .DisposeWith(disposables);


                this.Bind(ViewModel, vm => vm.SelectedBarrelType, v => v.BarrelType.SelectedIndex)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.ReestrCount, v => v.ReestrCount.Text);
                this.OneWayBind(ViewModel,
                                vm => vm.NetTotal,
                                v => v.NetSum.Text,
                                x => x.ToString("N"));

                this.OneWayBind(ViewModel, vm => vm.Batches, v => v.Batches.ItemsSource);
                this.OneWayBind(ViewModel, vm => vm.Materials, v => v.Materials.ItemsSource);


                this.BindCommand(ViewModel, vm => vm.FilterCommad, v => v.FilterCommand);
                this.BindCommand(ViewModel, vm => vm.FilterBySearchQueryCommand, v => v.FilterBySearchQueryCommand);
            });
        }
    }
}
