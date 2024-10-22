using Contracts.Services;
using Infrastructure.Configurations;
using MailKit.Net.Smtp;
using MimeKit;
using Serilog;
using Shared.Services.Email;

namespace Infrastructure.Services;

public class EmailSMTPService : IEmailSMTPService
{
    private readonly ILogger _logger;
    private readonly EmailSMTPSetting _emailSetting;
    private readonly SmtpClient _smtpClient;

    public EmailSMTPService(ILogger logger, EmailSMTPSetting emailSetting)
    {
        _logger = logger;
        _emailSetting = emailSetting;
        _smtpClient = new SmtpClient();
    }

    public async Task SendEmailAsync(MailRequest request, CancellationToken cancellationToken = default)
    {
        var emailMessage = new MimeMessage
        {
            Sender = new MailboxAddress(_emailSetting.DisplayName, request.From ?? _emailSetting.From),
            Subject = request.Subject,
            Body = new BodyBuilder
            {
                HtmlBody = request.Body
            }.ToMessageBody()
        };

        if (request.ToAddresses.Any())
        {
            foreach (var toAddress in request.ToAddresses)
            {
                emailMessage.To.Add(MailboxAddress.Parse(toAddress));
            }
        }
        else
        {
            emailMessage.To.Add(MailboxAddress.Parse(request.ToAddress));
        }
        try
        {
            await _smtpClient.ConnectAsync(_emailSetting.SMTPServer, _emailSetting.Port,
                _emailSetting.UseSsl, cancellationToken);
            await _smtpClient.AuthenticateAsync(_emailSetting.Username, _emailSetting.Password, cancellationToken);
            await _smtpClient.SendAsync(emailMessage, cancellationToken);
            await _smtpClient.DisconnectAsync(true, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.Error(ex.Message, ex);
        }
        finally
        {
            await _smtpClient.DisconnectAsync(true, cancellationToken);
            _smtpClient.Dispose();
        }
    }
}