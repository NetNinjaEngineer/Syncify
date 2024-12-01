using Syncify.Domain.Entities;
using Syncify.Domain.Interfaces;

namespace Syncify.Infrastructure.Persistence.Repositories;
public sealed class MessageRepository(ApplicationDbContext context)
    : GenericRepository<Message>(context), IMessageRepository
{
}
