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

export interface ApplicationInfoDto {
  /**
   * Version of the application
   * @minLength 1
   * @example "2.9.5"
   */
  applicationVersion: string;
}

/** Episode information */
export interface EpisodeDto {
  /**
   * Season of the episode
   * @format int32
   * @example 1
   */
  season: number;
  /**
   * Number of the episode
   * @format int32
   * @example 1
   */
  number: number;
  /**
   * Title of the episode
   * @minLength 1
   * @example "Demon Girl"
   */
  title: string;
  /**
   * For which show
   * @minLength 1
   * @example "Wellington Paranormal"
   */
  show: string;
  /**
   * When was the Episode discovered
   * @format date-time
   * @example "2022-04-02T05:16:45.3996669"
   */
  discovered: string;
}

export interface EpisodeWithSubtitlesDto {
  /**
   * Season of the episode
   * @format int32
   * @example 1
   */
  season: number;
  /**
   * Number of the episode
   * @format int32
   * @example 1
   */
  number: number;
  /**
   * Title of the episode
   * @minLength 1
   * @example "Demon Girl"
   */
  title: string;
  /**
   * For which show
   * @minLength 1
   * @example "Wellington Paranormal"
   */
  show: string;
  /**
   * When was the Episode discovered
   * @format date-time
   * @example "2022-04-02T05:16:45.3996669"
   */
  discovered: string;
  /** Subtitles for this episode */
  subtitles?: SubtitleDto[] | null;
}

/** Returns when there is an error */
export interface ErrorResponse {
  error?: string | null;
}

/** Use for the website to provide easy search for the user */
export interface SearchRequest {
  /**
   * Search for specific subtitle
   * @example "Wellington Paranormal S01E05"
   */
  search?: string | null;
  /**
   * Language of the subtitle
   * @example "English"
   */
  language?: string | null;
}

/** Represent the information relating to a show */
export interface ShowDto {
  /**
   * Unique ID of the show
   * @format uuid
   * @example "E9C1FA23-55AF-4711-8E34-3B31E2A75533"
   */
  id: string;
  /**
   * Name of the show
   * @minLength 1
   * @example "Wellington Paranormal"
   */
  name: string;
  /**
   * How many season the show has
   * @format int32
   * @example 5
   */
  nbSeasons: number;
  /**
   * Seasons available
   * @example [2,3,4,5,6]
   */
  seasons: number[];
}

export interface ShowSearchResponse {
  shows?: ShowDto[] | null;
}

export interface SubtitleDto {
  /**
   * Unique Id of the subtitle
   * @minLength 1
   * @example "1086727A-EB71-4B24-A209-7CF22374574D"
   */
  subtitleId: string;
  /**
   * Version of the subtitle
   * @minLength 1
   * @example "HDTV"
   */
  version: string;
  completed: boolean;
  hearingImpaired: boolean;
  corrected: boolean;
  hd: boolean;
  /**
   * Url to download the subtitle
   * @minLength 1
   * @example "/download/1086727A-EB71-4B24-A209-7CF22374574D"
   */
  downloadUri: string;
  /**
   * Language of the subtitle (in English)
   * @minLength 1
   * @example "English"
   */
  language: string;
  /**
   * When was the subtitle discovered in UTC
   * @format date-time
   * @example "2022-04-02T05:16:45.4001274"
   */
  discovered: string;
  /**
   * Number of times the subtitle was downloaded from the proxy
   * @format int64
   * @example 100
   */
  downloadCount: number;
}

export interface SubtitleSearchResponse {
  /** Matching subtitle for the filename and language */
  matchingSubtitles?: SubtitleDto[] | null;
  /** Episode information */
  episode?: EpisodeDto;
}

export interface TopShowDto {
  /** Represent the information relating to a show */
  show?: ShowDto;
  /** @format int64 */
  popularity?: number;
}

export interface TvShowSubtitleResponse {
  /** Episode with their subtitles */
  episodes?: EpisodeWithSubtitlesDto[] | null;
}

/** Returned when the search wasn't formatted properly */
export interface WrongFormatResponse {
  error?: string | null;
  search?: string | null;
}

export type QueryParamsType = Record<string | number, any>;
export type ResponseFormat = keyof Omit<Body, "body" | "bodyUsed">;

export interface FullRequestParams extends Omit<RequestInit, "body"> {
  /** set parameter to `true` for call `securityWorker` for this request */
  secure?: boolean;
  /** request path */
  path: string;
  /** content type of request body */
  type?: ContentType;
  /** query params */
  query?: QueryParamsType;
  /** format of response (i.e. response.json() -> format: "json") */
  format?: ResponseFormat;
  /** request body */
  body?: unknown;
  /** base url */
  baseUrl?: string;
  /** request cancellation token */
  cancelToken?: CancelToken;
}

export type RequestParams = Omit<FullRequestParams, "body" | "method" | "query" | "path">;

export interface ApiConfig<SecurityDataType = unknown> {
  baseUrl?: string;
  baseApiParams?: Omit<RequestParams, "baseUrl" | "cancelToken" | "signal">;
  securityWorker?: (securityData: SecurityDataType | null) => Promise<RequestParams | void> | RequestParams | void;
  customFetch?: typeof fetch;
}

