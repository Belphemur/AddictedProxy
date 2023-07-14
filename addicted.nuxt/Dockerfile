# syntax=docker/dockerfile:1

FROM node:alpine as pnpm-installed
ARG PNPM_VERSION=latest
RUN npm i -g pnpm@${PNPM_VERSION}

FROM pnpm-installed AS prod-deps-installed
COPY package*.json pnpm*.yaml ./
RUN pnpm fetch


FROM prod-deps-installed AS dev
WORKDIR /app
COPY . ./
ARG SENTRY_AUTH_TOKEN
ARG SENTRY_ORG
ARG SENTRY_PROJECT
ARG RELEASE_VERSION
ENV SENTRY_AUTH_TOKEN=${SENTRY_AUTH_TOKEN}
ENV SENTRY_ORG=${SENTRY_ORG}
ENV SENTRY_PROJECT=${SENTRY_PROJECT}
ENV RELEASE_VERSION=${RELEASE_VERSION}
ENV SENTRY_ENVIRONMENT=production

RUN pnpm recursive install --offline --frozen-lockfile
RUN pnpm run build

# production stage
FROM node:alpine as production-stage
COPY --from=dev /app/ /app/
WORKDIR /app

ENV NUXT_HOST=0.0.0.0
ENV NUXT_PORT=3000
ENV IPX_CACHE=true

EXPOSE 3000
CMD ["node", "/app/.output/server/index.mjs"]