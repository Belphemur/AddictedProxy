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
