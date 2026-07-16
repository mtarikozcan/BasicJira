using BasicJira.Application.Common.Interfaces;
using MediatR;

namespace BasicJira.Application.Comments.Commands.UpdateComment;

public class UpdateCommentCommandHandler : IRequestHandler<UpdateCommentCommand>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCommentCommandHandler(
        ICommentRepository commentRepository,
        IUnitOfWork unitOfWork)
    {
        _commentRepository = commentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(request.Id, cancellationToken);

        if (comment == null)
            throw new Exception("Comment not found.");

        comment.Comment = request.Comment;

        _commentRepository.Update(comment);

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
