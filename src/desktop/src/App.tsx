import { useEffect, useState } from 'react';
import { api, type Health } from './api';

const NAV = ['Dashboard', 'Biblioteca', 'Perfiles', 'Monitores', 'Carpetas', 'Etiquetas', 'Configuración'];

export function App() {
  const [section, setSection] = useState(NAV[0]);
  const [health, setHealth] = useState<Health | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let tries = 0;
    const tick = () =>
      api<Health>('/api/health')
        .then(setHealth)
        .catch((e) => {
          // El backend tarda un momento en arrancar.
          if (++tries < 20) setTimeout(tick, 500);
          else setError(String(e.message ?? e));
        });
    tick();
  }, []);

  return (
    <div className="shell">
      <nav className="nav" aria-label="Navegación principal">
        <h1 className="brand">VibeScreen</h1>
        <ul>
          {NAV.map((item) => (
            <li key={item}>
              <button className={item === section ? 'active' : ''} onClick={() => setSection(item)}>
                {item}
              </button>
            </li>
          ))}
        </ul>
      </nav>
      <main className="content">
        <h2>{section}</h2>
        {section === 'Dashboard' && (
          <p className={'status ' + (health ? 'ok' : error ? 'err' : '')}>
            {health
              ? `Backend conectado · v${health.version} · ${health.runtime}`
              : error
                ? `No se pudo conectar con el backend: ${error}`
                : 'Conectando con el backend…'}
          </p>
        )}
      </main>
    </div>
  );
}
