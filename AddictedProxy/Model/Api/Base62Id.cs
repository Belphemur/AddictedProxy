using System.Buffers.Binary;

namespace AddictedProxy.Model.Api;

/// <summary>
/// Canonical external encoding of API <see cref="Guid"/> identifiers.
///
/// Encoding: fixed-width base62 over the alphabet <c>0-9A-Za-z</c> (in that
/// order), 22 characters, MSB first, built from the 128-bit value of the Guid
/// interpreted big-endian across <see cref="Guid.ToByteArray()"/>.
///
/// Invariants:
/// <list type="bullet">
/// <item><b>Round-trip</b>: <c>Decode(Encode(g)) == g</c> for every Guid.</item>
/// <item><b>Canonical</b>: every Guid maps to exactly one string, and decode
/// rejects anything that is not exactly that canonical form — non-canonical
/// or legacy inputs (dashed GUID, other lengths, unknown characters, values
/// beyond 128 bits) fail closed.</item>
/// </list>
///
/// This type is the single source of truth for "what an API ID looks like";
/// see <c>docs/2026-10-20-api-id-base62.md</c>.
/// </summary>
public static class Base62Id
{
    /// <summary>
    /// Number of base62 digits required to represent a Guid:
    /// ceil(log62(2^128)) == 22.
    /// </summary>
    public const int Length = 22;

    /// <summary>
    /// API identifier alphabet: digits, then uppercase, then lowercase.
    /// The order is part of the wire contract (shared with the Go mock server).
    /// </summary>
    public const string Alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    private const int Radix = 62;
    private static readonly sbyte[] CharToDigit = BuildLookup();

    private static sbyte[] BuildLookup()
    {
        var lookup = new sbyte[128];
        Array.Fill(lookup, (sbyte)-1);
        for (var i = 0; i < Alphabet.Length; i++)
        {
            lookup[Alphabet[i]] = (sbyte)i;
        }
        return lookup;
    }

    /// <summary>
    /// Encode a Guid as the canonical fixed-22-char base62 string.
    /// </summary>
    public static string Encode(Guid id)
    {
        Span<char> digits = stackalloc char[Length];
        var value = ToUInt128(id);
        for (var i = Length - 1; i >= 0; i--)
        {
            (value, var remainder) = UInt128.DivRem(value, Radix);
            digits[i] = Alphabet[(int)remainder];
        }
        return new string(digits);
    }

    /// <summary>
    /// Strictly decode a canonical base62 API id back to its Guid.
    /// Accepts only the exact canonical 22-character form; returns
    /// <see langword="false"/> (fail closed) for anything else.
    /// </summary>
    public static bool TryDecode(ReadOnlySpan<char> value, out Guid id)
    {
        id = default;
        if (value.Length != Length)
        {
            return false;
        }

        UInt128 accumulator = 0;
        foreach (var c in value)
        {
            if (c >= CharToDigit.Length || CharToDigit[c] < 0)
            {
                return false;
            }

            var digit = (UInt128)CharToDigit[c];

            // accumulator * Radix + digit must stay within 128 bits.
            if (accumulator > UInt128.MaxValue / Radix)
            {
                return false;
            }

            var scaled = accumulator * Radix;
            if (scaled > UInt128.MaxValue - digit)
            {
                return false;
            }

            accumulator = scaled + digit;
        }

        id = ToGuid(accumulator);
        return true;
    }

    /// <inheritdoc cref="TryDecode(ReadOnlySpan{char}, out Guid)"/>
    public static bool TryDecode(string? value, out Guid id)
    {
        if (value is null)
        {
            id = default;
            return false;
        }
        return TryDecode(value.AsSpan(), out id);
    }

    /// <summary>
    /// Decode a canonical base62 API id; throws <see cref="FormatException"/>
    /// on anything that is not the canonical form. Prefer
    /// <see cref="TryDecode(ReadOnlySpan{char}, out Guid)"/> on input paths.
    /// </summary>
    public static Guid Decode(string value)
    {
        if (!TryDecode(value, out var id))
        {
            throw new FormatException($"Value '{value}' is not a valid canonical base62 API identifier ({Length} characters, alphabet '{Alphabet}').");
        }
        return id;
    }

    /// <summary>
    /// Map a Guid onto its 128-bit value: the 16 RFC-order (big-endian) bytes
    /// of the Guid interpreted big-endian, so UUIDv7 timestamp ordering is
    /// preserved. Matches <c>Guid.ToByteArray(bigEndian: true)</c>. The inverse
    /// of <see cref="ToGuid"/>.
    /// </summary>
    private static UInt128 ToUInt128(Guid id)
    {
        // RFC byte order (big-endian) so UUIDv7 timestamp ordering is
        // preserved; matches the Go mock, which reads the dashed hex in
        // plain order. Canonical vectors: docs/2026-10-20-api-id-base62.md.
        Span<byte> bytes = stackalloc byte[16];
        id.TryWriteBytes(bytes, bigEndian: true, out _);
        Span<byte> buffer = stackalloc byte[8];
        bytes[..8].CopyTo(buffer);
        var high = BinaryPrimitives.ReadUInt64BigEndian(buffer);
        bytes[8..].CopyTo(buffer);
        var low = BinaryPrimitives.ReadUInt64BigEndian(buffer);
        return new UInt128(high, low);
    }

    /// <summary>
    /// Exact inverse of <see cref="ToUInt128"/>.
    /// </summary>
    private static Guid ToGuid(UInt128 value)
    {
        Span<byte> bytes = stackalloc byte[16];
        Span<byte> buffer = stackalloc byte[8];
        BinaryPrimitives.WriteUInt64BigEndian(buffer, (ulong)(value >> 64));
        buffer.CopyTo(bytes);
        BinaryPrimitives.WriteUInt64BigEndian(buffer, (ulong)value);
        buffer.CopyTo(bytes[8..]);
        return new Guid(bytes.ToArray(), bigEndian: true);
    }
}
