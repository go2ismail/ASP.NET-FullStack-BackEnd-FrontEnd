// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;

using FluentValidation;

using MediatR;

namespace Application.Features.Members.Commands;

public class UploadProfilePictureMemberResult
{
    public string? ImageName { get; init; }
}

public class UploadProfilePictureMemberRequest : IRequest<UploadProfilePictureMemberResult>
{
    public string? UserId { get; init; }
    public required string OriginalFileName { get; init; }
    public required string Extension { get; init; }
    public required byte[] Data { get; init; }
    public required long Size { get; init; }
    public required string UserEmail { get; init; }
}

public class UploadProfilePictureMemberValidator : AbstractValidator<UploadProfilePictureMemberRequest>
{
    public UploadProfilePictureMemberValidator()
    {
        RuleFor(x => x.OriginalFileName)
            .NotEmpty();

        RuleFor(x => x.Extension)
            .NotEmpty();

        RuleFor(x => x.Data)
            .NotEmpty();

        RuleFor(x => x.Size)
            .NotEmpty();

        RuleFor(x => x.UserEmail)
            .NotEmpty();
    }
}

public class UploadProfilePictureMemberHandler : IRequestHandler<UploadProfilePictureMemberRequest, UploadProfilePictureMemberResult>
{
    private readonly IImageService _uploadImage;
    private readonly IIdentityService _identityService;

    public UploadProfilePictureMemberHandler(IImageService uploadImage, IIdentityService identityService)
    {
        _uploadImage = uploadImage;
        _identityService = identityService;
    }

    public async Task<UploadProfilePictureMemberResult> Handle(UploadProfilePictureMemberRequest request, CancellationToken cancellationToken)
    {
        var result = await _uploadImage.UploadAsync(
            request.UserId,
            request.OriginalFileName,
            request.Extension,
            request.Data,
            request.Size,
            cancellationToken);

        await _identityService.UpdateProfilePictureAsync(request.UserEmail, result);

        return new UploadProfilePictureMemberResult { ImageName = result };
    }
}
