using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Models.ViewModels;

namespace WeigthIndicator.Events
{
    public class RecipeChangedEvent: PubSubEvent<Recipe>{}

    public class ReestredAddedEvent
    {
        public ReestrObject Reestr { get; set; }
    }
}
