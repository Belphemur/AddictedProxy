<script setup lang="ts">
import MediaDetails from "@/components/media/MediaDetails.vue";
import { onUnmounted, ref, computed } from "vue";
import SubtitlesTable from "@/components/shows/SubtitlesTable.vue";
import SeasonPacksSection from "@/components/media/SeasonPacksSection.vue";
import type { DoneHandler, ProgressHandler } from "~/composables/hub/RefreshHub";
import { useRefreshHub } from "~/composables/hub/RefreshHub";
import type { EpisodeWithSubtitlesDto, MediaDetailsDto, SeasonPackSubtitleDto } from "~/composables/api/data-contracts";
import { useMedia, useShows, useSubtitles } from "~/composables/rest/api";
import SubtitleTypeChooser from "~/components/media/Download/SubtitleTypeChooser.vue";
import type { SubtitleType } from "~/composables/dto/SubtitleType";
import { SubtitleTypeFlag } from "~/composables/dto/SubtitleType";
import { trim } from "~/composables/utils/trim";
import { downloadZip } from "client-zip";
import { mevent } from "~/composables/data/event";
import { usePageLayout } from "~/composables/usePageLayout";
import { mdiDownload, mdiRefresh } from "@mdi/js";
import { last } from "lodash-es";

export interface Props {
  showId: string;
  initialSeason?: number;
}

const layout = usePageLayout();
const route = useRoute();
const props = defineProps<Props>();
const mediaApi = useMedia();
const showsApi = useShows();
let loadingEpisodes = ref(false);
let episodes = ref<EpisodeWithSubtitlesDto[] | null>([]);
const seasonPacks = ref<SeasonPackSubtitleDto[]>([]);
const refreshingProgress = ref<number | null>(null);
const downloadingProgress = ref<number | null>(null);
const downloadingInProgress = ref<boolean>(false);
const language = useLanguage();
const currentSeason = ref<number | undefined>(undefined);
const mediaInfo = ref<MediaDetailsDto>();
const subtitlesApi = useSubtitles();

await loadViewData();

const runtimeConfig = useRuntimeConfig();
let imageUrl = mediaInfo.value!.details?.backdropPath ?? mediaInfo.value!.details?.posterPath;
let twitterUrl = imageUrl;
if (imageUrl != null) {
  imageUrl += "?width=512&format=jpeg"
  twitterUrl += "?width=250&format=jpeg"
}

const seoSeasonSuffix = computed(() => {
  if (currentSeason.value == null) {
    return "";
  }

  return ` - Season ${currentSeason.value}`;
});

useSeoMeta({
  title: () => `Gestdown: Subtitles of ${mediaInfo.value!.media?.name}${seoSeasonSuffix.value}`,
  ogTitle: () => `Gestdown: Subtitles of ${mediaInfo.value!.media?.name}${seoSeasonSuffix.value}`,
  description: () => `Find all the subtitles in multiple language like English, French, etc ... your favorite show ${mediaInfo.value!.media?.name}${seoSeasonSuffix.value}`,
  ogDescription: () => `Find all the subtitles in multiple language like English, French, etc ... your favorite show ${mediaInfo.value!.media?.name}${seoSeasonSuffix.value}`,
  ogImage: new URL(imageUrl ?? '', runtimeConfig.public.api.clientUrl).href,
  articleTag: mediaInfo.value!.details?.genre ?? [],
  twitterImage: new URL(twitterUrl ?? '', runtimeConfig.public.api.clientUrl).href,
  ogImageAlt: () => `Poster of ${mediaInfo.value!.media?.name}${seoSeasonSuffix.value}`,
  twitterImageAlt: () => `Poster of ${mediaInfo.value!.media?.name}${seoSeasonSuffix.value}`,
  ogType: "website"
})

const {
  sendRefreshAsync,
  unsubscribeShowAsync,
  onProgress,
  offProgress,
  onDone,
  offDone,
  getEpisodes
} = useRefreshHub();

const progressHandler: ProgressHandler = (progress) => {
  if (refreshingProgress.value != null && progress.progress < refreshingProgress.value) {
    console.error("Got progress lower than current value");
    return;
  }
  refreshingProgress.value = progress.progress;
};

const doneHandler: DoneHandler = async (show) => {
  if (show.id != props.showId) {
    return;
  }
  refreshingProgress.value = null;
  await unsubscribeShowAsync(show.id!);

  mediaInfo.value = { details: mediaInfo.value?.details, media: show }
  if (currentSeason.value == undefined) {
    currentSeason.value = last(mediaInfo.value?.media?.seasons);
  }

  loadingEpisodes.value = true;
  episodes.value = await getEpisodes(show.id!, language.lang, currentSeason.value!);
  // Fetch season packs via REST since SignalR hub only streams episodes
  try {
    const response = (await showsApi.showsDetail(show.id!, currentSeason.value!, language.lang)).data;
    seasonPacks.value = response.seasonPacks ?? [];
  } catch {
    // Season packs are supplementary; don't fail the whole refresh
  }
  loadingEpisodes.value = false;
};


