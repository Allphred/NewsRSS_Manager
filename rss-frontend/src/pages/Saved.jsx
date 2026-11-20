import { useEffect, useState } from 'react';
import { getArticles } from '../services/articleService';
import Section from '../components/Section';
import ArticleList from '../components/ArticleList';
import StatusMessage from '../components/StatusMessage';

/** Lista "Ver más tarde": artículos con isSaved = true (status=saved en la API). */
function Saved() {
  const [articles, setArticles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    getArticles('saved')
      .then(setArticles)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }, []);

  // Cuando un artículo deja de estar guardado, se quita de esta lista.
  function handleSavedChange(id, isSaved) {
    if (!isSaved) setArticles(articles.filter((a) => a.id !== id));
  }

  return (
    <Section title="Ver más tarde">
      <StatusMessage loading={loading} error={error} />
      {!loading && !error && (
        <ArticleList articles={articles} onSavedChange={handleSavedChange} emptyText="Aún no has guardado artículos." />
      )}
    </Section>
  );
}

export default Saved;
