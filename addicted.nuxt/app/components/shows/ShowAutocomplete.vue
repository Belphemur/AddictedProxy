<template>
    <div class="search-container" ref="searchContainer">
        <v-text-field label="Name of the show" v-model="searchQuery" @input="onInput"
            @keydown.down.prevent="onArrowDown" @keydown.up.prevent="onArrowUp" @keydown.enter.prevent="onEnter"
            @keydown.escape="hideResults" @focus="onFocus" clearable :error-messages="error" :loading="isLoading"
            :prepend-inner-icon="mdiTelevision" @click:clear="clearSearch" />
        <v-card v-if="showResults" class="results-list" elevation="3">
            <v-list density="compact">
                <v-list-item v-for="(result, index) in results" :key="result.id || index"
                    :class="{ 'v-list-item--active': index === arrowCounter }" @click="selectResult(result)"
                    @mouseenter="arrowCounter = index">
                    <v-list-item-title v-html="highlightMatch(result.name)"></v-list-item-title>
                </v-list-item>
            </v-list>
        </v-card>
    </div>
</template>

<script setup lang="ts">
import { ref, watch, onMounted, onUnmounted, nextTick, computed } from 'vue';
import type { ShowDto } from "~/composables/api/data-contracts";
import { mdiTelevision } from "@mdi/js";

const emit = defineEmits<{
    (e: "selected", show: ShowDto): void,
    (e: "cleared"): void;
    (e: "search", query: string): void;
}>();

const props = defineProps<{
    results: ShowDto[],
    isLoading: boolean,
    error?: string,
}>();

const searchQuery = ref("");
const showResults = ref(false);
const arrowCounter = ref(-1);
const searchContainer = ref<HTMLElement>();

watch(() => props.results, () => {
    showResults.value = props.results.length > 0 && searchQuery.value.length >= 3;
    arrowCounter.value = -1;
});

const onInput = () => {
    const query = searchQuery.value.trim();
    showResults.value = false; // Hide results while typing
    if (query.length >= 3) {
        emit('search', query);
    } else if (query.length === 0) {
        emit('cleared');
    }
};

const onFocus = () => {
    if (props.results.length > 0 && searchQuery.value.length >= 3) {
        showResults.value = true;
    }
};

const onArrowDown = () => {
    if (!showResults.value) return;

    if (arrowCounter.value < props.results.length - 1) {
        arrowCounter.value++;
    } else {
        arrowCounter.value = 0; // Loop to first item
    }
    scrollToActiveItem();
};

const onArrowUp = () => {
    if (!showResults.value) return;

    if (arrowCounter.value > 0) {
        arrowCounter.value--;
    } else {
        arrowCounter.value = props.results.length - 1; // Loop to last item
    }
    scrollToActiveItem();
};

const scrollToActiveItem = async () => {
    await nextTick();
    const activeItem = searchContainer.value?.querySelector('.v-list-item--active');
    if (activeItem) {
        activeItem.scrollIntoView({ block: 'nearest' });
    }
};

const onEnter = () => {
    if (showResults.value && arrowCounter.value >= 0) {
        const result = props.results[arrowCounter.value];
        if (result) {
            selectResult(result);
        }
    }
};

const selectResult = (result: ShowDto) => {
    searchQuery.value = result.name ?? '';
    hideResults();
    emit('selected', result);
};

const hideResults = () => {
    showResults.value = false;
    arrowCounter.value = -1;
};

const clearSearch = () => {
    searchQuery.value = "";
    hideResults();
    emit('cleared');
};

// Memoized regex pattern for performance
const highlightRegex = computed(() => {
    const query = searchQuery.value.trim();
    if (!query) return null;

    // Escape special regex characters
    const escapedQuery = query.replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
    return new RegExp(`(${escapedQuery})`, 'gi');
});

// Optimized highlight function using computed regex
const highlightMatch = (text: string): string => {
    if (!text || !highlightRegex.value) {
        return text || '';
    }

    return text.replace(highlightRegex.value, '<span class="highlight">$1</span>');
};

// Click outside to close
const handleClickOutside = (event: MouseEvent) => {
    if (searchContainer.value && !searchContainer.value.contains(event.target as Node)) {
        hideResults();
    }
};

onMounted(() => {
    document.addEventListener('click', handleClickOutside);
});

onUnmounted(() => {
    document.removeEventListener('click', handleClickOutside);
});

</script>

<style scoped>
.search-container {
    position: relative;
}

.results-list {
    position: absolute;
    width: 100%;
    max-height: 300px;
    overflow-y: auto;
    z-index: 1000;
    margin-top: 4px;
}

.results-list :deep(.v-list) {
    padding: 0;
}

.results-list :deep(.v-list-item) {
    min-height: 40px;
    transition: background-color 0.2s ease;
}

.results-list :deep(.v-list-item:hover),
.results-list :deep(.v-list-item--active) {
    background-color: rgba(var(--v-theme-primary), 0.1);
}

.results-list :deep(.v-list-item-title) {
    font-size: 0.875rem;
}

.results-list :deep(.highlight) {
    background-color: rgba(var(--v-theme-primary), 0.3);
    font-weight: 600;
    border-radius: 2px;
    padding: 1px 2px;
}
</style>