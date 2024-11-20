using Syncify.Domain.Entities;

namespace Syncify.Domain.Interfaces;
public interface IFriendshipRepository : IGenericRepository<Friendship>
{
    Task<Friendship?> GetFriendshipAsync(string user1Id, string user2Id);

    Task<List<Friendship>> GetUserFriendshipsAsync(string userId);

    Task<List<Friendship>> GetPendingRequestsAsync(string userId);

    Task<bool> AreFriendsAsync(string user1Id, string user2Id);

    Task<int> GetFriendsCountAsync(string userId);
}
