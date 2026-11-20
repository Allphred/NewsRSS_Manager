/**
 * "Caja genérica" para secciones de la página. Se le pasa un título y el contenido que se escriba dentro de <Section>…</Section>.
 * Props: title. 
 */
function Section({ title, children }) {
  return (
    <section className="section">
      <h2 className="section-title">{title}</h2>
      {children}
    </section>
  );
}

export default Section;
