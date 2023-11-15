import {HubConnection, HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import type {EpisodeWithSubtitlesDto, ShowDto} from "~/composables/api/data-contracts";
import {getApiServerUrl} from "~/composables/rest/api";

let started = false;
let connection: HubConnection;

export function useRefreshHub() {

    connection ??= new HubConnectionBuilder()
        // @ts-ignore
        .withUrl(getApiServerUrl() + "/refresh")
        .configureLogging(LogLevel.Information)
        .withAutomaticReconnect()
        .build();

    async function start() {
        if (started) return;
        try {
            await connection.start();
            console.log("SignalR Connected.");
            started = true;
        } catch (err) {
            console.log(err);
            setTimeout(start, 5000);
        }
    }

    function sendRefreshAsync(showId: string) {
        return connection.invoke("RefreshShow", showId);
    }

    function getEpisodes(showId: string, language: string, season: number) {
        return new Promise<EpisodeWithSubtitlesDto[]>((resolve, reject) => {
            let episodes: Array<EpisodeWithSubtitlesDto> = [];
            const subscription = connection.stream<EpisodeWithSubtitlesDto>("GetEpisodes", showId, language, season).subscribe({
                next: (item) => {
                    episodes.push(item);
                },
                complete: () => {
                    resolve(episodes);
                    subscription.dispose();
                },
                error: (err) => {
                    reject(err);
                    subscription.dispose();
                }
            });

        });
    }

    function unsubscribeShowAsync(showId: string) {
        return connection.invoke("UnsubscribeRefreshShow", showId);
    }

    function offProgress(handler: ProgressHandler) {
        connection.off("Progress", handler);
    }

    function onProgress(handler: ProgressHandler) {
        connection.on("Progress", handler);
    }

    function onDone(handler: DoneHandler) {
        connection.on("Done", handler);
    }

    function offDone(handler: DoneHandler) {
        connection.off("Done", handler);
    }

    return {start, sendRefreshAsync, unsubscribeShowAsync, onProgress, offProgress, onDone, offDone, getEpisodes};
}

export interface Progress {
    showId: string;
    progress: number;
}

export type ProgressHandler = (progress: Progress) => void;
export type DoneHandler = (show: ShowDto) => void;