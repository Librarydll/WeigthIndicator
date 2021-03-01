using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeigthIndicator.Models
{
    public class FilterModel:ReactiveObject
    {
        public const string pattern = @"(\d+)(\s)(\d+)";
        private Regex _regex;
        [Reactive] public string SearchQuery { get; set; }
        [Reactive] public DateTime FromDate { get; set; }
        [Reactive] public DateTime ToDate { get; set; }
        [Reactive] public bool IsToDateInclude { get; set; }

        public FilterModel()
        {
            FromDate = DateTime.Now;
            ToDate = DateTime.Now.AddDays(1);
            IsToDateInclude = true;
            SearchQuery = string.Empty;
            _regex = new Regex(pattern);
        }

     
    }
}
