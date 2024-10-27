// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.Externals;
using Application.Services.Repositories;

using Domain.Entities;

using Microsoft.Extensions.Options;

namespace Infrastructure.DocumentManagers;

public class DocumentService(
    IUnitOfWork unitOfWork,
    IOptions<DocumentManagerSettings> settings,
    IBaseCommandRepository<FileDoc> docRepository
        ) : IDocumentService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly string _folderPath = Path.Combine(Directory.GetCurrentDirectory(), settings.Value.PathFolder);
    private readonly int _maxFileSizeInBytes = settings.Value.MaxFileSizeInMB * 1024 * 1024;
    private readonly IBaseCommandRepository<FileDoc> _docRepository = docRepository;

    public async Task<string> UploadAsync(
        string? userId,
        string originalFileName,
        string docExtension,
        byte[] fileData,
        long size,
        CancellationToken cancellationToken = default)
    {

        if (string.IsNullOrWhiteSpace(docExtension) || docExtension.Contains(Path.DirectorySeparatorChar) || docExtension.Contains(Path.AltDirectorySeparatorChar))
        {
            throw new DocumentManagerException($"Invalid file extension: {nameof(docExtension)}");
        }

        if (fileData == null || fileData.Length == 0)
        {
            throw new DocumentManagerException($"File data cannot be null or empty: {nameof(fileData)}");
        }

        if (fileData.Length > _maxFileSizeInBytes)
        {
            throw new DocumentManagerException($"File size exceeds the maximum allowed size of {_maxFileSizeInBytes / (1024 * 1024)} MB");
        }

        var fileName = $"{Guid.NewGuid():N}.{docExtension}";

        if (!Directory.Exists(_folderPath))
        {
            Directory.CreateDirectory(_folderPath);
        }

        var filePath = Path.Combine(_folderPath, fileName);

        await File.WriteAllBytesAsync(filePath, fileData, cancellationToken);

        var doc = new FileDoc(
            userId,
            fileName,
            null,
            originalFileName,
            fileName,
            docExtension,
            size);

        await _docRepository.CreateAsync(doc, cancellationToken);
        await _unitOfWork.SaveAsync(cancellationToken);

        return fileName;
    }

    public async Task<byte[]> GetFileAsync(string fileName, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_folderPath, fileName);

        if (!File.Exists(filePath))
        {
            filePath = Path.Combine(_folderPath, "wwwroot", fileName);
        }

        var result = await File.ReadAllBytesAsync(filePath, cancellationToken);

        return result;
    }
}
