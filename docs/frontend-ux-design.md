# Frontend UX & Design System

This document defines the visual design patterns, component conventions, and responsive strategies used in the Gestdown (AddictedProxy) Nuxt 4 frontend. All frontend work **must** follow these patterns for visual consistency.

## Tech Stack

- **Framework:** Nuxt 4 (Vue 3.5+), Vuetify 3
- **Icons:** `@mdi/js` (tree-shakeable SVG imports per component)
- **Font:** Google Fonts Roboto (weights: 100, 300, 400, 500, 700, 900)
- **Theme:** Vuetify `dark` theme (default)
- **Device detection:** `@nuxtjs/device` providing `useDevice()` composable (SSR user-agent based)

## Glassmorphism Design

The entire UI is built around **glass panels over a blurred background**. This is the core visual identity.

### Blurred Background

A fixed full-viewport background image (`/img/background-small.webp`) with `filter: blur(8px)` sits behind all content at `z-index: -1`. The `v-app` has `background: "none"` to remove Vuetify's default background.

### Glass Panel Hierarchy

All content panels use semi-transparent `v-sheet` components with `rounded="lg"`:

| Layer | Color | Usage |
|-------|-------|-------|
| **Primary panel** | `rgba(0,0,0,0.75)` | Main content containers (hero, show details, season card) |
| **App bar** | `rgba(0,0,0,0.7)` | Top navigation bar |
| **Nested item** | `rgba(255,255,255,0.08)` | Cards inside panels (mobile subtitle items) |
| **Expansion panel** | `rgba(255,255,255,0.05)` | Mobile episode accordions |
| **Transparent overlay** | `transparent` | Tables, expansion containers that inherit parent bg |

```html
<!-- Primary panel -->
<v-sheet rounded="lg" color="rgba(0,0,0,0.75)" class="pa-4 pa-sm-6">
  <!-- content -->
</v-sheet>

<!-- Nested item inside a panel -->
<v-sheet rounded="lg" color="rgba(255,255,255,0.08)" class="pa-3">
  <!-- subtitle card -->
</v-sheet>
```

### Transparent Data Table

Desktop data tables must be transparent to inherit the glass panel background:

```html
<v-data-table class="transparent-table" ...>
```

```css
.transparent-table {
  background: transparent !important;
}
.transparent-table :deep(.v-table__wrapper) {
  background: transparent;
}
```

## Color Palette

| Usage | Color | Value |
|-------|-------|-------|
| Primary actions/buttons | `primary` | Theme color (`#1867C0`) |
| Year highlight text | `text-light-blue-accent-1` | Vuetify utility |
| Error states | `error` | Vuetify error red |
| Success/completed | `success` | Vuetify success green |
| Source: SuperSubtitles | `teal` | Chip color |
| Source: Addic7ed | `blue-darken-2` | Chip color |
| Vote circle | `yellow` | Progress circular |
| Progress bars | `blue` | Linear progress |
| De-emphasized text | `text-medium-emphasis` | Vuetify utility class |

## Spacing Conventions

### Responsive Padding

Use responsive padding classes that scale up at the `sm` breakpoint:

| Context | Classes | Description |
|---------|---------|-------------|
| Primary panels | `pa-4 pa-sm-5` or `pa-4 pa-sm-6` | Roomier on desktop |
| Nested cards (mobile) | `pa-3` | Compact |
| Page container | `pa-2 pa-sm-4` | Tight on mobile |

### Margin Patterns

- `mb-4` — Between major sections
- `mt-2` — Sub-sections, progress bars
- `mt-1` — Minor spacing
- `mb-2` / `mb-3` — List items
- `ga-2` — Flex gap in button groups, chip rows

### Container Width

```html
<v-container fluid class="pa-2 pa-sm-4" style="max-width: 1600px">
```

## Typography

