<script setup lang="ts">
import MediaDetails from "@/components/media/MediaDetails.vue";
import { ref, computed } from "vue";
import SubtitlesTable from "@/components/shows/SubtitlesTable.vue";
import SeasonPacksSection from "@/components/media/SeasonPacksSection.vue";
import type { EpisodeWithSubtitlesDto, MediaDetailsDto, SeasonPackSubtitleDto } from "~/composables/api/data-contracts";
import { useMedia, useShows, useSubtitles } from "~/composables/rest/api";
import SubtitleTypeChooser from "~/components/media/Download/SubtitleTypeChooser.vue";
import type { SubtitleType } from "~/composables/dto/SubtitleType";
import { SubtitleTypeFlag } from "~/composables/dto/SubtitleType";
import { trim } from "~/composables/utils/trim";
import { downloadZip } from "client-zip";
import { mevent } from "~/composables/data/event";
import { usePageLayout } from "~/composables/usePageLayout";
import { mdiDownload } from "@mdi/js";
import { last } from "lodash-es";

export interface Props {
  showId: string;
}

const layout = usePageLayout();
const props = defineProps<Props>();
const mediaApi = useMedia();
const showsApi = useShows();
let loadingEpisodes = ref(false);
let episodes = ref<EpisodeWithSubtitlesDto[] | null>([]);
const seasonPacks = ref<SeasonPackSubtitleDto[]>([]);
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

const seoSeasonList = computed(() => {
  const seasons = mediaInfo.value?.media?.seasons;
  if (!seasons || seasons.length === 0) {
    return "";
  }
  return ` Seasons available: ${seasons.join(", ")}.`;
});

useSeoMeta({
  title: () => `Gestdown: Subtitles of ${mediaInfo.value!.media?.name}`,
  ogTitle: () => `Gestdown: Subtitles of ${mediaInfo.value!.media?.name}`,
  description: () => `Find all the subtitles in multiple languages like English, French, etc... for your favorite show ${mediaInfo.value!.media?.name}.${seoSeasonList.value}`,
  ogDescription: () => `Find all the subtitles in multiple languages like English, French, etc... for your favorite show ${mediaInfo.value!.media?.name}.${seoSeasonList.value}`,
  ogImage: new URL(imageUrl ?? '', runtimeConfig.public.api.clientUrl).href,
  articleTag: mediaInfo.value!.details?.genre ?? [],
  twitterImage: new URL(twitterUrl ?? '', runtimeConfig.public.api.clientUrl).href,
  ogImageAlt: () => `Poster of ${mediaInfo.value!.media?.name}`,
  twitterImageAlt: () => `Poster of ${mediaInfo.value!.media?.name}`,
  ogType: "website"
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

    currentSeason.value = lastSeason;
    episodes.value = data.value.episodeWithSubtitles;
    seasonPacks.value = data.value.seasonPacks ?? [];
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
})


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
              <v-btn v-if="onlyOneTypeAvailable && hasEpisodes" :prepend-icon="mdiDownload" color="primary" size="small"
                @click="handleDownloadClick" :disabled="downloadingInProgress">
                Download season
                <v-tooltip activator="parent" location="bottom">Download all subtitles of the season as ZIP file
                </v-tooltip>
              </v-btn>
              <v-btn v-else-if="hasEpisodes" :prepend-icon="mdiDownload" color="primary" size="small"
                :disabled="downloadingInProgress">
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
