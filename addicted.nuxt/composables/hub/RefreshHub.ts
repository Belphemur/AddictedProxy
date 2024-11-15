import {HubConnection, HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import type {EpisodeWithSubtitlesDto, ShowDto} from "~/composables/api/data-contracts";
import {getApiServerUrl} from "~/composables/rest/api";

let _connection: HubConnection;

export function useRefreshHub() {

    function getConnection() : HubConnection {
        return _connection ??= new HubConnectionBuilder()
            // @ts-ignore
            .withUrl(getApiServerUrl() + "/refresh")
            .configureLogging(LogLevel.Information)
            .withAutomaticReconnect()
            .withStatefulReconnect()
            .build();
    }

    async function EnsureConnected() {
        const conn = getConnection();
        if (conn.state == "Connected") return;
        try {
            await conn.start();
            console.log("SignalR Connected.");
        } catch (err) {
            console.log(err);
            setTimeout(EnsureConnected, 5000);
        }
    }

    async function sendRefreshAsync(showId: string) {
        await EnsureConnected();
        await getConnection().invoke("RefreshShow", showId);
    }

    async function getEpisodes(showId: string, language: string, season: number) {
        await EnsureConnected();

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

    async function unsubscribeShowAsync(showId: string) {
        await EnsureConnected();
        await getConnection().invoke("UnsubscribeRefreshShow", showId);
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

    return {sendRefreshAsync, unsubscribeShowAsync, onProgress, offProgress, onDone, offDone, getEpisodes};
}

export interface Progress {
    showId: string;
    progress: number;
}

export type ProgressHandler = (progress: Progress) => void;
export type DoneHandler = (show: ShowDto) => void;