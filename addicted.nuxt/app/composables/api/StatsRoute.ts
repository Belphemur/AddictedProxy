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

export namespace Stats {
  /**
   * No description
   * @tags Stats
   * @name DownloadsDetail
   * @summary Return the top show by downloads
   * @request GET:/stats/downloads/{top}
   */
  export namespace DownloadsDetail {
    export type RequestParams = {
      /**
       * @format int32
       * @min 1
       * @max 50
       */
      top: number;
    };
    export type RequestQuery = {};
    export type RequestBody = never;
    export type RequestHeaders = {};
    export type ResponseBody = TopShowDto[];
  }
}
