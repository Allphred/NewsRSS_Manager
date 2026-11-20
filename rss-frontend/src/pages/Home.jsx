import { useEffect, useState } from 'react';
import { getArticles } from '../services/articleService';
import { getFeeds } from '../services/feedService';
import AsciiBanner from '../components/AsciiBanner';
import Section from '../components/Section';
import FilterBar from '../components/FilterBar';
import ArticleList from '../components/ArticleList';
import StatusMessage from '../components/StatusMessage';

/** Landing page: banner + noticias nuevas con filtros por categoría y fuente. */
function Home() {
  const [articles, setArticles] = useState([]);
  const [feeds, setFeeds] = useState([]);
  const [category, setCategory] = useState('');
  const [feedId, setFeedId] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    getFeeds().then(setFeeds).catch((e) => setError(e.message));
  }, []);

  // [category] = se ejecuta al abrir la página y cada vez que cambia la categoría.
  useEffect(() => {
    setLoading(true);
    setError('');
    getArticles('new', category)
      .then(setArticles)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }, [category]);

  const categories = [...new Set(feeds.map((f) => f.category))];
  // La API no filtra por fuente, por eso este filtro
  const visible = feedId ? articles.filter((a) => a.feedId === Number(feedId)) : articles;

  return (
    <>
      <AsciiBanner />
      <Section title="Últimas noticias">
        <FilterBar
          categories={categories}
          feeds={feeds}
          category={category}
          feedId={feedId}
          onCategoryChange={setCategory}
          onFeedChange={setFeedId}
        />
        <StatusMessage loading={loading} error={error} />
        {!loading && !error && (
          <ArticleList articles={visible} emptyText="No hay noticias. Agrega una fuente y actualízala en la sección Fuentes." />
        )}
      </Section>
    </>
  );
}

export default Home;
