import { useState } from 'react';
import { rentalApplications } from '../../data/mockData';
import { formatMoney, formatDate, getStatusBadge } from '../../utils/helpers';
import { ClipboardList, Eye, CheckCircle, XCircle } from 'lucide-react';

export default function Applications({ role = 'Landlord' }) {
  const [selectedApp, setSelectedApp] = useState(null);
  const [showRejectModal, setShowRejectModal] = useState(false);
  const [rejectReason, setRejectReason] = useState('');

  const myApps = role === 'Tenant'
    ? rentalApplications.filter(a => a.tenantId === 4)
    : rentalApplications;

  return (
    <div>
      <div className="flex items-center justify-between mb-20">
        <div>
          <div className="page-title">Đơn xin thuê</div>
          <div className="page-desc">{role === 'Tenant' ? 'Các đơn xin thuê bạn đã nộp' : 'Đơn xin thuê đang chờ xem xét từ người thuê'}</div>
        </div>
      </div>

      <div className="stat-grid" style={{ gridTemplateColumns: 'repeat(3, 1fr)', marginBottom: 20 }}>
        <div className="stat-card"><div className="stat-icon yellow"><ClipboardList size={20}/></div><div className="stat-info"><div className="stat-label">Chờ xem xét</div><div className="stat-value">{myApps.filter(a=>a.status==='Pending').length}</div></div></div>
        <div className="stat-card"><div className="stat-icon green"><ClipboardList size={20}/></div><div className="stat-info"><div className="stat-label">Đã duyệt</div><div className="stat-value">{myApps.filter(a=>a.status==='Approved').length}</div></div></div>
        <div className="stat-card"><div className="stat-icon red"><ClipboardList size={20}/></div><div className="stat-info"><div className="stat-label">Từ chối</div><div className="stat-value">{myApps.filter(a=>a.status==='Rejected').length}</div></div></div>
      </div>

      <div className="card">
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>BDS</th>
                {role !== 'Tenant' && <th>Người nộp</th>}
                {role !== 'Tenant' && <th>Liên hệ</th>}
                <th>Ngày dọn vào</th>
                <th>Thời hạn thuê</th>
                <th>Số người</th>
                <th>Thu nhập/tháng</th>
                <th>Nghề nghiệp</th>
                <th>Trạng thái</th>
                <th>Ngày nộp</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {myApps.map(a => (
                <tr key={a.id}>
                  <td>
                    <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                      {a.propertyThumbnail && <img src={a.propertyThumbnail} alt="" style={{ width: 36, height: 36, borderRadius: 4, objectFit: 'cover' }} />}
                      <div>
                        <strong style={{ fontSize: 12 }}>{a.propertyTitle}</strong>
                      </div>
                    </div>
                  </td>
                  {role !== 'Tenant' && <td><strong>{a.tenantName}</strong></td>}
                  {role !== 'Tenant' && <td className="text-muted text-sm">{a.tenantEmail}<br/>{a.tenantPhone}</td>}
                  <td className="text-muted">{formatDate(a.moveInDate)}</td>
                  <td>{a.leaseDurationMonths} tháng</td>
                  <td>{a.numberOfOccupants} người</td>
                  <td className="text-green">{a.monthlyIncome ? formatMoney(a.monthlyIncome) : '—'}</td>
                  <td className="text-muted">{a.occupation || '—'}</td>
                  <td>{getStatusBadge(a.status)}</td>
                  <td className="text-muted">{formatDate(a.createdAt)}</td>
                  <td>
                    <div style={{ display: 'flex', gap: 6 }}>
                      <button className="btn btn-ghost btn-sm btn-icon" onClick={() => setSelectedApp(a)}><Eye size={13}/></button>
                      {role === 'Landlord' && a.status === 'Pending' && <>
                        <button className="btn btn-success btn-sm btn-icon" title="Duyệt"><CheckCircle size={13}/></button>
                        <button className="btn btn-danger btn-sm btn-icon" title="Từ chối" onClick={() => { setSelectedApp(a); setShowRejectModal(true); }}><XCircle size={13}/></button>
                      </>}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {selectedApp && !showRejectModal && (
        <div className="modal-overlay" onClick={() => setSelectedApp(null)}>
          <div className="modal" style={{ maxWidth: 660 }} onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">Đơn xin thuê #{selectedApp.id}</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setSelectedApp(null)}>✕</button>
            </div>
            <div className="modal-body">
              <div style={{ padding: 16, background: 'var(--bg-primary)', borderRadius: 8, marginBottom: 16 }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                  {selectedApp.propertyThumbnail && <img src={selectedApp.propertyThumbnail} alt="" style={{ width: 60, height: 60, borderRadius: 8, objectFit: 'cover' }} />}
                  <div>
                    <div className="fw-700" style={{ fontSize: 15 }}>{selectedApp.propertyTitle}</div>
                    <div className="text-muted text-sm">Người nộp: {selectedApp.tenantName} • {formatDate(selectedApp.createdAt)}</div>
                    <div className="mt-4">{getStatusBadge(selectedApp.status)}</div>
                  </div>
                </div>
              </div>
              <div className="grid-2">
                <div>
                  <div className="info-row"><span className="info-label">Email</span><span className="info-value">{selectedApp.tenantEmail}</span></div>
                  <div className="info-row"><span className="info-label">Điện thoại</span><span className="info-value">{selectedApp.tenantPhone}</span></div>
                  <div className="info-row"><span className="info-label">Nghề nghiệp</span><span className="info-value">{selectedApp.occupation || '—'}</span></div>
                  <div className="info-row"><span className="info-label">Nơi làm việc</span><span className="info-value">{selectedApp.employerName || '—'}</span></div>
                </div>
                <div>
                  <div className="info-row"><span className="info-label">Thu nhập/tháng</span><span className="info-value text-green fw-700">{selectedApp.monthlyIncome ? formatMoney(selectedApp.monthlyIncome) : '—'}</span></div>
                  <div className="info-row"><span className="info-label">Ngày dọn vào</span><span className="info-value">{formatDate(selectedApp.moveInDate)}</span></div>
                  <div className="info-row"><span className="info-label">Thời hạn thuê</span><span className="info-value">{selectedApp.leaseDurationMonths} tháng</span></div>
                  <div className="info-row"><span className="info-label">Số người ở</span><span className="info-value">{selectedApp.numberOfOccupants} người</span></div>
                </div>
              </div>
              {selectedApp.message && <div className="info-row"><span className="info-label">Lời nhắn</span><span className="info-value" style={{ whiteSpace: 'pre-wrap' }}>{selectedApp.message}</span></div>}
              {selectedApp.rejectionReason && (
                <div style={{ background: 'var(--danger-bg)', border: '1px solid var(--danger)', borderRadius: 8, padding: 12, marginTop: 12 }}>
                  <div className="fw-600 text-red mb-4">Lý do từ chối:</div>
                  <div className="text-sm">{selectedApp.rejectionReason}</div>
                </div>
              )}
              {selectedApp.reviewedAt && <div className="info-row mt-8"><span className="info-label">Ngày xem xét</span><span className="info-value">{formatDate(selectedApp.reviewedAt)}</span></div>}
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setSelectedApp(null)}>Đóng</button>
              {role === 'Landlord' && selectedApp.status === 'Pending' && <>
                <button className="btn btn-success">✓ Duyệt & Tạo HĐ</button>
                <button className="btn btn-danger" onClick={() => setShowRejectModal(true)}>✗ Từ chối</button>
              </>}
            </div>
          </div>
        </div>
      )}

      {showRejectModal && selectedApp && (
        <div className="modal-overlay" onClick={() => setShowRejectModal(false)}>
          <div className="modal" style={{ maxWidth: 440 }} onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">Từ chối đơn xin thuê</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setShowRejectModal(false)}>✕</button>
            </div>
            <div className="modal-body">
              <div className="form-group">
                <label className="form-label">Lý do từ chối *</label>
                <textarea className="form-control" rows={4} placeholder="Nhập lý do từ chối..." value={rejectReason} onChange={e => setRejectReason(e.target.value)} />
              </div>
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setShowRejectModal(false)}>Huỷ</button>
              <button className="btn btn-danger" onClick={() => { setShowRejectModal(false); setSelectedApp(null); setRejectReason(''); }}>Xác nhận từ chối</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
