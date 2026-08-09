// Package main provides a mock HTTP server for local UI development and Playwright testing.
// It emulates the AddictedProxy backend API with realistic show, episode, and subtitle data.
//
// Usage:
//
//	go run . [-port 8080]
//
// Then set the frontend environment variables:
//
//	APP_API_PATH=http://localhost:8080
//	APP_SERVER_PATH=http://localhost:8080
package main

import (
	_ "embed"
	"encoding/json"
	"flag"
	"fmt"
	"log"
	"net/http"
	"regexp"
	"strconv"
	"strings"
	"time"
)

// ── Data types (mirrors data-contracts.ts) ────────────────────────────────────

type ApplicationInfoDto struct {
	ApplicationVersion string `json:"applicationVersion"`
}

type MediaType string

const (
	MediaTypeShow  MediaType = "Show"
	MediaTypeMovie MediaType = "Movie"
)

type DetailsDto struct {
	PosterPath   string    `json:"posterPath"`
	Overview     string    `json:"overview"`
	OriginalName string    `json:"originalName"`
	MediaType    MediaType `json:"mediaType"`
	BackdropPath string    `json:"backdropPath"`
	VoteAverage  float64   `json:"voteAverage"`
	Genre        []string  `json:"genre"`
	TagLine      string    `json:"tagLine"`
	ReleaseYear  *int      `json:"releaseYear"`
	EnglishName  string    `json:"englishName"`
}

type ShowDto struct {
	ID        string `json:"id"`
	Name      string `json:"name"`
	NbSeasons int    `json:"nbSeasons"`
	Seasons   []int  `json:"seasons"`
	TvDbID    *int   `json:"tvDbId"`
	TmdbID    *int   `json:"tmdbId"`
	Slug      string `json:"slug"`
}

type MediaDetailsDto struct {
	Media   *ShowDto    `json:"media"`
	Details *DetailsDto `json:"details"`
}

type SubtitleDto struct {
	SubtitleID      string   `json:"subtitleId"`
	Version         string   `json:"version"`
	Completed       bool     `json:"completed"`
	HearingImpaired bool     `json:"hearingImpaired"`
	Corrected       bool     `json:"corrected"`
	HD              bool     `json:"hd"`
	DownloadURI     string   `json:"downloadUri"`
	Language        string   `json:"language"`
	Discovered      string   `json:"discovered"`
	DownloadCount   int64    `json:"downloadCount"`
	Source          string   `json:"source"`
	Qualities       []string `json:"qualities"`
	Release         *string  `json:"release"`
}

type EpisodeWithSubtitlesDto struct {
	Season     int            `json:"season"`
	Number     int            `json:"number"`
	Title      string         `json:"title"`
	Show       string         `json:"show"`
	Discovered string         `json:"discovered"`
	Subtitles  []*SubtitleDto `json:"subtitles"`
}

type MediaDetailsWithEpisodeAndSubtitlesDto struct {
	Details              MediaDetailsDto           `json:"details"`
	EpisodeWithSubtitles []EpisodeWithSubtitlesDto `json:"episodeWithSubtitles"`
	LastSeasonNumber     *int                      `json:"lastSeasonNumber"`
	SeasonPacks          []SeasonPackSubtitleDto   `json:"seasonPacks"`
}

type TvShowSubtitleResponse struct {
	Episodes    []EpisodeWithSubtitlesDto `json:"episodes"`
	SeasonPacks []SeasonPackSubtitleDto   `json:"seasonPacks"`
}

type ShowSearchResponse struct {
	Shows []ShowDto `json:"shows"`
}

type SeasonPackSubtitleDto struct {
	SubtitleID    string   `json:"subtitleId"`
	Language      string   `json:"language"`
	Version       string   `json:"version"`
	ReleaseGroups []string `json:"releaseGroups"`
	Uploader      *string  `json:"uploader"`
	UploadedAt    *string  `json:"uploadedAt"`
	Qualities     []string `json:"qualities"`
	Source        string   `json:"source"`
	DownloadURI   string   `json:"downloadUri"`
	DownloadCount int64    `json:"downloadCount"`
}

// ── Data file types ───────────────────────────────────────────────────────────

