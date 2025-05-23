<script setup lang="ts">
import * as vue from "vue";
import type { SubtitleType } from "~/composables/dto/SubtitleType";
import { SubtitleTypeFlag } from "~/composables/dto/SubtitleType";
import { useSubtitleType } from "~/stores/subtitleType";
import { mdiEarHearingOff, mdiFile, mdiSubtitles } from "@mdi/js";

interface Props {
  availableTypes?: SubtitleTypeFlag;
}

const props = defineProps<Props>();
const subtitleType = useSubtitleType();

// eslint-disable-next-line no-undef
const emit = defineEmits<{
  (e: "selected", type: SubtitleType): void,
}>();

// Check if specific subtitle type is available
const isTypeAvailable = (type: SubtitleTypeFlag) => {
  if (!props.availableTypes) return true;
  return (props.availableTypes & type) === type;
};

// Set default subtitle type based on availability
vue.watchEffect(() => {
  if (props.availableTypes) {
    if (isTypeAvailable(SubtitleTypeFlag.Regular) && !subtitleType.type) {
      subtitleType.type = "regular";
    } else if (isTypeAvailable(SubtitleTypeFlag.HearingImpaired) && !subtitleType.type) {
      subtitleType.type = "hearing_impaired";
    }
  }
});

const onDialogDownload = (dialogRef: vue.Ref<boolean>) => {
  if (subtitleType.type == null) return;
  dialogRef.value = false;
  emit("selected", subtitleType.type);
};
</script>

<template>
  <v-dialog width="auto" scrollable activator="parent">
    <template v-slot:default="{ isActive }">
      <v-card :prepend-icon="mdiFile" title="Select Subtitle Type">
        <v-divider class="mt-3"></v-divider>

        <v-card-text class="px-4" style="height: 300px;">
          <v-radio-group :model-value="subtitleType.type"
            @update:model-value="(v) => subtitleType.type = v as SubtitleType" column>
            <v-radio v-if="isTypeAvailable(SubtitleTypeFlag.Regular)" value="regular">
              <template v-slot:label>
                <v-icon start>{{ mdiSubtitles }}</v-icon>
                Regular
              </template>
            </v-radio>

            <v-radio v-if="isTypeAvailable(SubtitleTypeFlag.HearingImpaired)" value="hearing_impaired">
              <template v-slot:label>
                <v-icon start>{{ mdiEarHearingOff }}</v-icon>
                Additional audio descriptions
              </template>
            </v-radio>
          </v-radio-group>
        </v-card-text>

        <v-divider></v-divider>

        <v-card-actions>
          <v-btn text="Close" @click="isActive.value = false"></v-btn>

          <v-spacer></v-spacer>

          <v-btn color="surface-variant" text="Download" variant="flat" @click="onDialogDownload(isActive)"></v-btn>
        </v-card-actions>
      </v-card>
    </template>
  </v-dialog>
</template>

<style scoped></style>