import { useState } from 'react';
import ArticleCard from './ArticleCard';

/**
 * Lista de artículos con CONTADOR: el estado `limit` indica cuántos se muestran
 * y los botones − / + lo cambian de 5 en 5.
 * Props: articles (arreglo), onSavedChange (función opcional), emptyText (string).
 */
function ArticleList({ articles, onSavedChange, emptyText }) {
  const [limit, setLimit] = useState(10);

  if (articles.length === 0) return <p className="status">{emptyText}</p>;

  const shown = articles.slice(0, limit);

  return (
    <>
      <div className="counter">
        <span>Mostrando {shown.length} de {articles.length}</span>
        <button onClick={() => setLimit(limit - 5)} disabled={limit <= 5} aria-label="Mostrar menos">−</button>
        <strong>{limit}</strong>
        <button onClick={() => setLimit(limit + 5)} disabled={limit >= articles.length} aria-label="Mostrar más">+</button>
      </div>
      <ol className="article-list">
        {shown.map((a) => (
          <ArticleCard key={a.id} article={a} onSavedChange={onSavedChange} />
        ))}
      </ol>
    </>
  );
}

export default ArticleList;
