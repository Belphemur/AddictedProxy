// eslint-disable-next-line @typescript-eslint/ban-ts-comment
// @ts-ignore

const datalayer = process.browser ? (window._mtm = window._mtm || []) : [];
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const mtag = (data: any) => {
  datalayer.push(data);
};
// eslint-disable-next-line @typescript-eslint/no-explicit-any
export const mevent = (eventName: string, eventData: any) => {
  mtag({ event: eventName, ...eventData });
};
