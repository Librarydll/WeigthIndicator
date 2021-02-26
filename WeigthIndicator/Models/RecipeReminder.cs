using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeigthIndicator.Models
{
    public class RecipeReminder:ReactiveObject
    {
        [Reactive] public string RecipeShortName { get; set; }
        [Reactive] public double Remainder { get; set; }
        [Reactive] public bool IsCritical { get; set; }
    }
}
