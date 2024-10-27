// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.FileOperations.Commands;

public class CreateDocumentResult
{
    public string? DocumentName { get; init; }
}

public class CreateDocumentRequest : IRequest<CreateDocumentResult>
{
    public string? UserId { get; init; }
    public required string OriginalFileName { get; init; }
    public required string Extension { get; init; }
    public required byte[] Data { get; init; }
    public required long Size { get; init; }
}

public class CreateDocumentValidator : AbstractValidator<CreateDocumentRequest>
{
    public CreateDocumentValidator()
    {
        RuleFor(x => x.OriginalFileName)
            .NotEmpty();

        RuleFor(x => x.Extension)
            .NotEmpty();

        RuleFor(x => x.Data)
            .NotEmpty();

        RuleFor(x => x.Size)
            .NotEmpty();
    }
}

public class CreateDocumentHandler : IRequestHandler<CreateDocumentRequest, CreateDocumentResult>
{
    private readonly IDocumentService _uploadDocument;

    public CreateDocumentHandler(IDocumentService uploadDocument)
    {
        _uploadDocument = uploadDocument;
    }

    public async Task<CreateDocumentResult> Handle(CreateDocumentRequest request, CancellationToken cancellationToken)
    {
        var result = await _uploadDocument.UploadAsync(
            request.UserId,
            request.OriginalFileName,
            request.Extension,
            request.Data,
            request.Size,
            cancellationToken);

        return new CreateDocumentResult { DocumentName = result };
    }
}
