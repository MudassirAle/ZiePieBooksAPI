using Application.Interface;
using Core.Model;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Infrastructure.Service
{
    public class EmailService : IEmailService
    {
        private readonly MailSetting _mailSetting;

        public EmailService(IOptions<MailSetting> mailSettingOptions)
        {
            _mailSetting = mailSettingOptions.Value;
        }

        public async Task<ServiceResponse<bool>> SendEmailAsync(AppUser appUser, string password)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mailSetting.SenderEmail);
                email.To.Add(MailboxAddress.Parse(appUser.Email));
                email.Subject = $"Welcome To ZiePie Books";
                var builder = new BodyBuilder();
                builder.HtmlBody = GenerateInviteBody(appUser.Email, password);
                email.Body = builder.ToMessageBody();

                using (var smtp = new SmtpClient())
                {
                    ConfigureSmtpClient(smtp);

                    await smtp.SendAsync(email);
                    smtp.Disconnect(true);
                }

                response.IsSuccess = true;
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = $"Error while sending email: {ex.Message}";
            }

            return response;
        }

        private string GenerateInviteBody(string userEmail, string userPassword)
        {
            return $"<!DOCTYPE html>\r\n<html lang=\"en\">\r\n\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">\r\n    <title>Document</title>\r\n</head>\r\n\r\n<body style='font: 400 15px/1.6 \"Segoe UI\",system-ui,-apple-system,sans-serif;margin: 0;padding: 0;'>\r\n    <table align=\"center\" border=\"0\" cellpadding=\"20\" cellspacing=\"0\" id=\"contenttable\" style=\"background-color:#ffffff; text-align:left !important; margin-top:0 !important; margin-right: auto !important; margin-bottom:0 !important; margin-left: auto !important; border:none; width: 100% !important; max-width:600px !important;\" width=\"600\">\r\n        <thead>\r\n            <tr align=\"center\">\r\n                <td style=\"padding:10px 0;background-color:#ffffff;\">\r\n                    <img\r\n                        src=\"https://i.imgur.com/Cgpqzna.png\" width=\"151\" height=\"57\">\r\n                </td>\r\n            </tr>\r\n            <tr align=\"center\">\r\n                <td style=\"background-color: #084887;\">\r\n                    <p style=\"margin: 0;color:#fff;text-transform: uppercase;font-size: 20px;\">You're Invited!</p>\r\n                </td>\r\n            </tr>\r\n            <tr align=\"center\">\r\n                <td>\r\n                    <p style=\"margin:0\">You have been invited to ZiePie Books. You can login with the provided credentials, and set up a new password. Then you can proceed linkning your Bank Accounts.</p>\r\n                </td>\r\n            </tr>\r\n            <tr align=\"center\">\r\n                <td>\r\n                    <a href=\"https://ziepie-books-spa.mangosmoke-c41a2b4c.eastus.azurecontainerapps.io\" style=\"background-color: #e932ac;color:#fff;padding: 10px 15px;text-decoration: none;font-weight: bold;\">SET UP ACCOUNT</a>\r\n                </td>\r\n            </tr>\r\n            <tr align=\"left\">\r\n                <td>\r\n                    <table align=\"center\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"500\" style=\"background-color: #f2f2e5;padding: 50px;\">\r\n                        <thead>\r\n                            <tr>\r\n                                <td style=\"width: 130px;\">\r\n                                    <p style=\"margin: 0;font-size:16px;font-weight: bold;text-align: left;\">USERNAME</p>\r\n                                </td>\r\n                                <td align=\"left\">\r\n                                    <p style=\"margin: 0;font-size: 18px;padding: 5px;background-color: #fff;\">{userEmail}</p>\r\n                                </td>\r\n                            </tr>\r\n                            <tr>\r\n                                <td style=\"width: 130px;\">\r\n                                    <p style=\"font-size:16px;font-weight: bold;text-align: left;\">Password</p>\r\n                                </td>\r\n                                <td align=\"left\">\r\n                                    <p style=\"margin: 0;font-size: 18px;padding: 5px;background-color: #fff;\">{userPassword}</p>\r\n                                </td>\r\n                            </tr>\r\n                        </thead>\r\n                    </table>\r\n                </td>\r\n            </tr>\r\n            <tr align=\"center\">\r\n                <td>\r\n                    <p>Please ignore this email if you aren't the intended recipient</p>\r\n                </td>\r\n            </tr>\r\n        </thead>\r\n    </table>\r\n    \r\n   \r\n\r\n\r\n</body>\r\n\r\n</html>";
        }

        private void ConfigureSmtpClient(SmtpClient smtp)
        {
            smtp.Connect(_mailSetting.Server, _mailSetting.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSetting.SenderEmail, _mailSetting.Password);
        }

        public async Task<ServiceResponse<bool>> Send(Email emailModel)
        {
            var response = new ServiceResponse<bool>();

            try
            {
                var email = new MimeMessage();
                email.Sender = MailboxAddress.Parse(_mailSetting.SenderEmail);
                email.To.Add(MailboxAddress.Parse(emailModel.To));
                email.Subject = emailModel.Subject;

                var builder = new BodyBuilder();
                builder.HtmlBody = emailModel.Body;
                email.Body = builder.ToMessageBody();

                using (var smtp = new SmtpClient())
                {
                    ConfigureSmtpClient(smtp);

                    await smtp.SendAsync(email);
                    smtp.Disconnect(true);
                }

                response.IsSuccess = true;
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = $"Error while sending email: {ex.Message}";
            }

            return response;
        }
    }
}