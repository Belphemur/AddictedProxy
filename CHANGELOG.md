## [4.34.0](https://github.com/Belphemur/AddictedProxy/compare/v4.33.6...v4.34.0) (2025-01-29)

### Bug Fixes

* **deps:** update dependency @sentry/vue to v8.52.0 ([987a8bd](https://github.com/Belphemur/AddictedProxy/commit/987a8bdfa7ba6f8339bf27f798d384dc6d2fff84))
* **deps:** update dependency vuetify to v3.7.8 ([730aebc](https://github.com/Belphemur/AddictedProxy/commit/730aebc3cd20688d6b28fa38d8287eaee7c1dac9))
* **job:** remove the old job checking for completed ([614a523](https://github.com/Belphemur/AddictedProxy/commit/614a523f03bdd5fc212c5befbc496224be8d02af))

### Features

* **show:** Refresh the state of completed shows too ([16b76be](https://github.com/Belphemur/AddictedProxy/commit/16b76bef47a94ab7618f20a1c02a6153e43acf80))

## [4.33.6](https://github.com/Belphemur/AddictedProxy/compare/v4.33.5...v4.33.6) (2025-01-23)

### Bug Fixes

* **deps:** update dependency @sentry/vue to v8.49.0 ([4a657bb](https://github.com/Belphemur/AddictedProxy/commit/4a657bb5cea8771d849f90afee8c3cd15368f862))
* **deps:** update dependency @sentry/vue to v8.50.0 ([944de9c](https://github.com/Belphemur/AddictedProxy/commit/944de9c36a1239dfcbb40cff60c37b65bacfb85b))
* **deps:** update dependency @sentry/vue to v8.51.0 ([856b160](https://github.com/Belphemur/AddictedProxy/commit/856b16090cf449e3b18574cd66bd3bcd1af20a61))
* **deps:** update dependency vuetify to v3.7.7 ([dffbf23](https://github.com/Belphemur/AddictedProxy/commit/dffbf2341667cef970db34141c016440bb7fcdfa))
* issue with vite ([19e2748](https://github.com/Belphemur/AddictedProxy/commit/19e274869c0ac358a7087098a443029513b16f8d))

## [4.33.5](https://github.com/Belphemur/AddictedProxy/compare/v4.33.4...v4.33.5) (2025-01-09)

### Performance improvements

* **httphandler:** always use the modern version of the handler ([1580516](https://github.com/Belphemur/AddictedProxy/commit/1580516b9d1437cb3f213e258a95e29e38db689d))

## [4.33.4](https://github.com/Belphemur/AddictedProxy/compare/v4.33.3...v4.33.4) (2025-01-09)

### Performance improvements

* **httphandler:** keep them as long as it make sense ([f525af6](https://github.com/Belphemur/AddictedProxy/commit/f525af640fbc7618a7e11aab62fa44ccfaae6d6d))

## [4.33.3](https://github.com/Belphemur/AddictedProxy/compare/v4.33.2...v4.33.3) (2025-01-09)

### Performance improvements

* **query-params:** keep query params in logs ([913dccb](https://github.com/Belphemur/AddictedProxy/commit/913dccb1cb341b3c5a3e36c32868b97dbfd3d234))

## [4.33.2](https://github.com/Belphemur/AddictedProxy/compare/v4.33.1...v4.33.2) (2025-01-09)

### Performance improvements

* **proxy:** .NET supports socks since 6.0 ([1de03cd](https://github.com/Belphemur/AddictedProxy/commit/1de03cdb508995f7d787df481241dc3c3e94eee8))

### Bug Fixes

* store subtitle job not taken because too many fetch-subtitles waiting ([9ecc6cc](https://github.com/Belphemur/AddictedProxy/commit/9ecc6cc4af2fd983a043d167779e767cf373aad0))

## [4.33.1](https://github.com/Belphemur/AddictedProxy/compare/v4.33.0...v4.33.1) (2025-01-08)

### Performance improvements

* **fetch:** be sure to log a critical issue in the job ([1697e78](https://github.com/Belphemur/AddictedProxy/commit/1697e78df0ea19309cda7cf4d1c11ee50f057138))
* **tvshow:** use no tracking when only reading data ([57ad23f](https://github.com/Belphemur/AddictedProxy/commit/57ad23f0d05f20c9b16bc70587568c92fada7055))

### Bug Fixes

* **fetch:** fix fetch job crashing ([33298e1](https://github.com/Belphemur/AddictedProxy/commit/33298e1190f1ded33a24ce18cc9325c6eaa24ead))
* **fetch:** fix getting subtitles for shows ([458968d](https://github.com/Belphemur/AddictedProxy/commit/458968df58642359c4ec207651a939146a2107f4))

## [4.33.0](https://github.com/Belphemur/AddictedProxy/compare/v4.32.5...v4.33.0) (2025-01-08)

### Performance improvements

* **job:fetch:** cleanup some of the tech debt ([5dcd2de](https://github.com/Belphemur/AddictedProxy/commit/5dcd2de96611e1879a8e83148e4a2d7afa689388))
* **job:** reduce max concurrent job ([5b3451b](https://github.com/Belphemur/AddictedProxy/commit/5b3451b9ff8bec3e200899f41a7f087b38e8ba6a))
* **job:** update compatibility level ([b2926da](https://github.com/Belphemur/AddictedProxy/commit/b2926da419ac32371f547ff11cc5bca6590e4f14))
* **sentry:** remove duplicate handler ([b346efe](https://github.com/Belphemur/AddictedProxy/commit/b346efef90b0e10c9f797ddc06bd5d5b9df14e74))

### Bug Fixes

* **deps:** update dependency @sentry/vue to v8.47.0 ([721d9c4](https://github.com/Belphemur/AddictedProxy/commit/721d9c4bd83d76251d32f38ef13f6b22465667e8))
* **deps:** update dependency @sentry/vue to v8.48.0 ([90d3784](https://github.com/Belphemur/AddictedProxy/commit/90d37848e99d185cafc006886d2f2e063d311d7b))
* **deps:** update dependency bufferutil to v4.0.9 ([16ca3e3](https://github.com/Belphemur/AddictedProxy/commit/16ca3e32b81c762ea01c1645928c0772d9ca4330))
* **deps:** update semantic-release monorepo ([998a149](https://github.com/Belphemur/AddictedProxy/commit/998a149f24e4b83e9c1b67886f8e7f27a67321c5))
* **proxy-rotator:** fix getting locked ([e1c43e7](https://github.com/Belphemur/AddictedProxy/commit/e1c43e7a69b5fe9990828180ae32c75ee6a15e03))

### Features

* **proxy:** add a proxy rotator ([cab0ef9](https://github.com/Belphemur/AddictedProxy/commit/cab0ef94fb2a2b43ff16ec946f671d74aaeca0e3))

## [4.32.5](https://github.com/Belphemur/AddictedProxy/compare/v4.32.4...v4.32.5) (2024-12-18)

### Bug Fixes

* **deps:** update dependency vuetify to v3.7.6 ([e655958](https://github.com/Belphemur/AddictedProxy/commit/e655958763f5e9b99734b63ecc0acdd261d1018f))
* **job:unique:** clean up fingerprint if job is deleted ([aa5ce64](https://github.com/Belphemur/AddictedProxy/commit/aa5ce64a5ab9f9093be59c2fd9fbd3d95dac24ec))

## [4.32.4](https://github.com/Belphemur/AddictedProxy/compare/v4.32.3...v4.32.4) (2024-12-17)

### Bug Fixes

* **job:unique:** Be sure that unique job have a change to reuse the fingerprint if too old and wasn't cleanup properly ([120761e](https://github.com/Belphemur/AddictedProxy/commit/120761ea7e9bf936bf738131e14dc49596d23719))

## [4.32.3](https://github.com/Belphemur/AddictedProxy/compare/v4.32.2...v4.32.3) (2024-12-17)

### Bug Fixes

* **pinia:** use right version of pinia ([94f787d](https://github.com/Belphemur/AddictedProxy/commit/94f787dd684e9ccb44f0c5e3f3fdeb74fcd14f18))

## [4.32.2](https://github.com/Belphemur/AddictedProxy/compare/v4.32.1...v4.32.2) (2024-12-17)

### Bug Fixes

* **deps:** update dependency @sentry/vue to v8.46.0 ([f048486](https://github.com/Belphemur/AddictedProxy/commit/f0484867ff9d4a3f2f776f473ca37577e8052bb8))
* **job:** remove fingerprint when server process job ([6ea4087](https://github.com/Belphemur/AddictedProxy/commit/6ea408778c5eda8bcfb65e16102199caeea1e96a))
* **pinia:** fix issue with pinia version ([47d8ebd](https://github.com/Belphemur/AddictedProxy/commit/47d8ebdf181418834d51b7a00574b983a89d79dd))

## [4.32.1](https://github.com/Belphemur/AddictedProxy/compare/v4.32.0...v4.32.1) (2024-12-16)

### Performance improvements

* **disposing:** add disposing pattern for timer ([a504a4b](https://github.com/Belphemur/AddictedProxy/commit/a504a4bb90120a91742e14ea40f270e9e8ef5a95))
* **proxy-scrape:** add a little bit of jitter to avoid being exactly every x seconds ([ef762a4](https://github.com/Belphemur/AddictedProxy/commit/ef762a4d54dee0f946a0d6e018005515d27d75e3))
* **scraping:** add scraping interval for both AntiCaptcha and ProxyScrape ([1b0fd08](https://github.com/Belphemur/AddictedProxy/commit/1b0fd081e0acfac32bed9503b2228e9e72eade18))

## [4.32.0](https://github.com/Belphemur/AddictedProxy/compare/v4.31.0...v4.32.0) (2024-12-16)

### Performance improvements

* **anti-captcha:** add metrics to show balance left ([ae8793d](https://github.com/Belphemur/AddictedProxy/commit/ae8793db7ba25697389f5bac2c67cc5de22fba84))
* **anti-captcha:** use proper json context ([3e0323f](https://github.com/Belphemur/AddictedProxy/commit/3e0323ffe76649fb03c0b5d06b23385b82dd4b01))
* **proxy-scrape:** only try to get stats if in prod mode ([45aa012](https://github.com/Belphemur/AddictedProxy/commit/45aa0123a5572784c2af6b924436b21952a899f0))
* **proxy-scrape:** use json context ([00c402b](https://github.com/Belphemur/AddictedProxy/commit/00c402b28869fcfd3673464fd161b92e3cc222b9))

### Bug Fixes

* **proxy-scrape:** fix metrics not using the right max token ([1777027](https://github.com/Belphemur/AddictedProxy/commit/1777027bf1b8bcbf7e276c699d67ac438730214e))

### Features

* **anti-captcha:** add balance feature ([03f659e](https://github.com/Belphemur/AddictedProxy/commit/03f659e02c9cdd95755b95cf02f4d40c317404ba))

## [4.31.0](https://github.com/Belphemur/AddictedProxy/compare/v4.30.0...v4.31.0) (2024-12-16)

### Performance improvements

* **proxy-scrape:** no need to save more data than user agent ([7177878](https://github.com/Belphemur/AddictedProxy/commit/7177878bd7ec452c6367ebf5dfa651facd55d479))
* **tmdb:** add resilience ([4f6b845](https://github.com/Belphemur/AddictedProxy/commit/4f6b84565d0e9ea95a23387db85591a74243b5f1))

### Bug Fixes

* **anti-captcha:** remove standard hedging for now ([8206769](https://github.com/Belphemur/AddictedProxy/commit/8206769f711b15c5d2ebeaf42ae432799d7dd36a))
* **deps:** update dependency @sentry/vue to v8.45.0 ([79007fa](https://github.com/Belphemur/AddictedProxy/commit/79007fad86a57556fe5e2e54638f1b0a513b4de2))
* **deps:** update dependency @sentry/vue to v8.45.1 ([6cf2983](https://github.com/Belphemur/AddictedProxy/commit/6cf29838a1994954c8664905b316ed6f47224d82))
* **job:** avoid deprecation issue for job db ([dc0ee47](https://github.com/Belphemur/AddictedProxy/commit/dc0ee47bf88e27d5535bd039d747b27e104d3c89))
* **proxy-scrape:** 302 is expected ([cab48bd](https://github.com/Belphemur/AddictedProxy/commit/cab48bd8e36f23f0b24df420908c21e0c2917a5a))
* **proxy-scrape:** don't follow redirects ([6f2ab34](https://github.com/Belphemur/AddictedProxy/commit/6f2ab349e8bd676ddec84a119f69a3b4eb5c429b))
* **proxy-scrape:** fix wrong url used for login ([a06f143](https://github.com/Belphemur/AddictedProxy/commit/a06f14304901edddb404638fc53c5171e30718ad))
* **proxy-scrape:** fix wrong user agent sent ([b781ef4](https://github.com/Belphemur/AddictedProxy/commit/b781ef45a99882e00513b5736f9faae45cafdcbf))
* **proxy-scrape:** keep asking to get cf token ([492952b](https://github.com/Belphemur/AddictedProxy/commit/492952b50cc42bdad052bcfa4f3d901febd39d2f))
* **proxy-scrape:** use distributed cache to avoid losing the phpsessioid ([d585a18](https://github.com/Belphemur/AddictedProxy/commit/d585a18d91580beb1b5b89fa1b9246d408985c93))

### Features

* **anti-captcha:** provide anti-captcha solving ([431be30](https://github.com/Belphemur/AddictedProxy/commit/431be305aa7d61191b92c405770c1a969082756b))
* **inversion-control:** add concept of simple dependency ([2c095d4](https://github.com/Belphemur/AddictedProxy/commit/2c095d43644bd8d8b9d32c1a9408309200deb5bc))
* **proxy-scrape:** by pass login ([312a54e](https://github.com/Belphemur/AddictedProxy/commit/312a54e389f8aa1835651444c83217ef8ef225d2))

## [4.30.0](https://github.com/Belphemur/AddictedProxy/compare/v4.29.11...v4.30.0) (2024-12-12)

### Performance improvements

* **proxyscrape:** define base address and handler lifetime ([624ce9d](https://github.com/Belphemur/AddictedProxy/commit/624ce9df13b63c7ddd5541d917b67895579e5e22))

### Bug Fixes

* **deps:** update dependency @sentry/vue to v8.39.0 ([26ebc2d](https://github.com/Belphemur/AddictedProxy/commit/26ebc2db21a1ddf982c43daa4a2641d875cf2203))
* **deps:** update dependency @sentry/vue to v8.40.0 ([a51f891](https://github.com/Belphemur/AddictedProxy/commit/a51f89145f83e5b361c8f295bc5bb0c51fbb486b))
* **deps:** update dependency @sentry/vue to v8.41.0 ([98a144b](https://github.com/Belphemur/AddictedProxy/commit/98a144bd5705fcf32229ad02729c7ec24a77cac4))
* **deps:** update dependency @sentry/vue to v8.42.0 ([a11d905](https://github.com/Belphemur/AddictedProxy/commit/a11d905f58a6658283987f36ad6910a461ac843f))
* **deps:** update dependency @sentry/vue to v8.43.0 ([06946eb](https://github.com/Belphemur/AddictedProxy/commit/06946eb1788e384d021ceb05c64ed2fe9e3da240))
* **deps:** update dependency @sentry/vue to v8.44.0 ([ec01793](https://github.com/Belphemur/AddictedProxy/commit/ec01793be59b0eb64804f7ad4f6776c1013692ff))
* **deps:** update dependency vuetify to v3.7.5 ([e1ca991](https://github.com/Belphemur/AddictedProxy/commit/e1ca9917f7e04b7df7bfebfababea71d26ba7b5b))

### Features

* **proxyscrape:** add proxy scrape client ([b343706](https://github.com/Belphemur/AddictedProxy/commit/b3437066be2d89fbc6812fa349e47c3033acc56b))
* **proxyscrape:** give metrics in prometheus ([f481056](https://github.com/Belphemur/AddictedProxy/commit/f4810564dfd10ab7af35276bc6e5a594f71e5fa9))
* **proxyscrape:** use hosted service to gather metrics ([69efcd8](https://github.com/Belphemur/AddictedProxy/commit/69efcd83dd42a52f06fd071de0b242b27fafafed))

## [4.29.11](https://github.com/Belphemur/AddictedProxy/compare/v4.29.10...v4.29.11) (2024-11-15)

### Performance improvements

* **background:** be sure background is preloaded ([4a078bc](https://github.com/Belphemur/AddictedProxy/commit/4a078bcb730f1247da5946ba871f3715549fcf23))
* **media:poster:** Avoid preload both version of poster on mobile ([75388a1](https://github.com/Belphemur/AddictedProxy/commit/75388a1174823787535048057e9040f399b6ac92))
* **refresh:** avoid refreshing episode list when we got it at the end of refresh ([2aa1870](https://github.com/Belphemur/AddictedProxy/commit/2aa187067bd0cbfbe69b5d3b122d82e5c4689917))

### Bug Fixes

* **refresh:** fix issue with refreshing where nothing would happen ([3dfbb6d](https://github.com/Belphemur/AddictedProxy/commit/3dfbb6d9b04e85874bc85a918f4205eb24c98818))
* **refresh:** fix not refreshing episodes for all season found ([7d4e0f4](https://github.com/Belphemur/AddictedProxy/commit/7d4e0f4d6199e8bf1349e8a47faf99ee3abdf690))

## [4.29.10](https://github.com/Belphemur/AddictedProxy/compare/v4.29.9...v4.29.10) (2024-11-15)

### Performance improvements

* **icons:** stop using font for icons ([290e456](https://github.com/Belphemur/AddictedProxy/commit/290e456d44b6fe5a2d8e33a41f0062a79f753328))

## [4.29.9](https://github.com/Belphemur/AddictedProxy/compare/v4.29.8...v4.29.9) (2024-11-15)

### Performance improvements

* **chore:** update deps ([0825975](https://github.com/Belphemur/AddictedProxy/commit/0825975390f851a1587b5e094f512c824d4fae8a))

### Bug Fixes

* **deps:** update dependency @sentry/vue to v8.38.0 ([406650b](https://github.com/Belphemur/AddictedProxy/commit/406650bce989e8a99580b7f5565fdf4d2b40b558))
* **deps:** update dependency client-zip to v2.4.6 ([399c430](https://github.com/Belphemur/AddictedProxy/commit/399c430d37bff9561fe318f9b3aa62b813aff9ea))
* **show:media:** show that the show is refreshing ([aa984b2](https://github.com/Belphemur/AddictedProxy/commit/aa984b29bebec318f72e518e7ebd27416e1c0c6d))
* **show:** fix show not found on the website when not refreshed yet ([f818e2b](https://github.com/Belphemur/AddictedProxy/commit/f818e2b1854cfc1d63265f6c38367dee515cb6ab))

## [4.29.8](https://github.com/Belphemur/AddictedProxy/compare/v4.29.7...v4.29.8) (2024-11-08)

### Bug Fixes

* **media:** fix media detail page poster for normal and big resolutions ([4358aa2](https://github.com/Belphemur/AddictedProxy/commit/4358aa2338ee7e8918776a539fbc4ba33ded4443))

## [4.29.7](https://github.com/Belphemur/AddictedProxy/compare/v4.29.6...v4.29.7) (2024-11-08)

### Bug Fixes

* **media:** fix media detail page poster ([b492dbc](https://github.com/Belphemur/AddictedProxy/commit/b492dbc2084772eb251e8c2b181bccf32487c232))

## [4.29.6](https://github.com/Belphemur/AddictedProxy/compare/v4.29.5...v4.29.6) (2024-11-08)

### Bug Fixes

* **media:** fix media detail page poster ([2d8260c](https://github.com/Belphemur/AddictedProxy/commit/2d8260c6d3f386a77da61d83120ab6084220e4e2))

## [4.29.5](https://github.com/Belphemur/AddictedProxy/compare/v4.29.4...v4.29.5) (2024-11-08)

### Performance improvements

* **fetch:** add retry capabilities ([3a0d913](https://github.com/Belphemur/AddictedProxy/commit/3a0d913013bfa8431e98001a6444dc9773855cb2))

## [4.29.4](https://github.com/Belphemur/AddictedProxy/compare/v4.29.3...v4.29.4) (2024-11-08)

### Performance improvements

* **media:** improve image size for all size ([a63012b](https://github.com/Belphemur/AddictedProxy/commit/a63012b8dce3bb8efcb9ff2e4b67d4d966ae4e75))

## [4.29.3](https://github.com/Belphemur/AddictedProxy/compare/v4.29.2...v4.29.3) (2024-11-07)

### Performance improvements

* **trending:** improve display for all images ([e856382](https://github.com/Belphemur/AddictedProxy/commit/e856382492c5269cbe82d560f2f69e0d3f222d8b))

## [4.29.2](https://github.com/Belphemur/AddictedProxy/compare/v4.29.1...v4.29.2) (2024-11-07)

### Performance improvements

* **sentry:** move sentry and other libs to own chunks ([abfb071](https://github.com/Belphemur/AddictedProxy/commit/abfb07113ce16449e368a1d6b1bcc80f8669dd32))

## [4.29.1](https://github.com/Belphemur/AddictedProxy/compare/v4.29.0...v4.29.1) (2024-11-07)

### Bug Fixes

* **mobile:** make download button centered ([dc60aec](https://github.com/Belphemur/AddictedProxy/commit/dc60aec98c7dc836936e48967ba746c4c74576fb))

## [4.29.0](https://github.com/Belphemur/AddictedProxy/compare/v4.28.1...v4.29.0) (2024-11-07)

### Features

* **mobile:** improve mobile experience when downloading subtitle ([934f3f8](https://github.com/Belphemur/AddictedProxy/commit/934f3f8e693c1a6dba516682dd77486048dcb5a9))

## [4.28.1](https://github.com/Belphemur/AddictedProxy/compare/v4.28.0...v4.28.1) (2024-11-07)

### Performance improvements

* **image:** reduce size of image ([a2a60e2](https://github.com/Belphemur/AddictedProxy/commit/a2a60e2efc086b087e6053cf04a75abadb689d33))

## [4.28.0](https://github.com/Belphemur/AddictedProxy/compare/v4.27.1...v4.28.0) (2024-11-07)

### Performance improvements

* **media:** set the right width and height ([c89ec06](https://github.com/Belphemur/AddictedProxy/commit/c89ec06864a849ec5c2b302ad7d70952630f374e))
* **media:** use preload ([9d0143b](https://github.com/Belphemur/AddictedProxy/commit/9d0143be7439c5af7299cb65d0fb98c122c5e384))

### Features

* **picture:** add preload feature ([94c02a2](https://github.com/Belphemur/AddictedProxy/commit/94c02a2bf751fb53460d98d236ea3356ffdd690b))

## [4.27.1](https://github.com/Belphemur/AddictedProxy/compare/v4.27.0...v4.27.1) (2024-11-07)

### Performance improvements

* **media::details:** use backdrop image on mobile and fallback to poster when too big ([55bc4ec](https://github.com/Belphemur/AddictedProxy/commit/55bc4ec40a930780a08def48972bcc3ed0c380e8))
* **picture:** be sure picture can be configured with different source for specific needs with extra media query ([48fa58e](https://github.com/Belphemur/AddictedProxy/commit/48fa58e98bfc26e46383dd97ebb9a1e80d9e73e4))

## [4.27.0](https://github.com/Belphemur/AddictedProxy/compare/v4.26.0...v4.27.0) (2024-11-07)

### Performance improvements

* **image:** remove nuxt-image ([35defc6](https://github.com/Belphemur/AddictedProxy/commit/35defc60a3944c55002b9564519129693dbe4350))
* **media::detail:** use new optimized image ([146cbba](https://github.com/Belphemur/AddictedProxy/commit/146cbba8d1702a36309dfd906b2e49f1d8f4885b))
* **trending:** move to optimized images ([78a9a0f](https://github.com/Belphemur/AddictedProxy/commit/78a9a0f9eb77ad0063dd03e43c4f6472d6f02b85))

### Bug Fixes

* **deps:** update dependency vuetify to v3.7.4 ([7a7305f](https://github.com/Belphemur/AddictedProxy/commit/7a7305f7c2e7dd033f5322ce4cee81076bfc7fc2))
* **image:** use srcset and also specify width and height when known ([e7a5dec](https://github.com/Belphemur/AddictedProxy/commit/e7a5dec1e6333d78269b07f6f85238cdc022db3f))

### Features

* **image:** create own component for optimized picture instead of nuxt-image ([c50cb26](https://github.com/Belphemur/AddictedProxy/commit/c50cb26156e1d74a49252f99ddc4e93a0b4b80d8))

## [4.26.0](https://github.com/Belphemur/AddictedProxy/compare/v4.25.8...v4.26.0) (2024-11-06)

### Bug Fixes

* **mobile:** fix mobile horizontal scrolling ([d8e9601](https://github.com/Belphemur/AddictedProxy/commit/d8e9601b3690a3a138bfa4ac0d708dcac31fb153))

### Features

* **navigation:** only use drawer for mobile ([b291866](https://github.com/Belphemur/AddictedProxy/commit/b2918663e217f696df84d61457787e21c277bbf1))

## [4.25.8](https://github.com/Belphemur/AddictedProxy/compare/v4.25.7...v4.25.8) (2024-11-05)

### Performance improvements

* **ci:** improve building image ([bc04e6f](https://github.com/Belphemur/AddictedProxy/commit/bc04e6fc2d2d65b23010bf70cf7ef28a8feb999a))

## [4.25.7](https://github.com/Belphemur/AddictedProxy/compare/v4.25.6...v4.25.7) (2024-11-05)

### Performance improvements

* **vuetify:** better integration with nuxt ([95a3d93](https://github.com/Belphemur/AddictedProxy/commit/95a3d930882c7a3e5613d2dd63a3349d5dca2387))

### Bug Fixes

* **ci:** building docker image front-end ([c251483](https://github.com/Belphemur/AddictedProxy/commit/c2514835eaf38ceec8f3f1c6f6f9279101c1e2d5))

## [4.25.6](https://github.com/Belphemur/AddictedProxy/compare/v4.25.5...v4.25.6) (2024-11-05)

### Performance improvements

* **download:** avoid robot to scrape download links ([c460edf](https://github.com/Belphemur/AddictedProxy/commit/c460edf25a446facb29b8db8fa91b73370edc6e0))

### Bug Fixes

* **deps:** update dependency semantic-release to v24.2.0 ([2d01067](https://github.com/Belphemur/AddictedProxy/commit/2d010673c657c3629ea0f204baee2007cac75dde))
* **deps:** update dependency utf-8-validate to v6.0.5 ([7a820ee](https://github.com/Belphemur/AddictedProxy/commit/7a820eef6a844cda3c3bc0a02b594a67bd28abb4))
* **result:** remove unneeded extension ([2a3811e](https://github.com/Belphemur/AddictedProxy/commit/2a3811e2417ffbd54fc826110cfa261fc31d5600))

## [4.25.5](https://github.com/Belphemur/AddictedProxy/compare/v4.25.4...v4.25.5) (2024-10-23)

### Bug Fixes

* **ci:** fix missing ws lib in built server ([6bfafc7](https://github.com/Belphemur/AddictedProxy/commit/6bfafc71e56532b516b7ba9ee5672ad13c4fe9a0))

## [4.25.4](https://github.com/Belphemur/AddictedProxy/compare/v4.25.3...v4.25.4) (2024-10-23)

### Bug Fixes

* **websocket:** fix missing websocket lib ([5586a6f](https://github.com/Belphemur/AddictedProxy/commit/5586a6f3d7ee29a3d075cae9e5d906c79f8a6067))

## [4.25.3](https://github.com/Belphemur/AddictedProxy/compare/v4.25.2...v4.25.3) (2024-10-23)

### Bug Fixes

* **sitemap:** fix issue with sitemap not proxied properly ([0a4b6fa](https://github.com/Belphemur/AddictedProxy/commit/0a4b6faad06fe5e5fc2dffd7a5aa36a8f2673676))

## [4.25.2](https://github.com/Belphemur/AddictedProxy/compare/v4.25.1...v4.25.2) (2024-10-23)

### Bug Fixes

* **ci:** fix building frontend image ([3fb7dda](https://github.com/Belphemur/AddictedProxy/commit/3fb7ddab29b9d2ff74788b0d1cbf9d8a2944786b))
* **ci:** remove step relating to old build process ([9464e2d](https://github.com/Belphemur/AddictedProxy/commit/9464e2d77b1843b61cee8112e19ee7f256a95436))
* **frontend:** missing transpile ([f42754d](https://github.com/Belphemur/AddictedProxy/commit/f42754d9e6756d1932c4e4b004a74d250daad32e))

## [4.25.1](https://github.com/Belphemur/AddictedProxy/compare/v4.25.0...v4.25.1) (2024-10-23)

### Bug Fixes

* **frontend:** use lts image for node ([bd91891](https://github.com/Belphemur/AddictedProxy/commit/bd918917e8ef27f2d354d30e2137de3af24b7200))

## [4.25.0](https://github.com/Belphemur/AddictedProxy/compare/v4.24.0...v4.25.0) (2024-10-23)

### Features

* **media:page:** use optimized endpoint ([f0adc82](https://github.com/Belphemur/AddictedProxy/commit/f0adc82b9623f1f6e79e02dbf28f90fb4578380e))

## [4.24.0](https://github.com/Belphemur/AddictedProxy/compare/v4.23.6...v4.24.0) (2024-10-23)

### Performance improvements

* **messagepack:** use message pack for SignalR ([61b926b](https://github.com/Belphemur/AddictedProxy/commit/61b926b5006aa12d68a4db9b416c4c6d884348b5))
* **nuxt:** update to new contract ([6e98b34](https://github.com/Belphemur/AddictedProxy/commit/6e98b34ea2642ebcdc20662624c8b4a0f8470f9a))

### Bug Fixes

* data contract for media episode ([3d470a0](https://github.com/Belphemur/AddictedProxy/commit/3d470a030daa0025aeb3ded03ac5625c0e3087b2))
* **deps:** update dependency semantic-release to v24.1.3 ([e4bfa31](https://github.com/Belphemur/AddictedProxy/commit/e4bfa319d70250e9e6de45af02f3b092a7d9cf31))
* **deps:** update dependency vuetify to v3.7.3 ([80c4290](https://github.com/Belphemur/AddictedProxy/commit/80c4290b504c51407fe828cd40bdf840a9629596))

### Features

* **media:** add new route to get media details and subtitles instead of hub ([be074b4](https://github.com/Belphemur/AddictedProxy/commit/be074b40cff664276f9b0f2827ff029bda38cd59))
* **media:** add signalR endpoint to get show data for view ([e9608ec](https://github.com/Belphemur/AddictedProxy/commit/e9608ece04fd49d3c7b1b2ace78500bdad70b9f0))

## [4.23.6](https://github.com/Belphemur/AddictedProxy/compare/v4.23.5...v4.23.6) (2024-10-15)

### Performance improvements

* update all deps ([d9dcacd](https://github.com/Belphemur/AddictedProxy/commit/d9dcacd3f4f6f14341b0dd22f86cd524399d01d8))

### Bug Fixes

* clean up testing code ([80d00c1](https://github.com/Belphemur/AddictedProxy/commit/80d00c166a63e11a1bd540292bbbba7b92a8077e))
* **deps:** update dependency semantic-release to v24.1.2 ([ce73fe7](https://github.com/Belphemur/AddictedProxy/commit/ce73fe7921f35d65cd329b58cacf62c025e5b500))

## [4.23.5](https://github.com/Belphemur/AddictedProxy/compare/v4.23.4...v4.23.5) (2024-09-19)

### Performance improvements

* move to .NET 9.0 ([97cec8b](https://github.com/Belphemur/AddictedProxy/commit/97cec8b154043522b68be74f8b6102288a158950))
* update deps ([fbaa058](https://github.com/Belphemur/AddictedProxy/commit/fbaa05880c81389a101c8ecdb20e7882f173d398))

### Bug Fixes

* **deps:** update dependency semantic-release to v24.1.1 ([4fe5502](https://github.com/Belphemur/AddictedProxy/commit/4fe55021b1e3681b8a61002a21e524711d2ea6f5))
* **deps:** update dependency vuetify to v3.7.2 ([ae1a3e7](https://github.com/Belphemur/AddictedProxy/commit/ae1a3e7be72f9d9d213fa9cb60d3e7ba8a08ac2b))
* warning about wrong HTML ([442734c](https://github.com/Belphemur/AddictedProxy/commit/442734c4f594c010bcb61ff525d87022170b9bd4))

## [4.23.4](https://github.com/Belphemur/AddictedProxy/compare/v4.23.3...v4.23.4) (2024-08-30)

### Bug Fixes

* **deps:** update dependency vuetify to v3.7.1 ([8139350](https://github.com/Belphemur/AddictedProxy/commit/8139350c7c0ffd045edc70468dcf048652444ce3))
* **media:** remove frozen dictionary, not supported by caching. ([faed41d](https://github.com/Belphemur/AddictedProxy/commit/faed41ded5217f8a25bdbbba687f4566bbceff62))

## [4.23.3](https://github.com/Belphemur/AddictedProxy/compare/v4.23.2...v4.23.3) (2024-08-22)

### Bug Fixes

* can't use non entity framework supported methods ([b0e66f7](https://github.com/Belphemur/AddictedProxy/commit/b0e66f7343d3501edcb11a05642c2e33b2074187))

## [4.23.2](https://github.com/Belphemur/AddictedProxy/compare/v4.23.1...v4.23.2) (2024-08-22)

### Performance improvements

* update result dependency ([3195c75](https://github.com/Belphemur/AddictedProxy/commit/3195c75ca441f2e45f52a70773a9f24398c1e5df))

### Bug Fixes

* **deps:** update dependency semantic-release to v24.1.0 ([8956e7e](https://github.com/Belphemur/AddictedProxy/commit/8956e7ecebe8d495fd972b11e640984535f7c8a6))
* **deps:** update dependency vuetify to v3.7.0 ([6d3b8cd](https://github.com/Belphemur/AddictedProxy/commit/6d3b8cddd69de5a338c4821e098df4cef783d75a))

## [4.23.1](https://github.com/Belphemur/AddictedProxy/compare/v4.23.0...v4.23.1) (2024-08-15)

### Bug Fixes

* don't follow redirect for client either ([5a0a114](https://github.com/Belphemur/AddictedProxy/commit/5a0a114acd22213a260ba2ed579523a55f4db840))

## [4.23.0](https://github.com/Belphemur/AddictedProxy/compare/v4.22.13...v4.23.0) (2024-08-14)

### Performance improvements

* add logging ([955c44c](https://github.com/Belphemur/AddictedProxy/commit/955c44cb868c5a7f225cc0da0c5cd5965648cbf3))
* improve the httputils class ([bd7ae78](https://github.com/Belphemur/AddictedProxy/commit/bd7ae7807d13d9cd360669df12222c2dfbf808c3))

### Bug Fixes

* missing migration date ([1cfb923](https://github.com/Belphemur/AddictedProxy/commit/1cfb92365475b3618b16b1dc47190862d09eaf1e))

### Features

* add cleanup inbox for addicted creds ([6c0c91b](https://github.com/Belphemur/AddictedProxy/commit/6c0c91b17021fe68390d5c4770e512d74ebf2589))
* clean up inbox of all user and reset creds ([d515363](https://github.com/Belphemur/AddictedProxy/commit/d5153635314a6a30013289ee38c80d4526bd8487))

## [4.22.13](https://github.com/Belphemur/AddictedProxy/compare/v4.22.12...v4.22.13) (2024-08-14)

### Bug Fixes

* **deps:** update dependency vuetify to v3.6.14 ([4bfaf30](https://github.com/Belphemur/AddictedProxy/commit/4bfaf30eed333c9e011f27e1f6f12b125ed542ee))

## [4.22.12](https://github.com/Belphemur/AddictedProxy/compare/v4.22.11...v4.22.12) (2024-07-19)

### Performance improvements

* update user agent ([29b2274](https://github.com/Belphemur/AddictedProxy/commit/29b227428ebe2ccfc30b4bf35941b14f3ad2f4f7))

### Bug Fixes

* **deps:** update dependency @microsoft/signalr to v8.0.7 ([b365538](https://github.com/Belphemur/AddictedProxy/commit/b365538759f94cf03d38fb4589e1b1341ec02764))
* **deps:** update dependency vuetify to v3.6.11 ([d75968a](https://github.com/Belphemur/AddictedProxy/commit/d75968a5d79961a9e9b168d60203b507835d4f0f))
* **deps:** update dependency vuetify to v3.6.12 ([4a73828](https://github.com/Belphemur/AddictedProxy/commit/4a7382823e6c9fd37b97d8bc362091270c36b4c8))
* **deps:** update dependency vuetify to v3.6.13 ([010f375](https://github.com/Belphemur/AddictedProxy/commit/010f37521e822f1678ec95f3f43322d25ba0ab13))

## [4.22.11](https://github.com/Belphemur/AddictedProxy/compare/v4.22.10...v4.22.11) (2024-06-21)

### Performance improvements

* **trending:** Use all media present in props ([8b9a6a1](https://github.com/Belphemur/AddictedProxy/commit/8b9a6a10a977e4b2b3f37b7b39ad772616a3db99))

## [4.22.10](https://github.com/Belphemur/AddictedProxy/compare/v4.22.9...v4.22.10) (2024-06-21)

### Performance improvements

* **trending:** Optimize the trending loading to always return the right number of shows ([9e41ec6](https://github.com/Belphemur/AddictedProxy/commit/9e41ec623e6ff646d983c9ccfdcef86f20eeb9cf))

### Bug Fixes

* **deps:** update dependency vuetify to v3.6.10 ([614464a](https://github.com/Belphemur/AddictedProxy/commit/614464ae51bb4a1d162f83eb19fd1bea4cef65b1))
* **deps:** update dependency vuetify to v3.6.9 ([db3d19c](https://github.com/Belphemur/AddictedProxy/commit/db3d19c049ee5dd069578525c488a92287af842e))

## [4.22.9](https://github.com/Belphemur/AddictedProxy/compare/v4.22.8...v4.22.9) (2024-06-06)

### Performance improvements

* **trending:** show more trending if desktop ([6ff7a2c](https://github.com/Belphemur/AddictedProxy/commit/6ff7a2c0ea729cbd6fef02db4a4ad84d3f628a16))

## [4.22.8](https://github.com/Belphemur/AddictedProxy/compare/v4.22.7...v4.22.8) (2024-06-06)

### Performance improvements

* **caching:** change path of images ([14d063e](https://github.com/Belphemur/AddictedProxy/commit/14d063e61adb221336956bc9329321900303009d))

## [4.22.7](https://github.com/Belphemur/AddictedProxy/compare/v4.22.6...v4.22.7) (2024-06-06)

### Performance improvements

* add sharding key to metric and span ([913f5d3](https://github.com/Belphemur/AddictedProxy/commit/913f5d35da8d5b04793275fe4f5bc622da4cd988))
* **caching:** improve the caching mechanics ([979a2e3](https://github.com/Belphemur/AddictedProxy/commit/979a2e32af9fe93c4eb9e9a15ed6b05dd8ecb70e))

## [4.22.6](https://github.com/Belphemur/AddictedProxy/compare/v4.22.5...v4.22.6) (2024-06-03)

### Bug Fixes

* **download:bulk:** Make the download count increase ([cd5a79e](https://github.com/Belphemur/AddictedProxy/commit/cd5a79ee1e94c5c1033d09c30ad602415d20ca62))

## [4.22.5](https://github.com/Belphemur/AddictedProxy/compare/v4.22.4...v4.22.5) (2024-05-30)


### Bug Fixes

* **background:** fix background image size ([8d2829e](https://github.com/Belphemur/AddictedProxy/commit/8d2829ec7900d82ffc362174b7c170f05dc50ad2))
* **background:** use already resized background ([95975f8](https://github.com/Belphemur/AddictedProxy/commit/95975f8e6ba2dccaabf9b1648eae43f2db6f45e2))

## [4.22.4](https://github.com/Belphemur/AddictedProxy/compare/v4.22.3...v4.22.4) (2024-05-30)


### Bug Fixes

* **bulk:** Avoid issue where one of the subtitle is now unavailable ([98fc710](https://github.com/Belphemur/AddictedProxy/commit/98fc710fb41139e91f4ee8f8ac0742cc405bd951))
* **deps:** update dependency vuetify to v3.6.8 ([7c6817e](https://github.com/Belphemur/AddictedProxy/commit/7c6817e5539aa45309530493d70d0590ae529857))

## [4.22.3](https://github.com/Belphemur/AddictedProxy/compare/v4.22.2...v4.22.3) (2024-05-27)


### Bug Fixes

* **sitemap:** add proper styling ([4780667](https://github.com/Belphemur/AddictedProxy/commit/4780667e3ff8c0e6cb8dc56db60854b8ab99d553))

## [4.22.2](https://github.com/Belphemur/AddictedProxy/compare/v4.22.1...v4.22.2) (2024-05-27)


### Bug Fixes

* sitemap access from front-end ([22daa40](https://github.com/Belphemur/AddictedProxy/commit/22daa40240b8eb984a4d356c8937cae50666b3f1))

## [4.22.1](https://github.com/Belphemur/AddictedProxy/compare/v4.22.0...v4.22.1) (2024-05-25)


### Performance improvements

* **sentry:** add sentry for the nitro server ([276b958](https://github.com/Belphemur/AddictedProxy/commit/276b958227319707a8f9476593cb551024752247))
* **sentry:** remove sentry profiling server ([1816d68](https://github.com/Belphemur/AddictedProxy/commit/1816d68af0727b21633c38aaa69dfad80840aa57))


### Bug Fixes

* **sentry:** set tracing propagation correctly ([2628f80](https://github.com/Belphemur/AddictedProxy/commit/2628f80dc08d16e67f08c15323131ddddf7fc166))

## [4.22.0](https://github.com/Belphemur/AddictedProxy/compare/v4.21.0...v4.22.0) (2024-05-25)


### Features

* **bulk:** save the choice of the user for the type of subtitle ([c3d98c7](https://github.com/Belphemur/AddictedProxy/commit/c3d98c7d0e4314556fbbfcba2807034d1445ef5d))

## [4.21.0](https://github.com/Belphemur/AddictedProxy/compare/v4.20.21...v4.21.0) (2024-05-24)


### Bug Fixes

* **deps:** update sentry-javascript monorepo to v8.3.0 ([943f0b6](https://github.com/Belphemur/AddictedProxy/commit/943f0b64d9d6972b4491cc8313a2a27f1226d271))
* **deps:** update sentry-javascript monorepo to v8.4.0 ([20d46b7](https://github.com/Belphemur/AddictedProxy/commit/20d46b7c46c1c0d6efcf597ddeac071e38dd6c98))
* **subtitle::download:** be sure the scene is present in the file name and not the version ([11d5b3e](https://github.com/Belphemur/AddictedProxy/commit/11d5b3eae0ec8f7e8155a64279a4f3e2aa0ecbdb))


### Features

* **bulk:** add bulk download for a full season ([6b6987f](https://github.com/Belphemur/AddictedProxy/commit/6b6987fb329d5ad3b21eaee9d28b95c6dc701cff)), closes [#142](https://github.com/Belphemur/AddictedProxy/issues/142)
* **subtitle:** add version of subtitle in the path of the file ([a855b97](https://github.com/Belphemur/AddictedProxy/commit/a855b97385747197d9e6df68a323ba52f85d09f9))

## [4.20.21](https://github.com/Belphemur/AddictedProxy/compare/v4.20.20...v4.20.21) (2024-05-22)


### Performance improvements

* **cloudflare:** add beacon for cloudflare ([6286a5d](https://github.com/Belphemur/AddictedProxy/commit/6286a5de431a5b44688ca9fd7a6a4a6465deb570))

## [4.20.20](https://github.com/Belphemur/AddictedProxy/compare/v4.20.19...v4.20.20) (2024-05-22)


### Performance improvements

* **privacy:** update privacy policy ([28c33c3](https://github.com/Belphemur/AddictedProxy/commit/28c33c3f3f51aa00f6316c5a1c763b0b5fcad042))
* **websocket:** let's the lib call start when needed ([32351c5](https://github.com/Belphemur/AddictedProxy/commit/32351c5f69699efdda6dea4c81ee49e88508d5e6))


### Bug Fixes

* **deps:** update dependency vuetify to v3.6.7 ([e86c743](https://github.com/Belphemur/AddictedProxy/commit/e86c74397fd5e99e56a84975f1a910a53010df89))

## [4.20.19](https://github.com/Belphemur/AddictedProxy/compare/v4.20.18...v4.20.19) (2024-05-22)


### Performance improvements

* **websocket:** let's the lib call start when needed ([ea4aae4](https://github.com/Belphemur/AddictedProxy/commit/ea4aae40275205084fb3b76b9bb1479265a86c9a))

## [4.20.18](https://github.com/Belphemur/AddictedProxy/compare/v4.20.17...v4.20.18) (2024-05-21)


### Bug Fixes

* **sentry:** clean up bad merge code ([3c5fe2f](https://github.com/Belphemur/AddictedProxy/commit/3c5fe2fedee7c3c6fcdc8fe209f4483f7ac1ed25))

## [4.20.17](https://github.com/Belphemur/AddictedProxy/compare/v4.20.16...v4.20.17) (2024-05-21)


### Bug Fixes

* **cors:** proper regex matching ([94f6acc](https://github.com/Belphemur/AddictedProxy/commit/94f6acc28d74f6eecb428804ca9806b20c0df50c))
* **deps:** update sentry-javascript monorepo to v8.2.1 ([829490e](https://github.com/Belphemur/AddictedProxy/commit/829490e8b0480d537465fb799f316a9a28906384))
* **deps:** update sentry-javascript monorepo to v8.2.1 ([b778c2c](https://github.com/Belphemur/AddictedProxy/commit/b778c2cfa0d1fce64fb67be13e3360d68717933d))
* **imports:** fix imports ([069fd09](https://github.com/Belphemur/AddictedProxy/commit/069fd09fec4cc681b65b5aade0cfc814038cdeb9))
* **sentry:** fix performance tracker sentry ([766c441](https://github.com/Belphemur/AddictedProxy/commit/766c441b4cd9142ed98ff8434d725e633ec9b534))
* **sentry:** fix performance tracker sentry ([acc51e4](https://github.com/Belphemur/AddictedProxy/commit/acc51e414fa267a5ec3688dbcaac2c7b87f23fcf))

## [4.20.16](https://github.com/Belphemur/AddictedProxy/compare/v4.20.15...v4.20.16) (2024-05-15)


### Bug Fixes

* **fetch:** always remove the header that worker don't like ([44e2cbe](https://github.com/Belphemur/AddictedProxy/commit/44e2cbe921162d1ae4a994719ca729d2f4bec51e))
* **fetch:** properly remove the credentials and mode from right object ([194210e](https://github.com/Belphemur/AddictedProxy/commit/194210ef728bee572767152d45ad447c046cdf5c))
* **fetch:** Remove referer policy ([556c5f4](https://github.com/Belphemur/AddictedProxy/commit/556c5f472ae29d55ab53f29fd1d7eb1970c5ea25))

## [4.20.15](https://github.com/Belphemur/AddictedProxy/compare/v4.20.14...v4.20.15) (2024-05-15)


### Bug Fixes

* **sentry:** only add span for request from client side ([4cd84fb](https://github.com/Belphemur/AddictedProxy/commit/4cd84fb0c3b32542b8727bee4ef707efb957cad0))

## [4.20.14](https://github.com/Belphemur/AddictedProxy/compare/v4.20.13...v4.20.14) (2024-05-15)


### Performance improvements

* **cors:** accept anything from cloudflare page for the project ([cb02561](https://github.com/Belphemur/AddictedProxy/commit/cb025618259e065a2365a93eefb6aafa0f417cc2))
* **deps:** update deps ([b1f7009](https://github.com/Belphemur/AddictedProxy/commit/b1f7009b89f7aba7c86272938adadbca98273258))
* **sentry:** accept baggage header ([5226d48](https://github.com/Belphemur/AddictedProxy/commit/5226d488b1bac679b58ecf7439f8dd7059a0eaba))
* **sentry:** add sentry profile rate ([40fca78](https://github.com/Belphemur/AddictedProxy/commit/40fca78fabdab7d3ee3903b849d92cfb4dfb6589))
* **sentry:** reenable sentry in backend ([f5aba9c](https://github.com/Belphemur/AddictedProxy/commit/f5aba9c577b62d891c9b2d6a534ae5d1d6f20922))
* **sentry:** set distributed tracing ([65902b7](https://github.com/Belphemur/AddictedProxy/commit/65902b74ff0b2280a5bc21567e0dbd732a3a0fed))
* **tracing:** reenable distributed tracing in sentry ([d7504cf](https://github.com/Belphemur/AddictedProxy/commit/d7504cfcc86d884939f1caa80004ce40d2c6c806))

## [4.20.13](https://github.com/Belphemur/AddictedProxy/compare/v4.20.12...v4.20.13) (2024-05-15)


### Performance improvements

* **sentry:** add profiling ([063eeb3](https://github.com/Belphemur/AddictedProxy/commit/063eeb3c2ae836909f359360dfba812a26e049c5))

## [4.20.12](https://github.com/Belphemur/AddictedProxy/compare/v4.20.11...v4.20.12) (2024-05-15)


### Performance improvements

* **sentry:** enable inp ([bab021d](https://github.com/Belphemur/AddictedProxy/commit/bab021d02e27d3313a8d9ca2e5c6b7d1f1911c4a))

## [4.20.11](https://github.com/Belphemur/AddictedProxy/compare/v4.20.10...v4.20.11) (2024-05-15)


### Performance improvements

* **cors:** enable cloudflare cors ([f37e470](https://github.com/Belphemur/AddictedProxy/commit/f37e470bcb1d2a6b1f7b082736280b835158b1aa))
* **deps:** update dependencies ([1bb5c7f](https://github.com/Belphemur/AddictedProxy/commit/1bb5c7f3bfbf0b06d769eb76e791eb31d232e56d))
* **websocket:** only start the socket when asking to refresh ([3ca863a](https://github.com/Belphemur/AddictedProxy/commit/3ca863a5eeecfee1c1482cf8b4a4d32fd0be1fcc))


### Bug Fixes

* **deps:** update dependency vuetify to v3.6.6 ([7d552a9](https://github.com/Belphemur/AddictedProxy/commit/7d552a9b0ab74eb1384935b437ad103c3c0d083b))

## [4.20.10](https://github.com/Belphemur/AddictedProxy/compare/v4.20.9...v4.20.10) (2024-05-13)


### Performance improvements

* **sentry:** Add sentry ([3082c1f](https://github.com/Belphemur/AddictedProxy/commit/3082c1f2205506f0c7ba248bd63bfbcb84460676))

## [4.20.9](https://github.com/Belphemur/AddictedProxy/compare/v4.20.8...v4.20.9) (2024-05-12)


### Bug Fixes

* **cloudflare:** remove mode ([eed321c](https://github.com/Belphemur/AddictedProxy/commit/eed321c0f68b7e5adf4748c9d11e275693ff3e74))

## [4.20.8](https://github.com/Belphemur/AddictedProxy/compare/v4.20.7...v4.20.8) (2024-05-12)


### Bug Fixes

* **download:** issue with bad filename for downloading subtitles ([8be2f89](https://github.com/Belphemur/AddictedProxy/commit/8be2f891c2b747f0af17c52be5a006b87a5655b6))

## [4.20.7](https://github.com/Belphemur/AddictedProxy/compare/v4.20.6...v4.20.7) (2024-05-12)


### Bug Fixes

* **cors:** be sure cloudflare is accepted ([5a76760](https://github.com/Belphemur/AddictedProxy/commit/5a76760872f8e88ac3c1ada752ec9605dd7146d0))

## [4.20.6](https://github.com/Belphemur/AddictedProxy/compare/v4.20.5...v4.20.6) (2024-05-11)


### Bug Fixes

* **deps:** update dependency semantic-release to v23.1.0 ([5574626](https://github.com/Belphemur/AddictedProxy/commit/557462633c43a554f1388f82a8b1aec814f4ed79))
* **deps:** update dependency semantic-release to v23.1.1 ([454c76f](https://github.com/Belphemur/AddictedProxy/commit/454c76ffeb3d44443a5bf53ba4da9ee0679b8a22))
* **deps:** update dependency vuetify to v3.6.5 ([c29e557](https://github.com/Belphemur/AddictedProxy/commit/c29e557ce368f6b53f6d6c3064154b8bbc28a491))
* **frontend:** building the app on cloudflare ([080e758](https://github.com/Belphemur/AddictedProxy/commit/080e75849d27b574707588e52fc3c8740b67177c))

## [4.20.5](https://github.com/Belphemur/AddictedProxy/compare/v4.20.4...v4.20.5) (2024-05-09)


### Bug Fixes

* **attestation:** use lower case name for repo ([97ddab2](https://github.com/Belphemur/AddictedProxy/commit/97ddab27265400586681b7e0680c1990707c5cb5))

## [4.20.4](https://github.com/Belphemur/AddictedProxy/compare/v4.20.3...v4.20.4) (2024-05-09)


### Bug Fixes

* **attestation:** fix the subject name ([0b25c5f](https://github.com/Belphemur/AddictedProxy/commit/0b25c5fc51af19999e116b0caae3d681ce837429))

## [4.20.3](https://github.com/Belphemur/AddictedProxy/compare/v4.20.2...v4.20.3) (2024-05-09)


### Performance improvements

* **image:** attest image ([6b438c1](https://github.com/Belphemur/AddictedProxy/commit/6b438c1b65f9194814bbabcb8b13bd9566b970c4))

## [4.20.2](https://github.com/Belphemur/AddictedProxy/compare/v4.20.1...v4.20.2) (2024-05-09)


### Performance improvements

* **metrics:** disable default .NET metrics ([141ea8f](https://github.com/Belphemur/AddictedProxy/commit/141ea8f49391f62f6292e4a28bbfae4f856d39a4))


### Bug Fixes

* **deps:** update dependency semantic-release to v23.0.8 ([66f08ea](https://github.com/Belphemur/AddictedProxy/commit/66f08ea13da3d43cc3c502ba84fedd8648e44799))
* **deps:** update dependency vuetify to v3.5.15 ([c002c66](https://github.com/Belphemur/AddictedProxy/commit/c002c667b64c546b9c04082b294b0da3365c0a74))
* **deps:** update dependency vuetify to v3.5.16 ([ec6399d](https://github.com/Belphemur/AddictedProxy/commit/ec6399d722efc322d80bc0d20aa4f7534c1d3066))
* **deps:** update dependency vuetify to v3.5.17 ([a98b15c](https://github.com/Belphemur/AddictedProxy/commit/a98b15c8209cdede8fe0cb55068244d6e5da1857))
* **deps:** update dependency vuetify to v3.6.0 ([0d1e6e0](https://github.com/Belphemur/AddictedProxy/commit/0d1e6e083fdd3ca3fc4319258b880fc961e7b90d))
* **deps:** update dependency vuetify to v3.6.1 ([5828cfa](https://github.com/Belphemur/AddictedProxy/commit/5828cfa24a14c45f8cfaa9a26f7703b92ae47dbe))
* **deps:** update dependency vuetify to v3.6.3 ([9207e26](https://github.com/Belphemur/AddictedProxy/commit/9207e261d2daf5854b07a4a58c4a9be683c88454))
* **deps:** update dependency vuetify to v3.6.4 ([5cfeb9f](https://github.com/Belphemur/AddictedProxy/commit/5cfeb9f5a2d73fe3ea694d400d8fd0e93d28711a))
* **release:** downgrade bad lib ([7270700](https://github.com/Belphemur/AddictedProxy/commit/727070057e54950e977b17b98fb235d032db47cc))

## [4.20.1](https://github.com/Belphemur/AddictedProxy/compare/v4.20.0...v4.20.1) (2024-04-05)


### Bug Fixes

* **front-end:** background of the page to be smaller ([0b79bb5](https://github.com/Belphemur/AddictedProxy/commit/0b79bb579fe6ba80df49150c95410f6f72940fc4))

## [4.20.0](https://github.com/Belphemur/AddictedProxy/compare/v4.19.19...v4.20.0) (2024-04-04)


### Performance improvements

* **seo:** improve SEO ([8328af2](https://github.com/Belphemur/AddictedProxy/commit/8328af272d5b8efed77f265773c55eeeee7eabd6))
* **seo:** improve SEO ([8f80226](https://github.com/Belphemur/AddictedProxy/commit/8f80226efcbde18d6c210051765583b8bb58cfd0))
* **show:** move the slug to the ShowDto ([2939daa](https://github.com/Belphemur/AddictedProxy/commit/2939daa48d33bc4213f34f839b2a8b67645ff2db))


### Features

* **slug:** properly generate slug for media ([e303b2e](https://github.com/Belphemur/AddictedProxy/commit/e303b2ea082d301dc36c3423073760daa928c059))

## [4.19.19](https://github.com/Belphemur/AddictedProxy/compare/v4.19.18...v4.19.19) (2024-04-04)


### Performance improvements

* **frontend:** add language ([386ff07](https://github.com/Belphemur/AddictedProxy/commit/386ff07b310366847dfb886ebf05d6d6b24db0e7))
* **frontend:** have H1 for SEO ([94eb079](https://github.com/Belphemur/AddictedProxy/commit/94eb0793c1bad41e4e4159664815fb672e2f0f2c))


### Bug Fixes

* **deps:** update dependency semantic-release to v23.0.3 ([8fdc848](https://github.com/Belphemur/AddictedProxy/commit/8fdc84842438d14c34ddb6f2b7736b0f81717079))
* **deps:** update dependency semantic-release to v23.0.4 ([ab93fad](https://github.com/Belphemur/AddictedProxy/commit/ab93fad97c55b8078f394f583aa69d0b8135496a))
* **deps:** update dependency semantic-release to v23.0.5 ([31c650f](https://github.com/Belphemur/AddictedProxy/commit/31c650f223061120a60291d9c5a3f4becbe1ed0d))
* **deps:** update dependency semantic-release to v23.0.6 ([7c88289](https://github.com/Belphemur/AddictedProxy/commit/7c8828921a279f5431cdbbf292dc5edd34f04398))
* **deps:** update dependency semantic-release to v23.0.7 ([842318c](https://github.com/Belphemur/AddictedProxy/commit/842318ca0f2397190475c9af020c1538a2a4feab))
* **deps:** update dependency vuetify to v3.5.10 ([068448b](https://github.com/Belphemur/AddictedProxy/commit/068448bc8cb459a81ea77b6286dbc83c2e742241))
* **deps:** update dependency vuetify to v3.5.11 ([d606a84](https://github.com/Belphemur/AddictedProxy/commit/d606a84c6d566d99bcbad80da0bb4e6fdc66f0d6))
* **deps:** update dependency vuetify to v3.5.12 ([ccf3b0a](https://github.com/Belphemur/AddictedProxy/commit/ccf3b0af89051435985235b92064ca92cdb0b648))
* **deps:** update dependency vuetify to v3.5.13 ([c57a651](https://github.com/Belphemur/AddictedProxy/commit/c57a651a1740cd8ff8feb69495764e68de8480a9))
* **deps:** update dependency vuetify to v3.5.14 ([c648a53](https://github.com/Belphemur/AddictedProxy/commit/c648a5343d877bf348536797d5e758fcb4dc30d5))
* **deps:** update dependency vuetify to v3.5.9 ([df5a2a8](https://github.com/Belphemur/AddictedProxy/commit/df5a2a8fbb98847494a16fd60fce675aaf177cee))

## [4.19.18](https://github.com/Belphemur/AddictedProxy/compare/v4.19.17...v4.19.18) (2024-03-11)


### Performance improvements

* **subtitles:** Increase concurrency on subtitle fetching ([99c5cc4](https://github.com/Belphemur/AddictedProxy/commit/99c5cc4b0d917a0cc7d4dc5bbccdd3f638c2a16f))
* **subtitles:** Increase max timeout for fetching data ([fcfee74](https://github.com/Belphemur/AddictedProxy/commit/fcfee74aac0186b35402a85fa4a584c02d60c68e))


### Bug Fixes

* **deps:** update dependency vuetify to v3.5.8 ([e28f6c4](https://github.com/Belphemur/AddictedProxy/commit/e28f6c4978054def2e9111c7031bedb7c183ca09))

## [4.19.17](https://github.com/Belphemur/AddictedProxy/compare/v4.19.16...v4.19.17) (2024-03-04)


### Performance improvements

* **deps:** update deps ([bb51b7d](https://github.com/Belphemur/AddictedProxy/commit/bb51b7d496c0c7b236c5cd1c16785634a45b46cd))


### Bug Fixes

* **deps:** update dependency vuetify to v3.5.6 ([383368d](https://github.com/Belphemur/AddictedProxy/commit/383368dde97c4e9529ffbc411f52160faca3d245))
* **deps:** update dependency vuetify to v3.5.7 ([f87c1f4](https://github.com/Belphemur/AddictedProxy/commit/f87c1f4d3401702115f7983db5a94fddf1758eaa))

## [4.19.16](https://github.com/Belphemur/AddictedProxy/compare/v4.19.15...v4.19.16) (2024-02-14)


### Performance improvements

* **deps:** update deps and .net ([3c73e7b](https://github.com/Belphemur/AddictedProxy/commit/3c73e7bb65528639e79bf8c2397d59f510bdd647))

## [4.19.15](https://github.com/Belphemur/AddictedProxy/compare/v4.19.14...v4.19.15) (2024-02-09)


### Bug Fixes

* **background:** fix missing background ([ca35e29](https://github.com/Belphemur/AddictedProxy/commit/ca35e291697f6108143b848dc91b4926ca0666c8))
* **background:** fix missing background ([cc9b253](https://github.com/Belphemur/AddictedProxy/commit/cc9b253520e9ea595364e27d0495ca7ec38c70fe))

## [4.19.14](https://github.com/Belphemur/AddictedProxy/compare/v4.19.13...v4.19.14) (2024-02-09)


### Performance improvements

* **security:** use dumb-init for launching app and proper user ([62c699a](https://github.com/Belphemur/AddictedProxy/commit/62c699adf118bae775ea620c9661b9b6993b09f7))

## [4.19.13](https://github.com/Belphemur/AddictedProxy/compare/v4.19.12...v4.19.13) (2024-02-08)


### Bug Fixes

* **subtitle:** remove ordering to keep the proper numerical order of episodes ([953ec57](https://github.com/Belphemur/AddictedProxy/commit/953ec57bd4d3cd695379b86b4a70949a5b970735))

## [4.19.12](https://github.com/Belphemur/AddictedProxy/compare/v4.19.11...v4.19.12) (2024-02-08)


### Performance improvements

* improve front-end image ([cfb448f](https://github.com/Belphemur/AddictedProxy/commit/cfb448f8ac8d05dc9274fee0a65e7a383f68b635))
* **style:** improve style for <= 1080p of media page ([6f3e2ac](https://github.com/Belphemur/AddictedProxy/commit/6f3e2ac51d714e9020917996c96e092b5ffec187))
* **Subtitle:** use button to show it's clickable ([54706a4](https://github.com/Belphemur/AddictedProxy/commit/54706a4d3909bc694dfa6797374f39c2c95f34c9))
* update deps ([4670b6d](https://github.com/Belphemur/AddictedProxy/commit/4670b6d971d7687411836bbe50b9c868a67747e3))


### Bug Fixes

* client/server detection for api calls ([452a3c6](https://github.com/Belphemur/AddictedProxy/commit/452a3c671cc93221582aeed32141d2808303f61c))
* detecting server or client ([c5bfe6f](https://github.com/Belphemur/AddictedProxy/commit/c5bfe6f31da431319f9984e3fe66a7ead94e2602))
* **Subtitle:** Fix subtitle table to use grouping ([2507ab3](https://github.com/Belphemur/AddictedProxy/commit/2507ab30969676c5fae98e441a1d80df7a6a60d8))

## [4.19.11](https://github.com/Belphemur/AddictedProxy/compare/v4.19.10...v4.19.11) (2024-02-08)


### Performance improvements

* **deps:** update frontend deps ([8e516ec](https://github.com/Belphemur/AddictedProxy/commit/8e516ec87d9ecec757178ec819d9c7bf312099d0))

## [4.19.10](https://github.com/Belphemur/AddictedProxy/compare/v4.19.9...v4.19.10) (2024-01-17)


### Performance improvements

* **deps::aws:** fix aws dep S3 ([afdfaf1](https://github.com/Belphemur/AddictedProxy/commit/afdfaf1d77de8028dc8bf58101604da464864338))
* **deps:** update deps ([68d7387](https://github.com/Belphemur/AddictedProxy/commit/68d7387695077095675ad197e5b4dcd79778dace))


### Bug Fixes

* **deps:** update dependency semantic-release to v23 ([032ca3b](https://github.com/Belphemur/AddictedProxy/commit/032ca3b807594eccceb3ea7aba7203f16668284c))

## [4.19.9](https://github.com/Belphemur/AddictedProxy/compare/v4.19.8...v4.19.9) (2023-12-05)


### Performance improvements

* **deps:** update frontend deps ([d010daf](https://github.com/Belphemur/AddictedProxy/commit/d010dafe36f44d55f30d260b885ed2ec6427a4c7))

## [4.19.8](https://github.com/Belphemur/AddictedProxy/compare/v4.19.7...v4.19.8) (2023-11-30)


### Bug Fixes

* **provenance:** fix script syntax ([e138dbe](https://github.com/Belphemur/AddictedProxy/commit/e138dbec29265522b5927391c0c32d763db4884b))
* **provenance:** fix the migration script, fixing the syntax ([4b40dac](https://github.com/Belphemur/AddictedProxy/commit/4b40daca439f24b02c3caa1149803376915e9c42))

## [4.19.7](https://github.com/Belphemur/AddictedProxy/compare/v4.19.6...v4.19.7) (2023-11-30)


### Bug Fixes

* **provenance:** fix the migration script ([828f669](https://github.com/Belphemur/AddictedProxy/commit/828f669ff6f4c4b2f616004c5e1ce2ed87499705))

## [4.19.6](https://github.com/Belphemur/AddictedProxy/compare/v4.19.5...v4.19.6) (2023-11-30)


### Performance improvements

* **deps:** update frontend deps ([27792dd](https://github.com/Belphemur/AddictedProxy/commit/27792dd925352fad5b4edae5eae06f7b7ee21b4e))


### Bug Fixes

* **provenance:** don't rely on EF to update the field ([f6a4faa](https://github.com/Belphemur/AddictedProxy/commit/f6a4faaa8c936594b21adedea0a6215ad5e7bd88))
* **provenance:** use SQL trigger to provide update the updatedat field ([43c3143](https://github.com/Belphemur/AddictedProxy/commit/43c314320bb5137b314336cfb27b577e21d80fc0))

## [4.19.5](https://github.com/Belphemur/AddictedProxy/compare/v4.19.4...v4.19.5) (2023-11-21)


### Performance improvements

* **ci:** Better building of the docker image with proper version ([c7cde7f](https://github.com/Belphemur/AddictedProxy/commit/c7cde7fb0c27449060497a14b41047703b9c92e0))

## [4.19.4](https://github.com/Belphemur/AddictedProxy/compare/v4.19.3...v4.19.4) (2023-11-20)


### Bug Fixes

* **deps:** force pgsql version ([3589306](https://github.com/Belphemur/AddictedProxy/commit/35893061fe3a7af782a0d7f16fd0c14b7e60d8d7))

## [4.19.3](https://github.com/Belphemur/AddictedProxy/compare/v4.19.2...v4.19.3) (2023-11-17)


### Performance improvements

* **deps:** update deps to net 8.0 ([5bcb4b0](https://github.com/Belphemur/AddictedProxy/commit/5bcb4b056d0f0f6872f76d4f3871ef1f49def2bb))


### Bug Fixes

* **ci:** fix building image for server ([3cb828b](https://github.com/Belphemur/AddictedProxy/commit/3cb828b051ca71365630ba4dd923a18a3ae9de1c))

## [4.19.2](https://github.com/Belphemur/AddictedProxy/compare/v4.19.1...v4.19.2) (2023-11-15)


### Bug Fixes

* **ci:** fix building image for server ([395484e](https://github.com/Belphemur/AddictedProxy/commit/395484eb9c9f139320c66795654c5058dba57c70))

## [4.19.1](https://github.com/Belphemur/AddictedProxy/compare/v4.19.0...v4.19.1) (2023-11-15)


### Performance improvements

* **.NET:** update to .NET 8.0 ([f44e903](https://github.com/Belphemur/AddictedProxy/commit/f44e903cce947d12e97edb94f30d313ce309926c))
* **deps:** update frontend deps ([e554910](https://github.com/Belphemur/AddictedProxy/commit/e554910dd816ab77ec48f12f09d133b13a4f743c))
* **tvdb:id:** improve finding the tvdb id ([069b968](https://github.com/Belphemur/AddictedProxy/commit/069b968f6d769d53139bd4dad895c9f4fbdbf6fc))


### Bug Fixes

* **compressor:** fix getting dictionary file ([19d1da1](https://github.com/Belphemur/AddictedProxy/commit/19d1da17217fb9584ebeac03544f8159922e0a0d))
* **deps:** update package deps lock file ([d742649](https://github.com/Belphemur/AddictedProxy/commit/d7426491adfba478cccaa1554187aa306771683f))
* **frontend:** any issue relating to updating nuxt ([5c423f0](https://github.com/Belphemur/AddictedProxy/commit/5c423f06b7ca14f9e67de4af59438b6b5d5c050a))
* **httplogging:** fix issue with http logging not loaded ([6717250](https://github.com/Belphemur/AddictedProxy/commit/67172509b344d0a087f41e133e96f9a7e62c3720))

## [4.19.0](https://github.com/Belphemur/AddictedProxy/compare/v4.18.6...v4.19.0) (2023-11-08)


### Features

* **tvshow:** Find missing TVDBIDs after refreshing shows ([8c56d0f](https://github.com/Belphemur/AddictedProxy/commit/8c56d0f736653ebb89b5c5b84aaf1ef01ddc22a5))

## [4.18.6](https://github.com/Belphemur/AddictedProxy/compare/v4.18.5...v4.18.6) (2023-10-12)


### Performance improvements

* deps update ([fd486c2](https://github.com/Belphemur/AddictedProxy/commit/fd486c2c0b6cda8b5004a15e6358f714b34ea1bb))
* **download:** Use SQL to increment the download count of a subtitle ([cf96800](https://github.com/Belphemur/AddictedProxy/commit/cf96800190b513edf846abb362a00114d7beaf55))


### Bug Fixes

* **deps:** update dotnet monorepo to v7.0.12 ([df23422](https://github.com/Belphemur/AddictedProxy/commit/df23422a9ffe14e55e09c604d0f4fbaf01bc544e))
* **download:** fix wrong sql query to update download count ([4fe75da](https://github.com/Belphemur/AddictedProxy/commit/4fe75da693ce88444133a44e7938b36c6f34a019))

## [4.18.5](https://github.com/Belphemur/AddictedProxy/compare/v4.18.4...v4.18.5) (2023-09-26)


### Bug Fixes

* don't remove curl ([47ed534](https://github.com/Belphemur/AddictedProxy/commit/47ed534430ea486eab5327744c9ea9ff41901da7))

## [4.18.4](https://github.com/Belphemur/AddictedProxy/compare/v4.18.3...v4.18.4) (2023-09-26)


### Performance improvements

* improve seo of index page ([dc8b6c2](https://github.com/Belphemur/AddictedProxy/commit/dc8b6c23041c4bc3cfb6bfc74759779ebc9db2e8))
* improve the seo data ([3251d5b](https://github.com/Belphemur/AddictedProxy/commit/3251d5b9cf372f0b5da02050695e4332fe4a18fc))
* **subtitles:** keep hearing impaired at the bottom ([43161d2](https://github.com/Belphemur/AddictedProxy/commit/43161d2894c6558e2a0b5e12ce3cf14b1fcb63d2))


### Bug Fixes

* subtitle table to display properly the available episodes ([a411686](https://github.com/Belphemur/AddictedProxy/commit/a4116860210ea4f1e5a322b894ad9aed3dc72c04))
* url of images ([7a746a2](https://github.com/Belphemur/AddictedProxy/commit/7a746a2e8b84a45d73aac195d216b451db44e0b8))

## [4.18.3](https://github.com/Belphemur/AddictedProxy/compare/v4.18.2...v4.18.3) (2023-09-26)


### Bug Fixes

* building app for zstd ([6dfd8a4](https://github.com/Belphemur/AddictedProxy/commit/6dfd8a4c1a6ba9398901bf3b108ed4a11a87c117))

## [4.18.2](https://github.com/Belphemur/AddictedProxy/compare/v4.18.1...v4.18.2) (2023-09-26)


### Bug Fixes

* release ([4af2367](https://github.com/Belphemur/AddictedProxy/commit/4af23670dc7a8c2e64dd1d217b1266ad8fbc1e45))

## [4.18.0](https://github.com/Belphemur/AddictedProxy/compare/v4.17.15...v4.18.0) (2023-09-26)


### Performance improvements

* add created/updated to all models ([5c7a577](https://github.com/Belphemur/AddictedProxy/commit/5c7a57747aef1b7ada0f3039d634b146f415e6b6))
* add retry to download file from S3 ([c84f66b](https://github.com/Belphemur/AddictedProxy/commit/c84f66bc8e2f1a29d53ef85f89930f1dbc2797bd))
* **media:** update the seo metadata of the media view ([8ced6ba](https://github.com/Belphemur/AddictedProxy/commit/8ced6ba978c5000af9e27a1bc558b0e1d641c0fa))
* onetime migration remove ran at ([e7627db](https://github.com/Belphemur/AddictedProxy/commit/e7627db6c6a75a3e7df8de27615cb7ca81d756e3))
* update deps ([5a4ced4](https://github.com/Belphemur/AddictedProxy/commit/5a4ced42e9729b216dcb3f027d7409b0f8994060))


### Bug Fixes

* **deps:** update dependency semantic-release to v22 ([31c583d](https://github.com/Belphemur/AddictedProxy/commit/31c583d6f134765bef1970402464fbe0ee6380af))
* **image:** be sure the image always cover the full container on XS ([329e3ec](https://github.com/Belphemur/AddictedProxy/commit/329e3ec154ace64609af3786340b103546dd18b0))
* **image:** be sure the image always cover the full container on XS ([f73e17e](https://github.com/Belphemur/AddictedProxy/commit/f73e17e5bf6ed4fa9e6fd937c039412159550bbf))
* missing migration date ([dd24eb9](https://github.com/Belphemur/AddictedProxy/commit/dd24eb965470350b0ab5b4f9ce31ea08f460508c))
* updated at and created at for subtitles and episodes ([1f6a9d9](https://github.com/Belphemur/AddictedProxy/commit/1f6a9d96a41105fb0ee7a99c81c36f25f6dbda77))


### Features

* add support for auto created/updated fields ([9054721](https://github.com/Belphemur/AddictedProxy/commit/90547212741391b2252f1de2fac3057f983b7de7))
* add updated at and created at for all models and populate the data ([c2b188d](https://github.com/Belphemur/AddictedProxy/commit/c2b188dc634487dbf1f912a137a6a47ebbfd405c))

## [4.17.15](https://github.com/Belphemur/AddictedProxy/compare/v4.17.14...v4.17.15) (2023-09-14)


### Performance improvements

* **image:** image size of the trending page ([d3e0e69](https://github.com/Belphemur/AddictedProxy/commit/d3e0e69b5aab1ef083b47c318cfa81d33b0b967a))
* **MediaView:** Rework layout for mobile ([4ad8d1f](https://github.com/Belphemur/AddictedProxy/commit/4ad8d1f6388f1e51918eb993691e60d79b0f913a))

## [4.17.14](https://github.com/Belphemur/AddictedProxy/compare/v4.17.13...v4.17.14) (2023-09-14)


### Bug Fixes

* **image:** current the backend don't support AVIF ([340ce31](https://github.com/Belphemur/AddictedProxy/commit/340ce31b2f5559b00bc43455045db901111c0aed))

## [4.17.13](https://github.com/Belphemur/AddictedProxy/compare/v4.17.12...v4.17.13) (2023-09-14)


### Bug Fixes

* wrong version of nuxt/image ([e1ee97b](https://github.com/Belphemur/AddictedProxy/commit/e1ee97b4a8855c2b2b2959ad918c441cdf6852db))

## [4.17.12](https://github.com/Belphemur/AddictedProxy/compare/v4.17.11...v4.17.12) (2023-09-14)


### Performance improvements

* update deps ([d3cc526](https://github.com/Belphemur/AddictedProxy/commit/d3cc526fdf44e410c666849bb0da743cb798fdf9))

## [4.17.11](https://github.com/Belphemur/AddictedProxy/compare/v4.17.10...v4.17.11) (2023-09-02)


### Performance improvements

* improve speed of compressor factory ([a6693cd](https://github.com/Belphemur/AddictedProxy/commit/a6693cdb8202f1b38f58d44fc53ce4a816c0a490))

## [4.17.10](https://github.com/Belphemur/AddictedProxy/compare/v4.17.9...v4.17.10) (2023-09-01)


### Performance improvements

* detect OS ([35e9ca9](https://github.com/Belphemur/AddictedProxy/commit/35e9ca9ff0365d96019f98688124f3c8bf3fd867))
* optimize the detection of compression algo ([ed8776d](https://github.com/Belphemur/AddictedProxy/commit/ed8776da60ffbbd7dca96c90b0342178d20da7d4))

## [4.17.9](https://github.com/Belphemur/AddictedProxy/compare/v4.17.8...v4.17.9) (2023-09-01)


### Performance improvements

* ci use debian slim ([f7eb4ef](https://github.com/Belphemur/AddictedProxy/commit/f7eb4ef0acc43e935a26cef2cb6fb6552008560e))
* **ci:** change build process for nuxt app ([88887c1](https://github.com/Belphemur/AddictedProxy/commit/88887c1f55002dd4baa50930dd209419c9f08f08))
* **ci:** fix not building front-end ([a6374c9](https://github.com/Belphemur/AddictedProxy/commit/a6374c90a092d52799cf667ab041878ae17ced2c))
* **ci:** fix working directory ([edf0970](https://github.com/Belphemur/AddictedProxy/commit/edf0970fcd0821121822dd9c312d3916f33f692c))
* **ci:** use concurrency of github action ([c730173](https://github.com/Belphemur/AddictedProxy/commit/c730173a633fa04d0c26d2c4468cd8212444e48e))
* force release ([029f13c](https://github.com/Belphemur/AddictedProxy/commit/029f13c580ac5c0129a251b0ccb0d74c2f4c8d07))
* start with magic number that are longer to avoid conflict with other algorithmn ([f990a4b](https://github.com/Belphemur/AddictedProxy/commit/f990a4b63325146f21e6d6bdfa5ae5df547c702e))
* update deps ([c95988b](https://github.com/Belphemur/AddictedProxy/commit/c95988bda136cbfa69ce9beea7158b4b704f584e))


### Bug Fixes

* add missing package ([a52ad21](https://github.com/Belphemur/AddictedProxy/commit/a52ad210ae44e433589f296fdebf971dd14f59b4))
* ci recursive ([5104c1a](https://github.com/Belphemur/AddictedProxy/commit/5104c1a99f3eaa74cbb9df9e0093b2a16f9f2d9e))
* **ci:** fix building ([60a84c8](https://github.com/Belphemur/AddictedProxy/commit/60a84c80fb5beadf0fd5c82f7701bc534dec7d6e))
* **ci:** fix the building frontend ([aceb6c3](https://github.com/Belphemur/AddictedProxy/commit/aceb6c3b67c398470e44061c93225ac1e1cbaec6))
* **ci:** lock file missing ([190b3e2](https://github.com/Belphemur/AddictedProxy/commit/190b3e21c8327c9cba1910a716bdf051f3c854be))
* commit analyzer, currently semantic release isn't compatible with version 7.0 ([be0f8ad](https://github.com/Belphemur/AddictedProxy/commit/be0f8ada25b14a30f6d3d8804e817cf5232b5c80))
* **ipx:** add ipx as dependency ([9cee91e](https://github.com/Belphemur/AddictedProxy/commit/9cee91e219102782ab14520c3280cec1641f139b))
* **ipx:** fix building ipx ([23f427c](https://github.com/Belphemur/AddictedProxy/commit/23f427c6859d978414e98c0408ff9099e94f80ec))
* **ipx:** fix deps for ipx ([8888687](https://github.com/Belphemur/AddictedProxy/commit/8888687238a73e044a8c1950efa6a4bd702d6143))
* **ipx:** fix deps for ipx (again) ([e123684](https://github.com/Belphemur/AddictedProxy/commit/e123684c80ef847e30ac6061f4054a9fc3ac1221))
* **ipx:** fix deps for ipx (again) and again ([0069370](https://github.com/Belphemur/AddictedProxy/commit/0069370a39fd821848e4ee6e283a36d0baa7a476))

## [4.17.8](https://github.com/Belphemur/AddictedProxy/compare/v4.17.7...v4.17.8) (2023-08-22)


### Performance improvements

* **compressor:** Add ZSTD compressor trained on subtitles ([ea2e3a5](https://github.com/Belphemur/AddictedProxy/commit/ea2e3a5f2f2f44aa9d639dd03b8b15e320f0c090))
* **compressor:** change default compressor to be the zst srt dict one ([ff74f8f](https://github.com/Belphemur/AddictedProxy/commit/ff74f8f93c72dfa64aeed1d19277db46b97aafbc))
* **zstd:** use latest version of the lib ([188a8dd](https://github.com/Belphemur/AddictedProxy/commit/188a8dd475b63fb25c669c2c2c69c8846d669ffc))

## [4.17.7](https://github.com/Belphemur/AddictedProxy/compare/v4.17.6...v4.17.7) (2023-08-21)


### Bug Fixes

* **brotli:** fix issue with brotli compression ([b1645cb](https://github.com/Belphemur/AddictedProxy/commit/b1645cb7c7da1a94fe288a7a0fff0193c538d193))

## [4.17.6](https://github.com/Belphemur/AddictedProxy/compare/v4.17.5...v4.17.6) (2023-08-21)


### Performance improvements

* dockerfile improvement for the zstd lib ([6f312b1](https://github.com/Belphemur/AddictedProxy/commit/6f312b14c6fd5d0e63861db3916e58ae72469686))


### Bug Fixes

* **compression:** fix issue with decompression brotli file ([432c85d](https://github.com/Belphemur/AddictedProxy/commit/432c85dcc58db5512caf34da45bfd6902daeba91)), closes [#501](https://github.com/Belphemur/AddictedProxy/issues/501)

## [4.17.5](https://github.com/Belphemur/AddictedProxy/compare/v4.17.4...v4.17.5) (2023-08-21)


### Bug Fixes

* put the lib where .NET expect it ([73f6767](https://github.com/Belphemur/AddictedProxy/commit/73f676792887efabc060f4c0decc85430e0216c9))

## [4.17.4](https://github.com/Belphemur/AddictedProxy/compare/v4.17.3...v4.17.4) (2023-08-21)


### Bug Fixes

* libzstd missing ([919f1f1](https://github.com/Belphemur/AddictedProxy/commit/919f1f148dbb907d3541be8ff531776f6c1e5b4b))

## [4.17.3](https://github.com/Belphemur/AddictedProxy/compare/v4.17.2...v4.17.3) (2023-08-21)


### Bug Fixes

* **compressor:** fix issue with ZSTD lib ([7ed5d65](https://github.com/Belphemur/AddictedProxy/commit/7ed5d6597437e71c7861a70d620621a78512f378))

## [4.17.2](https://github.com/Belphemur/AddictedProxy/compare/v4.17.1...v4.17.2) (2023-08-21)


### Performance improvements

* **pyroscope:** remove pyroscope ([b2efc81](https://github.com/Belphemur/AddictedProxy/commit/b2efc818e9f1a841b113319d660488a712d0f120))
* **zstd:** add lib to base image ([5f3f7f8](https://github.com/Belphemur/AddictedProxy/commit/5f3f7f8730e204ca224a75c7053067f4e761c48a))
* **zstd:** Replace package to use the wrapper to the lib ([77a69fc](https://github.com/Belphemur/AddictedProxy/commit/77a69fc3c68ff603c51fdd8858961515649a7058))

## [4.17.1](https://github.com/Belphemur/AddictedProxy/compare/v4.17.0...v4.17.1) (2023-08-21)


### Performance improvements

* **compressor:** small optimization to avoid memory allocation if not needed ([14cffdc](https://github.com/Belphemur/AddictedProxy/commit/14cffdc826fcef30b997827858a98070db7d6cf0))

## [4.17.0](https://github.com/Belphemur/AddictedProxy/compare/v4.16.6...v4.17.0) (2023-08-19)


### Performance improvements

* **compressor:** remove unused method ([6755e96](https://github.com/Belphemur/AddictedProxy/commit/6755e961ad4207be899f9a495ac1ac9a39571b9b))
* improve factory contract to check for existence of service ([995a905](https://github.com/Belphemur/AddictedProxy/commit/995a9057ea91f9840d0d5d62c4e8e7e9a3096981))


### Bug Fixes

* **compressor:** continue to support brotli without signature ([2fc269c](https://github.com/Belphemur/AddictedProxy/commit/2fc269cf8309d4433990aafca2d39831c38a7726))
* **download:** fix not finding the subtitle file when using old file format ([0866512](https://github.com/Belphemur/AddictedProxy/commit/0866512cb239ab7b7ce2a91b293bde122da3073e))
* **factory:** Fix issue registering the factory ([3d426ff](https://github.com/Belphemur/AddictedProxy/commit/3d426ffcaa6077f8a865215a05e4311c895638e7))


### Features

* **compressor:** add zstd as compressor and make it the default ([18d99ad](https://github.com/Belphemur/AddictedProxy/commit/18d99adb546b0f2e42a335935dad8da189e8ab2c))
* **factory:** Add factory pattern to the IoC ([8d9f2b9](https://github.com/Belphemur/AddictedProxy/commit/8d9f2b9aad7ee9e1b9ea65b776d78443110004a3))
* Make factory able to use Enum ([605c35b](https://github.com/Belphemur/AddictedProxy/commit/605c35b82cf95745fa7a0a17cdd0dc7cfbdf1bc4))
* multiple compressor support ([e0fc568](https://github.com/Belphemur/AddictedProxy/commit/e0fc56859355f6a58bb4c556ae4fc2ac89b9b0b0))

## [4.16.6](https://github.com/Belphemur/AddictedProxy/compare/v4.16.5...v4.16.6) (2023-08-17)


### Performance improvements

* **fetch:** Increase concurrence of the fetch job ([9061a85](https://github.com/Belphemur/AddictedProxy/commit/9061a852eb64fa06b75a6767e3851beee03f48a2))

## [4.16.5](https://github.com/Belphemur/AddictedProxy/compare/v4.16.4...v4.16.5) (2023-08-11)


### Performance improvements

* **cache:** be sure to keep image in cache for 90 days in browser and cdn ([b32853d](https://github.com/Belphemur/AddictedProxy/commit/b32853d10b0e8809fff1788781b38802d098fcaf))

## [4.16.4](https://github.com/Belphemur/AddictedProxy/compare/v4.16.3...v4.16.4) (2023-08-10)


### Performance improvements

* **image:** cache metadata in memory ([e678e0d](https://github.com/Belphemur/AddictedProxy/commit/e678e0d78fc0211a3a34ce252ad586b513a3143f))
* **Media::Details:** Improve the loading time of media page ([309aceb](https://github.com/Belphemur/AddictedProxy/commit/309aceb35f0a52d8b97ed595e080fa09f3595966))

## [4.16.3](https://github.com/Belphemur/AddictedProxy/compare/v4.16.2...v4.16.3) (2023-08-07)


### Performance improvements

* **background:** reduce size of the background ([693bc27](https://github.com/Belphemur/AddictedProxy/commit/693bc27831ff0e607364e92b2ab09a64dff9295b))
* **image:** optimize the size of the images ([41a1a21](https://github.com/Belphemur/AddictedProxy/commit/41a1a211d2c666404900b1db03d3edd427c2ffec))

## [4.16.2](https://github.com/Belphemur/AddictedProxy/compare/v4.16.1...v4.16.2) (2023-08-07)


### Bug Fixes

* **search:** remove direct matching of Show name ([94ab49e](https://github.com/Belphemur/AddictedProxy/commit/94ab49eac67fd533c0a157253537b0992b2dc132))

## [4.16.1](https://github.com/Belphemur/AddictedProxy/compare/v4.16.0...v4.16.1) (2023-08-06)


### Performance improvements

* **faro:** remove faro ([fc6930e](https://github.com/Belphemur/AddictedProxy/commit/fc6930e6de5c74b18869ee9ecd342cbdb6aad3a1))

## [4.16.0](https://github.com/Belphemur/AddictedProxy/compare/v4.15.8...v4.16.0) (2023-08-03)


### Bug Fixes

* **media:** fix image path ([cd19d4d](https://github.com/Belphemur/AddictedProxy/commit/cd19d4dfe46f9ffe70f9ef667162d3764231e077))


### Features

* **front:** use own image provider for tmdb images ([243e310](https://github.com/Belphemur/AddictedProxy/commit/243e310c7058abb6f109f9705cda7c97bbd2b361))
* **image:** add way to dynamically resize images and store a cache ([35355d4](https://github.com/Belphemur/AddictedProxy/commit/35355d46f3160555980d54c567173796f6dd0d16))
* **image:** be sure to return the path of the image to use the API ([3d308af](https://github.com/Belphemur/AddictedProxy/commit/3d308af59523cbc97615844a2307b98c4d465be1))
* **tmdb:image:** Add way to get image from tmdb ([161fea2](https://github.com/Belphemur/AddictedProxy/commit/161fea29255f60ef145eaf1f43752c72a3789cb3))

## [4.15.8](https://github.com/Belphemur/AddictedProxy/compare/v4.15.7...v4.15.8) (2023-07-31)


### Performance improvements

* **deps:** update deps ([0f1ecd7](https://github.com/Belphemur/AddictedProxy/commit/0f1ecd707eb96c7f5b62d994c95df7bbe3001f0f))
* **deps:** update frontend deps ([b272f1e](https://github.com/Belphemur/AddictedProxy/commit/b272f1edfe6ad08ecb752038500b1be512f1d481))

## [4.15.7](https://github.com/Belphemur/AddictedProxy/compare/v4.15.6...v4.15.7) (2023-07-22)


### Performance improvements

* **faro:** Add Faro SDK to check performance ([fe23997](https://github.com/Belphemur/AddictedProxy/commit/fe23997dfb7a6a39b1e727eae82018e41c1e5208))

## [4.15.6](https://github.com/Belphemur/AddictedProxy/compare/v4.15.5...v4.15.6) (2023-07-21)


### Performance improvements

* **database:** put every db tools together ([7fab487](https://github.com/Belphemur/AddictedProxy/commit/7fab48754d5b5b9371f97c6d734a0f4730c8b70e))

## [4.15.5](https://github.com/Belphemur/AddictedProxy/compare/v4.15.4...v4.15.5) (2023-07-19)


### Performance improvements

* **caching:** use LRU Cache for Ipx ([f55fb67](https://github.com/Belphemur/AddictedProxy/commit/f55fb67f5232cfaf9cd9c437870f4809de53032b))
* **caching:** use own middleware to control ipx lib ([1aaa70e](https://github.com/Belphemur/AddictedProxy/commit/1aaa70ea6e135437a31fc21113921a7e451a442e))

## [4.15.4](https://github.com/Belphemur/AddictedProxy/compare/v4.15.3...v4.15.4) (2023-07-16)


### Performance improvements

* **caching:** force cache ipx fork ([c749c78](https://github.com/Belphemur/AddictedProxy/commit/c749c78e53c2936c42e4506dd7e14924323f7924))

## [4.15.3](https://github.com/Belphemur/AddictedProxy/compare/v4.15.2...v4.15.3) (2023-07-15)


### Performance improvements

* **details:** Cache show details for 14 days ([fce7ea5](https://github.com/Belphemur/AddictedProxy/commit/fce7ea5215b67de9317689c4efad088fa15fd750))

## [4.15.2](https://github.com/Belphemur/AddictedProxy/compare/v4.15.1...v4.15.2) (2023-07-14)


### Performance improvements

* **ssr:** use proper ssr method to do http calls ([44917ae](https://github.com/Belphemur/AddictedProxy/commit/44917ae9175070c185c62b6e9bcbee121299ec02))

## [4.15.1](https://github.com/Belphemur/AddictedProxy/compare/v4.15.0...v4.15.1) (2023-07-14)


### Bug Fixes

* **docker:** fix building offline ([13eac4d](https://github.com/Belphemur/AddictedProxy/commit/13eac4dca12efca11a7794388cdf7b8d3535051c))

## [4.15.0](https://github.com/Belphemur/AddictedProxy/compare/v4.14.19...v4.15.0) (2023-07-14)


### Performance improvements

* **nuxt-image:** use nuxt image with caching ([03d29fd](https://github.com/Belphemur/AddictedProxy/commit/03d29fdaf3ddd648c6024bbe29f491026065cf3b))


### Features

* **nuxt-image:** add back nuxt-image ([f285e44](https://github.com/Belphemur/AddictedProxy/commit/f285e441d5bbd1aa049fd48079cbf8e906f07324))

## [4.14.19](https://github.com/Belphemur/AddictedProxy/compare/v4.14.18...v4.14.19) (2023-07-13)


### Performance improvements

* **search:** remove uneeded component ([33ee236](https://github.com/Belphemur/AddictedProxy/commit/33ee236068263dbac87495a6a24151471de77997))
* **sentry:** remove sentry ([cf9d9d9](https://github.com/Belphemur/AddictedProxy/commit/cf9d9d97931e7ddd99fc6bb923009c66d6b08de6))

## [4.14.18](https://github.com/Belphemur/AddictedProxy/compare/v4.14.17...v4.14.18) (2023-07-13)


### Performance improvements

* **sentry:** get properly the router ([0030570](https://github.com/Belphemur/AddictedProxy/commit/0030570e17479bef6733e111a48ae02e84749c94))

## [4.14.17](https://github.com/Belphemur/AddictedProxy/compare/v4.14.16...v4.14.17) (2023-07-13)


### Performance improvements

* **docs:** Move documentation to readme.com ([516c589](https://github.com/Belphemur/AddictedProxy/commit/516c5898443101cbb1a523e62f1c5c9d615e3f4e))

## [4.14.16](https://github.com/Belphemur/AddictedProxy/compare/v4.14.15...v4.14.16) (2023-07-13)


### Bug Fixes

* **image:** remove nuxt/image ([6a67853](https://github.com/Belphemur/AddictedProxy/commit/6a678531e24883cf1d2cfac4bcb9f8665f28880c))

## [4.14.15](https://github.com/Belphemur/AddictedProxy/compare/v4.14.14...v4.14.15) (2023-07-13)


### Bug Fixes

* **sentry:** disable browser tracing ([12e7202](https://github.com/Belphemur/AddictedProxy/commit/12e72022c3a8e9a51ad0a9d1a1c591c049d51921))
* **sitemap:** no need for api gen for this controller ([7fdfe65](https://github.com/Belphemur/AddictedProxy/commit/7fdfe6501973c20ac2e0f31e004463708208d5be))

## [4.14.14](https://github.com/Belphemur/AddictedProxy/compare/v4.14.13...v4.14.14) (2023-07-08)


### Bug Fixes

* **image:** use lazy-src to be sure we have some space taken for image to come ([d658611](https://github.com/Belphemur/AddictedProxy/commit/d6586116b7e4cfae002d16e2fcda2bc8fcbffb15))

## [4.14.13](https://github.com/Belphemur/AddictedProxy/compare/v4.14.12...v4.14.13) (2023-07-08)


### Performance improvements

* **images:** optimize properly the images ([e78d56d](https://github.com/Belphemur/AddictedProxy/commit/e78d56d7126cbd381d7c8bd04ec244726770adfb))

## [4.14.12](https://github.com/Belphemur/AddictedProxy/compare/v4.14.11...v4.14.12) (2023-07-08)


### Performance improvements

* **UUID:** Use UUIDv7 for Postgres ([75f37ec](https://github.com/Belphemur/AddictedProxy/commit/75f37ec21041277dc69f5524eb13f4642d932844))

## [4.14.11](https://github.com/Belphemur/AddictedProxy/compare/v4.14.10...v4.14.11) (2023-07-08)


### Performance improvements

* **subtitle:** error message when no subtitles available in given language ([23b898e](https://github.com/Belphemur/AddictedProxy/commit/23b898e999eec3cfa3f47fd9c9c4331da292e488))
* **subtitles:** make language clearable ([7815f04](https://github.com/Belphemur/AddictedProxy/commit/7815f0408bd7164fde40193a66697748bf0b7823))
* **subtitles:** only show episode with subtitles ([33bae8c](https://github.com/Belphemur/AddictedProxy/commit/33bae8ca6ac63d22b66098a4dee65a85ed0d010b))

## [4.14.10](https://github.com/Belphemur/AddictedProxy/compare/v4.14.9...v4.14.10) (2023-07-08)


### Bug Fixes

* **display:** fix display issue on smaller resolutions ([d4ff6fe](https://github.com/Belphemur/AddictedProxy/commit/d4ff6fec60eef1a7df95e86f81ea0e70a1cbfcde))

## [4.14.9](https://github.com/Belphemur/AddictedProxy/compare/v4.14.8...v4.14.9) (2023-07-07)


### Performance improvements

* **trending:** improve trending performance by caching genre list ([51bc218](https://github.com/Belphemur/AddictedProxy/commit/51bc218bf976d81580889c74c8f49f54c0df1e39))

## [4.14.8](https://github.com/Belphemur/AddictedProxy/compare/v4.14.7...v4.14.8) (2023-07-06)


### Bug Fixes

* **build:** add sourcemap files ([8e0ff43](https://github.com/Belphemur/AddictedProxy/commit/8e0ff43fe2f2ba7b97a12aecc662b3460cff0090))
* **build:** fix sending the right project to docker for building ([b21dd46](https://github.com/Belphemur/AddictedProxy/commit/b21dd46f4f6c562e33c5f4872611d1b5917973d5))
* **css:** clean up css ([1283a46](https://github.com/Belphemur/AddictedProxy/commit/1283a46e8a85ad91ab015c1b188f05f2f5293cb9))

## [4.14.7](https://github.com/Belphemur/AddictedProxy/compare/v4.14.6...v4.14.7) (2023-07-06)


### Bug Fixes

* **front:** fix building ([e381b5a](https://github.com/Belphemur/AddictedProxy/commit/e381b5a85158fe6fa44c4d06d09b5ba5454609a9))

## [4.14.6](https://github.com/Belphemur/AddictedProxy/compare/v4.14.5...v4.14.6) (2023-07-06)


### Performance improvements

* **sentry:** add support for sentry ([270cd69](https://github.com/Belphemur/AddictedProxy/commit/270cd69e4396e750ccf730338ea5e9ab9187693b))

## [4.14.5](https://github.com/Belphemur/AddictedProxy/compare/v4.14.4...v4.14.5) (2023-07-06)


### Performance improvements

* **search:** put searching at the top of index page ([f8b61d4](https://github.com/Belphemur/AddictedProxy/commit/f8b61d4ffb6ade3450854ab6f46fb42849b578b3))

## [4.14.4](https://github.com/Belphemur/AddictedProxy/compare/v4.14.3...v4.14.4) (2023-07-06)


### Performance improvements

* **caching:** add caching to trending shows from tmdb ([dd6afd8](https://github.com/Belphemur/AddictedProxy/commit/dd6afd8ac1499f052b11471715416d4f3c0ea7a6))
* **index:** clean up index page to only have coded that is in use ([b82a59f](https://github.com/Belphemur/AddictedProxy/commit/b82a59f34f2c71dd3691e8d2dc4a0a7aa63c1930))
* **trending:** Improve the trending page to be rendered on the server ([de95dd4](https://github.com/Belphemur/AddictedProxy/commit/de95dd4afd13f74822af1a81acf935f9e79fc358))

## [4.14.3](https://github.com/Belphemur/AddictedProxy/compare/v4.14.2...v4.14.3) (2023-07-06)


### Performance improvements

* **trending:** return shows in order of trending ([7d8f6a0](https://github.com/Belphemur/AddictedProxy/commit/7d8f6a02d6c976b69451babf144a937861525625))


### Bug Fixes

* **api:** Fix url issue ([ee1b233](https://github.com/Belphemur/AddictedProxy/commit/ee1b233446cc051f5c7a34074f010125dd575dd4))

## [4.14.2](https://github.com/Belphemur/AddictedProxy/compare/v4.14.1...v4.14.2) (2023-07-06)


### Performance improvements

* **trending:** increase amount of trending shows ([c7f1a15](https://github.com/Belphemur/AddictedProxy/commit/c7f1a158119df44ffb7f3f82268b0307fbfeaf63))

## [4.14.1](https://github.com/Belphemur/AddictedProxy/compare/v4.14.0...v4.14.1) (2023-07-06)


### Performance improvements

* **connection:** split connection settings between server and client ([f1da7a7](https://github.com/Belphemur/AddictedProxy/commit/f1da7a725800d209bda9dd6ecf876d0edaeab8bf))
* **log:** log when can't find details of media ([4abc4b8](https://github.com/Belphemur/AddictedProxy/commit/4abc4b800ee4d0bda2e55c77f8d56ac6a6a57294))
* **media:view:** Only show details if we have them. ([b3f3129](https://github.com/Belphemur/AddictedProxy/commit/b3f3129a60b26276c77f92efd8d23bbd72c13ad9))

## [4.14.0](https://github.com/Belphemur/AddictedProxy/compare/v4.13.1...v4.14.0) (2023-07-06)


### Features

* **refresh:** fetch episodes data and show data after refreshing through websocket to get latest data ([bc4667b](https://github.com/Belphemur/AddictedProxy/commit/bc4667bd8839498ca9993b885c83ca80b95aca0d))

## [4.13.1](https://github.com/Belphemur/AddictedProxy/compare/v4.13.0...v4.13.1) (2023-07-06)


### Performance improvements

* **refresh:** refresh first latest seasons ([4970375](https://github.com/Belphemur/AddictedProxy/commit/4970375d205d9611d684dd40b18fea6296e687b6))


### Bug Fixes

* **media:view:** fix loading episode ([8de772e](https://github.com/Belphemur/AddictedProxy/commit/8de772ebe03074e8930a7acb94b6945f9a951814))

## [4.13.0](https://github.com/Belphemur/AddictedProxy/compare/v4.12.2...v4.13.0) (2023-07-06)


### Performance improvements

* **api:** Be sure that proper fields are nullable instead of all of them ([b71cf8d](https://github.com/Belphemur/AddictedProxy/commit/b71cf8d8dd84f1247bfef5d732820ff5de8c0a0e))


### Bug Fixes

* **date:** fix issue with date not provided for media from TMDB ([0479a02](https://github.com/Belphemur/AddictedProxy/commit/0479a02eceb09297c454f76d99a80d90b66db0db)), closes [#461](https://github.com/Belphemur/AddictedProxy/issues/461)
* **media:detail:** fix not finding details ([42ce380](https://github.com/Belphemur/AddictedProxy/commit/42ce38015527edefaaa504a23d2dec83fe1690d3)), closes [#462](https://github.com/Belphemur/AddictedProxy/issues/462)


### Features

* **api:** update api to be splitted ([ae74900](https://github.com/Belphemur/AddictedProxy/commit/ae749001bf2dddd91d29144d383c6a9424b30185))

## [4.12.2](https://github.com/Belphemur/AddictedProxy/compare/v4.12.1...v4.12.2) (2023-07-06)


### Bug Fixes

* **media:view:** fix size of card ([09dc580](https://github.com/Belphemur/AddictedProxy/commit/09dc580b4441b238dde93e7f03522f508fea058b))

## [4.12.1](https://github.com/Belphemur/AddictedProxy/compare/v4.12.0...v4.12.1) (2023-07-06)


### Bug Fixes

* **media:view:** be sure the size is consistent ([3522693](https://github.com/Belphemur/AddictedProxy/commit/3522693078f063f2bc4a0fe5ebcdc98b0aac85dc))

## [4.12.0](https://github.com/Belphemur/AddictedProxy/compare/v4.11.0...v4.12.0) (2023-07-06)


### Performance improvements

* Improve the user experience of media detail ([5e313c8](https://github.com/Belphemur/AddictedProxy/commit/5e313c8750ac5353ef0ef2ff30f2004d23e17603))


### Bug Fixes

* documentation ([12be099](https://github.com/Belphemur/AddictedProxy/commit/12be09944d6972417f30153230af108e2eeb180a))
* **genre:** fix getting genres ([5b4fbc3](https://github.com/Belphemur/AddictedProxy/commit/5b4fbc36fedc5c97b5bad068824ad17a635dbf26))
* **show:** Be sure the season are always ordered ([1b0d8df](https://github.com/Belphemur/AddictedProxy/commit/1b0d8dfa35c711b190a1cd1306d77fc657d7d21c))


### Features

* **index:** make index sent user to media info page ([551ebb6](https://github.com/Belphemur/AddictedProxy/commit/551ebb6c08cc9454a3e12ff84dbc55cd6b48f5a5))
* **media:date:** add air date to media ([6158ca6](https://github.com/Belphemur/AddictedProxy/commit/6158ca6bf03a6a5e0874fc7bb554d83f64f5b992))
* **Media:Detail:** Provide more date to the media ([c6f0c92](https://github.com/Belphemur/AddictedProxy/commit/c6f0c92ee36c131ab37269c0819048c04e110acf))
* **media:name:** add tmdb name to media calls ([f61e325](https://github.com/Belphemur/AddictedProxy/commit/f61e325e7425ad1402afba0c7b47894856c72c7d))
* **media:trending:** add trending to home page ([799e6b1](https://github.com/Belphemur/AddictedProxy/commit/799e6b1959e26b06b4581ad3ab27e7f23baa5267))
* **media:view:** Be sure it has all refresh feature of the index view ([92c9690](https://github.com/Belphemur/AddictedProxy/commit/92c9690de6b6e49732128d577a6333c4e7b16412))
* **tmdb:** add genre list ([6140a97](https://github.com/Belphemur/AddictedProxy/commit/6140a978b79a448ec730667fa01361d7094dc2dc))
* **tmdb:** get trending tv shows ([f8faeb6](https://github.com/Belphemur/AddictedProxy/commit/f8faeb659dc7a1080feb4440d6aa4054de5053bd))
* **trending:** add trending shows to api ([653e654](https://github.com/Belphemur/AddictedProxy/commit/653e654f8adf85d7d60c896452f6aa2eb42698a5))

## [4.11.0](https://github.com/Belphemur/AddictedProxy/compare/v4.10.1...v4.11.0) (2023-07-05)


### Bug Fixes

* **sitemap:** be sure to define the argument from route ([f4d1254](https://github.com/Belphemur/AddictedProxy/commit/f4d125460f39e0461f6f0572b8b4dd9016cfd4b0))


### Features

* **show:detail:** add detail link to search results ([e7c7556](https://github.com/Belphemur/AddictedProxy/commit/e7c75562df49e55900a2d6b47f133368ed51d9ed))

## [4.10.1](https://github.com/Belphemur/AddictedProxy/compare/v4.10.0...v4.10.1) (2023-07-05)


### Bug Fixes

* **sitemap:** fix route for swagger ([302626d](https://github.com/Belphemur/AddictedProxy/commit/302626db32b45fa77b9f90531770de77cad25231))

## [4.10.0](https://github.com/Belphemur/AddictedProxy/compare/v4.9.4...v4.10.0) (2023-07-05)


### Bug Fixes

* **sitemap:** proper change frequency depending if completed ([0279b1a](https://github.com/Belphemur/AddictedProxy/commit/0279b1aaacd93c9803a2652c81a9222c209c42ab))


### Features

* **sitemap:** Use new lib for generating sitemap ([1e4bf2f](https://github.com/Belphemur/AddictedProxy/commit/1e4bf2f3b8da2f414e524e554f5f3f46dba965a6))

## [4.9.4](https://github.com/Belphemur/AddictedProxy/compare/v4.9.3...v4.9.4) (2023-07-04)


### Performance improvements

* **search:** clean up the code to be more efficient ([79dec5f](https://github.com/Belphemur/AddictedProxy/commit/79dec5f7f3b2c684b0918cd47c078a900762f92d))

## [4.9.3](https://github.com/Belphemur/AddictedProxy/compare/v4.9.2...v4.9.3) (2023-07-04)


### Performance improvements

* **search:** Improve the usability of the search component ([9c80456](https://github.com/Belphemur/AddictedProxy/commit/9c804562b9fd5a5d8fd23b30a77a33b75754f917))

## [4.9.2](https://github.com/Belphemur/AddictedProxy/compare/v4.9.1...v4.9.2) (2023-07-04)


### Bug Fixes

* **api:ssr:** Fix route for api page ([9a92b08](https://github.com/Belphemur/AddictedProxy/commit/9a92b08262c9b0242f10f150cccde2bfc6c90b18))

## [4.9.1](https://github.com/Belphemur/AddictedProxy/compare/v4.9.0...v4.9.1) (2023-07-04)


### Bug Fixes

* **ssr:** fix api route for SSR ([6d0c6f5](https://github.com/Belphemur/AddictedProxy/commit/6d0c6f52c07c8028d23a1e47cffa6211fbfe509e))

## [4.9.0](https://github.com/Belphemur/AddictedProxy/compare/v4.8.2...v4.9.0) (2023-07-04)


### Performance improvements

* **Loading:** improve the loading of pages ([60a3d11](https://github.com/Belphemur/AddictedProxy/commit/60a3d11ba5d6d4faf933ee81cc84e2290c2b364b))
* **matomo:** don't touch the router ([817afb3](https://github.com/Belphemur/AddictedProxy/commit/817afb3bd48a10bbadc22c07c5eaf7721f596a13))
* **matomo:** Fix loading matomo ([11edb03](https://github.com/Belphemur/AddictedProxy/commit/11edb03ecc5f4f7263867b20e69c95a0cb68da8e))
* **store:** use store for the language ([6adc21a](https://github.com/Belphemur/AddictedProxy/commit/6adc21ac516340442bc7ad37436f4ea704a91581))
* **webfont:** add webfont ([15c40d9](https://github.com/Belphemur/AddictedProxy/commit/15c40d9a41322b6f710f5febac777e6decbba041))


### Bug Fixes

* **matomo:** fix tracking ([c35fa24](https://github.com/Belphemur/AddictedProxy/commit/c35fa24d1fc11775e9dca2f93dd1a17e3c67cea4))
* **media:view:** Fix mobile view ([b11041b](https://github.com/Belphemur/AddictedProxy/commit/b11041b4f0d002e37af4dcb6d5006c7ae59e864d))
* **media:view:** Fix the view of media ([06c2a8c](https://github.com/Belphemur/AddictedProxy/commit/06c2a8cd751941a7620fde55116965961bf0a127))
* **tracking:** Fix issue with tracking ([2c43201](https://github.com/Belphemur/AddictedProxy/commit/2c4320141bb5cfde6df5f63b8b5c57b456c730d2))


### Features

* **ssr:api:** Add api page ([2a60fad](https://github.com/Belphemur/AddictedProxy/commit/2a60fad8d2887f65e9d5d8388977d345a5555155))
* **ssr:privacy:** add privacy policy page ([d0b5f24](https://github.com/Belphemur/AddictedProxy/commit/d0b5f24cead3456afe809176ebe3fc18eceb7e81))
* **ssr:** get the home page to work ([26e06e0](https://github.com/Belphemur/AddictedProxy/commit/26e06e05297581625ebe850bccbaa3be70c72167))

## [4.8.2](https://github.com/Belphemur/AddictedProxy/compare/v4.8.1...v4.8.2) (2023-07-03)


### Bug Fixes

* **robots.txt:** fix robots.txt file ([fea2fe6](https://github.com/Belphemur/AddictedProxy/commit/fea2fe688228f677fb771d6cd2eb67be148b09bf))
* **sitemap:** Be sure to only list show with episodes ([364b0b7](https://github.com/Belphemur/AddictedProxy/commit/364b0b76a4bdd746dee316d58db1c2758ef758a4))
* **sitemap:** remove uneeded config ([4bb98e1](https://github.com/Belphemur/AddictedProxy/commit/4bb98e13bd5af17cab8125830188880e7bc49322))

## [4.8.1](https://github.com/Belphemur/AddictedProxy/compare/v4.8.0...v4.8.1) (2023-07-03)


### Bug Fixes

* **sitemap:** be sure it's hosted on the same domain ([8a9989a](https://github.com/Belphemur/AddictedProxy/commit/8a9989addcc99f06deac15ffb14fca01f4b8d44d))

## [4.8.0](https://github.com/Belphemur/AddictedProxy/compare/v4.7.0...v4.8.0) (2023-07-03)


### Performance improvements

* **lodash:** improve loading of lodash ([c934127](https://github.com/Belphemur/AddictedProxy/commit/c934127ebfcd123164a5208a618151ab175b405c))


### Bug Fixes

* **detail:** fix documentation for details ([2c7ffc1](https://github.com/Belphemur/AddictedProxy/commit/2c7ffc13d35f816b13d924220b68f72fc4dd3ea9))
* **media:poster:** send properly the poster ([4ff50fa](https://github.com/Belphemur/AddictedProxy/commit/4ff50fa9496cd712f8919f259c68b05b3f72ac9c))
* **media:** fix route ([348d62c](https://github.com/Belphemur/AddictedProxy/commit/348d62cd5e2b0575ca7f6d1e9a02856b2fc437cb))


### Features

* **media:info:** Add media info page ([55da38e](https://github.com/Belphemur/AddictedProxy/commit/55da38ed0f0b740802b3d197636f54a18edd67c4))
* **media:info:** Add media view ([05a2454](https://github.com/Belphemur/AddictedProxy/commit/05a24543547fd08f50c2fd33136cc97b8566640f))
* **Media:** Add Media details ([1f17670](https://github.com/Belphemur/AddictedProxy/commit/1f17670a84b424827058f17354b12dc58514a103))
* **sitemap:** add a way for the application to publish a sitemap ([afbcf0e](https://github.com/Belphemur/AddictedProxy/commit/afbcf0e704fed22b0f1e502336a4821fc7f0db9b))

## [4.7.0](https://github.com/Belphemur/AddictedProxy/compare/v4.6.13...v4.7.0) (2023-06-26)


### Bug Fixes

* **front:mobile:** fix mobile navigation ([d657b58](https://github.com/Belphemur/AddictedProxy/commit/d657b5880446bcbd139e2f14f4e3b05c39f055ee))
* **front:** fix building image ([462d82b](https://github.com/Belphemur/AddictedProxy/commit/462d82b4e40a6be42d7606395c8e1c11f83aa225))


### Features

* **front:api:** add api view ([f4d89ec](https://github.com/Belphemur/AddictedProxy/commit/f4d89ec86c3dbf935f65a1892ccfc1e3dc8dd1b6))
* **front:privacy:** Add privacy page ([5d656b9](https://github.com/Belphemur/AddictedProxy/commit/5d656b954a34302a14d0d189571eb248272c4f50))
* **front:search:** Add search box for shows and seasons ([3665b7a](https://github.com/Belphemur/AddictedProxy/commit/3665b7af64cdd986fa404bda68fc92d363e8489b))
* **front:search:** have searchbox and refresh functionality working ([2f2bca1](https://github.com/Belphemur/AddictedProxy/commit/2f2bca1ef45c7ed04622072443f56f75d1ae7784))
* **front:subtitles:** show the subtitles in the table ([3d70f29](https://github.com/Belphemur/AddictedProxy/commit/3d70f294ebb4290c6d3a8c13be844772742df34b))
* **front:** Navigation bar and rest of composables ([88100c1](https://github.com/Belphemur/AddictedProxy/commit/88100c1a57e17a338420ef88723185b06d64ba62))
* Update front-end to use vuetify ([7c483b7](https://github.com/Belphemur/AddictedProxy/commit/7c483b7b65f3cddd69ea4b272d021ead9d1c5234))

## [4.6.13](https://github.com/Belphemur/AddictedProxy/compare/v4.6.12...v4.6.13) (2023-06-25)


### Performance improvements

* **healthcheck:** add healthcheck to the app ([c094633](https://github.com/Belphemur/AddictedProxy/commit/c094633daeafd27b2cc5ce2082ba9fe6367d63f9))
* **healthcheck:** force docker to check for health ([71d96ba](https://github.com/Belphemur/AddictedProxy/commit/71d96ba80bd9e5d4076b29ebc8a3f364a45e4bfb))

## [4.6.12](https://github.com/Belphemur/AddictedProxy/compare/v4.6.11...v4.6.12) (2023-06-25)


### Performance improvements

* **Ratelimiting:** Add way to disable rate limiting ([5038997](https://github.com/Belphemur/AddictedProxy/commit/50389978fab2c90a93d8d236b1a53df6ae0606d0))

## [4.6.11](https://github.com/Belphemur/AddictedProxy/compare/v4.6.10...v4.6.11) (2023-06-22)


### Performance improvements

* use non alpine image ([c9f1586](https://github.com/Belphemur/AddictedProxy/commit/c9f1586d66e028828355432a35d82f96ef936445))

## [4.6.10](https://github.com/Belphemur/AddictedProxy/compare/v4.6.9...v4.6.10) (2023-06-22)


### Performance improvements

* **pyroscope:** Try in different folder ([85f8ef1](https://github.com/Belphemur/AddictedProxy/commit/85f8ef104187c483dfadcccaa4e931cc4dae02d2))

## [4.6.9](https://github.com/Belphemur/AddictedProxy/compare/v4.6.8...v4.6.9) (2023-06-22)


### Performance improvements

* **pyroscope:** fix missing lib ([5323cd5](https://github.com/Belphemur/AddictedProxy/commit/5323cd5a8d32a95804da2834cb7c43382beb612d))

## [4.6.8](https://github.com/Belphemur/AddictedProxy/compare/v4.6.7...v4.6.8) (2023-06-22)


### Performance improvements

* **pyroscope:** load the right lib ([472e241](https://github.com/Belphemur/AddictedProxy/commit/472e2411534ec4e0d055123b4a46d16ad41901c1))

## [4.6.7](https://github.com/Belphemur/AddictedProxy/compare/v4.6.6...v4.6.7) (2023-06-22)


### Performance improvements

* **Pyroscope:** Add Pyroscope to application ([d7fcd40](https://github.com/Belphemur/AddictedProxy/commit/d7fcd403bfa9bfb804527ad5c02b6d261c29e5c5))

## [4.6.6](https://github.com/Belphemur/AddictedProxy/compare/v4.6.5...v4.6.6) (2023-06-05)


### Performance improvements

* **deps:** Update all deps ([a90e890](https://github.com/Belphemur/AddictedProxy/commit/a90e89035d9a504fee6e1f33d25e0581a7897b6a))

## [4.6.5](https://github.com/Belphemur/AddictedProxy/compare/v4.6.4...v4.6.5) (2023-05-09)


### Performance improvements

* **pnpm:** fix version of pnpm ([e832c82](https://github.com/Belphemur/AddictedProxy/commit/e832c82822ace0351c80ee21f6e74f1cb891c51e))

## [4.6.4](https://github.com/Belphemur/AddictedProxy/compare/v4.6.3...v4.6.4) (2023-05-09)


### Bug Fixes

* **deps:** Fix missing dep ([55e8aef](https://github.com/Belphemur/AddictedProxy/commit/55e8aeff05128c8f97223d24635f034339986d12))


### Performance improvements

* **deps:** update deps ([7985d31](https://github.com/Belphemur/AddictedProxy/commit/7985d318c91a3d3a82e01a728b7bca2801158a1e))

## [4.6.3](https://github.com/Belphemur/AddictedProxy/compare/v4.6.2...v4.6.3) (2023-04-17)


### Performance improvements

* **deps:** update deps ([6ddd3d1](https://github.com/Belphemur/AddictedProxy/commit/6ddd3d12923fba61f3b39f4653ea726ff3bca069))
* **deps:** Update deps ([361188b](https://github.com/Belphemur/AddictedProxy/commit/361188bf71e6a4df73e93880116259c0cb89220d))

## [4.6.2](https://github.com/Belphemur/AddictedProxy/compare/v4.6.1...v4.6.2) (2023-03-20)


### Performance improvements

* **Privacy:** Touch up to be GDPR compliant ([f867adb](https://github.com/Belphemur/AddictedProxy/commit/f867adb078e9fc544317831e6b0332aa843e161f))

## [4.6.1](https://github.com/Belphemur/AddictedProxy/compare/v4.6.0...v4.6.1) (2023-03-20)


### Performance improvements

* **Privacy:** Add usage of Cloudflare ([99574cc](https://github.com/Belphemur/AddictedProxy/commit/99574ccba97affd9523457dfd8be999b8599d489))

## [4.6.0](https://github.com/Belphemur/AddictedProxy/compare/v4.5.16...v4.6.0) (2023-03-20)


### Features

* **Privacy:** Add privacy policy to the website ([586ec7d](https://github.com/Belphemur/AddictedProxy/commit/586ec7d02496b72227c97579ac5e25036e034c26))

## [4.5.16](https://github.com/Belphemur/AddictedProxy/compare/v4.5.15...v4.5.16) (2023-03-15)


### Performance improvements

* **deps:** Update dependencies ([429f177](https://github.com/Belphemur/AddictedProxy/commit/429f1772cd98507547651fc843338029ffa4568f))

## [4.5.15](https://github.com/Belphemur/AddictedProxy/compare/v4.5.14...v4.5.15) (2023-03-11)


### Performance improvements

* **deps:** update deps ([ab91fd0](https://github.com/Belphemur/AddictedProxy/commit/ab91fd0474d6a33251770b98d740d2ded6f1290b))

## [4.5.14](https://github.com/Belphemur/AddictedProxy/compare/v4.5.13...v4.5.14) (2023-03-04)


### Bug Fixes

* **FetchSubtitle:** Fix empty number of subtitles ([e940f28](https://github.com/Belphemur/AddictedProxy/commit/e940f288cb02ea1e6428cb992180b5516149cb85))

## [4.5.13](https://github.com/Belphemur/AddictedProxy/compare/v4.5.12...v4.5.13) (2023-03-04)


### Bug Fixes

* **ci:** always use LTS for node ([a1d74ee](https://github.com/Belphemur/AddictedProxy/commit/a1d74eecc9c133ae7a689ecd222eac5e750ae5b7))
* **ci:** disable most github api ([eb9ce4b](https://github.com/Belphemur/AddictedProxy/commit/eb9ce4bc6df6de85dafa285ae05c8f704e2778e3))
* **ci:** Fix dependencies of Semantic Release ([65400fa](https://github.com/Belphemur/AddictedProxy/commit/65400fa34096aa4196691765f5ad3bff45430c14))

## [4.5.12](https://github.com/Belphemur/AddictedProxy/compare/v4.5.11...v4.5.12) (2023-03-04)


### Performance improvements

* **Deps:** Updates deps ([ec2c00a](https://github.com/Belphemur/AddictedProxy/commit/ec2c00a444a4c0d768bd4a9c932f07d40fb706a8))
* **FetchJob:** Only schedule when we're sure the job is going to do something ([c3f141e](https://github.com/Belphemur/AddictedProxy/commit/c3f141ebeb0faea709e8f67c5bc138699f6fd43a))

## [4.5.11](https://github.com/Belphemur/AddictedProxy/compare/v4.5.10...v4.5.11) (2023-03-04)


### Performance improvements

* **uniquejob:** Refact naming ([45b6b06](https://github.com/Belphemur/AddictedProxy/commit/45b6b06a72859dbd3dfe26b8cbd905e75e657e98))

## [4.5.10](https://github.com/Belphemur/AddictedProxy/compare/v4.5.9...v4.5.10) (2023-03-04)


### Bug Fixes

* **uniqueJob:** use proper writeOnlyTransaction ([869facd](https://github.com/Belphemur/AddictedProxy/commit/869facd6c56a6d4f980f96c8792fe1a2fc886b35))

## [4.5.9](https://github.com/Belphemur/AddictedProxy/compare/v4.5.8...v4.5.9) (2023-03-04)


### Performance improvements

* **db:** use native transaction ([51f118d](https://github.com/Belphemur/AddictedProxy/commit/51f118d6f910e36f1fd9f50d7dbeb4ed05c3dd10))

## [4.5.8](https://github.com/Belphemur/AddictedProxy/compare/v4.5.7...v4.5.8) (2023-03-04)


### Performance improvements

* **Job:** Don't use redis anymore, use PGSQL for jobs ([e970414](https://github.com/Belphemur/AddictedProxy/commit/e9704147dd970d54f7b7231a63d69a33cce4a1d5))

## [4.5.7](https://github.com/Belphemur/AddictedProxy/compare/v4.5.6...v4.5.7) (2023-03-03)


### Bug Fixes

* **db::transaction:** Be sure the transaction is properly wrap into execution strategy. ([92ae5d9](https://github.com/Belphemur/AddictedProxy/commit/92ae5d95bcd9b6984bc10b8ee8c655877ca1ed52))

## [4.5.6](https://github.com/Belphemur/AddictedProxy/compare/v4.5.5...v4.5.6) (2023-03-03)


### Bug Fixes

* **Config:** Fix configuration for storage caching ([f755fb4](https://github.com/Belphemur/AddictedProxy/commit/f755fb42e89a932bc4f082149423814e3e00e849))
* **db:** enable retry on failure ([6cb1ee2](https://github.com/Belphemur/AddictedProxy/commit/6cb1ee263d2d06a54acb85a351fb9d613d694f13))

## [4.5.5](https://github.com/Belphemur/AddictedProxy/compare/v4.5.4...v4.5.5) (2023-03-03)


### Bug Fixes

* **tmdb:** Fix mapping movies ([a951716](https://github.com/Belphemur/AddictedProxy/commit/a951716b754eaa789568f710ac68b24df588053b))


### Performance improvements

* **Caching:** Use postgres for caching purpose ([91010cb](https://github.com/Belphemur/AddictedProxy/commit/91010cbbed4d92cad5227ceb491f91ebc0fe40e1))
* **DB:** Use proper connection strings ([09089cf](https://github.com/Belphemur/AddictedProxy/commit/09089cf27d6f25d5d0c41dd75de5967c919c380b))

## [4.5.4](https://github.com/Belphemur/AddictedProxy/compare/v4.5.3...v4.5.4) (2023-03-02)


### Performance improvements

* **tmdb:** Cleanup the code to remove repetitive one ([a38b176](https://github.com/Belphemur/AddictedProxy/commit/a38b176dbb707112f7826214463ca64a663b04d7))
* **Tmdb:** Improve the code of mapping shows/movie ([6abd7ab](https://github.com/Belphemur/AddictedProxy/commit/6abd7ab2fcf4b0de4b45bc44315c0f6eda9eea51))
* **tmdb:** Move all regex into own file ([cf39a4a](https://github.com/Belphemur/AddictedProxy/commit/cf39a4aa05d8eaaa834581720ead6e751965a8c3))

## [4.5.3](https://github.com/Belphemur/AddictedProxy/compare/v4.5.2...v4.5.3) (2023-03-01)


### Bug Fixes

* **tmdb:** Remove requirement on number of seasons ([2c5409f](https://github.com/Belphemur/AddictedProxy/commit/2c5409ffac27ff766e505cd41fe1a4fc03e1363a))

## [4.5.2](https://github.com/Belphemur/AddictedProxy/compare/v4.5.1...v4.5.2) (2023-03-01)


### Bug Fixes

* **CI:** Fix building image ([4e7b595](https://github.com/Belphemur/AddictedProxy/commit/4e7b595b193510d8ae8a127aa955fe2ce97f4fce))

## [4.5.1](https://github.com/Belphemur/AddictedProxy/compare/v4.5.0...v4.5.1) (2023-03-01)


### Bug Fixes

* **Show:** Updating shows losing data ([5495ae7](https://github.com/Belphemur/AddictedProxy/commit/5495ae7847e32533a9f0fa6c7672a0018e11e704))

## [4.5.0](https://github.com/Belphemur/AddictedProxy/compare/v4.4.9...v4.5.0) (2023-03-01)


### Features

* **Show:Dedupe:** Dedupe show that have different year/country ([f182639](https://github.com/Belphemur/AddictedProxy/commit/f182639fb597284886a1ca188d43c8559c2f8420))


### Performance improvements

* **Dedupe:** Optimize sql query ([493ca0d](https://github.com/Belphemur/AddictedProxy/commit/493ca0d562abf5e9da9b2c82dae87a2827f52654))
* **Performance:** Remove exporter when in debug mode ([f27849f](https://github.com/Belphemur/AddictedProxy/commit/f27849fb2398eea2a87f570209ae77a281055c74))

## [4.4.9](https://github.com/Belphemur/AddictedProxy/compare/v4.4.8...v4.4.9) (2023-03-01)


### Bug Fixes

* **Metrics:** Rate limiting doesn't support exemplars ([c902965](https://github.com/Belphemur/AddictedProxy/commit/c9029658bf90bee8490d5bf8d6eef7b6fdf4faaf))

## [4.4.8](https://github.com/Belphemur/AddictedProxy/compare/v4.4.7...v4.4.8) (2023-03-01)


### Performance improvements

* **tmdb:** Improve the matching logic ([857766e](https://github.com/Belphemur/AddictedProxy/commit/857766e3a062f1b7b82f293fa15b43f585a0ba51))
* **tmdb:** Match on number of seasons too ([8212e52](https://github.com/Belphemur/AddictedProxy/commit/8212e52ff77f429f696eead877cbf9843ed5a999))

## [4.4.7](https://github.com/Belphemur/AddictedProxy/compare/v4.4.6...v4.4.7) (2023-03-01)


### Performance improvements

* **tmdb:** No need to go more than 3 pages of results ([4a41e04](https://github.com/Belphemur/AddictedProxy/commit/4a41e04a3eb9a46779a135f54d89f6c6478abf63))

## [4.4.6](https://github.com/Belphemur/AddictedProxy/compare/v4.4.5...v4.4.6) (2023-03-01)


### Bug Fixes

* **Tmdb:** Fix UK matching to GB ([ffa34be](https://github.com/Belphemur/AddictedProxy/commit/ffa34be24b85068e06605c94197f13befa38c34f))
* **tvDb:** fix weird ordering by number of seasons ([a1262c7](https://github.com/Belphemur/AddictedProxy/commit/a1262c7b0d9d9feb523aca1e080351fb178f19e5))

## [4.4.5](https://github.com/Belphemur/AddictedProxy/compare/v4.4.4...v4.4.5) (2023-03-01)


### Bug Fixes

* **doc:** fix documentation of endpoint ([91921c9](https://github.com/Belphemur/AddictedProxy/commit/91921c98fe18d9e5024e8e81154dc0f92e89c35f))

## [4.4.4](https://github.com/Belphemur/AddictedProxy/compare/v4.4.3...v4.4.4) (2023-03-01)


### Bug Fixes

* **tmdb:** if can't find bbc show, give up ([62f167f](https://github.com/Belphemur/AddictedProxy/commit/62f167fe0648a0b3ccf2350dd038bf708c774a0e))

## [4.4.3](https://github.com/Belphemur/AddictedProxy/compare/v4.4.2...v4.4.3) (2023-03-01)


### Performance improvements

* **tmdb:** If show contains BBC that mean it's british, let's look for UK in the origin country directly ([9b41c1f](https://github.com/Belphemur/AddictedProxy/commit/9b41c1f0e9b10ab71b55ae466569c8c679bee0e3))

## [4.4.2](https://github.com/Belphemur/AddictedProxy/compare/v4.4.1...v4.4.2) (2023-03-01)


### Bug Fixes

* **tmdb:** Trust TMDB for the ordering, no need to do by date, that require to find all results. ([d863e0c](https://github.com/Belphemur/AddictedProxy/commit/d863e0c9c20ecbe2f08be21ba935c80415cd3648))

## [4.4.1](https://github.com/Belphemur/AddictedProxy/compare/v4.4.0...v4.4.1) (2023-03-01)


### Bug Fixes

* **TvDb:** Force resyncing the tvDb ([ee83351](https://github.com/Belphemur/AddictedProxy/commit/ee833510f05479a4aefdc3e800d0036865ec476c))

## [4.4.0](https://github.com/Belphemur/AddictedProxy/compare/v4.3.0...v4.4.0) (2023-03-01)


### Features

* **Show:** Add finding a show directly with the tvDbId ([17533f4](https://github.com/Belphemur/AddictedProxy/commit/17533f45afe7c98159783beff9ce69245dab53bd))


### Bug Fixes

* **tmdb:** Add country matching ([e491d9e](https://github.com/Belphemur/AddictedProxy/commit/e491d9ea42dedd82982ad757581fe9951437002b))
* **Tmdb:** Fix logic paging ([0170446](https://github.com/Belphemur/AddictedProxy/commit/01704467daa9b57900014b0876b8758d85851f0f))
* **Tmdb:** Fix matching show logic ([0cf22ca](https://github.com/Belphemur/AddictedProxy/commit/0cf22caa917cc3a35ccf2ab4d9dff3114f064750))
* **Tmdb:** Redo the pagination logic ([4c877d5](https://github.com/Belphemur/AddictedProxy/commit/4c877d58d84a75a795d924c6ea751a26fe05651a))
* **tvDbId:** return multiple shows with the tvDbId ([ae44df9](https://github.com/Belphemur/AddictedProxy/commit/ae44df9b42111b7c91fbba51e49640d1a1ee1224))
* **TvShow:** Don't override the tvdb id ([24eeca6](https://github.com/Belphemur/AddictedProxy/commit/24eeca6537461acb04fcfc008caa0e70cf7e95f1))
* **TvShow:** Keep the tv shows in sync ([b49f143](https://github.com/Belphemur/AddictedProxy/commit/b49f1435c8089ce925d9e1d728cfb267d6866845))


### Performance improvements

* **Tmdb:** Improve mapping using country and latest date for match ([ffec296](https://github.com/Belphemur/AddictedProxy/commit/ffec2960d544b48905e1d80affadc3167477512b))
* **TvShow:** Add index on tvdbid ([51a615b](https://github.com/Belphemur/AddictedProxy/commit/51a615b2022f4b4905fe73091ed129f6e8a38158))

## [4.3.0](https://github.com/Belphemur/AddictedProxy/compare/v4.2.14...v4.3.0) (2023-03-01)


### Features

* **Migration:** Add migration module for code migration ([9d964fe](https://github.com/Belphemur/AddictedProxy/commit/9d964febf8a108ab3b3e9d01e540403a68960ef0))
* **Migration:** Add one time migration ([d45eb7e](https://github.com/Belphemur/AddictedProxy/commit/d45eb7e3443210b4a58759b60a7f3bb231ac3c1a))
* **Tmdb:** Add getting external ids ([2ee2770](https://github.com/Belphemur/AddictedProxy/commit/2ee2770d144f27b87316469dcc4253a4bafe729d))
* **TvDb:** Add TvDb id to show ([81a3502](https://github.com/Belphemur/AddictedProxy/commit/81a35029e68e8109bee4e5cb9c92727b04967730))
* **tvdb:** Populate the tvdb ids for existing shows ([ac84ea9](https://github.com/Belphemur/AddictedProxy/commit/ac84ea98bbe5c07482e7f3530f9491583617915c))


### Bug Fixes

* **Migration:** be sure the migration run in own scope ([b0e4bc9](https://github.com/Belphemur/AddictedProxy/commit/b0e4bc9f5dce321d712ba9647c7c65fcb752c8cd))
* **MigrationDate:** Fix date attribute for migration ([3f2f8a2](https://github.com/Belphemur/AddictedProxy/commit/3f2f8a26c375fcdaeda98f7ccfa1a7f2a6ccfbcc))
* **Project:** Fix project setup ([46f69f7](https://github.com/Belphemur/AddictedProxy/commit/46f69f7efded46bd34f6e4c4bee19381b9c86dac))
* **Refresh::Ended:** Be sure to refresh ended show after the db has been updated ([1950340](https://github.com/Belphemur/AddictedProxy/commit/195034093a1a5013de95f32be6695612d0a0256b))


### Performance improvements

* **ratelimiting:** Keep exemplar ([a5ecff8](https://github.com/Belphemur/AddictedProxy/commit/a5ecff8c77860d09429457409bbb71c04d167c1d))
* **Telemetry:** make description optional ([33a063d](https://github.com/Belphemur/AddictedProxy/commit/33a063dc0f1553d36192d7a302e9169cc5ff71d4))
* **Tmdb:** Map the tvdb_id when matching shows ([dd2f037](https://github.com/Belphemur/AddictedProxy/commit/dd2f037f3edeb1fec62eaa724585aef9efd21cd0))

## [4.2.14](https://github.com/Belphemur/AddictedProxy/compare/v4.2.13...v4.2.14) (2023-02-26)


### Performance improvements

* **SqliteCache:** Add instrumentation ([ad9ae0d](https://github.com/Belphemur/AddictedProxy/commit/ad9ae0d3904939e985083c7151e913ec59af76a0))

## [4.2.13](https://github.com/Belphemur/AddictedProxy/compare/v4.2.12...v4.2.13) (2023-02-26)


### Bug Fixes

* **cache:** Fix not loading the right cache ([460c044](https://github.com/Belphemur/AddictedProxy/commit/460c0448e0e84d9049909aeb3da6d905c00d1b5b))
* **performance:** Make tracker singleton ([987314b](https://github.com/Belphemur/AddictedProxy/commit/987314bdc661a3249804e574ede48ab71af64282))

## [4.2.12](https://github.com/Belphemur/AddictedProxy/compare/v4.2.11...v4.2.12) (2023-02-26)


### Performance improvements

* **logging:** better logging ([aa1880c](https://github.com/Belphemur/AddictedProxy/commit/aa1880c7f75cc7f1504796f97c00d3228c6002c9))
* **redis:** verbose trace ([be8b7f5](https://github.com/Belphemur/AddictedProxy/commit/be8b7f5bc32bbc1afb4528ea576ee14038aa3aa6))
* **Span:** Only set detail when not a success ([e0a6b9e](https://github.com/Belphemur/AddictedProxy/commit/e0a6b9edc899b9e40d863d4d88cda82b61690b0f))

## [4.2.11](https://github.com/Belphemur/AddictedProxy/compare/v4.2.10...v4.2.11) (2023-02-26)


### Bug Fixes

* **Cache::set:** Time it properly ([516aeb4](https://github.com/Belphemur/AddictedProxy/commit/516aeb4f36c74b0502b25b30a2030cf0b8d4d91d))

## [4.2.10](https://github.com/Belphemur/AddictedProxy/compare/v4.2.9...v4.2.10) (2023-02-26)


### Performance improvements

* **SqliteCache::Trace:** Add tracing to sqlitecache ([68c1ca5](https://github.com/Belphemur/AddictedProxy/commit/68c1ca57fea201f4961598e421fa769222499832))

## [4.2.9](https://github.com/Belphemur/AddictedProxy/compare/v4.2.8...v4.2.9) (2023-02-26)


### Performance improvements

* **Hangfire:** Add hangfire ([d626a83](https://github.com/Belphemur/AddictedProxy/commit/d626a8303a4dbc5402d02b7fa0fd73d3b8caf101))

## [4.2.8](https://github.com/Belphemur/AddictedProxy/compare/v4.2.7...v4.2.8) (2023-02-26)


### Performance improvements

* **Span:** Log exception ([1f2c0ab](https://github.com/Belphemur/AddictedProxy/commit/1f2c0ab6f1906cd5141634185924a6ec5fe46553))
* **state:** Fix state to not be shown as failure ([38a56b2](https://github.com/Belphemur/AddictedProxy/commit/38a56b2cc5c176b855fa068e19bf78c7ef335927))

## [4.2.7](https://github.com/Belphemur/AddictedProxy/compare/v4.2.6...v4.2.7) (2023-02-26)


### Performance improvements

* **Activity:** Add tags to activity. ([eead1ba](https://github.com/Belphemur/AddictedProxy/commit/eead1baa0d17baea6b090405201d8bd593a72e2d))

## [4.2.6](https://github.com/Belphemur/AddictedProxy/compare/v4.2.5...v4.2.6) (2023-02-26)


### Performance improvements

* **OTLP:** Add SQL to instrumentation ([cbf32fc](https://github.com/Belphemur/AddictedProxy/commit/cbf32fca00faef1528a67fa010e949cea48744ee))

## [4.2.5](https://github.com/Belphemur/AddictedProxy/compare/v4.2.4...v4.2.5) (2023-02-26)


### Bug Fixes

* **otlp:** Remove exemplar ([f70c231](https://github.com/Belphemur/AddictedProxy/commit/f70c231ba4884e0e1f3e607a7d475d44c0077a4b))

## [4.2.4](https://github.com/Belphemur/AddictedProxy/compare/v4.2.3...v4.2.4) (2023-02-26)


### Bug Fixes

* **otlp:** Disable exemplar on cache metrics ([be43a53](https://github.com/Belphemur/AddictedProxy/commit/be43a53e79810b0bdd456737d302e417ad4d0dab))

## [4.2.3](https://github.com/Belphemur/AddictedProxy/compare/v4.2.2...v4.2.3) (2023-02-26)


### Bug Fixes

* **version:** Fix version of app ([a5a99ac](https://github.com/Belphemur/AddictedProxy/commit/a5a99ace09f73268dad10c3a668b247b49a610de))

## [4.2.2](https://github.com/Belphemur/AddictedProxy/compare/v4.2.1...v4.2.2) (2023-02-26)


### Bug Fixes

* **otlp:** Fix issue with not getting an activity because of sampling ([7458685](https://github.com/Belphemur/AddictedProxy/commit/7458685f836292a04e6ca1db7f692bd3d25d0d2d))

## [4.2.1](https://github.com/Belphemur/AddictedProxy/compare/v4.2.0...v4.2.1) (2023-02-26)


### Bug Fixes

* **otlp:** Fix bootstrapping ([61d2611](https://github.com/Belphemur/AddictedProxy/commit/61d261160976b780b074722e9e85d0a1a7edb19f))
* **otlp:** Fix otlp registration ([5ab6c11](https://github.com/Belphemur/AddictedProxy/commit/5ab6c111c8b2679c0cadba131d8d95ecf07ecc38))

## [4.2.0](https://github.com/Belphemur/AddictedProxy/compare/v4.1.19...v4.2.0) (2023-02-26)


### Features

* **otlp:** First version of OpenTelemetry ([65daab8](https://github.com/Belphemur/AddictedProxy/commit/65daab88967b933355c1c029982e68eed9c8ed8a))

## [4.1.19](https://github.com/Belphemur/AddictedProxy/compare/v4.1.18...v4.1.19) (2023-02-12)


### Bug Fixes

* **download:** Consider any redirect that isn't a download exceeded to be a deleting of subtitle ([50372cf](https://github.com/Belphemur/AddictedProxy/commit/50372cf277878ddf7916be1e0b9e9dcbccedaf29))

## [4.1.18](https://github.com/Belphemur/AddictedProxy/compare/v4.1.17...v4.1.18) (2023-02-03)


### Bug Fixes

* **doc:** Fix documentation relating to show uniqueId ([1f45254](https://github.com/Belphemur/AddictedProxy/commit/1f45254d729d6b6fc391c1cc571f1d66742f4e78))


### Performance improvements

* **docs:** Improve documentation ([d238590](https://github.com/Belphemur/AddictedProxy/commit/d2385907cad4854154fc707d9d4da6470e15d1d3))

## [4.1.17](https://github.com/Belphemur/AddictedProxy/compare/v4.1.16...v4.1.17) (2023-02-01)


### Performance improvements

* **SqliteCache:** track evicted entries ([8adcfb7](https://github.com/Belphemur/AddictedProxy/commit/8adcfb7b1ee4219f60e49a3ee9d1bc4a3b168cff))
* **Storage::Cache:** Track evictions ([9d06bf4](https://github.com/Belphemur/AddictedProxy/commit/9d06bf4c5225f1b9b4a847689bb661aba081cf95))

## [4.1.16](https://github.com/Belphemur/AddictedProxy/compare/v4.1.15...v4.1.16) (2023-01-29)


### Bug Fixes

* **Addicted:** Renable proxy ([d5063da](https://github.com/Belphemur/AddictedProxy/commit/d5063dad183b35706c01161668d381ac5732abbf))

## [4.1.15](https://github.com/Belphemur/AddictedProxy/compare/v4.1.14...v4.1.15) (2023-01-29)


### Performance improvements

* **Upstream::Counter:** Add counter to upstream request done to get subtitles ([a89d216](https://github.com/Belphemur/AddictedProxy/commit/a89d2169879d87e7827d02139897e0f50744fddd))

## [4.1.14](https://github.com/Belphemur/AddictedProxy/compare/v4.1.13...v4.1.14) (2023-01-27)


### Performance improvements

* **RateLimiting:** Improve building the rate limiter ([e6c5a59](https://github.com/Belphemur/AddictedProxy/commit/e6c5a59da6d770c68c9143ac0cff433b3e075020))

## [4.1.13](https://github.com/Belphemur/AddictedProxy/compare/v4.1.12...v4.1.13) (2023-01-27)


### Performance improvements

* **Cache:** Track cache hit and miss for storage ([fc84b5c](https://github.com/Belphemur/AddictedProxy/commit/fc84b5cf7f61b700ba7ac6882bb468f68bd4d3b5))
* **Metrics:** No ratelimiting ([97707f1](https://github.com/Belphemur/AddictedProxy/commit/97707f1b37d0565bc83ff31d0d7cfe3f53c7f677))
* **RateLimiting:** Add metric for rate limiting ([a8e7ef0](https://github.com/Belphemur/AddictedProxy/commit/a8e7ef01cc104ff3444fe757e28196d929f3fbde))

## [4.1.12](https://github.com/Belphemur/AddictedProxy/compare/v4.1.11...v4.1.12) (2023-01-27)


### Performance improvements

* **Addicted:** Use compression for all requests/responses ([3e6e46d](https://github.com/Belphemur/AddictedProxy/commit/3e6e46d88fdb3c17bab86e48412d7f54873c43ff))
* **Sentry:** Don't send trace headers ([680f00c](https://github.com/Belphemur/AddictedProxy/commit/680f00c791c4be8f755d4b0846617c1733dbb7a7))

## [4.1.11](https://github.com/Belphemur/AddictedProxy/compare/v4.1.10...v4.1.11) (2023-01-27)


### Performance improvements

* **Subtitle:** Wait longer before giving up on getting subtitles ([5b3b638](https://github.com/Belphemur/AddictedProxy/commit/5b3b638ba81c09f554e48491aff7f0f5ceeded48))

## [4.1.10](https://github.com/Belphemur/AddictedProxy/compare/v4.1.9...v4.1.10) (2023-01-27)


### Bug Fixes

* **Hangfire:** Keep prefix as the default one ([e1ee489](https://github.com/Belphemur/AddictedProxy/commit/e1ee489874f0276fea1993d85536e90a5bb6f30e))

## [4.1.9](https://github.com/Belphemur/AddictedProxy/compare/v4.1.8...v4.1.9) (2023-01-27)


### Performance improvements

* **Hangfire:** Be sure to have the load shared on all the nodes of the cluster ([980e1f2](https://github.com/Belphemur/AddictedProxy/commit/980e1f26270dce87ab7bc5cdb61f0db2a1af050c))

## [4.1.8](https://github.com/Belphemur/AddictedProxy/compare/v4.1.7...v4.1.8) (2023-01-27)


### Bug Fixes

* **SQL:** Use ILIKE instead of lower casing ([1e114cd](https://github.com/Belphemur/AddictedProxy/commit/1e114cd83a6a92f32f60847527512ff3a6586a90))

## [4.1.7](https://github.com/Belphemur/AddictedProxy/compare/v4.1.6...v4.1.7) (2023-01-27)


### Bug Fixes

* **SQL:** Include the season at the end of the query ([99177a7](https://github.com/Belphemur/AddictedProxy/commit/99177a7ed5854ad5434a62985293fc7d983d537a))

## [4.1.6](https://github.com/Belphemur/AddictedProxy/compare/v4.1.5...v4.1.6) (2023-01-27)


### Bug Fixes

* **Hangfire:** Only keep error for retry ([019cc70](https://github.com/Belphemur/AddictedProxy/commit/019cc7084f2cf2e6af5c2f91e24c217f2e4a0c9a))


### Performance improvements

* **Hangfire:** Mute logs about retrying jobs ([a5d5b1d](https://github.com/Belphemur/AddictedProxy/commit/a5d5b1dc95475206cb95f1beeffded49f84b8986))

## [4.1.5](https://github.com/Belphemur/AddictedProxy/compare/v4.1.4...v4.1.5) (2023-01-27)


### Bug Fixes

* **RateLimiting:** Access header from anywhere ([0547b2f](https://github.com/Belphemur/AddictedProxy/commit/0547b2f96e184d233ef690293d396078fa8cd4d2))

## [4.1.4](https://github.com/Belphemur/AddictedProxy/compare/v4.1.3...v4.1.4) (2023-01-27)


### Bug Fixes

* **RateLimiting:** Use Cloudflare header ([afd15a5](https://github.com/Belphemur/AddictedProxy/commit/afd15a5fa8267cdd07a8fe6250fdd9ed13d6c3b2))

## [4.1.3](https://github.com/Belphemur/AddictedProxy/compare/v4.1.2...v4.1.3) (2023-01-27)


### Bug Fixes

* **RateLimiting:** Fix when to get the ip of request ([4961b24](https://github.com/Belphemur/AddictedProxy/commit/4961b2469e6c3dce62c14a5000a02c86268271e0))

## [4.1.2](https://github.com/Belphemur/AddictedProxy/compare/v4.1.1...v4.1.2) (2023-01-27)


### Performance improvements

* **RateLimiting:** Add ratelimiting logging ([e0044ac](https://github.com/Belphemur/AddictedProxy/commit/e0044ac7ccb13674812a52a9fa20803d1d672640))

## [4.1.1](https://github.com/Belphemur/AddictedProxy/compare/v4.1.0...v4.1.1) (2023-01-27)


### Bug Fixes

* **IP:** Get the IP from the forward headers ([790af28](https://github.com/Belphemur/AddictedProxy/commit/790af285664765ffa4e6d8738aaf1f395eff03b6))


### Performance improvements

* **RateLimiting:** Use Microsoft rate limiter ([18c630b](https://github.com/Belphemur/AddictedProxy/commit/18c630be22a1920e6abdd7a524063c1dd12b69f3))

## [4.1.0](https://github.com/Belphemur/AddictedProxy/compare/v4.0.10...v4.1.0) (2023-01-26)


### Features

* **FileStorage:** Add ability to use file for caching purpose instead of distributed cache. ([46f1215](https://github.com/Belphemur/AddictedProxy/commit/46f1215f269f0e52e16ae1997711367b89e7c993))
* **SqliteCache:** Add support for Absolute Expiry ([098b506](https://github.com/Belphemur/AddictedProxy/commit/098b5066b2db3d200a9ed6e4c730f1ced675d685))


### Bug Fixes

* **SqlCache:** Check for directory empty too ([a8e16c0](https://github.com/Belphemur/AddictedProxy/commit/a8e16c0b07fece15cd9d21acf0bae506b5abb156))
* **SqlCache:** Only create directory if needed ([ff42039](https://github.com/Belphemur/AddictedProxy/commit/ff42039d638a0cbaca88684d101fcca2a1027311))
* **SqliteCache::AbsoluteExpiry:** Check properly the absolute expiry ([45f2780](https://github.com/Belphemur/AddictedProxy/commit/45f2780b056ac1431d2ec7feef1dac1190988eda))
* **SqliteCache:** Create directory if not exists ([5c7f67f](https://github.com/Belphemur/AddictedProxy/commit/5c7f67f586b179ddf6d7750e09d8093b46a4d1c3))
* **Sqlite:** Fix missing comma ([7625548](https://github.com/Belphemur/AddictedProxy/commit/7625548ac4757bd8b0b887e6594d6b18f1fe1972))
* **Storage::Caching:** Fix type ([bba0626](https://github.com/Belphemur/AddictedProxy/commit/bba0626dacb9ca454c9408ca27521fb9d94fd110))


### Performance improvements

* **job:** handle weird exceptions ([9acb0a2](https://github.com/Belphemur/AddictedProxy/commit/9acb0a2c5bcc5dc2b7c135dccbcbe80d10b4df66))
* **Job:** increase timeout ([4f91544](https://github.com/Belphemur/AddictedProxy/commit/4f91544b475d71801724c1cccb5772d5b33f2649))
* **job:** Make job retry less often ([1c77be6](https://github.com/Belphemur/AddictedProxy/commit/1c77be630e7fd3227b2f7267ff0b174cc7aeb670))
* **Job:** only wait 10 sec before giving up on scheduling a fetch job ([2a38987](https://github.com/Belphemur/AddictedProxy/commit/2a38987a83fc3c39bac2ed08bcfc513ec611e6b9))
* **SqliteCache:** Add auto vacuum capability ([3b28d14](https://github.com/Belphemur/AddictedProxy/commit/3b28d14097b2eea97164a90ac427a39bfb75dc8e))
* **Storage::Caching:** Use sqlite for storage caching ([c27f22f](https://github.com/Belphemur/AddictedProxy/commit/c27f22fd8ed8c6eb4975c11644f02b70793a1954))

## [4.0.10](https://github.com/Belphemur/AddictedProxy/compare/v4.0.9...v4.0.10) (2023-01-17)


### Bug Fixes

* **UniqueJob:** Fallback on recalculating the key ([97477dd](https://github.com/Belphemur/AddictedProxy/commit/97477dd4fe3ca86b6904191a600a5db18ca1e60e))
* **UniqueJob:** Fix deserializing the key properly ([41094d3](https://github.com/Belphemur/AddictedProxy/commit/41094d304a653c33673b08b35ebfe28e652c2746))

## [4.0.9](https://github.com/Belphemur/AddictedProxy/compare/v4.0.8...v4.0.9) (2023-01-17)


### Bug Fixes

* **Sentry:** Fix transaction sent to sentry ([b5d6376](https://github.com/Belphemur/AddictedProxy/commit/b5d637624583f167c095244b00211e2d741b4f51))

## [4.0.8](https://github.com/Belphemur/AddictedProxy/compare/v4.0.7...v4.0.8) (2023-01-17)


### Bug Fixes

* **Cred::Download:** Fix finding account that need to be redeemed ([e4adc9a](https://github.com/Belphemur/AddictedProxy/commit/e4adc9a36187a837f22e3c3acefa11070a74dce8))

## [4.0.7](https://github.com/Belphemur/AddictedProxy/compare/v4.0.6...v4.0.7) (2023-01-16)


### Bug Fixes

* **Job:** Fix job creation failure ([f262ed7](https://github.com/Belphemur/AddictedProxy/commit/f262ed7d692546c85c0b76ea1ac44d43f86270e8))

## [4.0.6](https://github.com/Belphemur/AddictedProxy/compare/v4.0.5...v4.0.6) (2023-01-14)


### Performance improvements

* **Redis:** One connection for the full application instead of one for Hangfire and one for caching ([2554993](https://github.com/Belphemur/AddictedProxy/commit/2554993a58e0afa42e69fc681d8c60e59412d6b6))
* **Swagger:** Improve the styling of dark mode ([ddd7c68](https://github.com/Belphemur/AddictedProxy/commit/ddd7c6840f3a5deba69f12a1e9a6ce546c86c808))

## [4.0.5](https://github.com/Belphemur/AddictedProxy/compare/v4.0.4...v4.0.5) (2023-01-14)


### Bug Fixes

* **swagger:** Use dist package for swagger ([c8b3e5b](https://github.com/Belphemur/AddictedProxy/commit/c8b3e5ba7b9d7f7b2ca6184905a350269026b789))
* **swagger:** Use dist package for swagger ([91f3661](https://github.com/Belphemur/AddictedProxy/commit/91f3661061fd0ccb28251ff8c1448b5e7e8b8762))

## [4.0.4](https://github.com/Belphemur/AddictedProxy/compare/v4.0.3...v4.0.4) (2023-01-14)


### Bug Fixes

* **chunking:** Fix chunking config for the front-end ([0e915e9](https://github.com/Belphemur/AddictedProxy/commit/0e915e9559b3afc72ae74de379cc11384c438e6a))

## [4.0.3](https://github.com/Belphemur/AddictedProxy/compare/v4.0.2...v4.0.3) (2023-01-14)


### Bug Fixes

* **route:** Fix route not showing ([853336d](https://github.com/Belphemur/AddictedProxy/commit/853336d4e06552289bdd5b13451c5475d9feb241))
* **swagger:** Fix css issue ([e013f75](https://github.com/Belphemur/AddictedProxy/commit/e013f75573d72428c118a960648d2c4c969db824))


### Performance improvements

* **Chunking:** Better chunking the production package ([6931d7c](https://github.com/Belphemur/AddictedProxy/commit/6931d7c2640a4314c5458b5fd735fb682e35b368))

## [4.0.2](https://github.com/Belphemur/AddictedProxy/compare/v4.0.1...v4.0.2) (2023-01-14)


### Bug Fixes

* **SwaggerUi:** Fix dark theme ([46829ec](https://github.com/Belphemur/AddictedProxy/commit/46829ec14e82b5a017cd1a58e484fed2e16f4fee))

## [4.0.1](https://github.com/Belphemur/AddictedProxy/compare/v4.0.0...v4.0.1) (2023-01-14)


### Bug Fixes

* **Chart:** Fix chart component for top 10 ([8c174d3](https://github.com/Belphemur/AddictedProxy/commit/8c174d313f7014aaa5ef3c18f6039962bf0cb977))

## [4.0.0](https://github.com/Belphemur/AddictedProxy/compare/v3.7.3...v4.0.0) (2023-01-14)


### ⚠ BREAKING CHANGES

* **Storage:** Remove UpLink

### Features

* **Storage:** Split compression from mean storage provider ([d32a099](https://github.com/Belphemur/AddictedProxy/commit/d32a09911d92cdbf401e1b6e6110ff35724bfccd))


### Bug Fixes

* **Storage::Cache:** Fix not finding file in storage provider ([c9c8c64](https://github.com/Belphemur/AddictedProxy/commit/c9c8c649409b33f331053305d5b7bfc08d9b5c37))


### Performance improvements

* **Subtitle::Cache:** Compress the subtitle in the database ([c01fa42](https://github.com/Belphemur/AddictedProxy/commit/c01fa420b38416390d0bba9a7166357c6f4ecfe7))

## [3.7.3](https://github.com/Belphemur/AddictedProxy/compare/v3.7.2...v3.7.3) (2023-01-14)


### Performance improvements

* **Prometheus:** Add prometheus Asp.NET metrics ([fb5e75d](https://github.com/Belphemur/AddictedProxy/commit/fb5e75d4b4e9cc7d9a61c1394438fd42182118b8))

## [3.7.2](https://github.com/Belphemur/AddictedProxy/compare/v3.7.1...v3.7.2) (2023-01-11)


### Bug Fixes

* **CachingConfig:** Fix config deserialization ([38d7133](https://github.com/Belphemur/AddictedProxy/commit/38d7133bcdbf6eac343f28cd8f049ed2d7b120a6))

## [3.7.1](https://github.com/Belphemur/AddictedProxy/compare/v3.7.0...v3.7.1) (2023-01-11)


### Performance improvements

* **Storage::Caching:** Make caching configurable ([de14bd2](https://github.com/Belphemur/AddictedProxy/commit/de14bd2c8225ea8db51b9f747774aa0013bbc69b))

## [3.7.0](https://github.com/Belphemur/AddictedProxy/compare/v3.6.0...v3.7.0) (2023-01-06)


### Features

* **Caching:** Add distributed caching to storage provider ([3054091](https://github.com/Belphemur/AddictedProxy/commit/305409184ce1babda49517700537fb2c044ee167))
* **Caching:** Use distributed caching for subtitles ([ccd5ba5](https://github.com/Belphemur/AddictedProxy/commit/ccd5ba5f48daae7eb6e76e15eab6f931efbea757))


### Bug Fixes

* **Caching:** Fix constructor for distributed caching of storage ([08d6b3b](https://github.com/Belphemur/AddictedProxy/commit/08d6b3bd813f9009af516e6fec83cd61a627f973))
* **Caching:** only reset position for memory streams ([6b56cc6](https://github.com/Belphemur/AddictedProxy/commit/6b56cc690f27128b8ac13d57d2b6223d6a15fa99))


### Performance improvements

* **Caching:** Improve the distributed caching of subtitle by using sharding key based on the episode id ([3fdd356](https://github.com/Belphemur/AddictedProxy/commit/3fdd356081d20734dd89e8ea3f8a8396c9a42b2a))
* **Caching:** Refactor the cache storage provider ([7d8f0cd](https://github.com/Belphemur/AddictedProxy/commit/7d8f0cdf7e62d6b8ee3a471e133ce548b67b65d2))

## [3.6.0](https://github.com/Belphemur/AddictedProxy/compare/v3.5.7...v3.6.0) (2023-01-01)


### Features

* **Search:** Add proper error message when can't find show ([07b3f0e](https://github.com/Belphemur/AddictedProxy/commit/07b3f0e500574c37c4863051e857491351c1510a))


### Bug Fixes

* **Search:** Fix not finding show and keeping the loading animation ([e8268c8](https://github.com/Belphemur/AddictedProxy/commit/e8268c89d70434f4867adf234406818962e5368f))

## [3.5.7](https://github.com/Belphemur/AddictedProxy/compare/v3.5.6...v3.5.7) (2022-12-26)


### Bug Fixes

* **Storage::CloudflareR2:** Fix issue with storing data in R2 ([a0aa194](https://github.com/Belphemur/AddictedProxy/commit/a0aa194589f8f4b516cd8d630c38f960000d321b))

## [3.5.6](https://github.com/Belphemur/AddictedProxy/compare/v3.5.5...v3.5.6) (2022-12-25)


### Bug Fixes

* **Redis:** fix parsing config ([e8e2f40](https://github.com/Belphemur/AddictedProxy/commit/e8e2f4043ef3833ae9075065cca70b30e1af1e37))


### Performance improvements

* **Redis:** Use timeout from the config ([5dae94e](https://github.com/Belphemur/AddictedProxy/commit/5dae94eac6f9f7cf2e9d75c62f6c3ede1bcbf8f5))

## [3.5.5](https://github.com/Belphemur/AddictedProxy/compare/v3.5.4...v3.5.5) (2022-12-22)


### Bug Fixes

* **DownloadCredJob:** Delete job after 10 failures ([b86cc6a](https://github.com/Belphemur/AddictedProxy/commit/b86cc6a2f5f27f68c227d1255c3b35ed69bddf03))
* **Subtitle::Search:** Fix language search like 'Chinese (Simplified)' ([0c5b0c6](https://github.com/Belphemur/AddictedProxy/commit/0c5b0c6f376cdb73194c727738c22248fa781c36))

## [3.5.4](https://github.com/Belphemur/AddictedProxy/compare/v3.5.3...v3.5.4) (2022-12-16)


### Performance improvements

* **Culture:** Keep cache longer ([741888f](https://github.com/Belphemur/AddictedProxy/commit/741888f9f8033d7e3b3f2a1c253f77ee61491ba8))
* **DownloadUsage:** Always keep it in sync with addicted ([45d6b05](https://github.com/Belphemur/AddictedProxy/commit/45d6b052d55a3857576b60eaff9e8df0839ba336))

## [3.5.3](https://github.com/Belphemur/AddictedProxy/compare/v3.5.2...v3.5.3) (2022-12-15)


### Performance improvements

* **FetchJob:** Reduce max attemps, keep the current backoff formula and increase timeout to 3.5 min ([55c1c5e](https://github.com/Belphemur/AddictedProxy/commit/55c1c5ee7b9cd2b08431e74dac852551bb4d6c4b))

## [3.5.2](https://github.com/Belphemur/AddictedProxy/compare/v3.5.1...v3.5.2) (2022-12-14)


### Bug Fixes

* **Subtitle:Deleted:** Handle properly the deletion of subtitle from Addic7ed ([2d860d4](https://github.com/Belphemur/AddictedProxy/commit/2d860d4c4c277953eae355fad87a04b0c845af62))

## [3.5.1](https://github.com/Belphemur/AddictedProxy/compare/v3.5.0...v3.5.1) (2022-12-14)


### Performance improvements

* **DownloadUsage:** Refactor the download usage to be handled by credential service ([04fdb59](https://github.com/Belphemur/AddictedProxy/commit/04fdb5900129b88d73df3d1c867bd9204bce900e))

## [3.5.0](https://github.com/Belphemur/AddictedProxy/compare/v3.4.5...v3.5.0) (2022-12-14)


### Features

* **Download::Usage:** Add function to track download usage of creds ([c0f23f5](https://github.com/Belphemur/AddictedProxy/commit/c0f23f5ba76b89391bba4ce677be9a81c2ab64d0))
* **DownloadUsage:** Create feature to reset the download usage of credentials hourly and when tagged as used ([1051bee](https://github.com/Belphemur/AddictedProxy/commit/1051bee89f54c886a52d397ecaa630ed11b7dccb))


### Bug Fixes

* **Download::Creds:** Be sure the creds get tagged as exceeded before continuing recursion ([0e7ddbe](https://github.com/Belphemur/AddictedProxy/commit/0e7ddbed24a2883101e6d8e5f050e61932bcc5b6))
* **DownloadUsage:** Be sure to consider overusage as fully used ([1d05e7d](https://github.com/Belphemur/AddictedProxy/commit/1d05e7dd98c50464432f662d5abff0183b16c250))

## [3.4.5](https://github.com/Belphemur/AddictedProxy/compare/v3.4.4...v3.4.5) (2022-12-12)


### Performance improvements

* **Download:** Change handler lifetime to get new IP at download ([f4115db](https://github.com/Belphemur/AddictedProxy/commit/f4115db682b486cb07bfb68a5c7d204b81086164))

## [3.4.4](https://github.com/Belphemur/AddictedProxy/compare/v3.4.3...v3.4.4) (2022-12-12)


### Performance improvements

* **Subtitle::Download:** Disable proxy for downloading subtitles ([5482e30](https://github.com/Belphemur/AddictedProxy/commit/5482e30d6969b75c5c13c276c1bc958d36453da5))

## [3.4.3](https://github.com/Belphemur/AddictedProxy/compare/v3.4.2...v3.4.3) (2022-12-12)


### Performance improvements

* **Job:** Remove from recurring job ([6e9808f](https://github.com/Belphemur/AddictedProxy/commit/6e9808ffca94bb1d51a9a3f438224ec86322af12))

## [3.4.2](https://github.com/Belphemur/AddictedProxy/compare/v3.4.1...v3.4.2) (2022-12-11)


### Bug Fixes

* **Subtitle::Language:** Don't use any bulk operation. Rely on good old entity framework. ([1bdfaf0](https://github.com/Belphemur/AddictedProxy/commit/1bdfaf0dbd3eb5814afb8e7a96c1cf383fc0f4f7))

## [3.4.1](https://github.com/Belphemur/AddictedProxy/compare/v3.4.0...v3.4.1) (2022-12-11)


### Bug Fixes

* **Subtitle::Language:** Fix updating the subtitle language. ([de27020](https://github.com/Belphemur/AddictedProxy/commit/de2702078f3cf8bc0547b6e8ecaefb5af408fd1f))

## [3.4.0](https://github.com/Belphemur/AddictedProxy/compare/v3.3.13...v3.4.0) (2022-12-11)


### Features

* **Language:** Use proper iso code for language ([d752b01](https://github.com/Belphemur/AddictedProxy/commit/d752b01a772b8a9913f4ecd32a00f782be4a7cf8))


### Bug Fixes

* **Culture:** Parsing of culture present on Addic7ed ([16e290e](https://github.com/Belphemur/AddictedProxy/commit/16e290ee60f9e6ad79ab2cd59f047ae7369f06cd))


### Performance improvements

* **Performance::Span:** Cleanup parent pointer on disposing ([7f3a0bb](https://github.com/Belphemur/AddictedProxy/commit/7f3a0bb2581dc4e8bb6fd21ccaf8d1e42f5a670d))
* **Subtitle::Language:** Improve job performance to set the language of subtitles ([afa40c3](https://github.com/Belphemur/AddictedProxy/commit/afa40c3427093b00c6a6f437c0eba1ac46f53db9))

## [3.3.13](https://github.com/Belphemur/AddictedProxy/compare/v3.3.12...v3.3.13) (2022-12-10)


### Performance improvements

* **Sentry:** Create Scope when creating transaction to be sure everything is clean. ([d6118d5](https://github.com/Belphemur/AddictedProxy/commit/d6118d5a2f943abec913dbe0316316f00a3456ba))

## [3.3.12](https://github.com/Belphemur/AddictedProxy/compare/v3.3.11...v3.3.12) (2022-12-10)


### Bug Fixes

* **Show::BBC:** Fix show from the BBC that couldn't be matched with TMDB. ([c5b8fc4](https://github.com/Belphemur/AddictedProxy/commit/c5b8fc4304e70ebbfdfa8e455ad2682b9797b4c6))

## [3.3.11](https://github.com/Belphemur/AddictedProxy/compare/v3.3.10...v3.3.11) (2022-12-10)


### Performance improvements

* **refresh::single:** Add timeout of 8 minute to do the full refresh ([c9d962f](https://github.com/Belphemur/AddictedProxy/commit/c9d962f1e35773c3acc48d8485a8e7f9b2c2dce9))

## [3.3.10](https://github.com/Belphemur/AddictedProxy/compare/v3.3.9...v3.3.10) (2022-12-10)


### Bug Fixes

* **Season:** Fix case where there isn't any season for the show ([a8b2180](https://github.com/Belphemur/AddictedProxy/commit/a8b21809a0f427754dbaa2ff8cbb90f6f34386bc))

## [3.3.9](https://github.com/Belphemur/AddictedProxy/compare/v3.3.8...v3.3.9) (2022-12-10)


### Bug Fixes

* **TVDB:** Fix not mapping Canceled state to Completed show ([4b50dc7](https://github.com/Belphemur/AddictedProxy/commit/4b50dc7d9ea1d8143448e9a075cab960b58234b0))

## [3.3.8](https://github.com/Belphemur/AddictedProxy/compare/v3.3.7...v3.3.8) (2022-12-10)


### Performance improvements

* **FetchJob::Delay:** Change the retry delay on job failure ([25f0ae1](https://github.com/Belphemur/AddictedProxy/commit/25f0ae1d6c72d5eba3a29febea32c7d5f68551c7))

## [3.3.7](https://github.com/Belphemur/AddictedProxy/compare/v3.3.6...v3.3.7) (2022-12-10)


### Performance improvements

* **FetchJob:** Only schedule fetch job when we know it's going to query addic7ed ([4318ea8](https://github.com/Belphemur/AddictedProxy/commit/4318ea89769cf4365f5e179baf6ad2ca7618eab4))

## [3.3.6](https://github.com/Belphemur/AddictedProxy/compare/v3.3.5...v3.3.6) (2022-12-10)


### Performance improvements

* **Hangfire:** increase lock timeout ([af50b51](https://github.com/Belphemur/AddictedProxy/commit/af50b51f821ab644bb360179d3585b4593f0258d))
* **redis:** Increase redis timeout ([6c723fa](https://github.com/Belphemur/AddictedProxy/commit/6c723faa87d9f8dbfeecf39e8fbfb31768f1d89b))

## [3.3.5](https://github.com/Belphemur/AddictedProxy/compare/v3.3.4...v3.3.5) (2022-12-10)


### Performance improvements

* **FetchJob:** Force fetch job to retry itself if couldn't finish in 2 minutes ([429492a](https://github.com/Belphemur/AddictedProxy/commit/429492a2743b2a8cc7960625351d491f809a0910))

## [3.3.4](https://github.com/Belphemur/AddictedProxy/compare/v3.3.3...v3.3.4) (2022-12-10)


### Bug Fixes

* **Job:** Be sure the unique key is properly calculated instead of relying on the full args ([6bc0e61](https://github.com/Belphemur/AddictedProxy/commit/6bc0e61aaa205b60ae13bea53a61507598d0ff7f))

## [3.3.3](https://github.com/Belphemur/AddictedProxy/compare/v3.3.2...v3.3.3) (2022-12-10)


### Performance improvements

* **Redis:** Increase timeout to 10 secs ([400c228](https://github.com/Belphemur/AddictedProxy/commit/400c22899f66fb473dd037b86d187bda09bc245e))

## [3.3.2](https://github.com/Belphemur/AddictedProxy/compare/v3.3.1...v3.3.2) (2022-12-10)


### Performance improvements

* **Hangfire:** Add the default queue back into the processing list. ([5a394f8](https://github.com/Belphemur/AddictedProxy/commit/5a394f8b8a92f9380cea53a00abe471954c4369c))

## [3.3.1](https://github.com/Belphemur/AddictedProxy/compare/v3.3.0...v3.3.1) (2022-12-09)


### Bug Fixes

* **hangfire:** disable require ssl ([eba9038](https://github.com/Belphemur/AddictedProxy/commit/eba903803e0a5d33b3894ea9ca39e701fdf7ec84))

## [3.3.0](https://github.com/Belphemur/AddictedProxy/compare/v3.2.11...v3.3.0) (2022-12-09)


### Features

* **JobSystem::Hangfire:** Replace old job system by hangfire ([ec9bdfa](https://github.com/Belphemur/AddictedProxy/commit/ec9bdfaee50d7fb8ab525332054b2a9489c12aed))


### Performance improvements

* **Job::Fingerprint:** be sure the finger print is properly generated ([2f463b4](https://github.com/Belphemur/AddictedProxy/commit/2f463b460a83bbc920daa72a98363b0b8f314c62))
* **Job::Fingerprint:** Use SHA384 to reduce collision chance ([94598a2](https://github.com/Belphemur/AddictedProxy/commit/94598a2f0802d51615d8dc6e6a4543e5fd32f79e))
* **Refresh:** Disallow multiple refresh job for same show ([17fd245](https://github.com/Belphemur/AddictedProxy/commit/17fd245c2ba3f3c87d9235a999b7008ebb191cd5))

## [3.2.11](https://github.com/Belphemur/AddictedProxy/compare/v3.2.10...v3.2.11) (2022-12-07)


### Performance improvements

* **Fetch:** reducing concurrency on fetch job ([d308c31](https://github.com/Belphemur/AddictedProxy/commit/d308c310f35b53f95b476763baf8bfc5199d8b41))

## [3.2.10](https://github.com/Belphemur/AddictedProxy/compare/v3.2.9...v3.2.10) (2022-11-30)


### Performance improvements

* **Addicted:** Change timeout to 35 seconds ([3737f40](https://github.com/Belphemur/AddictedProxy/commit/3737f4025cc5d5dcd9e8208698951423f929a1ba))
* **Debugging:** Add debugging tool to the image ([5b6b40e](https://github.com/Belphemur/AddictedProxy/commit/5b6b40e19d7da6bf246634f83bf2aa71be6390a6))

## [3.2.9](https://github.com/Belphemur/AddictedProxy/compare/v3.2.8...v3.2.9) (2022-11-29)


### Performance improvements

* **Culture::Parsing:** Refactor language parsing to use distributed cache and own object ([9cfe84e](https://github.com/Belphemur/AddictedProxy/commit/9cfe84e607a1b7c2647fdc0db57c48e191bcf943))

## [3.2.8](https://github.com/Belphemur/AddictedProxy/compare/v3.2.7...v3.2.8) (2022-11-28)


### Performance improvements

* **Fetch:** Increase concurrency ([0ea67b0](https://github.com/Belphemur/AddictedProxy/commit/0ea67b0292f06864c074096367723924c3ab0169))
* **Tracking:** improve tracking ([8004e7a](https://github.com/Belphemur/AddictedProxy/commit/8004e7a4e51b3e4cc29a4adda2ebd61eff013aa8))

## [3.2.7](https://github.com/Belphemur/AddictedProxy/compare/v3.2.6...v3.2.7) (2022-11-28)


### Bug Fixes

* **Subtitle:** Add missing designer file ([3162b43](https://github.com/Belphemur/AddictedProxy/commit/3162b43eb9fd2f31513ba9a652152d81f2d5c951))

## [3.2.6](https://github.com/Belphemur/AddictedProxy/compare/v3.2.5...v3.2.6) (2022-11-28)


### Bug Fixes

* **perf:** fix starting span properly when already one present ([abaf485](https://github.com/Belphemur/AddictedProxy/commit/abaf485bda483776358f81299766b500c264d418))
* **Perf:** naming ([f6793c6](https://github.com/Belphemur/AddictedProxy/commit/f6793c634e3176cfe33ac6882d1fed195cc19379))
* **Performance:** Only start transaction when there isn't one already ([9e633ed](https://github.com/Belphemur/AddictedProxy/commit/9e633edbd4bde684f4c85179c626fd57a133bab8))


### Performance improvements

* **Subtitle::Language:** Store proper language for subtitles ([7279f90](https://github.com/Belphemur/AddictedProxy/commit/7279f90dd897061f1d6196fde25d369bf47dc15e))

## [3.2.5](https://github.com/Belphemur/AddictedProxy/compare/v3.2.4...v3.2.5) (2022-11-28)


### Bug Fixes

* **Performance:** Only start transaction when there isn't one already ([e533934](https://github.com/Belphemur/AddictedProxy/commit/e533934b15d6049cef0b1d2d20111f3d9a06c3bd))

## [3.2.4](https://github.com/Belphemur/AddictedProxy/compare/v3.2.3...v3.2.4) (2022-11-28)


### Performance improvements

* **Tracking:** Improve performance monitoring ([68942a1](https://github.com/Belphemur/AddictedProxy/commit/68942a16d5fd04301d45f044ada4a197059abc2c))

## [3.2.3](https://github.com/Belphemur/AddictedProxy/compare/v3.2.2...v3.2.3) (2022-11-28)


### Performance improvements

* **outputCache:** Remove output cache middleware ([8bb7101](https://github.com/Belphemur/AddictedProxy/commit/8bb7101f21964ba3e35b3ee1d519434294614f0e))

## [3.2.2](https://github.com/Belphemur/AddictedProxy/compare/v3.2.1...v3.2.2) (2022-11-27)


### Performance improvements

* **FetchJob:** Improve concurrency ([19b24bf](https://github.com/Belphemur/AddictedProxy/commit/19b24bfcd5c9d92611e17e997d64e5c0dc42b64a))
* **popularity:** delete recording popularity ([2585ebf](https://github.com/Belphemur/AddictedProxy/commit/2585ebf9b1a9047dcdc3cedb215ff00f33cafe38))
* **popularity:** delete show popularity ([7c623a1](https://github.com/Belphemur/AddictedProxy/commit/7c623a173d6e603c3b694c5540fb5b8a2643ab8e))

## [3.2.1](https://github.com/Belphemur/AddictedProxy/compare/v3.2.0...v3.2.1) (2022-11-27)


### Bug Fixes

* **TvShow::Search:** Fix Case Insensitive search ([7d3f4b5](https://github.com/Belphemur/AddictedProxy/commit/7d3f4b59b89507e29b8d523c82d28e99bad08b48))

## [3.2.0](https://github.com/Belphemur/AddictedProxy/compare/v3.1.0...v3.2.0) (2022-11-27)


### Features

* **Postgres:** Use postgresql ([d27a5e4](https://github.com/Belphemur/AddictedProxy/commit/d27a5e49eb0e75daaf84c54f1ab05768505f2feb))


### Bug Fixes

* **TvShow::Search:** Fix search for TvShow ([8168008](https://github.com/Belphemur/AddictedProxy/commit/81680089061edd086395216ccbf9888005e72560))

## [3.1.0](https://github.com/Belphemur/AddictedProxy/compare/v3.0.1...v3.1.0) (2022-11-27)


### Features

* **Postgres:** Use postgresql ([1eb86a1](https://github.com/Belphemur/AddictedProxy/commit/1eb86a1cad2f5e39fd3408cf45d0b18d690b588c))


### Performance improvements

* **Show:** Set case insensitive collation for name of show ([6380a2f](https://github.com/Belphemur/AddictedProxy/commit/6380a2fd8f43fc7ccf95611e134046523570af6f))

## [3.0.2](https://github.com/Belphemur/AddictedProxy/compare/v3.0.1...v3.0.2) (2022-11-27)


### Performance improvements

* **Show:** Set case insensitive collation for name of show ([6380a2f](https://github.com/Belphemur/AddictedProxy/commit/6380a2fd8f43fc7ccf95611e134046523570af6f))

## [3.0.1](https://github.com/Belphemur/AddictedProxy/compare/v3.0.0...v3.0.1) (2022-11-27)


### Bug Fixes

* **Subtitles:** Fix issue when the season exists but don't have subtitles ([450e397](https://github.com/Belphemur/AddictedProxy/commit/450e3979677a3f3da5026551c2d649682a399193))

## [3.0.0](https://github.com/Belphemur/AddictedProxy/compare/v2.19.8...v3.0.0) (2022-11-27)


### ⚠ BREAKING CHANGES

* **DB:** Won't work anymore with sqlite

### Features

* **DB:** Use Mariadb as database ([a3a899d](https://github.com/Belphemur/AddictedProxy/commit/a3a899d22ed9b14a92ed2a3b81b65368854e27af))


### Bug Fixes

* **caching:** disable distributed caching for output cache ([2cf38e4](https://github.com/Belphemur/AddictedProxy/commit/2cf38e4757e69d8525f1c78dfc1862298c409000))
* **concurrency::db:** Fix issue with concurrency and database ([9d24eb3](https://github.com/Belphemur/AddictedProxy/commit/9d24eb3f44a726aff27b24b4069a5bec1c067355))
* **db:** Update seq number ([159f779](https://github.com/Belphemur/AddictedProxy/commit/159f7791b9a4008c03ce379356bdbb8372c967ce))
* **Fetch:** Reduce concurrency ([ab55497](https://github.com/Belphemur/AddictedProxy/commit/ab554974bdcf84c95962fbd21df065b134e9918d))
* **Ids:** Don't override ids when bulk update ([ddb4fc4](https://github.com/Belphemur/AddictedProxy/commit/ddb4fc468c4a45bf4b9ce7d69a36aea3ce41bbab))
* **Popularity:** Use the show Id ([829bf92](https://github.com/Belphemur/AddictedProxy/commit/829bf92220f4a975331e03c5621716fa53891b24))
* **RefreshShow::Concurrency:** Fix issue with refreshing 2 seasons at once ([4c67ec4](https://github.com/Belphemur/AddictedProxy/commit/4c67ec4089b2c993dab787e7b27c23292a108f04))
* **UserAgent:** Add new user agents ([dde0862](https://github.com/Belphemur/AddictedProxy/commit/dde08623bfaa73ae3a5a8c3112ffa66acd629bf2))


### Performance improvements

* **Client:** Remove bot param ([eab09bd](https://github.com/Belphemur/AddictedProxy/commit/eab09bd30686d2e927038632b4eb9cdecbae29f4))
* **refresher:** make them scoped ([922d0bd](https://github.com/Belphemur/AddictedProxy/commit/922d0bdfd7f9c2e9d115f9fab22f6ec1908d1206))
* **Season:** Update how to refresh seasons ([fd96f1c](https://github.com/Belphemur/AddictedProxy/commit/fd96f1cc1dd35fba5d817c6cf8bea4cdeb8a65f5))

## [2.19.8](https://github.com/Belphemur/AddictedProxy/compare/v2.19.7...v2.19.8) (2022-11-26)


### Bug Fixes

* **outputcache:** disable distributed ([f607baa](https://github.com/Belphemur/AddictedProxy/commit/f607baa1c739768aaf5f7cae9ded41e0ea7c9657))


### Performance improvements

* **NewRelic:** Remove it ([bb39fc3](https://github.com/Belphemur/AddictedProxy/commit/bb39fc3c80c7c4293b70e359bf795b5a1a9a5e05))
* **NewRelic:** Remove it from frontend ([d76e14a](https://github.com/Belphemur/AddictedProxy/commit/d76e14a1a45b983a8e601dc856662a3661fd8679))
* **Router:** Scrolling when changing page ([d8c700e](https://github.com/Belphemur/AddictedProxy/commit/d8c700e7fff5ddbbe7036c698438e00da94ecf8a))
* **Subtitle:** Retire endpoint ([eb59bfc](https://github.com/Belphemur/AddictedProxy/commit/eb59bfc392dc7909d2f5631049cc8df7ec6f3b9f))

## [2.19.7](https://github.com/Belphemur/AddictedProxy/compare/v2.19.6...v2.19.7) (2022-11-12)


### Bug Fixes

* **Creds:** Fix method use to save changes ([89ad1ab](https://github.com/Belphemur/AddictedProxy/commit/89ad1ab6ee09115bdd19a2be7e52964c0c9fdcf6))
* **EntityFramework:** Fix Bulk lib ([1baad3d](https://github.com/Belphemur/AddictedProxy/commit/1baad3d578598c09b1d59dca14387c9ba5ac85f3))


### Performance improvements

* **sentry:** Add session replay ([8d1d9a5](https://github.com/Belphemur/AddictedProxy/commit/8d1d9a5d83bf646b3f9b475a2fc1b117e535a007))
* **Sentry:** Session replay keep text ([c46c286](https://github.com/Belphemur/AddictedProxy/commit/c46c28644cfb4928145c0653241ed0c57f2410c6))

## [2.19.6](https://github.com/Belphemur/AddictedProxy/compare/v2.19.5...v2.19.6) (2022-11-12)


### Performance improvements

* **Deps:** Update deps ([2caa806](https://github.com/Belphemur/AddictedProxy/commit/2caa806fda164c280f8c56a9a71b23b55d7c5ff9))

## [2.19.5](https://github.com/Belphemur/AddictedProxy/compare/v2.19.4...v2.19.5) (2022-11-06)


### Performance improvements

* **Transaction:** Add new relic transaction ([8d1bcf7](https://github.com/Belphemur/AddictedProxy/commit/8d1bcf794c17d9f8f7d14525a5298589e3fad056))

## [2.19.4](https://github.com/Belphemur/AddictedProxy/compare/v2.19.3...v2.19.4) (2022-11-06)


### Bug Fixes

* **Outputcache:** Remove output cache for subtitles ([73b6651](https://github.com/Belphemur/AddictedProxy/commit/73b665148a60e69e065189c1fef38977d89a30de))
* **Subtitle::Download:** Fix UI download subtitle. ([9de7a36](https://github.com/Belphemur/AddictedProxy/commit/9de7a3634eaaae1226a09a19120ed9069767a22b))


### Performance improvements

* **Find:** Deprecate endpoint ([af58ec3](https://github.com/Belphemur/AddictedProxy/commit/af58ec3239c80a52528e1000836fbffe9f010695))
* **Search:** Remove old post api for show search ([66e00b3](https://github.com/Belphemur/AddictedProxy/commit/66e00b3545f051005daa5b7fdfef39f3cfa73d5f))

## [2.19.3](https://github.com/Belphemur/AddictedProxy/compare/v2.19.2...v2.19.3) (2022-11-06)


### Bug Fixes

* **APM:** new relic license key ([ee73f8a](https://github.com/Belphemur/AddictedProxy/commit/ee73f8a926241010b6f52f956dadbb3724623820))

## [2.19.2](https://github.com/Belphemur/AddictedProxy/compare/v2.19.1...v2.19.2) (2022-11-06)


### Bug Fixes

* **APM:** Wrong license key for new relic fixed. ([fd98ec4](https://github.com/Belphemur/AddictedProxy/commit/fd98ec4fec4980f3876004a2677eed3045936bd5))

## [2.19.1](https://github.com/Belphemur/AddictedProxy/compare/v2.19.0...v2.19.1) (2022-11-06)


### Performance improvements

* **APM:** Add new relic ([2f0957c](https://github.com/Belphemur/AddictedProxy/commit/2f0957cbc76a853f29ba15465823d80b62204033))
* **APM:** Add new relic front-end ([e97db8d](https://github.com/Belphemur/AddictedProxy/commit/e97db8d98547d821fe06145a1614d42e29327561))

## [2.19.0](https://github.com/Belphemur/AddictedProxy/compare/v2.18.3...v2.19.0) (2022-10-29)


### Features

* **Subtitles:** Get subtitles using show Unique ID ([af803b2](https://github.com/Belphemur/AddictedProxy/commit/af803b2b2c9e3f5050216399b368a980c4c334fc))

## [2.18.3](https://github.com/Belphemur/AddictedProxy/compare/v2.18.2...v2.18.3) (2022-10-23)


### Performance improvements

* **Locking:** Replace underlying lib ([43d5c32](https://github.com/Belphemur/AddictedProxy/commit/43d5c32be752b74d10edd8654865570333a2d129))

## [2.18.2](https://github.com/Belphemur/AddictedProxy/compare/v2.18.1...v2.18.2) (2022-10-23)


### Bug Fixes

* **Search:** Add min length to search ([a4518fc](https://github.com/Belphemur/AddictedProxy/commit/a4518fc17d3246774a1ace6c31441f22d2990303))


### Performance improvements

* **Search:** Finish the refactoring for searching subtitles ([cc8dbf1](https://github.com/Belphemur/AddictedProxy/commit/cc8dbf1c02f48e77e531d548c60cda51435573eb))
* **Subtitle::Search:** Add proper service to take care of searching for subtitles ([0a45e16](https://github.com/Belphemur/AddictedProxy/commit/0a45e169b593f23ad29594eb8a3c6c6b4b572e19))

## [2.18.1](https://github.com/Belphemur/AddictedProxy/compare/v2.18.0...v2.18.1) (2022-10-22)


### Bug Fixes

* **Show:** Fix search where we don't use everything from the route as the query ([5566472](https://github.com/Belphemur/AddictedProxy/commit/556647246e997c672cda7c4315bbb455564d4e29))


### Performance improvements

* **Caching:** Remove output cache from search show ([3ccdfcf](https://github.com/Belphemur/AddictedProxy/commit/3ccdfcf45ab5472d6fb551870a48b76e8ee5de4e))

## [2.18.0](https://github.com/Belphemur/AddictedProxy/compare/v2.17.7...v2.18.0) (2022-10-08)


### Features

* **Show:** Add Type of Show ([e9a82bc](https://github.com/Belphemur/AddictedProxy/commit/e9a82bc0c4ade034516b5cf26e6bd0edcaa5247f))
* **Show:** Check if they are movies ([c49c5a0](https://github.com/Belphemur/AddictedProxy/commit/c49c5a02114256310c9875efd46ceac9258ab470))
* **TMDB:** Add search for movies ([ca59ce7](https://github.com/Belphemur/AddictedProxy/commit/ca59ce7d04840757141e64b0352eb1cd319921f8))

## [2.17.7](https://github.com/Belphemur/AddictedProxy/compare/v2.17.6...v2.17.7) (2022-10-06)


### Performance improvements

* **Sentry:** Use proper project for it ([fe5bdc6](https://github.com/Belphemur/AddictedProxy/commit/fe5bdc641ebb7c30ee4324d3e485b1a188a8dbfa))

## [2.17.6](https://github.com/Belphemur/AddictedProxy/compare/v2.17.5...v2.17.6) (2022-10-05)


### Bug Fixes

* **Sentry:** Be sure we have distributed tracing from the front-end ([143fd44](https://github.com/Belphemur/AddictedProxy/commit/143fd44f46dd5edc1fe992ba7fd83796db319559))

## [2.17.5](https://github.com/Belphemur/AddictedProxy/compare/v2.17.4...v2.17.5) (2022-10-05)


### Bug Fixes

* **CORS:** Let the main website have access to the api ([13e20b5](https://github.com/Belphemur/AddictedProxy/commit/13e20b5218b60cce3fe6c55e29ffedec58d65791))
* **Jobs:** Fix issue with job crashing ([95e7f93](https://github.com/Belphemur/AddictedProxy/commit/95e7f9328ee1b32b856157e415f526250dd7524d))

## [2.17.4](https://github.com/Belphemur/AddictedProxy/compare/v2.17.3...v2.17.4) (2022-10-04)


### Performance improvements

* **Tracing:** Add tracing in the front-end ([ff540f7](https://github.com/Belphemur/AddictedProxy/commit/ff540f783f16e23153ce0dbd19342675c899e769))

## [2.17.3](https://github.com/Belphemur/AddictedProxy/compare/v2.17.2...v2.17.3) (2022-09-20)


### Bug Fixes

* **Caching:** Remove support for tags ([8dc7439](https://github.com/Belphemur/AddictedProxy/commit/8dc74395bcd8c909d5dc0c2910acc98e21e26200))
* **serialization:** Use context first than fallback to defaultJsonSerializer ([4db0ee7](https://github.com/Belphemur/AddictedProxy/commit/4db0ee74bdef84b379ecaf07faab29ee52b13841))

## [2.17.2](https://github.com/Belphemur/AddictedProxy/compare/v2.17.1...v2.17.2) (2022-09-20)


### Bug Fixes

* **serialization:** Use context first than fallback to defaultJsonSerializer ([4900b04](https://github.com/Belphemur/AddictedProxy/commit/4900b0445038fd8f69c1b12611b4e1601ba799ef))

## [2.17.1](https://github.com/Belphemur/AddictedProxy/compare/v2.17.0...v2.17.1) (2022-09-20)


### Bug Fixes

* **download:** Use proper error object when can't download sub ([091e2b6](https://github.com/Belphemur/AddictedProxy/commit/091e2b6f3197cab45b49fee5a956967992f85650))
* **JsonSerialization:** Add missing type ([1c38ec4](https://github.com/Belphemur/AddictedProxy/commit/1c38ec448f6be055c430f534c12edaea0309e829))

## [2.17.0](https://github.com/Belphemur/AddictedProxy/compare/v2.16.6...v2.17.0) (2022-09-19)


### Features

* **.NET:** Update to .NET 7.0 ([a6662e3](https://github.com/Belphemur/AddictedProxy/commit/a6662e3b8dd24d9aa7243ac4abbe2e280f22ff9d))
* **compression:** add request compression ([0b6090c](https://github.com/Belphemur/AddictedProxy/commit/0b6090c4e7e66111b59586df18a2884b15892837))
* **DistributedCaching:** Add distributed caching ([0960c35](https://github.com/Belphemur/AddictedProxy/commit/0960c351f6214a4a9928b1fd827d8f3861026095))
* **DistributedCaching:** Setup distributed caching with env vars ([1a19564](https://github.com/Belphemur/AddictedProxy/commit/1a195641dcfe5cbf26eb443045d2e65778cc8456))
* **OutputCache:** Replace ResponseCache by output cache ([e3848a7](https://github.com/Belphemur/AddictedProxy/commit/e3848a77097f0755f909a5063da3d8c2b1f7acab))


### Bug Fixes

* **Caching:** Be sure to send the right headers for response caching ([1cb0b4e](https://github.com/Belphemur/AddictedProxy/commit/1cb0b4e4d0c1dcc36d3b9608ea7a9bbf10b2bafa))

## [2.16.6](https://github.com/Belphemur/AddictedProxy/compare/v2.16.5...v2.16.6) (2022-09-18)


### Performance improvements

* **Caching:** Increase the cache for response caching ([aa61f8a](https://github.com/Belphemur/AddictedProxy/commit/aa61f8accef30c60a5de9ef6e3fd20df15c230e5))

## [2.16.5](https://github.com/Belphemur/AddictedProxy/compare/v2.16.4...v2.16.5) (2022-09-14)


### Bug Fixes

* **changelog:** Add missing key for changelog ([c614d69](https://github.com/Belphemur/AddictedProxy/commit/c614d69fa2307782580a564d688cfd813de853f2))


### Performance improvements

* **logging:** remove bad logging for prod ([9280a62](https://github.com/Belphemur/AddictedProxy/commit/9280a62d19c608ed2b25f3f40ed5b6b37acf8af0))

## [2.16.4](https://github.com/Belphemur/AddictedProxy/compare/v2.16.3...v2.16.4) (2022-09-14)


### Bug Fixes

* **caching:** Be sure to cache 404 when the show can't be found ([8b4d311](https://github.com/Belphemur/AddictedProxy/commit/8b4d3113bd22c754d7f5a03a620cd41655347974))

## [2.16.3](https://github.com/Belphemur/AddictedProxy/compare/v2.16.2...v2.16.3) (2022-09-14)

## [2.16.2](https://github.com/Belphemur/AddictedProxy/compare/v2.16.1...v2.16.2) (2022-09-12)

## [2.16.1](https://github.com/Belphemur/AddictedProxy/compare/v2.16.0...v2.16.1) (2022-09-12)


### Bug Fixes

* **TvShow::Search:** Always trim user inputs ([475e814](https://github.com/Belphemur/AddictedProxy/commit/475e814e1534a6d803c3418eb68af933f2ff67a2))

## [2.16.0](https://github.com/Belphemur/AddictedProxy/compare/v2.15.0...v2.16.0) (2022-09-12)


### Features

* **TvShow::Search:** Add Get route for proper caching ([fdee99c](https://github.com/Belphemur/AddictedProxy/commit/fdee99c6fdeacdb79ecd623cce89cba993daccdb))

## [2.15.0](https://github.com/Belphemur/AddictedProxy/compare/v2.14.1...v2.15.0) (2022-09-12)


### Features

* **Find:** Add proper get search that can be easily cached. ([b51b1cf](https://github.com/Belphemur/AddictedProxy/commit/b51b1cf549a1ee8b009666e85ee72884842e2d28))

## [2.14.1](https://github.com/Belphemur/AddictedProxy/compare/v2.14.0...v2.14.1) (2022-09-11)

## [2.14.0](https://github.com/Belphemur/AddictedProxy/compare/v2.13.4...v2.14.0) (2022-09-10)


### Features

* **Show::Completed:** Check regularly if on-going show ended. ([86cb4e5](https://github.com/Belphemur/AddictedProxy/commit/86cb4e5584e7485c0efff378ed615024bb463485))


### Bug Fixes

* **Database::Locking:** Fix possible lock of the Show table when checking state in TMDB ([8e78451](https://github.com/Belphemur/AddictedProxy/commit/8e78451af9872bb23e55f255dc6ecce3e7014e04))

## [2.13.4](https://github.com/Belphemur/AddictedProxy/compare/v2.13.3...v2.13.4) (2022-08-05)


### Bug Fixes

* **Popularity:** Fix failure to record popularity because of concurrency ([574e48c](https://github.com/Belphemur/AddictedProxy/commit/574e48c53dd1a2bd77aeb608b46b63a2d0441f84))

## [2.13.3](https://github.com/Belphemur/AddictedProxy/compare/v2.13.2...v2.13.3) (2022-08-05)


### Bug Fixes

* **Popularity:** Don't try to save the show ([4756e2a](https://github.com/Belphemur/AddictedProxy/commit/4756e2ae2ddd3cc77ffc6f0a0abbcf0b0a97b6db))

## [2.13.2](https://github.com/Belphemur/AddictedProxy/compare/v2.13.1...v2.13.2) (2022-08-05)


### Bug Fixes

* **Tmdb::Mapping:** Save every 50 shows ([7b534d3](https://github.com/Belphemur/AddictedProxy/commit/7b534d313d7b3c5c9e7808422f6dcc90753d46d7))

## [2.13.1](https://github.com/Belphemur/AddictedProxy/compare/v2.13.0...v2.13.1) (2022-08-05)


### Bug Fixes

* **Popularity:** Use a job to record the popularity of a show ([9e3e565](https://github.com/Belphemur/AddictedProxy/commit/9e3e565a32437e23b50015f7518aad4f2702a0d9))

## [2.13.0](https://github.com/Belphemur/AddictedProxy/compare/v2.12.0...v2.13.0) (2022-08-05)


### Features

* **Tmdb::Mapping:** Move show mapping into own job ([1ad5a93](https://github.com/Belphemur/AddictedProxy/commit/1ad5a93505e5674db81e9432c329d3caba62625a))

## [2.12.0](https://github.com/Belphemur/AddictedProxy/compare/v2.11.17...v2.12.0) (2022-08-05)


### Features

* **Refresh:** Don't refresh as often if it's a completed show ([4efb0bf](https://github.com/Belphemur/AddictedProxy/commit/4efb0bfb187e984663a5a95f6bc8d57fb1ba3a8a))
* **TMDB:** Add getting show details ([f46fec1](https://github.com/Belphemur/AddictedProxy/commit/f46fec1610a17bb9cd1e03a0cbd6274db76f04ad))
* **TMDB:** Add tv search to tmdb client ([412ee1c](https://github.com/Belphemur/AddictedProxy/commit/412ee1c8ae29bb89bff692a59c331bea8f158ca9))
* **Tmdb:** Automatically map show to tmdb ([c458a3f](https://github.com/Belphemur/AddictedProxy/commit/c458a3f0154c66ee8105de5407faeaba550b47e0))
* **TVDB:** Provide information from TVDB ([6891983](https://github.com/Belphemur/AddictedProxy/commit/68919839229805b65ef6027f4165bde9f35acd03))


### Bug Fixes

* **ShowDetails:** Runtime is nullable ([bfc892d](https://github.com/Belphemur/AddictedProxy/commit/bfc892d2ddd5e1d025c96f0bf97e85ada3dfc587))
* **Tmdb::Search:** Fix infinite search when no results ([77438d6](https://github.com/Belphemur/AddictedProxy/commit/77438d69e5d9a41d11049bd222eee52491bca002))
* **Tmdb::ShowDetails:** Fix show without known number of season and episodes ([256f575](https://github.com/Belphemur/AddictedProxy/commit/256f575b30c74bceffd072da1a0f29bfe8793b3a))
* **Tmdb:** Fix building uri for the client ([2b8120b](https://github.com/Belphemur/AddictedProxy/commit/2b8120b532c4c60723027f2d1ef8d6865219dcf4))
* **Tmdb:** Fix the returned type of search ([a96fca8](https://github.com/Belphemur/AddictedProxy/commit/a96fca8aa5610ffc2eccf92bd5d2d130cdf7f922))

## [2.11.17](https://github.com/Belphemur/AddictedProxy/compare/v2.11.16...v2.11.17) (2022-08-03)


### Enhancements

* **Addic7ed::Season:** Don't crash if there isn't any season ([4a80210](https://github.com/Belphemur/AddictedProxy/commit/4a8021064929217ac2515f68eeb27c58e67b9c7b))
* **Addic7ed:** Remove retry on nothing to parse ([ef6368e](https://github.com/Belphemur/AddictedProxy/commit/ef6368e05e12ecdd350ce076a489abed05d81614))
* **Performance:** Reduce sampling rate to not bust sentry limit ([d529d96](https://github.com/Belphemur/AddictedProxy/commit/d529d960b13159ca1cb9d70e5c8c30bb8f90a069))
* **Perf:** Reduce sampling. Reaching quota. ([0ef4db1](https://github.com/Belphemur/AddictedProxy/commit/0ef4db14f9c61ebd5f64ef3388be2ef981b001ee))

## [2.11.16](https://github.com/Belphemur/AddictedProxy/compare/v2.11.15...v2.11.16) (2022-07-30)


### Enhancements

* **analytics:** Use matomo ([1c6fa37](https://github.com/Belphemur/AddictedProxy/commit/1c6fa37bce5c1519635f24bc0c77bb8aeb599282))
* **Refresh:** Different refresh for different season. Last season refresh more often than older seasons. ([6dfe5b7](https://github.com/Belphemur/AddictedProxy/commit/6dfe5b722b19ef816fc9089d565f8f0687c07249))

## [2.11.15](https://github.com/Belphemur/AddictedProxy/compare/v2.11.14...v2.11.15) (2022-07-23)


### Enhancements

* **analytics:** Use matomo ([99326f6](https://github.com/Belphemur/AddictedProxy/commit/99326f637035514184823fde9bb130293c479993))

## [2.11.14](https://github.com/Belphemur/AddictedProxy/compare/v2.11.13...v2.11.14) (2022-07-23)


### Enhancements

* **analytics:** track page view ([d4f72a3](https://github.com/Belphemur/AddictedProxy/commit/d4f72a34b98f1e66fc170be6f9fd62f579a41ed9))
* **analytics:** use GTM ([1475748](https://github.com/Belphemur/AddictedProxy/commit/14757488258482355d55d9b5a1f0b4202dde3dec))

## [2.11.13](https://github.com/Belphemur/AddictedProxy/compare/v2.11.12...v2.11.13) (2022-07-18)


### Bug Fixes

* **API:** Use the dist bundle ([ec1e6b4](https://github.com/Belphemur/AddictedProxy/commit/ec1e6b4afe6c91c4d3e856b2bc50a87f3aca465c))

## [2.11.12](https://github.com/Belphemur/AddictedProxy/compare/v2.11.11...v2.11.12) (2022-07-17)


### Bug Fixes

* **sentry:** fix sentry release workflow ([90a84dd](https://github.com/Belphemur/AddictedProxy/commit/90a84dd9bcd5da771eb28ba364d99e3db708a43c))
* **Sentry:** split sentry from main release ([8e881d1](https://github.com/Belphemur/AddictedProxy/commit/8e881d182730a09d65ec86bb25055ddc6373012e))

## [2.11.11](https://github.com/Belphemur/AddictedProxy/compare/v2.11.10...v2.11.11) (2022-07-17)


### Bug Fixes

* **Menu:** Fix the order of items in the menu ([26bbd94](https://github.com/Belphemur/AddictedProxy/commit/26bbd94146b39b6dbcb8c89660ea87bd5ab6736d))


### Enhancements

* **Stats:** Add loading skeleton ([bab29ec](https://github.com/Belphemur/AddictedProxy/commit/bab29ec08539cf024c07cac2820a889239395a8f))

## [2.11.10](https://github.com/Belphemur/AddictedProxy/compare/v2.11.9...v2.11.10) (2022-07-17)


### Bug Fixes

* **FetchSubtitle:** Fix issue where the span/transaction isn't tagged as failed when an exception arise. ([84c35b4](https://github.com/Belphemur/AddictedProxy/commit/84c35b4fe54c05ea380eb4aeb2a163947150aec6))
* **FetchSubtitle:** Since jobs are now queued, we should reload the show to be sure we have the right data. ([b62bc14](https://github.com/Belphemur/AddictedProxy/commit/b62bc14312ab93a95ef7c13f9bf9756def3cb475))


### Enhancements

* **Season::Refresh:** Only refresh seasons when we can't find the requested one. ([d4b552a](https://github.com/Belphemur/AddictedProxy/commit/d4b552a0d59e72187f7543bf80f79dcdbd7fc635))
* **UserAgent:** Add Safari ([98cc29f](https://github.com/Belphemur/AddictedProxy/commit/98cc29f2f27ea520c86f46ad8fa5a91cd1758caa))

## [2.11.9](https://github.com/Belphemur/AddictedProxy/compare/v2.11.8...v2.11.9) (2022-07-15)


### Bug Fixes

* **CircuitBreaker:** Remove the circuit breaker, we have enough retries to not need that. ([420b76f](https://github.com/Belphemur/AddictedProxy/commit/420b76f940a800d67adde99baf6210fd3ad1ffbb))

## [2.11.8](https://github.com/Belphemur/AddictedProxy/compare/v2.11.7...v2.11.8) (2022-07-13)


### Bug Fixes

* **FetchSubtitle:** Use a queue for the jobs ([01c0d8d](https://github.com/Belphemur/AddictedProxy/commit/01c0d8d665f9e45d1d664362249e9656408a70a8))

## [2.11.7](https://github.com/Belphemur/AddictedProxy/compare/v2.11.6...v2.11.7) (2022-07-12)


### Bug Fixes

* **Locking:** Fix deprecation of method ([a030c01](https://github.com/Belphemur/AddictedProxy/commit/a030c017cb3ca807ec0912ca78d6126b216df862))


### Enhancements

* **Backoff:** Use an improved version of backoff ([fdd0775](https://github.com/Belphemur/AddictedProxy/commit/fdd07757d74fa83b297f0a36235c2792185076c6))

## [2.11.6](https://github.com/Belphemur/AddictedProxy/compare/v2.11.5...v2.11.6) (2022-07-12)


### Enhancements

* **logs:** reenable logs ([6b497b5](https://github.com/Belphemur/AddictedProxy/commit/6b497b507847afe1348411128841ffb83e6cf0c6))

## [2.11.5](https://github.com/Belphemur/AddictedProxy/compare/v2.11.4...v2.11.5) (2022-07-12)


### Bug Fixes

* **Timeout:** Fix issue with downloading ([9b3a6e2](https://github.com/Belphemur/AddictedProxy/commit/9b3a6e2a35c1a739002c37392cf131c28cea72bc))


### Enhancements

* **Timeout:** Improve timeout and waiting to retry ([41c64ee](https://github.com/Belphemur/AddictedProxy/commit/41c64eecaa65a2024c7980efbeff96577f719436))
* **Timeout:** Improve timeout and waiting to retry ([278f89b](https://github.com/Belphemur/AddictedProxy/commit/278f89baf958c07c6f008c9ff5da402065aecd7e))

## [2.11.4](https://github.com/Belphemur/AddictedProxy/compare/v2.11.3...v2.11.4) (2022-07-12)


### Enhancements

* **Retry:** Improve retries strategy and add circuit breaker ([dc884ec](https://github.com/Belphemur/AddictedProxy/commit/dc884ec04cb351cb3248127d62e4d7a74782684f))

## [2.11.3](https://github.com/Belphemur/AddictedProxy/compare/v2.11.2...v2.11.3) (2022-07-12)


### Bug Fixes

* **HTTP:** Fix timing for HTTP calls to Addic7ed ([2e3272c](https://github.com/Belphemur/AddictedProxy/commit/2e3272c24b4c3950fb931ed65158166ee3cfae91))

## [2.11.2](https://github.com/Belphemur/AddictedProxy/compare/v2.11.1...v2.11.2) (2022-07-12)


### Enhancements

* **UserAgents:** Proper generation of valid user agents ([8bfcfef](https://github.com/Belphemur/AddictedProxy/commit/8bfcfef57f2b8de30a2d0b85d2402df0369ccbbc))

## [2.11.1](https://github.com/Belphemur/AddictedProxy/compare/v2.11.0...v2.11.1) (2022-07-12)


### Enhancements

* **Logging:** Move prod logging to warning ([dced8fd](https://github.com/Belphemur/AddictedProxy/commit/dced8fdfcc6ad1b54b29da354a6c16e99af2e466))

## [2.11.0](https://github.com/Belphemur/AddictedProxy/compare/v2.10.10...v2.11.0) (2022-07-12)


### Features

* **Popularity::Downloads:** Add top show by downloads ([04fb846](https://github.com/Belphemur/AddictedProxy/commit/04fb8465f76b5a5bdb13a1d62f307f87cb166de3))
* **Stats::Downloads:** Add downloads stats to the front-end ([5eb4e02](https://github.com/Belphemur/AddictedProxy/commit/5eb4e0288230b8150277ab29aa27f020c3810477))

## [2.10.10](https://github.com/Belphemur/AddictedProxy/compare/v2.10.9...v2.10.10) (2022-07-12)


### Bug Fixes

* **Fetch::Subtitle:** Increase max runtime to fetch subtitles. ([62656f8](https://github.com/Belphemur/AddictedProxy/commit/62656f8461b55fecdca3fa40969904b7dc79a6e0))

## [2.10.9](https://github.com/Belphemur/AddictedProxy/compare/v2.10.8...v2.10.9) (2022-07-11)


### Enhancements

* **HTTP::perf:** Implement retries properly ([044c6e0](https://github.com/Belphemur/AddictedProxy/commit/044c6e069068cf0182c18e7f37d95bd2607fd492))

## [2.10.8](https://github.com/Belphemur/AddictedProxy/compare/v2.10.7...v2.10.8) (2022-07-11)


### Enhancements

* **HTTP::perf:** Retry automatically on timeout. ([bf2c11c](https://github.com/Belphemur/AddictedProxy/commit/bf2c11c597604c637404862fa9f8c1ccadec848f))

## [2.10.7](https://github.com/Belphemur/AddictedProxy/compare/v2.10.6...v2.10.7) (2022-07-11)


### Enhancements

* **HTTP::perf:** Improve HTTP quering performance ([9e5bf3f](https://github.com/Belphemur/AddictedProxy/commit/9e5bf3fdc4023d91a66176008aaa92a344263f10))

## [2.10.6](https://github.com/Belphemur/AddictedProxy/compare/v2.10.5...v2.10.6) (2022-07-07)


### Bug Fixes

* **Download:** Fix issue with retrying download in case of HTTP Exception ([51e1815](https://github.com/Belphemur/AddictedProxy/commit/51e1815e5c97bf976c9522491b4c25b1d49674a6)), closes [#ADDICTEDPROXY-3](https://github.com/Belphemur/AddictedProxy/issues/ADDICTEDPROXY-3)

## [2.10.5](https://github.com/Belphemur/AddictedProxy/compare/v2.10.4...v2.10.5) (2022-07-06)


### Bug Fixes

* **Vite:** Fix config of Vite ([2518b01](https://github.com/Belphemur/AddictedProxy/commit/2518b019ca28482af69dae9053c853a8acdd5ac4))

## [2.10.4](https://github.com/Belphemur/AddictedProxy/compare/v2.10.3...v2.10.4) (2022-07-06)


### Bug Fixes

* **Api:** Fix issue with the api code gen ([224dc19](https://github.com/Belphemur/AddictedProxy/commit/224dc19e3d48c517e46772c74473b6fbd710a1fd))

## [2.10.3](https://github.com/Belphemur/AddictedProxy/compare/v2.10.2...v2.10.3) (2022-07-06)


### Bug Fixes

* **frontend::download:** Fix downloading subtitle with StreamSaver ([3187e98](https://github.com/Belphemur/AddictedProxy/commit/3187e98508606634017ebe25f9c8da9440acd4d3))
* **SideBar:** Be sure the dark/light theme toggle is at the right place ([3af8f28](https://github.com/Belphemur/AddictedProxy/commit/3af8f28fd2169e8f3bd5ef287013209e14e89247))


### Enhancements

* **Vite:** Move to vite for the front-end ([ba034a1](https://github.com/Belphemur/AddictedProxy/commit/ba034a16bf835a0d01cde2a79f3ddb9e81309a8e))

## [2.10.2](https://github.com/Belphemur/AddictedProxy/compare/v2.10.1...v2.10.2) (2022-07-04)


### Enhancements

* **Perf:** reduce sampling for performance tracking ([5e25265](https://github.com/Belphemur/AddictedProxy/commit/5e25265692b11ff753c48b21e032f46e929786e1))

## [2.10.1](https://github.com/Belphemur/AddictedProxy/compare/v2.10.0...v2.10.1) (2022-07-04)


### Bug Fixes

* **Download:** Fix issue where download can't copy the stream. ([3d3f428](https://github.com/Belphemur/AddictedProxy/commit/3d3f428b7f6be35f7a8d6785806647f40ffe8ff1)), closes [#ADDICTEDPROXY-37](https://github.com/Belphemur/AddictedProxy/issues/ADDICTEDPROXY-37)

## [2.10.0](https://github.com/Belphemur/AddictedProxy/compare/v2.9.6...v2.10.0) (2022-06-17)


### Features

* **Show::Priority:** Set a priority for show to avoid matching wrong shows ([3886442](https://github.com/Belphemur/AddictedProxy/commit/38864423de4417fbbdf4dfda3568ff78f6ac6c5b))

## [2.9.6](https://github.com/Belphemur/AddictedProxy/compare/v2.9.5...v2.9.6) (2022-06-16)


### Enhancements

* **Creds:** Save in bulk the download creds ([e655840](https://github.com/Belphemur/AddictedProxy/commit/e655840e39e412a01ea6057f7e1d9c03b558a6a3))
* **Version:** Add endpoint to get version ([20c47cd](https://github.com/Belphemur/AddictedProxy/commit/20c47cd7e4caffa7d483948dc5e9411a4e8099ae))

## [2.9.5](https://github.com/Belphemur/AddictedProxy/compare/v2.9.4...v2.9.5) (2022-06-13)


### Bug Fixes

* **Download:** Add auto retry downloading subtitle ([d3551cb](https://github.com/Belphemur/AddictedProxy/commit/d3551cb60d4f34119d76501249ee706d4cf55dc8))

## [2.9.4](https://github.com/Belphemur/AddictedProxy/compare/v2.9.3...v2.9.4) (2022-06-12)


### Bug Fixes

* **S3Storage:** Can't find the subtitle ([dd42c78](https://github.com/Belphemur/AddictedProxy/commit/dd42c7846421e37c2a8eb2fd67180b0681f6bd57))

## [2.9.3](https://github.com/Belphemur/AddictedProxy/compare/v2.9.2...v2.9.3) (2022-06-11)


### Bug Fixes

* **Frontend::Router:** Issue with order of route ([d5cb7fa](https://github.com/Belphemur/AddictedProxy/commit/d5cb7fa80f1fc1351b1846d8f891797c8ea5f2dd))
* **Middleware:** Don't use weird middleware to track subtitle download ([58efe87](https://github.com/Belphemur/AddictedProxy/commit/58efe878321462ce678f8d4eadb8678ab5b7a5bc))


### Enhancements

* **PerformanceTracking:** Not much transactions for now so we can keep the performance sample rate at 1 ([5612633](https://github.com/Belphemur/AddictedProxy/commit/56126334d76d9f81e3aa1f207f00b639c1631552))
* **RateLimiting:** Be less limiting ([c0cd412](https://github.com/Belphemur/AddictedProxy/commit/c0cd4126df2d0efcdb67b6b0d1c617e281f90410))

## [2.9.2](https://github.com/Belphemur/AddictedProxy/compare/v2.9.1...v2.9.2) (2022-06-10)


### Enhancements

* **DownwloadCreds:** Set a proper max runtime to avoid lingering job. ([e9361d8](https://github.com/Belphemur/AddictedProxy/commit/e9361d84f176d587f0263785ab9944f447f1aac8))
* **Logging:** Improve logging of redeeming download creds ([64b4da0](https://github.com/Belphemur/AddictedProxy/commit/64b4da05dd1eb17a36addc8ed7375b5d5625912a))

## [2.9.1](https://github.com/Belphemur/AddictedProxy/compare/v2.9.0...v2.9.1) (2022-06-07)


### Enhancements

* **DownloadCount:** Always increment the download count even when response comes from cache ([5baad0f](https://github.com/Belphemur/AddictedProxy/commit/5baad0f87e77a1c5e4805bcb73aa2ed360417213))
* **RateLimiting:** Be less restrictive on the rate limiting ([22fca37](https://github.com/Belphemur/AddictedProxy/commit/22fca37c00812efa27b2cde70f9726333d84e779))

## [2.9.0](https://github.com/Belphemur/AddictedProxy/compare/v2.8.5...v2.9.0) (2022-06-03)


### Features

* **Bootstrap::Conditional:** Add conditional bootstrap ([70ffcab](https://github.com/Belphemur/AddictedProxy/commit/70ffcabfeb955c78f83ae83e3cfc9c6598502c14))
* **Storage::S3:** Add support for S3 ([1384b74](https://github.com/Belphemur/AddictedProxy/commit/1384b7429393c90fbef145341ebf66430170a06a))

## [2.8.5](https://github.com/Belphemur/AddictedProxy/compare/v2.8.4...v2.8.5) (2022-06-02)


### Enhancements

* **Caching:** Add proper information for caching ([e547a64](https://github.com/Belphemur/AddictedProxy/commit/e547a6441636a0744695b16e7d758fe1c92d6043))

## [2.8.4](https://github.com/Belphemur/AddictedProxy/compare/v2.8.3...v2.8.4) (2022-06-02)


### Bug Fixes

* **CoSign:** Fix signing images ([ec7dfdb](https://github.com/Belphemur/AddictedProxy/commit/ec7dfdbf25dfbda6c4b8165553ef9b8b99bf8b5d))

## [2.8.3](https://github.com/Belphemur/AddictedProxy/compare/v2.8.2...v2.8.3) (2022-06-02)


### Enhancements

* **Parsing:** Update angle sharp ([71eb40e](https://github.com/Belphemur/AddictedProxy/commit/71eb40eee4ecefa47cfac948847efdf190f1eb62))

## [2.8.2](https://github.com/Belphemur/AddictedProxy/compare/v2.8.1...v2.8.2) (2022-06-02)


### Enhancements

* **Jobs:** Don't let jobs run forever, put a max runtime to them ([9b3ff0a](https://github.com/Belphemur/AddictedProxy/commit/9b3ff0ae4eff4ebb3933a35c3ddf1d082c771c55))
* **Performance::Tracking:** Improve the way we deal with the span to track performance ([be9618c](https://github.com/Belphemur/AddictedProxy/commit/be9618cefd84021f081ca284391e7599a1886dbf))

## [2.8.1](https://github.com/Belphemur/AddictedProxy/compare/v2.8.0...v2.8.1) (2022-05-31)


### Bug Fixes

* **Database:** No need for pragma busy_timeout. The CommandTimeout is a better pattern that already take care of the different retries strategies when the database is locked. ([a2d90a0](https://github.com/Belphemur/AddictedProxy/commit/a2d90a02c8785854648547f8d7c823b8ff835099))

## [2.8.0](https://github.com/Belphemur/AddictedProxy/compare/v2.7.7...v2.8.0) (2022-05-31)


### Features

* **Caching::Subtitle:** Cache subtitle file in memory for 24h ([b4deb6b](https://github.com/Belphemur/AddictedProxy/commit/b4deb6b1b656511707d3d5b1269c07d66af6fda1))
* **Response::Caching:** Add proper caching ([ae617a2](https://github.com/Belphemur/AddictedProxy/commit/ae617a257be2586e22be8c18b0c988ae36a55e39))

## [2.7.7](https://github.com/Belphemur/AddictedProxy/compare/v2.7.6...v2.7.7) (2022-05-31)


### Bug Fixes

* **Database::Deadlock:** Fix keeping the transaction way too long ([c883fdd](https://github.com/Belphemur/AddictedProxy/commit/c883fddb5075cee17bc1dc3d33d8224627ede6a2)), closes [#87](https://github.com/Belphemur/AddictedProxy/issues/87)

## [2.7.6](https://github.com/Belphemur/AddictedProxy/compare/v2.7.5...v2.7.6) (2022-05-30)


### Bug Fixes

* **Database:** Only register once the callback on the connection ([d226b6a](https://github.com/Belphemur/AddictedProxy/commit/d226b6a8aa4fa4966f059d8da8038f143f7f0b68))

### [2.7.5](https://github.com/Belphemur/AddictedProxy/compare/v2.7.4...v2.7.5) (2022-05-30)


### Bug Fixes

* **DB:** Fix database locked issue ([f169843](https://github.com/Belphemur/AddictedProxy/commit/f169843f192f1d14bd1951320520fd881c6413ff)), closes [#85](https://github.com/Belphemur/AddictedProxy/issues/85)

### [2.7.4](https://github.com/Belphemur/AddictedProxy/compare/v2.7.3...v2.7.4) (2022-05-29)


### Bug Fixes

* **Creds:** Fix issue where not finding credentials throw exception. ([f8a26de](https://github.com/Belphemur/AddictedProxy/commit/f8a26de44e9fa0f68c501383faa3c0be6f6112b8)), closes [#1](https://github.com/Belphemur/AddictedProxy/issues/1)


### Enhancements

* **Creds::DownloadExceeded:** Improve the redeem logic of credentials ([658dcd0](https://github.com/Belphemur/AddictedProxy/commit/658dcd066895f090c84bf9587354f39f8771216f))
* **RateLimiting:** Accept more request per hours ([a76c2ba](https://github.com/Belphemur/AddictedProxy/commit/a76c2ba2ac9e0a1011832024df9934ba5d8f5c1c))

### [2.7.3](https://github.com/Belphemur/AddictedProxy/compare/v2.7.2...v2.7.3) (2022-05-28)


### Bug Fixes

* **Uplink:** Catch when we can't find the subtitle in uplink ([8ab327c](https://github.com/Belphemur/AddictedProxy/commit/8ab327cb504152a67000d3c4fcfcd6da79024e63))

### [2.7.2](https://github.com/Belphemur/AddictedProxy/compare/v2.7.1...v2.7.2) (2022-05-28)


### Bug Fixes

* **Download:** Don't get the min from all the table, only the selectable creds ([f19b35b](https://github.com/Belphemur/AddictedProxy/commit/f19b35bb84b1743ef318f0496374407ad7ee5541))

### [2.7.1](https://github.com/Belphemur/AddictedProxy/compare/v2.7.0...v2.7.1) (2022-05-28)


### Bug Fixes

* **Download:** Add missing referer ([309a074](https://github.com/Belphemur/AddictedProxy/commit/309a07433dfba956a874c8a2364cc5675973c862))
* **Download:** Fallback downloading without credentials relying on the proxy ([5ce3886](https://github.com/Belphemur/AddictedProxy/commit/5ce38862d5e2b1e1d878cbe2c064ab31a6ff244b))
* **Download:** Only tag as exceeded if a credential was used. ([a91a187](https://github.com/Belphemur/AddictedProxy/commit/a91a1876188b432234439cf932c57515ecb08b55))

## [2.7.0](https://github.com/Belphemur/AddictedProxy/compare/v2.6.1...v2.7.0) (2022-05-28)


### Features

* **Creds::Download:** Auto redeem credentials after 24h ([4c469d3](https://github.com/Belphemur/AddictedProxy/commit/4c469d35e04aceeb0df9972c665c646643d93d36))


### Enhancements

* **Creds:** Tag credentials as unable to be used for downloading subtitle ([b998295](https://github.com/Belphemur/AddictedProxy/commit/b998295ea18f38f70d55a2dee38710e9a5cf7c69))
* **RateLimiting:** Make the rate limiting less limiting ([fc4602b](https://github.com/Belphemur/AddictedProxy/commit/fc4602be3d742253e6f1765c8b0a73fad4a50812))

### [2.6.1](https://github.com/Belphemur/AddictedProxy/compare/v2.6.0...v2.6.1) (2022-05-28)


### Bug Fixes

* **SQLite:** Fix locking issue with litestream ([2f61a43](https://github.com/Belphemur/AddictedProxy/commit/2f61a43dc67df53dfe77cf19d5ec3eb541bafe91))

## [2.6.0](https://github.com/Belphemur/AddictedProxy/compare/v2.5.6...v2.6.0) (2022-05-28)


### Features

* **Creds::Download:** Add download usage to creds ([12bda16](https://github.com/Belphemur/AddictedProxy/commit/12bda1685d9ce65481196711b3ae800e3f00517b))
* **Creds::Download:** Use the least used credential for download ([93ca3b8](https://github.com/Belphemur/AddictedProxy/commit/93ca3b860c02304de786aee7da0c10ed532db327))


### Bug Fixes

* **Sentry:** Don't log task cancelled exceptions ([4ed6d82](https://github.com/Belphemur/AddictedProxy/commit/4ed6d82feffdb288c3efa49de2253f2b87bf7a1e))

### [2.5.6](https://github.com/Belphemur/AddictedProxy/compare/v2.5.5...v2.5.6) (2022-05-27)


### Enhancements

* **Search:** Avoid refreshing show so often when we know we won't do anything since it's not time to refresh the show data ([4f87636](https://github.com/Belphemur/AddictedProxy/commit/4f8763666695659de744dc33e974cf137bc2bc48))

### [2.5.5](https://github.com/Belphemur/AddictedProxy/compare/v2.5.4...v2.5.5) (2022-05-26)


### Bug Fixes

* **RateLimiting:** Use the proper header of cloudflare ([802e2a1](https://github.com/Belphemur/AddictedProxy/commit/802e2a1ce4ee7e0837976e60044b328d76e06bcf))

### [2.5.4](https://github.com/Belphemur/AddictedProxy/compare/v2.5.3...v2.5.4) (2022-05-26)


### Enhancements

* **Refresh::Timing:** Increase time to wait before refreshing data ([849b85c](https://github.com/Belphemur/AddictedProxy/commit/849b85cfdd58df07cf2a5323ee17119519f24cfb))
* **Season:** Improve when to refresh season ([e79f73c](https://github.com/Belphemur/AddictedProxy/commit/e79f73cc13ac925d4d36beeb9f8547e7a552c7e3))

### [2.5.3](https://github.com/Belphemur/AddictedProxy/compare/v2.5.2...v2.5.3) (2022-05-26)


### Enhancements

* **Performance:** Improve refresh episode performance tracking ([6d0b89e](https://github.com/Belphemur/AddictedProxy/commit/6d0b89e7d4cb7c17403206b1bc4bff5d6f2c10a4))
* **Refresh:** When manually refreshing, refresh all season ([bfeb1cf](https://github.com/Belphemur/AddictedProxy/commit/bfeb1cffcafbbd95fe94c1081b5112e44c990afa))
* **Tracing:** Improve tracing of Show Refresher ([daec003](https://github.com/Belphemur/AddictedProxy/commit/daec0030c1f7250a9a58d95da3a73ebefeaf5ec6))

### [2.5.2](https://github.com/Belphemur/AddictedProxy/compare/v2.5.1...v2.5.2) (2022-05-25)


### Bug Fixes

* **Performance:** Fix wrong transaction used for tracing ([cc79adb](https://github.com/Belphemur/AddictedProxy/commit/cc79adb0a0d8a753699b65fa892176f4e6544ebe))

### [2.5.1](https://github.com/Belphemur/AddictedProxy/compare/v2.5.0...v2.5.1) (2022-05-25)


### Bug Fixes

* **performance:** Use the IHub from sentry ([91de2b3](https://github.com/Belphemur/AddictedProxy/commit/91de2b3b607ac13c8c93d5fc5a8200fdb55c4e8b))
* **Subtitle:** Fix the uniqueness on wrong column ([8b4751a](https://github.com/Belphemur/AddictedProxy/commit/8b4751a1f316b9f66f98441f600fc2d41b681909))
* **Tracing:** Remove details from main transaction ([12c534d](https://github.com/Belphemur/AddictedProxy/commit/12c534da45aa1a7b68f34dba9e26c2807d347ea0))


### Enhancements

* **Logging:** Add log for any search done ([5176deb](https://github.com/Belphemur/AddictedProxy/commit/5176debe08dd07fafcc15ebf7f81d730e22abe57))

## [2.5.0](https://github.com/Belphemur/AddictedProxy/compare/v2.4.2...v2.5.0) (2022-05-25)


### Features

* **Performance:** Add performance library ([44cf096](https://github.com/Belphemur/AddictedProxy/commit/44cf09616c3f8f389580c35647c46bce4220fdc5))


### Bug Fixes

* **Download:** Fix possible crash when Addic7ed doesn't return a subtitle ([d0167b2](https://github.com/Belphemur/AddictedProxy/commit/d0167b2794b2e6e085d5bead51bce1ec2cc7566f))


### Enhancements

* **Performance:** Add concept of nesting transaction ([f920952](https://github.com/Belphemur/AddictedProxy/commit/f920952428e3105cb58d6c8a387867859f3668ce))
* **Performance:** Proper tracking of refresh ([d627c15](https://github.com/Belphemur/AddictedProxy/commit/d627c15924b41e557e1bd0f40b3a695f374f4e52))
* **Performance:** Simplify contract to only deal with Span ([9abed79](https://github.com/Belphemur/AddictedProxy/commit/9abed795ef1f7d6c0820746ee0bd142cb84042e3))
* **Performance:** Track performance of the different jobs ([9468e42](https://github.com/Belphemur/AddictedProxy/commit/9468e42176a35c18baa9bde0571ce70f943008dc))
* **Performance:** Use using to be sure the object is disposed ([5c10709](https://github.com/Belphemur/AddictedProxy/commit/5c10709107c47e74670553ba729820f5d9c15ace))

### [2.4.2](https://github.com/Belphemur/AddictedProxy/compare/v2.4.1...v2.4.2) (2022-05-24)


### Bug Fixes

* **Episode::Parsing:** Fix grouping of subtitle by unique episode ([3c9f437](https://github.com/Belphemur/AddictedProxy/commit/3c9f43714ae88f3495a2f1d8c0743e276943b23e))

### [2.4.1](https://github.com/Belphemur/AddictedProxy/compare/v2.4.0...v2.4.1) (2022-05-24)


### Bug Fixes

* **Episode::GetEpisodes:** Don't track getting episode for UI ([f652e4b](https://github.com/Belphemur/AddictedProxy/commit/f652e4bdba60d726a4ba98a8bfa955ccde17f740))
* **Episode::Refresh:** Fix the code that refresh the episode ([83b82c0](https://github.com/Belphemur/AddictedProxy/commit/83b82c01deb7ba9fc2a0a9c1aa74c09e32312feb))
* **Logging:** Only log inner exception ([068746e](https://github.com/Belphemur/AddictedProxy/commit/068746e6e01cdc234834cb9b91bde4d508fb5e61))

## [2.4.0](https://github.com/Belphemur/AddictedProxy/compare/v2.3.0...v2.4.0) (2022-05-24)


### Features

* **TransactionManager:** add manager for transaction ([a81b53c](https://github.com/Belphemur/AddictedProxy/commit/a81b53cceefb1cc4dc28739af2c379255c531b5c))


### Bug Fixes

* **Episode::Id:** Fix conflict on Id when bulk insert ([e0babdd](https://github.com/Belphemur/AddictedProxy/commit/e0babddb70393729df98d1ba29e1deba54bc7247))
* **Transaction:** Be sure to end the transaction if exception is triggered ([afea8fe](https://github.com/Belphemur/AddictedProxy/commit/afea8fef2211f2fb539dd060424267d1ecb775c1))

## [2.3.0](https://github.com/Belphemur/AddictedProxy/compare/v2.2.0...v2.3.0) (2022-05-20)


### Features

* **Search::API:** Return a 423 when the data isn't available yet for the episode ([6bbd519](https://github.com/Belphemur/AddictedProxy/commit/6bbd519a34bd39b0ce225ca2cd9918706713ec7f))

## [2.2.0](https://github.com/Belphemur/AddictedProxy/compare/v2.1.7...v2.2.0) (2022-05-19)


### Features

* **Search:** Automatically select latest season ([62778ee](https://github.com/Belphemur/AddictedProxy/commit/62778eea46d1f031228dab6419a51ac7ced47ceb))

### [2.1.7](https://github.com/Belphemur/AddictedProxy/compare/v2.1.6...v2.1.7) (2022-05-17)


### Enhancements

* **SEO:** Have proper SEO meta ([c6c4c68](https://github.com/Belphemur/AddictedProxy/commit/c6c4c682252843b5cfdafc4388dc71dbe5777d6d))

### [2.1.6](https://github.com/Belphemur/AddictedProxy/compare/v2.1.5...v2.1.6) (2022-05-17)


### Bug Fixes

* **API:** Add documentation for API server ([8d08c4d](https://github.com/Belphemur/AddictedProxy/commit/8d08c4d1bb593219e6c930f67aac5de4b3a63eae))

### [2.1.5](https://github.com/Belphemur/AddictedProxy/compare/v2.1.4...v2.1.5) (2022-05-17)


### Enhancements

* **Icon:** Use icon of element-plus to reduce size of bundle ([95d0a9c](https://github.com/Belphemur/AddictedProxy/commit/95d0a9c6cebb0773fe98418b12c781010728fb3a))
* **logging:** add logging the search query done on show ([8c95836](https://github.com/Belphemur/AddictedProxy/commit/8c9583680f79bba42fa1b453a2a6da2cf121f593))

### [2.1.4](https://github.com/Belphemur/AddictedProxy/compare/v2.1.3...v2.1.4) (2022-05-17)


### Enhancements

* **Backoff:** Proper backoff strategy when addicted doesn't answer ([de5bfe0](https://github.com/Belphemur/AddictedProxy/commit/de5bfe073eb3d13f19a5c646e476b65148660377))

### [2.1.3](https://github.com/Belphemur/AddictedProxy/compare/v2.1.2...v2.1.3) (2022-05-17)


### Bug Fixes

* **log:** add proper logging for websocket rate limited ([def3b04](https://github.com/Belphemur/AddictedProxy/commit/def3b04b176f489a408f7057e24cfb98fa1766c8))


### Enhancements

* **Performance:** Add configurable performance sample rate ([260dcfa](https://github.com/Belphemur/AddictedProxy/commit/260dcfa8cdb2b62f3cf32bda88e37895832f520f))
* **Subtitle:** Improve performance of getting subtitles of a show by giving the language to the db ([ccb817e](https://github.com/Belphemur/AddictedProxy/commit/ccb817ed082ddfbd1b150229516494c1db71aecd))

### [2.1.2](https://github.com/Belphemur/AddictedProxy/compare/v2.1.1...v2.1.2) (2022-05-17)


### Bug Fixes

* **cors:** Have proper cors in place ([ba2a98d](https://github.com/Belphemur/AddictedProxy/commit/ba2a98dec4f7428e2c20c24ec48cf2faf50e2f01))

### [2.1.1](https://github.com/Belphemur/AddictedProxy/compare/v2.1.0...v2.1.1) (2022-05-17)


### Bug Fixes

* **CI:** Fix building frontend image ([919fb39](https://github.com/Belphemur/AddictedProxy/commit/919fb3990c1b81839fd248b741cb83a3189cfc93))

## [2.1.0](https://github.com/Belphemur/AddictedProxy/compare/v2.0.0...v2.1.0) (2022-05-17)


### Features

* **ApiDoc:** Have the api documentation in the Single Page App ([29d6a50](https://github.com/Belphemur/AddictedProxy/commit/29d6a50622513fd0efc970a535525556adf5d12f))
* **background:** Add nice background to the application ([428e8b9](https://github.com/Belphemur/AddictedProxy/commit/428e8b93cfbea9bf1bd106a3bbeacc777e6fe290))
* **Frontend:** Add logo and dark mode ([6d4f24e](https://github.com/Belphemur/AddictedProxy/commit/6d4f24e0536e6a608a5098ba4d2cdeede35b427f))
* **Hub::frontend:** Add frontend hub for refresh ([7732950](https://github.com/Belphemur/AddictedProxy/commit/77329506000a7e245dc37a737701d5b0d6b68330))
* **Hub::frontend:** Use the hub to send refresh request ([311306b](https://github.com/Belphemur/AddictedProxy/commit/311306bd6f173ca0f05e2ccb2f4b1e2fd7e9d8a5))
* **Hub::Refresh::Unsubscribe:** Be able to unsubscribe from specific show refresh ([bdd3f3b](https://github.com/Belphemur/AddictedProxy/commit/bdd3f3bec555801e94fbc28d0c127bf7655f8275))
* **Hub:** Add websocket to be able to interact with refreshing show ([3a6b586](https://github.com/Belphemur/AddictedProxy/commit/3a6b586a40faaa32e43c24841c5a1108342fbb9d))
* **language:** save the language in local storage ([475758b](https://github.com/Belphemur/AddictedProxy/commit/475758b0347e9da53862278d32421313fc5e1561))
* **Page::Title:** Add proper page title depending on the route ([8a4aaeb](https://github.com/Belphemur/AddictedProxy/commit/8a4aaebc46cf501fdf1181cb92165da49be8b6aa))
* **Refresh::search:** The user can now redo their search when the refresh is done ([5f3e214](https://github.com/Belphemur/AddictedProxy/commit/5f3e21458227b6efb6b16fe2a514d9928e9d1c96))
* **Refresh:** Only refresh the new season and the latest one ([d9c6e07](https://github.com/Belphemur/AddictedProxy/commit/d9c6e071caf591387e2b786b93bbeb442f74d8b0))
* **Refresh:** Show all the current refresh in the UI ([adf5c68](https://github.com/Belphemur/AddictedProxy/commit/adf5c6817e9a9076f08198828c4291ecb6454ff9))
* **Subtitle::frontend:** Add subtitle table with search result ([5bfbfc6](https://github.com/Belphemur/AddictedProxy/commit/5bfbfc68800195551e369730db32771a20b71ae6))
* **Subtitle:Download:** Be able to download the subtitle from the UI ([7f1e5e4](https://github.com/Belphemur/AddictedProxy/commit/7f1e5e427e833e564cd1e4dd326dd50f00e3c89c))
* **Title:** Proper title for the name of the show and season on the page ([a828c01](https://github.com/Belphemur/AddictedProxy/commit/a828c01b91fb5957c9ba2a7590d2f75ac011dca2))


### Bug Fixes

* **config:** Be sure the config has the right api path ([6e3d84e](https://github.com/Belphemur/AddictedProxy/commit/6e3d84ebd2bc7ccdc7678b0436f26273116280f8))
* **CORS:** Add missing header for JS ([64c2988](https://github.com/Belphemur/AddictedProxy/commit/64c2988df40c49f8b047e664182caa5ad6f698f4))
* **Cors:** Be sure swagger have proper cors applied to it ([a044cbc](https://github.com/Belphemur/AddictedProxy/commit/a044cbc485287a1432ca7a495063e53cfbf06d07))
* **etag:** Be sure the etag of subtitle is implemented properly ([9491b63](https://github.com/Belphemur/AddictedProxy/commit/9491b63b3dcbe4c4b937d440a0cbcafa0e30cafc))
* **Hub::Progress:** Send progress as a single object ([2322155](https://github.com/Belphemur/AddictedProxy/commit/2322155c1da7510d77178b26040c2de6488c2b06))
* **Hub::Show:** Return the right show to the Hub ([93a5d03](https://github.com/Belphemur/AddictedProxy/commit/93a5d0350a8643afe1f03cd8512cabf283941e6f))
* **Hub:** Add proper way to register the hub and send message to it ([537736a](https://github.com/Belphemur/AddictedProxy/commit/537736a8583a66ceb3eaa0e61e6c3662901ed093))
* **hub:** Hub don't support cancellation token in their call ([ce58cb2](https://github.com/Belphemur/AddictedProxy/commit/ce58cb28ec9f5984b043cb415521e67fc7975fa2))
* **menu:** Fix background color ([a9a9530](https://github.com/Belphemur/AddictedProxy/commit/a9a9530db9c77947bb760b490318de4d334daf51))
* **migration:** Always run the migrations ([e257898](https://github.com/Belphemur/AddictedProxy/commit/e2578985f0460133671a72e1c2ba36983d794d9b))
* **Parser:** Fix the parser for change of language of the table ([7edbd29](https://github.com/Belphemur/AddictedProxy/commit/7edbd291f73a741c0954f47139c9b6f6c22e72f7))
* **Parser:** Make the parser future-proof ([442adcb](https://github.com/Belphemur/AddictedProxy/commit/442adcb7df7f95bec5efb382b185e0aab210b014))
* **Refresh::Progress:** Be sure the progress is set to 100 when finishing processing show ([ecdef1f](https://github.com/Belphemur/AddictedProxy/commit/ecdef1fa2de74442c4619729dc0302ceca3b711a))
* **Refresh::Progress:** Use interpolation to calculate the progress ([6e1a41a](https://github.com/Belphemur/AddictedProxy/commit/6e1a41a4c13089f55daae4cc328726ba15c1d913))
* **Refresh:** Fix possible division by 0 ([07e7302](https://github.com/Belphemur/AddictedProxy/commit/07e7302846ea557d62204778e951ea25db9fd1ce))
* **Refresh:** Issue with the interaction between vue module. ([4ccaf8b](https://github.com/Belphemur/AddictedProxy/commit/4ccaf8b5045443fb33f1362d72161861a283f5f8))
* **Refresh:** Possible division by 0 ([b7e870d](https://github.com/Belphemur/AddictedProxy/commit/b7e870d1224a0300c13039e9f81a5abdc71e31dd))
* **Refresh:** Remove concurrent season refreshing ([5c05a92](https://github.com/Belphemur/AddictedProxy/commit/5c05a9206f3650eb64e71601a59a3c4f98b8e504))
* **Search::Validation:** Put the validation on the right parameter ([8752821](https://github.com/Belphemur/AddictedProxy/commit/87528219b7fb4fabc4bec7bfa46c4cd14f4537dd))
* **Search:** Clean up previous search result when search isn't possible ([09e323d](https://github.com/Belphemur/AddictedProxy/commit/09e323d36bec074b69e75b4fb5e41f74a86c810f))
* **Search:** Only send search when more than 3 characters ([e998c41](https://github.com/Belphemur/AddictedProxy/commit/e998c416f54fa4361d20552bacd0403c9cfaa636))
* **Season:** Send all the available season, not just the number ([882aefe](https://github.com/Belphemur/AddictedProxy/commit/882aefe0b01a7120f372d6fc048de277958d591d))
* **Subtitles:** Filtering subtitles by language ([31d359d](https://github.com/Belphemur/AddictedProxy/commit/31d359d841be78005f2868a36cfafd608b56f1b7))


### Enhancements

* **Download:** Add download progress when downloading a subtitle ([4b315aa](https://github.com/Belphemur/AddictedProxy/commit/4b315aa8ca00c4d147807be451ff7cc478a55d40))
* **Download:** Add suffix for hearing impared subtitles ([b9e42b5](https://github.com/Belphemur/AddictedProxy/commit/b9e42b5a046b22c9cb046019e5b5ec5288aaef4c))
* **Fetching:** Increase timeout to 30 seconds ([3ac8416](https://github.com/Belphemur/AddictedProxy/commit/3ac841609f57a0fc8c197748f9a0e4e1865a5bf7))
* **frontend:** Add some styling ([9852e9b](https://github.com/Belphemur/AddictedProxy/commit/9852e9bc3d70ed1dc7414d960193301af55dc810))
* **Refresh:** Rework the code to fetch 2 season simultaneously for a show ([46dab2f](https://github.com/Belphemur/AddictedProxy/commit/46dab2f486e01aaa462f44ca6b5e3b845f5eae65))
* **Search:** Consistent messaging in the application ([58d22e9](https://github.com/Belphemur/AddictedProxy/commit/58d22e9a705c163fa7e181635aa8598ffcf86d90))
* **Search:** Improve the usability of the search by giving contextual tooltips. ([d8636cd](https://github.com/Belphemur/AddictedProxy/commit/d8636cd75ed48281d4bf1d9c4de0ee2a373a228a))
* **Subtitle:Download:** Block the button when download subtitle ([dbd8784](https://github.com/Belphemur/AddictedProxy/commit/dbd8784685e04d7016f3672d99aab7103d85b710))

## [2.0.0](https://github.com/Belphemur/AddictedProxy/compare/v1.1.1...v2.0.0) (2022-05-09)


### ⚠ BREAKING CHANGES

* **Show:** Use GUID instead of long for ShowId

### Features

* **API:** Update API of the FrontEnd application ([7bccf85](https://github.com/Belphemur/AddictedProxy/commit/7bccf855ee1371149aa5d0e26a757a32892616ae))
* **frontend::show:** Search working to find a specific show ([59340d8](https://github.com/Belphemur/AddictedProxy/commit/59340d8fe1738ceb8d593456bd1d5e9a14f78126))
* **frontend::show:** When not finding season, force a refresh of the show ([3f14672](https://github.com/Belphemur/AddictedProxy/commit/3f14672636d6704ba3bf2f2ff101a7ad84dbd142))
* **FrontEnd:** Start building the VueJS front-end ([14dbb81](https://github.com/Belphemur/AddictedProxy/commit/14dbb8113de9a940a81b8e371bac6c9171c38d36))
* **Show:** Add unique Id to Shows ([5b6e70a](https://github.com/Belphemur/AddictedProxy/commit/5b6e70a2f261e43917b4de6b90fc63bfecfb090c))
* **Show:** Refresh all the season and episode of selected show ([e0d7ee1](https://github.com/Belphemur/AddictedProxy/commit/e0d7ee1a514c24fa70e6e2dd34741377c1befbc5))
* **Shows:** Add search for shows ([fdfff35](https://github.com/Belphemur/AddictedProxy/commit/fdfff3511bf37ac0454bc77b64a8641c8b600c9c))
* **Subtitles:** Get all available subtitles for a specific season of a show ([040a976](https://github.com/Belphemur/AddictedProxy/commit/040a9767a16044b631e3a729a8f47e06e6cb9246))


### Bug Fixes

* **Cors:** Allow cors from everywhere for now ([ea07f4d](https://github.com/Belphemur/AddictedProxy/commit/ea07f4defc26240ded2ee17d1613395fead1fc2b))
* **Locking:** The lock key wasn't generated properly ([8449799](https://github.com/Belphemur/AddictedProxy/commit/844979936b402a8ce2fd39b7e137550e0dd6fa46))
* **Parsing::Completion:** Only parse the completion percentage when it's present. ([ca73a0e](https://github.com/Belphemur/AddictedProxy/commit/ca73a0e95d7144aa00a9d7882e35a2cf9ff70b10))
* **Search:** min length ([2d3671b](https://github.com/Belphemur/AddictedProxy/commit/2d3671b0538ad742a7edc6007d31a93f2f82dcac))
* **Show::Search:** Be sure to use Like function to use the collation NOCASE ([9499345](https://github.com/Belphemur/AddictedProxy/commit/9499345dc8567ffb750428b89feb410ab625a19f))
* **Show:** Be sure we load properly all the season of the show before refreshing ([4350901](https://github.com/Belphemur/AddictedProxy/commit/43509011ea0d4fb5dfc58b06aa025a98e8034347))
* **Subtitle:** Avoid updating the Guid when syncing subtitles ([0385cd1](https://github.com/Belphemur/AddictedProxy/commit/0385cd1c48422296e2c0ceacf0e8e838f5d64e50))


### Enhancements

* **Refresh:** Be sure we can't have multiple refresh of same season/same show ([6783372](https://github.com/Belphemur/AddictedProxy/commit/67833723e85bda1e4e7998e84e12deca768b735a))
* **Refresh:** Change the route to use the ID first ([db7aaca](https://github.com/Belphemur/AddictedProxy/commit/db7aaca0b422f2ee3146f356a7554fc5007084b6))
* **Show:** Make show use a GUID instead of Long ([63f5be7](https://github.com/Belphemur/AddictedProxy/commit/63f5be72e0d6db92f81dbcfd2e9e1ec70f0b1d5f))
* **Subtitle:** Add completion percentage to subtitle ([a519a54](https://github.com/Belphemur/AddictedProxy/commit/a519a5489285fc0d65a9a920dbe7d2898a0054e2))
* **Subtitle:** Only store subtitles that are completed ([b0aca65](https://github.com/Belphemur/AddictedProxy/commit/b0aca656f051c175b001376ff798b690f78b40c4))

### [1.1.1](https://github.com/Belphemur/AddictedProxy/compare/v1.1.0...v1.1.1) (2022-05-03)


### Enhancements

* **Documentation:** Set proper version for the API ([213a814](https://github.com/Belphemur/AddictedProxy/commit/213a81442d6be4c5c16e6b3b1960516adcc5929a))

## [1.1.0](https://github.com/Belphemur/AddictedProxy/compare/v1.0.5...v1.1.0) (2022-05-03)


### Features

* **Search:** Add simple search feature ([833a092](https://github.com/Belphemur/AddictedProxy/commit/833a092f7c0643af3c438ed7ab01de36d7f3d8a6))
* **Search:** Make filename optional ([a6afa7a](https://github.com/Belphemur/AddictedProxy/commit/a6afa7a477ccf1d7fd68efd06795f8855feceae4))
* **Subtitle:** Add download count ([cb829ba](https://github.com/Belphemur/AddictedProxy/commit/cb829ba776aaeeeb86df4ee4a64f4eea244fa11c))


### Bug Fixes

* **Subtitle:** Fix case where the subtitle file isn't available in the storage ([e0626e3](https://github.com/Belphemur/AddictedProxy/commit/e0626e3e66291ea2faff03e6a9a4cd8de402e5fe))


### Enhancements

* **Documentation:** Improve the swagger documentation ([6faf1ea](https://github.com/Belphemur/AddictedProxy/commit/6faf1ea1e9a8bd315192671c01a584e58d1fe542))
* **Documentation:** Move documentation to /api ([ba2c27a](https://github.com/Belphemur/AddictedProxy/commit/ba2c27a2c9ba3d0d62bede0ddb042fa81ca7a570))
* **Subtitle::Download:** Be sure the downloaded file has the name of the show, the season, episode number and language. ([68dd8e7](https://github.com/Belphemur/AddictedProxy/commit/68dd8e74cc9856377ec84477810e8ebb6992e63e))
* **Subtitle::Download:** Be sure the subtitle file has proper name when downloading ([2fa486f](https://github.com/Belphemur/AddictedProxy/commit/2fa486f7009a89507ce9e1483fd25a7797393dcd))

### [1.0.5](https://github.com/Belphemur/AddictedProxy/compare/v1.0.4...v1.0.5) (2022-04-24)


### Bug Fixes

* **Creds:** Crash when no creds available ([e08c94a](https://github.com/Belphemur/AddictedProxy/commit/e08c94a5f9b28c83173b88ea4e45f28072cef6d5))
* **Docker:** Set the right directly for the database ([a656ac1](https://github.com/Belphemur/AddictedProxy/commit/a656ac10c63facb4caf783011f14d6ec0a79bb20))
* **Sqlite:** Fix migration scriptscan't drop columns ([a39fff3](https://github.com/Belphemur/AddictedProxy/commit/a39fff388e881726fb0da7ced0ae62a11fac12dc))

### [1.0.4](https://github.com/Belphemur/AddictedProxy/compare/v1.0.3...v1.0.4) (2022-04-24)


### Bug Fixes

* **Docker:** Be sure the signing can happen ([b6004cb](https://github.com/Belphemur/AddictedProxy/commit/b6004cb2371e814a25385c17a972dfc06f3376da))

### [1.0.3](https://github.com/Belphemur/AddictedProxy/compare/v1.0.2...v1.0.3) (2022-04-24)


### Bug Fixes

* **CI:** Only give the version for the tag ([bd7e843](https://github.com/Belphemur/AddictedProxy/commit/bd7e843b330b02ac2f3bd5108697405b9d21b0cb))

### [1.0.2](https://github.com/Belphemur/AddictedProxy/compare/v1.0.1...v1.0.2) (2022-04-24)


### Bug Fixes

* **Changelog:** Fix the changelog ([6f10a9f](https://github.com/Belphemur/AddictedProxy/commit/6f10a9f6cd81df95d3961dd084c76ee92daed91d))


### Enhancements

* **CI:** Fix the tagging of the docker image ([662c8e8](https://github.com/Belphemur/AddictedProxy/commit/662c8e80185bb0de5252684482f9766d7ad97776))

### [1.0.1](https://github.com/Belphemur/AddictedProxy/compare/v1.0.0...v1.0.1) (2022-04-24)


### Bug Fixes

* **Docker:** Fix the build image to actually run the application ([13d3646](https://github.com/Belphemur/AddictedProxy/commit/13d3646ac8357afe93313721a3f5f1ac56f051e0))
* **Subtitle:** GUID make the guid consistent and uppercase ([94b1597](https://github.com/Belphemur/AddictedProxy/commit/94b159736cb97e5af6db4ecb6a0498a3ef72a10e))


### Enhancements

* **Cleanup:** Continue the code cleanup ([77656f7](https://github.com/Belphemur/AddictedProxy/commit/77656f7fca339239b8122a4d148870202de1fbde))
* **Database:** Make the database folder configurable with DB_PATH ([4ca71fa](https://github.com/Belphemur/AddictedProxy/commit/4ca71fa1046d712aa72ceca3d94b517e5f4ad349))
* **Subtitle:** Remove unused class ([d50e205](https://github.com/Belphemur/AddictedProxy/commit/d50e2052e300f3ba0415d9f045a5913e0f52aac8))


## 1.0.0 (2022-04-23)


### Features

* **Proxy:** First version of the proxy ([15e382e](https://github.com/Belphemur/AddictedProxy/commit/15e382ee528a99ae9c34fd7512db47aa02e22739))
