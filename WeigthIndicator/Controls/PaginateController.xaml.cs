using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Controls
{
    /// <summary>
    /// Interaction logic for PaginateController.xaml
    /// </summary>
    public partial class PaginateController : UserControl
    {
        public PaginateController()
        {
            InitializeComponent();
        }
        public ICollectionView ReestrsCollectionView { get; private set; }

        public int PageIndex
        {
            get { return (int)GetValue(PageIndexProperty); }
            set { SetValue(PageIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PageIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PageIndexProperty =
            DependencyProperty.Register("PageIndex", typeof(int), typeof(PaginateController), new PropertyMetadata(1));



        public int TotalPages
        {
            get { return (int)GetValue(TotalPagesProperty); }
            set { SetValue(TotalPagesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TotalPages.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TotalPagesProperty =
            DependencyProperty.Register("TotalPages", typeof(int), typeof(PaginateController), new PropertyMetadata(1));

        public int ItemsPerPage
        {
            get { return (int)GetValue(ItemsPerPageProperty); }
            set { SetValue(ItemsPerPageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemsPerPage.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsPerPageProperty =
            DependencyProperty.Register("ItemsPerPage", typeof(int), typeof(PaginateController), new PropertyMetadata(1));




        public ObservableCollection<Reestr> ReestrsCollection
        {
            get { return (ObservableCollection<Reestr>)GetValue(ReestrsCollectionProperty); }
            set { SetValue(ReestrsCollectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ReestrsCollection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ReestrsCollectionProperty =
            DependencyProperty.Register("ReestrsCollection", typeof(ObservableCollection<Reestr>), typeof(PaginateController), new PropertyMetadata(new ObservableCollection<Reestr>(), OnCollectionChanged));


        private static void OnCollectionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as PaginateController).OnCollectioChnaged();
        }

        private void OnCollectioChnaged()
        {
            if (ReestrsCollection != null)
            {
                ReestrsCollectionView = CollectionViewSource.GetDefaultView(ReestrsCollection);
                ReestrsCollectionView.SortDescriptions.Add(new SortDescription("PackingDate", ListSortDirection.Descending));
            }
        }
    }
}
