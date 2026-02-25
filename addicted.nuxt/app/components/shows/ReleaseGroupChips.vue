<template>
    <div class="d-flex flex-wrap ga-1">
        <v-chip v-for="group in groups" :key="group" :color="getColor(group)" size="small" label>
            {{ group }}
        </v-chip>
    </div>
</template>

<script setup lang="ts">
import { useGroupColor } from "~/composables/useGroupColor";

interface Props {
    groups: string[];
}

defineProps<Props>();
const { getColor } = useGroupColor();
</script>