// ShowData is the shape of one entry in data/shows.json.
type ShowData struct {
	ID             string     `json:"id"`
	Name           string     `json:"name"`
	NbSeasons      int        `json:"nbSeasons"`
	Seasons        []int      `json:"seasons"`
	TvDbID         *int       `json:"tvDbId"`
	TmdbID         *int       `json:"tmdbId"`
	Slug           string     `json:"slug"`
	SeasonPackOnly bool       `json:"seasonPackOnly"`
	Details        DetailsDto `json:"details"`
	// EpisodeTitles is a fallback list used when a season has no entry in SeasonEpisodes.
	EpisodeTitles []string `json:"episodeTitles"`
	// SeasonEpisodes maps season number (as string key) to episode title list.
	// When present for a given season it takes priority over EpisodeTitles.
	SeasonEpisodes map[string][]string `json:"seasonEpisodes"`
}

// EpisodeSubtitleConfig holds the cycling patterns used to generate per-episode subtitle variants.
type EpisodeSubtitleConfig struct {
	VersionCycle        []string   `json:"versionCycle"`
	CompletedCycle      []bool     `json:"completedCycle"`
	SourceCycle         []string   `json:"sourceCycle"`
	QualityCycles       [][]string `json:"qualityCycles"`
	DownloadCountBase   int64      `json:"downloadCountBase"`
	DownloadCountStep   int64      `json:"downloadCountStep"`
	HiDownloadCountBase int64      `json:"hiDownloadCountBase"`
	HiDownloadCountStep int64      `json:"hiDownloadCountStep"`
}

// SeasonPackConfig holds the template values used for every generated season pack.
type SeasonPackConfig struct {
	Version          string   `json:"version"`
	ReleaseGroups    []string `json:"releaseGroups"`
	Uploader         string   `json:"uploader"`
	Qualities        []string `json:"qualities"`
	Source           string   `json:"source"`
	DownloadCount    int64    `json:"downloadCount"`
	UploadedHoursAgo int      `json:"uploadedHoursAgo"`
}

// AppConfig holds application-level mock values.
type AppConfig struct {
	Version string `json:"version"`
}

// MockConfig is the shape of data/config.json.
type MockConfig struct {
	App              AppConfig             `json:"app"`
	EpisodeSubtitles EpisodeSubtitleConfig `json:"episodeSubtitles"`
	SeasonPack       SeasonPackConfig      `json:"seasonPack"`
}

// ── Embedded data files ───────────────────────────────────────────────────────

//go:embed data/shows.json
var showsJSON []byte

//go:embed data/config.json
var configJSON []byte

// ── Runtime data (populated from embedded JSON in init) ───────────────────────

var (
	allShows     []ShowDto
	showByID     map[string]*ShowDto
	showDetails  map[string]*DetailsDto
	showDataByID map[string]*ShowData
	mockConfig   MockConfig
)

func init() {
	var showsData []ShowData
	if err := json.Unmarshal(showsJSON, &showsData); err != nil {
		log.Fatalf("failed to parse data/shows.json: %v", err)
	}

	allShows = make([]ShowDto, len(showsData))
	showByID = make(map[string]*ShowDto, len(showsData))
	showDetails = make(map[string]*DetailsDto, len(showsData))
	showDataByID = make(map[string]*ShowData, len(showsData))

	for i := range showsData {
		sd := &showsData[i]
		allShows[i] = ShowDto{
			ID:        sd.ID,
			Name:      sd.Name,
			NbSeasons: sd.NbSeasons,
			Seasons:   sd.Seasons,
			TvDbID:    sd.TvDbID,
			TmdbID:    sd.TmdbID,
			Slug:      sd.Slug,
		}
		showByID[sd.ID] = &allShows[i]
		showDetails[sd.ID] = &sd.Details
		showDataByID[sd.ID] = sd
	}

	if err := json.Unmarshal(configJSON, &mockConfig); err != nil {
		log.Fatalf("failed to parse data/config.json: %v", err)
	}

	// Validate that all episode subtitle cycles are non-empty to prevent
	// divide-by-zero panics in buildEpisodes.
	cfg := mockConfig.EpisodeSubtitles
	if len(cfg.VersionCycle) == 0 {
		log.Fatal("data/config.json: episodeSubtitles.versionCycle must not be empty")
	}
	if len(cfg.SourceCycle) == 0 {
		log.Fatal("data/config.json: episodeSubtitles.sourceCycle must not be empty")
	}
	if len(cfg.CompletedCycle) == 0 {
		log.Fatal("data/config.json: episodeSubtitles.completedCycle must not be empty")
	}
	if len(cfg.QualityCycles) == 0 {
		log.Fatal("data/config.json: episodeSubtitles.qualityCycles must not be empty")
	}
}

