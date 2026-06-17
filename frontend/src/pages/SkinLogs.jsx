import { useState, useEffect } from 'react';
import { api } from '../api';

const LEVELS = ['Low', 'Medium', 'High'];
const blank = { date: new Date().toISOString().slice(0,10), skinConditionScore: 7, hydration: 'Medium', oiliness: 'Medium', sensitivity: 'Low', notes: '' };

export default function SkinLogs() {
  const [logs, setLogs] = useState([]);
  const [streak, setStreak] = useState(null);
  const [modal, setModal] = useState(null); // null | 'create' | {log}
  const [form, setForm] = useState(blank);
  const [loading, setLoading] = useState(true);
  const [msg, setMsg] = useState('');

  const load = () => Promise.all([api.getSkinLogs(), api.getStreak()])
    .then(([l, s]) => { setLogs(l); setStreak(s); }).finally(() => setLoading(false));

  useEffect(() => { load(); }, []);

  const openCreate = () => { setForm(blank); setModal('create'); };
  const openEdit = (log) => {
    setForm({ date: log.date.slice(0,10), skinConditionScore: log.skinConditionScore,
      hydration: log.hydration, oiliness: log.oiliness, sensitivity: log.sensitivity, notes: log.notes || '' });
    setModal(log);
  };

  const save = async () => {
    try {
      const payload = { ...form, date: new Date(form.date).toISOString(), productsUsed: '' };
      if (modal === 'create') await api.createSkinLog(payload);
      else await api.updateSkinLog(modal.id, { skinConditionScore: form.skinConditionScore,
        hydration: form.hydration, oiliness: form.oiliness, sensitivity: form.sensitivity,
        notes: form.notes, productsUsed: '' });
      setMsg('✅ Log saved!'); setModal(null); load();
    } catch (e) { setMsg('❌ ' + e.message); }
  };

  const del = async (id) => {
    if (!confirm('Delete this log?')) return;
    await api.deleteSkinLog(id); setMsg('Deleted.'); load();
  };

  if (loading) return <div className="loading">Loading...</div>;

  return (
    <div>
      <div className="page-header">
        <h2>📓 Skin Logs</h2>
        <p>Track your daily skin condition</p>
      </div>

      {msg && <div className="alert alert-success" onClick={() => setMsg('')}>{msg}</div>}

      <div className="stats-row" style={{marginBottom:24}}>
        <div className="stat-card"><div className="stat-label">🔥 Streak</div><div className="stat-value">{streak?.currentStreak ?? 0}</div><div className="stat-sub">days</div></div>
        <div className="stat-card"><div className="stat-label">🏆 Longest</div><div className="stat-value">{streak?.longestStreak ?? 0}</div><div className="stat-sub">days</div></div>
        <div className="stat-card"><div className="stat-label">📝 Total</div><div className="stat-value">{streak?.totalLogs ?? 0}</div><div className="stat-sub">logs</div></div>
      </div>

      <div className="card">
        <div className="card-header">
          <span className="card-title">All Logs</span>
          <button className="btn btn-primary" onClick={openCreate}>+ New Log</button>
        </div>
        {logs.length === 0 ? (
          <div className="empty-state"><div className="empty-icon">📓</div><h3>No logs yet</h3><p>Add your first skin log to start tracking</p></div>
        ) : (
          <div className="table-wrap">
            <table>
              <thead><tr><th>Date</th><th>Score</th><th>Hydration</th><th>Oiliness</th><th>Sensitivity</th><th>Notes</th><th></th></tr></thead>
              <tbody>
                {logs.map(l => (
                  <tr key={l.id}>
                    <td>{new Date(l.date).toLocaleDateString()}</td>
                    <td>
                      <div style={{display:'flex',alignItems:'center',gap:8}}>
                        {l.skinConditionScore}/10
                        <div className="score-bar" style={{width:50}}><div className="score-fill" style={{width:`${l.skinConditionScore*10}%`}}/></div>
                      </div>
                    </td>
                    <td><span className="badge badge-rose">{l.hydration}</span></td>
                    <td><span className="badge badge-sage">{l.oiliness}</span></td>
                    <td><span className="badge badge-rose">{l.sensitivity}</span></td>
                    <td style={{maxWidth:200,overflow:'hidden',textOverflow:'ellipsis',whiteSpace:'nowrap'}}>{l.notes}</td>
                    <td>
                      <div style={{display:'flex',gap:6}}>
                        <button className="btn btn-secondary btn-sm" onClick={() => openEdit(l)}>Edit</button>
                        <button className="btn btn-danger btn-sm" onClick={() => del(l.id)}>Delete</button>
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
              <span className="modal-title">{modal === 'create' ? 'New Skin Log' : 'Edit Log'}</span>
              <button className="modal-close" onClick={() => setModal(null)}>×</button>
            </div>
            {modal === 'create' && (
              <div className="form-group">
                <label className="form-label">Date</label>
                <input type="date" className="form-control" value={form.date}
                  onChange={e => setForm({...form, date: e.target.value})} />
              </div>
            )}
            <div className="form-group">
              <label className="form-label">Skin Condition Score: {form.skinConditionScore}/10</label>
              <input type="range" min="1" max="10" value={form.skinConditionScore}
                onChange={e => setForm({...form, skinConditionScore: +e.target.value})}
                style={{width:'100%', accentColor:'var(--rose-dark)'}} />
            </div>
            {['hydration','oiliness','sensitivity'].map(field => (
              <div className="form-group" key={field}>
                <label className="form-label">{field.charAt(0).toUpperCase()+field.slice(1)}</label>
                <select className="form-control" value={form[field]}
                  onChange={e => setForm({...form, [field]: e.target.value})}>
                  {LEVELS.map(l => <option key={l}>{l}</option>)}
                </select>
              </div>
            ))}
            <div className="form-group">
              <label className="form-label">Notes</label>
              <textarea className="form-control" rows={3} value={form.notes}
                onChange={e => setForm({...form, notes: e.target.value})} />
            </div>
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
