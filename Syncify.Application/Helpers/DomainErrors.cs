namespace Syncify.Application.Helpers;
public static class DomainErrors
{
    public static class Following
    {
        public const string CanNotFollowYourself = "You can not follow yourself";
        public const string CanNotUnFollowYourself = "You cannot unfollow yourself.";
        public const string NoFollowing = "No following between them";
    }

    public static class Friendship
    {
        public const string CanNotSendFriendRequestToYourSelf = "Cannot send friend request to yourself";
        public const string PendingFriendRequest = "Friend request already sent";
        public const string BlockedFriendRequest = "Unable to send friend request";
        public const string RejectedFriendRequest = "Your friendship request rejected";
        public const string AlreadyAcceptedFriendRequest = "Users are already friends";
        public const string UndefindFriendRequestStatus = "Invalid friend request status";
        public const string UnableToCreateFriendRequest = "Unable to create friend Request.";
        public const string NotFoundFriendRequest = "Friend request not found";
        public const string UnauthorizedToAcceptFriendRequest = "Not authorized to accept this friend request";
        public const string FriendRequestMustBePending = "Friend request must be pending to accept";
    }

    public static class Users
    {
        public const string UserNotExists = "User is not exist.";
    }
}
