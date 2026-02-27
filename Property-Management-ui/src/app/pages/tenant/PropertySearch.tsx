import { useState } from 'react';
import { properties } from '../../utils/mockData';
import { formatMoney, getStatusBadge, getPropertyTypeLabel } from '../../utils/helpers';
import { Search, Filter, Bed, Bath, Maximize2, Heart, MapPin, X, Eye, Calendar, Clock, Users, Briefcase, DollarSign, MessageSquare, Home } from 'lucide-react';
import PropertyDetailModal from '../../components/PropertyDetailModal';

export default function PropertySearch() {
  const [search, setSearch] = useState('');
  const [filters, setFilters] = useState({ type: 'All', minPrice: '', maxPrice: '', minBedrooms: '', city: '' });
  const [selectedProp, setSelectedProp] = useState(null);
  const [showApplyModal, setShowApplyModal] = useState(false);
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
      <div className="search-filter-card">
        {/* Search Row */}
        <div className="search-row">
          <div className="search-input-wrapper">
            <Search size={18} className="search-icon" />
            <input 
              className="search-input" 
              placeholder="Tìm kiếm tên, quận..." 
              value={search} 
              onChange={e => setSearch(e.target.value)} 
            />
          </div>
          <input 
            className="filter-input" 
            placeholder="Thành phố" 
            value={filters.city} 
            onChange={e => setFilters({...filters, city: e.target.value})} 
          />
        </div>

        {/* Filter Row */}
        <div className="filter-row">
          <select 
            className="filter-select" 
            value={filters.type} 
            onChange={e => setFilters({...filters, type: e.target.value})}
          >
            <option value="All">Loại BDS</option>
            {['Apartment','House','Room','Villa','Commercial'].map(t => 
              <option key={t} value={t}>{getPropertyTypeLabel(t)}</option>
            )}
          </select>

          <input 
            className="filter-input" 
            type="number" 
            placeholder="Giá từ (VND)" 
            value={filters.minPrice} 
            onChange={e => setFilters({...filters, minPrice: e.target.value})} 
          />

          <input 
            className="filter-input" 
            type="number" 
            placeholder="Giá đến (VND)" 
            value={filters.maxPrice} 
            onChange={e => setFilters({...filters, maxPrice: e.target.value})} 
          />

          <select 
            className="filter-select" 
            value={filters.minBedrooms} 
            onChange={e => setFilters({...filters, minBedrooms: e.target.value})}
          >
            <option value="">Số phòng ngủ</option>
            {[1,2,3,4,5].map(n => 
              <option key={n} value={n}>≥ {n} phòng ngủ</option>
            )}
          </select>

          <button 
            className="btn-clear-filter" 
            onClick={() => setFilters({ type: 'All', minPrice: '', maxPrice: '', minBedrooms: '', city: '' })}
          >
            <X size={14}/> Xoá lọc
          </button>
        </div>

        {/* Result Count */}
        <div className="filter-result">
          Tìm thấy {filtered.length} bất động sản
        </div>
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
                  {p.status === 'Available' && (
                    <>
                      <button 
                        className="btn btn-ghost btn-sm" 
                        onClick={() => setSelectedProp(p)}
                      >
                        <Eye size={14} /> Xem chi tiết
                      </button>
                      <button 
                        className="btn btn-primary btn-sm" 
                        onClick={() => { 
                          setSelectedProp(p); 
                          setShowApplyModal(true); 
                        }}
                      >
                        Nộp đơn
                      </button>
                    </>
                  )}
                  {p.status === 'Rented' && <button className="btn btn-secondary btn-sm" disabled>Đã thuê</button>}
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Detail Modal */}
      {selectedProp && !showApplyModal && (
        <PropertyDetailModal
          property={selectedProp}
          onClose={() => setSelectedProp(null)}
          showApplyButton={selectedProp.status === 'Available'}
          onApply={() => {
            setShowApplyModal(true);
          }}
        />
      )}

      {/* Apply Modal */}
      {showApplyModal && selectedProp && (
        <div className="modal-overlay" onClick={() => setShowApplyModal(false)}>
          <div className="modal" style={{ maxWidth: 620 }} onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">Nộp đơn xin thuê</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setShowApplyModal(false)}>✕</button>
            </div>
            <div className="modal-body">
              {/* Property Info Box */}
              <div className="apply-property-info">
                <div className="apply-property-icon">
                  <Home size={20} />
                </div>
                <div>
                  <div className="apply-property-title">{selectedProp.title}</div>
                  <div className="apply-property-price">{formatMoney(selectedProp.monthlyRent)}/tháng</div>
                </div>
              </div>

              {/* Form Fields with Icons */}
              <div className="form-row">
                <div className="form-group-with-icon">
                  <label className="form-label">
                    Ngày dọn vào <span className="text-red">*</span>
                  </label>
                  <div className="input-icon-wrapper">
                    <Calendar size={16} className="input-icon" />
                    <input 
                      className="form-control-with-icon" 
                      type="date" 
                      value={applyForm.moveInDate} 
                      onChange={e => setApplyForm({...applyForm, moveInDate: e.target.value})} 
                    />
                  </div>
                </div>
                <div className="form-group-with-icon">
                  <label className="form-label">
                    Thời hạn thuê (tháng) <span className="text-red">*</span>
                  </label>
                  <div className="input-icon-wrapper">
                    <Clock size={16} className="input-icon" />
                    <input 
                      className="form-control-with-icon" 
                      type="number" 
                      min={1} 
                      placeholder="12"
                      value={applyForm.leaseDuration} 
                      onChange={e => setApplyForm({...applyForm, leaseDuration: e.target.value})} 
                    />
                  </div>
                </div>
              </div>

              <div className="form-row">
                <div className="form-group-with-icon">
                  <label className="form-label">
                    Số người ở <span className="text-red">*</span>
                  </label>
                  <div className="input-icon-wrapper">
                    <Users size={16} className="input-icon" />
                    <input 
                      className="form-control-with-icon" 
                      type="number" 
                      min={1} 
                      placeholder="1"
                      value={applyForm.occupants} 
                      onChange={e => setApplyForm({...applyForm, occupants: e.target.value})} 
                    />
                  </div>
                </div>
                <div className="form-group-with-icon">
                  <label className="form-label">Nghề nghiệp</label>
                  <div className="input-icon-wrapper">
                    <Briefcase size={16} className="input-icon" />
                    <input 
                      className="form-control-with-icon" 
                      placeholder="VD: Kỹ sư, Giáo viên..." 
                      value={applyForm.occupation} 
                      onChange={e => setApplyForm({...applyForm, occupation: e.target.value})} 
                    />
                  </div>
                </div>
              </div>

              <div className="form-group-with-icon">
                <label className="form-label">Thu nhập hàng tháng (VND)</label>
                <div className="input-icon-wrapper">
                  <DollarSign size={16} className="input-icon" />
                  <input 
                    className="form-control-with-icon" 
                    type="number" 
                    placeholder="15000000" 
                    value={applyForm.income} 
                    onChange={e => setApplyForm({...applyForm, income: e.target.value})} 
                  />
                </div>
              </div>

              <div className="form-group-with-icon">
                <label className="form-label">Lời nhắn cho chủ nhà</label>
                <div className="input-icon-wrapper">
                  <MessageSquare size={16} className="input-icon textarea-icon" />
                  <textarea 
                    className="form-control-with-icon" 
                    rows={3} 
                    placeholder="Giới thiệu bản thân, lý do thuê..." 
                    value={applyForm.message} 
                    onChange={e => setApplyForm({...applyForm, message: e.target.value})} 
                  />
                </div>
              </div>
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setShowApplyModal(false)}>Huỷ</button>
              <button className="btn btn-primary" onClick={() => { setShowApplyModal(false); setSelectedProp(null); }}>Gửi đơn xin thuê</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}