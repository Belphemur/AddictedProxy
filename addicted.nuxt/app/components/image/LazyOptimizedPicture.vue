<script setup lang="ts">
import OptimizedPicture from "~/components/image/OptimizedPicture.vue";
import type {PictureSource, SupportedFormat} from "~/components/image/OptimizedPicture.vue";

export interface Props {
  src: string;
  sources: PictureSource[];
  alt: string;
  formats: SupportedFormat[];
  preload?: boolean;
  placeholderText?: string;
  placeholderColor?: string;
}

const props = withDefaults(defineProps<Props>(), {
  preload: false,
  placeholderColor: '#1a1a1a'
});

const isLoaded = ref(false);
const hasError = ref(false);
const imageRef = ref<HTMLElement | null>(null);
const shouldLoad = ref(false);

// Use Intersection Observer for lazy loading
onMounted(() => {
  if (!imageRef.value) return;

  // If preload is true, load immediately
  if (props.preload) {
    shouldLoad.value = true;
    return;
  }

  const observer = new IntersectionObserver(
      (entries) => {
        entries.forEach((entry) => {
          if (entry.isIntersecting) {
            shouldLoad.value = true;
            observer.disconnect();
          }
        });
      },
      {
        rootMargin: '50px', // Start loading 50px before the image enters viewport
        threshold: 0.01
      }
  );

  observer.observe(imageRef.value);

  onUnmounted(() => {
    observer.disconnect();
  });
});

const onImageLoad = () => {
  isLoaded.value = true;
};

const onImageError = () => {
  hasError.value = true;
  isLoaded.value = true; // Hide placeholder even on error
};

// Generate initials from placeholder text for a nicer placeholder
const initials = computed(() => {
  if (!props.placeholderText) return '';
  const words = props.placeholderText.split(' ').filter(w => w.length > 0);
  if (words.length === 1) {
    return (words[0] ?? '').substring(0, 2).toUpperCase();
  }
  return words.slice(0, 2).map(w => (w ?? '')[0]?.toUpperCase() ?? '').join('');
});
</script>

<template>
  <div ref="imageRef" class="lazy-image-wrapper">
    <!-- Placeholder -->
    <div v-if="!isLoaded" class="lazy-image-placeholder" :style="{ backgroundColor: placeholderColor }">
      <svg viewBox="0 0 100 100" xmlns="http://www.w3.org/2000/svg" class="placeholder-svg">
        <rect width="100" height="100" :fill="placeholderColor"/>
        <text
          x="50"
          y="50"
          text-anchor="middle"
          dominant-baseline="middle"
          fill="rgba(255, 255, 255, 0.9)"
          font-size="16"
          font-weight="bold"
          font-family="system-ui, -apple-system, sans-serif"
        >
          {{ initials }}
        </text>
        <text
          x="50"
          y="70"
          text-anchor="middle"
          dominant-baseline="middle"
          fill="rgba(255, 255, 255, 0.7)"
          font-size="6"
          font-family="system-ui, -apple-system, sans-serif"
          class="placeholder-text"
        >
          {{ placeholderText }}
        </text>
      </svg>
      <!-- Loading spinner -->
      <div class="loading-spinner">
        <svg width="40" height="40" viewBox="0 0 50 50" class="spinner">
          <circle
            cx="25"
            cy="25"
            r="20"
            fill="none"
            stroke="rgba(255, 255, 255, 0.6)"
            stroke-width="4"
            stroke-dasharray="80, 200"
            stroke-linecap="round"
          />
        </svg>
      </div>
    </div>

    <!-- Actual image (loaded lazily) -->
    <div
      v-show="shouldLoad"
      class="lazy-image-content"
      :class="{ 'is-loaded': isLoaded, 'has-error': hasError }"
    >
      <optimized-picture
        v-if="shouldLoad"
        :src="src"
        :sources="sources"
        :alt="alt"
        :formats="formats"
        :preload="preload"
        @load="onImageLoad"
        @error="onImageError"
      />
    </div>
  </div>
</template>

<style scoped>
.lazy-image-wrapper {
  position: relative;
  width: 100%;
  height: 100%;
  min-height: 225px; /* Minimum height to ensure placeholder is visible */
  overflow: hidden;
  display: block;
}

.lazy-image-placeholder {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  min-height: 225px; /* Match wrapper min-height */
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #1a1a1a 0%, #2d2d2d 100%);
  animation: pulse 2s cubic-bezier(0.4, 0, 0.6, 1) infinite;
  z-index: 1;
}

.placeholder-svg {
  width: 100%;
  height: 100%;
  min-height: 225px;
}

.placeholder-text {
  opacity: 0.9;
}

/* Wrap long text in SVG */
@media (max-width: 600px) {
  .placeholder-text {
    font-size: 4px;
  }
}

.loading-spinner {
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  margin-top: 30px;
  z-index: 2;
}

.spinner {
  animation: rotate 2s linear infinite;
}

.spinner circle {
  animation: dash 1.5s ease-in-out infinite;
}

@keyframes rotate {
  100% {
    transform: rotate(360deg);
  }
}

@keyframes dash {
  0% {
    stroke-dasharray: 1, 200;
    stroke-dashoffset: 0;
  }
  50% {
    stroke-dasharray: 89, 200;
    stroke-dashoffset: -35px;
  }
  100% {
    stroke-dasharray: 89, 200;
    stroke-dashoffset: -124px;
  }
}

@keyframes pulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.8;
  }
}

.lazy-image-content {
  position: relative;
  opacity: 0;
  transition: opacity 0.4s ease-in-out;
  width: 100%;
  height: 100%;
  z-index: 0;
}

.lazy-image-content.is-loaded {
  opacity: 1;
  z-index: 2; /* Move above placeholder when loaded */
}

.lazy-image-content.has-error {
  display: none;
}

/* Ensure images inside maintain their styling */
.lazy-image-content :deep(picture),
.lazy-image-content :deep(img) {
  display: block;
  width: 100%;
  height: auto;
  object-fit: cover;
}
</style>