// buildEpisodes generates mock episodes with subtitles for a given show, season, and language.
// Returns an empty slice for shows marked seasonPackOnly in data/shows.json.
func buildEpisodes(show *ShowDto, season int, language string) []EpisodeWithSubtitlesDto {
	sd, ok := showDataByID[show.ID]
	if !ok || sd.SeasonPackOnly {
		return []EpisodeWithSubtitlesDto{}
	}

	cfg := mockConfig.EpisodeSubtitles
	// Prefer per-season episode titles when available, fall back to the shared list.
	var titles []string
	if sd.SeasonEpisodes != nil {
		titles = sd.SeasonEpisodes[strconv.Itoa(season)]
	}
	if len(titles) == 0 {
		titles = sd.EpisodeTitles
	}
	if len(titles) == 0 {
		titles = []string{"Episode 1", "Episode 2", "Episode 3", "Episode 4", "Episode 5"}
	}

	discovered := time.Now().UTC().Format(time.RFC3339)
	episodes := make([]EpisodeWithSubtitlesDto, len(titles))

	for i, title := range titles {
		epNum := i + 1
		ver := cfg.VersionCycle[(epNum-1)%len(cfg.VersionCycle)]
		source := cfg.SourceCycle[(epNum-1)%len(cfg.SourceCycle)]
		completed := cfg.CompletedCycle[(epNum-1)%len(cfg.CompletedCycle)]
		qualities := cfg.QualityCycles[(epNum-1)%len(cfg.QualityCycles)]

		subID := fmt.Sprintf("sub-%s-s%02de%02d-%s", show.ID[:8], season, epNum, language)
		hiSubID := fmt.Sprintf("sub-%s-s%02de%02d-%s-hi", show.ID[:8], season, epNum, language)
		rel := fmt.Sprintf("%s.S%02dE%02d.%s", strings.ReplaceAll(show.Name, " ", "."), season, epNum, ver)

		subs := []*SubtitleDto{
			{
				SubtitleID:      subID,
				Version:         ver,
				Completed:       completed,
				HearingImpaired: false,
				Corrected:       false,
				HD:              true,
				DownloadURI:     fmt.Sprintf("/subtitles/download/%s", subID),
				Language:        language,
				Discovered:      discovered,
				DownloadCount:   cfg.DownloadCountBase + int64(epNum)*cfg.DownloadCountStep,
				Source:          source,
				Qualities:       qualities,
				Release:         &rel,
			},
			{
				SubtitleID:      hiSubID,
				Version:         ver + " [HI]",
				Completed:       true,
				HearingImpaired: true,
				Corrected:       false,
				HD:              true,
				DownloadURI:     fmt.Sprintf("/subtitles/download/%s", hiSubID),
				Language:        language,
				Discovered:      discovered,
				DownloadCount:   cfg.HiDownloadCountBase + int64(epNum)*cfg.HiDownloadCountStep,
				Source:          source,
				Qualities:       qualities,
				Release:         &rel,
			},
		}

		episodes[i] = EpisodeWithSubtitlesDto{
			Season:     season,
			Number:     epNum,
			Title:      title,
			Show:       show.Name,
			Discovered: discovered,
			Subtitles:  subs,
		}
	}
	return episodes
}

// buildSeasonPacks generates mock season pack subtitles using the template in data/config.json.
// Season packs are generated for every show, regardless of seasonPackOnly: shows with
// seasonPackOnly=true have packs but no per-episode subtitles; episode-based shows have both.
func buildSeasonPacks(show *ShowDto, season int, language string) []SeasonPackSubtitleDto {
	sp := mockConfig.SeasonPack
	uploader := sp.Uploader
	uploadedAt := time.Now().Add(-time.Duration(sp.UploadedHoursAgo) * time.Hour).UTC().Format(time.RFC3339)
	spID := fmt.Sprintf("sp_%s-s%02d-%s", show.ID[:8], season, language)
	return []SeasonPackSubtitleDto{
		{
			SubtitleID:    spID,
			Language:      language,
			Version:       sp.Version,
			ReleaseGroups: sp.ReleaseGroups,
			Uploader:      &uploader,
			UploadedAt:    &uploadedAt,
			Qualities:     sp.Qualities,
			Source:        sp.Source,
			DownloadURI:   fmt.Sprintf("/subtitles/download/%s", spID),
			DownloadCount: sp.DownloadCount,
		},
	}
}

