/* eslint-disable */
/* tslint:disable */
/*
 * ---------------------------------------------------------------
 * ## THIS FILE WAS GENERATED VIA SWAGGER-TYPESCRIPT-API        ##
 * ##                                                           ##
 * ## AUTHOR: acacode                                           ##
 * ## SOURCE: https://github.com/acacode/swagger-typescript-api ##
 * ---------------------------------------------------------------
 */

import { MediaDetailsDto } from "./data-contracts";
import { HttpClient, RequestParams } from "./http-client";

export class Media<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Media
   * @name TrendingDetail
   * @summary Get the trending media of the last week
   * @request GET:/media/trending/{max}
   */
  trendingDetail = (max: number, params: RequestParams = {}) =>
    this.request<MediaDetailsDto[], string>({
      path: `/media/trending/${max}`,
      method: "GET",
      format: "json",
      ...params,
    });
  /**
   * No description
   *
   * @tags Media
   * @name MediaDetails
   * @summary Get the details of a specific show
   * @request GET:/media/{showId}/details
   */
  mediaDetails = (showId: string, params: RequestParams = {}) =>
    this.request<MediaDetailsDto, void | string>({
      path: `/media/${showId}/details`,
      method: "GET",
      format: "json",
      ...params,
    });
}
