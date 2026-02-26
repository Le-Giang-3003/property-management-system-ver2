import { tenantDashboard, properties } from '../../data/mockData';
import { formatMoney, formatDate, getStatusBadge } from '../../utils/helpers';
import { FileText, CreditCard, Wrench, Calendar, Home, AlertCircle, CheckCircle } from 'lucide-react';

export default function TenantDashboard() {
  const { activeLease, nextPayment, openMaintenanceRequests, upcomingBookings } = tenantDashboard;
  const availableProps = properties.filter(p => p.status === 'Available');

  return (
    <div>
      <div className="mb-20">
        <div className="page-title">Dashboard Người thuê</div>
        <div className="page-desc">Tổng quan tình trạng thuê nhà của bạn</div>
      </div>

      <div className="stat-grid">
        <div className="stat-card">
          <div className="stat-icon green"><FileText size={22}/></div>
          <div className="stat-info">
            <div className="stat-label">Hợp đồng hiện tại</div>
            <div className="stat-value" style={{ fontSize: 16 }}>{activeLease ? activeLease.leaseNumber : '—'}</div>
            <div className="stat-change">{activeLease ? `Hết hạn ${formatDate(activeLease.endDate)}` : 'Không có HĐ hiệu lực'}</div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon yellow"><CreditCard size={22}/></div>
          <div className="stat-info">
            <div className="stat-label">Thanh toán tiếp theo</div>
            <div className="stat-value" style={{ fontSize: 18 }}>{nextPayment ? formatMoney(nextPayment.amount) : '—'}</div>
            <div className="stat-change down">{nextPayment ? `Đến hạn ${formatDate(nextPayment.dueDate)}` : 'Không có'}</div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon red"><Wrench size={22}/></div>
          <div className="stat-info">
            <div className="stat-label">Bảo trì đang mở</div>
            <div className="stat-value">{openMaintenanceRequests}</div>
            <div className="stat-change">{openMaintenanceRequests > 0 ? 'Đang xử lý' : 'Tất cả ổn'}</div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon blue"><Calendar size={22}/></div>
          <div className="stat-info">
            <div className="stat-label">Lịch xem nhà</div>
            <div className="stat-value">{upcomingBookings.length}</div>
            <div className="stat-change">Sắp tới</div>
          </div>
        </div>
      </div>

      <div className="grid-2 mb-24">
        {/* Active Lease Card */}
        <div className="card">
          <div className="card-header"><div className="card-title">🏠 Hợp đồng đang thuê</div>{activeLease && getStatusBadge(activeLease.status)}</div>
          {activeLease ? (
            <div>
              <div style={{ padding: '16px', background: 'var(--bg-primary)', borderRadius: 8, marginBottom: 12 }}>
                <div className="fw-700" style={{ fontSize: 16, color: 'var(--accent-light)', marginBottom: 4 }}>{activeLease.propertyTitle}</div>
                <div className="text-muted text-sm">📍 {activeLease.propertyAddress}</div>
              </div>
              <div className="info-row"><span className="info-label">Mã hợp đồng</span><span className="info-value fw-600">{activeLease.leaseNumber}</span></div>
              <div className="info-row"><span className="info-label">Chủ nhà</span><span className="info-value">{activeLease.landlordName}</span></div>
              <div className="info-row"><span className="info-label">Tiền thuê/tháng</span><span className="info-value text-green fw-700">{formatMoney(activeLease.monthlyRent)}</span></div>
              <div className="info-row"><span className="info-label">Tiền đặt cọc</span><span className="info-value">{formatMoney(activeLease.depositAmount)}</span></div>
              <div className="info-row"><span className="info-label">Thời hạn</span><span className="info-value">{formatDate(activeLease.startDate)} → {formatDate(activeLease.endDate)}</span></div>

              {/* Progress bar */}
              <div style={{ marginTop: 16 }}>
                <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: 12, color: 'var(--text-muted)', marginBottom: 6 }}>
                  <span>{formatDate(activeLease.startDate)}</span>
                  <span>Thời gian còn lại</span>
                  <span>{formatDate(activeLease.endDate)}</span>
                </div>
                <div className="progress-bar">
                  <div className="progress-fill" style={{ width: '30%' }} />
                </div>
              </div>
            </div>
          ) : (
            <div className="empty-state"><div className="empty-icon">📄</div><p>Bạn chưa có hợp đồng nào đang hiệu lực</p><button className="btn btn-primary mt-16">Tìm kiếm BDS</button></div>
          )}
        </div>

        {/* Next Payment */}
        <div className="card">
          <div className="card-header"><div className="card-title">💳 Thanh toán sắp đến hạn</div></div>
          {nextPayment ? (
            <div>
              <div style={{ textAlign: 'center', padding: '20px', background: 'var(--bg-primary)', borderRadius: 8, marginBottom: 16 }}>
                <div className="text-muted text-sm mb-8">Số tiền cần thanh toán</div>
                <div style={{ fontSize: 32, fontWeight: 800, color: 'var(--warning)' }}>{formatMoney(nextPayment.amount)}</div>
                <div className="mt-8 text-muted">Đến hạn {formatDate(nextPayment.dueDate)}</div>
                <div className="mt-4">{getStatusBadge(nextPayment.status)}</div>
              </div>
              <div className="info-row"><span className="info-label">Mô tả</span><span className="info-value">{nextPayment.description}</span></div>
              <div style={{ marginTop: 16, display: 'flex', gap: 10 }}>
                <button className="btn btn-primary" style={{ flex: 1 }}>💳 Thanh toán ngay</button>
                <button className="btn btn-secondary">Xem lịch sử</button>
              </div>
            </div>
          ) : (
            <div className="empty-state"><div className="empty-icon">✅</div><p>Không có thanh toán nào sắp đến hạn</p></div>
          )}
        </div>
      </div>

      {upcomingBookings.length > 0 && (
        <div className="card mb-24">
          <div className="card-header"><div className="card-title">📅 Lịch xem nhà sắp tới</div></div>
          <div style={{ display: 'flex', gap: 12 }}>
            {upcomingBookings.map(b => (
              <div key={b.id} style={{ flex: 1, padding: 16, background: 'var(--bg-primary)', borderRadius: 8, border: '1px solid var(--border)' }}>
                <div className="fw-700" style={{ marginBottom: 6 }}>{b.propertyTitle}</div>
                <div className="text-muted text-sm">📅 {formatDate(b.scheduledDate)}</div>
                <div className="text-muted text-sm">⏰ {b.startTime} - {b.endTime}</div>
                <div className="mt-8">{getStatusBadge(b.status)}</div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Available Properties to Browse */}
      <div className="card">
        <div className="card-header">
          <div className="card-title">🏘️ BDS có thể thuê gần đây</div>
          <button className="btn btn-ghost btn-sm">Xem tất cả →</button>
        </div>
        <div className="property-grid">
          {availableProps.slice(0, 3).map(p => (
            <div key={p.id} className="property-card">
              {p.images[0] ? <img className="property-image" src={p.images[0].imageUrl} alt={p.title} /> : <div className="property-image-placeholder">🏠</div>}
              <div className="property-body">
                <div className="property-type-badge">{p.propertyType}</div>
                <div className="property-title">{p.title}</div>
                <div className="property-address">📍 {p.district}, {p.city}</div>
                <div className="property-specs">
                  <div className="spec-item">🛏 {p.bedrooms}</div>
                  <div className="spec-item">🚿 {p.bathrooms}</div>
                  <div className="spec-item">📐 {p.area}m²</div>
                </div>
                <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', borderTop: '1px solid var(--border)', paddingTop: 12 }}>
                  <div><div className="property-price">{formatMoney(p.monthlyRent)}</div><div className="property-price-sub">/tháng</div></div>
                  <button className="btn btn-primary btn-sm">Xem chi tiết</button>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}
