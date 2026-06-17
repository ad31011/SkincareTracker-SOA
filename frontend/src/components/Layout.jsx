import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../AuthContext';

export default function Layout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => { logout(); navigate('/login'); };

  return (
    <div className="app-shell">
      <div className="sidebar">
        <div className="sidebar-logo">
          <span className="logo-icon">🌸</span>
          <h1>Skincare<br />Tracker</h1>
        </div>

        <span className="nav-section-label">My Skin</span>
        <NavLink className={({isActive}) => `nav-item${isActive?' active':''}`} to="/dashboard">
          <span className="nav-icon">🏠</span> Dashboard
        </NavLink>
        <NavLink className={({isActive}) => `nav-item${isActive?' active':''}`} to="/skinlogs">
          <span className="nav-icon">📓</span> Skin Logs
        </NavLink>
        <NavLink className={({isActive}) => `nav-item${isActive?' active':''}`} to="/routines">
          <span className="nav-icon">🌿</span> Routines
        </NavLink>
        <NavLink className={({isActive}) => `nav-item${isActive?' active':''}`} to="/progress">
          <span className="nav-icon">📈</span> Progress
        </NavLink>

        <span className="nav-section-label">Library</span>
        <NavLink className={({isActive}) => `nav-item${isActive?' active':''}`} to="/products">
          <span className="nav-icon">🧴</span> Products
        </NavLink>
        <NavLink className={({isActive}) => `nav-item${isActive?' active':''}`} to="/ingredients">
          <span className="nav-icon">🔬</span> Ingredients
        </NavLink>

        {user?.role === 'Admin' && (
          <>
            <span className="nav-section-label">Admin</span>
            <NavLink className={({isActive}) => `nav-item${isActive?' active':''}`} to="/users">
              <span className="nav-icon">👥</span> Users
            </NavLink>
          </>
        )}

        <div className="sidebar-footer">
          <div className="user-pill">
            <div className="user-avatar">🌸</div>
            <div className="user-info">
              <div className="user-name">{user?.name}</div>
              <div className="user-role">{user?.role}</div>
            </div>
          </div>
          <button className="btn btn-ghost btn-full" style={{marginTop:10}} onClick={handleLogout}>
            Sign Out
          </button>
        </div>
      </div>

      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
}
