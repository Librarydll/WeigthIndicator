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

namespace WeigthIndicator.Views
{
    /// <summary>
    /// Interaction logic for ReestrView.xaml
    /// </summary>
    public partial class ReestrView : ReactiveUserControl<ReestrViewModel>
    {
        public ReestrView()
        {
            InitializeComponent();
            ViewModel = DataContext as ReestrViewModel;
            this.WhenActivated(disposables =>
            {
                this.OneWayBind(ViewModel,
                    vm => vm.ReestrsCollection,
                    v => v.ReestrsCollection.ItemsSource)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.FilterModel.FromDate, v => v.FromDate.SelectedDate);
                this.Bind(ViewModel, vm => vm.FilterModel.ToDate, v => v.ToDate.SelectedDate);
                this.Bind(ViewModel, vm => vm.FilterModel.IsToDateInclude, v => v.IncludeToDate.IsChecked);
                this.Bind(ViewModel, vm => vm.FilterModel.SearchQuery, v => v.SearchQuery.Text);
                this.Bind(ViewModel, vm => vm.MaterialGroup, v => v.MaterialGroup.Text);

                this.Bind(ViewModel, vm => vm.SelectedBarrelType, v => v.BarrelType.SelectedIndex)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.ReestrCount, v => v.ReestrCount.Text);
                this.OneWayBind(ViewModel,
                                vm => vm.NetTotal,
                                v => v.NetSum.Text,
                                x => x.ToString("N"));


                this.Bind(ViewModel, vm => vm.SelectedPrintViewType, v => v.PrintViewTypeCmb.SelectedItem)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.SelectedMonth, v => v.MonthComboBox.SelectedItem)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.SelectedYear, v => v.YearComboBox.SelectedItem)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    vm => vm.PrinterViewTypes,
                    v => v.PrintViewTypeCmb.ItemsSource)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel, vm => vm.Batches, v => v.Batches.ItemsSource);
                this.OneWayBind(ViewModel, vm => vm.Materials, v => v.Materials.ItemsSource);


                this.BindCommand(ViewModel, vm => vm.FilterCommad, v => v.FilterCommand);
                this.BindCommand(ViewModel, vm => vm.FilterBySearchQueryCommand, v => v.FilterBySearchQueryCommand);
                this.BindCommand(ViewModel, vm => vm.ExportExcelCommand, v => v.ExportExcelCommand);


            });

            MonthComboBox.ItemsSource = System.Globalization.CultureInfo
                            .GetCultureInfo("en-US")
                            .DateTimeFormat
                            .MonthNames
                            .Where(m => !string.IsNullOrEmpty(m))
                            .ToList();

            YearComboBox.ItemsSource = Enumerable.Range(2015, 20).Select(y => y.ToString())
                                     .ToList();
        }

        private void PrintViewTypeCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MaterailGroupTextBlock != null)
            {
                MaterailGroupTextBlock.Visibility = Visibility.Collapsed;
                MaterialGroup.Visibility = Visibility.Collapsed;
                MonthComboBox.Visibility = Visibility.Collapsed;
                YearComboBox.Visibility = Visibility.Collapsed;

                if (PrintViewTypeCmb.SelectedIndex == 2)
                {
                    MaterailGroupTextBlock.Visibility = Visibility.Visible;
                    MaterialGroup.Visibility = Visibility.Visible;
                }

                if (PrintViewTypeCmb.SelectedIndex == 3)
                {
                    MonthComboBox.Visibility = Visibility.Visible;
                    YearComboBox.Visibility = Visibility.Visible;
                }
            }
        }
    }
}
