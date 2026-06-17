import { useState, useEffect } from 'react';
import { api } from '../api';
import { useAuth } from '../AuthContext';

export default function Ingredients() {
  const { user } = useAuth();
  const isAdmin = user?.role === 'Admin';
  const [ingredients, setIngredients] = useState([]);
  const [modal, setModal] = useState(false);
  const [form, setForm] = useState({ name:'', conflictsWith:'', notes:'' });
  const [msg, setMsg] = useState('');
  const [loading, setLoading] = useState(true);

  const load = () => api.getIngredients().then(setIngredients).finally(() => setLoading(false));
  useEffect(() => { load(); }, []);

  const save = async () => {
    try {
      await api.createIngredient(form);
      setMsg('✅ Ingredient added!'); setModal(false); setForm({name:'',conflictsWith:'',notes:''}); load();
    } catch(e) { setMsg('❌ '+e.message); }
  };

  const del = async (id) => {
    if (!confirm('Delete ingredient?')) return;
    await api.deleteIngredient(id); load();
  };

  if (loading) return <div className="loading">Loading...</div>;

  return (
    <div>
      <div className="page-header"><h2>🔬 Ingredients</h2><p>Ingredient library and conflict checker</p></div>
      {msg && <div className="alert alert-success" onClick={() => setMsg('')}>{msg}</div>}
      <div className="card">
        <div className="card-header">
          <span className="card-title">All Ingredients ({ingredients.length})</span>
          {isAdmin && <button className="btn btn-primary" onClick={() => setModal(true)}>+ Add</button>}
        </div>
        {ingredients.length === 0 ? (
          <div className="empty-state"><div className="empty-icon">🔬</div><h3>No ingredients yet</h3></div>
        ) : (
          <div className="table-wrap">
            <table>
              <thead><tr><th>Name</th><th>Conflicts With</th><th>Notes</th>{isAdmin && <th></th>}</tr></thead>
              <tbody>
                {ingredients.map(i => (
                  <tr key={i.id}>
                    <td style={{fontWeight:600}}>{i.name}</td>
                    <td>
                      {i.conflictsWith
                        ? i.conflictsWith.split(',').map(c => (
                          <span key={c} className="badge badge-admin" style={{marginRight:4}}>{c.trim()}</span>
                        ))
                        : <span style={{color:'var(--text-light)',fontSize:12}}>None</span>}
                    </td>
                    <td style={{color:'var(--text-light)',fontSize:13}}>{i.notes}</td>
                    {isAdmin && (
                      <td><button className="btn btn-danger btn-sm" onClick={() => del(i.id)}>Delete</button></td>
                    )}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {modal && (
        <div className="modal-overlay open" onClick={e => e.target.classList.contains('modal-overlay') && setModal(false)}>
          <div className="modal">
            <div className="modal-header">
              <span className="modal-title">Add Ingredient</span>
              <button className="modal-close" onClick={() => setModal(false)}>×</button>
            </div>
            <div className="form-group">
              <label className="form-label">Name</label>
              <input className="form-control" value={form.name} onChange={e => setForm({...form,name:e.target.value})} />
            </div>
            <div className="form-group">
              <label className="form-label">Conflicts With (comma-separated)</label>
              <input className="form-control" placeholder="e.g. Retinol, Vitamin C" value={form.conflictsWith}
                onChange={e => setForm({...form,conflictsWith:e.target.value})} />
            </div>
            <div className="form-group">
              <label className="form-label">Notes</label>
              <textarea className="form-control" rows={2} value={form.notes}
                onChange={e => setForm({...form,notes:e.target.value})} />
            </div>
            <div className="modal-footer">
              <button className="btn btn-ghost" onClick={() => setModal(false)}>Cancel</button>
              <button className="btn btn-primary" onClick={save}>Add</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
