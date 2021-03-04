using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Models;

namespace WeigthIndicator.Events
{
    public class RecipeChangedEvent: PubSubEvent<Recipe>{}

    public class ReestredAddedEvent
    {
        public Reestr Reestr { get; set; }
    }
}
