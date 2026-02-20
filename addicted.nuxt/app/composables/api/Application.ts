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

import type { ApplicationInfoDto } from "./data-contracts";
import { HttpClient } from "./http-client";
import type { RequestParams } from "./http-client";

export class Application<
  SecurityDataType = unknown,
> extends HttpClient<SecurityDataType> {
  /**
   * No description
   *
   * @tags Application
   * @name InfoList
   * @summary Information about the application
   * @request GET:/application/info
   */
  infoList = (params: RequestParams = {}) =>
    this.request<ApplicationInfoDto, any>({
      path: `/application/info`,
      method: "GET",
      format: "json",
      ...params,
    });
}
