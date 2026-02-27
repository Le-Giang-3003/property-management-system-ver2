import {
  adminDashboard,
  properties,
  users,
  leases,
  payments,
  maintenanceRequests,
  auditLogs,
} from "../../utils/mockData";
import {
  formatMoney,
  formatDate,
  formatDateTime,
  getMonthLabel,
  getStatusBadge,
} from "../../utils/helpers";
import {
  LineChart,
  Line,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from "recharts";
import {
  Building2,
  Users,
  FileText,
  TrendingUp,
  AlertCircle,
  CheckCircle,
  Clock,
  DollarSign,
} from "lucide-react";

const chartData = adminDashboard.revenueTrend.map((d) => ({
  name: getMonthLabel(d.year, d.month),
  revenue: d.revenue / 1000000,
}));

export default function AdminDashboard() {
  const pendingProps = properties.filter(
    (p) => p.status === "Pending",
  );
  const activeLeases = leases.filter(
    (l) => l.status === "Active",
  );
  const recentUsers = [...users]
    .sort(
      (a, b) => new Date(b.createdAt) - new Date(a.createdAt),
    )
    .slice(0, 5);

  return (
    <div>
      <div className="mb-20">
        <div className="page-title">Dashboard Quản trị</div>
        <div className="page-desc">
          Tổng quan hệ thống quản lý bất động sản
        </div>
      </div>

      <div className="stat-grid">
        <div className="stat-card">
          <div className="stat-icon purple">
            <Users size={22} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Tổng Users</div>
            <div className="stat-value">
              {adminDashboard.totalUsers.toLocaleString()}
            </div>
            <div className="stat-change up">
              ↑ +12 tháng này
            </div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon blue">
            <Building2 size={22} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Tổng BDS</div>
            <div className="stat-value">
              {adminDashboard.totalProperties}
            </div>
            <div className="stat-change up">↑ +5 tháng này</div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon green">
            <FileText size={22} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Hợp đồng hiệu lực</div>
            <div className="stat-value">
              {adminDashboard.activeLeases}
            </div>
            <div className="stat-change up">↑ +3 tháng này</div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon yellow">
            <DollarSign size={22} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Tổng doanh thu</div>
            <div className="stat-value">
              {(
                adminDashboard.totalRevenue / 1000000000
              ).toFixed(2)}
              B
            </div>
            <div className="stat-change up">↑ VND</div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon red">
            <AlertCircle size={22} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Chờ duyệt BDS</div>
            <div className="stat-value">
              {adminDashboard.pendingApprovals}
            </div>
            <div className="stat-change down">Cần xem xét</div>
          </div>
        </div>
      </div>

      {/* Revenue Chart - Full Width */}
      <div className="card mb-24">
        <div className="card-header">
          <div>
            <div className="card-title">
              Xu hướng doanh thu
            </div>
            <div className="card-subtitle">
              12 tháng gần nhất (triệu VND)
            </div>
          </div>
          <TrendingUp
            size={18}
            style={{ color: "var(--accent-light)" }}
          />
        </div>
        <ResponsiveContainer width="100%" height={280}>
          <LineChart data={chartData}>
            <CartesianGrid
              strokeDasharray="3 3"
              stroke="rgba(255, 255, 255, 0.05)"
              vertical={false}
            />
            <XAxis
              dataKey="name"
              tick={{
                fill: "var(--text-muted)",
                fontSize: 11,
              }}
              tickLine={false}
              axisLine={{ stroke: "rgba(255, 255, 255, 0.1)" }}
            />
            <YAxis
              tick={{
                fill: "var(--text-muted)",
                fontSize: 11,
              }}
              tickLine={false}
              axisLine={false}
            />
            <Tooltip
              contentStyle={{
                background: "var(--bg-card)",
                border: "1px solid var(--border)",
                borderRadius: 8,
                color: "var(--text-primary)",
              }}
              formatter={(val) => [
                `${val}M VND`,
                "Doanh thu",
              ]}
            />
            <Line
              type="monotone"
              dataKey="revenue"
              stroke="#6366f1"
              strokeWidth={2}
              dot={false}
              activeDot={{ r: 4, fill: "#6366f1" }}
            />
          </LineChart>
        </ResponsiveContainer>
      </div>

      {/* Users & Activity */}
      <div className="grid-2 mb-24">
        <div className="card">
          <div className="card-header">
            <div className="card-title">Người dùng mới</div>
          </div>
          <div className="table-container">
            <table>
              <thead>
                <tr>
                  <th>TÊN</th>
                  <th>EMAIL</th>
                  <th>VAI TRÒ</th>
                  <th>NGÀY TẠO</th>
                </tr>
              </thead>
              <tbody>
                {recentUsers.map((u) => (
                  <tr key={u.id}>
                    <td>
                      <strong>{u.fullName}</strong>
                    </td>
                    <td className="text-muted">{u.email}</td>
                    <td>
                      {u.isLandlord ? (
                        <span className="badge badge-purple">
                          Chủ nhà
                        </span>
                      ) : u.isTenant ? (
                        <span className="badge badge-info">
                          Người thuê
                        </span>
                      ) : (
                        <span className="badge badge-gray">
                          Admin
                        </span>
                      )}
                    </td>
                    <td className="text-muted">
                      {formatDate(u.createdAt)}
                    </td>
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
          <div style={{ padding: "0 20px" }}>
            {auditLogs.slice(0, 5).map((log, idx) => (
              <div
                key={log.id}
                className="activity-log-item"
                style={{
                  paddingTop: 16,
                  paddingBottom: 16,
                  borderBottom: idx < 4 ? "1px solid var(--border)" : "none",
                }}
              >
                <div
                  style={{
                    display: "flex",
                    alignItems: "flex-start",
                    gap: 10,
                  }}
                >
                  <div
                    style={{
                      width: 8,
                      height: 8,
                      borderRadius: "50%",
                      background: idx === 0 ? "#fbbf24" : "var(--text-muted)",
                      marginTop: 4,
                      flexShrink: 0,
                    }}
                  />
                  <div style={{ flex: 1 }}>
                    <div
                      style={{
                        fontSize: 13,
                        color: "var(--text-primary)",
                        fontWeight: 500,
                        marginBottom: 4,
                      }}
                    >
                      {log.action}
                    </div>
                    <div
                      style={{
                        fontSize: 12,
                        color: "var(--text-muted)",
                        lineHeight: 1.5,
                      }}
                    >
                      {log.details} • {log.userName}
                    </div>
                    <div
                      style={{
                        fontSize: 11,
                        color: "var(--text-muted)",
                        marginTop: 4,
                      }}
                    >
                      {formatDateTime(log.createdAt)}
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>

      <div className="card">
        <div className="card-header">
          <div className="card-title">
            Hợp đồng hiệu lực gần đây
          </div>
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
              {leases.map((l) => (
                <tr key={l.id}>
                  <td>
                    <strong>{l.leaseNumber}</strong>
                  </td>
                  <td>{l.propertyTitle}</td>
                  <td>{l.landlordName}</td>
                  <td>{l.tenantName}</td>
                  <td>{getStatusBadge(l.status)}</td>
                  <td className="text-green fw-600">
                    {formatMoney(l.monthlyRent)}
                  </td>
                  <td className="text-muted">
                    {formatDate(l.startDate)} →{" "}
                    {formatDate(l.endDate)}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}