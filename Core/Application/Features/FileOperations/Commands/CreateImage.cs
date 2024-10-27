// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.FileOperations.Commands;

public class CreateImageResult
{
    public string? ImageName { get; init; }
}

public class CreateImageRequest : IRequest<CreateImageResult>
{
    public string? UserId { get; init; }
    public required string OriginalFileName { get; init; }
    public required string Extension { get; init; }
    public required byte[] Data { get; init; }
    public required long Size { get; init; }
}

public class CreateImageValidator : AbstractValidator<CreateImageRequest>
{
    public CreateImageValidator()
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

public class CreateImageHandler : IRequestHandler<CreateImageRequest, CreateImageResult>
{
    private readonly IImageService _uploadImage;

    public CreateImageHandler(IImageService uploadImage)
    {
        _uploadImage = uploadImage;
    }

    public async Task<CreateImageResult> Handle(CreateImageRequest request, CancellationToken cancellationToken)
    {
        var result = await _uploadImage.UploadAsync(
            request.UserId,
            request.OriginalFileName,
            request.Extension,
            request.Data,
            request.Size,
            cancellationToken);

        return new CreateImageResult { ImageName = result };
    }
}
