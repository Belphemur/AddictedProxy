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

// guidToByteArray parses a dashed UUID into its 16 RFC-order (big-endian)
// bytes, matching Guid.ToByteArray(bigEndian: true) / Guid.TryWriteBytes on
// the C# side.
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

	out := make([]byte, 16)
	for i := 0; i < 16; i++ {
		out[i] = nibble2byte(nibbles, i*2)
	}
	return out, nil
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