| Usage | Classes |
|-------|---------|
| Page title (hero) | `text-h5 text-sm-h4 font-weight-bold` |
| Panel title (show name) | `text-h5` |
| Section header | `text-h6` |
| Label/subtitle | `text-subtitle-1 font-weight-bold` |
| Body (mobile) | `text-body-2` |
| De-emphasized | `text-medium-emphasis` |
| Tagline | `text-medium-emphasis` + `<i>` |

## Button Patterns

### Standard Action Button

```html
<v-btn color="primary" size="small" class="text-none" :prepend-icon="mdiRefresh">
  Refresh
  <v-tooltip activator="parent" location="bottom">Description</v-tooltip>
</v-btn>
```

**Rules:**
- **Color:** Always `color="primary"` for action buttons
- **Size:** `size="small"` for in-panel actions
- **Text casing:** Always `class="text-none"` (prevents Vuetify uppercase)
- **Icons:** Use `prepend-icon` with `@mdi/js` SVG paths (never icon fonts)
- **Tooltips:** Nest `<v-tooltip activator="parent" location="bottom">` inside the button

### Button Variants

| Context | Pattern |
|---------|---------|
| Primary action | `color="primary"` (default filled) |
| Nav button | `variant="text" size="small"` |
| Text toggle | `variant="text" size="small" class="text-none text-medium-emphasis"` |
| Dialog action | `color="primary" variant="flat"` |
| Table download | `color="primary" :prepend-icon="mdiDownload"` |

### Button Grouping

When a header and buttons share a row, group buttons in a nested `div` to prevent awkward wrapping on mobile:

```html
<div class="d-flex align-center flex-wrap ga-2 mb-4">
  <h2 class="text-h6">Section Title</h2>
  <v-spacer />
  <div class="d-flex ga-2">
    <v-btn ...>Action 1</v-btn>
    <v-btn ...>Action 2</v-btn>
  </div>
</div>
```

## Responsive Design (Mobile vs Desktop)

### Device Detection

Uses `@nuxtjs/device` which detects from the **server-side request User-Agent header**. This means SSR renders the correct layout based on the requesting device.

```ts
const device = useDevice();
// or
const { isMobile } = useDevice();
```

### Conditional Rendering Patterns

| Feature | Desktop | Mobile |
|---------|---------|--------|
| Subtitles display | `<v-data-table>` with `group-by` | `<v-expansion-panels>` with cards |
| Overview text | 3-line CSS clamp, always visible | Collapsible toggle (hidden by default) |
| Poster image | Portrait poster | Backdrop landscape (portrait orientation) |

### Mobile Overview Toggle

On mobile, long text sections (like overview) should be collapsible and hidden by default:

```html
<div v-if="device.isMobile">
  <v-btn variant="text" size="small" class="text-none text-medium-emphasis"
    :append-icon="showOverview ? mdiChevronUp : mdiChevronDown"
    @click="showOverview = !showOverview">
    {{ showOverview ? 'Hide overview' : 'Show overview' }}
  </v-btn>
  <p v-show="showOverview" class="mt-1 text-body-2">{{ text }}</p>
</div>
<div v-else v-once>
  <h6 class="text-subtitle-1 font-weight-bold">Overview</h6>
  <p class="overview-clamp">{{ text }}</p>
</div>
```

### Desktop Overview Clamp

```css
.overview-clamp {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
```

### Responsive Classes

- Padding: `pa-4 pa-sm-6`
- Typography: `text-h5 text-sm-h4`, `text-body-2 text-sm-body-1`
- Columns: `cols="12" sm="6" lg="3"`

## Image Handling

Use `LazyOptimizedPicture` for all images:

- **Lazy loading** via Intersection Observer (50px root margin)
- **Preload mode** for above-the-fold images (uses `requestIdleCallback`)
- **Placeholder** with SVG initials, gradient background (`#1a1a1a` → `#2d2d2d`), pulse animation, and loading spinner
- **Fade-in transition** (`opacity 0` → `1`, `0.4s ease-in-out`)
- **Responsive aspect-ratio** CSS injected via `useHead()` to prevent layout shift
- **Dual formats:** `['webp', 'jpeg']`
- **SSR-safe IDs:** Uses `useId()` (Vue 3.5+) for deterministic hydration

