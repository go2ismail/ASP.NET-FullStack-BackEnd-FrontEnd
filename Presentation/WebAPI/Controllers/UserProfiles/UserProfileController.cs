// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.Accounts.Queries;
using Application.Features.Members.Commands;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using WebAPI.Common.Filters;
using WebAPI.Common.Models;

namespace WebAPI.Controllers.UserProfiles;


[Route("api/[controller]")]
public class UserProfileController(ISender sender, IConfiguration configuration) : BaseApiController(sender)
{
    private readonly IConfiguration _configuration = configuration;

    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetUsersByUserId")]
    public async Task<ActionResult<ApiSuccessResult<GetUsersByUserIdResult>>> GetUsersByUserIdAsync(
        [FromQuery] string userId,
        CancellationToken cancellationToken)
    {
        var command = new GetUsersByUserIdRequest
        {
            UserId = userId
        };
        var response = await _sender.Send(command, cancellationToken);

        return Ok(new ApiSuccessResult<GetUsersByUserIdResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetUsersByUserIdAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Update")]
    [HttpPost("UpdateUserProfile")]
    public async Task<ActionResult<ApiSuccessResult<UpdateMemberResult>>> UpdateUserProfileAsync(UpdateMemberRequest request, CancellationToken cancellationToken)
    {
        var isDemoVersion = _configuration.GetValue<bool>("IsDemoVersion");
        var defaultAdminEmail = _configuration["AspNetIdentity:DefaultAdmin:Email"];

        if (
            isDemoVersion
            && request.Email.Equals(defaultAdminEmail, StringComparison.OrdinalIgnoreCase)
            && !string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new ApiErrorResult
            {
                Code = StatusCodes.Status400BadRequest,
                Message = "Demo version can not change DefaultAdmin user password."
            });
        }


        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<UpdateMemberResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(UpdateUserProfileAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Delete")]
    [HttpDelete("DeleteUserProfile")]
    public async Task<ActionResult<ApiSuccessResult<DeleteMemberResult>>> DeleteUserProfileAsync(DeleteMemberRequest request, CancellationToken cancellationToken)
    {
        var defaultAdminEmail = _configuration["AspNetIdentity:DefaultAdmin:Email"];
        if (request.Email.Equals(defaultAdminEmail, StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new ApiErrorResult
            {
                Code = StatusCodes.Status400BadRequest,
                Message = "Cannot delete the DefaultAdmin user."
            });
        }

        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<DeleteMemberResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(DeleteUserProfileAsync)}",
            Content = response
        });
    }


    [ClaimBasedAuthorization("Update")]
    [HttpPost("UploadProfilePictureMember")]
    public async Task<ActionResult<UploadProfilePictureMemberResult>> UploadProfilePictureMemberAsync(
    IFormFile imageFile,
    [FromQuery] string userEmail,
    CancellationToken cancellationToken)
    {
        if (imageFile == null || imageFile.Length == 0)
        {
            return BadRequest("Invalid file.");
        }

        using (var memoryStream = new MemoryStream())
        {
            await imageFile.CopyToAsync(memoryStream, cancellationToken);
            var fileData = memoryStream.ToArray();
            var extension = Path.GetExtension(imageFile.FileName).TrimStart('.');

            var command = new UploadProfilePictureMemberRequest
            {
                OriginalFileName = imageFile.FileName,
                Extension = extension,
                Data = fileData,
                Size = fileData.Length,
                UserEmail = userEmail,
            };

            var result = await _sender.Send(command, cancellationToken);

            if (result?.ImageName == null)
            {
                return StatusCode(500, "An error occurred while uploading the image.");
            }

            return Ok(new ApiSuccessResult<UploadProfilePictureMemberResult>
            {
                Code = StatusCodes.Status200OK,
                Message = $"Success executing {nameof(UploadProfilePictureMemberAsync)}",
                Content = result
            });
        }
    }

}