onProgress(progressHandler);
onDone(doneHandler);
onUnmounted(() => {
  offProgress(progressHandler);
  offDone(doneHandler);
});

async function loadViewData() {
  loadingEpisodes.value = true;

  const { data, error } = await useAsyncData(async () => (await mediaApi.episodesDetail(props.showId, language.lang)).data!);
  if (error.value != null) {
    throw createError({ statusCode: 404, statusMessage: `Show ${props.showId} not found` });
  }
  mediaInfo.value = data.value!.details;
  if (data.value?.lastSeasonNumber == null) {
    loadingEpisodes.value = false;
    return;
  }

  try {
    const lastSeason = data.value.lastSeasonNumber;
    const availableSeasons: number[] = mediaInfo.value?.media?.seasons ?? [];

    // Determine target season: use initialSeason if valid, otherwise latest.
    let targetSeason: number;
    if (props.initialSeason != null && availableSeasons.includes(props.initialSeason)) {
      targetSeason = props.initialSeason;
    } else if (props.initialSeason != null) {
      // Requested season doesn't exist — 404.
      const showName = mediaInfo.value?.media?.name ?? 'this show';
      throw createError({ statusCode: 404, statusMessage: `Season ${props.initialSeason} not found for ${showName}` });
    } else {
      targetSeason = lastSeason;
    }

    currentSeason.value = targetSeason;

    if (targetSeason === lastSeason) {
      episodes.value = data.value.episodeWithSubtitles;
      seasonPacks.value = data.value.seasonPacks ?? [];
    } else {
      // Fetch the specific requested season.
      const response = (await showsApi.showsDetail(props.showId, targetSeason, language.lang)).data;
      episodes.value = response.episodes ?? [];
      seasonPacks.value = response.seasonPacks ?? [];
    }
  } finally {
    loadingEpisodes.value = false;
  }
}

watch([currentSeason, language], async ([newSeason], [oldSeason]) => {
  if (loadingEpisodes.value) {
    console.warn("Loading episodes already in progress, skipping season change");
    return;
  }
  if (oldSeason == undefined && newSeason != undefined) {
    return;
  }
  loadingEpisodes.value = true;

  if (currentSeason.value != undefined) {
    const response = (await showsApi.showsDetail(props.showId, currentSeason.value!, language.lang)).data;
    episodes.value = response.episodes!;
    seasonPacks.value = response.seasonPacks ?? [];
  }

  loadingEpisodes.value = false;

  // Sync URL when the user changes season on the season route.
  if (newSeason !== undefined && newSeason !== oldSeason && route.name === 'show-season') {
    await navigateTo({
      name: 'show-season',
      params: { ...route.params, seasonNumber: newSeason }
    });
  }
})


const refreshShow = async () => {
  refreshingProgress.value = 0;
  await sendRefreshAsync(props.showId);
};

if (currentSeason.value == undefined) {
  await refreshShow();
  loadingEpisodes.value = true;
}

const formattedProgress = computed(() => {
  let keyword = "";
  if (refreshingProgress.value == null) {
    return "";
  }
  switch (true) {
    case refreshingProgress.value == 100:
      return `Ready`;
    case refreshingProgress.value < 25:
      keyword = "Fetching seasons";
      break;
    case refreshingProgress.value >= 25:
      keyword = "Fetching subtitles";
      break;
    default:
      keyword = "Fetching";
      break;
  }
  return `${keyword}: (${refreshingProgress.value}%)`;
});

const availableSubtitleTypes = computed(() => {
  if (!episodes.value || episodes.value.length === 0) {
    return SubtitleTypeFlag.None;
  }

  let hasRegular = false;
  let hasHearingImpaired = false;

  for (const episode of episodes.value) {
    for (const subtitle of episode.subtitles!) {
      if (!subtitle?.hearingImpaired) {
        hasRegular = true;
      } else {
        hasHearingImpaired = true;
      }
      if (hasRegular && hasHearingImpaired) {
        break;
      }
    }
  }

  let result = SubtitleTypeFlag.None;
  if (hasRegular) result |= SubtitleTypeFlag.Regular;
  if (hasHearingImpaired) result |= SubtitleTypeFlag.HearingImpaired;

  return result;
});

// Check if there are any episodes with subtitles
const hasEpisodes = computed(() => episodes.value?.some(e => (e.subtitles?.length ?? 0) > 0) ?? false);

// Check if there's only one subtitle type available
const onlyOneTypeAvailable = computed(() => {
  return availableSubtitleTypes.value === SubtitleTypeFlag.Regular ||
    availableSubtitleTypes.value === SubtitleTypeFlag.HearingImpaired;
});

const hasSeasonPacks = computed(() => seasonPacks.value.length > 0);

// Get the only available subtitle type if there's only one
const getOnlyAvailableType = (): SubtitleType | null => {
  if (availableSubtitleTypes.value === SubtitleTypeFlag.Regular) {
    return "regular";
  } else if (availableSubtitleTypes.value === SubtitleTypeFlag.HearingImpaired) {
    return "hearing_impaired";
  }
  return null;
};

