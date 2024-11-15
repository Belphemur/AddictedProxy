<script setup lang="ts">
import * as vue from "vue";
import type {SubtitleType} from "~/composables/dto/SubtitleType";
import {useSubtitleType} from "~/stores/subtitleType";
import {mdiEarHearingOff, mdiFile, mdiSubtitles} from "@mdi/js";

const subtitleType = useSubtitleType();
// eslint-disable-next-line no-undef
const emit = defineEmits<{
  (e: "selected", type: SubtitleType): void,
}>();

const onDialogDownload = (dialogRef: vue.Ref<boolean>) => {
  if (subtitleType.type == null) return;
  dialogRef.value = false;
  emit("selected", subtitleType.type);
}

</script>

<template>
  <v-dialog
      width="auto"
      scrollable
      activator="parent"
  >
    <template v-slot:default="{ isActive }">
      <v-card
          :prepend-icon="mdiFile"
          title="Select Subtitle Type"
      >
        <v-divider class="mt-3"></v-divider>

        <v-card-text class="px-4" style="height: 300px;">
          <v-radio-group
              :model-value="subtitleType.type"
              @update:model-value="(v) => subtitleType.type = v as SubtitleType"
              column
          >
            <v-radio
                value="regular"
            >
              <template v-slot:label>
                <v-icon start >{{ mdiSubtitles }}</v-icon>
                Regular
              </template>
            </v-radio>

            <v-radio
                value="hearing_impaired"
            >
              <template v-slot:label>
                <v-icon start >{{mdiEarHearingOff}}</v-icon>
                Additional audio descriptions
              </template>
            </v-radio>
          </v-radio-group>
        </v-card-text>

        <v-divider></v-divider>

        <v-card-actions>
          <v-btn
              text="Close"
              @click="isActive.value = false"
          ></v-btn>

          <v-spacer></v-spacer>

          <v-btn
              color="surface-variant"
              text="Download"
              variant="flat"
              @click="onDialogDownload(isActive)"
          ></v-btn>
        </v-card-actions>
      </v-card>
    </template>
  </v-dialog>
</template>

<style scoped>

</style>