// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.Members.Commands;
using Application.Features.Members.Queries;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using WebAPI.Common.Filters;
using WebAPI.Common.Models;

namespace WebAPI.Controllers.Members;


[Route("api/[controller]")]
public class MemberController : BaseApiController
{
    private readonly IConfiguration _configuration;
    public MemberController(ISender sender, IConfiguration configuration) : base(sender)
    {
        _configuration = configuration;
    }

    [ClaimBasedAuthorization("Delete")]
    [HttpDelete("DeleteMember")]
    public async Task<ActionResult<ApiSuccessResult<DeleteMemberResult>>> DeleteMemberAsync(DeleteMemberRequest request, CancellationToken cancellationToken)
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
            Message = $"Success executing {nameof(DeleteMemberAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Create")]
    [HttpPost("CreateMember")]
    public async Task<ActionResult<ApiSuccessResult<CreateMemberResult>>> CreateMemberAsync(CreateMemberRequest request, CancellationToken cancellationToken)
    {
        var response = await _sender.Send(request, cancellationToken);

        return Ok(new ApiSuccessResult<CreateMemberResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(CreateMemberAsync)}",
            Content = response
        });
    }

    [ClaimBasedAuthorization("Update")]
    [HttpPost("UpdateMember")]
    public async Task<ActionResult<ApiSuccessResult<UpdateMemberResult>>> UpdateMemberAsync(UpdateMemberRequest request, CancellationToken cancellationToken)
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
            Message = $"Success executing {nameof(UpdateMemberAsync)}",
            Content = response
        });
    }


    [ClaimBasedAuthorization("Read")]
    [HttpGet("GetMembers")]
    public async Task<ActionResult<ApiSuccessResult<GetMembersResult>>> GetMembersAsync(
        [FromQuery(Name = "$skip")] int skip,
        [FromQuery(Name = "$top")] int top,
        [FromQuery(Name = "$orderby")] string orderBy,
        [FromQuery] string searchValue,
        CancellationToken cancellationToken)
    {
        int page = (skip / top) + 1;
        int limit = top;

        var orderByParts = orderBy.Split(' ');
        var sortBy = orderByParts[0];
        var sortDirection = orderByParts.Length > 1 ? orderByParts[1].ToLower() : "asc";

        var command = new GetMembersRequest
        {
            Page = page,
            Limit = limit,
            SortBy = sortBy,
            SortDirection = sortDirection,
            searchValue = searchValue,
        };
        var response = await _sender.Send(command, cancellationToken);

        return Ok(new ApiSuccessResult<GetMembersResult>
        {
            Code = StatusCodes.Status200OK,
            Message = $"Success executing {nameof(GetMembersAsync)}",
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
