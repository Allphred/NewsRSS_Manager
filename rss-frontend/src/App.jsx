import { Routes, Route, Navigate } from 'react-router-dom';
import MainLayout from './layouts/MainLayout';
import Home from './pages/Home';
import Saved from './pages/Saved';
import Feeds from './pages/Feeds';

/** define qué página se muestra según la URL. */
function App() {
  return (
    <MainLayout>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/guardados" element={<Saved />} />
        <Route path="/fuentes" element={<Feeds />} />
        <Route path="*" element={<Navigate to="/" replace />} />
      </Routes>
    </MainLayout>
  );
}

export default App;
