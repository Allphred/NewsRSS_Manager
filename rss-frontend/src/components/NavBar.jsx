import { NavLink } from 'react-router-dom';

/** Barra de navegación. NavLink cambia de página sin recargar y marca el enlace activo. */
function NavBar() {
  return (
    <nav className="navbar">
      <NavLink to="/" end>Inicio</NavLink>
      <NavLink to="/guardados">Ver más tarde</NavLink>
      <NavLink to="/fuentes">Fuentes</NavLink>
    </nav>
  );
}

export default NavBar;
