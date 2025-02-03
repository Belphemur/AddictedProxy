# syntax=docker/dockerfile:1
FROM node:lts-alpine AS base
ARG SENTRY_AUTH_TOKEN
ARG SENTRY_ORG
ARG SENTRY_PROJECT
ARG RELEASE_VERSION
ENV SENTRY_ENVIRONMENT=production


FROM base AS builder
RUN apk add --no-cache --update python3 make g++
WORKDIR /app
RUN npm i -g corepack@latest && corepack enable pnpm
COPY . /app
RUN pnpm install --frozen-lockfile
RUN NUXT_PRESET=node-server pnpm build
RUN cd .output/server && rm -rf node_modules && pnpm install && pnpm add ws


FROM base
ARG SENTRY_AUTH_TOKEN
ARG SENTRY_ORG
ARG SENTRY_PROJECT
ARG RELEASE_VERSION
ENV SENTRY_AUTH_TOKEN=${SENTRY_AUTH_TOKEN}
ENV SENTRY_ORG=${SENTRY_ORG}
ENV SENTRY_PROJECT=${SENTRY_PROJECT}
ENV RELEASE_VERSION=${RELEASE_VERSION}

RUN apk add --no-cache dumb-init && \
    adduser -D nuxtuser

# set non root user
USER nuxtuser

COPY --from=builder /app/.output /app/.output
WORKDIR /app

ENV NUXT_HOST=0.0.0.0
ENV NUXT_PORT=3000

EXPOSE 3000
CMD ["dumb-init", "node",".output/server/index.mjs"]