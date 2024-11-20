using Microsoft.AspNetCore.Identity;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;
using Syncify.Application.Interfaces.Services;
using Syncify.Domain.Entities;
using Syncify.Domain.Entities.Identity;
using Syncify.Domain.Enums;
using Syncify.Domain.Interfaces;
using Syncify.Domain.Specifications;
using System.Net;

namespace Syncify.Services;
public sealed class FriendshipService(
    IFriendshipRepository friendshipRepository,
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork) : IFriendshipService
{
    public async Task<Result<FriendshipResponseDto>> SendFriendRequestAsync(
        SendFriendshipRequestDto sendFriendshipRequest)
    {
        var requester = await userManager.FindByIdAsync(sendFriendshipRequest.RequesterId);

        var addressee = await userManager.FindByIdAsync(sendFriendshipRequest.AddresseeId);

        if (requester is null || addressee is null)
            return Result<FriendshipResponseDto>.Failure(HttpStatusCode.NotFound, "User not found");

        if (sendFriendshipRequest.AddresseeId == sendFriendshipRequest.RequesterId)
            return Result<FriendshipResponseDto>.Failure(HttpStatusCode.BadRequest, "Cannot send friend request to yourself");


        var existingFriendship = await friendshipRepository.GetFriendshipAsync(
            sendFriendshipRequest.RequesterId,
            sendFriendshipRequest.AddresseeId);

        if (existingFriendship is not null)
        {
            return Result<FriendshipResponseDto>.Failure(
                HttpStatusCode.BadRequest,
                existingFriendship.FriendshipStatus switch
                {
                    FriendshipStatus.Pending => "Friend request already sent",
                    FriendshipStatus.Accepted => "Users are already friends",
                    FriendshipStatus.Blocked => "Unable to send friend request",
                    FriendshipStatus.Rejected => "Your friendship request rejected",
                    _ => "Invalid friendship status"
                });
        }

        var friendShip = new Friendship
        {
            Id = Guid.NewGuid(),
            RequesterId = sendFriendshipRequest.RequesterId,
            ReceiverId = sendFriendshipRequest.AddresseeId,
            FriendshipStatus = FriendshipStatus.Pending,
            UpdatedAt = DateTimeOffset.Now
        };

        var createdFriendship = await friendshipRepository.CreateAsync(friendShip);

        var friendShipWithRequesterAndReceiver =
            await friendshipRepository.GetBySpecificationAndIdAsync(
                new FriendshipRequestWithRequesterAndReceiverSpecification(),
                createdFriendship.Id);

        await unitOfWork.SaveChangesAsync();

        if (friendShipWithRequesterAndReceiver != null)
            return Result<FriendshipResponseDto>.Success(new FriendshipResponseDto(
                string.Concat(
                    friendShipWithRequesterAndReceiver.Requester.FirstName, " ",
                    friendShipWithRequesterAndReceiver.Requester.LastName),
                string.Concat(friendShipWithRequesterAndReceiver.Receiver.FirstName, " ",
                    friendShipWithRequesterAndReceiver.Receiver.LastName),
                friendShipWithRequesterAndReceiver.FriendshipStatus.ToString(),
                friendShipWithRequesterAndReceiver.CreatedAt,
                friendShipWithRequesterAndReceiver.UpdatedAt
            ));

        return Result<FriendshipResponseDto>.Failure(
            HttpStatusCode.BadRequest,
            "Unable to create friend Request.");
    }

    public async Task<Result<FriendshipResponseDto>> AcceptFriendRequestAsync(AcceptFriendRequestDto acceptFriendRequest)
    {
        var friendship = await friendshipRepository.GetByIdAsync(acceptFriendRequest.FriendshipId);

        if (friendship == null)
            return Result<FriendshipResponseDto>.Failure(HttpStatusCode.NotFound, "Friend request not found");

        if (friendship.ReceiverId != acceptFriendRequest.UserId)
            return Result<FriendshipResponseDto>.Failure(HttpStatusCode.Unauthorized, "Not authorized to accept this friend request");

        if (friendship.FriendshipStatus != FriendshipStatus.Pending)
            return Result<FriendshipResponseDto>.Failure(HttpStatusCode.BadRequest, "Friend request is not pending");

        friendship.FriendshipStatus = FriendshipStatus.Accepted;
        friendship.UpdatedAt = DateTimeOffset.Now;

        friendshipRepository.Update(friendship);

        await unitOfWork.SaveChangesAsync();

        // Send notification to requester

        var friendShipWithRequesterAndReceiver =
            await friendshipRepository.GetBySpecificationAndIdAsync(
                new FriendshipRequestWithRequesterAndReceiverSpecification(),
                friendship.Id);

        return Result<FriendshipResponseDto>.Success(new FriendshipResponseDto(
            string.Concat(
                friendShipWithRequesterAndReceiver.Requester.FirstName, " ",
                friendShipWithRequesterAndReceiver.Requester.LastName),
            string.Concat(friendShipWithRequesterAndReceiver.Receiver.FirstName, " ",
                friendShipWithRequesterAndReceiver.Receiver.LastName),
            friendShipWithRequesterAndReceiver.FriendshipStatus.ToString(),
            friendShipWithRequesterAndReceiver.CreatedAt,
            friendShipWithRequesterAndReceiver.UpdatedAt
        ));

    }
}
