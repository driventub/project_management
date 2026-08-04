FROM node:lts-slim
WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install
EXPOSE 4200
CMD ["npx", "ng", "serve", "--host", "0.0.0.0"]