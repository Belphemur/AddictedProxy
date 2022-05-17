# syntax=docker/dockerfile:1

FROM node:alpine as build-stage
WORKDIR /app
COPY package*.json pnpm*.yaml ./
RUN npm i -g pnpm
RUN pnpm install
COPY . .
RUN pnpm run build

# production stage
FROM nginx:stable-alpine as production-stage
COPY --from=build-stage /app/dist /usr/share/nginx/html
COPY default.conf /etc/nginx/conf.d/
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]