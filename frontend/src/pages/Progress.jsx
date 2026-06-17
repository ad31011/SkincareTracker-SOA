import { useState, useEffect } from 'react';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer } from 'recharts';
import { api } from '../api';

const fmt = (d) => new Date(d).toISOString().slice(0,10);

export default function Progress() {
  const [from, setFrom] = useState(fmt(Date.now() - 29*86400000));
  const [to, setTo] = useState(fmt(Date.now()));
  const [data, setData] = useState([]);
  const [streak, setStreak] = useState(null);
  const [loading, setLoading] = useState(true);

  const levelToNum = (l) => l === 'High' ? 3 : l === 'Medium' ? 2 : 1;

  const load = () => {
    setLoading(true);
    Promise.all([api.getProgress(from, to), api.getStreak()])
      .then(([p, s]) => {
        setData(p.map(d => ({
          date: new Date(d.date).toLocaleDateString(),
          Score: d.score,
          Hydration: levelToNum(d.hydration),
          Oiliness: levelToNum(d.oiliness),
          Sensitivity: levelToNum(d.sensitivity),
        })));
        setStreak(s);
      }).finally(() => setLoading(false));
  };

  useEffect(() => { load(); }, []);

  return (
    <div>
      <div className="page-header"><h2>📈 Progress</h2><p>Track your skin improvement over time</p></div>

      <div className="stats-row">
        <div className="stat-card"><div className="stat-label">🔥 Streak</div><div className="stat-value">{streak?.currentStreak ?? 0}</div><div className="stat-sub">days</div></div>
        <div className="stat-card"><div className="stat-label">🏆 Longest</div><div className="stat-value">{streak?.longestStreak ?? 0}</div><div className="stat-sub">days</div></div>
        <div className="stat-card"><div className="stat-label">📝 Total Logs</div><div className="stat-value">{streak?.totalLogs ?? 0}</div></div>
        <div className="stat-card"><div className="stat-label">📊 Data Points</div><div className="stat-value">{data.length}</div><div className="stat-sub">in range</div></div>
      </div>

      <div className="card" style={{marginBottom:20}}>
        <div style={{display:'flex',gap:16,alignItems:'flex-end'}}>
          <div className="form-group" style={{marginBottom:0}}>
            <label className="form-label">From</label>
            <input type="date" className="form-control" value={from} onChange={e => setFrom(e.target.value)} />
          </div>
          <div className="form-group" style={{marginBottom:0}}>
            <label className="form-label">To</label>
            <input type="date" className="form-control" value={to} onChange={e => setTo(e.target.value)} />
          </div>
          <button className="btn btn-primary" onClick={load}>Filter</button>
        </div>
      </div>

      {loading ? <div className="loading">Loading chart...</div> : data.length === 0 ? (
        <div className="card"><div className="empty-state"><div className="empty-icon">📈</div><h3>No data in this range</h3><p>Log your skin condition to see progress</p></div></div>
      ) : (
        <>
          <div className="card" style={{marginBottom:20}}>
            <div className="card-title" style={{marginBottom:16}}>Skin Condition Score</div>
            <ResponsiveContainer width="100%" height={240}>
              <LineChart data={data}>
                <CartesianGrid strokeDasharray="3 3" stroke="var(--border)" />
                <XAxis dataKey="date" tick={{fontSize:11}} />
                <YAxis domain={[1,10]} tick={{fontSize:11}} />
                <Tooltip />
                <Line type="monotone" dataKey="Score" stroke="#c97070" strokeWidth={2} dot={{r:3}} />
              </LineChart>
            </ResponsiveContainer>
          </div>
          <div className="card">
            <div className="card-title" style={{marginBottom:16}}>Hydration / Oiliness / Sensitivity (1=Low, 3=High)</div>
            <ResponsiveContainer width="100%" height={240}>
              <LineChart data={data}>
                <CartesianGrid strokeDasharray="3 3" stroke="var(--border)" />
                <XAxis dataKey="date" tick={{fontSize:11}} />
                <YAxis domain={[1,3]} ticks={[1,2,3]} tickFormatter={v => ['','Low','Med','High'][v]} tick={{fontSize:11}} />
                <Tooltip />
                <Legend />
                <Line type="monotone" dataKey="Hydration" stroke="#8fad8f" strokeWidth={2} dot={false} />
                <Line type="monotone" dataKey="Oiliness" stroke="#e8a0a0" strokeWidth={2} dot={false} />
                <Line type="monotone" dataKey="Sensitivity" stroke="#c9a030" strokeWidth={2} dot={false} />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </>
      )}
    </div>
  );
}
