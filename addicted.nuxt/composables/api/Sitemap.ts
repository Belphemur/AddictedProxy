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

import { HttpClient, RequestParams } from "./http-client";

export class Sitemap<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Sitemap
   * @name MediaSitemap
   * @request GET:/sitemap/media/{page}
   */
  mediaSitemap = (page: number, params: RequestParams = {}) =>
    this.request<void, any>({
      path: `/sitemap/media/${page}`,
      method: "GET",
      ...params,
    });
}
