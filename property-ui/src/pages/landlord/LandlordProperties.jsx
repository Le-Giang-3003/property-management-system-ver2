import { useState } from 'react';
import { properties } from '../../data/mockData';
import { formatMoney, formatDate, getStatusBadge, getPropertyTypeLabel } from '../../utils/helpers';
import { Plus, Edit, Trash, Eye, Home, Bed, Bath, Maximize2 } from 'lucide-react';

const myProps = properties.filter(p => p.landlordId === 2);

export default function LandlordProperties() {
  const [showModal, setShowModal] = useState(false);
  const [selectedProp, setSelectedProp] = useState(null);
  const [viewDetail, setViewDetail] = useState(null);
  const [form, setForm] = useState({
    title: '', description: '', propertyType: 'Apartment', address: '', city: 'TP.HCM',
    district: '', ward: '', area: '', bedrooms: 1, bathrooms: 1, floors: '',
    monthlyRent: '', depositAmount: '', amenities: '', allowPets: false, allowSmoking: false, maxOccupants: ''
  });

  const openCreate = () => { setSelectedProp(null); setForm({ title: '', description: '', propertyType: 'Apartment', address: '', city: 'TP.HCM', district: '', ward: '', area: '', bedrooms: 1, bathrooms: 1, floors: '', monthlyRent: '', depositAmount: '', amenities: '', allowPets: false, allowSmoking: false, maxOccupants: '' }); setShowModal(true); };
  const openEdit = (p) => { setSelectedProp(p); setForm({ title: p.title, description: p.description, propertyType: p.propertyType, address: p.address, city: p.city, district: p.district, ward: p.ward || '', area: p.area, bedrooms: p.bedrooms, bathrooms: p.bathrooms, floors: p.floors || '', monthlyRent: p.monthlyRent, depositAmount: p.depositAmount, amenities: p.amenities ? JSON.parse(p.amenities).join(', ') : '', allowPets: p.allowPets, allowSmoking: p.allowSmoking, maxOccupants: p.maxOccupants || '' }); setShowModal(true); };

  return (
    <div>
      <div className="flex items-center justify-between mb-20">
        <div>
          <div className="page-title">BDS của tôi</div>
          <div className="page-desc">Quản lý và đăng tin bất động sản cho thuê</div>
        </div>
        <button className="btn btn-primary" onClick={openCreate}><Plus size={16} /> Đăng tin mới</button>
      </div>

      <div className="stat-grid" style={{ gridTemplateColumns: 'repeat(5, 1fr)', marginBottom: 20 }}>
        {['Tất cả', 'Còn trống', 'Đã thuê', 'Chờ duyệt', 'Nháp'].map((label, i) => {
          const counts = [myProps.length, myProps.filter(p => p.status === 'Available').length, myProps.filter(p => p.status === 'Rented').length, myProps.filter(p => p.status === 'Pending').length, myProps.filter(p => p.status === 'Draft').length];
          const colors = ['purple', 'green', 'blue', 'yellow', 'gray'];
          return (
            <div key={label} className="stat-card" style={{ padding: '14px 16px' }}>
              <div className="stat-info">
                <div className="stat-label">{label}</div>
                <div className="stat-value" style={{ fontSize: 22 }}>{counts[i]}</div>
              </div>
            </div>
          );
        })}
      </div>

      <div className="property-grid">
        {myProps.map(p => (
          <div key={p.id} className="property-card">
            {p.images[0] ? (
              <img className="property-image" src={p.images[0].imageUrl} alt={p.title} />
            ) : (
              <div className="property-image-placeholder">🏠</div>
            )}
            <div className="property-body">
              <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 6 }}>
                <span className="property-type-badge">{getPropertyTypeLabel(p.propertyType)}</span>
                {getStatusBadge(p.status)}
              </div>
              <div className="property-title">{p.title}</div>
              <div className="property-address">📍 {p.district}, {p.city}</div>
              <div className="property-specs">
                <div className="spec-item"><Bed size={13}/> {p.bedrooms} PN</div>
                <div className="spec-item"><Bath size={13}/> {p.bathrooms} WC</div>
                <div className="spec-item"><Maximize2 size={13}/> {p.area}m²</div>
              </div>
              <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', borderTop: '1px solid var(--border)', paddingTop: 12, marginTop: 4 }}>
                <div>
                  <div className="property-price">{formatMoney(p.monthlyRent)}</div>
                  <div className="property-price-sub">/ tháng</div>
                </div>
                <div style={{ display: 'flex', gap: 6 }}>
                  <button className="btn btn-ghost btn-sm btn-icon" onClick={() => setViewDetail(p)} title="Xem chi tiết"><Eye size={14} /></button>
                  <button className="btn btn-ghost btn-sm btn-icon" onClick={() => openEdit(p)} title="Chỉnh sửa"><Edit size={14} /></button>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Create/Edit Modal */}
      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal" style={{ maxWidth: 700 }} onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">{selectedProp ? 'Chỉnh sửa BDS' : 'Đăng tin BDS mới'}</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setShowModal(false)}>✕</button>
            </div>
            <div className="modal-body">
              <div className="form-row">
                <div className="form-group">
                  <label className="form-label">Tiêu đề *</label>
                  <input className="form-control" placeholder="VD: Căn hộ 2PN Vinhomes..." value={form.title} onChange={e => setForm({ ...form, title: e.target.value })} />
                </div>
                <div className="form-group">
                  <label className="form-label">Loại BDS *</label>
                  <select className="form-control" value={form.propertyType} onChange={e => setForm({ ...form, propertyType: e.target.value })}>
                    {['Apartment', 'House', 'Room', 'Villa', 'Commercial'].map(t => <option key={t} value={t}>{getPropertyTypeLabel(t)}</option>)}
                  </select>
                </div>
              </div>
              <div className="form-group">
                <label className="form-label">Mô tả</label>
                <textarea className="form-control" rows={3} placeholder="Mô tả chi tiết về BDS..." value={form.description} onChange={e => setForm({ ...form, description: e.target.value })} />
              </div>
              <div className="form-row">
                <div className="form-group">
                  <label className="form-label">Địa chỉ *</label>
                  <input className="form-control" placeholder="Số nhà, tên đường..." value={form.address} onChange={e => setForm({ ...form, address: e.target.value })} />
                </div>
                <div className="form-group">
                  <label className="form-label">Thành phố *</label>
                  <input className="form-control" value={form.city} onChange={e => setForm({ ...form, city: e.target.value })} />
                </div>
              </div>
              <div className="form-row">
                <div className="form-group">
                  <label className="form-label">Quận/Huyện *</label>
                  <input className="form-control" value={form.district} onChange={e => setForm({ ...form, district: e.target.value })} />
                </div>
                <div className="form-group">
                  <label className="form-label">Phường/Xã</label>
                  <input className="form-control" value={form.ward} onChange={e => setForm({ ...form, ward: e.target.value })} />
                </div>
              </div>
              <div className="form-row">
                <div className="form-group">
                  <label className="form-label">Diện tích (m²) *</label>
                  <input className="form-control" type="number" value={form.area} onChange={e => setForm({ ...form, area: e.target.value })} />
                </div>
                <div className="form-group">
                  <label className="form-label">Số tầng</label>
                  <input className="form-control" type="number" value={form.floors} onChange={e => setForm({ ...form, floors: e.target.value })} />
                </div>
              </div>
              <div className="form-row">
                <div className="form-group">
                  <label className="form-label">Phòng ngủ *</label>
                  <input className="form-control" type="number" min={0} value={form.bedrooms} onChange={e => setForm({ ...form, bedrooms: e.target.value })} />
                </div>
                <div className="form-group">
                  <label className="form-label">Phòng tắm *</label>
                  <input className="form-control" type="number" min={1} value={form.bathrooms} onChange={e => setForm({ ...form, bathrooms: e.target.value })} />
                </div>
              </div>
              <div className="form-row">
                <div className="form-group">
                  <label className="form-label">Tiền thuê/tháng (VND) *</label>
                  <input className="form-control" type="number" value={form.monthlyRent} onChange={e => setForm({ ...form, monthlyRent: e.target.value })} />
                </div>
                <div className="form-group">
                  <label className="form-label">Tiền đặt cọc (VND) *</label>
                  <input className="form-control" type="number" value={form.depositAmount} onChange={e => setForm({ ...form, depositAmount: e.target.value })} />
                </div>
              </div>
              <div className="form-group">
                <label className="form-label">Tiện ích (phân cách bởi dấu phẩy)</label>
                <input className="form-control" placeholder="VD: Wifi, Điều hoà, Thang máy, Bãi xe..." value={form.amenities} onChange={e => setForm({ ...form, amenities: e.target.value })} />
              </div>
              <div style={{ display: 'flex', gap: 20 }}>
                <label style={{ display: 'flex', alignItems: 'center', gap: 8, cursor: 'pointer', fontSize: 13 }}>
                  <input type="checkbox" checked={form.allowPets} onChange={e => setForm({ ...form, allowPets: e.target.checked })} />
                  <span>Được nuôi thú cưng</span>
                </label>
                <label style={{ display: 'flex', alignItems: 'center', gap: 8, cursor: 'pointer', fontSize: 13 }}>
                  <input type="checkbox" checked={form.allowSmoking} onChange={e => setForm({ ...form, allowSmoking: e.target.checked })} />
                  <span>Được hút thuốc</span>
                </label>
              </div>
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setShowModal(false)}>Huỷ</button>
              <button className="btn btn-primary" onClick={() => setShowModal(false)}>{selectedProp ? 'Lưu thay đổi' : 'Đăng tin'}</button>
            </div>
          </div>
        </div>
      )}

      {/* Detail view modal */}
      {viewDetail && (
        <div className="modal-overlay" onClick={() => setViewDetail(null)}>
          <div className="modal" style={{ maxWidth: 680 }} onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">{viewDetail.title}</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setViewDetail(null)}>✕</button>
            </div>
            <div className="modal-body">
              {viewDetail.images[0] && <img src={viewDetail.images[0].imageUrl} alt="" style={{ width: '100%', height: 200, objectFit: 'cover', borderRadius: 8, marginBottom: 16 }} />}
              <div className="grid-2">
                <div>
                  <div className="info-row"><span className="info-label">Trạng thái</span><span className="info-value">{getStatusBadge(viewDetail.status)}</span></div>
                  <div className="info-row"><span className="info-label">Diện tích</span><span className="info-value">{viewDetail.area} m²</span></div>
                  <div className="info-row"><span className="info-label">Phòng ngủ</span><span className="info-value">{viewDetail.bedrooms}</span></div>
                  <div className="info-row"><span className="info-label">Phòng tắm</span><span className="info-value">{viewDetail.bathrooms}</span></div>
                  <div className="info-row"><span className="info-label">Lượt xem</span><span className="info-value">{viewDetail.viewCount} lượt</span></div>
                </div>
                <div>
                  <div className="info-row"><span className="info-label">Địa chỉ</span><span className="info-value">{viewDetail.address}, {viewDetail.district}, {viewDetail.city}</span></div>
                  <div className="info-row"><span className="info-label">Tiền thuê</span><span className="info-value text-green fw-700">{formatMoney(viewDetail.monthlyRent)}</span></div>
                  <div className="info-row"><span className="info-label">Đặt cọc</span><span className="info-value">{formatMoney(viewDetail.depositAmount)}</span></div>
                  <div className="info-row"><span className="info-label">Thú cưng</span><span className="info-value">{viewDetail.allowPets ? '✅ Cho phép' : '❌ Không'}</span></div>
                  <div className="info-row"><span className="info-label">Hút thuốc</span><span className="info-value">{viewDetail.allowSmoking ? '✅ Cho phép' : '❌ Không'}</span></div>
                </div>
              </div>
              <div className="info-row"><span className="info-label">Mô tả</span><span className="info-value">{viewDetail.description}</span></div>
              {viewDetail.amenities && (
                <div className="info-row">
                  <span className="info-label">Tiện ích</span>
                  <div style={{ display: 'flex', flexWrap: 'wrap', gap: 6 }}>
                    {JSON.parse(viewDetail.amenities).map(a => <span key={a} className="badge badge-purple">{a}</span>)}
                  </div>
                </div>
              )}
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setViewDetail(null)}>Đóng</button>
              <button className="btn btn-primary" onClick={() => { setViewDetail(null); openEdit(viewDetail); }}>Chỉnh sửa</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
