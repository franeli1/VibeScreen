declare global {
  interface Window {
    vibe: { backendInfo(): Promise<{ baseUrl: string; token: string }> };
  }
}

export async function api<T>(path: string, init?: RequestInit): Promise<T> {
  const { baseUrl, token } = await window.vibe.backendInfo();
  const res = await fetch(baseUrl + path, {
    ...init,
    headers: { ...init?.headers, 'X-Vibe-Token': token },
  });
  if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
  return res.json() as Promise<T>;
}

export type Health = { status: string; version: string; runtime: string };
