using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeigthIndicator.Models
{
    public class SelectableItem<T> : ReactiveObject
    {
        public event Action<SelectableItem<T>> SelectedChanged;
        private bool _IsSelected;
        public bool IsSelected
        {
            get => _IsSelected;
            set
            {
                this.RaiseAndSetIfChanged(ref _IsSelected, value);
                SelectedChanged?.Invoke(this);
            }
        }
        public T Item { get; set; }
    }
}
