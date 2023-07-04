// eslint-disable-next-line @typescript-eslint/no-explicit-any
const mtag = (data: any) => {
    // @ts-ignore
    if (process.server || window == undefined || window._mtm == undefined) return;
    // @ts-ignore
    (window._mtm as any).push(data);
};
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const mevent = (eventName: string, eventData: any) => {
    mtag({event: eventName, ...eventData});
};
