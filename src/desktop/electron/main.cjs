// Proceso principal de Electron: ventana, bandeja y supervisión del backend. Sin lógica de negocio.
const { app, BrowserWindow, Tray, Menu, ipcMain, nativeImage } = require('electron');
const { spawn } = require('node:child_process');
const crypto = require('node:crypto');
const net = require('node:net');
const path = require('node:path');

const isDev = process.env.VITE_DEV === '1';
const token = crypto.randomBytes(32).toString('hex');
let backend = null;
let backendPort = 0;
let win = null;
let tray = null;
let quitting = false;

function freePort() {
  return new Promise((resolve, reject) => {
    const srv = net.createServer();
    srv.listen(0, '127.0.0.1', () => {
      const { port } = srv.address();
      srv.close(() => resolve(port));
    });
    srv.on('error', reject);
  });
}

async function startBackend() {
  backendPort = await freePort();
  const env = { ...process.env, VIBESCREEN_TOKEN: token };
  const urls = `http://127.0.0.1:${backendPort}`;
  const root = path.resolve(__dirname, '../../..');
  backend = isDev
    ? spawn('dotnet', ['run', '--project', path.join(root, 'src/backend/VibeScreen.Api'), '--urls', urls], { env })
    : spawn(path.join(process.resourcesPath, 'backend', 'VibeScreen.Api.exe'), ['--urls', urls], { env });

  backend.stdout.on('data', (d) => console.log('[backend]', String(d).trim()));
  backend.stderr.on('data', (d) => console.error('[backend]', String(d).trim()));
  backend.on('exit', (code) => {
    console.warn('[backend] terminó con código', code);
    if (!quitting) backend = null; // TODO: reinicio con backoff y aviso grave al usuario
  });
}

function createWindow() {
  win = new BrowserWindow({
    width: 1280,
    height: 800,
    backgroundColor: '#0A090A',
    show: false,
    webPreferences: {
      preload: path.join(__dirname, 'preload.cjs'),
      contextIsolation: true,
      nodeIntegration: false,
      sandbox: true,
    },
  });
  win.once('ready-to-show', () => win.show());
  // Cerrar la ventana no cierra la app: sigue en la bandeja (requisito 4.6).
  win.on('close', (e) => {
    if (!quitting) { e.preventDefault(); win.hide(); }
  });
  if (isDev) win.loadURL('http://localhost:5173');
  else win.loadFile(path.join(__dirname, '../dist/index.html'));
}

function createTray() {
  tray = new Tray(nativeImage.createEmpty()); // TODO: icono real
  tray.setToolTip('VibeScreen');
  tray.setContextMenu(Menu.buildFromTemplate([
    { label: 'Abrir VibeScreen', click: () => { win?.show(); win?.focus(); } },
    { type: 'separator' },
    { label: 'Pausar / reanudar', enabled: false },
    { label: 'Fondo siguiente', enabled: false },
    { label: 'Fondo anterior', enabled: false },
    { type: 'separator' },
    { label: 'Salir', click: () => { quitting = true; app.quit(); } },
  ]));
  tray.on('click', () => win?.show());
}

ipcMain.handle('backend:info', () => ({ baseUrl: `http://127.0.0.1:${backendPort}`, token }));

if (!app.requestSingleInstanceLock()) app.quit();
app.on('second-instance', () => { win?.show(); win?.focus(); });

app.whenReady().then(async () => {
  await startBackend();
  createWindow();
  createTray();
});

app.on('before-quit', () => { quitting = true; backend?.kill(); });
app.on('window-all-closed', () => { /* la app vive en la bandeja */ });
