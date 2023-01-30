﻿using ReactiveUI;
using System;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Factory;
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
            ViewModel = DataContext as ShellViewModel;
            this.WhenActivated(disposables =>
            {

                this.OneWayBind(ViewModel,
                    vm => vm.ReestrsCollection,
                    v => v.ReestrsCollection.ItemsSource)
                    .DisposeWith(disposables);

                this.Bind(ViewModel, vm => vm.MaterialGroup, v => v.MaterialGroup.Text);
                this.Bind(ViewModel, vm => vm.ItemWeigth, v => v.ItemWeigth.Text);
                this.Bind(ViewModel, vm => vm.ProgressValue, v => v.ProgressBar.Value);
                this.Bind(ViewModel, vm => vm.MaxProgressValue, v => v.ProgressBar.Maximum);
                this.Bind(ViewModel, vm => vm.IsAutoMode, v => v.@switch.IsOn);
                this.BindCommand(ViewModel, vm => vm.OpenReestrSettingCommand, v => v.ReestrSetting);
                this.BindCommand(ViewModel, vm => vm.SaveCommand, v => v.SaveBtn);
                this.BindCommand(ViewModel, vm => vm.EditCommand, v => v.EditCommand, x => x.SelectedReestr);
                this.BindCommand(ViewModel, vm => vm.PrintCommand, v => v.PrintCommand, x => x.SelectedReestr);

                this.Bind(ViewModel, vm => vm.SelectedReestr, v => v.ReestrsCollection.SelectedItem);
                this.Bind(ViewModel, vm => vm.ReestrCount, v => v.ReestrCount.Text);
                this.OneWayBind(ViewModel,
                    vm => vm.NetTotal,
                    v => v.NetSum.Text,
                   x => x.ToString("N"));


                this.Bind(ViewModel, vm => vm.SelectedPrintViewType, v => v.PrintViewTypeCmb.SelectedItem)
                    .DisposeWith(disposables);


                this.OneWayBind(ViewModel,
                    vm => vm.PrinterViewTypes,
                    v => v.PrintViewTypeCmb.ItemsSource)
                    .DisposeWith(disposables);

                this.OneWayBind(ViewModel,
                    vm => vm.ReestrsCollection,
                    v => v.Pagination.ReestrsCollection)
                    .DisposeWith(disposables);

                

            });
       
            this.Events().KeyDown
                        .Where(x => x.Key == Key.I && Keyboard.IsKeyDown(Key.LeftCtrl))
                        .Select(x => Unit.Default)
                        .InvokeCommand(this, x => x.ViewModel.Imitation);

            this.Events().KeyDown
                         .Where(x => x.Key == Key.Enter && (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift)))
                         .Select(x => Unit.Default)
                         .InvokeCommand(ViewModel, x => x.SaveCommand);

            this.WhenAnyValue(x => x.ViewModel)
                     .SelectMany(x => x.Initialize())
                     .Subscribe(x => ViewModel.FillCollection(x));

            ReestrsCollection.Items.CurrentChanged += Items_CurrentChanged;
        }

        private void Items_CurrentChanged(object sender, EventArgs e)
        {
            (ReestrsCollection.ItemsSource as ObservableCollection<Reestr>).CollectionChanged -= ShellView_CollectionChanged;
            (ReestrsCollection.ItemsSource as ObservableCollection<Reestr>).CollectionChanged += ShellView_CollectionChanged;
        }

        private void ShellView_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (ReestrsCollection.Items.Count > 0)
                ReestrsCollection.ScrollIntoView(ReestrsCollection.Items[0]);
        }

        private void PrintViewTypeCmb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MaterailGroupTextBlock != null)
            {
                if (PrintViewTypeCmb.SelectedIndex == 2)
                {
                    MaterailGroupTextBlock.Visibility = Visibility.Visible;
                    MaterialGroup.Visibility = Visibility.Visible;
                }
                else
                {
                    MaterailGroupTextBlock.Visibility = Visibility.Collapsed;
                    MaterialGroup.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
