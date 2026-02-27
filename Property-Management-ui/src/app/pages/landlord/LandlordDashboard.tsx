import {
  leases,
  properties,
  payments,
  maintenanceRequests,
  bookings,
  landlordDashboard,
} from "../../utils/mockData";
import {
  formatMoney,
  formatDate,
  formatDateTime,
  getMonthLabel,
  getStatusBadge,
  getPriorityBadge,
} from "../../utils/helpers";
import {
  LineChart,
  Line,
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
} from "recharts";
import {
  Building2,
  TrendingUp,
  AlertCircle,
  Clock,
  CreditCard,
  Calendar,
  Wrench,
  CheckCircle,
  DollarSign,
  Users,
  Home,
} from "lucide-react";

const chartData = landlordDashboard.revenueTrend.map((d) => ({
  name: getMonthLabel(d.year, d.month),
  revenue: d.revenue / 1000000,
}));

const myProps = properties.filter((p) => p.landlordId === 2);
const myPayments = payments.slice(0, 5);
const myMaintenance = maintenanceRequests
  .filter((m) => m.propertyId === 2 || m.propertyId === 1)
  .slice(0, 5);
const myBookings = bookings.filter(
  (b) => b.status === "Pending" || b.status === "Confirmed",
).slice(0, 5);

export default function LandlordDashboard() {
  return (
    <div>
      <div className="mb-20">
        <div className="page-title">Dashboard Chủ nhà</div>
        <div className="page-desc">
          Tổng quan quản lý bất động sản của bạn
        </div>
      </div>

      <div className="stat-grid">
        <div className="stat-card">
          <div className="stat-icon purple">
            <Building2 size={22} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Tổng BDS</div>
            <div className="stat-value">
              {landlordDashboard.totalProperties}
            </div>
            <div className="stat-change">
              {
                myProps.filter((p) => p.status === "Rented")
                  .length
              }{" "}
              đang cho thuê
            </div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon green">
            <TrendingUp size={22} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Tỷ lệ lấp đầy</div>
            <div className="stat-value">
              {landlordDashboard.occupancyRate}%
            </div>
            <div className="stat-change up">↑ +5% tháng này</div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon blue">
            <DollarSign size={22} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Thu tháng này</div>
            <div className="stat-value">
              {(
                landlordDashboard.monthlyRevenue / 1000000
              ).toFixed(1)}
              M
            </div>
            <div className="stat-change up">↑ VND</div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon yellow">
            <Clock size={22} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Chờ thanh toán</div>
            <div className="stat-value">
              {(
                landlordDashboard.pendingPayments / 1000000
              ).toFixed(1)}
              M
            </div>
            <div className="stat-change down">VND</div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon red">
            <Wrench size={22} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Bảo trì cần xử lý</div>
            <div className="stat-value">
              {landlordDashboard.activeMaintenanceRequests}
            </div>
            <div className="stat-change down">Cần chú ý</div>
          </div>
        </div>
      </div>

      {/* Revenue Chart - Full Width */}
      <div className="card mb-24">
        <div className="card-header">
          <div>
            <div className="card-title">
              Doanh thu 12 tháng gần nhất
            </div>
            <div className="card-subtitle">
              Xu hướng doanh thu (triệu VND)
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
              formatter={(v) => [`${v}M VND`, "Doanh thu"]}
            />
            <Line
              type="monotone"
              dataKey="revenue"
              stroke="#10b981"
              strokeWidth={2}
              dot={false}
              activeDot={{ r: 4, fill: "#10b981" }}
            />
          </LineChart>
        </ResponsiveContainer>
      </div>

      {/* Properties & Bookings */}
      <div className="grid-2 mb-24">
        <div className="card">
          <div className="card-header">
            <div className="card-title">Bất động sản của tôi</div>
            <span className="badge badge-info">{myProps.length} BDS</span>
          </div>
          <div className="dashboard-list">
            {myProps.map((p) => (
              <div key={p.id} className="dashboard-list-item">
                {p.images[0] ? (
                  <img
                    src={p.images[0].imageUrl}
                    alt=""
                    className="dashboard-list-img"
                  />
                ) : (
                  <div className="dashboard-list-img-placeholder">
                    🏠
                  </div>
                )}
                <div className="dashboard-list-info">
                  <div className="dashboard-list-title">
                    {p.title}
                  </div>
                  <div className="dashboard-list-subtitle">
                    {formatMoney(p.monthlyRent)}/tháng
                  </div>
                </div>
                {getStatusBadge(p.status)}
              </div>
            ))}
          </div>
        </div>

        <div className="card">
          <div className="card-header">
            <div className="card-title">Lịch xem nhà hôm nay</div>
            <span className="badge badge-warning">
              {myBookings.filter((b) => b.status === "Pending").length} chờ duyệt
            </span>
          </div>
          {myBookings.length === 0 ? (
            <div className="empty-state">
              <div className="empty-icon">📅</div>
              <p>Không có lịch xem nhà</p>
            </div>
          ) : (
            <div className="dashboard-list">
              {myBookings.map((b) => (
                <div key={b.id} className="dashboard-booking-item">
                  <div className="dashboard-booking-info">
                    <div className="dashboard-booking-title">
                      {b.propertyTitle}
                    </div>
                    <div className="dashboard-booking-subtitle">
                      {b.tenantName} • {formatDate(b.scheduledDate)} {b.startTime}
                    </div>
                  </div>
                  {getStatusBadge(b.status)}
                </div>
              ))}
            </div>
          )}
        </div>
      </div>

      {/* Payments & Maintenance */}
      <div className="grid-2 mb-24">
        <div className="card">
          <div className="card-header">
            <div className="card-title">Thanh toán gần đây</div>
          </div>
          <div className="dashboard-table">
            <table>
              <thead>
                <tr>
                  <th>MÔ TẢ</th>
                  <th>NGƯỜI THUÊ</th>
                  <th>SỐ TIỀN</th>
                  <th>TRẠNG THÁI</th>
                </tr>
              </thead>
              <tbody>
                {myPayments.map((p) => (
                  <tr key={p.id}>
                    <td>
                      <div className="dashboard-table-title">{p.description}</div>
                    </td>
                    <td>
                      <div className="dashboard-table-subtitle">{p.tenantName}</div>
                    </td>
                    <td>
                      <div className="dashboard-table-price">
                        {formatMoney(p.amount)}
                      </div>
                    </td>
                    <td>{getStatusBadge(p.status)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>

        <div className="card">
          <div className="card-header">
            <div className="card-title">Yêu cầu bảo trì</div>
            <span className="badge badge-warning">
              {myMaintenance.filter((m) => m.status === "Open" || m.status === "InProgress").length} đang xử lý
            </span>
          </div>
          <div className="dashboard-table">
            <table>
              <thead>
                <tr>
                  <th>TIÊU ĐỀ</th>
                  <th>MỨC ĐỘ</th>
                  <th>TRẠNG THÁI</th>
                </tr>
              </thead>
              <tbody>
                {myMaintenance.map((m) => (
                  <tr key={m.id}>
                    <td>
                      <div className="dashboard-table-title">{m.title}</div>
                      <div className="dashboard-table-subtitle">{m.propertyTitle}</div>
                    </td>
                    <td>{getPriorityBadge(m.priority)}</td>
                    <td>{getStatusBadge(m.status)}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      </div>
    </div>
  );
}