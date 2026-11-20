import logo from '../assets/logo.svg'; // Utilizand forma 1: imagen desde src/assets

/**
 * Encabezado como un periodico: fecha, logo y título.
 * Props recibidas desde MainLayout: title y tagline.
 */
function Header({ title, tagline }) {
  const today = new Date().toLocaleDateString('es-ES', {
    weekday: 'long', day: 'numeric', month: 'long', year: 'numeric',
  });

  return (
    <header className="masthead">
      <div className="masthead-top">
        <span>{today}</span>
        <span>{tagline}</span>
      </div>
      <div className="masthead-title">
        <img src={logo} alt="Logo de El Compendio" width="52" height="52" />
        <h1>{title}</h1>
      </div>
    </header>
  );
}

export default Header;
