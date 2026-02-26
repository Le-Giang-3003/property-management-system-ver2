import { useState } from 'react';
import { properties } from '../../data/mockData';
import { formatMoney, getStatusBadge, getPropertyTypeLabel } from '../../utils/helpers';
import { Search, Filter, Bed, Bath, Maximize2, Heart, MapPin, X } from 'lucide-react';

export default function PropertySearch() {
  const [search, setSearch] = useState('');
  const [filters, setFilters] = useState({ type: 'All', minPrice: '', maxPrice: '', minBedrooms: '', city: '' });
  const [selectedProp, setSelectedProp] = useState(null);
  const [showApplyModal, setShowApplyModal] = useState(false);
  const [showBookingModal, setShowBookingModal] = useState(false);
  const [favorites, setFavorites] = useState([]);
  const [applyForm, setApplyForm] = useState({ moveInDate: '', leaseDuration: 12, occupants: 1, occupation: '', income: '', message: '' });

  const available = properties.filter(p => p.status === 'Available' || p.status === 'Rented');
  const filtered = available.filter(p => {
    const matchSearch = !search || p.title.toLowerCase().includes(search.toLowerCase()) || p.city.toLowerCase().includes(search.toLowerCase()) || p.district.toLowerCase().includes(search.toLowerCase());
    const matchType = filters.type === 'All' || p.propertyType === filters.type;
    const matchMin = !filters.minPrice || p.monthlyRent >= Number(filters.minPrice);
    const matchMax = !filters.maxPrice || p.monthlyRent <= Number(filters.maxPrice);
    const matchBed = !filters.minBedrooms || p.bedrooms >= Number(filters.minBedrooms);
    const matchCity = !filters.city || p.city.toLowerCase().includes(filters.city.toLowerCase());
    return matchSearch && matchType && matchMin && matchMax && matchBed && matchCity;
  });

  const toggleFav = (id) => setFavorites(prev => prev.includes(id) ? prev.filter(f => f !== id) : [...prev, id]);

  return (
    <div>
      <div className="mb-20">
        <div className="page-title">Tìm kiếm Bất động sản</div>
        <div className="page-desc">Tìm thuê nhà phù hợp với nhu cầu của bạn</div>
      </div>

      {/* Search + Filter */}
      <div className="card mb-20">
        <div style={{ display: 'flex', gap: 12, flexWrap: 'wrap', marginBottom: 12 }}>
          <div className="header-search" style={{ flex: 2, minWidth: 200 }}>
            <Search size={14} style={{ color: 'var(--text-muted)' }} />
            <input placeholder="Tìm kiếm tên, quận, thành phố..." value={search} onChange={e => setSearch(e.target.value)} style={{ width: '100%' }} />
          </div>
          <input className="form-control" placeholder="Thành phố" value={filters.city} onChange={e => setFilters({...filters, city: e.target.value})} style={{ flex: 1, minWidth: 120 }} />
        </div>
        <div style={{ display: 'flex', gap: 10, flexWrap: 'wrap' }}>
          <select className="form-control" value={filters.type} onChange={e => setFilters({...filters, type: e.target.value})} style={{ width: 'auto', flex: 1, minWidth: 120 }}>
            <option value="All">Loại BDS</option>
            {['Apartment','House','Room','Villa','Commercial'].map(t => <option key={t} value={t}>{getPropertyTypeLabel(t)}</option>)}
          </select>
          <input className="form-control" type="number" placeholder="Giá từ (VND)" value={filters.minPrice} onChange={e => setFilters({...filters, minPrice: e.target.value})} style={{ flex: 1, minWidth: 140 }} />
          <input className="form-control" type="number" placeholder="Giá đến (VND)" value={filters.maxPrice} onChange={e => setFilters({...filters, maxPrice: e.target.value})} style={{ flex: 1, minWidth: 140 }} />
          <select className="form-control" value={filters.minBedrooms} onChange={e => setFilters({...filters, minBedrooms: e.target.value})} style={{ width: 'auto', flex: 1, minWidth: 120 }}>
            <option value="">Số phòng ngủ</option>
            {[1,2,3,4,5].map(n => <option key={n} value={n}>≥ {n} phòng ngủ</option>)}
          </select>
          <button className="btn btn-ghost btn-sm" onClick={() => setFilters({ type: 'All', minPrice: '', maxPrice: '', minBedrooms: '', city: '' })}>
            <X size={13}/> Xoá lọc
          </button>
        </div>
        <div className="mt-8 text-muted text-sm">Tìm thấy {filtered.length} bất động sản</div>
      </div>

      {/* Property Grid */}
      <div className="property-grid">
        {filtered.map(p => (
          <div key={p.id} className="property-card" style={{ position: 'relative' }}>
            <button style={{ position: 'absolute', top: 10, right: 10, zIndex: 1, background: 'rgba(0,0,0,0.5)', border: 'none', borderRadius: '50%', width: 32, height: 32, display: 'flex', alignItems: 'center', justifyContent: 'center', cursor: 'pointer', color: favorites.includes(p.id) ? '#ef4444' : '#fff' }} onClick={(e) => { e.stopPropagation(); toggleFav(p.id); }}>
              <Heart size={14} fill={favorites.includes(p.id) ? '#ef4444' : 'none'} />
            </button>
            {p.images[0] ? <img className="property-image" src={p.images[0].imageUrl} alt={p.title} onClick={() => setSelectedProp(p)} /> : <div className="property-image-placeholder" onClick={() => setSelectedProp(p)}>🏠</div>}
            <div className="property-body">
              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 6 }}>
                <span className="property-type-badge">{getPropertyTypeLabel(p.propertyType)}</span>
                {getStatusBadge(p.status)}
              </div>
              <div className="property-title" onClick={() => setSelectedProp(p)}>{p.title}</div>
              <div className="property-address"><MapPin size={12}/> {p.district}, {p.city}</div>
              <div className="property-specs">
                <span className="spec-item"><Bed size={13}/> {p.bedrooms} PN</span>
                <span className="spec-item"><Bath size={13}/> {p.bathrooms} WC</span>
                <span className="spec-item"><Maximize2 size={13}/> {p.area}m²</span>
              </div>
              <div style={{ borderTop: '1px solid var(--border)', paddingTop: 12, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                <div>
                  <div className="property-price">{formatMoney(p.monthlyRent)}</div>
                  <div className="property-price-sub">/tháng</div>
                </div>
                <div style={{ display: 'flex', gap: 6 }}>
                  {p.status === 'Available' && <>
                    <button className="btn btn-ghost btn-sm" onClick={() => { setSelectedProp(p); setShowBookingModal(true); }}>📅 Xem nhà</button>
                    <button className="btn btn-primary btn-sm" onClick={() => { setSelectedProp(p); setShowApplyModal(true); }}>Nộp đơn</button>
                  </>}
                  {p.status === 'Rented' && <button className="btn btn-secondary btn-sm" disabled>Đã thuê</button>}
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Detail Modal */}
      {selectedProp && !showApplyModal && !showBookingModal && (
        <div className="modal-overlay" onClick={() => setSelectedProp(null)}>
          <div className="modal" style={{ maxWidth: 720 }} onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">{selectedProp.title}</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setSelectedProp(null)}>✕</button>
            </div>
            <div className="modal-body">
              {selectedProp.images[0] && <img src={selectedProp.images[0].imageUrl} alt="" style={{ width: '100%', height: 240, objectFit: 'cover', borderRadius: 8, marginBottom: 16 }} />}
              <div style={{ display: 'flex', gap: 8, marginBottom: 16 }}>
                <span className="badge badge-purple">{getPropertyTypeLabel(selectedProp.propertyType)}</span>
                {getStatusBadge(selectedProp.status)}
              </div>
              <div className="grid-2">
                <div>
                  <div className="info-row"><span className="info-label">Địa chỉ</span><span className="info-value">{selectedProp.address}, {selectedProp.district}, {selectedProp.city}</span></div>
                  <div className="info-row"><span className="info-label">Diện tích</span><span className="info-value">{selectedProp.area} m²</span></div>
                  <div className="info-row"><span className="info-label">Phòng ngủ</span><span className="info-value">{selectedProp.bedrooms}</span></div>
                  <div className="info-row"><span className="info-label">Phòng tắm</span><span className="info-value">{selectedProp.bathrooms}</span></div>
                  <div className="info-row"><span className="info-label">Số tầng</span><span className="info-value">{selectedProp.floors || '—'}</span></div>
                </div>
                <div>
                  <div className="info-row"><span className="info-label">Tiền thuê/tháng</span><span className="info-value text-green fw-700" style={{ fontSize: 18 }}>{formatMoney(selectedProp.monthlyRent)}</span></div>
                  <div className="info-row"><span className="info-label">Tiền đặt cọc</span><span className="info-value">{formatMoney(selectedProp.depositAmount)}</span></div>
                  <div className="info-row"><span className="info-label">Max người ở</span><span className="info-value">{selectedProp.maxOccupants || '—'}</span></div>
                  <div className="info-row"><span className="info-label">Thú cưng</span><span className="info-value">{selectedProp.allowPets ? '✅' : '❌'}</span></div>
                  <div className="info-row"><span className="info-label">Hút thuốc</span><span className="info-value">{selectedProp.allowSmoking ? '✅' : '❌'}</span></div>
                </div>
              </div>
              <div className="info-row"><span className="info-label">Mô tả</span><span className="info-value">{selectedProp.description}</span></div>
              {selectedProp.amenities && (
                <div className="info-row">
                  <span className="info-label">Tiện ích</span>
                  <div style={{ display: 'flex', flexWrap: 'wrap', gap: 6 }}>
                    {JSON.parse(selectedProp.amenities).map(a => <span key={a} className="badge badge-purple">{a}</span>)}
                  </div>
                </div>
              )}
              {selectedProp.landlord && (
                <div style={{ padding: 16, background: 'var(--bg-primary)', borderRadius: 8, marginTop: 16, display: 'flex', alignItems: 'center', gap: 12 }}>
                  <div className="user-avatar" style={{ width: 44, height: 44, fontSize: 16 }}>{selectedProp.landlord.fullName[0]}</div>
                  <div>
                    <div className="fw-700">{selectedProp.landlord.fullName}</div>
                    <div className="text-muted text-sm">📞 {selectedProp.landlord.phoneNumber}</div>
                    {selectedProp.landlord.isIdentityVerified && <span className="badge badge-success mt-4" style={{ fontSize: 10 }}>✓ Xác thực</span>}
                  </div>
                </div>
              )}
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setSelectedProp(null)}>Đóng</button>
              {selectedProp.status === 'Available' && <>
                <button className="btn btn-ghost" onClick={() => setShowBookingModal(true)}>📅 Đặt lịch xem</button>
                <button className="btn btn-primary" onClick={() => setShowApplyModal(true)}>Nộp đơn thuê</button>
              </>}
            </div>
          </div>
        </div>
      )}

      {/* Apply Modal */}
      {showApplyModal && selectedProp && (
        <div className="modal-overlay" onClick={() => setShowApplyModal(false)}>
          <div className="modal" style={{ maxWidth: 580 }} onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">Nộp đơn xin thuê</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setShowApplyModal(false)}>✕</button>
            </div>
            <div className="modal-body">
              <div style={{ padding: 12, background: 'var(--accent-glow)', borderRadius: 8, marginBottom: 16, fontSize: 13 }}>
                🏠 <strong>{selectedProp.title}</strong> — {formatMoney(selectedProp.monthlyRent)}/tháng
              </div>
              <div className="form-row">
                <div className="form-group"><label className="form-label">Ngày dọn vào *</label><input className="form-control" type="date" value={applyForm.moveInDate} onChange={e => setApplyForm({...applyForm, moveInDate: e.target.value})} /></div>
                <div className="form-group"><label className="form-label">Thời hạn thuê (tháng) *</label><input className="form-control" type="number" min={1} value={applyForm.leaseDuration} onChange={e => setApplyForm({...applyForm, leaseDuration: e.target.value})} /></div>
              </div>
              <div className="form-row">
                <div className="form-group"><label className="form-label">Số người ở *</label><input className="form-control" type="number" min={1} value={applyForm.occupants} onChange={e => setApplyForm({...applyForm, occupants: e.target.value})} /></div>
                <div className="form-group"><label className="form-label">Nghề nghiệp</label><input className="form-control" placeholder="VD: Kỹ sư, Giáo viên..." value={applyForm.occupation} onChange={e => setApplyForm({...applyForm, occupation: e.target.value})} /></div>
              </div>
              <div className="form-group"><label className="form-label">Thu nhập hàng tháng (VND)</label><input className="form-control" type="number" placeholder="0" value={applyForm.income} onChange={e => setApplyForm({...applyForm, income: e.target.value})} /></div>
              <div className="form-group"><label className="form-label">Lời nhắn cho chủ nhà</label><textarea className="form-control" rows={3} placeholder="Giới thiệu bản thân, lý do thuê..." value={applyForm.message} onChange={e => setApplyForm({...applyForm, message: e.target.value})} /></div>
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setShowApplyModal(false)}>Huỷ</button>
              <button className="btn btn-primary" onClick={() => { setShowApplyModal(false); setSelectedProp(null); }}>Gửi đơn xin thuê</button>
            </div>
          </div>
        </div>
      )}

      {/* Booking Modal */}
      {showBookingModal && selectedProp && (
        <div className="modal-overlay" onClick={() => setShowBookingModal(false)}>
          <div className="modal" style={{ maxWidth: 440 }} onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">Đặt lịch xem nhà</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setShowBookingModal(false)}>✕</button>
            </div>
            <div className="modal-body">
              <div style={{ padding: 12, background: 'var(--accent-glow)', borderRadius: 8, marginBottom: 16, fontSize: 13 }}>
                🏠 <strong>{selectedProp.title}</strong>
              </div>
              <div className="form-group"><label className="form-label">Ngày xem *</label><input className="form-control" type="date" /></div>
              <div className="form-group"><label className="form-label">Giờ bắt đầu *</label><input className="form-control" type="time" /></div>
              <div className="form-group"><label className="form-label">Lời nhắn</label><textarea className="form-control" rows={3} placeholder="Nhắn gửi chủ nhà..." /></div>
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setShowBookingModal(false)}>Huỷ</button>
              <button className="btn btn-primary" onClick={() => { setShowBookingModal(false); setSelectedProp(null); }}>Đặt lịch</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
