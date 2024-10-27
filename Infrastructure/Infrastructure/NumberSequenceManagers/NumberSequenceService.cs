// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Application.Services.CQS.Queries;
using Application.Services.Externals;
using Application.Services.Repositories;

using Domain.Entities;

namespace Infrastructure.NumberSequenceManagers;

public class NumberSequenceService(
    IUnitOfWork unitOfWork,
    IBaseCommandRepository<NumberSequence> numberSequenceRepository) : INumberSequenceService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IBaseCommandRepository<NumberSequence> _numberSequenceRepository = numberSequenceRepository;
    private readonly object _lockObject = new();

    private NumberSequence? GetNumberSequence(
        string entityName,
        string? prefix,
        string? suffix)
    {
        var query = _numberSequenceRepository
            .GetQuery()
            .Where(x => x.EntityName == entityName
                && (x.Prefix == prefix || (x.Prefix == null && prefix == null))
                && (x.Suffix == suffix || (x.Suffix == null && suffix == null)));

        query = query.ApplyIsDeletedFilter();

        return query.FirstOrDefault();
    }

    private void UpdateNumberSequence(
        string? userId,
        NumberSequence sequence)
    {
        sequence.Update(userId);

        _numberSequenceRepository.Update(sequence);
        _unitOfWork.Save();
    }

    private NumberSequence InsertNumberSequence(
        string? userId,
        string entityName,
        string? prefix,
        string? suffix)
    {
        var newSequence = new NumberSequence(
            userId,
            entityName,
            prefix,
            suffix);

        _numberSequenceRepository.Create(newSequence);
        _unitOfWork.Save();

        return newSequence;
    }

    public string GenerateNumberSequence(
        string? userId,
        string entityName,
        string? prefix,
        string? suffix,
        bool useDate = true,
        int padding = 4)
    {
        var result = string.Empty;

        if (string.IsNullOrEmpty(entityName))
        {
            throw new NumberSequenceException("Parameter entityName must not be null");
        }

        lock (_lockObject)
        {
            NumberSequence? sequence = GetNumberSequence(
                entityName,
                prefix,
                suffix);

            if (sequence != null)
            {
                UpdateNumberSequence(userId, sequence);
            }
            else
            {
                sequence = InsertNumberSequence(userId, entityName, prefix, suffix);
            }

            string formattedNumber = $"{prefix}{sequence.LastUsedCount.ToString().PadLeft(padding, '0')}{(useDate ? DateTime.Now.ToString("yyyyMMdd") : "")}{suffix}";
            result = formattedNumber;
        }

        return result;
    }
}
