import { useState } from "react";
import { users } from "../../utils/mockData";
import {
  formatDate,
  getStatusBadge,
  getRoleName,
} from "../../utils/helpers";
import {
  Search,
  UserPlus,
  Ban,
  CheckCircle,
  Shield,
} from "lucide-react";

export default function AdminUsers() {
  const [search, setSearch] = useState("");
  const [roleFilter, setRoleFilter] = useState("All");
  const [selectedUser, setSelectedUser] = useState(null);

  const filtered = users.filter((u) => {
    const matchSearch =
      u.fullName.toLowerCase().includes(search.toLowerCase()) ||
      u.email.toLowerCase().includes(search.toLowerCase());
    const matchRole =
      roleFilter === "All" ||
      (roleFilter === "Admin" && u.role === "Admin") ||
      (roleFilter === "Landlord" && u.isLandlord) ||
      (roleFilter === "Tenant" && u.isTenant && !u.isLandlord);
    return matchSearch && matchRole;
  });

  return (
    <div>
      <div className="flex items-center justify-between mb-20">
        <div>
          <div className="page-title">Quản lý Người dùng</div>
          <div className="page-desc">
            Quản lý tất cả tài khoản trong hệ thống
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
          <div className="stat-icon purple">
            <Shield size={20} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Admin</div>
            <div className="stat-value">
              {users.filter((u) => u.role === "Admin").length}
            </div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon blue">
            <Shield size={20} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Chủ nhà</div>
            <div className="stat-value">
              {users.filter((u) => u.isLandlord).length}
            </div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon green">
            <Shield size={20} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Người thuê</div>
            <div className="stat-value">
              {
                users.filter((u) => u.isTenant && !u.isLandlord)
                  .length
              }
            </div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon red">
            <Ban size={20} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Bị khoá</div>
            <div className="stat-value">
              {users.filter((u) => !u.isActive).length}
            </div>
          </div>
        </div>
      </div>

      <div className="filter-bar">
        <div
          className="header-search"
          style={{ flex: 1, maxWidth: 320 }}
        >
          <Search
            size={14}
            style={{ color: "var(--text-muted)" }}
          />
          <input
            placeholder="Tìm kiếm theo tên, email..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>
        {["All", "Admin", "Landlord", "Tenant"].map((r) => (
          <button
            key={r}
            className={`btn ${roleFilter === r ? "btn-primary" : "btn-ghost"} btn-sm`}
            onClick={() => setRoleFilter(r)}
          >
            {r === "All"
              ? "Tất cả"
              : r === "Admin"
                ? "Admin"
                : r === "Landlord"
                  ? "Chủ nhà"
                  : "Người thuê"}
          </button>
        ))}
      </div>

      <div className="card">
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>User</th>
                <th>Email</th>
                <th>Điện thoại</th>
                <th>Vai trò</th>
                <th>Xác thực</th>
                <th>Trạng thái</th>
                <th>Đăng nhập cuối</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((u) => (
                <tr key={u.id}>
                  <td>
                    <div
                      style={{
                        display: "flex",
                        alignItems: "center",
                        gap: 10,
                      }}
                    >
                      <div
                        className="user-avatar"
                        style={{
                          width: 32,
                          height: 32,
                          fontSize: 12,
                        }}
                      >
                        {u.fullName[0]}
                      </div>
                      <div>
                        <strong>{u.fullName}</strong>
                        <div className="text-sm text-muted">
                          #{u.id}
                        </div>
                      </div>
                    </div>
                  </td>
                  <td className="text-muted">{u.email}</td>
                  <td className="text-muted">
                    {u.phoneNumber}
                  </td>
                  <td>
                    {u.role === "Admin" ? (
                      <span className="badge badge-danger">
                        Admin
                      </span>
                    ) : u.isLandlord && u.isTenant ? (
                      <span className="badge badge-purple">
                        Chủ nhà & Thuê
                      </span>
                    ) : u.isLandlord ? (
                      <span className="badge badge-purple">
                        Chủ nhà
                      </span>
                    ) : (
                      <span className="badge badge-info">
                        Người thuê
                      </span>
                    )}
                  </td>
                  <td>
                    <div
                      style={{
                        display: "flex",
                        gap: 4,
                        flexWrap: "wrap",
                      }}
                    >
                      {u.isEmailVerified && (
                        <span
                          className="badge badge-success"
                          style={{ fontSize: 10 }}
                        >
                          📧 Email
                        </span>
                      )}
                      {u.isIdentityVerified && (
                        <span
                          className="badge badge-success"
                          style={{ fontSize: 10 }}
                        >
                          🪪 CCCD
                        </span>
                      )}
                    </div>
                  </td>
                  <td>
                    {u.isActive ? (
                      <span className="badge badge-success">
                        Hoạt động
                      </span>
                    ) : (
                      <span className="badge badge-danger">
                        Bị khoá
                      </span>
                    )}
                  </td>
                  <td className="text-muted">
                    {formatDate(u.lastLoginAt)}
                  </td>
                  <td>
                    <div style={{ display: "flex", gap: 6 }}>
                      <button
                        className="btn btn-ghost btn-sm"
                        onClick={() => setSelectedUser(u)}
                      >
                        Xem
                      </button>
                      {u.isActive ? (
                        <button
                          className="btn btn-danger btn-sm"
                          title="Khoá tài khoản"
                        >
                          <Ban size={13} />
                        </button>
                      ) : (
                        <button
                          className="btn btn-success btn-sm"
                          title="Mở khoá"
                        >
                          <CheckCircle size={13} />
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
        <div
          style={{
            padding: "12px 14px",
            borderTop: "1px solid var(--border)",
            fontSize: 12,
            color: "var(--text-muted)",
          }}
        >
          Hiển thị {filtered.length} / {users.length} người dùng
        </div>
      </div>

      {selectedUser && (
        <div
          className="modal-overlay"
          onClick={() => setSelectedUser(null)}
        >
          <div
            className="modal"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="modal-header">
              <span className="modal-title">
                Chi tiết người dùng
              </span>
              <button
                className="modal-close btn btn-ghost btn-sm btn-icon"
                onClick={() => setSelectedUser(null)}
              >
                ✕
              </button>
            </div>
            <div className="modal-body">
              <div
                style={{
                  display: "flex",
                  alignItems: "center",
                  gap: 16,
                  marginBottom: 20,
                  padding: "16px",
                  background: "var(--bg-primary)",
                  borderRadius: 10,
                }}
              >
                <div className="avatar-lg">
                  {selectedUser.fullName[0]}
                </div>
                <div>
                  <div
                    style={{
                      fontSize: 18,
                      fontWeight: 700,
                      color: "var(--text-primary)",
                    }}
                  >
                    {selectedUser.fullName}
                  </div>
                  <div className="text-muted text-sm">
                    {selectedUser.email}
                  </div>
                  <div style={{ marginTop: 6 }}>
                    {selectedUser.isActive ? (
                      <span className="badge badge-success">
                        Hoạt động
                      </span>
                    ) : (
                      <span className="badge badge-danger">
                        Bị khoá
                      </span>
                    )}
                  </div>
                </div>
              </div>
              <div className="info-row">
                <span className="info-label">ID</span>
                <span className="info-value">
                  #{selectedUser.id}
                </span>
              </div>
              <div className="info-row">
                <span className="info-label">Điện thoại</span>
                <span className="info-value">
                  {selectedUser.phoneNumber}
                </span>
              </div>
              <div className="info-row">
                <span className="info-label">Vai trò</span>
                <span className="info-value">
                  {getRoleName(
                    selectedUser.role,
                    selectedUser.isTenant,
                    selectedUser.isLandlord,
                  )}
                </span>
              </div>
              <div className="info-row">
                <span className="info-label">
                  Email xác thực
                </span>
                <span className="info-value">
                  {selectedUser.isEmailVerified
                    ? "✅ Đã xác thực"
                    : "❌ Chưa xác thực"}
                </span>
              </div>
              <div className="info-row">
                <span className="info-label">
                  CCCD xác thực
                </span>
                <span className="info-value">
                  {selectedUser.isIdentityVerified
                    ? "✅ Đã xác thực"
                    : "❌ Chưa xác thực"}
                </span>
              </div>
              <div className="info-row">
                <span className="info-label">Ngày tạo</span>
                <span className="info-value">
                  {formatDate(selectedUser.createdAt)}
                </span>
              </div>
              <div className="info-row">
                <span className="info-label">
                  Đăng nhập cuối
                </span>
                <span className="info-value">
                  {formatDate(selectedUser.lastLoginAt)}
                </span>
              </div>
            </div>
            <div className="modal-footer">
              <button
                className="btn btn-secondary"
                onClick={() => setSelectedUser(null)}
              >
                Đóng
              </button>
              {selectedUser.isActive ? (
                <button className="btn btn-danger">
                  Khoá tài khoản
                </button>
              ) : (
                <button className="btn btn-success">
                  Mở khoá
                </button>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
}