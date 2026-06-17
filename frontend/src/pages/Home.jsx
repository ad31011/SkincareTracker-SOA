import { Link } from 'react-router-dom';

export default function Home() {
  return (
    <div style={{ fontFamily: "'Segoe UI', system-ui, sans-serif", background: '#0d0608', minHeight: '100vh', color: '#fff', overflowX: 'hidden' }}>

      {/* NAV */}
      <nav style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '20px 48px', position: 'fixed', top: 0, left: 0, right: 0, zIndex: 100, background: 'rgba(13,6,8,0.85)', backdropFilter: 'blur(12px)', borderBottom: '1px solid rgba(201,112,112,0.1)' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
          <span style={{ fontSize: 22 }}>🌸</span>
          <span style={{ fontWeight: 800, fontSize: 18, color: '#e8a0a0', letterSpacing: '-0.02em' }}>Skincare Tracker</span>
        </div>
        <div style={{ display: 'flex', gap: 12 }}>
          <Link to="/login" style={{ padding: '9px 22px', borderRadius: 8, border: '1px solid rgba(201,112,112,0.4)', color: '#e8a0a0', textDecoration: 'none', fontSize: 14, fontWeight: 600, transition: 'all .15s' }}
            onMouseEnter={e => e.target.style.background='rgba(201,112,112,0.1)'}
            onMouseLeave={e => e.target.style.background='transparent'}>
            Sign In
          </Link>
          <Link to="/register" style={{ padding: '9px 22px', borderRadius: 8, background: '#c97070', color: '#fff', textDecoration: 'none', fontSize: 14, fontWeight: 600 }}>
            Get Started
          </Link>
        </div>
      </nav>

      {/* HERO */}
      <section style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column', alignItems: 'center', justifyContent: 'center', textAlign: 'center', padding: '120px 24px 80px', position: 'relative' }}>
        {/* Background glow */}
        <div style={{ position: 'absolute', top: '20%', left: '50%', transform: 'translateX(-50%)', width: 600, height: 600, background: 'radial-gradient(circle, rgba(201,112,112,0.15) 0%, transparent 70%)', pointerEvents: 'none' }} />
        <div style={{ position: 'absolute', top: '10%', left: '10%', width: 300, height: 300, background: 'radial-gradient(circle, rgba(143,173,143,0.08) 0%, transparent 70%)', pointerEvents: 'none' }} />

        <div style={{ position: 'relative', zIndex: 1 }}>
          <div style={{ display: 'inline-flex', alignItems: 'center', gap: 8, background: 'rgba(201,112,112,0.12)', border: '1px solid rgba(201,112,112,0.25)', borderRadius: 99, padding: '6px 16px', fontSize: 13, color: '#e8a0a0', marginBottom: 32, fontWeight: 500 }}>
            <span>✨</span> Track · Analyse · Glow
          </div>

          <h1 style={{ fontSize: 'clamp(40px, 7vw, 80px)', fontWeight: 900, lineHeight: 1.05, letterSpacing: '-0.04em', margin: '0 0 24px', maxWidth: 800 }}>
            Your skin deserves
            <br />
            <span style={{ background: 'linear-gradient(135deg, #e8a0a0 0%, #c97070 50%, #f5dada 100%)', WebkitBackgroundClip: 'text', WebkitTextFillColor: 'transparent', backgroundClip: 'text' }}>
              a smarter routine
            </span>
          </h1>

          <p style={{ fontSize: 18, color: 'rgba(255,255,255,0.55)', maxWidth: 520, margin: '0 auto 40px', lineHeight: 1.7 }}>
            Log your skin condition daily, build ingredient-aware routines, and watch your progress unfold over time.
          </p>

          <div style={{ display: 'flex', gap: 14, justifyContent: 'center', flexWrap: 'wrap' }}>
            <Link to="/register" style={{ padding: '14px 32px', borderRadius: 10, background: '#c97070', color: '#fff', textDecoration: 'none', fontSize: 16, fontWeight: 700, boxShadow: '0 0 30px rgba(201,112,112,0.4)' }}>
              Start for free →
            </Link>
            <Link to="/login" style={{ padding: '14px 32px', borderRadius: 10, border: '1px solid rgba(255,255,255,0.12)', color: 'rgba(255,255,255,0.7)', textDecoration: 'none', fontSize: 16, fontWeight: 600 }}>
              Sign in
            </Link>
          </div>

          {/* Stats row */}
          <div style={{ display: 'flex', gap: 48, justifyContent: 'center', marginTop: 72, flexWrap: 'wrap' }}>
            {[['Daily Logs', 'Track every day'], ['Ingredient Checker', 'Avoid conflicts'], ['Progress Charts', 'See trends']].map(([title, sub]) => (
              <div key={title} style={{ textAlign: 'center' }}>
                <div style={{ fontSize: 16, fontWeight: 700, color: '#e8a0a0' }}>{title}</div>
                <div style={{ fontSize: 13, color: 'rgba(255,255,255,0.4)', marginTop: 2 }}>{sub}</div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* FEATURES */}
      <section style={{ padding: '100px 48px', maxWidth: 1100, margin: '0 auto' }}>
        <div style={{ textAlign: 'center', marginBottom: 64 }}>
          <p style={{ fontSize: 13, fontWeight: 700, letterSpacing: '.1em', textTransform: 'uppercase', color: '#c97070', marginBottom: 12 }}>Everything you need</p>
          <h2 style={{ fontSize: 42, fontWeight: 800, letterSpacing: '-0.03em', margin: 0 }}>Built for your skin journey</h2>
        </div>

        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))', gap: 20 }}>
          {[
            { icon: '📓', title: 'Daily Skin Logs', desc: 'Track hydration, oiliness, and sensitivity every day with a simple score. Build a complete picture of your skin over time.' },
            { icon: '⚠️', title: 'Conflict Checker', desc: 'Automatically flags incompatible ingredient combinations like Retinol + AHA before you apply them — no dermatology degree needed.' },
            { icon: '🌿', title: 'Routine Builder', desc: 'Create AM and PM routines with the exact products you use. Set which days of the week each routine runs.' },
            { icon: '📈', title: 'Progress Charts', desc: 'Interactive charts show how your skin condition, hydration, and oiliness trend over any date range you choose.' },
            { icon: '🧴', title: 'Product Library', desc: 'A shared library of skincare products tagged with their active ingredients. Browse what works before you buy.' },
            { icon: '🔐', title: 'Private & Secure', desc: 'Your skin data is yours. JWT authentication keeps every log, routine, and note private to your account.' },
          ].map(f => (
            <div key={f.title} style={{ background: 'rgba(255,255,255,0.03)', border: '1px solid rgba(255,255,255,0.07)', borderRadius: 16, padding: '28px 28px 32px' }}>
              <div style={{ fontSize: 30, marginBottom: 16 }}>{f.icon}</div>
              <h3 style={{ fontSize: 17, fontWeight: 700, margin: '0 0 10px', color: '#f5dada' }}>{f.title}</h3>
              <p style={{ fontSize: 14, color: 'rgba(255,255,255,0.45)', lineHeight: 1.7, margin: 0 }}>{f.desc}</p>
            </div>
          ))}
        </div>
      </section>

      {/* HOW IT WORKS */}
      <section style={{ padding: '80px 48px', background: 'rgba(201,112,112,0.04)', borderTop: '1px solid rgba(201,112,112,0.08)', borderBottom: '1px solid rgba(201,112,112,0.08)' }}>
        <div style={{ maxWidth: 900, margin: '0 auto', textAlign: 'center' }}>
          <p style={{ fontSize: 13, fontWeight: 700, letterSpacing: '.1em', textTransform: 'uppercase', color: '#c97070', marginBottom: 12 }}>How it works</p>
          <h2 style={{ fontSize: 38, fontWeight: 800, letterSpacing: '-0.03em', marginBottom: 56 }}>Three steps to better skin</h2>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(220px, 1fr))', gap: 32 }}>
            {[
              { n: '01', title: 'Create your account', desc: 'Sign up and set your skin type and concerns. Takes 30 seconds.' },
              { n: '02', title: 'Build your routine', desc: 'Add the products you already use and set your AM/PM schedule.' },
              { n: '03', title: 'Log & watch trends', desc: 'Rate your skin daily. Charts update automatically as you log.' },
            ].map(s => (
              <div key={s.n} style={{ textAlign: 'left' }}>
                <div style={{ fontSize: 48, fontWeight: 900, color: 'rgba(201,112,112,0.2)', lineHeight: 1, marginBottom: 16, letterSpacing: '-0.04em' }}>{s.n}</div>
                <h3 style={{ fontSize: 16, fontWeight: 700, margin: '0 0 8px', color: '#f5dada' }}>{s.title}</h3>
                <p style={{ fontSize: 14, color: 'rgba(255,255,255,0.4)', lineHeight: 1.7, margin: 0 }}>{s.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA */}
      <section style={{ padding: '100px 24px', textAlign: 'center', position: 'relative' }}>
        <div style={{ position: 'absolute', top: '50%', left: '50%', transform: 'translate(-50%,-50%)', width: 500, height: 300, background: 'radial-gradient(circle, rgba(201,112,112,0.12) 0%, transparent 70%)', pointerEvents: 'none' }} />
        <div style={{ position: 'relative', zIndex: 1 }}>
          <h2 style={{ fontSize: 'clamp(32px, 5vw, 56px)', fontWeight: 900, letterSpacing: '-0.04em', marginBottom: 20 }}>
            Ready to understand<br />your skin?
          </h2>
          <p style={{ fontSize: 16, color: 'rgba(255,255,255,0.45)', marginBottom: 36 }}>Free to use. No credit card required.</p>
          <Link to="/register" style={{ display: 'inline-block', padding: '16px 40px', borderRadius: 12, background: '#c97070', color: '#fff', textDecoration: 'none', fontSize: 17, fontWeight: 700, boxShadow: '0 0 40px rgba(201,112,112,0.35)' }}>
            Create your account →
          </Link>
        </div>
      </section>

      {/* FOOTER */}
      <footer style={{ borderTop: '1px solid rgba(255,255,255,0.06)', padding: '28px 48px', display: 'flex', alignItems: 'center', justifyContent: 'space-between', flexWrap: 'wrap', gap: 12 }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
          <span>🌸</span>
          <span style={{ fontWeight: 700, color: '#e8a0a0', fontSize: 14 }}>Skincare Tracker</span>
        </div>
        <p style={{ fontSize: 12, color: 'rgba(255,255,255,0.25)', margin: 0 }}>
          Built for Service Oriented Architecture · South East European University · 2026
        </p>
      </footer>
    </div>
  );
}
