import { useEffect, useState } from 'react';
import { getFeeds, createFeed, deleteFeed, refreshFeed } from '../services/feedService';
import Section from '../components/Section';
import FeedForm from '../components/FeedForm';
import FeedItem from '../components/FeedItem';
import StatusMessage from '../components/StatusMessage';

/** Administración de fuentes RSS: listar, agregar, actualizar y eliminar. */
function Feeds() {
  const [feeds, setFeeds] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [notice, setNotice] = useState('');

  function loadFeeds() {
    getFeeds()
      .then(setFeeds)
      .catch((e) => setError(e.message))
      .finally(() => setLoading(false));
  }

  useEffect(loadFeeds, []);

  // Si createFeed falla, el error sube a FeedForm, que lo muestra.
  async function handleCreate(feed) {
    const created = await createFeed(feed);
    try {
      const r = await refreshFeed(created.id); // descarga sus artículos de inmediato
      setNotice(`Fuente agregada: ${r.articulosNuevos} artículos nuevos.`);
    } catch (e) {
      setNotice(`Fuente agregada, pero no se pudo leer el RSS: ${e.message}`);
    }
    loadFeeds();
  }

  async function handleRefresh(feed) {
    try {
      const r = await refreshFeed(feed.id);
      setError('');
      setNotice(`${feed.name}: ${r.articulosNuevos} artículos nuevos.`);
      loadFeeds();
    } catch (e) {
      setError(e.message);
    }
  }

  async function handleDelete(feed) {
    if (!window.confirm(`¿Eliminar "${feed.name}" y todos sus artículos?`)) return;
    try {
      await deleteFeed(feed.id);
      setNotice(`Fuente "${feed.name}" eliminada.`);
      loadFeeds();
    } catch (e) {
      setError(e.message);
    }
  }

  return (
    <>
      <Section title="Agregar fuente">
        <FeedForm onSubmit={handleCreate} />
        {notice && <p className="status">{notice}</p>}
      </Section>
      <Section title="Mis fuentes">
        <StatusMessage loading={loading} error={error} />
        {!loading && feeds.length === 0 && <p className="status">No hay fuentes registradas.</p>}
        <ul className="feed-list">
          {feeds.map((f) => (
            <FeedItem key={f.id} feed={f} onRefresh={handleRefresh} onDelete={handleDelete} />
          ))}
        </ul>
      </Section>
    </>
  );
}

export default Feeds;
