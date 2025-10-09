# Gestdown - Addic7ed Proxy Frontend

Gestdown is a modern web application that serves as a proxy for the Addic7ed subtitle website, making it easier to search for and download subtitles for your favorite TV shows.

## What is Gestdown?

Gestdown provides a user-friendly interface to browse and download subtitles from Addic7ed. Instead of navigating the original website, users can:

- **Search for TV shows** with an intuitive autocomplete search interface
- **Browse trending shows** to discover popular content
- **View episode listings** with available subtitles for each show
- **Download subtitles** in various formats and languages
- **Filter by subtitle type** and language preferences
- **Real-time updates** via SignalR for subtitle availability

## Technology Stack

- **Framework**: Nuxt 4.0.3 (Vue 3)
- **UI Library**: Vuetify 3.9.4
- **State Management**: Pinia with persisted state
- **Real-time Communication**: SignalR (@microsoft/signalr)
- **Build Tool**: Vite 7.1.1
- **TypeScript**: Full TypeScript support
- **Package Manager**: pnpm 10.18.1
- **Monitoring**: Sentry integration for error tracking
- **Analytics**: Cloudflare Analytics and Matomo

## Features

### Core Functionality
- **Smart Search**: Autocomplete search component for quick show discovery
- **Trending Media**: View currently trending shows with detailed information
- **Episode Management**: Browse all episodes with subtitle availability
- **Multi-language Support**: Download subtitles in your preferred language
- **Subtitle Type Selection**: Choose between different subtitle formats
- **Optimized Images**: Responsive image components with optimization

### Technical Features
- **Server-Side Rendering (SSR)**: Enhanced SEO and performance
- **Responsive Design**: Mobile and desktop optimized
- **API Integration**: Type-safe API client generated from Swagger
- **SEO Optimized**: Sitemap generation and meta tags
- **Docker Support**: Containerized deployment ready

## Setup

Make sure to install the dependencies using pnpm:

```bash
pnpm install
```

## Development Server

Start the development server on `http://localhost:3000`:

```bash
pnpm run dev
```

The server will be accessible on your local network (runs with `--host` flag).

## API Code Generation

Generate TypeScript API clients from Swagger:

```bash
pnpm run gen-api
```

This will fetch the API specification from `http://localhost/api/v1/swagger.json` and generate type-safe API clients in the `composables/api/` directory.

## Production

Build the application for production:

```bash
pnpm run build
```

Preview the production build locally:

```bash
pnpm run preview
```

## Environment Variables

Configure the following environment variables for proper operation:

- `APP_URL`: Public URL of the application
- `APP_API_PATH`: Client-side API endpoint
- `APP_SERVER_PATH`: Server-side API endpoint
- `APP_CLOUDFLARE_ANALYTIC_TOKEN`: Cloudflare Analytics token
- `APP_MATOMO`: Matomo analytics URL
- `SENTRY_DSN`: Sentry error tracking DSN
- `SENTRY_AUTH_TOKEN`: Sentry authentication token
- `SENTRY_ORG`: Sentry organization
- `SENTRY_PROJECT`: Sentry project name
- `SENTRY_ENVIRONMENT`: Deployment environment

## Docker Deployment

The project includes Docker support for easy deployment:

```bash
docker-compose up -d
```

## Project Structure

- `/app`: Main application code
  - `/components`: Reusable Vue components (media, shows, search, etc.)
  - `/composables`: Composable functions including API clients
  - `/pages`: Route pages (index, show details, API documentation)
  - `/layouts`: Application layouts
  - `/stores`: Pinia stores for state management
  - `/plugins`: Nuxt plugins (analytics, client-side features)
- `/public`: Static assets (images, icons, robots.txt)
- `/server`: Server-side code (sitemap proxy)

## Contributing

When adding new API endpoints, make sure to regenerate the API clients using `pnpm run gen-api` to maintain type safety.

## Learn More

- [Nuxt 4 Documentation](https://nuxt.com/docs)
- [Vuetify Documentation](https://vuetifyjs.com/)
- [Pinia Documentation](https://pinia.vuejs.org/)
