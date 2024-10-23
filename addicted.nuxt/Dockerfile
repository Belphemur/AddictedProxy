# syntax=docker/dockerfile:1

FROM node:lts-slim
ARG SENTRY_AUTH_TOKEN
ARG SENTRY_ORG
ARG SENTRY_PROJECT
ARG RELEASE_VERSION
ENV SENTRY_AUTH_TOKEN=${SENTRY_AUTH_TOKEN}
ENV SENTRY_ORG=${SENTRY_ORG}
ENV SENTRY_PROJECT=${SENTRY_PROJECT}
ENV RELEASE_VERSION=${RELEASE_VERSION}
ENV SENTRY_ENVIRONMENT=production

RUN  apt update && \
     apt install -y dumb-init && \
     rm -rf /var/cache/apt/archives /var/lib/apt/lists/* && \
     adduser --disabled-password --gecos '' nuxtuser
# set non root user
USER nuxtuser

COPY .output /app/
WORKDIR /app

ENV HOST=0.0.0.0 PORT=3000 NODE_ENV=production
ENV NUXT_HOST=0.0.0.0
ENV NUXT_PORT=3000
ENV IPX_CACHE=true

EXPOSE 3000
CMD ["dumb-init","node","/app/server/index.mjs"]