using Azure;
using Azure.Communication.Email;

namespace dionizos_backend_app
{
    public interface IMailing
    {
        public Response<EmailSendResult>  SendEmail(List<EmailAddress> recipients, string htmlEmailContent, string emailTitle);
        public Response<EmailSendResult> SendEmailCode(string email, string code, string? displayName = null);
    }
    public class Mailing: IMailing
    {
        public Response<EmailSendResult> SendEmail(List<EmailAddress> recipients, string htmlEmailContent, string emailTitle)
        {
            string? commsKey = System.Environment.GetEnvironmentVariable("COMMUNICATIONS_CONNSTRING");
            if (commsKey is null)
            {
                throw new NullReferenceException("COMMUNICATIONS_CONNSTRING is null");
            }
            EmailClient emailClient = new EmailClient(commsKey);
            EmailContent emailContent = new EmailContent(emailTitle);
            emailContent.Html = htmlEmailContent;
            List<EmailAddress> emailAddresses = recipients;
            EmailRecipients emailRecipients = new EmailRecipients(emailAddresses);
            EmailMessage emailMessage = new EmailMessage("dionizos@2fddd5a2-7a02-4962-a40e-0dd9ceffb1df.azurecomm.net", emailRecipients, emailContent);
            var emailResult = emailClient.Send(WaitUntil.Started,emailMessage, CancellationToken.None);
            var result = emailResult.WaitForCompletionAsync();
            return result.Result;
        }

        public Response<EmailSendResult> SendEmailCode(string email, string code, string? displayName = null)
        {
            var list = new List<EmailAddress> { new EmailAddress(email, displayName ?? email) };
            string body = "Welcome in Dionizos. Your code is: " + code;
            return SendEmail(list,body, "Dionizos - Activation Code");
        }
    }
}
