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
        public const string CanNotBeFriendOfYourself = "Can not be a friend of yourself";
    }

    public static class Users
    {
        public const string UserNotExists = "User is not exist.";
        public const string UserUnauthorized = "You are not authenticated";

        public const string UnableToCreateAccount =
            "Some errors happened when creating your account, please try again !!";

        public const string UnkownUser = "Unknown User.";
        public const string UnableToUpdateUser = "Unable to Update The User.";
        public const string InvalidAuthCode = "Invalid authentication code.";
        public const string AuthCodeExpired = "Authentication code is expired.";
        public const string AlreadyEmailConfirmed = "Email is cofirmed yet.";
        public const string UserNotFound = "User '{0}' not found.";
        public const string CannotCreateFbUser = "Can not create facebook user.";
        public const string FbFailedAuthentication = "Facebook authentication failed!";
        public const string EmailNotConfirmed = "Email is not confirmed.";
        public const string InvalidCredientials = "Invalid email or password.";
        public const string CodeExpired = "Code has expired. Please request a new reset code.";
        public const string UserHasPrivacy = "User has a privacy setting.";
        public const string UserNotHasPrivacySetting = "User not have privacy settings";
        public const string Invalid2FaCode = "Invalid 2FA Code.";
        public const string InvalidTokenProvider = "Invalid 2FA Token Provider.";

        public const string TwoFactorRequired =
            "Two Factor Authentication Required To Complete Login, check your inbox and verify your 2fa code.";

        public const string TwoFactorAlreadyDisabled = "Two-factor authentication is already disabled for this user.";
        public const string Disable2FaFailed = "Failed to disable two-factor authentication.";
    }

    public static class Roles
    {
        public const string ErrorCreatingRole = "Error creating role '{0}'.";
        public const string RoleNotFound = "Role not found: '{0}'.";
        public const string ErrorUpdatingRole = "Error updating role '{0}'.";
        public const string ErrorDeletingRole = "Error deleting role '{0}'.";
        public const string ErrorAssigningRole = "Error assigning role '{0}' to user '{1}'.";
        public const string ErrorAddingClaim = "Error adding claim to {0}'.";
        public const string ErrorAddingClaimToRole = "Error adding claim to '{0}' role.";
    }

    public static class Conversation
    {
        public const string ConversationNotExisted = "Conversation with id '{0}' was not existed";
        public const string ShouldStartConversation = "You should start a conversation first.";
        public const string CanNotStartConversationToSelf = "You can not start conversation to yourself.";
        public const string NoConversationBetweenThem = "There is an existed conversation between {0} and {1}";
    }

    public static class Messages
    {
        public const string CanNotSendMessagesToSelf = "You can not send messages to yourself.";
        public const string MessageNotFound = "Message not found.";
    }
}
