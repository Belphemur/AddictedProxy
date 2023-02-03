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
