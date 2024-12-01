using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Syncify.Domain.Entities;
using Syncify.Domain.Entities.Identity;
using Syncify.Domain.Enums;
using Syncify.Domain.Interfaces;

namespace Syncify.Infrastructure.Persistence.Repositories;
public sealed class FriendshipRepository(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager) : GenericRepository<Friendship>(context), IFriendshipRepository
{
    public async Task<Friendship?> GetFriendshipAsync(string user1Id, string user2Id)
        => await context.FriendshipRequests
            .AsNoTracking()
            .Include(f => f.Requester)
            .Include(f => f.Receiver)
            .FirstOrDefaultAsync(f => f.RequesterId == user1Id && f.ReceiverId == user2Id ||
                                      f.RequesterId == user2Id && f.ReceiverId == user1Id);

    public async Task<List<Friendship>> GetUserFriendshipsAsync(string userId)
        => await context.FriendshipRequests
            .AsNoTracking()
            .Include(f => f.Requester)
            .Include(f => f.Receiver)
            .Where(f =>
                (f.RequesterId == userId || f.ReceiverId == userId)
                && f.FriendshipStatus == FriendshipStatus.Accepted)
            .ToListAsync();

    public async Task<List<Friendship>> GetPendingRequestsForRequesterAsync(string requesterId)
        => await context.FriendshipRequests
            .AsNoTracking()
            .Include(f => f.Requester)
            .Include(f => f.Receiver)
            .Where(f => f.RequesterId == requesterId &&
                        f.FriendshipStatus == FriendshipStatus.Pending)
            .ToListAsync();

    public async Task<List<Friendship>> GetPendingRequestsForReceiverAsync(string receiverId)
        => await context.FriendshipRequests
            .AsNoTracking()
            .Include(f => f.Requester)
            .Include(f => f.Receiver)
            .Where(f => f.ReceiverId == receiverId && f.FriendshipStatus == FriendshipStatus.Pending)
            .ToListAsync();

    public async Task<bool> AreFriendsAsync(string user1Id, string user2Id)
        => await context.FriendshipRequests
            .AsNoTracking()
            .Include(f => f.Requester)
            .Include(f => f.Receiver)
            .AnyAsync(f => ((f.RequesterId == user1Id && f.ReceiverId == user2Id) ||
                            (f.RequesterId == user2Id && f.ReceiverId == user1Id)) &&
                      f.FriendshipStatus == FriendshipStatus.Accepted);

    public async Task<int> GetFriendsCountAsync(string userId)
        => await context.FriendshipRequests
            .AsNoTracking()
            .Include(f => f.Receiver)
            .Include(f => f.Requester)
            .CountAsync(f => f.RequesterId == userId || f.ReceiverId == userId);

    public async Task<List<Friendship>> GetAcceptedFriendshipsForReceiverAsync(string receiverId)
    {
        var receivedFriendrequests = await userManager.Users
            .AsNoTracking()
            .Where(user => user.Id == receiverId)
            .SelectMany(user => user.ReceivedFriendRequests)
            .Include(f => f.Receiver)
            .Include(f => f.Requester)
            .Where(f => f.FriendshipStatus == FriendshipStatus.Accepted)
            .ToListAsync();

        return receivedFriendrequests;
    }


}
