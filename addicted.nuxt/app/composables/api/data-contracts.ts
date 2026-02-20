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

export interface ApplicationInfoDto {
  /**
   * Version of the application
   * @minLength 1
   * @example "2.9.5"
   */
  applicationVersion: string;
}

export interface DetailsDto {
  /**
   * URL of the poster
   * @minLength 1
   * @example "https://upload.wikimedia.org/wikipedia/en/thumb/5/54/Bloodhounds_%28South_Korean_TV_series%29.jpg/250px-Bloodhounds_%28South_Korean_TV_series%29.jpg"
   */
  posterPath: string;
  /**
   * Short description of the media, usually the plot
   * @minLength 1
   * @example "Bloodhounds depicts a story about two young boxers who set foot in the world of private loans in pursuit of money and get caught up in a huge force"
   */
  overview: string;
  /**
   * Original name in its own language, useful for non-english shows
   * @minLength 1
   * @example "사냥개들"
   */
  originalName: string;
  /** Represent the type of media */
  mediaType: MediaType;
  /**
   * URL of the backdrop image for the show
   * @minLength 1
   * @example "https://upload.wikimedia.org/wikipedia/en/thumb/5/54/Bloodhounds_%28South_Korean_TV_series%29.jpg/250px-Bloodhounds_%28South_Korean_TV_series%29.jpg"
   */
  backdropPath: string;
  /**
   * Percentage of user votes
   * @format double
   * @example 0.85
   */
  voteAverage: number;
  /**
   * Genre of the media
   * @example ["action","horror"]
   */
  genre: string[];
  /**
   * Tagline of the media
   * @minLength 1
   * @example "The best show on earth"
   */
  tagLine: string;
  /**
   * Year of release
   * @format int32
   * @example 2023
   */
  releaseYear?: number | null;
  /**
   * English name of the show
   * @minLength 1
   * @example "Bloodhounds (2024)"
   */
  englishName: string;
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

export interface MediaDetailsDto {
  /** Represent the information relating to a show */
  media?: ShowDto;
  details?: DetailsDto;
}

/** Represent a media with its details and episodes with subtitles */
export interface MediaDetailsWithEpisodeAndSubtitlesDto {
  details: MediaDetailsDto;
  episodeWithSubtitles: EpisodeWithSubtitlesDto[];
  /** @format int32 */
  lastSeasonNumber?: number | null;
}

/** Represent the type of media */
export type MediaType = "Show" | "Movie";

/** Source provider of the subtitle */
export type DataSource = "Addic7ed" | "SuperSubtitles";

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
   * Seasons available in ascending order
   * @example [2,3,4,5,6]
   */
  seasons: number[];
  /**
   * Id of the show on the TvDB
   * @format int32
   * @example 344280
   */
  tvDbId?: number | null;
  /**
   * ID of the show in TMDB if available
   * @format int32
   * @example 80475
   */
  tmdbId?: number | null;
  /**
   * Slug to be used in URL for the show
   * <example>wellington-paranormal</example>
   * @minLength 1
   */
  slug: string;
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
  /** Whether this subtitle has HD quality (720P, 1080P, or 2160P). Derived from qualities. */
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
  /** Source provider of the subtitle */
  source: DataSource;
  /**
   * Available video qualities for this subtitle.
   * @example ["Q720P","Q1080P"]
   */
  qualities: VideoQuality[];
  /**
   * Full release name from the provider.
   * Populated for SuperSubtitles; null for Addic7ed.
   * @example "Show.S01E03.720p.BluRay.x264-GROUP"
   */
  release?: string | null;
}

export enum VideoQuality {
  None = "None",
  Q360P = "Q360P",
  Q480P = "Q480P",
  Q720P = "Q720P",
  Q1080P = "Q1080P",
  Q2160P = "Q2160P",
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
