using BasicJira.Application.Common.Exceptions;
using BasicJira.Application.Common.Interfaces;
using BasicJira.Domain.Entities;
using MediatR;

namespace BasicJira.Application.Users.Commands.DeleteUser;

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUserService;

    public DeleteUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUserService)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _currentUserService = currentUserService;
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
            throw new NotFoundException(nameof(AppUser), request.Id);

        var currentUserId = _currentUserService.UserId
            ?? throw new ForbiddenException();

        if (request.Id == currentUserId)
        {
            throw new ConflictException(
                "You cannot delete your own account.");
        }

        _userRepository.Remove(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
