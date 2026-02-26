import { useState } from 'react';
import { leases } from '../../data/mockData';
import { formatMoney, formatDate, getStatusBadge } from '../../utils/helpers';
import { FileText, Eye, Plus } from 'lucide-react';

export default function Leases({ role = 'Admin' }) {
  const [selectedLease, setSelectedLease] = useState(null);
  const [statusFilter, setStatusFilter] = useState('All');
  const [showCreateModal, setShowCreateModal] = useState(false);

  const myLeases = role === 'Tenant'
    ? leases.filter(l => l.tenantId === 4)
    : role === 'Landlord'
    ? leases.filter(l => l.landlordId === 2)
    : leases;

  const filtered = statusFilter === 'All' ? myLeases : myLeases.filter(l => l.status === statusFilter);

  return (
    <div>
      <div className="flex items-center justify-between mb-20">
        <div>
          <div className="page-title">Hợp đồng thuê</div>
          <div className="page-desc">{role === 'Admin' ? 'Tất cả hợp đồng trong hệ thống' : role === 'Landlord' ? 'Các hợp đồng của BDS bạn cho thuê' : 'Các hợp đồng thuê của bạn'}</div>
        </div>
        {role === 'Landlord' && <button className="btn btn-primary" onClick={() => setShowCreateModal(true)}><Plus size={16}/> Tạo hợp đồng</button>}
      </div>

      <div className="stat-grid" style={{ gridTemplateColumns: 'repeat(4, 1fr)', marginBottom: 20 }}>
        {['Active', 'Pending', 'Expired', 'Terminated'].map((s, i) => (
          <div key={s} className="stat-card">
            <div className={`stat-icon ${['green','yellow','blue','red'][i]}`}><FileText size={20}/></div>
            <div className="stat-info">
              <div className="stat-label">{s === 'Active' ? 'Hiệu lực' : s === 'Pending' ? 'Chờ ký' : s === 'Expired' ? 'Hết hạn' : 'Chấm dứt'}</div>
              <div className="stat-value">{myLeases.filter(l => l.status === s).length}</div>
            </div>
          </div>
        ))}
      </div>

      <div className="filter-bar">
        {['All','Active','Pending','Expired','Terminated'].map(s => (
          <button key={s} className={`btn ${statusFilter === s ? 'btn-primary' : 'btn-ghost'} btn-sm`} onClick={() => setStatusFilter(s)}>
            {s === 'All' ? 'Tất cả' : s === 'Active' ? '🟢 Hiệu lực' : s === 'Pending' ? '🟡 Chờ ký' : s === 'Expired' ? '⚫ Hết hạn' : '🔴 Chấm dứt'}
          </button>
        ))}
      </div>

      <div className="card">
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>Mã HĐ</th>
                <th>BDS</th>
                {role !== 'Tenant' && <th>Người thuê</th>}
                {role !== 'Landlord' && <th>Chủ nhà</th>}
                <th>Trạng thái</th>
                <th>Chữ ký</th>
                <th>Tiền thuê</th>
                <th>Thời hạn</th>
                <th>Xem</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map(l => (
                <tr key={l.id}>
                  <td><strong style={{ color: 'var(--accent-light)' }}>{l.leaseNumber}</strong></td>
                  <td style={{ maxWidth: 160, fontSize: 12 }}>{l.propertyTitle}</td>
                  {role !== 'Tenant' && <td>{l.tenantName}</td>}
                  {role !== 'Landlord' && <td>{l.landlordName}</td>}
                  <td>{getStatusBadge(l.status)}</td>
                  <td>
                    <div style={{ fontSize: 11 }}>
                      {l.landlordSigned && l.tenantSigned ? '✅ Cả hai' : !l.landlordSigned ? '⏳ Chủ nhà' : '⏳ Người thuê'}
                    </div>
                  </td>
                  <td className="text-green fw-600">{formatMoney(l.monthlyRent)}</td>
                  <td className="text-muted" style={{ fontSize: 12 }}>{formatDate(l.startDate)}<br/>→ {formatDate(l.endDate)}</td>
                  <td><button className="btn btn-ghost btn-sm btn-icon" onClick={() => setSelectedLease(l)}><Eye size={13}/></button></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {selectedLease && (
        <div className="modal-overlay" onClick={() => setSelectedLease(null)}>
          <div className="modal" style={{ maxWidth: 680 }} onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">{selectedLease.leaseNumber}</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setSelectedLease(null)}>✕</button>
            </div>
            <div className="modal-body">
              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 16, padding: 16, background: 'var(--bg-primary)', borderRadius: 8 }}>
                <div>
                  <div className="text-muted text-sm">Mã hợp đồng</div>
                  <div className="fw-700" style={{ fontSize: 20, color: 'var(--accent-light)' }}>{selectedLease.leaseNumber}</div>
                </div>
                {getStatusBadge(selectedLease.status)}
              </div>
              <div className="grid-2">
                <div>
                  <div className="info-row"><span className="info-label">BDS</span><span className="info-value">{selectedLease.propertyTitle}</span></div>
                  <div className="info-row"><span className="info-label">Chủ nhà</span><span className="info-value">{selectedLease.landlordName}</span></div>
                  <div className="info-row"><span className="info-label">Người thuê</span><span className="info-value">{selectedLease.tenantName}</span></div>
                  <div className="info-row"><span className="info-label">Tiền thuê</span><span className="info-value text-green fw-700">{formatMoney(selectedLease.monthlyRent)}</span></div>
                  <div className="info-row"><span className="info-label">Đặt cọc</span><span className="info-value">{formatMoney(selectedLease.depositAmount)}</span></div>
                </div>
                <div>
                  <div className="info-row"><span className="info-label">Ngày bắt đầu</span><span className="info-value">{formatDate(selectedLease.startDate)}</span></div>
                  <div className="info-row"><span className="info-label">Ngày kết thúc</span><span className="info-value">{formatDate(selectedLease.endDate)}</span></div>
                  <div className="info-row"><span className="info-label">Ngày thanh toán</span><span className="info-value">Ngày {selectedLease.paymentDueDay}/tháng</span></div>
                  <div className="info-row"><span className="info-label">Phí trễ hạn</span><span className="info-value">{selectedLease.lateFeePercentage}%/tháng</span></div>
                </div>
              </div>
              {selectedLease.terms && <div className="info-row"><span className="info-label">Điều khoản</span><span className="info-value text-sm">{selectedLease.terms}</span></div>}
              {selectedLease.specialConditions && <div className="info-row"><span className="info-label">Điều khoản đặc biệt</span><span className="info-value text-sm">{selectedLease.specialConditions}</span></div>}
              <div style={{ padding: 14, background: 'var(--bg-primary)', borderRadius: 8, marginTop: 16 }}>
                <div className="fw-600 mb-8 text-sm">Tình trạng chữ ký</div>
                <div style={{ display: 'flex', gap: 10 }}>
                  <div style={{ flex: 1, padding: 12, border: `1px solid ${selectedLease.landlordSigned ? 'var(--success)' : 'var(--border)'}`, borderRadius: 8, textAlign: 'center' }}>
                    <div>{selectedLease.landlordSigned ? '✅' : '⏳'}</div>
                    <div className="text-sm fw-600 mt-4">Chủ nhà</div>
                    <div className="text-muted" style={{ fontSize: 11 }}>{selectedLease.landlordSigned ? formatDate(selectedLease.landlordSignedAt) : 'Chưa ký'}</div>
                  </div>
                  <div style={{ flex: 1, padding: 12, border: `1px solid ${selectedLease.tenantSigned ? 'var(--success)' : 'var(--border)'}`, borderRadius: 8, textAlign: 'center' }}>
                    <div>{selectedLease.tenantSigned ? '✅' : '⏳'}</div>
                    <div className="text-sm fw-600 mt-4">Người thuê</div>
                    <div className="text-muted" style={{ fontSize: 11 }}>{selectedLease.tenantSigned ? formatDate(selectedLease.tenantSignedAt) : 'Chưa ký'}</div>
                  </div>
                </div>
              </div>
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setSelectedLease(null)}>Đóng</button>
              {role === 'Tenant' && !selectedLease.tenantSigned && <button className="btn btn-primary">✍️ Ký hợp đồng</button>}
              {role === 'Landlord' && selectedLease.status === 'Active' && <button className="btn btn-danger">Chấm dứt HĐ</button>}
              {role === 'Landlord' && selectedLease.status === 'Expired' && <button className="btn btn-primary">Gia hạn HĐ</button>}
            </div>
          </div>
        </div>
      )}

      {showCreateModal && (
        <div className="modal-overlay" onClick={() => setShowCreateModal(false)}>
          <div className="modal" style={{ maxWidth: 580 }} onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">Tạo hợp đồng mới</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setShowCreateModal(false)}>✕</button>
            </div>
            <div className="modal-body">
              <div className="form-group"><label className="form-label">BDS *</label><select className="form-control"><option>Chọn BDS đang có người xin thuê...</option></select></div>
              <div className="form-group"><label className="form-label">Người thuê *</label><select className="form-control"><option>Chọn từ đơn xin thuê đã duyệt...</option></select></div>
              <div className="form-row">
                <div className="form-group"><label className="form-label">Ngày bắt đầu *</label><input className="form-control" type="date" /></div>
                <div className="form-group"><label className="form-label">Ngày kết thúc *</label><input className="form-control" type="date" /></div>
              </div>
              <div className="form-row">
                <div className="form-group"><label className="form-label">Tiền thuê (VND) *</label><input className="form-control" type="number" /></div>
                <div className="form-group"><label className="form-label">Tiền đặt cọc (VND) *</label><input className="form-control" type="number" /></div>
              </div>
              <div className="form-group"><label className="form-label">Điều khoản hợp đồng</label><textarea className="form-control" rows={4} placeholder="Nhập điều khoản..." /></div>
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setShowCreateModal(false)}>Huỷ</button>
              <button className="btn btn-primary" onClick={() => setShowCreateModal(false)}>Tạo hợp đồng</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
