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
FROM nginx:stable-alpine as production-stage
COPY --from=dev /app/dist /usr/share/nginx/html
COPY default.conf /etc/nginx/conf.d/
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]