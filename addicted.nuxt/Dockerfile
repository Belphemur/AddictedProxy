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
RUN pnpm recursive install --offline --frozen-lockfile
RUN pnpm run build

# production stage
FROM node:alpine as production-stage
COPY --from=dev /app/ /app/
WORKDIR /app

ENV NUXT_HOST=0.0.0.0
ENV NUXT_PORT=3000

EXPOSE 3000
CMD ["node", "/app/.output/server/index.mjs"]