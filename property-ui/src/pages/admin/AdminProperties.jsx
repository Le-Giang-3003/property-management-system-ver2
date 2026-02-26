import { useState } from 'react';
import { properties } from '../../data/mockData';
import { formatMoney, formatDate, getStatusBadge, getPropertyTypeLabel } from '../../utils/helpers';
import { Search, Plus, Eye, CheckCircle, XCircle, Bed, Bath, Maximize2 } from 'lucide-react';

export default function AdminProperties() {
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState('All');
  const [selectedProp, setSelectedProp] = useState(null);
  const [showRejectModal, setShowRejectModal] = useState(false);
  const [rejectReason, setRejectReason] = useState('');

  const filtered = properties.filter(p => {
    const matchSearch = p.title.toLowerCase().includes(search.toLowerCase()) || p.city.toLowerCase().includes(search.toLowerCase());
    const matchStatus = statusFilter === 'All' || p.status === statusFilter;
    return matchSearch && matchStatus;
  });

  return (
    <div>
      <div className="flex items-center justify-between mb-20">
        <div>
          <div className="page-title">Quản lý Bất động sản</div>
          <div className="page-desc">Duyệt và quản lý tất cả bất động sản trong hệ thống</div>
        </div>
      </div>

      <div className="filter-bar">
        <div className="header-search" style={{ flex: 1, maxWidth: 320 }}>
          <Search size={14} style={{ color: 'var(--text-muted)' }} />
          <input placeholder="Tìm kiếm BDS..." value={search} onChange={e => setSearch(e.target.value)} />
        </div>
        {['All', 'Available', 'Rented', 'Pending', 'Draft', 'Rejected'].map(s => (
          <button key={s} className={`btn ${statusFilter === s ? 'btn-primary' : 'btn-ghost'} btn-sm`} onClick={() => setStatusFilter(s)}>
            {s === 'All' ? 'Tất cả' : getStatusBadge(s)}
          </button>
        ))}
      </div>

      <div className="card">
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Tên BDS</th>
                <th>Loại</th>
                <th>Địa chỉ</th>
                <th>Chủ nhà</th>
                <th>Trạng thái</th>
                <th>Tiền thuê</th>
                <th>Ngày tạo</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map(p => (
                <tr key={p.id}>
                  <td><span className="text-muted">#{p.id}</span></td>
                  <td>
                    <div style={{ display: 'flex', alignItems: 'center', gap: 10 }}>
                      {p.images[0] ? (
                        <img src={p.images[0].imageUrl} alt="" style={{ width: 40, height: 40, borderRadius: 6, objectFit: 'cover', flexShrink: 0 }} />
                      ) : (
                        <div style={{ width: 40, height: 40, borderRadius: 6, background: 'var(--bg-input)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>🏠</div>
                      )}
                      <div>
                        <strong>{p.title}</strong>
                        <div className="text-sm text-muted">{p.area}m² • {p.bedrooms}PN • {p.bathrooms}WC</div>
                      </div>
                    </div>
                  </td>
                  <td><span className="badge badge-purple">{getPropertyTypeLabel(p.propertyType)}</span></td>
                  <td className="text-muted">{p.district}, {p.city}</td>
                  <td>{p.landlord.fullName}</td>
                  <td>{getStatusBadge(p.status)}</td>
                  <td className="text-green fw-600">{formatMoney(p.monthlyRent)}</td>
                  <td className="text-muted">{formatDate(p.createdAt)}</td>
                  <td>
                    <div style={{ display: 'flex', gap: 6 }}>
                      <button className="btn btn-ghost btn-sm btn-icon" onClick={() => setSelectedProp(p)} title="Xem chi tiết"><Eye size={14} /></button>
                      {p.status === 'Pending' && <>
                        <button className="btn btn-success btn-sm btn-icon" title="Duyệt"><CheckCircle size={14} /></button>
                        <button className="btn btn-danger btn-sm btn-icon" title="Từ chối" onClick={() => { setSelectedProp(p); setShowRejectModal(true); }}><XCircle size={14} /></button>
                      </>}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <div style={{ padding: '12px 14px', borderTop: '1px solid var(--border)', fontSize: 12, color: 'var(--text-muted)' }}>
          Hiển thị {filtered.length} / {properties.length} bất động sản
        </div>
      </div>

      {/* Detail Modal */}
      {selectedProp && !showRejectModal && (
        <div className="modal-overlay" onClick={() => setSelectedProp(null)}>
          <div className="modal" style={{ maxWidth: 680 }} onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">{selectedProp.title}</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setSelectedProp(null)}>✕</button>
            </div>
            <div className="modal-body">
              {selectedProp.images[0] && (
                <img src={selectedProp.images[0].imageUrl} alt="" style={{ width: '100%', height: 220, objectFit: 'cover', borderRadius: 8, marginBottom: 16 }} />
              )}
              <div className="grid-2">
                <div>
                  <div className="info-row"><span className="info-label">Loại BDS</span><span className="info-value">{getPropertyTypeLabel(selectedProp.propertyType)}</span></div>
                  <div className="info-row"><span className="info-label">Trạng thái</span><span className="info-value">{getStatusBadge(selectedProp.status)}</span></div>
                  <div className="info-row"><span className="info-label">Diện tích</span><span className="info-value">{selectedProp.area} m²</span></div>
                  <div className="info-row"><span className="info-label">Phòng ngủ</span><span className="info-value">{selectedProp.bedrooms}</span></div>
                  <div className="info-row"><span className="info-label">Phòng tắm</span><span className="info-value">{selectedProp.bathrooms}</span></div>
                  <div className="info-row"><span className="info-label">Tầng</span><span className="info-value">{selectedProp.floors || '—'}</span></div>
                </div>
                <div>
                  <div className="info-row"><span className="info-label">Địa chỉ</span><span className="info-value">{selectedProp.address}, {selectedProp.ward || ''}, {selectedProp.district}, {selectedProp.city}</span></div>
                  <div className="info-row"><span className="info-label">Tiền thuê/tháng</span><span className="info-value text-green fw-700">{formatMoney(selectedProp.monthlyRent)}</span></div>
                  <div className="info-row"><span className="info-label">Tiền đặt cọc</span><span className="info-value">{formatMoney(selectedProp.depositAmount)}</span></div>
                  <div className="info-row"><span className="info-label">Chủ nhà</span><span className="info-value">{selectedProp.landlord.fullName}</span></div>
                  <div className="info-row"><span className="info-label">SĐT chủ nhà</span><span className="info-value">{selectedProp.landlord.phoneNumber}</span></div>
                  <div className="info-row"><span className="info-label">Thú cưng</span><span className="info-value">{selectedProp.allowPets ? '✅ Cho phép' : '❌ Không'}</span></div>
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
            </div>
            {selectedProp.status === 'Pending' && (
              <div className="modal-footer">
                <button className="btn btn-success" onClick={() => setSelectedProp(null)}>✓ Duyệt</button>
                <button className="btn btn-danger" onClick={() => setShowRejectModal(true)}>✗ Từ chối</button>
              </div>
            )}
          </div>
        </div>
      )}

      {showRejectModal && (
        <div className="modal-overlay" onClick={() => setShowRejectModal(false)}>
          <div className="modal" style={{ maxWidth: 440 }} onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">Từ chối BDS</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setShowRejectModal(false)}>✕</button>
            </div>
            <div className="modal-body">
              <p style={{ marginBottom: 12, fontSize: 13, color: 'var(--text-secondary)' }}>Vui lòng nhập lý do từ chối cho <strong>{selectedProp?.title}</strong>:</p>
              <div className="form-group">
                <label className="form-label">Lý do từ chối</label>
                <textarea className="form-control" rows={4} placeholder="Nhập lý do..." value={rejectReason} onChange={e => setRejectReason(e.target.value)} />
              </div>
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setShowRejectModal(false)}>Huỷ</button>
              <button className="btn btn-danger" onClick={() => { setShowRejectModal(false); setSelectedProp(null); setRejectReason(''); }}>Xác nhận từ chối</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