// Handle download button click
const handleDownloadClick = () => {
  if (onlyOneTypeAvailable.value) {
    const type = getOnlyAvailableType();
    if (type) {
      downloadSeasonSubtitles(type);
    }
  }
  // If multiple types available, SubtitleTypeChooser dialog will open automatically
};

const downloadSeasonSubtitles = async (type: SubtitleType) => {
  downloadingInProgress.value = true;
  mevent("bulk-download-subtitles", { show: mediaInfo.value?.media?.name, season: currentSeason.value, type: type });
  const subtitles = episodes.value!.flatMap((e) => e.subtitles).filter((s) => type == "regular" ? !s?.hearingImpaired : s?.hearingImpaired);
  let downloaded = 0;
  const subtitleResponses = subtitles.map(async (s) => {
    try {
      const response = await subtitlesApi.downloadSubtitle(s!.subtitleId);
      if (!response.ok) {
        console.error(`Failed to download subtitle ${s?.subtitleId}`);
        return null;
      }

      s!.downloadCount++;

      const header = response.headers.get("Content-Disposition");
      const parts = header!.split(";");
      const filename = trim(parts[1].split("=")[1] ?? `${s?.subtitleId}.srt`, '"');
      downloaded++;
      downloadingProgress.value = downloaded / subtitles.length * 100;
      return { name: filename, input: response };
    } catch (e) {
      console.error(`Failed to download subtitle ${s?.subtitleId}`, e);
      return null;
    }
  });

  const responses = (await Promise.all(subtitleResponses)).filter((r) => r != null) as {
    name: string,
    input: Response
  }[];

  const zip = await downloadZip(responses, {
    buffersAreUTF8: true
  }).blob();
  downloadingProgress.value = 100;

  const url = URL.createObjectURL(zip);

  const link = document.createElement('a');
  link.href = url;
  link.rel = "noopener nofollow noreferrer";
  link.download = `${mediaInfo.value?.media?.name} - Season ${currentSeason.value} - ${type}.zip`;
  link.click();

  downloadingInProgress.value = false;
  downloadingProgress.value = null;
};
</script>

<template>
  <div>
    <media-details v-if="mediaInfo?.details != null" :details="mediaInfo" v-model="currentSeason" />
    <v-row justify="center" class="mt-3">
      <v-col cols="12" md="10" lg="8">
        <AdsUnit style="display:block" data-ad-client="ca-pub-7284443005140816" data-ad-slot="8373307473"
          data-ad-format="auto" data-full-width-responsive />
      </v-col>
    </v-row>
    <v-progress-linear v-if="refreshingProgress != null" v-model="refreshingProgress" color="blue" height="18"
      class="mt-2">
      {{ formattedProgress }}
    </v-progress-linear>
    <v-progress-linear v-if="downloadingProgress != null" v-model="downloadingProgress" color="blue" height="18"
      class="mt-2">
      Downloading subtitles
    </v-progress-linear>
    <div class="mt-2">
      <v-skeleton-loader type="card" :loading="loadingEpisodes">
        <v-sheet rounded="lg" :color="layout.colors.primaryPanel" :class="layout.classes.primaryPanel">
          <div class="d-flex align-center flex-wrap ga-2 mb-4">
            <h2 class="text-h6">Season {{ currentSeason }}</h2>
            <v-spacer />
            <div class="d-flex ga-2">
              <v-btn :prepend-icon="mdiRefresh" color="primary" size="small" @click="refreshShow"
                :disabled="refreshingProgress != null || downloadingInProgress">Refresh
                <v-tooltip activator="parent" location="bottom">Fetch from Addic7ed
                </v-tooltip>
              </v-btn>
              <v-btn v-if="onlyOneTypeAvailable && hasEpisodes" :prepend-icon="mdiDownload" color="primary" size="small"
                @click="handleDownloadClick" :disabled="refreshingProgress != null || downloadingInProgress">
                Download season
                <v-tooltip activator="parent" location="bottom">Download all subtitles of the season as ZIP file
                </v-tooltip>
              </v-btn>
              <v-btn v-else-if="hasEpisodes" :prepend-icon="mdiDownload" color="primary" size="small"
                :disabled="refreshingProgress != null || downloadingInProgress">
                <SubtitleTypeChooser @selected="downloadSeasonSubtitles" :available-types="availableSubtitleTypes" />
                Download season
                <v-tooltip activator="parent" location="bottom">Download all subtitles of the season as ZIP file
                </v-tooltip>
              </v-btn>
            </div>
          </div>
          <div v-if="hasSeasonPacks" class="mb-4">
            <h3 class="text-subtitle-1 font-weight-medium mb-2">Season Packs</h3>
            <season-packs-section :season-packs="seasonPacks" />
          </div>
          <v-divider v-if="hasSeasonPacks && hasEpisodes" class="mb-4" />
          <h3 v-if="hasSeasonPacks && hasEpisodes" class="text-subtitle-1 font-weight-medium mb-2">Episodes</h3>
          <subtitles-table :episodes="episodes" :season-pack-count="seasonPacks.length"></subtitles-table>
        </v-sheet>
      </v-skeleton-loader>
    </div>
  </div>
</template>

<style scoped></style>