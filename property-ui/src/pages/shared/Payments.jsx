import { useState } from 'react';
import { payments } from '../../data/mockData';
import { formatMoney, formatDate, getStatusBadge, getPaymentTypeLabel } from '../../utils/helpers';
import { CreditCard, Eye, CheckCircle, XCircle } from 'lucide-react';

export default function Payments({ role = 'Landlord' }) {
  const [statusFilter, setStatusFilter] = useState('All');
  const [selectedPayment, setSelectedPayment] = useState(null);
  const [showConfirmModal, setShowConfirmModal] = useState(false);

  const myPayments = role === 'Tenant'
    ? payments.filter(p => p.tenantName === 'Phạm Thị Hoa')
    : role === 'Landlord'
    ? payments.filter(p => p.landlordName === 'Trần Thị Lan')
    : payments;

  const filtered = statusFilter === 'All' ? myPayments : myPayments.filter(p => p.status === statusFilter);

  const summary = {
    totalPaid: myPayments.filter(p=>p.status==='Completed').reduce((s,p)=>s+p.amount,0),
    totalPending: myPayments.filter(p=>p.status==='Pending').reduce((s,p)=>s+p.amount,0),
    totalOverdue: myPayments.filter(p=>p.status==='Overdue').reduce((s,p)=>s+p.amount,0),
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-20">
        <div>
          <div className="page-title">Thanh toán</div>
          <div className="page-desc">{role === 'Tenant' ? 'Theo dõi lịch sử và tình trạng thanh toán tiền thuê' : 'Quản lý các khoản thanh toán từ người thuê'}</div>
        </div>
      </div>

      <div className="stat-grid" style={{ gridTemplateColumns: 'repeat(3, 1fr)', marginBottom: 20 }}>
        <div className="stat-card">
          <div className="stat-icon green"><CreditCard size={20}/></div>
          <div className="stat-info"><div className="stat-label">Đã thanh toán</div><div className="stat-value" style={{ fontSize: 20 }}>{formatMoney(summary.totalPaid)}</div></div>
        </div>
        <div className="stat-card">
          <div className="stat-icon yellow"><CreditCard size={20}/></div>
          <div className="stat-info"><div className="stat-label">Chờ thanh toán</div><div className="stat-value" style={{ fontSize: 20 }}>{formatMoney(summary.totalPending)}</div></div>
        </div>
        <div className="stat-card">
          <div className="stat-icon red"><CreditCard size={20}/></div>
          <div className="stat-info"><div className="stat-label">Quá hạn</div><div className="stat-value" style={{ fontSize: 20 }}>{formatMoney(summary.totalOverdue)}</div></div>
        </div>
      </div>

      <div className="filter-bar">
        {['All', 'Completed', 'Pending', 'Overdue', 'Cancelled'].map(s => (
          <button key={s} className={`btn ${statusFilter === s ? 'btn-primary' : 'btn-ghost'} btn-sm`} onClick={() => setStatusFilter(s)}>
            {s === 'All' ? 'Tất cả' : s === 'Completed' ? '✅ Đã TT' : s === 'Pending' ? '⏳ Chờ TT' : s === 'Overdue' ? '🔴 Quá hạn' : '❌ Huỷ'}
          </button>
        ))}
      </div>

      <div className="card">
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Mã HĐ</th>
                <th>BDS</th>
                {role !== 'Tenant' && <th>Người thuê</th>}
                <th>Loại</th>
                <th>Số tiền</th>
                <th>Trạng thái</th>
                <th>Ngày đến hạn</th>
                <th>Ngày TT</th>
                <th>Phương thức</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map(p => (
                <tr key={p.id}>
                  <td className="text-muted">#{p.id}</td>
                  <td><strong style={{ color: 'var(--accent-light)' }}>{p.leaseNumber}</strong></td>
                  <td style={{ maxWidth: 160, fontSize: 12 }}>{p.propertyTitle}</td>
                  {role !== 'Tenant' && <td>{p.tenantName}</td>}
                  <td><span className="badge badge-gray">{getPaymentTypeLabel(p.paymentType)}</span></td>
                  <td className="fw-700 text-green">{formatMoney(p.amount)}</td>
                  <td>{getStatusBadge(p.status)}</td>
                  <td className="text-muted">{formatDate(p.dueDate)}</td>
                  <td className="text-muted">{formatDate(p.paidDate)}</td>
                  <td className="text-muted">{p.paymentMethod || '—'}</td>
                  <td>
                    <div style={{ display: 'flex', gap: 6 }}>
                      <button className="btn btn-ghost btn-sm btn-icon" onClick={() => setSelectedPayment(p)}><Eye size={13}/></button>
                      {role === 'Tenant' && p.status === 'Pending' && (
                        <button className="btn btn-primary btn-sm">Thanh toán</button>
                      )}
                      {role === 'Landlord' && p.status === 'Pending' && (
                        <button className="btn btn-success btn-sm btn-icon" title="Xác nhận nhận tiền"><CheckCircle size={13}/></button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <div style={{ padding: '12px 14px', borderTop: '1px solid var(--border)', fontSize: 12, color: 'var(--text-muted)' }}>
          Hiển thị {filtered.length} / {myPayments.length} thanh toán
        </div>
      </div>

      {selectedPayment && (
        <div className="modal-overlay" onClick={() => setSelectedPayment(null)}>
          <div className="modal" onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">Chi tiết thanh toán #{selectedPayment.id}</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setSelectedPayment(null)}>✕</button>
            </div>
            <div className="modal-body">
              <div style={{ textAlign: 'center', padding: 20, background: 'var(--bg-primary)', borderRadius: 8, marginBottom: 16 }}>
                <div className="fw-700" style={{ fontSize: 32, color: 'var(--success)' }}>{formatMoney(selectedPayment.amount)}</div>
                <div className="text-muted mt-4">{getPaymentTypeLabel(selectedPayment.paymentType)}</div>
                <div className="mt-8">{getStatusBadge(selectedPayment.status)}</div>
              </div>
              <div className="info-row"><span className="info-label">Mã hợp đồng</span><span className="info-value">{selectedPayment.leaseNumber}</span></div>
              <div className="info-row"><span className="info-label">BDS</span><span className="info-value">{selectedPayment.propertyTitle}</span></div>
              <div className="info-row"><span className="info-label">Người thuê</span><span className="info-value">{selectedPayment.tenantName}</span></div>
              <div className="info-row"><span className="info-label">Chủ nhà</span><span className="info-value">{selectedPayment.landlordName}</span></div>
              <div className="info-row"><span className="info-label">Ngày đến hạn</span><span className="info-value">{formatDate(selectedPayment.dueDate)}</span></div>
              <div className="info-row"><span className="info-label">Ngày thanh toán</span><span className="info-value">{formatDate(selectedPayment.paidDate)}</span></div>
              <div className="info-row"><span className="info-label">Phương thức TT</span><span className="info-value">{selectedPayment.paymentMethod || '—'}</span></div>
              <div className="info-row"><span className="info-label">Mã giao dịch</span><span className="info-value">{selectedPayment.transactionId || '—'}</span></div>
              {selectedPayment.lateFeeAmount && <div className="info-row"><span className="info-label">Phí trễ hạn</span><span className="info-value text-red">{formatMoney(selectedPayment.lateFeeAmount)}</span></div>}
              {selectedPayment.description && <div className="info-row"><span className="info-label">Ghi chú</span><span className="info-value">{selectedPayment.description}</span></div>}
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setSelectedPayment(null)}>Đóng</button>
              {role === 'Tenant' && selectedPayment.status === 'Pending' && (
                <button className="btn btn-primary">💳 Thanh toán ngay</button>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
