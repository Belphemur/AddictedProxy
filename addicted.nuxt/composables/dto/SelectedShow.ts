import { ShowDto } from "~/composables/api/api";

export interface SelectedShow {
  show: ShowDto;
  language: string;
  season: number;
}
