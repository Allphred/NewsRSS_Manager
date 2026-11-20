import { useState } from 'react';

/**
 * Formulario para agregar una fuente (inputs controlados).
 * Prop: onSubmit (función async del padre que llama a la API).
 */
function FeedForm({ onSubmit }) {
  const [form, setForm] = useState({ name: '', url: '', category: '' });
  const [error, setError] = useState('');
  const [sending, setSending] = useState(false);

  function handleChange(e) {
    setForm({ ...form, [e.target.name]: e.target.value });
  }

  async function handleSubmit(e) {
    e.preventDefault(); // evita que el navegador recargue la página
    setSending(true);
    setError('');
    try {
      await onSubmit(form);
      setForm({ name: '', url: '', category: '' });
    } catch (err) {
      setError(err.message);
    } finally {
      setSending(false);
    }
  }

  return (
    <form className="feed-form" onSubmit={handleSubmit}>
      <input name="name" placeholder="Nombre (ej. BBC Mundo)" value={form.name} onChange={handleChange} required />
      <input name="url" type="url" placeholder="URL del RSS" value={form.url} onChange={handleChange} required />
      <input name="category" placeholder="Categoría" value={form.category} onChange={handleChange} required />
      <button type="submit" disabled={sending}>{sending ? 'Agregando…' : 'Agregar fuente'}</button>
      {error && <p className="status error">{error}</p>}
    </form>
  );
}

export default FeedForm;
