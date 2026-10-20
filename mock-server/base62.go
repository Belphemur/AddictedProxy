// Canonical base62 encoding of API document identifiers.
//
// The wire contract lives in the C# backend
// (AddictedProxy/Model/Api/Base62Id.cs) and is specified in
// docs/2026-10-20-api-id-base62.md: a Guid is encoded as a fixed-width
// 22-character positional base62 string over the alphabet 0-9A-Za-z, from the
// 128-bit value of Guid.ToByteArray() (first three UUID fields little-endian,
// read big-endian). This file is the mock-server implementation; correctness
// is pinned by the same canonical vectors as the C# suite
// (ApiId*Tests in AddictedProxy.Services.Tests).

package main

import (
	"fmt"
	"math/big"
	"strings"
)

const (
	alphabet    = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"
	apiIDLength = 22
)

// APIId encodes a dashed UUID string into the canonical base62 API identifier.
func APIId(dashed string) (string, error) {
	bytes, err := guidToByteArray(dashed)
	if err != nil {
		return "", err
	}
	value := new(big.Int).SetBytes(bytes) // big-endian interpretation
	digits := make([]byte, apiIDLength)
	for i := apiIDLength - 1; i >= 0; i-- {
		digit := uint64(0)
		value, digit = divMod(value, 62)
		digits[i] = alphabet[digit]
	}
	if value.Sign() != 0 {
		return "", errNotAUuid
	}
	return string(digits), nil
}

// MustAPIId encodes a dashed UUID string, panicking on invalid input. Use for
// fixture-borne identifiers where a bad value is a programming error.
func MustAPIId(dashed string) string {
	id, err := APIId(dashed)
	if err != nil {
		panic(err)
	}
	return id
}

var errNotAUuid = fmt.Errorf("value is not a dashed UUID")

func divMod(value *big.Int, radix uint64) (*big.Int, uint64) {
	quotient, remainder := new(big.Int).QuoRem(value, big.NewInt(int64(radix)), new(big.Int))
	return quotient, remainder.Uint64()
}

// guidToByteArray replicates Guid.ToByteArray(): standard UUID fields with the
// first three groups stored little-endian.
func guidToByteArray(dashed string) ([]byte, error) {
	hex := strings.ReplaceAll(dashed, "-", "")
	if len(hex) != 32 {
		return nil, errNotAUuid
	}
	nibbles := make([]byte, 32)
	for i, c := range hex {
		n, err := hexNibble(c)
		if err != nil {
			return nil, err
		}
		nibbles[i] = byte(n)
	}

	var out [16]byte
	// groups: 8-4-4-4-12 hex chars; first three stored little-endian.
	group := func(dst, hexStart, hexLen int, littleEndian bool) error {
		if !littleEndian {
			for i := 0; i < hexLen/2; i++ {
				out[dst+i] = nibble2byte(nibbles, hexStart+i*2)
			}
			return nil
		}
		for i := 0; i < hexLen/2; i++ {
			out[dst+i] = nibble2byte(nibbles, hexStart+(hexLen-2)-i*2)
		}
		return nil
	}

	if err := group(0, 0, 8, true); err != nil {
		return nil, err
	}
	if err := group(4, 8, 4, true); err != nil {
		return nil, err
	}
	if err := group(6, 12, 4, true); err != nil {
		return nil, err
	}
	if err := group(8, 16, 8, false); err != nil {
		return nil, err
	}
	if err := group(12, 24, 8, false); err != nil {
		return nil, err
	}
	return out[:], nil
}

func nibble2byte(nibbles []byte, start int) byte {
	return nibbles[start]<<4 | nibbles[start+1]
}

func hexNibble(c rune) (int, error) {
	switch {
	case c >= '0' && c <= '9':
		return int(c - '0'), nil
	case c >= 'a' && c <= 'f':
		return int(c-'a') + 10, nil
	case c >= 'A' && c <= 'F':
		return int(c-'A') + 10, nil
	}
	return -1, errNotAUuid
}
