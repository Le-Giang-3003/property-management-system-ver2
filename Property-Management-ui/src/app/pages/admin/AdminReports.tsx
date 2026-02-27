import { useState } from "react";
import {
  properties,
  revenueReports,
  adminDashboard,
} from "../../utils/mockData";
import {
  formatMoney,
  getMonthLabel,
} from "../../utils/helpers";
import {
  BarChart,
  Bar,
  XAxis,
  YAxis,
  CartesianGrid,
  Tooltip,
  ResponsiveContainer,
  PieChart,
  Pie,
  Cell,
  Legend,
} from "recharts";

const COLORS = [
  "#6366f1",
  "#10b981",
  "#f59e0b",
  "#ef4444",
  "#3b82f6",
  "#8b5cf6",
];

const chartData = adminDashboard.revenueTrend.map((d) => ({
  name: getMonthLabel(d.year, d.month),
  revenue: d.revenue / 1000000,
}));

const pieData = [
  { name: "Tiền thuê", value: 82 },
  { name: "Đặt cọc", value: 12 },
  { name: "Phí trễ", value: 4 },
  { name: "Bảo trì", value: 2 },
];

export default function AdminReports() {
  const [tab, setTab] = useState("revenue");

  return (
    <div>
      <div className="mb-20">
        <div className="page-title">Báo cáo & Thống kê</div>
        <div className="page-desc">
          Phân tích doanh thu, tỷ lệ lấp đầy và các chỉ số quan
          trọng
        </div>
      </div>

      <div className="tabs">
        <div
          className={`tab ${tab === "revenue" ? "active" : ""}`}
          onClick={() => setTab("revenue")}
        >
          💰 Doanh thu
        </div>
        <div
          className={`tab ${tab === "occupancy" ? "active" : ""}`}
          onClick={() => setTab("occupancy")}
        >
          🏠 Tỷ lệ lấp đầy
        </div>
        <div
          className={`tab ${tab === "property" ? "active" : ""}`}
          onClick={() => setTab("property")}
        >
          📊 Theo BDS
        </div>
      </div>

      {tab === "revenue" && (
        <div>
          <div
            className="stat-grid mb-24"
            style={{ gridTemplateColumns: "repeat(4, 1fr)" }}
          >
            <div className="stat-card">
              <div className="stat-info">
                <div className="stat-label">
                  Tổng doanh thu (năm 2025)
                </div>
                <div
                  className="stat-value"
                  style={{ fontSize: 20 }}
                >
                  2.45B
                </div>
                <div className="stat-change up">↑ VND</div>
              </div>
            </div>
            <div className="stat-card">
              <div className="stat-info">
                <div className="stat-label">
                  Doanh thu tháng này
                </div>
                <div
                  className="stat-value"
                  style={{ fontSize: 20 }}
                >
                  192M
                </div>
                <div className="stat-change down">
                  ↓ -26% vs tháng trước
                </div>
              </div>
            </div>
            <div className="stat-card">
              <div className="stat-info">
                <div className="stat-label">
                  Trung bình/tháng
                </div>
                <div
                  className="stat-value"
                  style={{ fontSize: 20 }}
                >
                  204M
                </div>
                <div className="stat-change up">
                  ↑ VND/tháng
                </div>
              </div>
            </div>
            <div className="stat-card">
              <div className="stat-info">
                <div className="stat-label">Phí trễ hạn</div>
                <div
                  className="stat-value"
                  style={{ fontSize: 20 }}
                >
                  8.75M
                </div>
                <div className="stat-change down">
                  VND năm nay
                </div>
              </div>
            </div>
          </div>

          <div className="grid-2 mb-24">
            <div className="card">
              <div className="card-header">
                <div className="card-title">
                  Doanh thu theo tháng (triệu VND)
                </div>
              </div>
              <ResponsiveContainer width="100%" height={220}>
                <BarChart data={chartData}>
                  <CartesianGrid
                    strokeDasharray="3 3"
                    stroke="var(--border)"
                  />
                  <XAxis
                    dataKey="name"
                    tick={{
                      fill: "var(--text-muted)",
                      fontSize: 11,
                    }}
                    tickLine={false}
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
                    formatter={(v) => [`${v}M VND`]}
                  />
                  <Bar
                    dataKey="revenue"
                    fill="#6366f1"
                    radius={[4, 4, 0, 0]}
                  />
                </BarChart>
              </ResponsiveContainer>
            </div>
            <div className="card">
              <div className="card-header">
                <div className="card-title">
                  Cơ cấu doanh thu
                </div>
              </div>
              <ResponsiveContainer width="100%" height={220}>
                <PieChart>
                  <Pie
                    data={pieData}
                    cx="50%"
                    cy="50%"
                    innerRadius={60}
                    outerRadius={90}
                    dataKey="value"
                    label={({ name, value }) =>
                      `${name}: ${value}%`
                    }
                    labelLine={false}
                  >
                    {pieData.map((_, i) => (
                      <Cell
                        key={i}
                        fill={COLORS[i % COLORS.length]}
                      />
                    ))}
                  </Pie>
                  <Legend
                    wrapperStyle={{
                      fontSize: 12,
                      color: "var(--text-secondary)",
                    }}
                  />
                </PieChart>
              </ResponsiveContainer>
            </div>
          </div>
        </div>
      )}

      {tab === "occupancy" && (
        <div>
          <div
            className="stat-grid mb-20"
            style={{ gridTemplateColumns: "repeat(3, 1fr)" }}
          >
            <div className="stat-card">
              <div className="stat-info">
                <div className="stat-label">
                  Trung bình lấp đầy
                </div>
                <div className="stat-value">50%</div>
              </div>
            </div>
            <div className="stat-card">
              <div className="stat-info">
                <div className="stat-label">BDS đang thuê</div>
                <div className="stat-value">1 / 2</div>
              </div>
            </div>
            <div className="stat-card">
              <div className="stat-info">
                <div className="stat-label">
                  Số ngày trống TB
                </div>
                <div className="stat-value">45</div>
              </div>
            </div>
          </div>
          <div className="card">
            <div className="card-header">
              <div className="card-title">
                Tỷ lệ lấp đầy theo BDS
              </div>
            </div>
            <table
              style={{
                width: "100%",
                borderCollapse: "collapse",
                fontSize: 13,
              }}
            >
              <thead>
                <tr>
                  <th
                    style={{
                      padding: "10px 14px",
                      textAlign: "left",
                      color: "var(--text-muted)",
                      fontSize: 11,
                      fontWeight: 600,
                    }}
                  >
                    BDS
                  </th>
                  <th
                    style={{
                      padding: "10px 14px",
                      textAlign: "left",
                      color: "var(--text-muted)",
                      fontSize: 11,
                    }}
                  >
                    Đang thuê
                  </th>
                  <th
                    style={{
                      padding: "10px 14px",
                      textAlign: "left",
                      color: "var(--text-muted)",
                      fontSize: 11,
                    }}
                  >
                    Tỷ lệ
                  </th>
                  <th
                    style={{
                      padding: "10px 14px",
                      textAlign: "left",
                      color: "var(--text-muted)",
                      fontSize: 11,
                    }}
                  >
                    Progress
                  </th>
                </tr>
              </thead>
              <tbody>
                {[
                  {
                    title: "Căn hộ Vinhomes",
                    occupied: 28,
                    total: 28,
                    rate: 100,
                  },
                  {
                    title: "Nhà phố Thảo Điền",
                    occupied: 28,
                    total: 28,
                    rate: 100,
                  },
                  {
                    title: "Studio Q7",
                    occupied: 0,
                    total: 28,
                    rate: 0,
                  },
                  {
                    title: "Phòng trọ Gò Vấp",
                    occupied: 0,
                    total: 28,
                    rate: 0,
                  },
                ].map((r) => (
                  <tr
                    key={r.title}
                    style={{
                      borderBottom: "1px solid var(--border)",
                    }}
                  >
                    <td style={{ padding: "14px" }}>
                      <strong>{r.title}</strong>
                    </td>
                    <td
                      style={{
                        padding: "14px",
                        color: "var(--text-muted)",
                      }}
                    >
                      {r.occupied}/{r.total} ngày
                    </td>
                    <td style={{ padding: "14px" }}>
                      <strong
                        style={{
                          color:
                            r.rate > 0
                              ? "var(--success)"
                              : "var(--danger)",
                        }}
                      >
                        {r.rate}%
                      </strong>
                    </td>
                    <td style={{ padding: "14px", width: 200 }}>
                      <div className="progress-bar">
                        <div
                          className="progress-fill"
                          style={{
                            width: `${r.rate}%`,
                            background:
                              r.rate > 50
                                ? "var(--success)"
                                : "var(--danger)",
                          }}
                        />
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}

      {tab === "property" && (
        <div className="card">
          <div className="card-header">
            <div className="card-title">
              Báo cáo doanh thu theo BDS
            </div>
          </div>
          <div className="table-container">
            <table>
              <thead>
                <tr>
                  <th>BDS</th>
                  <th>Tổng tiền thuê thu được</th>
                  <th>Phí trễ hạn</th>
                  <th>Chi phí bảo trì</th>
                  <th>Doanh thu gộp</th>
                  <th>Doanh thu ròng</th>
                </tr>
              </thead>
              <tbody>
                {revenueReports.map((r) => (
                  <tr key={r.propertyId}>
                    <td>
                      <strong>{r.propertyTitle}</strong>
                    </td>
                    <td className="text-green">
                      {formatMoney(r.totalRentCollected)}
                    </td>
                    <td className="text-yellow">
                      {formatMoney(r.totalLateFees)}
                    </td>
                    <td className="text-red">
                      {formatMoney(r.totalMaintenanceCost)}
                    </td>
                    <td className="fw-600">
                      {formatMoney(r.grossRevenue)}
                    </td>
                    <td className="text-green fw-700">
                      {formatMoney(r.netRevenue)}
                    </td>
                  </tr>
                ))}
              </tbody>
              <tfoot>
                <tr
                  style={{
                    background: "var(--bg-input)",
                    fontWeight: 700,
                  }}
                >
                  <td
                    style={{
                      padding: "12px 14px",
                      color: "var(--text-primary)",
                    }}
                  >
                    TỔNG
                  </td>
                  <td
                    style={{
                      padding: "12px 14px",
                      color: "var(--success)",
                    }}
                  >
                    {formatMoney(
                      revenueReports.reduce(
                        (s, r) => s + r.totalRentCollected,
                        0,
                      ),
                    )}
                  </td>
                  <td
                    style={{
                      padding: "12px 14px",
                      color: "var(--warning)",
                    }}
                  >
                    {formatMoney(
                      revenueReports.reduce(
                        (s, r) => s + r.totalLateFees,
                        0,
                      ),
                    )}
                  </td>
                  <td
                    style={{
                      padding: "12px 14px",
                      color: "var(--danger)",
                    }}
                  >
                    {formatMoney(
                      revenueReports.reduce(
                        (s, r) => s + r.totalMaintenanceCost,
                        0,
                      ),
                    )}
                  </td>
                  <td
                    style={{
                      padding: "12px 14px",
                      color: "var(--text-primary)",
                    }}
                  >
                    {formatMoney(
                      revenueReports.reduce(
                        (s, r) => s + r.grossRevenue,
                        0,
                      ),
                    )}
                  </td>
                  <td
                    style={{
                      padding: "12px 14px",
                      color: "var(--success)",
                    }}
                  >
                    {formatMoney(
                      revenueReports.reduce(
                        (s, r) => s + r.netRevenue,
                        0,
                      ),
                    )}
                  </td>
                </tr>
              </tfoot>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}