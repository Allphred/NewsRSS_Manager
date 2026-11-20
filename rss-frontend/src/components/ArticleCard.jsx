import { useState } from 'react';
import { updateArticle } from '../services/articleService';
import { formatDate, getDomain, parseDescription } from '../utils/format';

/**
 * Un artículo. Al inicio solo muestra el título y datos básicos.
 * Estados (useState):
 *  - open  -> VISIBILIDAD del resumen: { open && ... } lo muestra u oculta.
 *  - saved -> botón que CAMBIA DE ESTADO (☆ Ver más tarde / ★ Guardado); se sincroniza con la API (PATCH).
 * Props: article (objeto de la API) y onSavedChange (función opcional del padre).
 */
function ArticleCard({ article, onSavedChange }) {
  const [open, setOpen] = useState(false);
  const [saved, setSaved] = useState(article.isSaved);
  const [error, setError] = useState('');
  const { text, image } = parseDescription(article.description);

  async function handleSave() {
    try {
      await updateArticle(article.id, { isSaved: !saved });
      setSaved(!saved);
      setError('');
      if (onSavedChange) onSavedChange(article.id, !saved);
    } catch (e) {
      setError(e.message);
    }
  }

  return (
    <li className="article">
      <div>
        <a className="article-title" href={article.link} target="_blank" rel="noreferrer">
          {article.title}
        </a>
        <span className="domain"> ({getDomain(article.link)})</span>
      </div>
      <p className="meta">
        {article.feed?.name} <span className="tag">{article.feed?.category}</span> {formatDate(article.publishedDate)}
      </p>
      <div className="actions">
        <button className="link-btn" onClick={() => setOpen(!open)}>
          {open ? '▲ Ocultar resumen' : '▼ Ver resumen'}
        </button>
        <button className={saved ? 'link-btn active' : 'link-btn'} onClick={handleSave}>
          {saved ? '★ Guardado' : '☆ Ver más tarde'}
        </button>
      </div>
      {error && <p className="status error">{error}</p>}
      {open && (
        <div className="article-body">
          {/* Forma 3: imagen que vive en otro sitio web (URL del RSS) */}
          {image && <img src={image} alt="" />}
          <p>{text || 'Este artículo no incluye descripción.'}</p>
        </div>
      )}
    </li>
  );
}

export default ArticleCard;
