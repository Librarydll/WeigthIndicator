using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeigthIndicator.Models.Infrastructure
{
    public class PaginationHelper : BindableBase
    {
        public event Action ItemsPerPageChanged;
        private int _itemsPerPage = 10;
        public int ItemsPerPage
        {
            get { return _itemsPerPage; }
            set
            {
                SetProperty(ref _itemsPerPage, value);
                ItemsPerPageChanged?.Invoke();
            }
        }

        private int _currentPage = 1;
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                SetProperty(ref _currentPage, value);
                RaisePropertyChanged(nameof(HasNext));
                RaisePropertyChanged(nameof(HasPrevious));
                RaisePropertyChanged(nameof(ItemNumberFromBegining));
                RaisePropertyChanged(nameof(ItemNumberFromEnd));
            }
        }
        public int ItemNumberFromBegining
        {
            get
            {
                if (CurrentPage == 1) return 1;
                return (CurrentPage - 1) * ItemsPerPage;
            }
        }
        public int ItemNumberFromEnd
        {
            get
            {
                if (CurrentPage * ItemsPerPage > ItemsCount)
                {
                    return ItemsCount;
                }
                return CurrentPage * ItemsPerPage;
            }
        }

        private int _itemsCount;
        public int ItemsCount
        {
            get => _itemsCount;
            set
            {
                SetProperty(ref _itemsCount, value);
                RaisePropertyChanged(nameof(ItemNumberFromBegining));
                RaisePropertyChanged(nameof(ItemNumberFromEnd));
                RaisePropertyChanged(nameof(HasNext));
                RaisePropertyChanged(nameof(HasPrevious));
            }
        }

        public bool HasNext => CurrentPage * ItemsPerPage < ItemsCount;
        public bool HasPrevious => CurrentPage > 1;
    }

    public class ItemPerPage
    {
        public int ItemsCount { get; set; }

        public static IEnumerable<ItemPerPage> GetItemPerPages()
        {
            yield return new ItemPerPage { ItemsCount = 10 };
            yield return new ItemPerPage { ItemsCount = 20 };
            yield return new ItemPerPage { ItemsCount = 50 };
        }
    }
}
