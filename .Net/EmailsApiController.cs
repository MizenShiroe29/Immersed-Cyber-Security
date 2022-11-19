using Microsoft.AspNetCore.Mvc;
using Sabio.Services;
using Sabio.Web.Controllers;
using Microsoft.Extensions.Logging;
using Sabio.Web.Models.Responses;
using System;
using Sabio.Models.Requests;

namespace Sabio.Web.Api.Controllers
{
    [Route("api/emails")]
    [ApiController]
    public class EmailsApiController : BaseApiController
    {

        private IEmailsService _service = null;
        private IAuthenticationService<int> _authService = null;

        public EmailsApiController(IEmailsService service,
            ILogger<EmailsApiController> logger, IAuthenticationService<int> authService) : base(logger)
        {
            _service = service;
            _authService = authService;
        }


        [HttpPost]
        public ActionResult<ItemResponse<int>> Create(EmailsAddRequest model)
        {

            ObjectResult result = null;

            try
            {            
                
                _service.WelcomeEmail();
                ItemResponse<int> response = new ItemResponse<int>();
                result = Created201(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }

            return result;
        }
        [HttpPost("contact")]
        public ActionResult<ItemResponse<int>> OnContact(ContactUsAddRequest model)
        {

            ObjectResult result = null;

            try
            {
               _service.ContactUsEmail(model);
               _service.SendConfirmContactUsEmail(model);

                ItemResponse<int> response = new ItemResponse<int>();
                result = Created201(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }

            return result;
        }


        [HttpPost("test")]
        public ActionResult<ItemResponse<int>> Add(EmailsAddRequest model)
        {

            ObjectResult result = null;

            try
            {
                _service.PhishingEmail();
                ItemResponse<int> response = new ItemResponse<int>();
                result = Created201(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
                ErrorResponse response = new ErrorResponse(ex.Message);
                result = StatusCode(500, response);
            }

            return result;
        }
    }
}
