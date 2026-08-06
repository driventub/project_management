FROM node:lts-slim AS dev
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
EXPOSE 4200
CMD ["npx", "ng", "serve", "--host", "0.0.0.0"]

FROM dev AS test
RUN apt-get update \
    && apt-get install -y --no-install-recommends firefox-esr \
    && rm -rf /var/lib/apt/lists/*
CMD ["npx", "ng", "test", "--watch=false", "--browsers=FirefoxHeadless"]

FROM node:lts-slim AS build
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
COPY . .
RUN npx ng build --configuration production

FROM nginx:1.27-alpine AS production
COPY --from=build /app/dist/sakai-ng /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
