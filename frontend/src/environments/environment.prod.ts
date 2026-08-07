export const environment = {
  production: true,
  // relative path: nginx (spa service, Phase 1 pending) reverse-proxies /api to the api container.
  apiUrl: '/api',
  // same nginx proxy needs to forward /hubs (and its WebSocket upgrade) to the api container.
  hubUrl: '/hubs/tablero'
};
