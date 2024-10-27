// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.FileOperations.Queries;

public class GetDocumentResult
{
    public byte[]? Data { get; init; }
}

public class GetDocumentRequest : IRequest<GetDocumentResult>
{
    public required string DocumentName { get; init; }
}

public class GetDocumentValidator : AbstractValidator<GetDocumentRequest>
{
    public GetDocumentValidator()
    {
        RuleFor(x => x.DocumentName)
            .NotEmpty();
    }
}

public class GetDocumentHandler : IRequestHandler<GetDocumentRequest, GetDocumentResult>
{
    private readonly IDocumentService _documentService;

    public GetDocumentHandler(IDocumentService documentService)
    {
        _documentService = documentService;
    }

    public async Task<GetDocumentResult> Handle(GetDocumentRequest request, CancellationToken cancellationToken)
    {
        var result = await _documentService.GetFileAsync(request.DocumentName, cancellationToken);

        return new GetDocumentResult { Data = result };
    }
}

