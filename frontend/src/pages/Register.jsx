import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { api } from '../api';

const SKIN_TYPES = ['Oily','Dry','Combination','Normal','Sensitive'];
const CONCERNS = ['Acne','Wrinkles','Hyperpigmentation','Dryness','Sensitivity','Redness','Pores'];

export default function Register() {
  const navigate = useNavigate();
  const [form, setForm] = useState({ name:'', email:'', password:'', skinType:'Normal', skinConcerns:[] });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const toggleConcern = (c) => {
    setForm(f => ({
      ...f,
      skinConcerns: f.skinConcerns.includes(c)
        ? f.skinConcerns.filter(x => x !== c)
        : [...f.skinConcerns, c]
    }));
  };

  const submit = async (e) => {
    e.preventDefault();
    if (form.password.length < 6) { setError('Password must be at least 6 characters.'); return; }
    setLoading(true); setError('');
    try {
      await api.register({ ...form, skinConcerns: form.skinConcerns.join(',') });
      navigate('/login');
    } catch (err) { setError(err.message); }
    finally { setLoading(false); }
  };

  return (
    <div className="auth-page">
      <div className="auth-card">
        <div className="auth-logo">
          <span className="logo-icon">🌸</span>
          <h1>Create Account</h1>
          <p>Start tracking your skin journey</p>
        </div>
        {error && <div className="alert alert-error">{error}</div>}
        <form onSubmit={submit}>
          <div className="form-group">
            <label className="form-label">Full Name</label>
            <input className="form-control" required value={form.name}
              onChange={e => setForm({...form, name: e.target.value})} />
          </div>
          <div className="form-group">
            <label className="form-label">Email</label>
            <input className="form-control" type="email" required value={form.email}
              onChange={e => setForm({...form, email: e.target.value})} />
          </div>
          <div className="form-group">
            <label className="form-label">Password</label>
            <input className="form-control" type="password" required value={form.password}
              onChange={e => setForm({...form, password: e.target.value})} />
          </div>
          <div className="form-group">
            <label className="form-label">Skin Type</label>
            <select className="form-control" value={form.skinType}
              onChange={e => setForm({...form, skinType: e.target.value})}>
              {SKIN_TYPES.map(t => <option key={t}>{t}</option>)}
            </select>
          </div>
          <div className="form-group">
            <label className="form-label">Skin Concerns</label>
            <div style={{display:'flex',flexWrap:'wrap',gap:8,marginTop:6}}>
              {CONCERNS.map(c => (
                <button type="button" key={c}
                  className={`btn btn-sm ${form.skinConcerns.includes(c) ? 'btn-primary' : 'btn-ghost'}`}
                  onClick={() => toggleConcern(c)}>{c}</button>
              ))}
            </div>
          </div>
          <button className="btn btn-primary btn-full" type="submit" disabled={loading}>
            {loading ? 'Creating...' : 'Create Account'}
          </button>
        </form>
        <div className="auth-footer">
          Have an account? <Link to="/login">Sign in</Link>
        </div>
      </div>
    </div>
  );
}
