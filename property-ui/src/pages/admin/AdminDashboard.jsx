import { adminDashboard, properties, users, leases, payments, maintenanceRequests, auditLogs } from '../../data/mockData';
import { formatMoney, formatDate, formatDateTime, getMonthLabel, getStatusBadge } from '../../utils/helpers';
import { AreaChart, Area, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { Building2, Users, FileText, TrendingUp, AlertCircle, CheckCircle, Clock, DollarSign } from 'lucide-react';

const chartData = adminDashboard.revenueTrend.map(d => ({
  name: getMonthLabel(d.year, d.month),
  revenue: d.revenue / 1000000,
}));

export default function AdminDashboard() {
  const pendingProps = properties.filter(p => p.status === 'Pending');
  const activeLeases = leases.filter(l => l.status === 'Active');
  const recentUsers = [...users].sort((a, b) => new Date(b.createdAt) - new Date(a.createdAt)).slice(0, 5);

  return (
    <div>
      <div className="mb-20">
        <div className="page-title">Dashboard Quản trị</div>
        <div className="page-desc">Tổng quan hệ thống quản lý bất động sản</div>
      </div>

      <div className="stat-grid">
        <div className="stat-card">
          <div className="stat-icon purple"><Users size={22} /></div>
          <div className="stat-info">
            <div className="stat-label">Tổng Users</div>
            <div className="stat-value">{adminDashboard.totalUsers.toLocaleString()}</div>
            <div className="stat-change up">↑ +12 tháng này</div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon blue"><Building2 size={22} /></div>
          <div className="stat-info">
            <div className="stat-label">Tổng BDS</div>
            <div className="stat-value">{adminDashboard.totalProperties}</div>
            <div className="stat-change up">↑ +5 tháng này</div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon green"><FileText size={22} /></div>
          <div className="stat-info">
            <div className="stat-label">Hợp đồng hiệu lực</div>
            <div className="stat-value">{adminDashboard.activeLeases}</div>
            <div className="stat-change up">↑ +3 tháng này</div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon yellow"><DollarSign size={22} /></div>
          <div className="stat-info">
            <div className="stat-label">Tổng doanh thu</div>
            <div className="stat-value">{(adminDashboard.totalRevenue / 1000000000).toFixed(2)}B</div>
            <div className="stat-change up">↑ VND</div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon red"><AlertCircle size={22} /></div>
          <div className="stat-info">
            <div className="stat-label">Chờ duyệt BDS</div>
            <div className="stat-value">{adminDashboard.pendingApprovals}</div>
            <div className="stat-change down">Cần xem xét</div>
          </div>
        </div>
      </div>

      <div className="grid-2 mb-24">
        <div className="card">
          <div className="card-header">
            <div>
              <div className="card-title">Xu hướng doanh thu</div>
              <div className="card-subtitle">12 tháng gần nhất (triệu VND)</div>
            </div>
            <TrendingUp size={18} style={{ color: 'var(--accent-light)' }} />
          </div>
          <ResponsiveContainer width="100%" height={200}>
            <AreaChart data={chartData}>
              <defs>
                <linearGradient id="colorRev" x1="0" y1="0" x2="0" y2="1">
                  <stop offset="5%" stopColor="#6366f1" stopOpacity={0.3} />
                  <stop offset="95%" stopColor="#6366f1" stopOpacity={0} />
                </linearGradient>
              </defs>
              <CartesianGrid strokeDasharray="3 3" stroke="var(--border)" />
              <XAxis dataKey="name" tick={{ fill: 'var(--text-muted)', fontSize: 11 }} tickLine={false} />
              <YAxis tick={{ fill: 'var(--text-muted)', fontSize: 11 }} tickLine={false} axisLine={false} />
              <Tooltip contentStyle={{ background: 'var(--bg-card)', border: '1px solid var(--border)', borderRadius: 8, color: 'var(--text-primary)' }} formatter={(val) => [`${val}M VND`, 'Doanh thu']} />
              <Area type="monotone" dataKey="revenue" stroke="#6366f1" fill="url(#colorRev)" strokeWidth={2} dot={{ fill: '#6366f1', r: 3 }} />
            </AreaChart>
          </ResponsiveContainer>
        </div>

        <div className="card">
          <div className="card-header">
            <div className="card-title">BDS chờ duyệt</div>
            <span className={`badge badge-warning`}>{pendingProps.length} chờ</span>
          </div>
          {pendingProps.length === 0 ? (
            <div className="empty-state"><div className="empty-icon">✅</div><p>Không có BDS nào chờ duyệt</p></div>
          ) : (
            <div>
              {pendingProps.map(p => (
                <div key={p.id} className="info-row" style={{ padding: '12px 0', alignItems: 'center' }}>
                  <div style={{ flex: 1 }}>
                    <div className="fw-600" style={{ fontSize: 13, color: 'var(--text-primary)' }}>{p.title}</div>
                    <div className="text-sm text-muted">{p.city} • {p.district} • {p.landlord.fullName}</div>
                  </div>
                  <div style={{ display: 'flex', gap: 6 }}>
                    <button className="btn btn-success btn-sm">✓ Duyệt</button>
                    <button className="btn btn-danger btn-sm">✗ Từ chối</button>
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>

      <div className="grid-2 mb-24">
        <div className="card">
          <div className="card-header">
            <div className="card-title">Users mới đăng ký</div>
          </div>
          <div className="table-container">
            <table>
              <thead>
                <tr>
                  <th>Tên</th>
                  <th>Email</th>
                  <th>Vai trò</th>
                  <th>Ngày tạo</th>
                </tr>
              </thead>
              <tbody>
                {recentUsers.map(u => (
                  <tr key={u.id}>
                    <td><strong>{u.fullName}</strong></td>
                    <td className="text-muted">{u.email}</td>
                    <td>{u.isLandlord ? <span className="badge badge-purple">Chủ nhà</span> : u.isTenant ? <span className="badge badge-info">Người thuê</span> : <span className="badge badge-gray">Admin</span>}</td>
                    <td className="text-muted">{formatDate(u.createdAt)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>

        <div className="card">
          <div className="card-header">
            <div className="card-title">Nhật ký hoạt động</div>
          </div>
          <div>
            {auditLogs.slice(0, 5).map((log, idx) => (
              <div key={log.id} className="timeline-item" style={{ paddingBottom: 14 }}>
                <div className="timeline-dot" style={{ background: idx === 0 ? 'var(--accent-glow)' : 'var(--bg-input)', color: 'var(--accent-light)', fontSize: 12 }}>
                  {idx === 0 ? '🔔' : '📋'}
                </div>
                <div className="timeline-content">
                  <div className="timeline-title">{log.action}</div>
                  <div className="timeline-desc">{log.details} • {log.userName}</div>
                  <div style={{ fontSize: 11, color: 'var(--text-muted)', marginTop: 2 }}>{formatDateTime(log.createdAt)}</div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>

      <div className="card">
        <div className="card-header">
          <div className="card-title">Hợp đồng hiệu lực gần đây</div>
        </div>
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>Mã HĐ</th>
                <th>BDS</th>
                <th>Chủ nhà</th>
                <th>Người thuê</th>
                <th>Trạng thái</th>
                <th>Tiền thuê</th>
                <th>Thời hạn</th>
              </tr>
            </thead>
            <tbody>
              {leases.map(l => (
                <tr key={l.id}>
                  <td><strong>{l.leaseNumber}</strong></td>
                  <td>{l.propertyTitle}</td>
                  <td>{l.landlordName}</td>
                  <td>{l.tenantName}</td>
                  <td>{getStatusBadge(l.status)}</td>
                  <td className="text-green fw-600">{formatMoney(l.monthlyRent)}</td>
                  <td className="text-muted">{formatDate(l.startDate)} → {formatDate(l.endDate)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
