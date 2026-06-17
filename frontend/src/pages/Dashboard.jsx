import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { api } from '../api';
import { useAuth } from '../AuthContext';

export default function Dashboard() {
  const { user } = useAuth();
  const [streak, setStreak] = useState(null);
  const [logs, setLogs] = useState([]);
  const [routines, setRoutines] = useState([]);
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([
      api.getStreak(),
      api.getSkinLogs(),
      api.getRoutines(),
      api.getProducts(),
    ]).then(([s, l, r, p]) => {
      setStreak(s); setLogs(l.slice(0,5)); setRoutines(r); setProducts(p);
    }).finally(() => setLoading(false));
  }, []);

  if (loading) return <div className="loading">Loading dashboard...</div>;

  const todayRoutines = routines.filter(r => {
    const day = ['Sun','Mon','Tue','Wed','Thu','Fri','Sat'][new Date().getDay()];
    return r.isActive && r.daysOfWeek.includes(day);
  });

  const avgScore = logs.length
    ? (logs.reduce((s,l) => s + l.skinConditionScore, 0) / logs.length).toFixed(1)
    : '—';

  return (
    <div>
      <div className="page-header">
        <h2>Welcome back, {user?.name} 🌸</h2>
        <p>Here's your skin health summary</p>
      </div>

      <div className="stats-row">
        <div className="stat-card">
          <div className="stat-label">🔥 Current Streak</div>
          <div className="stat-value">{streak?.currentStreak ?? 0}</div>
          <div className="stat-sub">days in a row</div>
        </div>
        <div className="stat-card">
          <div className="stat-label">📝 Total Logs</div>
          <div className="stat-value">{streak?.totalLogs ?? 0}</div>
          <div className="stat-sub">entries logged</div>
        </div>
        <div className="stat-card">
          <div className="stat-label">🌿 Routines</div>
          <div className="stat-value">{routines.length}</div>
          <div className="stat-sub">{todayRoutines.length} active today</div>
        </div>
        <div className="stat-card">
          <div className="stat-label">🧴 Products</div>
          <div className="stat-value">{products.length}</div>
          <div className="stat-sub">in library</div>
        </div>
        <div className="stat-card">
          <div className="stat-label">⭐ Avg Score</div>
          <div className="stat-value">{avgScore}</div>
          <div className="stat-sub">last 5 logs</div>
        </div>
      </div>

      <div className="dashboard-grid">
        <div className="card">
          <div className="card-header">
            <span className="card-title">📓 Recent Skin Logs</span>
            <Link className="btn btn-secondary btn-sm" to="/skinlogs">View All</Link>
          </div>
          {logs.length === 0 ? (
            <div className="empty-state">
              <div className="empty-icon">📓</div>
              <h3>No logs yet</h3>
              <p>Start tracking your skin health</p>
            </div>
          ) : (
            <div className="table-wrap">
              <table>
                <thead><tr><th>Date</th><th>Score</th><th>Hydration</th><th>Oiliness</th></tr></thead>
                <tbody>
                  {logs.map(l => (
                    <tr key={l.id}>
                      <td>{new Date(l.date).toLocaleDateString()}</td>
                      <td>
                        <div style={{display:'flex',alignItems:'center',gap:8}}>
                          {l.skinConditionScore}/10
                          <div className="score-bar" style={{width:60}}>
                            <div className="score-fill" style={{width:`${l.skinConditionScore*10}%`}} />
                          </div>
                        </div>
                      </td>
                      <td><span className="badge badge-rose">{l.hydration}</span></td>
                      <td><span className="badge badge-sage">{l.oiliness}</span></td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>

        <div className="card">
          <div className="card-header">
            <span className="card-title">🌿 Today's Routines</span>
            <Link className="btn btn-secondary btn-sm" to="/routines">Manage</Link>
          </div>
          {todayRoutines.length === 0 ? (
            <div className="empty-state">
              <div className="empty-icon">🌿</div>
              <h3>No routines today</h3>
              <p>Set up your skincare routine</p>
            </div>
          ) : (
            todayRoutines.map(r => (
              <div key={r.id} style={{padding:'12px 0', borderBottom:'1px solid var(--border)'}}>
                <div style={{fontWeight:600,color:'var(--text)'}}>{r.name}</div>
                <div style={{fontSize:12,color:'var(--text-light)',marginTop:3}}>
                  {r.timeOfDay} · {r.products?.length ?? 0} products
                </div>
              </div>
            ))
          )}
        </div>
      </div>
    </div>
  );
}
