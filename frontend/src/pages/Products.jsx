import { useState, useEffect } from 'react';
import { api } from '../api';
import { useAuth } from '../AuthContext';

const CATEGORIES = ['Cleanser','Toner','Serum','Moisturizer','SPF','Treatment','Eye','Mask'];
const blank = { name:'', brand:'', category:'Cleanser', description:'', ingredientIds:[] };

export default function Products() {
  const { user } = useAuth();
  const isAdmin = user?.role === 'Admin';
  const [products, setProducts] = useState([]);
  const [ingredients, setIngredients] = useState([]);
  const [modal, setModal] = useState(null);
  const [form, setForm] = useState(blank);
  const [msg, setMsg] = useState('');
  const [loading, setLoading] = useState(true);

  const load = () => Promise.all([api.getProducts(), api.getIngredients()])
    .then(([p, i]) => { setProducts(p); setIngredients(i); }).finally(() => setLoading(false));

  useEffect(() => { load(); }, []);

  const openCreate = () => { setForm(blank); setModal('create'); };
  const openEdit = (p) => {
    setForm({ name:p.name, brand:p.brand, category:p.category, description:p.description,
      ingredientIds: ingredients.filter(i => p.ingredients.includes(i.name)).map(i => i.id) });
    setModal(p);
  };

  const save = async () => {
    try {
      if (modal === 'create') await api.createProduct(form);
      else await api.updateProduct(modal.id, form);
      setMsg('✅ Product saved!'); setModal(null); load();
    } catch(e) { setMsg('❌ ' + e.message); }
  };

  const del = async (id) => {
    if (!confirm('Delete product?')) return;
    await api.deleteProduct(id); setMsg('Deleted.'); load();
  };

  const toggleIngredient = (id) => {
    setForm(f => ({
      ...f,
      ingredientIds: f.ingredientIds.includes(id)
        ? f.ingredientIds.filter(x => x !== id)
        : [...f.ingredientIds, id]
    }));
  };

  if (loading) return <div className="loading">Loading...</div>;

  return (
    <div>
      <div className="page-header">
        <h2>🧴 Products</h2>
        <p>Skincare product library</p>
      </div>
      {msg && <div className="alert alert-success" onClick={() => setMsg('')}>{msg}</div>}
      <div className="card">
        <div className="card-header">
          <span className="card-title">All Products ({products.length})</span>
          {isAdmin && <button className="btn btn-primary" onClick={openCreate}>+ Add Product</button>}
        </div>
        {products.length === 0 ? (
          <div className="empty-state"><div className="empty-icon">🧴</div><h3>No products yet</h3></div>
        ) : (
          <div className="table-wrap">
            <table>
              <thead><tr><th>Name</th><th>Brand</th><th>Category</th><th>Ingredients</th>{isAdmin && <th></th>}</tr></thead>
              <tbody>
                {products.map(p => (
                  <tr key={p.id}>
                    <td style={{fontWeight:600}}>{p.name}</td>
                    <td>{p.brand}</td>
                    <td><span className="badge badge-rose">{p.category}</span></td>
                    <td style={{maxWidth:200}}>
                      {p.ingredients.slice(0,3).map(i => (
                        <span key={i} className="badge badge-sage" style={{marginRight:4}}>{i}</span>
                      ))}
                      {p.ingredients.length > 3 && <span style={{fontSize:11,color:'var(--text-light)'}}>+{p.ingredients.length-3} more</span>}
                    </td>
                    {isAdmin && (
                      <td>
                        <div style={{display:'flex',gap:6}}>
                          <button className="btn btn-secondary btn-sm" onClick={() => openEdit(p)}>Edit</button>
                          <button className="btn btn-danger btn-sm" onClick={() => del(p.id)}>Delete</button>
                        </div>
                      </td>
                    )}
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
              <span className="modal-title">{modal==='create'?'Add Product':'Edit Product'}</span>
              <button className="modal-close" onClick={() => setModal(null)}>×</button>
            </div>
            <div className="form-row">
              <div className="form-group">
                <label className="form-label">Name</label>
                <input className="form-control" value={form.name} onChange={e => setForm({...form,name:e.target.value})} />
              </div>
              <div className="form-group">
                <label className="form-label">Brand</label>
                <input className="form-control" value={form.brand} onChange={e => setForm({...form,brand:e.target.value})} />
              </div>
            </div>
            <div className="form-group">
              <label className="form-label">Category</label>
              <select className="form-control" value={form.category} onChange={e => setForm({...form,category:e.target.value})}>
                {CATEGORIES.map(c => <option key={c}>{c}</option>)}
              </select>
            </div>
            <div className="form-group">
              <label className="form-label">Description</label>
              <textarea className="form-control" rows={2} value={form.description}
                onChange={e => setForm({...form,description:e.target.value})} />
            </div>
            <div className="form-group">
              <label className="form-label">Ingredients</label>
              <div style={{display:'flex',flexWrap:'wrap',gap:6,marginTop:6,maxHeight:120,overflowY:'auto'}}>
                {ingredients.map(i => (
                  <button type="button" key={i.id}
                    className={`btn btn-sm ${form.ingredientIds.includes(i.id)?'btn-primary':'btn-ghost'}`}
                    onClick={() => toggleIngredient(i.id)}>{i.name}</button>
                ))}
              </div>
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
