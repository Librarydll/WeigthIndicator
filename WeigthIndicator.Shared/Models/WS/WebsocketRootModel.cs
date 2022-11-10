using System;
using System.Collections.Generic;
using System.Text;
using WeigthIndicator.Shared.Models;

namespace WeigthIndicator.Shared.Models
{
    public class WebsocketRootModel
    {
        public MessageType MessageType { get; set; }
        public AuthorizeModel AuthorizeModel { get; set; }
        public AuthorizeResult AuthorizeResult { get; set; }
        public CreateOutcomeModel CreateOutcomeModel { get; set; }
        public CreateOutcomeResult CreateOutcomeResult { get; set; }
        public ReestrQrObject Reestr { get; set; }
    }

    public enum MessageType
    {
        None,
        Authorize,
        AuthorizeResult,
        CreateOutcomeModel,
        CreateOutcomeResult,
        ReestrAdded,
        ReestrRemoved
    }
}