## Data Display

### Desktop: Data Table

```html
<v-data-table :items="items" :headers="headers" :items-per-page="-1"
  hide-default-footer :group-by="groupBy" class="transparent-table">
```

- Show all items (`items-per-page="-1"`)
- No footer
- Episode grouping via `group-by`
- Custom slots for icons, chips, and download buttons

### Mobile: Expansion Panels

```html
<v-expansion-panels bg-color="transparent">
  <v-expansion-panel v-for="episode in episodes" bg-color="rgba(255,255,255,0.05)">
    <v-expansion-panel-title>
      <h3>Ep {{ episode.number }}. {{ episode.title }}</h3>
    </v-expansion-panel-title>
    <v-expansion-panel-text>
      <v-sheet rounded="lg" color="rgba(255,255,255,0.08)" class="pa-3">
        <!-- compact flex layout for subtitle details -->
      </v-sheet>
    </v-expansion-panel-text>
  </v-expansion-panel>
</v-expansion-panels>
```

### Chips

- **Source chips:** `<v-chip size="small" label>` with `teal` (SuperSubtitles) or `blue-darken-2` (Addic7ed)
- **Quality chips:** Use `ShowsQualityChips` component

## Loading States

| Context | Component | Pattern |
|---------|-----------|---------|
| Panel loading | `<v-skeleton-loader type="card">` | Wraps entire sheet |
| Page initial load | `<Suspense>` + `<v-progress-linear indeterminate>` | Fallback slot |
| Progress tracking | `<v-progress-linear v-model height="18">` | Text inside bar |
| Button loading | `:loading="true"` on `v-btn` | Mobile download buttons |
| Image loading | LazyOptimizedPicture placeholder | SVG + spinner |

## Form Controls

- `<v-autocomplete>` for searchable selects (language): `clearable`, `hide-details`
- `<v-select>` for simple selects (season): `hide-details`
- Place in `<v-row dense>` with responsive column sizing

## Navigation

- **AppBar:** `density="compact"`, `elevation="2"`, `color="rgba(0,0,0,0.7)"`
- Route-based nav buttons from `router.getRoutes()` filtered by `meta.order`
- `variant="text" size="small"` for nav buttons

## Performance Optimizations

- `v-once` on static content blocks that don't change after initial render
- `useId()` for SSR-safe unique identifiers (replaces `Math.random()`)
- `useHead()` for injecting responsive CSS (aspect-ratio) without client JS
- Lazy image loading with Intersection Observer
- `requestIdleCallback` for non-critical image preloading

## Verifying Changes with Playwright

**All frontend visual changes must be verified using Playwright MCP** before committing. This ensures the rendered output matches expectations on both desktop and mobile viewports.

### Desktop Verification

1. Navigate to the page with `mcp_playwright_browser_navigate`
2. Take a full-page screenshot with `mcp_playwright_browser_take_screenshot({ fullPage: true })`
3. Interact with elements (expand groups, click buttons) and screenshot again

### Mobile Verification

Because `@nuxtjs/device` detects mobile from the **server-side request UA header**, you must set a mobile User-Agent via the browser context:

```js
// Set mobile UA for SSR detection + client override
const context = page.context();
await context.setExtraHTTPHeaders({
  'User-Agent': 'Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) ...'
});
await context.addInitScript(() => {
  Object.defineProperty(navigator, 'userAgent', {
    get: () => 'Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) ...'
  });
});
// Also resize viewport
await page.setViewportSize({ width: 390, height: 844 });
```

> **Warning:** `context.addInitScript` is permanent per browser context. To switch back to desktop, you must close the page and create a new one.

### Verification Checklist

- [ ] Desktop screenshot at default viewport
- [ ] Desktop interaction (expand groups, click controls)
- [ ] Mobile screenshot with device emulation
- [ ] Mobile interaction (expand panels, toggle sections)
- [ ] No hydration mismatch warnings in console
