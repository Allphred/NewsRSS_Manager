import { request } from './http';

/** GET /api/feeds — lista todas las fuentes RSS. */
export const getFeeds = () => request('/feeds');

/** POST /api/feeds — crea una fuente { name, url, category }. */
export const createFeed = (feed) => request('/feeds', { method: 'POST', body: JSON.stringify(feed) });

/** DELETE /api/feeds/{id} — elimina la fuente y, en cascada, sus artículos. */
export const deleteFeed = (id) => request(`/feeds/${id}`, { method: 'DELETE' });

/** POST /api/feeds/{id}/refresh — descarga el RSS. Devuelve { feedId, articulosNuevos }. */
export const refreshFeed = (id) => request(`/feeds/${id}/refresh`, { method: 'POST' });
