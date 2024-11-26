using Microsoft.AspNetCore.Identity;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;
using Syncify.Application.Helpers;
using Syncify.Application.Interfaces.Services;
using Syncify.Domain.Entities;
using Syncify.Domain.Entities.Identity;
using Syncify.Domain.Enums;
using Syncify.Domain.Interfaces;
using Syncify.Domain.Specifications;
using System.Net;

namespace Syncify.Services.Services;
public sealed class FriendshipService(
    UserManager<ApplicationUser> userManager,
    IUnitOfWork unitOfWork) : IFriendshipService
{
    public async Task<Result<FriendshipResponseDto>> SendFriendRequestAsync(
        SendFriendshipRequestDto sendFriendshipRequest)
    {
        var requester = await userManager.FindByIdAsync(sendFriendshipRequest.RequesterId);

        var addressee = await userManager.FindByIdAsync(sendFriendshipRequest.AddresseeId);

        if (requester is null || addressee is null)
            return Result<FriendshipResponseDto>.Failure(
                HttpStatusCode.NotFound,
                DomainErrors.Users.UserNotExists);

        if (sendFriendshipRequest.AddresseeId == sendFriendshipRequest.RequesterId)
            return Result<FriendshipResponseDto>.Failure(
                HttpStatusCode.Conflict,
                DomainErrors.Friendship.CanNotSendFriendRequestToYourSelf);


        var existingFriendship = await unitOfWork.FriendshipRepository
            .GetFriendshipAsync(sendFriendshipRequest.RequesterId, sendFriendshipRequest.AddresseeId);

        if (existingFriendship is not null)
        {
            return Result<FriendshipResponseDto>.Failure(
                HttpStatusCode.BadRequest,
                existingFriendship.FriendshipStatus switch
                {
                    FriendshipStatus.Pending => DomainErrors.Friendship.PendingFriendRequest,
                    FriendshipStatus.Accepted => DomainErrors.Friendship.AlreadyAcceptedFriendRequest,
                    FriendshipStatus.Blocked => DomainErrors.Friendship.BlockedFriendRequest,
                    FriendshipStatus.Rejected => DomainErrors.Friendship.RejectedFriendRequest,
                    _ => DomainErrors.Friendship.UndefindFriendRequestStatus
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

        var createdFriendship = await unitOfWork.FriendshipRepository.CreateAsync(friendShip);

        var fShip =
            await unitOfWork.FriendshipRepository.GetBySpecificationAndIdAsync(
                new FriendshipRequestWithRequesterAndReceiverSpecification(), createdFriendship.Id);

        await unitOfWork.SaveChangesAsync();

        if (fShip != null)
            return Result<FriendshipResponseDto>.Success(
                new FriendshipResponseDto(
                string.Concat(fShip.Requester.FirstName, " ", fShip.Requester.LastName),
                string.Concat(fShip.Receiver.FirstName, " ", fShip.Receiver.LastName),
                fShip.FriendshipStatus.ToString(),
                fShip.CreatedAt,
                fShip.UpdatedAt
            ));

        return Result<FriendshipResponseDto>.Failure(
            HttpStatusCode.BadRequest,
            DomainErrors.Friendship.UnableToCreateFriendRequest);
    }

    public async Task<Result<FriendshipResponseDto>> AcceptFriendRequestAsync(AcceptFriendRequestDto acceptFriendRequest)
    {
        var friendship = await unitOfWork.FriendshipRepository.GetByIdAsync(acceptFriendRequest.FriendshipId);

        if (friendship == null)
            return Result<FriendshipResponseDto>.Failure(
                HttpStatusCode.NotFound,
                DomainErrors.Friendship.NotFoundFriendRequest);

        if (friendship.ReceiverId != acceptFriendRequest.UserId)
            return Result<FriendshipResponseDto>.Failure(
                HttpStatusCode.Unauthorized,
                DomainErrors.Friendship.UnauthorizedToAcceptFriendRequest);

        if (friendship.FriendshipStatus != FriendshipStatus.Pending)
            return Result<FriendshipResponseDto>.Failure(
                HttpStatusCode.BadRequest, DomainErrors.Friendship.FriendRequestMustBePending);

        friendship.FriendshipStatus = FriendshipStatus.Accepted;
        friendship.UpdatedAt = DateTimeOffset.Now;

        unitOfWork.FriendshipRepository.Update(friendship);

        await unitOfWork.SaveChangesAsync();

        // Send notification to requester

        var fShip = await unitOfWork.FriendshipRepository
            .GetBySpecificationAndIdAsync(
                new FriendshipRequestWithRequesterAndReceiverSpecification(),
                friendship.Id);

        if (fShip is not null)
        {
            return Result<FriendshipResponseDto>.Success(new FriendshipResponseDto(
                string.Concat(fShip.Requester.FirstName, " ", fShip.Requester.LastName),
                string.Concat(fShip.Receiver.FirstName, " ", fShip.Receiver.LastName),
                fShip.FriendshipStatus.ToString(),
                fShip.CreatedAt,
                fShip.UpdatedAt
            ));
        }

        return Result<FriendshipResponseDto>.Failure(HttpStatusCode.BadRequest, DomainErrors.Friendship.NotFoundFriendRequest);
    }

}
