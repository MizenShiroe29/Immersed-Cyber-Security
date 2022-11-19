using Sabio.Data.Providers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sabio.Models.Requests;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using SendGrid.Helpers.Mail;
using SendGrid;
using Microsoft.Extensions.Options;
using Sabio.Models.AppSettings;
using Microsoft.AspNetCore.Hosting;
using System.IO;



namespace Sabio.Services
{
    public class EmailsService : IEmailsService
    {



        private AppKeys _appKeys;
        private readonly IWebHostEnvironment _webHostEnvironment;
         IDataProvider _data = null;

        public EmailsService(IDataProvider data, IOptions<AppKeys> appKeys, IWebHostEnvironment webHostEnvironment)
        {
            _appKeys = appKeys.Value;
            _data = data;
            _webHostEnvironment = webHostEnvironment;
           
        }

        private async Task SendEmail( SendGridMessage msg) 
        {  
            var client = new SendGridClient(_appKeys.SendGridAppKey);
            var response = await client.SendEmailAsync(msg);
        }
        private async Task SendPhishing(SendGridMessage notice)
        {
            var client = new SendGridClient(_appKeys.SendGridKey);
            var response = await client.SendEmailAsync(notice);
   
        }

        public async void WelcomeEmail()
        {
           
            var from = new EmailAddress("fakeEmail@dispostable.com", "Example User");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress("fakeEmail@dispostable.com", "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = GetTemplate();
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent );
            await SendEmail(msg);
        }
        public async void ContactUsEmail(ContactUsAddRequest model)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            var from = new EmailAddress(model.From);
            var subject = model.Subject;
            var to = new EmailAddress("immersed.contact@dispostable.com");            
            string path = Path.Combine(webRootPath, "EmailTemplates", "ContactUsReply.html");
            var htmlContent = File.ReadAllText(path).Replace("{{message}}", model.Message);
            var message = new SendGridMessage()
            {
                From = from,
                Subject = subject,
                ReplyTo = to,
                HtmlContent = htmlContent
            };
            await SendEmail(message);
        }

        public async void SendConfirmContactUsEmail(  ContactUsAddRequest model)
        {
            
            var from = new EmailAddress("immersed.contact@dispostable.com");
            var subject = "Immersed: Confirmation email";           
            var replyto = new EmailAddress(model.From);            
            var htmlContent = ContactUsConfirmation();
            var message = new SendGridMessage()
            {
                From = from,
                Subject = subject,
                HtmlContent = htmlContent
            };
            await SendEmail(message);

           

        }
        public async void SendConfirmEmail(string token, string email)
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            var from = new EmailAddress("fakeEmail@dispostable.com", "Example User");
            var subject = "Immersed Registration Completion Next Step Needed";     
            string path = Path.Combine(webRootPath, "EmailTemplates", "EmailConfirmation.html");
            var htmlContent = File.ReadAllText(path).Replace("{{token}}", token).Replace("{{email}}", email);

            var message = new SendGridMessage()
            {
                From = from,
                Subject = subject,
                HtmlContent = htmlContent
            };
            message.AddTo(new EmailAddress(email));
            await SendEmail(message);
        }

        public string GetTemplate()
        {
            string webRootPath = _webHostEnvironment.WebRootPath;        
            string path = "";
            path = Path.Combine(webRootPath,"EmailTemplates", "WelcomeTemplate.html" );
            string template = File.ReadAllText(path);
            return template;        

        }

        public string ContactUsConfirmation()
        {
            string webRootPath = _webHostEnvironment.WebRootPath;
            string path = "";
            path = Path.Combine(webRootPath, "EmailTemplates", "ContactUsConfirmation.html");
            string template = File.ReadAllText(path);
            return template;
        }


        public async void PhishingEmail()
        {
            var from = new EmailAddress("fakeEmail@dispostable.com", "Example User");
            var subject = "Special offer boost your account account!";
            var to = new EmailAddress("fakeEmail@dispostable.com", "Example User");
            var plainTextContent = "Boost your account account!";
            var htmlContent = PhishingTemplate();
            var notice = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            await SendPhishing(notice);
        }

        public string PhishingTemplate()
        {
            string webRootPath = _webHostEnvironment.WebRootPath;

            string path = "";
            path = Path.Combine(webRootPath, "EmailTemplates", "PhishingEmail.html");

            string phishing = File.ReadAllText(path);
            return phishing;
        }


    }
} 
