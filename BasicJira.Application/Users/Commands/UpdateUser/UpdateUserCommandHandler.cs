using BasicJira.Application.Common.Interfaces;
using MediatR;

namespace BasicJira.Application.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user == null)
            throw new Exception("User not found.");

        user.FullName = request.FullName;
        user.Email = request.Email;
        user.Role = request.Role;

        _userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