export interface HttpResponse<D extends unknown, E extends unknown = unknown> extends Response {
  data: D;
  error: E;
}

type CancelToken = Symbol | string | number;

export enum ContentType {
  Json = "application/json",
  FormData = "multipart/form-data",
  UrlEncoded = "application/x-www-form-urlencoded",
  Text = "text/plain",
}

export class HttpClient<SecurityDataType = unknown> {
  public baseUrl: string = "";
  private securityData: SecurityDataType | null = null;
  private securityWorker?: ApiConfig<SecurityDataType>["securityWorker"];
  private abortControllers = new Map<CancelToken, AbortController>();
  private customFetch = (...fetchParams: Parameters<typeof fetch>) => fetch(...fetchParams);

  private baseApiParams: RequestParams = {
    credentials: "same-origin",
    headers: {},
    redirect: "follow",
    referrerPolicy: "no-referrer",
  };

  constructor(apiConfig: ApiConfig<SecurityDataType> = {}) {
    Object.assign(this, apiConfig);
  }

  public setSecurityData = (data: SecurityDataType | null) => {
    this.securityData = data;
  };

  protected encodeQueryParam(key: string, value: any) {
    const encodedKey = encodeURIComponent(key);
    return `${encodedKey}=${encodeURIComponent(typeof value === "number" ? value : `${value}`)}`;
  }

  protected addQueryParam(query: QueryParamsType, key: string) {
    return this.encodeQueryParam(key, query[key]);
  }

  protected addArrayQueryParam(query: QueryParamsType, key: string) {
    const value = query[key];
    return value.map((v: any) => this.encodeQueryParam(key, v)).join("&");
  }

  protected toQueryString(rawQuery?: QueryParamsType): string {
    const query = rawQuery || {};
    const keys = Object.keys(query).filter((key) => "undefined" !== typeof query[key]);
    return keys
      .map((key) => (Array.isArray(query[key]) ? this.addArrayQueryParam(query, key) : this.addQueryParam(query, key)))
      .join("&");
  }

  protected addQueryParams(rawQuery?: QueryParamsType): string {
    const queryString = this.toQueryString(rawQuery);
    return queryString ? `?${queryString}` : "";
  }

  private contentFormatters: Record<ContentType, (input: any) => any> = {
    [ContentType.Json]: (input: any) =>
      input !== null && (typeof input === "object" || typeof input === "string") ? JSON.stringify(input) : input,
    [ContentType.Text]: (input: any) => (input !== null && typeof input !== "string" ? JSON.stringify(input) : input),
    [ContentType.FormData]: (input: any) =>
      Object.keys(input || {}).reduce((formData, key) => {
        const property = input[key];
        formData.append(
          key,
          property instanceof Blob
            ? property
            : typeof property === "object" && property !== null
            ? JSON.stringify(property)
            : `${property}`,
        );
        return formData;
      }, new FormData()),
    [ContentType.UrlEncoded]: (input: any) => this.toQueryString(input),
  };

  protected mergeRequestParams(params1: RequestParams, params2?: RequestParams): RequestParams {
    return {
      ...this.baseApiParams,
      ...params1,
      ...(params2 || {}),
      headers: {
        ...(this.baseApiParams.headers || {}),
        ...(params1.headers || {}),
        ...((params2 && params2.headers) || {}),
      },
    };
  }

  protected createAbortSignal = (cancelToken: CancelToken): AbortSignal | undefined => {
    if (this.abortControllers.has(cancelToken)) {
      const abortController = this.abortControllers.get(cancelToken);
      if (abortController) {
        return abortController.signal;
      }
      return void 0;
    }

    const abortController = new AbortController();
    this.abortControllers.set(cancelToken, abortController);
    return abortController.signal;
  };

  public abortRequest = (cancelToken: CancelToken) => {
    const abortController = this.abortControllers.get(cancelToken);

    if (abortController) {
      abortController.abort();
      this.abortControllers.delete(cancelToken);
    }
  };

  public request = async <T = any, E = any>({
    body,
    secure,
    path,
    type,
    query,
    format,
    baseUrl,
    cancelToken,
    ...params
  }: FullRequestParams): Promise<HttpResponse<T, E>> => {
    const secureParams =
      ((typeof secure === "boolean" ? secure : this.baseApiParams.secure) &&
        this.securityWorker &&
        (await this.securityWorker(this.securityData))) ||
      {};
    const requestParams = this.mergeRequestParams(params, secureParams);
    const queryString = query && this.toQueryString(query);
    const payloadFormatter = this.contentFormatters[type || ContentType.Json];
    const responseFormat = format || requestParams.format;

    return this.customFetch(`${baseUrl || this.baseUrl || ""}${path}${queryString ? `?${queryString}` : ""}`, {
      ...requestParams,
      headers: {
        ...(requestParams.headers || {}),
        ...(type && type !== ContentType.FormData ? { "Content-Type": type } : {}),
      },
      signal: cancelToken ? this.createAbortSignal(cancelToken) : requestParams.signal,
      body: typeof body === "undefined" || body === null ? null : payloadFormatter(body),
    }).then(async (response) => {
      const r = response as HttpResponse<T, E>;
      r.data = null as unknown as T;
      r.error = null as unknown as E;

      const data = !responseFormat
        ? r
        : await response[responseFormat]()
            .then((data) => {
              if (r.ok) {
                r.data = data;
              } else {
                r.error = data;
              }
              return r;
            })
            .catch((e) => {
              r.error = e;
              return r;
            });

      if (cancelToken) {
        this.abortControllers.delete(cancelToken);
      }

      if (!response.ok) throw data;
      return data;
    });
  };
}

