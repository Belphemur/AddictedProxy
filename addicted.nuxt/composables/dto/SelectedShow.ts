import { ShowDto } from "~/composables/api/api";

export interface SelectedShow {
  show: ShowDto;
  language: string;
  season: number;
}

export interface SelectedLangSeason {
  language: string;
  season: number|undefined;
}