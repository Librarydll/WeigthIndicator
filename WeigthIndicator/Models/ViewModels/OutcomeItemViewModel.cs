using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Models.ViewModels
{
    public class OutcomeItemViewModel : ReactiveObject
    {
        public int Id { get; set; }
        [Reactive] public double Count { get; set; }
        public string Comment { get; set; }
        public int ReestrId { get; set; }
        public Reestr Reestr { get; set; }
        public int OutcomeId { get; set; }

        public static IEnumerable<OutcomeItemViewModel> BuildIn(IEnumerable<OutcomeItem> items)
        {
            foreach (var item in items)
            {
                yield return new OutcomeItemViewModel
                {
                    Comment = item.Comment,
                    Count = item.Count,
                    OutcomeId = item.OutcomeId,
                    Reestr = item.Reestr,
                    ReestrId = item.ReestrId,
                    Id =item.Id
                };
            }
        }

        public static IEnumerable<OutcomeItem> BuildOut(IEnumerable<OutcomeItemViewModel> items)
        {
            foreach (var item in items)
            {
                yield return new OutcomeItem
                {
                    Comment = item.Comment,
                    Count = item.Count,
                    Id = item.Id,
                    OutcomeId = item.OutcomeId,
                    ReestrId = item.ReestrId
                };
            }
        }
    }
}
