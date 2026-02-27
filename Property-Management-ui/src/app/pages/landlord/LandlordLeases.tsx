import { useState } from "react";
import { leases } from "../../utils/mockData";
import {
  formatMoney,
  formatDate,
  getStatusBadge,
} from "../../utils/helpers";
import { FileText, Eye, CheckCircle, Clock, XCircle, AlertCircle } from "lucide-react";
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, Legend, ResponsiveContainer, Area, AreaChart } from "recharts";

const myLeases = leases.filter((l) => l.landlordId === 2);

// Prepare chart data - lease timeline
const chartData = [
  { date: "1/2024", active: 0, pending: 1, expired: 0, terminated: 0 },
  { date: "3/2024", active: 1, pending: 0, expired: 0, terminated: 0 },
  { date: "7/2024", active: 2, pending: 1, expired: 0, terminated: 0 },
  { date: "9/2024", active: 3, pending: 0, expired: 0, terminated: 0 },
  { date: "11/2024", active: 4, pending: 0, expired: 1, terminated: 0 },
  { date: "1/2025", active: 3, pending: 1, expired: 1, terminated: 0 },
  { date: "Hiện tại", active: 1, pending: 1, expired: 1, terminated: 0 },
];

export default function LandlordLeases() {
  const [selectedLease, setSelectedLease] = useState(null);

  return (
    <div>
      <div className="flex items-center justify-between mb-20">
        <div>
          <div className="page-title">Hợp đồng thuê</div>
          <div className="page-desc">
            Quản lý tất cả hợp đồng cho thuê của bạn
          </div>
        </div>
      </div>

      <div
        className="stat-grid"
        style={{
          gridTemplateColumns: "repeat(4, 1fr)",
          marginBottom: 20,
        }}
      >
        <div className="stat-card">
          <div className="stat-icon green">
            <FileText size={20} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Đang hiệu lực</div>
            <div className="stat-value">
              {
                myLeases.filter((l) => l.status === "Active")
                  .length
              }
            </div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon yellow">
            <FileText size={20} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Chờ ký</div>
            <div className="stat-value">
              {
                myLeases.filter((l) => l.status === "Pending")
                  .length
              }
            </div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon blue">
            <FileText size={20} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Hết hạn</div>
            <div className="stat-value">
              {
                myLeases.filter((l) => l.status === "Expired")
                  .length
              }
            </div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon red">
            <FileText size={20} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Chấm dứt</div>
            <div className="stat-value">
              {
                myLeases.filter(
                  (l) => l.status === "Terminated",
                ).length
              }
            </div>
          </div>
        </div>
      </div>

      {/* Timeline Chart */}
      <div className="card" style={{ marginBottom: 24, padding: 24 }}>
        <div style={{ marginBottom: 20 }}>
          <h3 style={{ fontSize: 18, fontWeight: 700, color: "white", marginBottom: 4 }}>
            Timeline Hợp đồng
          </h3>
          <p style={{ fontSize: 14, color: "#9ca3af" }}>
            Biểu đồ theo dõi trạng thái hợp đồng theo thời gian
          </p>
        </div>
        <ResponsiveContainer width="100%" height={300}>
          <AreaChart data={chartData}>
            <defs>
              <linearGradient id="colorActive" x1="0" y1="0" x2="0" y2="1">
                <stop offset="5%" stopColor="#10b981" stopOpacity={0.8}/>
                <stop offset="95%" stopColor="#10b981" stopOpacity={0}/>
              </linearGradient>
              <linearGradient id="colorPending" x1="0" y1="0" x2="0" y2="1">
                <stop offset="5%" stopColor="#f59e0b" stopOpacity={0.8}/>
                <stop offset="95%" stopColor="#f59e0b" stopOpacity={0}/>
              </linearGradient>
              <linearGradient id="colorExpired" x1="0" y1="0" x2="0" y2="1">
                <stop offset="5%" stopColor="#3b82f6" stopOpacity={0.8}/>
                <stop offset="95%" stopColor="#3b82f6" stopOpacity={0}/>
              </linearGradient>
              <linearGradient id="colorTerminated" x1="0" y1="0" x2="0" y2="1">
                <stop offset="5%" stopColor="#ef4444" stopOpacity={0.8}/>
                <stop offset="95%" stopColor="#ef4444" stopOpacity={0}/>
              </linearGradient>
            </defs>
            <CartesianGrid strokeDasharray="3 3" stroke="#2d3142" />
            <XAxis 
              dataKey="date" 
              stroke="#9ca3af" 
              style={{ fontSize: 12 }}
            />
            <YAxis 
              stroke="#9ca3af" 
              style={{ fontSize: 12 }}
            />
            <Tooltip 
              contentStyle={{ 
                backgroundColor: "#1a1d2e", 
                border: "1px solid #2d3142",
                borderRadius: 8,
                color: "white"
              }}
            />
            <Legend 
              wrapperStyle={{ paddingTop: 20 }}
              formatter={(value) => {
                const labels = {
                  active: "Đang hiệu lực",
                  pending: "Chờ ký",
                  expired: "Hết hạn",
                  terminated: "Chấm dứt"
                };
                return labels[value] || value;
              }}
            />
            <Area 
              type="monotone" 
              dataKey="active" 
              stroke="#10b981" 
              fillOpacity={1} 
              fill="url(#colorActive)" 
            />
            <Area 
              type="monotone" 
              dataKey="pending" 
              stroke="#f59e0b" 
              fillOpacity={1} 
              fill="url(#colorPending)" 
            />
            <Area 
              type="monotone" 
              dataKey="expired" 
              stroke="#3b82f6" 
              fillOpacity={1} 
              fill="url(#colorExpired)" 
            />
            <Area 
              type="monotone" 
              dataKey="terminated" 
              stroke="#ef4444" 
              fillOpacity={1} 
              fill="url(#colorTerminated)" 
            />
          </AreaChart>
        </ResponsiveContainer>
      </div>

      <div className="card">
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>Mã HĐ</th>
                <th>BDS</th>
                <th>Người thuê</th>
                <th>Trạng thái</th>
                <th>Chữ ký</th>
                <th>Tiền thuê/tháng</th>
                <th>Thời hạn</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {myLeases.map((l) => (
                <tr key={l.id}>
                  <td>
                    <strong
                      style={{ color: "var(--accent-light)" }}
                    >
                      {l.leaseNumber}
                    </strong>
                  </td>
                  <td>{l.propertyTitle}</td>
                  <td>{l.tenantName}</td>
                  <td>{getStatusBadge(l.status)}</td>
                  <td>
                    <div style={{ display: "flex", gap: 4 }}>
                      <span
                        style={{ fontSize: 11 }}
                        className={`badge ${l.landlordSigned ? "badge-success" : "badge-gray"}`}
                      >
                        {l.landlordSigned
                          ? "✓ Chủ nhà"
                          : "⏳ Chủ nhà"}
                      </span>
                      <span
                        style={{ fontSize: 11 }}
                        className={`badge ${l.tenantSigned ? "badge-success" : "badge-gray"}`}
                      >
                        {l.tenantSigned
                          ? "✓ Người thuê"
                          : "⏳ Người thuê"}
                      </span>
                    </div>
                  </td>
                  <td className="text-green fw-600">
                    {formatMoney(l.monthlyRent)}
                  </td>
                  <td className="text-muted">
                    <div style={{ fontSize: 12 }}>
                      {formatDate(l.startDate)}
                    </div>
                    <div style={{ fontSize: 12 }}>
                      → {formatDate(l.endDate)}
                    </div>
                  </td>
                  <td>
                    <button
                      className="btn btn-ghost btn-sm"
                      onClick={() => setSelectedLease(l)}
                    >
                      <Eye size={13} /> Chi tiết
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {selectedLease && (
        <div
          className="modal-overlay"
          onClick={() => setSelectedLease(null)}
        >
          <div
            className="modal"
            style={{ maxWidth: 680 }}
            onClick={(e) => e.stopPropagation()}
          >
            <div className="modal-header">
              <span className="modal-title">
                Hợp đồng {selectedLease.leaseNumber}
              </span>
              <button
                className="modal-close btn btn-ghost btn-sm btn-icon"
                onClick={() => setSelectedLease(null)}
              >
                ✕
              </button>
            </div>
            <div className="modal-body">
              <div
                style={{
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "space-between",
                  marginBottom: 16,
                  padding: 16,
                  background: "var(--bg-primary)",
                  borderRadius: 8,
                }}
              >
                <div>
                  <div
                    className="fw-700"
                    style={{
                      fontSize: 18,
                      color: "var(--accent-light)",
                    }}
                  >
                    {selectedLease.leaseNumber}
                  </div>
                  <div className="text-muted text-sm">
                    Tạo ngày{" "}
                    {formatDate(selectedLease.createdAt)}
                  </div>
                </div>
                {getStatusBadge(selectedLease.status)}
              </div>
              <div className="grid-2">
                <div>
                  <div className="info-row">
                    <span className="info-label">BDS</span>
                    <span className="info-value">
                      {selectedLease.propertyTitle}
                    </span>
                  </div>
                  <div className="info-row">
                    <span className="info-label">Địa chỉ</span>
                    <span className="info-value text-sm">
                      {selectedLease.propertyAddress}
                    </span>
                  </div>
                  <div className="info-row">
                    <span className="info-label">Chủ nhà</span>
                    <span className="info-value">
                      {selectedLease.landlordName}
                    </span>
                  </div>
                  <div className="info-row">
                    <span className="info-label">
                      Người thuê
                    </span>
                    <span className="info-value">
                      {selectedLease.tenantName}
                    </span>
                  </div>
                </div>
                <div>
                  <div className="info-row">
                    <span className="info-label">
                      Tiền thuê/tháng
                    </span>
                    <span className="info-value text-green fw-700">
                      {formatMoney(selectedLease.monthlyRent)}
                    </span>
                  </div>
                  <div className="info-row">
                    <span className="info-label">Đặt cọc</span>
                    <span className="info-value">
                      {formatMoney(selectedLease.depositAmount)}
                    </span>
                  </div>
                  <div className="info-row">
                    <span className="info-label">
                      Ngày thanh toán
                    </span>
                    <span className="info-value">
                      Ngày {selectedLease.paymentDueDay} hàng
                      tháng
                    </span>
                  </div>
                  <div className="info-row">
                    <span className="info-label">
                      Phí trễ hạn
                    </span>
                    <span className="info-value">
                      {selectedLease.lateFeePercentage}%/tháng
                    </span>
                  </div>
                </div>
              </div>
              <div className="info-row">
                <span className="info-label">Ngày bắt đầu</span>
                <span className="info-value">
                  {formatDate(selectedLease.startDate)}
                </span>
              </div>
              <div className="info-row">
                <span className="info-label">
                  Ngày kết thúc
                </span>
                <span className="info-value">
                  {formatDate(selectedLease.endDate)}
                </span>
              </div>
              {selectedLease.terms && (
                <div className="info-row">
                  <span className="info-label">Điều khoản</span>
                  <span className="info-value text-sm">
                    {selectedLease.terms}
                  </span>
                </div>
              )}
              {selectedLease.specialConditions && (
                <div className="info-row">
                  <span className="info-label">
                    Điều khoản đặc biệt
                  </span>
                  <span className="info-value text-sm">
                    {selectedLease.specialConditions}
                  </span>
                </div>
              )}
              <div
                className="mb-16 mt-16"
                style={{
                  padding: 14,
                  background: "var(--bg-primary)",
                  borderRadius: 8,
                }}
              >
                <div
                  className="fw-600 mb-8"
                  style={{ fontSize: 13 }}
                >
                  Tình trạng chữ ký
                </div>
                <div style={{ display: "flex", gap: 12 }}>
                  <div
                    style={{
                      flex: 1,
                      padding: 12,
                      border: `1px solid ${selectedLease.landlordSigned ? "var(--success)" : "var(--border)"}`,
                      borderRadius: 8,
                      textAlign: "center",
                    }}
                  >
                    <div
                      style={{ fontSize: 20, marginBottom: 4 }}
                    >
                      {selectedLease.landlordSigned
                        ? "✅"
                        : "⏳"}
                    </div>
                    <div
                      className="fw-600"
                      style={{ fontSize: 12 }}
                    >
                      Chủ nhà
                    </div>
                    <div className="text-muted text-sm">
                      {selectedLease.landlordSigned
                        ? formatDate(
                            selectedLease.landlordSignedAt,
                          )
                        : "Chưa ký"}
                    </div>
                  </div>
                  <div
                    style={{
                      flex: 1,
                      padding: 12,
                      border: `1px solid ${selectedLease.tenantSigned ? "var(--success)" : "var(--border)"}`,
                      borderRadius: 8,
                      textAlign: "center",
                    }}
                  >
                    <div
                      style={{ fontSize: 20, marginBottom: 4 }}
                    >
                      {selectedLease.tenantSigned ? "✅" : "⏳"}
                    </div>
                    <div
                      className="fw-600"
                      style={{ fontSize: 12 }}
                    >
                      Người thuê
                    </div>
                    <div className="text-muted text-sm">
                      {selectedLease.tenantSigned
                        ? formatDate(
                            selectedLease.tenantSignedAt,
                          )
                        : "Chưa ký"}
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div className="modal-footer">
              <button
                className="btn btn-secondary"
                onClick={() => setSelectedLease(null)}
              >
                Đóng
              </button>
              {selectedLease.status === "Active" && (
                <button className="btn btn-danger">
                  Chấm dứt HĐ
                </button>
              )}
              {selectedLease.status === "Expired" && (
                <button className="btn btn-primary">
                  Gia hạn HĐ
                </button>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}