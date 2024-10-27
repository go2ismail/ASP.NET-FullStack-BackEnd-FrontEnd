// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Features.FileOperations.Commands;
using Application.Features.FileOperations.Queries;

using Infrastructure.DocumentManagers;
using Infrastructure.ImageManagers;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WebAPI.Common.Models;

namespace WebAPI.Controllers.FileOperations;

[Route("api/[controller]")]
public class FileOperationController : BaseApiController
{
    public FileOperationController(ISender sender) : base(sender)
    {
    }

    [Authorize]
    [HttpPost("UploadImage")]
    public async Task<ActionResult<CreateImageResult>> UploadImageAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Invalid file.");
        }

        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream, cancellationToken);
            var fileData = memoryStream.ToArray();
            var extension = Path.GetExtension(file.FileName).TrimStart('.');

            var command = new CreateImageRequest
            {
                OriginalFileName = file.FileName,
                Extension = extension,
                Data = fileData,
                Size = fileData.Length
            };

            var result = await _sender.Send(command, cancellationToken);

            if (result?.ImageName == null)
            {
                return StatusCode(500, "An error occurred while uploading the image.");
            }

            return Ok(new ApiSuccessResult<CreateImageResult>
            {
                Code = StatusCodes.Status200OK,
                Message = $"Success executing {nameof(UploadImageAsync)}",
                Content = result
            });
        }
    }


    [Authorize]
    [HttpGet("GetImage")]
    public async Task<IActionResult> GetImageAsync(
        [FromQuery] string imageName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(imageName) || Path.GetExtension(imageName) == string.Empty)
        {
            imageName = "noimage.png";
        }

        var request = new GetImageRequest
        {
            ImageName = imageName
        };

        var result = await _sender.Send(request, cancellationToken);

        if (result?.Data == null)
        {
            return NotFound("Image not found.");
        }

        var extension = Path.GetExtension(imageName).ToLower();
        var mimeType = ImageManagerHelper.GetMimeType(extension);

        return File(result.Data, mimeType);
    }


    [Authorize]
    [HttpPost("UploadDocument")]
    public async Task<ActionResult<CreateDocumentResult>> UploadDocumentAsync(IFormFile file, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Invalid file.");
        }

        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream, cancellationToken);
            var fileData = memoryStream.ToArray();
            var extension = Path.GetExtension(file.FileName).TrimStart('.');

            var command = new CreateDocumentRequest
            {
                OriginalFileName = file.FileName,
                Extension = extension,
                Data = fileData,
                Size = fileData.Length
            };

            var result = await _sender.Send(command, cancellationToken);

            if (result?.DocumentName == null)
            {
                return StatusCode(500, "An error occurred while uploading the document.");
            }

            return Ok(new ApiSuccessResult<CreateDocumentResult>
            {
                Code = StatusCodes.Status200OK,
                Message = $"Success executing {nameof(UploadDocumentAsync)}",
                Content = result
            });
        }
    }


    [Authorize]
    [HttpGet("GetDocument")]
    public async Task<IActionResult> GetDocumentAsync(
        [FromQuery] string documentName,
        CancellationToken cancellationToken)
    {

        if (string.IsNullOrEmpty(documentName) || Path.GetExtension(documentName) == string.Empty)
        {
            documentName = "nofile.txt";
        }

        var request = new GetDocumentRequest
        {
            DocumentName = documentName
        };

        var result = await _sender.Send(request, cancellationToken);

        if (result?.Data == null)
        {
            return NotFound("Document not found.");
        }

        var extension = Path.GetExtension(documentName).ToLower();
        var mimeType = DocumentManagerHelper.GetMimeType(extension);

        return File(result.Data, mimeType);
    }
}
