import Header from '../components/Header';
import NavBar from '../components/NavBar';
import Footer from '../components/Footer';

/**
 * Estructura común de todas las páginas
 */
function MainLayout({ children }) {
  return (
    <>
      <Header title="El Compendio" tagline="Tus fuentes, tu portada" />
      <NavBar />
      <main className="container">{children}</main>
      <Footer />
    </>
  );
}

export default MainLayout;
