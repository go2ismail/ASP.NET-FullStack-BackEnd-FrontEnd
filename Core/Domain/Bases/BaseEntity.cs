// ----------------------------------------------------------------------------
// Developer:      Ismail Hamzah
// Email:         go2ismail@gmail.com
// ----------------------------------------------------------------------------

using Domain.Interfaces;

namespace Domain.Bases;

/// <summary>
/// Base class for all entities, providing a unique identifier and a globally unique identifier (GUID).
/// Typically used as a base class for other entities to inherit common properties.
/// </summary>
public abstract class BaseEntity : IHasSequentialId
{
    public string Id { get; set; } = null!;

    public BaseEntity()
    {
        Id = GenerateSequentialGuid();
    }

    private static readonly object _lock = new object(); // Thread safety

    private string GenerateSequentialGuid()
    {
        byte[] guidArray = Guid.NewGuid().ToByteArray();

        var baseDate = new DateTime(1900, 1, 1);
        var now = DateTime.UtcNow;

        TimeSpan timeSpan = now - baseDate;
        byte[] daysArray = BitConverter.GetBytes(timeSpan.Days);
        byte[] msecsArray = BitConverter.GetBytes((long)(timeSpan.TotalMilliseconds % 86400000));

        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(daysArray);
            Array.Reverse(msecsArray);
        }

        lock (_lock) // Ensure thread safety
        {
            Array.Copy(daysArray, daysArray.Length - 2, guidArray, guidArray.Length - 6, 2);
            Array.Copy(msecsArray, msecsArray.Length - 4, guidArray, guidArray.Length - 4, 4);
        }

        return new Guid(guidArray).ToString();
    }
}

