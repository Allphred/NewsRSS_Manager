/** Funciones de formato. No dependen de React. */

/** Convierte una fecha ISO en texto legible. Si la fecha no existe (año 0001) muestra "Sin fecha". */
export function formatDate(iso) {
  if (!iso || new Date(iso).getFullYear() < 2000) return 'Sin fecha';
  return new Date(iso).toLocaleDateString('es-ES', { day: 'numeric', month: 'short', year: 'numeric' });
}

/** Devuelve el dominio de un enlace, ej. "bbc.com". */
export function getDomain(link) {
  try {
    return new URL(link).hostname.replace('www.', '');
  } catch {
    return '';
  }
}

/**
 * La descripción de un RSS suele traer HTML. En lugar de insertarlo tal cual
 * (riesgo de seguridad) se extrae solo el texto y la primera imagen.
 */
export function parseDescription(html) {
  const doc = new DOMParser().parseFromString(html || '', 'text/html');
  const src = doc.querySelector('img')?.getAttribute('src') || '';
  const text = (doc.body.textContent || '').trim();

  // Algunos feeds, como Hacker News, usan "Comments" como descripción.
  // No es el resumen del artículo: lo tratamos como contenido vacío.
  const isCommentsOnly = /^comments$/i.test(text);

  return {
    text: isCommentsOnly ? '' : text,
    image: src.startsWith('http') ? src : null
  };
}
