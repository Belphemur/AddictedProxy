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

export namespace Media {
  /**
   * No description
   * @tags Media
   * @name TrendingDetail
   * @summary Get the trending media of the last week
   * @request GET:/media/trending/{max}
   */
  export namespace TrendingDetail {
    export type RequestParams = {
      /**
       * @format int32
       * @min 0
       * @max 15
       */
      max: number;
    };
    export type RequestQuery = {};
    export type RequestBody = never;
    export type RequestHeaders = {};
    export type ResponseBody = MediaDetailsDto[];
  }

  /**
   * No description
   * @tags Media
   * @name MediaDetails
   * @summary Get the details of a specific show
   * @request GET:/media/{showId}/details
   */
  export namespace MediaDetails {
    export type RequestParams = {
      /** @format uuid */
      showId: string;
    };
    export type RequestQuery = {};
    export type RequestBody = never;
    export type RequestHeaders = {};
    export type ResponseBody = MediaDetailsDto;
  }
}
