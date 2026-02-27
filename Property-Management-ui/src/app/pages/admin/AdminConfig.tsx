import { useState } from "react";
import { systemConfigs, auditLogs } from "../../utils/mockData";
import { formatDateTime } from "../../utils/helpers";
import { Settings, Save, RotateCcw } from "lucide-react";

export default function AdminConfig() {
  const [configs, setConfigs] = useState(systemConfigs);
  const [tab, setTab] = useState("config");
  const [editing, setEditing] = useState(null);

  const handleEdit = (id, value) =>
    setConfigs((prev) =>
      prev.map((c) => (c.id === id ? { ...c, value } : c)),
    );

  return (
    <div>
      <div className="mb-20">
        <div className="page-title">Cấu hình hệ thống</div>
        <div className="page-desc">
          Quản lý các tham số và cài đặt của hệ thống
        </div>
      </div>

      <div className="tabs">
        <div
          className={`tab ${tab === "config" ? "active" : ""}`}
          onClick={() => setTab("config")}
        >
          ⚙️ Cấu hình
        </div>
        <div
          className={`tab ${tab === "audit" ? "active" : ""}`}
          onClick={() => setTab("audit")}
        >
          📋 Nhật ký kiểm tra
        </div>
      </div>

      {tab === "config" && (
        <div>
          <div className="card mb-20">
            <div className="card-header">
              <div className="card-title">Tham số hệ thống</div>
              <button className="btn btn-primary btn-sm">
                <Save size={13} /> Lưu tất cả
              </button>
            </div>
            <div className="table-container">
              <table>
                <thead>
                  <tr>
                    <th>Tham số</th>
                    <th>Giá trị</th>
                    <th>Loại</th>
                    <th>Đơn vị</th>
                    <th>Mô tả</th>
                    <th>Trạng thái</th>
                    <th>Thao tác</th>
                  </tr>
                </thead>
                <tbody>
                  {configs.map((c) => (
                    <tr key={c.id}>
                      <td>
                        <strong
                          style={{
                            fontFamily: "monospace",
                            fontSize: 12,
                            color: "var(--accent-light)",
                          }}
                        >
                          {c.key}
                        </strong>
                      </td>
                      <td>
                        {editing === c.id ? (
                          <input
                            className="form-control"
                            style={{
                              width: 120,
                              padding: "4px 8px",
                              fontSize: 12,
                            }}
                            value={c.value}
                            onChange={(e) =>
                              handleEdit(c.id, e.target.value)
                            }
                            onBlur={() => setEditing(null)}
                            autoFocus
                          />
                        ) : (
                          <strong>
                            {c.value} {c.unit}
                          </strong>
                        )}
                      </td>
                      <td>
                        <span className="badge badge-gray">
                          {c.type}
                        </span>
                      </td>
                      <td className="text-muted">
                        {c.unit || "—"}
                      </td>
                      <td
                        className="text-muted"
                        style={{ maxWidth: 200, fontSize: 12 }}
                      >
                        {c.description}
                      </td>
                      <td>
                        {c.isActive ? (
                          <span className="badge badge-success">
                            Bật
                          </span>
                        ) : (
                          <span className="badge badge-gray">
                            Tắt
                          </span>
                        )}
                      </td>
                      <td>
                        <button
                          className="btn btn-ghost btn-sm"
                          onClick={() => setEditing(c.id)}
                        >
                          Sửa
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>

          <div className="grid-3">
            {[
              {
                title: "Thanh toán",
                icon: "💳",
                items: configs.filter(
                  (c) => c.type === "Payment",
                ),
              },
              {
                title: "Đặt lịch",
                icon: "📅",
                items: configs.filter(
                  (c) => c.type === "Booking",
                ),
              },
              {
                title: "Hệ thống",
                icon: "⚙️",
                items: configs.filter(
                  (c) =>
                    c.type === "System" || c.type === "Lease",
                ),
              },
            ].map((group) => (
              <div key={group.title} className="card">
                <div className="card-header">
                  <div className="card-title">
                    {group.icon} Nhóm {group.title}
                  </div>
                </div>
                {group.items.map((item) => (
                  <div key={item.id} className="info-row">
                    <span
                      className="info-label"
                      style={{ fontSize: 11 }}
                    >
                      {item.description}
                    </span>
                    <span className="info-value fw-600 text-purple">
                      {item.value} {item.unit}
                    </span>
                  </div>
                ))}
              </div>
            ))}
          </div>
        </div>
      )}

      {tab === "audit" && (
        <div className="card">
          <div className="card-header">
            <div className="card-title">
              Nhật ký kiểm tra hệ thống
            </div>
            <span className="badge badge-info">
              {auditLogs.length} hoạt động
            </span>
          </div>
          <div className="table-container">
            <table>
              <thead>
                <tr>
                  <th>#</th>
                  <th>Người thực hiện</th>
                  <th>Hành động</th>
                  <th>Chi tiết</th>
                  <th>Thời gian</th>
                </tr>
              </thead>
              <tbody>
                {auditLogs.map((log) => (
                  <tr key={log.id}>
                    <td className="text-muted">{log.id}</td>
                    <td>
                      <div
                        style={{
                          display: "flex",
                          alignItems: "center",
                          gap: 8,
                        }}
                      >
                        <div
                          className="user-avatar"
                          style={{
                            width: 28,
                            height: 28,
                            fontSize: 11,
                          }}
                        >
                          {log.userName[0]}
                        </div>
                        <strong>{log.userName}</strong>
                      </div>
                    </td>
                    <td>
                      <span
                        className="badge badge-purple"
                        style={{
                          fontFamily: "monospace",
                          fontSize: 10,
                        }}
                      >
                        {log.action}
                      </span>
                    </td>
                    <td
                      className="text-muted"
                      style={{
                        maxWidth: 280,
                        fontSize: 12,
                        whiteSpace: "normal",
                      }}
                    >
                      {log.details}
                    </td>
                    <td className="text-muted">
                      {formatDateTime(log.createdAt)}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
    </div>
  );
}