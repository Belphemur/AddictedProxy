/**
 * Use for the website to provide easy search for the user
 */
export interface SearchRequest {
    /**
     * Search for specific subtitle
     * @example Wellington Paranormal S01E05
     */
    search?: string | null;

    /**
     * Language of the subtitle
     * @example English
     */
    language?: string | null;
}