import type {ShowDto} from "~/composables/api/data-contracts";

export interface SelectedShow {
  show: ShowDto;
  language: string;
  season: number;
}