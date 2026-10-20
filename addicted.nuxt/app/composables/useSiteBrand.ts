/**
 * Single source of truth for the user-facing brand/site name and origin.
 *
 * The site name defaults to "Subvault" to match the subvault.tv default URL in
 * nuxt.config.ts runtimeConfig. Override at build time (APP_SITENAME) or
 * runtime (NUXT_PUBLIC_SITENAME); the URL is APP_URL / NUXT_PUBLIC_URL.
 *
 * Pages call this composable instead of hard-coding "Gestdown", so the brand
 * stays fully configurable and is never baked into the build output.
 */
export const useSiteBrand = () => {
  const cfg = useRuntimeConfig().public;
  const siteName = cfg.siteName;
  const siteUrl = cfg.url;
  const apiDocsUrl = cfg.apiDocsUrl;
  let siteDomain = "";
  try {
    siteDomain = new URL(siteUrl).hostname.replace(/^www\./, "");
  } catch {
    siteDomain = "";
  }
  return { siteName, siteUrl, siteDomain, apiDocsUrl };
};
