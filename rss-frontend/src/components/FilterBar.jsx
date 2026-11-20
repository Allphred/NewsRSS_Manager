/**
 * Filtros del inicio. No guarda estado propio: recibe los valores por props
 * y avisa al padre (Home) con las funciones onCategoryChange y onFeedChange.
 */
function FilterBar({ categories, feeds, category, feedId, onCategoryChange, onFeedChange }) {
  return (
    <div className="filters">
      <label>
        Categoría
        <select value={category} onChange={(e) => onCategoryChange(e.target.value)}>
          <option value="">Todas</option>
          {categories.map((c) => (
            <option key={c} value={c}>{c}</option>
          ))}
        </select>
      </label>
      <label>
        Fuente
        <select value={feedId} onChange={(e) => onFeedChange(e.target.value)}>
          <option value="">Todas</option>
          {feeds.map((f) => (
            <option key={f.id} value={f.id}>{f.name}</option>
          ))}
        </select>
      </label>
    </div>
  );
}

export default FilterBar;
