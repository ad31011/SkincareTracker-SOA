import { useState, useEffect } from 'react';
import { api } from '../api';

export default function Users() {
  const [users, setUsers] = useState([]);
  const [msg, setMsg] = useState('');
  const [loading, setLoading] = useState(true);

  const load = () => api.getUsers().then(setUsers).finally(() => setLoading(false));
  useEffect(() => { load(); }, []);

  const del = async (id) => {
    if (!confirm('Delete this user?')) return;
    await api.deleteUser(id); setMsg('User deleted.'); load();
  };

  if (loading) return <div className="loading">Loading...</div>;

  return (
    <div>
      <div className="page-header"><h2>👥 Users</h2><p>User management (Admin only)</p></div>
      {msg && <div className="alert alert-success" onClick={() => setMsg('')}>{msg}</div>}
      <div className="card">
        <div className="card-header">
          <span className="card-title">All Users ({users.length})</span>
        </div>
        <div className="table-wrap">
          <table>
            <thead><tr><th>Name</th><th>Email</th><th>Role</th><th>Skin Type</th><th>Concerns</th><th>Joined</th><th></th></tr></thead>
            <tbody>
              {users.map(u => (
                <tr key={u.id}>
                  <td style={{fontWeight:600}}>{u.name}</td>
                  <td>{u.email}</td>
                  <td><span className={`badge ${u.role==='Admin'?'badge-admin':'badge-rose'}`}>{u.role}</span></td>
                  <td>{u.skinType}</td>
                  <td style={{fontSize:12,color:'var(--text-light)'}}>{u.skinConcerns || '—'}</td>
                  <td style={{fontSize:12}}>{new Date(u.createdAt).toLocaleDateString()}</td>
                  <td>
                    {u.role !== 'Admin' && (
                      <button className="btn btn-danger btn-sm" onClick={() => del(u.id)}>Delete</button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
