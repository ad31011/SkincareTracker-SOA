const BASE = 'http://localhost:52182/api';

function getToken() { return localStorage.getItem('token'); }

async function req(method, path, body) {
  const res = await fetch(`${BASE}${path}`, {
    method,
    headers: {
      'Content-Type': 'application/json',
      ...(getToken() ? { Authorization: `Bearer ${getToken()}` } : {}),
    },
    body: body ? JSON.stringify(body) : undefined,
  });
  if (!res.ok) {
    const err = await res.json().catch(() => ({ message: 'Request failed' }));
    throw new Error(err.message || 'Request failed');
  }
  const text = await res.text();
  return text ? JSON.parse(text) : null;
}

export const api = {
  // Auth
  login: (data) => req('POST', '/auth/login', data),
  register: (data) => req('POST', '/auth/register', data),
  me: () => req('GET', '/auth/me'),

  // Products
  getProducts: () => req('GET', '/products'),
  createProduct: (data) => req('POST', '/products', data),
  updateProduct: (id, data) => req('PUT', `/products/${id}`, data),
  deleteProduct: (id) => req('DELETE', `/products/${id}`),

  // Ingredients
  getIngredients: () => req('GET', '/ingredients'),
  createIngredient: (data) => req('POST', '/ingredients', data),
  deleteIngredient: (id) => req('DELETE', `/ingredients/${id}`),

  // Routines
  getRoutines: () => req('GET', '/routines'),
  createRoutine: (data) => req('POST', '/routines', data),
  updateRoutine: (id, data) => req('PUT', `/routines/${id}`, data),
  deleteRoutine: (id) => req('DELETE', `/routines/${id}`),
  getRoutineConflicts: (id) => req('GET', `/routines/${id}/conflicts`),

  // Skin Logs
  getSkinLogs: () => req('GET', '/skinlogs'),
  createSkinLog: (data) => req('POST', '/skinlogs', data),
  updateSkinLog: (id, data) => req('PUT', `/skinlogs/${id}`, data),
  deleteSkinLog: (id) => req('DELETE', `/skinlogs/${id}`),
  getStreak: () => req('GET', '/skinlogs/streak'),
  getProgress: (from, to) => req('GET', `/skinlogs/progress?from=${from}&to=${to}`),

  // Users (admin)
  getUsers: () => req('GET', '/users'),
  deleteUser: (id) => req('DELETE', `/users/${id}`),
};
