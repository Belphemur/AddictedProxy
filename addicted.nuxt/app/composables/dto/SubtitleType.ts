export type SubtitleType = "regular" | "hearing_impaired";

export enum SubtitleTypeFlag {
  None = 0,
  Regular = 1 << 0,
  HearingImpaired = 1 << 1,
  All = Regular | HearingImpaired,
}
