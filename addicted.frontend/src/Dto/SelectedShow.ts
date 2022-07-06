import { ShowDto } from "~/api";

export interface SelectedShow {
  show: ShowDto;
  language: string;
  season: number;
}
