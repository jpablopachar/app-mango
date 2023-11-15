using email_service.Models;
using Microsoft.EntityFrameworkCore;

namespace email_service.Data
{
    public class EmailDbContext : DbContext
    {
        public EmailDbContext(DbContextOptions<EmailDbContext> options) : base(options) { }

        public DbSet<EmailLogger> EmailLoggers { get; set; }
    }
}