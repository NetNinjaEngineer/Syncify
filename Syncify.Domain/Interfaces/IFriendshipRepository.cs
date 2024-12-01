using Syncify.Domain.Entities;

namespace Syncify.Domain.Interfaces;
public interface IFriendshipRepository : IGenericRepository<Friendship>
{
    Task<Friendship?> GetFriendshipAsync(string user1Id, string user2Id);

    Task<List<Friendship>> GetUserFriendshipsAsync(string userId);

    Task<List<Friendship>> GetPendingRequestsForRequesterAsync(string requesterId);

    Task<List<Friendship>> GetPendingRequestsForReceiverAsync(string receiverId);

    Task<bool> AreFriendsAsync(string user1Id, string user2Id);

    Task<int> GetFriendsCountAsync(string userId);

    Task<List<Friendship>> GetAcceptedFriendshipsForReceiverAsync(string receiverId);
}
