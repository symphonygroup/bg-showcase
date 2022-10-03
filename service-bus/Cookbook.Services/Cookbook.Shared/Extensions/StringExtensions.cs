using System.Security.Cryptography;
using System.Text;

namespace Cookbook.Shared.Extensions;

public static class StringExtensions
{
    /// <summary>
    ///     The namespace for fully-qualified domain names (from RFC 4122, Appendix C).
    /// </summary>
    public static readonly Guid DnsNamespace = new("6ba7b810-9dad-11d1-80b4-00c04fd430c8");

    /// <summary>
    ///     The namespace for URLs (from RFC 4122, Appendix C).
    /// </summary>
    public static readonly Guid UrlNamespace = new("6ba7b811-9dad-11d1-80b4-00c04fd430c8");

    /// <summary>
    ///     The namespace for ISO OIDs (from RFC 4122, Appendix C).
    /// </summary>
    public static readonly Guid IsoOidNamespace = new("6ba7b812-9dad-11d1-80b4-00c04fd430c8");

    public static Guid ToOid(this string name)
    {
        return name.ToNamespacedGuid(IsoOidNamespace);
    }

    public static Guid ToNamespacedGuid(this string name, Guid namespaceId)
    {
        if (name == null)
            throw new ArgumentNullException(nameof(name));

        // convert the name to a sequence of octets (as defined by the standard or conventions of its namespace)
        // ASSUME: UTF-8 encoding is always appropriate
        var nameBytes = Encoding.UTF8.GetBytes(name);

        // convert the namespace UUID to network order
        var namespaceBytes = namespaceId.ToByteArray();
        SwapByteOrder(namespaceBytes);

        // compute the hash of the name space ID concatenated with the name
        byte[] hash;
        using (HashAlgorithm algorithm = SHA1.Create())
        {
            algorithm.TransformBlock(namespaceBytes, 0, namespaceBytes.Length, null, 0);
            algorithm.TransformFinalBlock(nameBytes, 0, nameBytes.Length);
            hash = algorithm.Hash;
        }

        // most bytes from the hash are copied straight to the bytes of the new GUID
        var newGuid = new byte[16];
        Array.Copy(hash, 0, newGuid, 0, 16);

        // set the four most significant bits (bits 12 through 15) of the time_hi_and_version field to the appropriate 4-bit version number
        newGuid[6] = (byte)((newGuid[6] & 0x0F) | (5 << 4));

        // set the two most significant bits (bits 6 and 7) of the clock_seq_hi_and_reserved to zero and one, respectively
        newGuid[8] = (byte)((newGuid[8] & 0x3F) | 0x80);

        // convert the resulting UUID to local byte order (step 13)
        SwapByteOrder(newGuid);
        return new Guid(newGuid);
    }

    // Converts a GUID (expressed as a byte array) to/from network order (MSB-first).
    private static void SwapByteOrder(byte[] guid)
    {
        SwapBytes(guid, 0, 3);
        SwapBytes(guid, 1, 2);
        SwapBytes(guid, 4, 5);
        SwapBytes(guid, 6, 7);
    }

    private static void SwapBytes(byte[] guid, int left, int right)
    {
        (guid[left], guid[right]) = (guid[right], guid[left]);
    }
}