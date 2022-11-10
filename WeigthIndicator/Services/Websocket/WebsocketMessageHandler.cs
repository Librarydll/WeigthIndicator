using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeigthIndicator.Domain.Exceptions;
using WeigthIndicator.Domain.Models;
using WeigthIndicator.Domain.Services;
using WeigthIndicator.Shared.Models;

namespace WeigthIndicator.Services
{
    public class WebsocketMessageHandler : IWebsocketMessageHandler
    {
        public static WebsocketRootModel None { get; } = new WebsocketRootModel { MessageType = MessageType.None };

        private readonly IOutcomeDataService _outcomeDataService;
        private readonly IUserDataService _userDataService;

        public event Action<ReestrQrObject> ReestrAddedEvent;
        public event Action<ReestrQrObject> ReestrRemovedEvent;
        public WebsocketMessageHandler(
            IOutcomeDataService outcomeDataService ,
            IUserDataService userDataService)
        {
            _outcomeDataService = outcomeDataService;
            _userDataService = userDataService;
        }


        public async Task<string> HandleMessage(string data)
        {
            var root = JsonConvert.DeserializeObject<WebsocketRootModel>(data);
            Task<WebsocketRootModel> task =null;
            switch (root.MessageType)
            {
                case MessageType.Authorize:
                    task = HandleAuthorize(root);
                    break;
                case MessageType.CreateOutcomeModel:
                    task = HandleCreateOutcome(root);
                    break;
                case MessageType.ReestrAdded:
                    task = HandleReestrAdded(root);
                    break;
                case MessageType.ReestrRemoved:
                    task = HandleReestrRemoved(root);
                    break;
                default:
                    break;
            }
            if (task != null)
            {
                var result = await task;
                return JsonConvert.SerializeObject(result);
            }
            return string.Empty;
        }

        private Task<WebsocketRootModel> HandleReestrRemoved(WebsocketRootModel root)
        {
            ReestrRemovedEvent?.Invoke(root.Reestr);
            return Task.FromResult(None);
        }

        private Task<WebsocketRootModel> HandleReestrAdded(WebsocketRootModel root)
        {
            ReestrAddedEvent?.Invoke(root.Reestr);
            return Task.FromResult(None);
        }

        private async Task<WebsocketRootModel> HandleAuthorize(WebsocketRootModel model)
        {
            var authenticationResult = await _userDataService.Login(model.AuthorizeModel.Login, model.AuthorizeModel.Password);
            var preparedModel = new AuthorizeResult
            {
                ErrorMessage = authenticationResult.Error,
                FIO = authenticationResult.User?.FullName,
                Id = authenticationResult.User == null ? 0 : authenticationResult.User.Id,
                IsSuccess = authenticationResult.IsSuccess
            };

            return new WebsocketRootModel { MessageType = MessageType.AuthorizeResult, AuthorizeResult = preparedModel };
        }
        private async Task<WebsocketRootModel> HandleCreateOutcome(WebsocketRootModel root)
        {
            var outComeRequest = root.CreateOutcomeModel;
            var outcomeItems = outComeRequest.Reestrs.Select(x => new OutcomeItem
            {
                Count = x.Net,
                ReestrId = x.Id
            });

            var outcome = new Outcome
            {
                Comment = outComeRequest.Comment,
                Created = DateTime.Now,
                Title = outComeRequest.Title,
                UserId = outComeRequest.UserId,
                OutcomeItems = outcomeItems.ToList()
            };
            var msgType = MessageType.None;
            ReestrQrObject reestr = null;
            try
            {
                await _outcomeDataService.CreateOutcome(outcome);
            }
            catch (ReestrAlreadyOutcomedException ex)
            {
                msgType = MessageType.CreateOutcomeResult;
                reestr = new ReestrQrObject
                {
                    BarrelNumber = ex.Reestr.BarrelNumber,
                    BatchNumber = ex.Reestr.BatchNumber,
                    Id = ex.Reestr.Id,
                    Net = ex.Reestr.Net,
                    PackingDate = ex.Reestr.PackingDate,
                    ProductionDate = ex.Reestr.BarrelStorage.ProductionDate,
                    ProductionTitle = ex.Reestr.Recipe.LongNameRu
                };
            }
            catch(Exception ex)
            {
                msgType = MessageType.CreateOutcomeResult;
            }

            return new WebsocketRootModel
            {
                MessageType = msgType,
                CreateOutcomeResult = new CreateOutcomeResult
                {
                    IsSuccuess = msgType  == MessageType.None,
                    Reestr = reestr
                }
            };
        }

    }
}