// ── HTTP helpers ─────────────────────────────────────────────────────────────

func jsonResponse(w http.ResponseWriter, status int, v any) {
	w.Header().Set("Content-Type", "application/json")
	w.Header().Set("Access-Control-Allow-Origin", "*")
	w.Header().Set("Access-Control-Allow-Headers", "Content-Type, Authorization")
	w.Header().Set("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS")
	w.WriteHeader(status)
	if err := json.NewEncoder(w).Encode(v); err != nil {
		log.Printf("encode error: %v", err)
	}
}

func corsMiddleware(next http.Handler) http.Handler {
	return http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		w.Header().Set("Access-Control-Allow-Origin", "*")
		w.Header().Set("Access-Control-Allow-Headers", "Content-Type, Authorization")
		w.Header().Set("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS")
		if r.Method == http.MethodOptions {
			w.WriteHeader(http.StatusNoContent)
			return
		}
		next.ServeHTTP(w, r)
	})
}

// ── Route handlers ────────────────────────────────────────────────────────────

// GET /application/info
func handleApplicationInfo(w http.ResponseWriter, r *http.Request) {
	jsonResponse(w, http.StatusOK, ApplicationInfoDto{ApplicationVersion: mockConfig.App.Version})
}

// GET /media/trending/{max}
func handleTrending(w http.ResponseWriter, r *http.Request, max int) {
	medias := make([]MediaDetailsDto, 0, max)
	for i, show := range allShows {
		if i >= max {
			break
		}
		s := show
		d := showDetails[show.ID]
		medias = append(medias, MediaDetailsDto{Media: &s, Details: d})
	}
	jsonResponse(w, http.StatusOK, medias)
}

// GET /media/{showId}/details
func handleMediaDetails(w http.ResponseWriter, r *http.Request, showID string) {
	show, ok := showByID[showID]
	if !ok {
		jsonResponse(w, http.StatusNotFound, map[string]string{"error": "show not found"})
		return
	}
	jsonResponse(w, http.StatusOK, MediaDetailsDto{Media: show, Details: showDetails[showID]})
}

// GET /media/{showId}/episodes/{language}
func handleMediaEpisodes(w http.ResponseWriter, r *http.Request, showID, language string) {
	show, ok := showByID[showID]
	if !ok {
		jsonResponse(w, http.StatusNotFound, map[string]string{"error": "show not found"})
		return
	}

	lastSeason := show.Seasons[len(show.Seasons)-1]
	episodes := buildEpisodes(show, lastSeason, language)
	seasonPacks := buildSeasonPacks(show, lastSeason, language)

	jsonResponse(w, http.StatusOK, MediaDetailsWithEpisodeAndSubtitlesDto{
		Details: MediaDetailsDto{
			Media:   show,
			Details: showDetails[showID],
		},
		EpisodeWithSubtitles: episodes,
		LastSeasonNumber:     &lastSeason,
		SeasonPacks:          seasonPacks,
	})
}

// GET /shows/search/{search}
func handleShowSearch(w http.ResponseWriter, r *http.Request, query string) {
	q := strings.ToLower(query)
	var results []ShowDto
	for _, show := range allShows {
		if strings.Contains(strings.ToLower(show.Name), q) {
			results = append(results, show)
		}
	}
	if results == nil {
		results = []ShowDto{}
	}
	jsonResponse(w, http.StatusOK, ShowSearchResponse{Shows: results})
}

// GET /shows/{showId}/{seasonNumber}/{language}
func handleShowSeason(w http.ResponseWriter, r *http.Request, showID string, season int, language string) {
	show, ok := showByID[showID]
	if !ok {
		jsonResponse(w, http.StatusNotFound, map[string]string{"error": "show not found"})
		return
	}
	episodes := buildEpisodes(show, season, language)
	seasonPacks := buildSeasonPacks(show, season, language)
	jsonResponse(w, http.StatusOK, TvShowSubtitleResponse{
		Episodes:    episodes,
		SeasonPacks: seasonPacks,
	})
}

