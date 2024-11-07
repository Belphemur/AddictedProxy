<script setup lang="ts">
import {encodeQueryItem, joinURL} from "ufo";

// Define the breakpoints
const breakpoints = {
  xs: 0,
  sm: 600,
  md: 900,
  lg: 1200,
  xl: 1536,
} as const;

// Define a type for the breakpoint keys
type BreakpointKey = keyof typeof breakpoints;

export interface Props {
  src: string;
  sources: PictureSource[];
  alt: string;
  formats: SupportedFormat[];
}

export interface PictureSource {
  size: BreakpointKey;
  width?: number | string;
  height?: number | string;
}

export type SupportedFormat = 'webp' | 'jpeg' | 'png'

const props = defineProps<Props>();
const baseUrl = useRuntimeConfig().public.api.clientUrl;

// Function to get the max-width media query
const toMediaQuery = (size: BreakpointKey): string => {
  const maxWidth = breakpoints[size];

  // Special case for 'xs' as it doesn't have a max-width
  if (size === 'xs') {
    return `(max-width: ${breakpoints.sm - 1}px)`;
  }

  // For other sizes, use the next breakpoint's value minus 1
  const sizes = Object.keys(breakpoints) as BreakpointKey[];
  const currentIndex = sizes.indexOf(size);
  const nextSize = sizes[currentIndex + 1];

  if (nextSize) {
    return `(max-width: ${breakpoints[nextSize] - 1}px)`;
  }

  // If it's the largest size, there's no upper limit
  return `(min-width: ${maxWidth}px)`;
}

const vXToPx = (vX: string): number => {
  if (window == undefined) {
    throw new Error('window is not defined. Using vw/vh only works if rendered client side.');
  }
  const type = vX.slice(-2);
  const size = parseInt(vX.slice(0, -2), 10);
  const viewportSize = type === 'vw' ? window.innerWidth : window.innerHeight;
  return (viewportSize / 100) * size;
}

const toSrcSet = (source: PictureSource, format: SupportedFormat | null) => {
  let srcSet = props.src;
  const queryParams: Record<string, any> = {};

  if (format) {
    queryParams.format = format;
  }
  if (source.width) {
    queryParams.width = typeof source.width === 'string' ? vXToPx(source.width) : source.width;
  }
  if (source.height) {
    queryParams.height = typeof source.height === 'string' ? vXToPx(source.height) : source.height;
  }

  const queryString = Object.keys(queryParams)
      .map(key => `${encodeQueryItem(key, queryParams[key])}`)
      .join('&');

  if (queryString) {
    srcSet += `?${queryString}`;
  }

  return joinURL(baseUrl, srcSet);
}

const toImageType = (format: SupportedFormat) => {
  switch (format) {
    case 'webp':
      return 'image/webp';
    case 'jpeg':
      return 'image/jpeg';
    case 'png':
      return 'image/png';
  }
}

</script>

<template>
  <picture>
    <template v-for="source in props.sources">
      <template v-for="format  in props.formats">
        <source :srcset="toSrcSet(source, format)" :media="toMediaQuery(source.size)" :type="toImageType(format)" :width="source.width" :height="source.height">
      </template>
    </template>
    <img :src="toSrcSet(props.sources.at(-1), props.formats[0])" :alt="alt">
  </picture>
</template>

<style scoped>

</style>