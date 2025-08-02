using ConversationService.Domain.Entities;
using ConversationService.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ConversationService.Infrastructure.DbContext;

public class ConversationServiceContext : Microsoft.EntityFrameworkCore.DbContext
{
    private const string SCHEMA = "conversation";

    public ConversationServiceContext(DbContextOptions<ConversationServiceContext> options) : base(options) 
    {
    }
    
    public DbSet<Conversation> Conversations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SCHEMA);
        modelBuilder.ApplyConfiguration(new ConversationConfiguration());
    }
}