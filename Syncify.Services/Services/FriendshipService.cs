using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Syncify.Application.Bases;
using Syncify.Application.DTOs.FriendshipRequests;
using Syncify.Application.Features.FriendRequests.Commands.CurrentUserAcceptFriendRequest;
using Syncify.Application.Features.FriendRequests.Commands.CurrentUserSendFriendRequest;
using Syncify.Application.Features.FriendRequests.Queries.AreFriendsForCurrentUser;
using Syncify.Application.Features.FriendRequests.Queries.CheckIfAreFriends;
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
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    IMapper mapper) : IFriendshipService
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

        unitOfWork.Repository<Friendship>()?.Create(friendShip);

        await unitOfWork.SaveChangesAsync();

        var fShip =
            await unitOfWork.FriendshipRepository.GetBySpecificationAndIdAsync(
                new FriendshipRequestWithRequesterAndReceiverSpecification(), friendShip.Id);

        if (fShip == null)
            return Result<FriendshipResponseDto>.Failure(
                HttpStatusCode.BadRequest,
                DomainErrors.Friendship.UnableToCreateFriendRequest);

        return Result<FriendshipResponseDto>.Success(
            new FriendshipResponseDto(
                string.Concat(fShip.Requester.FirstName, " ", fShip.Requester.LastName),
                string.Concat(fShip.Receiver.FirstName, " ", fShip.Receiver.LastName),
                fShip.FriendshipStatus.ToString(),
                fShip.CreatedAt,
                fShip.UpdatedAt
            ));

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

        // Send realtime notification to requester

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

    public async Task<Result<IEnumerable<PendingFriendshipRequest>>> GetLoggedInUserRequestedFriendshipsAsync()
    {
        var pendingRequests = await unitOfWork.FriendshipRepository.GetPendingRequestsForReceiverAsync(currentUser.Id);
        var mappedPendingRequests = mapper.Map<IEnumerable<PendingFriendshipRequest>>(pendingRequests);
        return Result<IEnumerable<PendingFriendshipRequest>>.Success(mappedPendingRequests);
    }

    public async Task<Result<IEnumerable<GetUserAcceptedFriendshipDto>>> GetLoggedInUserAcceptedFriendshipsAsync()
    {
        var acceptedFriendships = await unitOfWork.FriendshipRepository.GetAcceptedFriendshipsForReceiverAsync(currentUser.Id);
        var mappedAcceptedFriendships = mapper.Map<IEnumerable<GetUserAcceptedFriendshipDto>>(acceptedFriendships);
        return Result<IEnumerable<GetUserAcceptedFriendshipDto>>.Success(mappedAcceptedFriendships);
    }

    public async Task<Result<bool>> AreFriendsAsync(CheckIfAreFriendsQuery query)
        => Result<bool>.Success(await unitOfWork.FriendshipRepository.AreFriendsAsync(query.User1Id, query.User2Id));

    public async Task<Result<bool>> AreFriendsWithCurrentUserAsync(AreFriendsForCurrentUserQuery query)
        => query.FriendId == currentUser.Id ?
            Result<bool>.Failure(HttpStatusCode.Conflict, message: DomainErrors.Friendship.CanNotBeFriendOfYourself) :
            Result<bool>.Success(await unitOfWork.FriendshipRepository.AreFriendsAsync(currentUser.Id, query.FriendId));

    public async Task<Result<FriendshipResponseDto>> SendFriendRequestCurrentUserAsync(
        CurrentUserSendFriendRequestCommand command)
    {
        var requester = await userManager.FindByIdAsync(currentUser.Id);

        var addressee = await userManager.FindByIdAsync(command.FriendId);

        if (requester is null || addressee is null)
            return Result<FriendshipResponseDto>.Failure(
                HttpStatusCode.NotFound,
                DomainErrors.Users.UserNotExists);

        if (command.FriendId == currentUser.Id)
            return Result<FriendshipResponseDto>.Failure(
                HttpStatusCode.Conflict,
                DomainErrors.Friendship.CanNotSendFriendRequestToYourSelf);

        var existingFriendship = await unitOfWork.FriendshipRepository
            .GetFriendshipAsync(command.FriendId, currentUser.Id);

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
            RequesterId = currentUser.Id,
            ReceiverId = command.FriendId,
            FriendshipStatus = FriendshipStatus.Pending,
            UpdatedAt = DateTimeOffset.Now
        };

        unitOfWork.Repository<Friendship>()?.Create(friendShip);

        await unitOfWork.SaveChangesAsync();

        var fShip =
            await unitOfWork.FriendshipRepository.GetBySpecificationAndIdAsync(
                new FriendshipRequestWithRequesterAndReceiverSpecification(), friendShip.Id);

        if (fShip == null)
            return Result<FriendshipResponseDto>.Failure(
                HttpStatusCode.BadRequest,
                DomainErrors.Friendship.UnableToCreateFriendRequest);

        // send realtime notification

        return Result<FriendshipResponseDto>.Success(
            new FriendshipResponseDto(
                string.Concat(fShip.Requester.FirstName, " ", fShip.Requester.LastName),
                string.Concat(fShip.Receiver.FirstName, " ", fShip.Receiver.LastName),
                fShip.FriendshipStatus.ToString(),
                fShip.CreatedAt,
                fShip.UpdatedAt
            ));

    }

    public async Task<Result<FriendshipResponseDto>> CurrentUserAcceptFriendRequestAsync(
        CurrentUserAcceptFriendRequestCommand command)
    {
        var friendship = await unitOfWork.FriendshipRepository.GetByIdAsync(command.FriendshipId);

        if (friendship == null)
            return Result<FriendshipResponseDto>.Failure(
                HttpStatusCode.NotFound,
                DomainErrors.Friendship.NotFoundFriendRequest);

        if (friendship.ReceiverId != currentUser.Id)
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

        // Send realtime notification to requester

        var fShip = await unitOfWork.FriendshipRepository
            .GetBySpecificationAndIdAsync(
                new FriendshipRequestWithRequesterAndReceiverSpecification(),
                friendship.Id);

        if (fShip is not null)
        {
            return Result<FriendshipResponseDto>.Success(
                new FriendshipResponseDto(
                string.Concat(fShip.Requester.FirstName, " ", fShip.Requester.LastName),
                string.Concat(fShip.Receiver.FirstName, " ", fShip.Receiver.LastName),
                fShip.FriendshipStatus.ToString(),
                fShip.CreatedAt,
                fShip.UpdatedAt));
        }

        return Result<FriendshipResponseDto>.Failure(
            HttpStatusCode.BadRequest,
            DomainErrors.Friendship.NotFoundFriendRequest);

    }
}
