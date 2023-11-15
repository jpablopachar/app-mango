using System.Text;
using email_service.Data;
using email_service.Dtos;
using email_service.Interfaces;
using email_service.Message;
using email_service.Models;
using Microsoft.EntityFrameworkCore;

namespace email_service.Services
{
    public class EmailService : IEmailService
    {
        private readonly DbContextOptions<EmailDbContext> _dbContextOptions;

        public EmailService(DbContextOptions<EmailDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }

        /// <summary>Takes a CartDto object, generates an email message with the
        /// cart details, and logs and sends the email asynchronously.</summary>
        /// <param name="CartDto">Represents a shopping cart.</param>
        public async Task EmailCartAndLogAsync(CartDto cartDto)
        {
            StringBuilder message = new();

            message.AppendLine("<br/>Email del carrito de compras solicitado ");
            message.AppendLine("<br/>Total " + cartDto.CartHeader!.CartTotal);
            message.Append("<br/>");
            message.Append("<ul>");

            foreach (var item in cartDto.CartDetails!)
            {
                message.Append("<li>");
                message.Append(item.Product!.Name + " x " + item.Count);
                message.Append("</li>");
            }

            message.Append("</ul>");

            await LogAndEmail(message.ToString(), cartDto.CartHeader.Email!);
        }

        /// <summary>Logs and sends an email notification for a newly placed
        /// order. </summary>
        /// <param name="RewardsMessage">Contains information about a rewards
        /// message.</param>
        public async Task LogOrderPlacedAsync(RewardsMessage rewardsMessage)
        {
            string message = "Nuevo pedido realizado. <br/> Orden ID : " + rewardsMessage.OrderId;

            await LogAndEmail(message, "jpablopachar1993@gmail.com");
        }

        /// <summary>Registers a user's email and logs a message with the email.</summary>
        /// <param name="email">Represents the email address of the user being
        /// registered.</param>
        public async Task RegisterUserEmailAndLogAsync(string email)
        {
            string message = "Usuario registrado correctamente. <br/> Email : " + email;

            await LogAndEmail(message, "jpablopachar1993@gmail.com");
        }

        /// <summary>Logs a message and sends an email, and returns `true` if successful
        /// or `false` if an exception occurs.</summary>
        /// <param name="message">Represents the log message that you want to save and
        /// email.</param>
        /// <param name="email">Is the email address to which the log message should
        /// be sent.</param>
        /// <returns>`Task<bool>`.</returns>
        private async Task<bool> LogAndEmail(string message, string email)
        {
            try
            {
                EmailLogger emailLog = new()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };

                await using var _db = new EmailDbContext(_dbContextOptions);

                await _db.EmailLoggers.AddAsync(emailLog);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}