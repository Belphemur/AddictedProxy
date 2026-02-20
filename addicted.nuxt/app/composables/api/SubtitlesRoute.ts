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

import { SearchRequest, SubtitleSearchResponse } from "./data-contracts";

export namespace Subtitles {
  /**
   * No description
   * @tags Subtitles
   * @name DownloadSubtitle
   * @summary Download specific subtitle
   * @request GET:/subtitles/download/{subtitleId}
   */
  export namespace DownloadSubtitle {
    export type RequestParams = {
      /** @format uuid */
      subtitleId: string;
    };
    export type RequestQuery = {};
    export type RequestBody = never;
    export type RequestHeaders = {};
    export type ResponseBody = void;
  }

  /**
   * No description
   * @tags Subtitles
   * @name SearchCreate
   * @summary Search for a specific episode
   * @request POST:/subtitles/search
   * @deprecated
   */
  export namespace SearchCreate {
    export type RequestParams = {};
    export type RequestQuery = {};
    export type RequestBody = SearchRequest;
    export type RequestHeaders = {};
    export type ResponseBody = SubtitleSearchResponse;
  }

  /**
   * No description
   * @tags Subtitles
   * @name FindDetail
   * @summary Find specific episode (same as search but easily cacheable)
   * @request GET:/subtitles/find/{language}/{show}/{season}/{episode}
   * @deprecated
   */
  export namespace FindDetail {
    export type RequestParams = {
      /** Language to search for */
      language: string;
      /** Name of the show */
      show: string;
      /**
       * Season number to look for
       * @format int32
       * @min 0
       */
      season: number;
      /**
       * Episode number to look for
       * @format int32
       * @min 0
       */
      episode: number;
    };
    export type RequestQuery = {};
    export type RequestBody = never;
    export type RequestHeaders = {};
    export type ResponseBody = SubtitleSearchResponse;
  }

  /**
   * @description Start by using the /shows/search/SHOW_NAME to find the showUniqueId of the show you're interested in. Then use the subtitles/get/ endpoint to get subtitle for the episode you want.
   * @tags Subtitles
   * @name GetSubtitles
   * @summary Get subtitles for an episode of a given show in the wanted language
   * @request GET:/subtitles/get/{showUniqueId}/{season}/{episode}/{language}
   */
  export namespace GetSubtitles {
    export type RequestParams = {
      /**
       * Language to search for
       * @minLength 2
       */
      language: string;
      /**
       * Unique ID of the show, you can get it from Shows::Search
       * @format uuid
       */
      showUniqueId: string;
      /**
       * Season number to look for
       * @format int32
       * @min 0
       */
      season: number;
      /**
       * Episode number to look for
       * @format int32
       * @min 0
       */
      episode: number;
    };
    export type RequestQuery = {};
    export type RequestBody = never;
    export type RequestHeaders = {};
    export type ResponseBody = SubtitleSearchResponse;
  }
}
