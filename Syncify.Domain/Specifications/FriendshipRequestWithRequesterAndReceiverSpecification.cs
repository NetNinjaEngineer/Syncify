using Syncify.Domain.Entities;

namespace Syncify.Domain.Specifications;
public sealed class FriendshipRequestWithRequesterAndReceiverSpecification : BaseSpecification<Friendship>
{
    public FriendshipRequestWithRequesterAndReceiverSpecification()
    {
        AddIncludes(f => f.Requester);
        AddIncludes(f => f.Receiver);
    }
}
