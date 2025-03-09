<script setup lang="ts">
import * as vue from "vue";
import type { SubtitleType } from "~/composables/dto/SubtitleType";
import { useSubtitleType } from "~/stores/subtitleType";
import { mdiEarHearingOff, mdiFile, mdiSubtitles } from "@mdi/js";

interface Props {
  availableTypes?: {
    regular: boolean;
    hearing_impaired: boolean;
  };
}

const props = defineProps<Props>();
const subtitleType = useSubtitleType();

// eslint-disable-next-line no-undef
const emit = defineEmits<{
  (e: "selected", type: SubtitleType): void,
}>();

// Derive onlyOneType from availableTypes
const onlyOneType = computed(() => {
  if (!props.availableTypes) return false;
  const { regular, hearing_impaired } = props.availableTypes;
  return (regular && !hearing_impaired) || (!regular && hearing_impaired);
});

// Set default subtitle type based on availability
vue.watchEffect(() => {
  if (props.availableTypes) {
    if (props.availableTypes.regular && !subtitleType.type) {
      subtitleType.type = "regular";
    } else if (props.availableTypes.hearing_impaired && !subtitleType.type) {
      subtitleType.type = "hearing_impaired";
    }
  }
});

// Automatically select the only available type if there's only one
const onDialogOpen = (dialogRef: vue.Ref<boolean>) => {
  if (onlyOneType.value) {
    // If there's only one type available, select it and emit directly
    if (props.availableTypes?.regular && !props.availableTypes?.hearing_impaired) {
      subtitleType.type = "regular";
      emit("selected", "regular");
      // Don't open dialog
      return false;
    }
    else if (!props.availableTypes?.regular && props.availableTypes?.hearing_impaired) {
      subtitleType.type = "hearing_impaired";
      emit("selected", "hearing_impaired");
      // Don't open dialog
      return false;
    }
  }
  return true;
};

const onDialogDownload = (dialogRef: vue.Ref<boolean>) => {
  if (subtitleType.type == null) return;
  dialogRef.value = false;
  emit("selected", subtitleType.type);
};
</script>

<template>
  <v-dialog width="auto" scrollable activator="parent" @click:outside="(e) => onDialogOpen(e)">
    <template v-slot:default="{ isActive }">
      <v-card v-if="onDialogOpen(isActive)" :prepend-icon="mdiFile" title="Select Subtitle Type">
        <v-divider class="mt-3"></v-divider>

        <v-card-text class="px-4" style="height: 300px;">
          <v-radio-group :model-value="subtitleType.type"
            @update:model-value="(v) => subtitleType.type = v as SubtitleType" column>
            <v-radio v-if="!availableTypes || availableTypes.regular" value="regular">
              <template v-slot:label>
                <v-icon start>{{ mdiSubtitles }}</v-icon>
                Regular
              </template>
            </v-radio>

            <v-radio v-if="!availableTypes || availableTypes.hearing_impaired" value="hearing_impaired">
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