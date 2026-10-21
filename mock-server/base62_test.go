package main

import (
	"testing"
)

// Canonical cross-language vectors — the wire contract shared with the C#
// backend suite (Base62IdTests in AddictedProxy.Services.Tests). Mirror of
// docs/2026-10-20-api-id-base62.md § codec.
var canonicalVectors = map[string]string{
	"00000000-0000-0000-0000-000000000000": "0000000000000000000000",
	"ffffffff-ffff-ffff-ffff-ffffffffffff": "7n42DGM5Tflk9n8mt7Fhc7",
	"E9C1FA23-55AF-4711-8E34-3B31E2A75533": "775rJjCDufZ0nuSVDeriR1",
	"1086727A-EB71-4B24-A209-7CF22374574D": "0VBJpBmUXk2twqNJdYbHmH",
	// Show fixtures in data/shows.json — mirrored in e2e specs.
	"a1b2c3d4-0001-0001-0001-000000000001": "4v7SXznCWD5QQLn9P9mSaf",
	"a1b2c3d4-0002-0002-0002-000000000002": "4v7SXznCcFo1E8yZ6ItWba",
	"a1b2c3d4-0003-0003-0003-000000000003": "4v7SXznCiIWc1w9ynS0acV",
	"a1b2c3d4-0004-0004-0004-000000000004": "4v7SXznCoLFCpjLOUb7edQ",
}

func TestAPIId_CanonicalVectors(t *testing.T) {
	for guid, want := range canonicalVectors {
		got, err := APIId(guid)
		if err != nil {
			t.Fatalf("APIId(%q) failed: %v", guid, err)
		}
		if got != want {
			t.Errorf("APIId(%q) = %q, want %q", guid, got, want)
		}
	}
}

func TestAPIId_RejectsNonUUID(t *testing.T) {
	for _, bad := range []string{"", "short", "not-a-uuid!", "zzzzzzzz-zzzz-zzzz-zzzz-zzzzzzzzzzzz", "a1b2c3d4-0001-0001-0001-0000000000"} {
		if _, err := APIId(bad); err == nil {
			t.Errorf("APIId(%q) should fail", bad)
		}
		if !panics(func() { MustAPIId(bad) }) {
			t.Errorf("MustAPIId(%q) should panic", bad)
		}
	}
}

// panics reports whether calling f raised a panic (a clean return is *not* a
// panic — the deferred recover in the closure must distinguish the two).
func panics(f func()) (panicked bool) {
	defer func() {
		if recover() != nil {
			panicked = true
		}
	}()
	f()
	return
}

func TestAPIId_Deterministic(t *testing.T) {
	for guid := range canonicalVectors {
		first, _ := APIId(guid)
		second, _ := APIId(guid)
		if first != second {
			t.Errorf("APIId(%q) not deterministic: %q vs %q", guid, first, second)
		}
	}
}
