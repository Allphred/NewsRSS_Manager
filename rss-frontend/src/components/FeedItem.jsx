import { formatDate } from '../utils/format';

/**
 * Una fuente RSS en la lista de administración.
 * feed (objeto de la API)
 */
function FeedItem({ feed, onRefresh, onDelete }) {
  return (
    <li className="feed-row">
      <div>
        <strong>{feed.name}</strong> <span className="tag">{feed.category}</span>
        <div className="meta">{feed.url}</div>
        <div className="meta">
          Última actualización: {feed.lastFetchedAt ? formatDate(feed.lastFetchedAt) : 'nunca'}
        </div>
      </div>
      <div className="actions">
        <button className="link-btn" onClick={() => onRefresh(feed)}>Actualizar</button>
        <button className="link-btn danger" onClick={() => onDelete(feed)}>Eliminar</button>
      </div>
    </li>
  );
}

export default FeedItem;
