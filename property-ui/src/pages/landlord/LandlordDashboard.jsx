import { landlordDashboard, properties, payments, maintenanceRequests, bookings } from '../../data/mockData';
import { formatMoney, formatDate, getMonthLabel, getStatusBadge } from '../../utils/helpers';
import { AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { Building2, TrendingUp, AlertCircle, Clock, CreditCard, Calendar, Wrench, CheckCircle } from 'lucide-react';

const chartData = landlordDashboard.revenueTrend.map(d => ({
  name: getMonthLabel(d.year, d.month),
  revenue: d.revenue / 1000000,
}));

const myProps = properties.filter(p => p.landlordId === 2);
const myPayments = payments.slice(0, 3);
const myMaintenance = maintenanceRequests.filter(m => m.propertyId === 2 || m.propertyId === 1).slice(0, 3);
const myBookings = bookings.filter(b => b.status === 'Pending' || b.status === 'Confirmed');

export default function LandlordDashboard() {
  return (
    <div>
      <div className="mb-20">
        <div className="page-title">Dashboard Chủ nhà</div>
        <div className="page-desc">Tổng quan quản lý bất động sản của bạn</div>
      </div>

      <div className="stat-grid">
        <div className="stat-card">
          <div className="stat-icon purple"><Building2 size={22} /></div>
          <div className="stat-info">
            <div className="stat-label">BDS của tôi</div>
            <div className="stat-value">{landlordDashboard.totalProperties}</div>
            <div className="stat-change">{myProps.filter(p => p.status === 'Rented').length} đang cho thuê</div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon green"><TrendingUp size={22} /></div>
          <div className="stat-info">
            <div className="stat-label">Tỷ lệ lấp đầy</div>
            <div className="stat-value">{landlordDashboard.occupancyRate}%</div>
            <div className="stat-change up">Tháng này</div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon blue"><CreditCard size={22} /></div>
          <div className="stat-info">
            <div className="stat-label">Thu tháng này</div>
            <div className="stat-value">{(landlordDashboard.monthlyRevenue / 1000000).toFixed(0)}M</div>
            <div className="stat-change up">↑ VND</div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon yellow"><Clock size={22} /></div>
          <div className="stat-info">
            <div className="stat-label">Chờ thanh toán</div>
            <div className="stat-value">{(landlordDashboard.pendingPayments / 1000000).toFixed(1)}M</div>
            <div className="stat-change down">VND</div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon red"><Wrench size={22} /></div>
          <div className="stat-info">
            <div className="stat-label">Bảo trì đang xử lý</div>
            <div className="stat-value">{landlordDashboard.activeMaintenanceRequests}</div>
            <div className="stat-change down">Cần chú ý</div>
          </div>
        </div>
      </div>

      <div className="grid-2 mb-24">
        <div className="card">
          <div className="card-header">
            <div className="card-title">Doanh thu 12 tháng (triệu VND)</div>
            <TrendingUp size={18} style={{ color: 'var(--accent-light)' }} />
          </div>
          <ResponsiveContainer width="100%" height={200}>
            <AreaChart data={chartData}>
              <defs>
                <linearGradient id="grad" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="5%" stopColor="#10b981" stopOpacity={0.3} />
                  <stop offset="95%" stopColor="#10b981" stopOpacity={0} />
                </linearGradient>
              </defs>
              <CartesianGrid strokeDasharray="3 3" stroke="var(--border)" />
              <XAxis dataKey="name" tick={{ fill: 'var(--text-muted)', fontSize: 11 }} tickLine={false} />
              <YAxis tick={{ fill: 'var(--text-muted)', fontSize: 11 }} tickLine={false} axisLine={false} />
              <Tooltip contentStyle={{ background: 'var(--bg-card)', border: '1px solid var(--border)', borderRadius: 8, color: 'var(--text-primary)' }} formatter={(v) => [`${v}M`, 'Doanh thu']} />
              <Area type="monotone" dataKey="revenue" stroke="#10b981" fill="url(#grad)" strokeWidth={2} />
            </AreaChart>
          </ResponsiveContainer>
        </div>

        <div className="card">
          <div className="card-header"><div className="card-title">Trạng thái BDS</div></div>
          <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
            {myProps.map(p => (
              <div key={p.id} style={{ display: 'flex', alignItems: 'center', gap: 12, padding: '12px', background: 'var(--bg-primary)', borderRadius: 8 }}>
                {p.images[0] ? <img src={p.images[0].imageUrl} alt="" style={{ width: 48, height: 48, borderRadius: 6, objectFit: 'cover' }} /> : <div style={{ width: 48, height: 48, borderRadius: 6, background: 'var(--border)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>🏠</div>}
                <div style={{ flex: 1, minWidth: 0 }}>
                  <div className="fw-600" style={{ fontSize: 13, whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}>{p.title}</div>
                  <div className="text-sm text-muted">{formatMoney(p.monthlyRent)}/tháng</div>
                </div>
                {getStatusBadge(p.status)}
              </div>
            ))}
          </div>
        </div>
      </div>

      <div className="grid-2 mb-24">
        <div className="card">
          <div className="card-header"><div className="card-title">Thanh toán gần đây</div></div>
          {myPayments.map(p => (
            <div key={p.id} className="info-row" style={{ alignItems: 'center' }}>
              <div style={{ flex: 1 }}>
                <div className="fw-600" style={{ fontSize: 13 }}>{p.description}</div>
                <div className="text-sm text-muted">{p.tenantName} • HĐ {p.leaseNumber}</div>
              </div>
              <div style={{ textAlign: 'right' }}>
                <div className="fw-700 text-green">{formatMoney(p.amount)}</div>
                {getStatusBadge(p.status)}
              </div>
            </div>
          ))}
        </div>

        <div className="card">
          <div className="card-header"><div className="card-title">Yêu cầu bảo trì</div><span className="badge badge-warning">Cần xem xét</span></div>
          {myMaintenance.map(m => (
            <div key={m.id} className="info-row" style={{ alignItems: 'center' }}>
              <div style={{ flex: 1 }}>
                <div className="fw-600" style={{ fontSize: 13 }}>{m.title}</div>
                <div className="text-sm text-muted">{m.propertyTitle} • {m.requesterName}</div>
              </div>
              <div style={{ textAlign: 'right' }}>
                {getStatusBadge(m.status)}
              </div>
            </div>
          ))}
        </div>
      </div>

      <div className="card">
        <div className="card-header"><div className="card-title">Lịch xem nhà sắp tới</div></div>
        {myBookings.length === 0 ? (
          <div className="empty-state"><div className="empty-icon">📅</div><p>Không có lịch xem nhà</p></div>
        ) : (
          <div className="table-container">
            <table>
              <thead><tr><th>BDS</th><th>Người hẹn</th><th>Ngày xem</th><th>Giờ</th><th>Trạng thái</th><th>Thao tác</th></tr></thead>
              <tbody>
                {myBookings.map(b => (
                  <tr key={b.id}>
                    <td><strong>{b.propertyTitle}</strong></td>
                    <td>{b.tenantName}</td>
                    <td>{formatDate(b.scheduledDate)}</td>
                    <td>{b.startTime} - {b.endTime}</td>
                    <td>{getStatusBadge(b.status)}</td>
                    <td>
                      {b.status === 'Pending' && <div style={{ display: 'flex', gap: 6 }}>
                        <button className="btn btn-success btn-sm">✓ Xác nhận</button>
                        <button className="btn btn-danger btn-sm">✗ Từ chối</button>
                      </div>}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
