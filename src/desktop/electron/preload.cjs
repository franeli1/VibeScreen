const { contextBridge, ipcRenderer } = require('electron');

contextBridge.exposeInMainWorld('vibe', {
  backendInfo: () => ipcRenderer.invoke('backend:info'),
});
