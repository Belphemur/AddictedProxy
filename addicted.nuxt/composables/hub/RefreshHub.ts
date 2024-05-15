import {HubConnection, HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import type {EpisodeWithSubtitlesDto, ShowDto} from "~/composables/api/data-contracts";
import {getApiServerUrl} from "~/composables/rest/api";



export function useRefreshHub() {
    let started = false;
    let connection: HubConnection;
    function getConnection() : HubConnection {
        return connection ??= new HubConnectionBuilder()
            // @ts-ignore
            .withUrl(getApiServerUrl() + "/refresh")
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .withStatefulReconnect()
            .build();
    }

    async function start() {
        if (started) return;
        try {
            await getConnection().start();
            console.log("SignalR Connected.");
            started = true;
        } catch (err) {
            console.log(err);
            setTimeout(start, 5000);
        }
    }

    function sendRefreshAsync(showId: string) {
        return getConnection().invoke("RefreshShow", showId);
    }

    function getEpisodes(showId: string, language: string, season: number) {
        return new Promise<EpisodeWithSubtitlesDto[]>((resolve, reject) => {
            let episodes: Array<EpisodeWithSubtitlesDto> = [];
            const subscription = getConnection().stream<EpisodeWithSubtitlesDto>("GetEpisodes", showId, language, season).subscribe({
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
        return getConnection().invoke("UnsubscribeRefreshShow", showId);
    }

    function offProgress(handler: ProgressHandler) {
        getConnection().off("Progress", handler);
    }

    function onProgress(handler: ProgressHandler) {
        getConnection().on("Progress", handler);
    }

    function onDone(handler: DoneHandler) {
        getConnection().on("Done", handler);
    }

    function offDone(handler: DoneHandler) {
        getConnection().off("Done", handler);
    }

    return {start, sendRefreshAsync, unsubscribeShowAsync, onProgress, offProgress, onDone, offDone, getEpisodes};
}

export interface Progress {
    showId: string;
    progress: number;
}

export type ProgressHandler = (progress: Progress) => void;
export type DoneHandler = (show: ShowDto) => void;