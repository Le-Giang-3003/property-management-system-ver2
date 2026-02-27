import { useState } from "react";
import { properties } from "../../utils/mockData";
import {
  formatMoney,
  formatDate,
  getStatusBadge,
  getPropertyTypeLabel,
} from "../../utils/helpers";
import {
  Search,
  Plus,
  Eye,
  CheckCircle,
  XCircle,
  Bed,
  Bath,
  Maximize2,
  Edit,
  Trash2,
  Lock,
  Check,
  X,
} from "lucide-react";
import PropertyDetailModal from "../../components/PropertyDetailModal";

export default function AdminProperties() {
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("All");
  const [selectedProp, setSelectedProp] = useState(null);
  const [showRejectModal, setShowRejectModal] = useState(false);
  const [rejectReason, setRejectReason] = useState("");

  const filtered = properties.filter((p) => {
    const matchSearch =
      p.title.toLowerCase().includes(search.toLowerCase()) ||
      p.city.toLowerCase().includes(search.toLowerCase()) ||
      p.landlord.fullName.toLowerCase().includes(search.toLowerCase());
    const matchStatus =
      statusFilter === "All" || p.status === statusFilter;
    return matchSearch && matchStatus;
  });

  return (
    <div>
      <div className="flex items-center justify-between mb-20">
        <div>
          <div className="page-title">Quản lý Bất động sản</div>
          <div className="page-desc">
            Duyệt và quản lý tất cả bất động sản trong hệ thống
          </div>
        </div>
      </div>

      {/* Search + Filter Tabs */}
      <div className="admin-search-filter">
        <div className="admin-search-wrapper">
          <Search size={18} className="admin-search-icon" />
          <input
            className="admin-search-input"
            placeholder="Tìm kiếm BĐS, chủ nhà, địa chỉ..."
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>
        <div className="admin-filter-tabs">
          {[
            { key: "All", label: "Tất cả" },
            { key: "Available", label: "Đăng đăng" },
            { key: "Rented", label: "Đã thuê" },
            { key: "Pending", label: "Chờ duyệt" },
            { key: "Rejected", label: "Từ chối" },
            { key: "Draft", label: "Bị khoá" },
          ].map((item) => (
            <button
              key={item.key}
              className={`admin-filter-tab ${statusFilter === item.key ? "active" : ""}`}
              onClick={() => setStatusFilter(item.key)}
            >
              {item.label}
            </button>
          ))}
        </div>
      </div>

      {/* Table */}
      <div className="card">
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>TÊN BĐS</th>
                <th>LOẠI</th>
                <th>CHỦ NHÀ</th>
                <th>TRẠNG THÁI</th>
                <th>TIỀN THUÊ</th>
                <th>HÀNH ĐỘNG</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((p) => (
                <tr key={p.id}>
                  <td>
                    <span className="text-muted">#{p.id}</span>
                  </td>
                  <td>
                    <div className="admin-property-cell">
                      {p.images[0] ? (
                        <img
                          src={p.images[0].imageUrl}
                          alt=""
                          className="admin-property-thumb"
                        />
                      ) : (
                        <div className="admin-property-thumb-placeholder">
                          🏠
                        </div>
                      )}
                      <div className="admin-property-info">
                        <div className="admin-property-name">
                          {p.title}
                        </div>
                        <div className="admin-property-meta">
                          {p.district}, {p.city}
                        </div>
                      </div>
                    </div>
                  </td>
                  <td>
                    <span className="badge badge-purple">
                      {getPropertyTypeLabel(p.propertyType)}
                    </span>
                  </td>
                  <td className="admin-landlord-cell">
                    <span className="admin-landlord-text">LÂN VĂN Chính</span>
                  </td>
                  <td>{getStatusBadge(p.status)}</td>
                  <td className="text-green fw-700">
                    {formatMoney(p.monthlyRent)}
                  </td>
                  <td>
                    <div className="admin-action-buttons">
                      {/* Nút Xem - luôn hiển thị */}
                      <button
                        className="admin-btn-view"
                        onClick={() => setSelectedProp(p)}
                        title="Xem chi tiết"
                      >
                        <Eye size={16} />
                      </button>
                      
                      {/* Nếu status là Pending - hiển thị Check và X */}
                      {p.status === "Pending" && (
                        <>
                          <button
                            className="admin-btn-approve"
                            title="Duyệt"
                          >
                            <Check size={16} />
                          </button>
                          <button
                            className="admin-btn-reject"
                            onClick={() => {
                              setSelectedProp(p);
                              setShowRejectModal(true);
                            }}
                            title="Từ chối"
                          >
                            <X size={16} />
                          </button>
                        </>
                      )}
                      
                      {/* Nếu status là Available - hiển thị Lock */}
                      {p.status === "Available" && (
                        <button
                          className="admin-btn-lock"
                          title="Khóa"
                        >
                          <Lock size={16} />
                        </button>
                      )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Detail Modal */}
      {selectedProp && !showRejectModal && (
        <PropertyDetailModal
          property={selectedProp}
          onClose={() => setSelectedProp(null)}
          onApprove={() => setSelectedProp(null)}
          onReject={() => setShowRejectModal(true)}
        />
      )}

      {showRejectModal && (
        <div
          className="modal-overlay"
          onClick={() => setShowRejectModal(false)}
        >
          <div
            className="modal"
            style={{ maxWidth: 440 }}
            onClick={(e) => e.stopPropagation()}
          >
            <div className="modal-header">
              <span className="modal-title">Từ chối BDS</span>
              <button
                className="modal-close btn btn-ghost btn-sm btn-icon"
                onClick={() => setShowRejectModal(false)}
              >
                ✕
              </button>
            </div>
            <div className="modal-body">
              <p
                style={{
                  marginBottom: 12,
                  fontSize: 13,
                  color: "var(--text-secondary)",
                }}
              >
                Vui lòng nhập lý do từ chối cho{" "}
                <strong>{selectedProp?.title}</strong>:
              </p>
              <div className="form-group">
                <label className="form-label">
                  Lý do từ chối
                </label>
                <textarea
                  className="form-control"
                  rows={4}
                  placeholder="Nhập lý do..."
                  value={rejectReason}
                  onChange={(e) =>
                    setRejectReason(e.target.value)
                  }
                />
              </div>
            </div>
            <div className="modal-footer">
              <button
                className="btn btn-secondary"
                onClick={() => setShowRejectModal(false)}
              >
                Huỷ
              </button>
              <button
                className="btn btn-danger"
                onClick={() => {
                  setShowRejectModal(false);
                  setSelectedProp(null);
                  setRejectReason("");
                }}
              >
                Xác nhận từ chối
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}