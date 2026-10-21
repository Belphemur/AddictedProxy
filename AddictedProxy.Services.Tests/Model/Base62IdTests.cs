using System.Numerics;
using AddictedProxy.Model.Api;
using FluentAssertions;

namespace AddictedProxy.Services.Tests.Model;

[TestFixture]
public class Base62IdTests
{
    /// <summary>
    /// Canonical cross-language vectors — the wire contract shared with the Go
    /// mock server (see docs/2026-10-20-api-id-base62.md). These literals pin
    /// the byte mapping (Guid big-endian RFC byte order) and the alphabet
    /// ordering; they were derived from an independent big-integer oracle.
    /// </summary>
    private static readonly (Guid Guid, string Base62)[] CanonicalVectors =
    [
        (new Guid("00000000-0000-0000-0000-000000000000"), "0000000000000000000000"),
        (new Guid("ffffffff-ffff-ffff-ffff-ffffffffffff"), "7n42DGM5Tflk9n8mt7Fhc7"),
        (new Guid("E9C1FA23-55AF-4711-8E34-3B31E2A75533"), "775rJjCDufZ0nuSVDeriR1"),
        (new Guid("1086727A-EB71-4B24-A209-7CF22374574D"), "0VBJpBmUXk2twqNJdYbHmH"),
    ];

    [Test]
    public void Encode_CanonicalVectors_ProducePinnedStrings()
    {
        foreach (var (guid, expected) in CanonicalVectors)
        {
            Base62Id.Encode(guid).Should().Be(expected, $"vector {guid}");
        }
    }

    [Test]
    public void TryDecode_CanonicalVectors_ProducePinnedGuids()
    {
        foreach (var (expected, base62) in CanonicalVectors)
        {
            Base62Id.TryDecode(base62, out var decoded).Should().BeTrue($"vector {base62}");
            decoded.Should().Be(expected, $"vector {base62}");
        }
    }

    [Test]
    public void Encode_AlwaysEmitsCanonicalForm()
    {
        // Arrange
        const int samples = 1024;

        // Act + Assert
        for (var i = 0; i < samples; i++)
        {
            var guid = Guid.NewGuid();
            var encoded = Base62Id.Encode(guid);

            encoded.Length.Should().Be(Base62Id.Length, $"sample {i}: {guid}");
            encoded.All(c => Base62Id.Alphabet.Contains(c))
                .Should().BeTrue($"sample {i}: {guid} → {encoded} must only use the base62 alphabet");
        }
    }

    [Test]
    public void EncodeDecode_RoundTripsForRandomGuids()
    {
        // Arrange
        const int samples = 1024;

        // Act + Assert
        for (var i = 0; i < samples; i++)
        {
            var guid = Guid.NewGuid();
            var encoded = Base62Id.Encode(guid);

            Base62Id.TryDecode(encoded, out var decoded).Should().BeTrue($"sample {i}");
            decoded.Should().Be(guid, $"sample {i}: round-trip through {encoded}");
        }
    }

    [Test]
    public void Decode_ValidCanonicalId_ReturnsGuid()
    {
        // Act
        Func<Guid> decode = () => Base62Id.Decode("775rJjCDufZ0nuSVDeriR1");

        // Assert
        decode.Should().NotThrow().Which.Should().Be(new Guid("E9C1FA23-55AF-4711-8E34-3B31E2A75533"));
    }

    [TestCase("")] // empty
    [TestCase("15tKKYmsRZNNgPJoOScXf")] // 21 valid characters — wrong canonical length
    [TestCase("775rJjCDufZ0nuSVDeriR10")] // 23 valid characters — wrong canonical length
    [TestCase("15tKKYmsRZNNgPJoOScXf-")] // '-' is not in the base62 alphabet
    [TestCase("775rJjCDufZ0nuSVDeriR1_")] // '_' is not in the base62 alphabet
    [TestCase("775rJjCDufZ0nuSVDeriR1+")] // '+' is not in the base62 alphabet
    [TestCase("775rJjCDufZ0nuSVDeriR1/")] // '/' is not in the base62 alphabet
    [TestCase("E9C1FA23-55AF-4711-8E34-3B31E2A75533")] // legacy dashed Guid must fail closed
    [TestCase("zzzzzzzzzzzzzzzzzzzzzz")] // 62^22 - 1 exceeds the 128-bit value space (overflow)
    [TestCase("1-5tKKYmsRZNNgPJoOScXfn")] // separator smuggled inside the id
    public void TryDecode_NonCanonicalOrLegacyInput_FailsClosed(string invalid)
    {
        Base62Id.TryDecode(invalid, out var _).Should().BeFalse($"'{invalid}' must be rejected");
    }

    [Test]
    public void Decode_IllegalInput_ThrowsFormatException()
    {
        Action decode = () => Base62Id.Decode("not-a-valid-id!");
        decode.Should().Throw<FormatException>();
    }

    /// <summary>
    /// Independent oracle: same documented mapping computed with
    /// <see cref="BigInteger"/> instead of <see cref="UInt128"/>, so the two
    /// implementations cannot share the same arithmetic bug.
    /// </summary>
    private static string BigIntegerOracle(string dashed)
    {
        // UUIDs are parsed in plain RFC (big-endian) field order.
        var hex = dashed.Replace("-", string.Empty);
        Span<byte> bytes = stackalloc byte[16];
        for (var i = 0; i < 16; i++)
        {
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }

        var value = new BigInteger(bytes, isUnsigned: true, isBigEndian: true);
        Span<char> digits = stackalloc char[Base62Id.Length];
        for (var i = Base62Id.Length - 1; i >= 0; i--)
        {
            (value, var remainder) = BigInteger.DivRem(value, 62);
            digits[i] = Base62Id.Alphabet[(int)remainder];
        }
        return digits.ToString();
    }

    [Test]
    public void Encode_MatchesIndependentBigIntegerOracle()
    {
        // Arrange: corner cases plus randoms — both encode paths must agree.
        var guids = new List<Guid>
        {
            Guid.Empty,
            new("ffffffff-ffff-ffff-ffff-ffffffffffff"),
            new("E9C1FA23-55AF-4711-8E34-3B31E2A75533"),
        };
        guids.AddRange(Enumerable.Range(0, 256).Select(_ => Guid.NewGuid()));

        // Act + Assert
        foreach (var guid in guids)
        {
            var oracleExpected = BigIntegerOracle(guid.ToString());
            Base62Id.Encode(guid).Should().Be(oracleExpected, $"oracle mismatch for {guid}");
        }
    }
}
