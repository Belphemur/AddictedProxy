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
  SearchRequest,
  SubtitleSearchResponse,
  WrongFormatResponse,
} from "./data-contracts";
import { ContentType, HttpClient } from "./http-client";
import type { RequestParams } from "./http-client";

export class Subtitles<
  SecurityDataType = unknown,
> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Subtitles
   * @name DownloadSubtitle
   * @summary Download specific subtitle
   * @request GET:/subtitles/download/{subtitleId}
   */
  downloadSubtitle = (subtitleId: string, params: RequestParams = {}) =>
    this.request<void, ErrorResponse>({
      path: `/subtitles/download/${subtitleId}`,
      method: "GET",
      ...params,
    });
  /**
   * No description
   *
   * @tags Subtitles
   * @name SearchCreate
   * @summary Search for a specific episode
   * @request POST:/subtitles/search
   * @deprecated
   */
  searchCreate = (data: SearchRequest, params: RequestParams = {}) =>
    this.request<
      SubtitleSearchResponse,
      WrongFormatResponse | ErrorResponse | string
    >({
      path: `/subtitles/search`,
      method: "POST",
      body: data,
      type: ContentType.Json,
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Subtitles
   * @name FindDetail
   * @summary Find specific episode (same as search but easily cacheable)
   * @request GET:/subtitles/find/{language}/{show}/{season}/{episode}
   * @deprecated
   */
  findDetail = (
    language: string,
    show: string,
    season: number,
    episode: number,
    params: RequestParams = {},
  ) =>
    this.request<
      SubtitleSearchResponse,
      WrongFormatResponse | ErrorResponse | string
    >({
      path: `/subtitles/find/${language}/${show}/${season}/${episode}`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * @description Start by using the /shows/search/SHOW_NAME to find the showUniqueId of the show you're interested in. Then use the subtitles/get/ endpoint to get subtitle for the episode you want.
   *
   * @tags Subtitles
   * @name GetSubtitles
   * @summary Get subtitles for an episode of a given show in the wanted language
   * @request GET:/subtitles/get/{showUniqueId}/{season}/{episode}/{language}
   */
  getSubtitles = (
    language: string,
    showUniqueId: string,
    season: number,
    episode: number,
    params: RequestParams = {},
  ) =>
    this.request<
      SubtitleSearchResponse,
      WrongFormatResponse | ErrorResponse | void | string
    >({
      path: `/subtitles/get/${showUniqueId}/${season}/${episode}/${language}`,
      method: "GET",
      format: "json",
      ...params,
    });
}
