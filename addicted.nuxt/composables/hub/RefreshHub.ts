import {HubConnection, HubConnectionBuilder, LogLevel} from "@microsoft/signalr";
import {ShowDto} from "~/composables/api/api";

let started = false;
let connection: HubConnection;

export function useRefreshHub() {

    // if(!process.client)
    //     return {start: () => {}, sendRefreshAsync: () => {}, unsubscribeShowAsync: () => {}, onProgress: () => {}, offProgress: () => {}, onDone: () => {}, offDone: () => {}};
    const config = useRuntimeConfig();
    connection ??= new HubConnectionBuilder()
        // @ts-ignore
        .withUrl(config.public.api.url + "/refresh")
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

    return {start, sendRefreshAsync, unsubscribeShowAsync, onProgress, offProgress, onDone, offDone};
}

export interface Progress {
    showId: string;
    progress: number;
}

export type ProgressHandler = (progress: Progress) => void;
export type DoneHandler = (show: ShowDto) => void;