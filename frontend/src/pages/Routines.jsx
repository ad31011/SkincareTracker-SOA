import { useState, useEffect } from 'react';
import { api } from '../api';

const DAYS = ['Mon','Tue','Wed','Thu','Fri','Sat','Sun'];
const blank = { name:'', timeOfDay:'AM', daysOfWeek: DAYS.join(','), products:[] };

export default function Routines() {
  const [routines, setRoutines] = useState([]);
  const [products, setProducts] = useState([]);
  const [modal, setModal] = useState(null);
  const [form, setForm] = useState(blank);
  const [conflicts, setConflicts] = useState([]);
  const [msg, setMsg] = useState('');
  const [loading, setLoading] = useState(true);

  const load = () => Promise.all([api.getRoutines(), api.getProducts()])
    .then(([r,p]) => { setRoutines(r); setProducts(p); }).finally(() => setLoading(false));

  useEffect(() => { load(); }, []);

  const openCreate = () => { setForm({...blank}); setConflicts([]); setModal('create'); };
  const openEdit = (r) => {
    setForm({ name:r.name, timeOfDay:r.timeOfDay, daysOfWeek:r.daysOfWeek,
      isActive: r.isActive,
      products: r.products.map(p => ({ productId: p.productId, stepOrder: p.stepOrder })) });
    setModal(r);
    api.getRoutineConflicts(r.id).then(setConflicts).catch(() => setConflicts([]));
  };

  const toggleDay = (d) => {
    const days = form.daysOfWeek ? form.daysOfWeek.split(',').filter(Boolean) : [];
    const next = days.includes(d) ? days.filter(x => x !== d) : [...days, d];
    setForm(f => ({...f, daysOfWeek: next.join(',')}));
  };

  const toggleProduct = (id) => {
    setForm(f => {
      const exists = f.products.find(p => p.productId === id);
      const next = exists
        ? f.products.filter(p => p.productId !== id)
        : [...f.products, { productId: id, stepOrder: f.products.length + 1 }];
      return {...f, products: next};
    });
  };

  const save = async () => {
    try {
      const payload = { name: form.name, timeOfDay: form.timeOfDay, daysOfWeek: form.daysOfWeek, products: form.products };
      if (modal === 'create') await api.createRoutine(payload);
      else await api.updateRoutine(modal.id, { ...payload, isActive: form.isActive ?? true });
      setMsg('✅ Routine saved!'); setModal(null); load();
    } catch(e) { setMsg('❌ '+e.message); }
  };

  const del = async (id) => {
    if (!confirm('Delete routine?')) return;
    await api.deleteRoutine(id); setMsg('Deleted.'); load();
  };

  if (loading) return <div className="loading">Loading...</div>;

  const selectedDays = form.daysOfWeek ? form.daysOfWeek.split(',').filter(Boolean) : [];

  return (
    <div>
      <div className="page-header"><h2>🌿 Routines</h2><p>Manage your skincare routines</p></div>
      {msg && <div className="alert alert-success" onClick={() => setMsg('')}>{msg}</div>}
      <div className="card">
        <div className="card-header">
          <span className="card-title">My Routines</span>
          <button className="btn btn-primary" onClick={openCreate}>+ New Routine</button>
        </div>
        {routines.length === 0 ? (
          <div className="empty-state"><div className="empty-icon">🌿</div><h3>No routines yet</h3><p>Create your first skincare routine</p></div>
        ) : (
          <div className="table-wrap">
            <table>
              <thead><tr><th>Name</th><th>Time</th><th>Days</th><th>Products</th><th>Status</th><th></th></tr></thead>
              <tbody>
                {routines.map(r => (
                  <tr key={r.id}>
                    <td style={{fontWeight:600}}>{r.name}</td>
                    <td><span className="badge badge-rose">{r.timeOfDay}</span></td>
                    <td style={{fontSize:12}}>{r.daysOfWeek}</td>
                    <td>{r.products?.length ?? 0} products</td>
                    <td><span className={`badge ${r.isActive ? 'badge-sage' : 'badge-admin'}`}>{r.isActive ? 'Active' : 'Inactive'}</span></td>
                    <td>
                      <div style={{display:'flex',gap:6}}>
                        <button className="btn btn-secondary btn-sm" onClick={() => openEdit(r)}>Edit</button>
                        <button className="btn btn-danger btn-sm" onClick={() => del(r.id)}>Delete</button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {modal && (
        <div className="modal-overlay open" onClick={e => e.target.classList.contains('modal-overlay') && setModal(null)}>
          <div className="modal">
            <div className="modal-header">
              <span className="modal-title">{modal==='create'?'New Routine':'Edit Routine'}</span>
              <button className="modal-close" onClick={() => setModal(null)}>×</button>
            </div>
            <div className="form-group">
              <label className="form-label">Routine Name</label>
              <input className="form-control" value={form.name} onChange={e => setForm({...form,name:e.target.value})} />
            </div>
            <div className="form-group">
              <label className="form-label">Time of Day</label>
              <select className="form-control" value={form.timeOfDay} onChange={e => setForm({...form,timeOfDay:e.target.value})}>
                <option>AM</option><option>PM</option>
              </select>
            </div>
            <div className="form-group">
              <label className="form-label">Days</label>
              <div style={{display:'flex',gap:6,marginTop:6}}>
                {DAYS.map(d => (
                  <button type="button" key={d}
                    className={`btn btn-sm ${selectedDays.includes(d)?'btn-primary':'btn-ghost'}`}
                    onClick={() => toggleDay(d)}>{d}</button>
                ))}
              </div>
            </div>
            {modal !== 'create' && (
              <div className="form-group">
                <label className="form-label">
                  <input type="checkbox" checked={form.isActive ?? true}
                    onChange={e => setForm({...form,isActive:e.target.checked})} style={{marginRight:6}} />
                  Active
                </label>
              </div>
            )}
            <div className="form-group">
              <label className="form-label">Products</label>
              <div style={{display:'flex',flexWrap:'wrap',gap:6,marginTop:6,maxHeight:140,overflowY:'auto'}}>
                {products.map(p => {
                  const sel = form.products.find(x => x.productId === p.id);
                  return (
                    <button type="button" key={p.id}
                      className={`btn btn-sm ${sel?'btn-primary':'btn-ghost'}`}
                      onClick={() => toggleProduct(p.id)}>
                      {sel ? `${sel.stepOrder}. ` : ''}{p.name}
                    </button>
                  );
                })}
              </div>
            </div>
            {conflicts.length > 0 && (
              <div className="conflict-box">
                <h4>⚠️ Ingredient Conflicts</h4>
                {conflicts.map((c,i) => (
                  <div key={i} className="conflict-item">• {c.message}</div>
                ))}
              </div>
            )}
            <div className="modal-footer">
              <button className="btn btn-ghost" onClick={() => setModal(null)}>Cancel</button>
              <button className="btn btn-primary" onClick={save}>Save</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
