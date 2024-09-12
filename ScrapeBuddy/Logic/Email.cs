namespace ScrapeBuddy.Logic;
using System;
using System.Net;
using System.Net.Mail;
public class Email
{
    
    private SmtpClient _smtp;
    public Email()
    {
        _smtp= new SmtpClient();
        _smtp.Host = "smtp.mailersend.net";
        _smtp.Port = 587;
        
        _smtp.UseDefaultCredentials = false;
        _smtp.Credentials = new NetworkCredential("MS_T59Hzo@trial-351ndgwp9xxlzqx8.mlsender.net","t4oSEsMk76d9bqMC");
        _smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
        _smtp.Timeout = 20000;
        _smtp.EnableSsl = true;

    }
    /// <summary>
    /// Sends an email to the specified email adresss;
    /// </summary>
    /// <param name="recipientEmailAdress"></param>
    /// <param name="subject"> The subject of the email</param>
    /// <param name="message"> the message of the email</param>
    /// <param name "username"> The username of the ricipient email</param>
    public void Send(string recipientEmailAdress, string subject,string username, string message)
    {
        MailMessage mailMessage = new MailMessage();
        mailMessage.From = (new MailAddress("MS_T59Hzo@trial-351ndgwp9xxlzqx8.mlsender.net"));
        mailMessage.To.Add(new MailAddress(recipientEmailAdress));
        mailMessage.Subject = subject;
        String messageBody = "Hello " + username + ",<br><br><br>";
        messageBody += "<strong>"+subject + "</strong><br>";
        messageBody += message +"<br><br>";
        messageBody += "Greets, your Scrapebuddy Team";
        
        mailMessage.Body = messageBody;
        mailMessage.IsBodyHtml = true;
        _smtp.Send(mailMessage);

    }
}