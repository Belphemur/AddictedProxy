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

import { TopShowDto } from "./data-contracts";
import { HttpClient, RequestParams } from "./http-client";

export class Stats<
  SecurityDataType = unknown,
> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Stats
   * @name DownloadsDetail
   * @summary Return the top show by downloads
   * @request GET:/stats/downloads/{top}
   */
  downloadsDetail = (top: number, params: RequestParams = {}) =>
    this.request<TopShowDto[], any>({
      path: `/stats/downloads/${top}`,
      method: "GET",
      format: "json",
      ...params,
    });
}
