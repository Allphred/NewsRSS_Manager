/** Muestra "Cargando…" o un mensaje de error. No muestra nada si no hay ninguno. */
function StatusMessage({ loading, error }) {
  if (loading) return <p className="status">Cargando…</p>;
  if (error) return <p className="status error">{error}</p>;
  return null;
}

export default StatusMessage;
