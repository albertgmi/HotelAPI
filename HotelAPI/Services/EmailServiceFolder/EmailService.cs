using HotelAPI.Entities;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
namespace HotelAPI.Services.EmailServiceFolder
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUser;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;

        public EmailService(IConfiguration configuration)
        {
            var emailSettings = configuration.GetSection("EmailSettings");
            _smtpServer = emailSettings["SmtpServer"];
            _smtpPort = int.Parse(emailSettings["SmtpPort"]);
            _smtpUser = emailSettings["SmtpUser"];
            _smtpPassword = Environment.GetEnvironmentVariable("SmtpPassword", EnvironmentVariableTarget.Machine);
            _fromEmail = emailSettings["FromEmail"];
        }
        public async Task SendEmailAsync(Hotel hotel, Room room, User user, Reservation reservation)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_fromEmail));
            email.To.Add(MailboxAddress.Parse(user.Email));
            email.Subject = $"New reservation in hotel {hotel.Name} has been created!";
            email.Body = new TextPart("plain")
            {
                Text = $"Dear {user.FirstName},\n\n" +
                       $"Your reservation for room {room.Name} in hotel {hotel.Name} has been successfully created.\n\n" +
                       $"Check-in Date: {reservation.CheckInDate:yyyy-MM-dd}\n" +
                       $"Check-out Date: {reservation.CheckOutDate:yyyy-MM-dd}\n" +
                       $"Total Price: {reservation.TotalPrice} PLN\n\n" +
                       $"To secure your reservation, please ensure that the total amount is paid before your check-in date.\n" +
                       $"Thank you for choosing us! We look forward to welcoming you at {hotel.Name}.\n" +
                       $"If you have any questions or need further assistance, feel free to contact us.\n\n" +
                       $"Best regards,\n" +
                       $"The {hotel.Name} Team"
            };
            using var smtp = new SmtpClient();
            try
            {
                await smtp.ConnectAsync(_smtpServer, _smtpPort, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_smtpUser, _smtpPassword);
                await smtp.SendAsync(email);
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
