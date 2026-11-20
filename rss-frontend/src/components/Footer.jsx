/** Pie de página con el año actual y créditos. */
function Footer() {
  return (
    <footer className="footer">
      <p>El Compendio © {new Date().getFullYear()}</p>
    </footer>
  );
}

export default Footer;
