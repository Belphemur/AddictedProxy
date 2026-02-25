/**
 * Generates a deterministic, readable color for a release group name.
 * Colors are derived from a hash of the text and constrained to be
 * legible on a dark background (high saturation, light enough luminance).
 */
export const useGroupColor = () => {
  const cache = new Map<string, string>();

  const getColor = (text: string): string => {
    const cached = cache.get(text);
    if (cached) return cached;

    // Simple string hash (djb2)
    let hash = 5381;
    for (let i = 0; i < text.length; i++) {
      hash = ((hash << 5) + hash + text.charCodeAt(i)) | 0;
    }

    // Use the hash to pick hue (0-360), keep saturation and lightness
    // in ranges that ensure readability on dark backgrounds
    const hue = (((hash & 0xffff) % 360) + 360) % 360;
    const saturation = 55 + (((hash >>> 16) & 0xff) % 25); // 55-79%
    const lightness = 65 + (((hash >>> 8) & 0xff) % 15); // 65-79%

    const color = `hsl(${hue}, ${saturation}%, ${lightness}%)`;
    cache.set(text, color);
    return color;
  };

  return { getColor };
};

/**
 * Split a comma-separated version string into individual release group names.
 */
export const parseReleaseGroups = (version: string): string[] =>
  version
    .split(",")
    .map((s) => s.trim())
    .filter(Boolean);
