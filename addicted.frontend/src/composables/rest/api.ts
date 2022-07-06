import { Api } from "~/api/api";

export const api = new Api({ baseUrl: import.meta.env.VITE_APP_API_PATH });
