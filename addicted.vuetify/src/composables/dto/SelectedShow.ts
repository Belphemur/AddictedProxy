import { ShowDto } from "@/api/api";

export interface SelectedShow {
  show: ShowDto;
  language: string;
  season: number;
}
