// URL base de la API, leída del archivo .env .
const API_URL = import.meta.env.VITE_API_URL;

/**
 * Hace una petición HTTP a la API y devuelve el JSON (o null si no hay cuerpo, ej. 204).
 * Si el servidor responde con error, lanza un Error con un mensaje legible
 * (la API devuelve { error: "..." }.
 */
export async function request(path, options = {}) {
  const res = await fetch(API_URL + path, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });
  const text = await res.text();

  if (!res.ok) {
    let message = `Error ${res.status}`;
    if (text) {
      try {
        const json = JSON.parse(text);
        message = json.error || json.title || text;
      } catch {
        message = text;
      }
    }
    throw new Error(message);
  }
  return text ? JSON.parse(text) : null;
}
