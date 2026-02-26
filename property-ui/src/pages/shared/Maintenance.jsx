import { useState } from 'react';
import { maintenanceRequests } from '../../data/mockData';
import { formatMoney, formatDate, getStatusBadge, getPriorityBadge, getCategoryLabel } from '../../utils/helpers';
import { Wrench, Plus, Eye, CheckCircle, Star } from 'lucide-react';

export default function Maintenance({ role = 'Landlord' }) {
  const [statusFilter, setStatusFilter] = useState('All');
  const [showModal, setShowModal] = useState(false);
  const [selectedReq, setSelectedReq] = useState(null);
  const [showCompleteModal, setShowCompleteModal] = useState(false);
  const [form, setForm] = useState({ title: '', description: '', category: 'Plumbing', priority: 'Medium' });
  const [completeForm, setCompleteForm] = useState({ resolution: '', actualCost: '' });

  const myRequests = role === 'Tenant'
    ? maintenanceRequests.filter(m => m.requestedBy === 4)
    : role === 'Landlord'
    ? maintenanceRequests.filter(m => m.propertyId === 1 || m.propertyId === 2)
    : maintenanceRequests;

  const filtered = statusFilter === 'All' ? myRequests : myRequests.filter(m => m.status === statusFilter);

  const summary = {
    open: myRequests.filter(m => m.status === 'Open').length,
    inProgress: myRequests.filter(m => m.status === 'InProgress').length,
    resolved: myRequests.filter(m => m.status === 'Resolved').length,
    cancelled: myRequests.filter(m => m.status === 'Cancelled').length,
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-20">
        <div>
          <div className="page-title">Yêu cầu bảo trì</div>
          <div className="page-desc">{role === 'Tenant' ? 'Gửi và theo dõi yêu cầu sửa chữa, bảo trì' : 'Quản lý yêu cầu bảo trì từ người thuê'}</div>
        </div>
        {role === 'Tenant' && <button className="btn btn-primary" onClick={() => setShowModal(true)}><Plus size={16}/> Gửi yêu cầu mới</button>}
      </div>

      <div className="stat-grid" style={{ gridTemplateColumns: 'repeat(4, 1fr)', marginBottom: 20 }}>
        <div className="stat-card"><div className="stat-icon yellow"><Wrench size={20}/></div><div className="stat-info"><div className="stat-label">Đang mở</div><div className="stat-value">{summary.open}</div></div></div>
        <div className="stat-card"><div className="stat-icon blue"><Wrench size={20}/></div><div className="stat-info"><div className="stat-label">Đang xử lý</div><div className="stat-value">{summary.inProgress}</div></div></div>
        <div className="stat-card"><div className="stat-icon green"><Wrench size={20}/></div><div className="stat-info"><div className="stat-label">Đã giải quyết</div><div className="stat-value">{summary.resolved}</div></div></div>
        <div className="stat-card"><div className="stat-icon red"><Wrench size={20}/></div><div className="stat-info"><div className="stat-label">Đã huỷ</div><div className="stat-value">{summary.cancelled}</div></div></div>
      </div>

      <div className="filter-bar">
        {['All', 'Open', 'InProgress', 'Resolved', 'Cancelled'].map(s => (
          <button key={s} className={`btn ${statusFilter === s ? 'btn-primary' : 'btn-ghost'} btn-sm`} onClick={() => setStatusFilter(s)}>
            {s === 'All' ? 'Tất cả' : s === 'Open' ? '🟡 Mở' : s === 'InProgress' ? '🔵 Đang xử lý' : s === 'Resolved' ? '🟢 Giải quyết' : '⭕ Huỷ'}
          </button>
        ))}
      </div>

      <div className="card">
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>Tiêu đề</th>
                <th>BDS</th>
                {role !== 'Tenant' && <th>Người yêu cầu</th>}
                <th>Danh mục</th>
                <th>Ưu tiên</th>
                <th>Trạng thái</th>
                <th>Chi phí dự tính</th>
                <th>Ngày</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map(m => (
                <tr key={m.id}>
                  <td><strong>{m.title}</strong></td>
                  <td className="text-muted" style={{ fontSize: 12 }}>{m.propertyTitle}</td>
                  {role !== 'Tenant' && <td>{m.requesterName}</td>}
                  <td><span className="badge badge-gray">{getCategoryLabel(m.category)}</span></td>
                  <td>{getPriorityBadge(m.priority)}</td>
                  <td>{getStatusBadge(m.status)}</td>
                  <td className="text-muted">{m.estimatedCost ? formatMoney(m.estimatedCost) : '—'}</td>
                  <td className="text-muted">{formatDate(m.createdAt)}</td>
                  <td>
                    <div style={{ display: 'flex', gap: 6 }}>
                      <button className="btn btn-ghost btn-sm btn-icon" onClick={() => setSelectedReq(m)}><Eye size={13}/></button>
                      {role === 'Landlord' && m.status === 'InProgress' && (
                        <button className="btn btn-success btn-sm" onClick={() => { setSelectedReq(m); setShowCompleteModal(true); }}><CheckCircle size={13}/> Hoàn thành</button>
                      )}
                      {role === 'Tenant' && m.status === 'Resolved' && !m.rating && (
                        <button className="btn btn-warning btn-sm"><Star size={13}/> Đánh giá</button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Create Request Modal (Tenant) */}
      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">Gửi yêu cầu bảo trì mới</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setShowModal(false)}>✕</button>
            </div>
            <div className="modal-body">
              <div className="form-group">
                <label className="form-label">Tiêu đề *</label>
                <input className="form-control" placeholder="Mô tả ngắn gọn sự cố..." value={form.title} onChange={e => setForm({...form, title: e.target.value})} />
              </div>
              <div className="form-row">
                <div className="form-group">
                  <label className="form-label">Danh mục</label>
                  <select className="form-control" value={form.category} onChange={e => setForm({...form, category: e.target.value})}>
                    {['Plumbing','Electrical','Painting','Appliance','Structural','Cleaning','Other'].map(c => <option key={c} value={c}>{getCategoryLabel(c)}</option>)}
                  </select>
                </div>
                <div className="form-group">
                  <label className="form-label">Mức độ ưu tiên</label>
                  <select className="form-control" value={form.priority} onChange={e => setForm({...form, priority: e.target.value})}>
                    {['Low','Medium','High','Critical'].map(p => <option key={p} value={p}>{p === 'Low' ? 'Thấp' : p === 'Medium' ? 'Trung bình' : p === 'High' ? 'Cao' : 'Khẩn cấp'}</option>)}
                  </select>
                </div>
              </div>
              <div className="form-group">
                <label className="form-label">Mô tả chi tiết *</label>
                <textarea className="form-control" rows={4} placeholder="Mô tả chi tiết sự cố, vị trí, thời gian phát hiện..." value={form.description} onChange={e => setForm({...form, description: e.target.value})} />
              </div>
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setShowModal(false)}>Huỷ</button>
              <button className="btn btn-primary" onClick={() => setShowModal(false)}>Gửi yêu cầu</button>
            </div>
          </div>
        </div>
      )}

      {/* Detail Modal */}
      {selectedReq && !showCompleteModal && (
        <div className="modal-overlay" onClick={() => setSelectedReq(null)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">#{selectedReq.id} - {selectedReq.title}</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setSelectedReq(null)}>✕</button>
            </div>
            <div className="modal-body">
              <div style={{ display: 'flex', gap: 8, marginBottom: 16 }}>
                {getStatusBadge(selectedReq.status)}
                {getPriorityBadge(selectedReq.priority)}
                <span className="badge badge-gray">{getCategoryLabel(selectedReq.category)}</span>
              </div>
              <div className="info-row"><span className="info-label">BDS</span><span className="info-value">{selectedReq.propertyTitle}</span></div>
              <div className="info-row"><span className="info-label">Người yêu cầu</span><span className="info-value">{selectedReq.requesterName}</span></div>
              <div className="info-row"><span className="info-label">Mô tả</span><span className="info-value" style={{ whiteSpace: 'pre-wrap' }}>{selectedReq.description}</span></div>
              <div className="info-row"><span className="info-label">Chi phí dự tính</span><span className="info-value">{selectedReq.estimatedCost ? formatMoney(selectedReq.estimatedCost) : '—'}</span></div>
              <div className="info-row"><span className="info-label">Chi phí thực tế</span><span className="info-value">{selectedReq.actualCost ? formatMoney(selectedReq.actualCost) : '—'}</span></div>
              {selectedReq.scheduledDate && <div className="info-row"><span className="info-label">Ngày hẹn</span><span className="info-value">{formatDate(selectedReq.scheduledDate)}</span></div>}
              {selectedReq.assignedToName && <div className="info-row"><span className="info-label">Người sửa</span><span className="info-value">{selectedReq.assignedToName}</span></div>}
              {selectedReq.resolution && <div className="info-row"><span className="info-label">Kết quả xử lý</span><span className="info-value">{selectedReq.resolution}</span></div>}
              {selectedReq.resolvedAt && <div className="info-row"><span className="info-label">Ngày giải quyết</span><span className="info-value">{formatDate(selectedReq.resolvedAt)}</span></div>}
              {selectedReq.rating && (
                <div className="info-row">
                  <span className="info-label">Đánh giá</span>
                  <span className="info-value">{'⭐'.repeat(selectedReq.rating)} ({selectedReq.rating}/5) — {selectedReq.feedback}</span>
                </div>
              )}
              <div className="info-row"><span className="info-label">Ngày tạo</span><span className="info-value">{formatDate(selectedReq.createdAt)}</span></div>
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setSelectedReq(null)}>Đóng</button>
            </div>
          </div>
        </div>
      )}

      {/* Complete Modal */}
      {showCompleteModal && selectedReq && (
        <div className="modal-overlay" onClick={() => setShowCompleteModal(false)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">Hoàn thành yêu cầu bảo trì</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setShowCompleteModal(false)}>✕</button>
            </div>
            <div className="modal-body">
              <div className="form-group">
                <label className="form-label">Kết quả xử lý *</label>
                <textarea className="form-control" rows={4} placeholder="Mô tả những gì đã được thực hiện..." value={completeForm.resolution} onChange={e => setCompleteForm({...completeForm, resolution: e.target.value})} />
              </div>
              <div className="form-group">
                <label className="form-label">Chi phí thực tế (VND)</label>
                <input className="form-control" type="number" placeholder="0" value={completeForm.actualCost} onChange={e => setCompleteForm({...completeForm, actualCost: e.target.value})} />
              </div>
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setShowCompleteModal(false)}>Huỷ</button>
              <button className="btn btn-success" onClick={() => { setShowCompleteModal(false); setSelectedReq(null); }}>✓ Xác nhận hoàn thành</button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
