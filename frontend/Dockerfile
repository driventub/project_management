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
