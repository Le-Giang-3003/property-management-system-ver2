import { useState } from "react";
import { bookings } from "../../utils/mockData";
import {
  formatDate,
  getStatusBadge,
} from "../../utils/helpers";
import {
  Calendar,
  Plus,
  Eye,
  CheckCircle,
  XCircle,
} from "lucide-react";

export default function Bookings({ role = "Landlord" }) {
  const [selectedBooking, setSelectedBooking] = useState(null);
  const [showCreateModal, setShowCreateModal] = useState(false);
  const [createForm, setCreateForm] = useState({
    propertyId: "",
    scheduledDate: "",
    startTime: "",
    message: "",
  });

  const myBookings =
    role === "Tenant"
      ? bookings.filter((b) => b.tenantId === 5)
      : bookings;

  return (
    <div>
      <div className="flex items-center justify-between mb-20">
        <div>
          <div className="page-title">Lịch xem nhà</div>
          <div className="page-desc">
            {role === "Tenant"
              ? "Quản lý lịch hẹn xem nhà của bạn"
              : "Quản lý lịch hẹn xem nhà từ người thuê"}
          </div>
        </div>
        {role === "Tenant" && (
          <button
            className="btn btn-primary"
            onClick={() => setShowCreateModal(true)}
          >
            <Plus size={16} /> Đặt lịch xem nhà
          </button>
        )}
      </div>

      <div
        className="stat-grid"
        style={{
          gridTemplateColumns: "repeat(4, 1fr)",
          marginBottom: 20,
        }}
      >
        <div className="stat-card">
          <div className="stat-icon yellow">
            <Calendar size={20} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Chờ xác nhận</div>
            <div className="stat-value">
              {
                myBookings.filter((b) => b.status === "Pending")
                  .length
              }
            </div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon green">
            <Calendar size={20} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Đã xác nhận</div>
            <div className="stat-value">
              {
                myBookings.filter(
                  (b) => b.status === "Confirmed",
                ).length
              }
            </div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon red">
            <Calendar size={20} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Đã huỷ</div>
            <div className="stat-value">
              {
                myBookings.filter(
                  (b) => b.status === "Cancelled",
                ).length
              }
            </div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon blue">
            <Calendar size={20} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Tổng lịch hẹn</div>
            <div className="stat-value">
              {myBookings.length}
            </div>
          </div>
        </div>
      </div>

      <div
        className="property-grid"
        style={{
          gridTemplateColumns:
            "repeat(auto-fill, minmax(300px, 1fr))",
        }}
      >
        {myBookings.map((b) => (
          <div
            key={b.id}
            className="card"
            style={{
              cursor: "pointer",
              transition: "all 0.2s",
            }}
            onClick={() => setSelectedBooking(b)}
          >
            <div
              style={{
                display: "flex",
                gap: 12,
                marginBottom: 12,
              }}
            >
              {b.propertyThumbnail ? (
                <img
                  src={b.propertyThumbnail}
                  alt=""
                  style={{
                    width: 60,
                    height: 60,
                    borderRadius: 8,
                    objectFit: "cover",
                    flexShrink: 0,
                  }}
                />
              ) : (
                <div
                  style={{
                    width: 60,
                    height: 60,
                    borderRadius: 8,
                    background: "var(--bg-input)",
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    fontSize: 24,
                  }}
                >
                  🏠
                </div>
              )}
              <div style={{ flex: 1, minWidth: 0 }}>
                <div
                  className="fw-700"
                  style={{
                    fontSize: 14,
                    whiteSpace: "nowrap",
                    overflow: "hidden",
                    textOverflow: "ellipsis",
                  }}
                >
                  {b.propertyTitle}
                </div>
                <div className="text-muted text-sm">
                  {role === "Tenant"
                    ? `Chủ nhà: ${b.landlordName}`
                    : `Người thuê: ${b.tenantName}`}
                </div>
              </div>
            </div>

            <div
              style={{
                display: "grid",
                gridTemplateColumns: "1fr 1fr",
                gap: 8,
                marginBottom: 12,
                padding: "10px",
                background: "var(--bg-primary)",
                borderRadius: 8,
              }}
            >
              <div>
                <div className="text-sm text-muted">
                  📅 Ngày xem
                </div>
                <div
                  className="fw-600"
                  style={{ fontSize: 13 }}
                >
                  {formatDate(b.scheduledDate)}
                </div>
              </div>
              <div>
                <div className="text-sm text-muted">⏰ Giờ</div>
                <div
                  className="fw-600"
                  style={{ fontSize: 13 }}
                >
                  {b.startTime} - {b.endTime}
                </div>
              </div>
            </div>

            <div
              style={{
                display: "flex",
                alignItems: "center",
                justifyContent: "space-between",
              }}
            >
              {getStatusBadge(b.status)}
              <div
                style={{ display: "flex", gap: 6 }}
                onClick={(e) => e.stopPropagation()}
              >
                {role === "Landlord" &&
                  b.status === "Pending" && (
                    <>
                      <button
                        className="btn btn-success btn-sm btn-icon"
                        title="Xác nhận"
                      >
                        <CheckCircle size={13} />
                      </button>
                      <button
                        className="btn btn-danger btn-sm btn-icon"
                        title="Từ chối"
                      >
                        <XCircle size={13} />
                      </button>
                    </>
                  )}
                {b.status !== "Cancelled" && (
                  <button
                    className="btn btn-danger btn-sm"
                    onClick={() => {}}
                  >
                    Huỷ
                  </button>
                )}
              </div>
            </div>
          </div>
        ))}
      </div>

      {selectedBooking && (
        <div
          className="modal-overlay"
          onClick={() => setSelectedBooking(null)}
        >
          <div
            className="modal"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="modal-header">
              <span className="modal-title">
                Chi tiết lịch hẹn #{selectedBooking.id}
              </span>
              <button
                className="modal-close btn btn-ghost btn-sm btn-icon"
                onClick={() => setSelectedBooking(null)}
              >
                ✕
              </button>
            </div>
            <div className="modal-body">
              <div className="info-row">
                <span className="info-label">BDS</span>
                <span className="info-value">
                  {selectedBooking.propertyTitle}
                </span>
              </div>
              <div className="info-row">
                <span className="info-label">Người thuê</span>
                <span className="info-value">
                  {selectedBooking.tenantName}
                </span>
              </div>
              <div className="info-row">
                <span className="info-label">Chủ nhà</span>
                <span className="info-value">
                  {selectedBooking.landlordName}
                </span>
              </div>
              <div className="info-row">
                <span className="info-label">Trạng thái</span>
                <span className="info-value">
                  {getStatusBadge(selectedBooking.status)}
                </span>
              </div>
              <div className="info-row">
                <span className="info-label">Ngày xem</span>
                <span className="info-value">
                  {formatDate(selectedBooking.scheduledDate)}
                </span>
              </div>
              <div className="info-row">
                <span className="info-label">Giờ</span>
                <span className="info-value">
                  {selectedBooking.startTime} -{" "}
                  {selectedBooking.endTime}
                </span>
              </div>
              {selectedBooking.message && (
                <div className="info-row">
                  <span className="info-label">Lời nhắn</span>
                  <span className="info-value">
                    {selectedBooking.message}
                  </span>
                </div>
              )}
              {selectedBooking.confirmationNotes && (
                <div className="info-row">
                  <span className="info-label">
                    Ghi chú xác nhận
                  </span>
                  <span className="info-value">
                    {selectedBooking.confirmationNotes}
                  </span>
                </div>
              )}
              {selectedBooking.cancellationReason && (
                <div
                  style={{
                    background: "var(--danger-bg)",
                    border: "1px solid var(--danger)",
                    borderRadius: 8,
                    padding: 12,
                    marginTop: 12,
                  }}
                >
                  <div className="fw-600 text-red mb-4">
                    Lý do huỷ:
                  </div>
                  <div className="text-sm">
                    {selectedBooking.cancellationReason}
                  </div>
                </div>
              )}
              <div className="info-row">
                <span className="info-label">Ngày tạo</span>
                <span className="info-value">
                  {formatDate(selectedBooking.createdAt)}
                </span>
              </div>
            </div>
            <div className="modal-footer">
              <button
                className="btn btn-secondary"
                onClick={() => setSelectedBooking(null)}
              >
                Đóng
              </button>
              {role === "Landlord" &&
                selectedBooking.status === "Pending" && (
                  <>
                    <button className="btn btn-success">
                      ✓ Xác nhận
                    </button>
                    <button className="btn btn-danger">
                      ✗ Từ chối
                    </button>
                  </>
                )}
            </div>
          </div>
        </div>
      )}

      {showCreateModal && (
        <div
          className="modal-overlay"
          onClick={() => setShowCreateModal(false)}
        >
          <div
            className="modal"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="modal-header">
              <span className="modal-title">
                Đặt lịch xem nhà
              </span>
              <button
                className="modal-close btn btn-ghost btn-sm btn-icon"
                onClick={() => setShowCreateModal(false)}
              >
                ✕
              </button>
            </div>
            <div className="modal-body">
              <div className="form-group">
                <label className="form-label">Ngày xem *</label>
                <input
                  className="form-control"
                  type="date"
                  value={createForm.scheduledDate}
                  onChange={(e) =>
                    setCreateForm({
                      ...createForm,
                      scheduledDate: e.target.value,
                    })
                  }
                />
              </div>
              <div className="form-group">
                <label className="form-label">
                  Giờ bắt đầu *
                </label>
                <input
                  className="form-control"
                  type="time"
                  value={createForm.startTime}
                  onChange={(e) =>
                    setCreateForm({
                      ...createForm,
                      startTime: e.target.value,
                    })
                  }
                />
              </div>
              <div className="form-group">
                <label className="form-label">
                  Lời nhắn cho chủ nhà
                </label>
                <textarea
                  className="form-control"
                  rows={3}
                  placeholder="Nhắn gửi đến chủ nhà..."
                  value={createForm.message}
                  onChange={(e) =>
                    setCreateForm({
                      ...createForm,
                      message: e.target.value,
                    })
                  }
                />
              </div>
            </div>
            <div className="modal-footer">
              <button
                className="btn btn-secondary"
                onClick={() => setShowCreateModal(false)}
              >
                Huỷ
              </button>
              <button
                className="btn btn-primary"
                onClick={() => setShowCreateModal(false)}
              >
                Đặt lịch
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}