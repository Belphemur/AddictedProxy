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
