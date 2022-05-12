import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { ShowDto } from "@/api";

const connection = new HubConnectionBuilder()
  .withUrl(process.env.VUE_APP_API_PATH + "/refresh")
  .configureLogging(LogLevel.Information)
  .withAutomaticReconnect()
  .build();

async function start() {
  try {
    await connection.start();
    console.log("SignalR Connected.");
  } catch (err) {
    console.log(err);
    setTimeout(start, 5000);
  }
}

start();

export interface Progress {
  showId: string;
  progress: number;
}

export type ProgressHandler = (progress: Progress) => void;
export type DoneHandler = (show: ShowDto) => void;

export function sendRefreshAsync(showId: string) {
  return connection.invoke("RefreshShow", showId);
}

export function onProgress(handler: ProgressHandler) {
  connection.on("Progress", handler);
}

export function OffProgress(handler: ProgressHandler) {
  connection.off("Progress", handler);
}

export function OnDone(handler: DoneHandler) {
  connection.on("Done", handler);
}

export function OffDone(handler: DoneHandler) {
  connection.off("Done", handler);
}
