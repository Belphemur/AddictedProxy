/* eslint-disable */
/* tslint:disable */
// @ts-nocheck
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

import type { ShowSearchResponse, TvShowSubtitleResponse } from "./data-contracts";

export namespace Shows {
  /**
   * No description
   * @tags TvShows
   * @name SearchDetail
   * @summary Search shows that contains the given query
   * @request GET:/shows/search/{search}
   */
  export namespace SearchDetail {
    export type RequestParams = {
      /**
       * Name of the show to search for
       * @minLength 3
       */
      search: string;
    };
    export type RequestQuery = {};
    export type RequestBody = never;
    export type RequestHeaders = {};
    export type ResponseBody = ShowSearchResponse;
  }

  /**
   * No description
   * @tags TvShows
   * @name ExternalTvdbDetail
   * @summary Get a show by it's TvDB id: https://thetvdb.com/
   * @request GET:/shows/external/tvdb/{tvdbId}
   */
  export namespace ExternalTvdbDetail {
    export type RequestParams = {
      /**
       * Id of the show on TvDB
       * @format int32
       */
      tvdbId: number;
    };
    export type RequestQuery = {};
    export type RequestBody = never;
    export type RequestHeaders = {};
    export type ResponseBody = ShowSearchResponse;
  }

  /**
   * No description
   * @tags TvShows
   * @name RefreshCreate
   * @summary Refresh a specific show
   * @request POST:/shows/{showId}/refresh
   */
  export namespace RefreshCreate {
    export type RequestParams = {
      /** @format uuid */
      showId: string;
    };
    export type RequestQuery = {};
    export type RequestBody = never;
    export type RequestHeaders = {};
    export type ResponseBody = void;
  }

  /**
   * No description
   * @tags TvShows
   * @name ShowsDetail
   * @summary Get all subtitle of the given season for a specific language
   * @request GET:/shows/{showId}/{seasonNumber}/{language}
   */
  export namespace ShowsDetail {
    export type RequestParams = {
      /** @format uuid */
      showId: string;
      /** @format int32 */
      seasonNumber: number;
      language: string;
    };
    export type RequestQuery = {};
    export type RequestBody = never;
    export type RequestHeaders = {};
    export type ResponseBody = TvShowSubtitleResponse;
  }
}