// POST /shows/{showId}/refresh
func handleShowRefresh(w http.ResponseWriter, r *http.Request) {
	w.WriteHeader(http.StatusAccepted)
}

// GET /subtitles/download/{subtitleId}
func handleSubtitleDownload(w http.ResponseWriter, r *http.Request, subtitleID string) {
	srt := fmt.Sprintf(`1
00:00:01,000 --> 00:00:04,000
Mock subtitle for: %s

2
00:00:05,000 --> 00:00:08,000
This is a placeholder subtitle file
generated by the mock server.

3
00:00:09,000 --> 00:00:12,000
Use the real API for actual subtitles.
`, subtitleID)

	filename := fmt.Sprintf(`%s.srt`, subtitleID)
	w.Header().Set("Content-Type", "text/plain; charset=utf-8")
	w.Header().Set("Content-Disposition", fmt.Sprintf(`attachment; filename="%s"`, filename))
	w.Header().Set("Access-Control-Allow-Origin", "*")
	w.WriteHeader(http.StatusOK)
	fmt.Fprint(w, srt)
}

// ── Router ────────────────────────────────────────────────────────────────────

var (
	reTrending      = regexp.MustCompile(`^/media/trending/(\d+)$`)
	reMediaDetails  = regexp.MustCompile(`^/media/([^/]+)/details$`)
	reMediaEpisodes = regexp.MustCompile(`^/media/([^/]+)/episodes/([^/]+)$`)
	reShowSearch    = regexp.MustCompile(`^/shows/search/(.+)$`)
	reShowRefresh   = regexp.MustCompile(`^/shows/([^/]+)/refresh$`)
	reShowSeason    = regexp.MustCompile(`^/shows/([^/]+)/(\d+)/([^/]+)$`)
	reSubDownload   = regexp.MustCompile(`^/subtitles/download/(.+)$`)
)

func router(w http.ResponseWriter, r *http.Request) {
	log.Printf("%s %s", r.Method, r.URL.Path)

	path := r.URL.Path

	if path == "/application/info" {
		handleApplicationInfo(w, r)
		return
	}

	if m := reTrending.FindStringSubmatch(path); m != nil {
		max, _ := strconv.Atoi(m[1])
		handleTrending(w, r, max)
		return
	}

	if m := reMediaDetails.FindStringSubmatch(path); m != nil {
		handleMediaDetails(w, r, m[1])
		return
	}

	if m := reMediaEpisodes.FindStringSubmatch(path); m != nil {
		handleMediaEpisodes(w, r, m[1], m[2])
		return
	}

	if m := reShowSearch.FindStringSubmatch(path); m != nil {
		handleShowSearch(w, r, m[1])
		return
	}

	if m := reShowRefresh.FindStringSubmatch(path); m != nil && r.Method == http.MethodPost {
		handleShowRefresh(w, r)
		return
	}

	if m := reShowSeason.FindStringSubmatch(path); m != nil {
		season, _ := strconv.Atoi(m[2])
		handleShowSeason(w, r, m[1], season, m[3])
		return
	}

	if m := reSubDownload.FindStringSubmatch(path); m != nil {
		handleSubtitleDownload(w, r, m[1])
		return
	}

	http.NotFound(w, r)
}

// ── main ──────────────────────────────────────────────────────────────────────

func main() {
	port := flag.Int("port", 8080, "port to listen on")
	flag.Parse()

	addr := fmt.Sprintf(":%d", *port)
	log.Printf("Mock API server starting on http://localhost%s", addr)
	log.Printf("Set APP_API_PATH=http://localhost%s and APP_SERVER_PATH=http://localhost%s in your Nuxt dev environment.", addr, addr)

	mux := http.NewServeMux()
	mux.Handle("/", corsMiddleware(http.HandlerFunc(router)))

	srv := &http.Server{
		Addr:         addr,
		Handler:      mux,
		ReadTimeout:  15 * time.Second,
		WriteTimeout: 15 * time.Second,
		IdleTimeout:  60 * time.Second,
	}

	if err := srv.ListenAndServe(); err != nil {
		log.Fatalf("server error: %v", err)
	}
}
