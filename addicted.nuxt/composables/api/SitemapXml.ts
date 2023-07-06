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

export class SitemapXml<SecurityDataType = unknown> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Sitemap
   * @name SitemapXmlList
   * @request GET:/sitemap.xml
   */
  sitemapXmlList = (page: number, params: RequestParams = {}) =>
    this.request<void, any>({
      path: `/sitemap.xml`,
      method: "GET",
      ...params,
    });
}
