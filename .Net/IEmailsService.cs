using Sabio.Models.Requests;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace Sabio.Services
{
    public interface IEmailsService
    {
        public void WelcomeEmail();
        public void PhishingEmail();
        public void SendConfirmEmail(string token, string email);

        public void ContactUsEmail(ContactUsAddRequest model);

        public void SendConfirmContactUsEmail(ContactUsAddRequest model);
    }
}