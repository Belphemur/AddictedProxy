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

import type {
  ErrorResponse,
  ShowSearchResponse,
  TvShowSubtitleResponse,
} from "./data-contracts";
import { HttpClient } from "./http-client";
import type { RequestParams } from "./http-client";

export class Shows<
  SecurityDataType = unknown,
> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags TvShows
   * @name SearchDetail
   * @summary Search shows that contains the given query
   * @request GET:/shows/search/{search}
   */
  searchDetail = (search: string, params: RequestParams = {}) =>
    this.request<ShowSearchResponse, string>({
      path: `/shows/search/${search}`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags TvShows
   * @name ExternalTvdbDetail
   * @summary Get a show by it's TvDB id: https://thetvdb.com/
   * @request GET:/shows/external/tvdb/{tvdbId}
   */
  externalTvdbDetail = (tvdbId: number, params: RequestParams = {}) =>
    this.request<ShowSearchResponse, string>({
      path: `/shows/external/tvdb/${tvdbId}`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags TvShows
   * @name RefreshCreate
   * @summary Refresh a specific show
   * @request POST:/shows/{showId}/refresh
   */
  refreshCreate = (showId: string, params: RequestParams = {}) =>
    this.request<void, void>({
      path: `/shows/${showId}/refresh`,
      method: "POST",
      ...params,
    });
  /**
   * No description
   *
   * @tags TvShows
   * @name ShowsDetail
   * @summary Get all subtitle of the given season for a specific language
   * @request GET:/shows/{showId}/{seasonNumber}/{language}
   */
  showsDetail = (
    showId: string,
    seasonNumber: number,
    language: string,
    params: RequestParams = {},
  ) =>
    this.request<TvShowSubtitleResponse, ErrorResponse | void | string>({
      path: `/shows/${showId}/${seasonNumber}/${language}`,
      method: "GET",
      format: "json",
      ...params,
    });
}
