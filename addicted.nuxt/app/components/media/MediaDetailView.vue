<script setup lang="ts">
import MediaDetails from "@/components/media/MediaDetails.vue";
import { onUnmounted, ref, computed } from "vue";
import SubtitlesTable from "@/components/shows/SubtitlesTable.vue";
import type { DoneHandler, ProgressHandler } from "~/composables/hub/RefreshHub";
import { useRefreshHub } from "~/composables/hub/RefreshHub";
import type { EpisodeWithSubtitlesDto, MediaDetailsDto } from "~/composables/api/data-contracts";
import { useMedia, useShows, useSubtitles } from "~/composables/rest/api";
import SubtitleTypeChooser from "~/components/media/Download/SubtitleTypeChooser.vue";
import type { SubtitleType } from "~/composables/dto/SubtitleType";
import { SubtitleTypeFlag } from "~/composables/dto/SubtitleType";
import { trim } from "~/composables/utils/trim";
import { downloadZip } from "client-zip";
import { mevent } from "~/composables/data/event";
import { mdiDownload, mdiRefresh } from "@mdi/js";
import { useSubtitleType } from "~/stores/subtitleType";
import { last } from "lodash-es";

export interface Props {
  showId: string;
}

const props = defineProps<Props>();
const mediaApi = useMedia();
const showsApi = useShows();
let loadingEpisodes = ref(false);
let episodes = ref<EpisodeWithSubtitlesDto[] | null>([]);
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

useSeoMeta({
  title: `Gestdown: Subtitles of ${mediaInfo.value!.media?.name}`,
  ogTitle: `Gestdown: Subtitles of ${mediaInfo.value!.media?.name}`,
  description: `Find all the subtitles in multiple language like English, French, etc ... your favorite show ${mediaInfo.value!.media?.name}`,
  ogDescription: `Find all the subtitles in multiple language like English, French, etc ... your favorite show ${mediaInfo.value!.media?.name}`,
  ogImage: new URL(imageUrl ?? '', runtimeConfig.public.api.clientUrl).href,
  articleTag: mediaInfo.value!.details?.genre ?? [],
  twitterImage: new URL(twitterUrl ?? '', runtimeConfig.public.api.clientUrl).href,
  ogImageAlt: `Poster of ${mediaInfo.value!.media?.name}`,
  twitterImageAlt: `Poster of ${mediaInfo.value!.media?.name}`,
  ogType: "website"
})

watch([currentSeason, language], async ([newSeason], [oldSeason]) => {
  loadingEpisodes.value = true;
  if (oldSeason == undefined && newSeason != undefined) {
    return;
  }
  const {
    data,
  } = await useAsyncData(async () => {
    if (currentSeason.value == undefined) {
      return [];
    }
    return (await showsApi.showsDetail(props.showId, currentSeason.value!, language.lang)).data.episodes!;
  });

  episodes.value = data.value;
  loadingEpisodes.value = false;
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
  loadingEpisodes.value = false;
};


onProgress(progressHandler);
onDone(doneHandler);
onUnmounted(() => {
  offProgress(progressHandler);
  offDone(doneHandler);
});

async function loadViewData() {
  const { data, error } = await useAsyncData(async () => (await mediaApi.episodesDetail(props.showId, language.lang)).data!);
  if (error.value != null) {
    throw createError({ statusCode: 404, statusMessage: `Show ${props.showId} not found` });
  }
  mediaInfo.value = data.value!.details;
  if (data.value?.lastSeasonNumber == null) {
    return;
  }
  currentSeason.value = data.value?.lastSeasonNumber
  episodes.value = data.value?.episodeWithSubtitles;
}


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

// Check if there's only one subtitle type available
const onlyOneTypeAvailable = computed(() => {
  return availableSubtitleTypes.value === SubtitleTypeFlag.Regular ||
    availableSubtitleTypes.value === SubtitleTypeFlag.HearingImpaired;
});

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
    <v-row v-if="mediaInfo?.details != null">
      <v-col cols="12" offset="0" lg="8" offset-lg="2">
        <media-details :details="mediaInfo" v-model="currentSeason" />
      </v-col>
    </v-row>
    <v-row justify="center" v-if="refreshingProgress != null">
      <v-col cols="10">
        <v-col cols="11" align-self="center">
          <v-progress-linear v-model="refreshingProgress" color="blue" height="18">
            {{ formattedProgress }}
          </v-progress-linear>
        </v-col>
      </v-col>
    </v-row>
    <v-row justify="center" v-if="downloadingProgress != null">
      <v-col cols="10">
        <v-col cols="11" align-self="center">
          <v-progress-linear v-model="downloadingProgress" color="blue" height="18">
            Downloading subtitles
          </v-progress-linear>
        </v-col>
      </v-col>
    </v-row>
    <v-row justify="center">
      <v-col cols="12">
        <v-skeleton-loader type="card" :loading="loadingEpisodes">
          <v-card width="100%">
            <v-card-title>Season {{ currentSeason }}</v-card-title>
            <v-card-actions>
              <v-btn :prepend-icon="mdiRefresh" class="text-none mb-4" color="indigo-lighten-3" @click="refreshShow"
                :disabled="refreshingProgress != null || downloadingInProgress">Refresh
                <v-tooltip activator="parent" location="end">Fetch from Addic7ed
                </v-tooltip>
              </v-btn>
              <v-spacer></v-spacer>
              <v-btn v-if="onlyOneTypeAvailable" :prepend-icon="mdiDownload" class="text-none mb-4"
                color="indigo-lighten-3" @click="handleDownloadClick"
                :disabled="refreshingProgress != null || downloadingInProgress">
                Download season
                <v-tooltip activator="parent" location="end">Download all subtitles of the season as ZIP file
                </v-tooltip>
              </v-btn>
              <v-btn v-else :prepend-icon="mdiDownload" class="text-none mb-4" color="indigo-lighten-3"
                :disabled="refreshingProgress != null || downloadingInProgress">
                <SubtitleTypeChooser @selected="downloadSeasonSubtitles" :available-types="availableSubtitleTypes" />
                Download season
                <v-tooltip activator="parent" location="end">Download all subtitles of the season as ZIP file
                </v-tooltip>
              </v-btn>
            </v-card-actions>
            <v-card-text>
              <subtitles-table :episodes="episodes"></subtitles-table>
            </v-card-text>
          </v-card>
        </v-skeleton-loader>
      </v-col>
    </v-row>
  </div>
</template>

<style scoped></style>