/**
 * @title Gestdown: Addicted Proxy
 * @version 2.19.2
 *
 * Provide a full api to search and download subtitles from Addic7ed website.
 */
export class Api<SecurityDataType extends unknown> extends HttpClient<SecurityDataType> {
  application = {
    /**
     * No description
     *
     * @tags Application
     * @name InfoList
     * @summary Information about the application
     * @request GET:/application/info
     */
    infoList: (params: RequestParams = {}) =>
      this.request<ApplicationInfoDto, any>({
        path: `/application/info`,
        method: "GET",
        format: "json",
        ...params,
      }),
  };
  stats = {
    /**
     * No description
     *
     * @tags Stats
     * @name GetStats
     * @summary Return the top show by popularity
     * @request GET:/stats/top/{top}
     */
    getStats: (top: number, params: RequestParams = {}) =>
      this.request<TopShowDto[], any>({
        path: `/stats/top/${top}`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Stats
     * @name DownloadsDetail
     * @summary Return the top show by downloads
     * @request GET:/stats/downloads/{top}
     */
    downloadsDetail: (top: number, params: RequestParams = {}) =>
      this.request<TopShowDto[], any>({
        path: `/stats/downloads/${top}`,
        method: "GET",
        format: "json",
        ...params,
      }),
  };
  subtitles = {
    /**
     * No description
     *
     * @tags Subtitles
     * @name DownloadSubtitle
     * @summary Download specific subtitle
     * @request GET:/subtitles/download/{subtitleId}
     */
    downloadSubtitle: (subtitleId: string, params: RequestParams = {}) =>
      this.request<void, void | ErrorResponse>({
        path: `/subtitles/download/${subtitleId}`,
        method: "GET",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Subtitles
     * @name SearchCreate
     * @summary Search for a specific episode
     * @request POST:/subtitles/search
     */
    searchCreate: (data: SearchRequest, params: RequestParams = {}) =>
      this.request<SubtitleSearchResponse, WrongFormatResponse | ErrorResponse | string>({
        path: `/subtitles/search`,
        method: "POST",
        body: data,
        type: ContentType.Json,
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Subtitles
     * @name FindDetail
     * @summary Find specific episode (same as search but easily cacheable)
     * @request GET:/subtitles/find/{language}/{show}/{season}/{episode}
     * @deprecated
     */
    findDetail: (language: string, show: string, season: number, episode: number, params: RequestParams = {}) =>
      this.request<SubtitleSearchResponse, WrongFormatResponse | ErrorResponse | string>({
        path: `/subtitles/find/${language}/${show}/${season}/${episode}`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags Subtitles
     * @name GetSubtitles
     * @summary Get subtitles for an episode of a given show in the wanted language
     * @request GET:/subtitles/get/{showUniqueId}/{season}/{episode}/{language}
     */
    getSubtitles: (
      language: string,
      showUniqueId: string,
      season: number,
      episode: number,
      params: RequestParams = {},
    ) =>
      this.request<SubtitleSearchResponse, WrongFormatResponse | ErrorResponse | void | string>({
        path: `/subtitles/get/${showUniqueId}/${season}/${episode}/${language}`,
        method: "GET",
        format: "json",
        ...params,
      }),
  };
  shows = {
    /**
     * No description
     *
     * @tags TvShows
     * @name SearchDetail
     * @summary Search shows that contains the given query
     * @request GET:/shows/search/{search}
     */
    searchDetail: (search: string, params: RequestParams = {}) =>
      this.request<ShowSearchResponse, string>({
        path: `/shows/search/${search}`,
        method: "GET",
        format: "json",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TvShows
     * @name RefreshCreate
     * @summary Refresh a specific show
     * @request POST:/shows/{showId}/refresh
     */
    refreshCreate: (showId: string, params: RequestParams = {}) =>
      this.request<void, void>({
        path: `/shows/${showId}/refresh`,
        method: "POST",
        ...params,
      }),

    /**
     * No description
     *
     * @tags TvShows
     * @name ShowsDetail
     * @summary Get all subtitle of the given season for a specific language
     * @request GET:/shows/{showId}/{seasonNumber}/{language}
     */
    showsDetail: (showId: string, seasonNumber: number, language: string, params: RequestParams = {}) =>
      this.request<TvShowSubtitleResponse, void | string>({
        path: `/shows/${showId}/${seasonNumber}/${language}`,
        method: "GET",
        format: "json",
        ...params,
      }),
  };
}
