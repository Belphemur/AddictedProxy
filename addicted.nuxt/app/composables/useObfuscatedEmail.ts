const words = [
  "crimson", "azure", "golden", "silver", "copper", "ivory", "amber", "jade",
  "coral", "indigo", "violet", "scarlet", "tawny", "onyx", "pearl", "russet",
  "cobalt", "umber", "sienna", "maple", "cedar", "spruce", "aspen", "birch",
  "swift", "bold", "brave", "keen", "deft", "calm", "wise", "fair",
  "noble", "lunar", "solar", "polar", "delta", "sigma", "omega", "alpha",
  "ember", "frost", "storm", "ridge", "cliff", "shore", "grove", "vale",
  "blade", "stone", "flame", "spark", "drift", "surge", "flare", "gleam",
];

/**
 * Renders an email address as text onto a canvas element to deter automated scraping.
 * Font and colour are derived from the canvas element's computed styles so they
 * stay in sync with the Vuetify theme.
 */
export function drawEmailOnCanvas(canvas: HTMLCanvasElement, email: string, width: number): void {
  const ctx = canvas.getContext("2d");
  if (!ctx) return;
  const style = getComputedStyle(canvas);
  const fontFamily = style.fontFamily || "Arial, sans-serif";
  const textColor = style.getPropertyValue("--v-theme-on-surface").trim()
    ? `rgb(${style.getPropertyValue("--v-theme-on-surface").trim()})`
    : "#e0e0e0";
  ctx.clearRect(0, 0, width, 34);
  ctx.font = `500 15px ${fontFamily}`;
  ctx.fillStyle = textColor;
  ctx.fillText(email, 4, 23);
  // Expose the email to screen readers via ARIA. The address is already
  // obfuscated (rotating random word), so static scrapers gain nothing useful.
  canvas.setAttribute("role", "img");
  canvas.setAttribute("aria-label", email);
}

/**
 * Composable for obfuscated email display.
 *
 * Picks a random word from the shared word list and builds an email address of
 * the form `{prefix}-{word}@{domain}`. `useState` ensures the same word is used
 * on both the server and the client, preventing hydration mismatches. The word is
 * also stable across SWR-cached page renders for the lifetime of the cache entry.
 *
 * @param stateKey  Unique Nuxt state key (e.g. `"dmca-email-word"`).
 * @param prefix    Local-part prefix before the random word (e.g. `"dmca"`).
 * @param domain    Email domain (e.g. `"admincmd.com"`).
 * @param width     Canvas width in pixels (defaults to 340).
 */
export function useObfuscatedEmail(
  stateKey: string,
  prefix: string,
  domain: string,
  width = 340,
) {
  const selectedWord = useState(stateKey, () => words[Math.floor(Math.random() * words.length)]);
  const email = computed(() => `${prefix}-${selectedWord.value}@${domain}`);
  const canvasRef = ref<HTMLCanvasElement | null>(null);

  onMounted(() => {
    if (!canvasRef.value) return;
    drawEmailOnCanvas(canvasRef.value, email.value, width);
  });

  return { email, canvasRef, canvasWidth: width };
}
