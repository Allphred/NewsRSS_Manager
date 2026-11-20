import { request } from './http';

/** GET /api/articles — status: new | saved | read | discarded | all; category es opcional. */
export const getArticles = (status = 'new', category = '') =>
  request(`/articles?status=${status}` + (category ? `&category=${encodeURIComponent(category)}` : ''));

/** PATCH /api/articles/{id} — cambia solo los campos enviados */
export const updateArticle = (id, changes) =>
  request(`/articles/${id}`, { method: 'PATCH', body: JSON.stringify(changes) });
