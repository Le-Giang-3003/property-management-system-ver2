import { useState } from "react";
import { maintenanceRequests, leases } from "../../utils/mockData";
import {
  formatMoney,
  formatDate,
  formatDateTime,
  getStatusBadge,
  getPriorityBadge,
  getCategoryLabel,
} from "../../utils/helpers";
import {
  Wrench,
  Plus,
  Eye,
  CheckCircle,
  Star,
  Search,
  Upload,
  X,
  Image as ImageIcon,
  Building2,
  UserCheck,
  XCircle,
} from "lucide-react";

export default function Maintenance({ role = "Landlord" }) {
  const [statusFilter, setStatusFilter] = useState("All");
  const [searchQuery, setSearchQuery] = useState("");
  const [showModal, setShowModal] = useState(false);
  const [selectedReq, setSelectedReq] = useState(null);
  const [showCompleteModal, setShowCompleteModal] = useState(false);
  const [showApproveModal, setShowApproveModal] = useState(false);
  const [showRejectModal, setShowRejectModal] = useState(false);
  const [rejectReason, setRejectReason] = useState("");
  const [form, setForm] = useState({
    title: "",
    description: "",
    category: "Plumbing",
    priority: "Medium",
    propertyId: "",
    desiredDate: "",
  });
  const [uploadedImages, setUploadedImages] = useState<string[]>([]);
  const [completeForm, setCompleteForm] = useState({
    resolution: "",
    actualCost: "",
  });
  const [approveForm, setApproveForm] = useState({
    technicianName: "",
    technicianPhone: "",
  });

  const handleImageUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (files) {
      const newImages: string[] = [];
      Array.from(files).forEach((file) => {
        const reader = new FileReader();
        reader.onload = (event) => {
          if (event.target?.result) {
            newImages.push(event.target.result as string);
            if (newImages.length === files.length) {
              setUploadedImages([...uploadedImages, ...newImages]);
            }
          }
        };
        reader.readAsDataURL(file);
      });
    }
  };

  const removeImage = (index: number) => {
    setUploadedImages(uploadedImages.filter((_, i) => i !== index));
  };

  // Get tenant's active leases
  const tenantLeases = role === "Tenant"
    ? leases.filter(l => l.tenantId === 4 && l.status === "Active")
    : [];

  const myRequests =
    role === "Tenant"
      ? maintenanceRequests.filter((m) => m.requestedBy === 4)
      : role === "Landlord"
        ? maintenanceRequests.filter(
            (m) => m.propertyId === 1 || m.propertyId === 2,
          )
        : maintenanceRequests;

  const filtered = myRequests.filter((m) => {
    const matchesStatus = statusFilter === "All" || m.status === statusFilter;
    const matchesSearch =
      !searchQuery ||
      m.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
      m.propertyTitle.toLowerCase().includes(searchQuery.toLowerCase());
    return matchesStatus && matchesSearch;
  });

  return (
    <div>
      <div className="flex items-center justify-between mb-20">
        <div>
          <div className="page-title">Yêu cầu bảo trì</div>
          <div className="page-desc">
            Theo dõi các yêu cầu sửa chữa, bảo trì
          </div>
        </div>
        {role === "Tenant" && (
          <button
            className="btn btn-primary"
            onClick={() => setShowModal(true)}
          >
            <Plus size={16} /> Tạo yêu cầu mới
          </button>
        )}
      </div>

      {/* Search */}
      <div className="card mb-16" style={{ padding: 16 }}>
        <div className="admin-search-wrapper" style={{ margin: 0 }}>
          <Search size={18} className="admin-search-icon" />
          <input
            className="admin-search-input"
            placeholder="Tìm theo tên yêu cầu..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
          />
        </div>
      </div>

      {/* Filter Tabs */}
      <div className="filter-bar">
        {[
          { key: "All", label: "Tất cả" },
          { key: "InProgress", label: "Đang xử lý" },
          { key: "Open", label: "Đang chờ" },
          { key: "Resolved", label: "Đã xong" },
        ].map((item) => (
          <button
            key={item.key}
            className={`btn ${statusFilter === item.key ? "btn-primary" : "btn-ghost"} btn-sm`}
            onClick={() => setStatusFilter(item.key)}
          >
            {item.label}
          </button>
        ))}
      </div>

      <div className="card">
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>TIÊU ĐỀ / BẤT ĐỘNG SẢN</th>
                <th>NGÀY YÊU CẦU</th>
                <th>MỨC ĐỘ</th>
                <th>DANH MỤC</th>
                <th>TRẠNG THÁI</th>
                <th>HÀNH ĐỘNG</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map((m) => (
                <tr key={m.id}>
                  <td>
                    <div style={{ display: "flex", flexDirection: "column", gap: 4 }}>
                      <strong style={{ fontSize: 13, color: "var(--text-primary)" }}>
                        {m.title}
                      </strong>
                      <span style={{ fontSize: 11, color: "var(--text-muted)" }}>
                        {m.propertyTitle}
                      </span>
                    </div>
                  </td>
                  <td className="text-muted">
                    {formatDateTime(m.createdAt)}
                  </td>
                  <td>{getPriorityBadge(m.priority)}</td>
                  <td>{getCategoryBadge(m.category)}</td>
                  <td>{getStatusBadge(m.status)}</td>
                  <td>
                    <div style={{ display: 'flex', gap: 4, alignItems: 'center' }}>
                      {/* Eye icon - View details */}
                      <button
                        className="btn btn-ghost btn-sm btn-icon"
                        onClick={() => setSelectedReq(m)}
                        title="Xem chi tiết"
                        style={{
                          width: 32,
                          height: 32,
                          padding: 0,
                          display: "flex",
                          alignItems: "center",
                          justifyContent: "center",
                        }}
                      >
                        <Eye size={16} />
                      </button>
                      
                      {/* Landlord actions */}
                      {role === "Landlord" && m.status === "Open" && (
                        <>
                          {/* Approve - Đã kêu thợ */}
                          <button
                            className="btn btn-ghost btn-sm btn-icon"
                            onClick={() => {
                              setSelectedReq(m);
                              setShowApproveModal(true);
                            }}
                            title="Đã kêu thợ"
                            style={{
                              width: 32,
                              height: 32,
                              padding: 0,
                              display: "flex",
                              alignItems: "center",
                              justifyContent: "center",
                              color: "var(--success)",
                            }}
                          >
                            <UserCheck size={16} />
                          </button>
                          
                          {/* Reject - Từ chối */}
                          <button
                            className="btn btn-ghost btn-sm btn-icon"
                            onClick={() => {
                              setSelectedReq(m);
                              setShowRejectModal(true);
                            }}
                            title="Từ chối"
                            style={{
                              width: 32,
                              height: 32,
                              padding: 0,
                              display: "flex",
                              alignItems: "center",
                              justifyContent: "center",
                              color: "var(--danger)",
                            }}
                          >
                            <XCircle size={16} />
                          </button>
                        </>
                      )}
                      
                      {/* Complete action for InProgress status */}
                      {role === "Landlord" && m.status === "InProgress" && (
                        <button
                          className="btn btn-ghost btn-sm btn-icon"
                          onClick={() => {
                            setSelectedReq(m);
                            setShowCompleteModal(true);
                          }}
                          title="Hoàn thành"
                          style={{
                            width: 32,
                            height: 32,
                            padding: 0,
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            color: "var(--success)",
                          }}
                        >
                          <CheckCircle size={16} />
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

      {/* Create Request Modal (Tenant) */}
      {showModal && (
        <div
          className="modal-overlay"
          onClick={() => setShowModal(false)}
        >
          <div
            className="modal"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="modal-header">
              <span className="modal-title">
                Gửi yêu cầu bảo trì mới
              </span>
              <button
                className="modal-close btn btn-ghost btn-sm btn-icon"
                onClick={() => setShowModal(false)}
              >
                ✕
              </button>
            </div>
            <div className="modal-body">
              <div className="form-group">
                <label className="form-label">Chọn bất động sản *</label>
                <select
                  className="form-control"
                  value={form.propertyId}
                  onChange={(e) =>
                    setForm({ ...form, propertyId: e.target.value })
                  }
                >
                  <option value="">-- Chọn BDS đang thuê --</option>
                  {tenantLeases.map((lease) => (
                    <option key={lease.id} value={lease.propertyId}>
                      {lease.propertyTitle} - {lease.propertyAddress}
                    </option>
                  ))}
                </select>
                {tenantLeases.length === 0 && (
                  <div style={{ 
                    marginTop: 8, 
                    fontSize: 12, 
                    color: 'var(--danger)',
                    display: 'flex',
                    alignItems: 'center',
                    gap: 6 
                  }}>
                    <Building2 size={14} />
                    <span>Bạn chưa có hợp đồng thuê nào đang hiệu lực</span>
                  </div>
                )}
              </div>
              <div className="form-group">
                <label className="form-label">Tiêu đề *</label>
                <input
                  className="form-control"
                  placeholder="Mô tả ngắn gọn sự cố..."
                  value={form.title}
                  onChange={(e) =>
                    setForm({ ...form, title: e.target.value })
                  }
                />
              </div>
              <div className="form-row">
                <div className="form-group">
                  <label className="form-label">Danh mục</label>
                  <select
                    className="form-control"
                    value={form.category}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        category: e.target.value,
                      })
                    }
                  >
                    {[
                      "Plumbing",
                      "Electrical",
                      "Painting",
                      "Appliance",
                      "Structural",
                      "Cleaning",
                      "Other",
                    ].map((c) => (
                      <option key={c} value={c}>
                        {getCategoryLabel(c)}
                      </option>
                    ))}
                  </select>
                </div>
                <div className="form-group">
                  <label className="form-label">
                    Mức độ ưu tiên
                  </label>
                  <select
                    className="form-control"
                    value={form.priority}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        priority: e.target.value,
                      })
                    }
                  >
                    {["Low", "Medium", "High", "Critical"].map(
                      (p) => (
                        <option key={p} value={p}>
                          {p === "Low"
                            ? "Thấp"
                            : p === "Medium"
                              ? "Trung bình"
                              : p === "High"
                                ? "Cao"
                                : "Khẩn cấp"}
                        </option>
                      ),
                    )}
                  </select>
                </div>
              </div>
              <div className="form-group">
                <label className="form-label">
                  Mô tả chi tiết *
                </label>
                <textarea
                  className="form-control"
                  rows={5}
                  placeholder="Mô tả chi tiết sự cố, vị trí, thời gian phát hiện..."
                  value={form.description}
                  onChange={(e) =>
                    setForm({
                      ...form,
                      description: e.target.value,
                    })
                  }
                />
              </div>
              
              <div className="form-group">
                <label className="form-label">
                  Ngày mong muốn thợ đến sửa
                </label>
                <input
                  type="date"
                  className="form-control"
                  value={form.desiredDate}
                  onChange={(e) =>
                    setForm({
                      ...form,
                      desiredDate: e.target.value,
                    })
                  }
                  min={new Date().toISOString().split('T')[0]}
                />
              </div>
              
              {/* Upload Images Section */}
              <div className="form-group">
                <label className="form-label">Hình ảnh minh họa</label>
                <div
                  style={{
                    border: "2px dashed var(--border)",
                    borderRadius: 8,
                    padding: 24,
                    textAlign: "center",
                    background: "var(--bg-hover)",
                    cursor: "pointer",
                    position: "relative",
                  }}
                  onClick={() => document.getElementById("file-upload")?.click()}
                >
                  <input
                    id="file-upload"
                    type="file"
                    multiple
                    accept="image/*"
                    onChange={handleImageUpload}
                    style={{ display: "none" }}
                  />
                  <Upload
                    size={32}
                    style={{
                      color: "var(--text-muted)",
                      margin: "0 auto 12px",
                    }}
                  />
                  <div style={{ color: "var(--text-primary)", marginBottom: 4 }}>
                    Click để tải ảnh lên
                  </div>
                  <div style={{ color: "var(--text-muted)", fontSize: 12 }}>
                    Hỗ trợ JPG, PNG (Tối đa 5MB mỗi ảnh)
                  </div>
                </div>
                
                {/* Image Preview Grid */}
                {uploadedImages.length > 0 && (
                  <div
                    style={{
                      display: "grid",
                      gridTemplateColumns: "repeat(4, 1fr)",
                      gap: 12,
                      marginTop: 16,
                    }}
                  >
                    {uploadedImages.map((image, index) => (
                      <div
                        key={index}
                        style={{
                          position: "relative",
                          borderRadius: 8,
                          overflow: "hidden",
                          paddingTop: "100%",
                        }}
                      >
                        <img
                          src={image}
                          alt={`Upload ${index + 1}`}
                          style={{
                            position: "absolute",
                            top: 0,
                            left: 0,
                            width: "100%",
                            height: "100%",
                            objectFit: "cover",
                          }}
                        />
                        <button
                          onClick={(e) => {
                            e.stopPropagation();
                            removeImage(index);
                          }}
                          style={{
                            position: "absolute",
                            top: 6,
                            right: 6,
                            width: 24,
                            height: 24,
                            borderRadius: "50%",
                            background: "rgba(239, 68, 68, 0.9)",
                            border: "none",
                            color: "white",
                            cursor: "pointer",
                            display: "flex",
                            alignItems: "center",
                            justifyContent: "center",
                            padding: 0,
                          }}
                        >
                          <X size={14} />
                        </button>
                      </div>
                    ))}
                  </div>
                )}
              </div>
            </div>
            <div className="modal-footer">
              <button
                className="btn btn-secondary"
                onClick={() => {
                  setShowModal(false);
                  setUploadedImages([]);
                  setForm({
                    title: "",
                    description: "",
                    category: "Plumbing",
                    priority: "Medium",
                    propertyId: "",
                    desiredDate: "",
                  });
                }}
              >
                Hủy
              </button>
              <button
                className="btn btn-primary"
                onClick={() => {
                  // Submit logic here
                  setShowModal(false);
                  setUploadedImages([]);
                  setForm({
                    title: "",
                    description: "",
                    category: "Plumbing",
                    priority: "Medium",
                    propertyId: "",
                    desiredDate: "",
                  });
                }}
              >
                Gửi yêu cầu
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Detail Modal */}
      {selectedReq && !showCompleteModal && (
        <div
          className="modal-overlay"
          onClick={() => setSelectedReq(null)}
        >
          <div
            className="modal"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="modal-header">
              <span className="modal-title">
                #{selectedReq.id} - {selectedReq.title}
              </span>
              <button
                className="modal-close btn btn-ghost btn-sm btn-icon"
                onClick={() => setSelectedReq(null)}
              >
                ✕
              </button>
            </div>
            <div className="modal-body">
              <div
                style={{
                  display: "flex",
                  gap: 8,
                  marginBottom: 16,
                }}
              >
                {getStatusBadge(selectedReq.status)}
                {getPriorityBadge(selectedReq.priority)}
                <span className="badge badge-gray">
                  {getCategoryLabel(selectedReq.category)}
                </span>
              </div>
              <div className="info-row">
                <span className="info-label">BDS</span>
                <span className="info-value">
                  {selectedReq.propertyTitle}
                </span>
              </div>
              <div className="info-row">
                <span className="info-label">
                  Người yêu cầu
                </span>
                <span className="info-value">
                  {selectedReq.requesterName}
                </span>
              </div>
              <div className="info-row">
                <span className="info-label">Mô tả</span>
                <span
                  className="info-value"
                  style={{ whiteSpace: "pre-wrap" }}
                >
                  {selectedReq.description}
                </span>
              </div>
              {selectedReq.scheduledDate && (
                <div className="info-row">
                  <span className="info-label">Ngày hẹn</span>
                  <span className="info-value">
                    {formatDate(selectedReq.scheduledDate)}
                  </span>
                </div>
              )}
              {selectedReq.assignedToName && (
                <div className="info-row">
                  <span className="info-label">Tên thợ</span>
                  <span className="info-value">
                    {selectedReq.assignedToName}
                  </span>
                </div>
              )}
              <div className="info-row">
                <span className="info-label">Ngày tạo</span>
                <span className="info-value">
                  {formatDate(selectedReq.createdAt)}
                </span>
              </div>
              
              {/* Display Images */}
              {uploadedImages && uploadedImages.length > 0 && (
                <div style={{ marginTop: 16 }}>
                  <div className="info-label" style={{ marginBottom: 12 }}>
                    Hình ảnh đính kèm
                  </div>
                  <div
                    style={{
                      display: "grid",
                      gridTemplateColumns: "repeat(3, 1fr)",
                      gap: 12,
                    }}
                  >
                    {uploadedImages.map((image, index) => (
                      <div
                        key={index}
                        style={{
                          position: "relative",
                          borderRadius: 8,
                          overflow: "hidden",
                          paddingTop: "100%",
                          border: "1px solid var(--border)",
                        }}
                      >
                        <img
                          src={image}
                          alt={`Attached ${index + 1}`}
                          style={{
                            position: "absolute",
                            top: 0,
                            left: 0,
                            width: "100%",
                            height: "100%",
                            objectFit: "cover",
                          }}
                        />
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div>
            <div className="modal-footer">
              {role === "Landlord" && selectedReq.status === "Open" && (
                <>
                  <button
                    className="btn btn-danger"
                    onClick={() => {
                      setShowRejectModal(true);
                    }}
                  >
                    Từ chối
                  </button>
                  <button
                    className="btn btn-success"
                    onClick={() => {
                      setShowApproveModal(true);
                    }}
                  >
                    Phê duyệt
                  </button>
                </>
              )}
              {role === "Landlord" && selectedReq.status === "InProgress" && (
                <button
                  className="btn btn-success"
                  onClick={() => {
                    setShowCompleteModal(true);
                  }}
                >
                  <CheckCircle size={16} /> Hoàn thành
                </button>
              )}
              <button
                className="btn btn-secondary"
                onClick={() => setSelectedReq(null)}
              >
                Đóng
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Complete Modal */}
      {showCompleteModal && selectedReq && (
        <div
          className="modal-overlay"
          onClick={() => setShowCompleteModal(false)}
        >
          <div
            className="modal"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="modal-header">
              <span className="modal-title">
                Hoàn thành yêu cầu bảo trì
              </span>
              <button
                className="modal-close btn btn-ghost btn-sm btn-icon"
                onClick={() => setShowCompleteModal(false)}
              >
                ✕
              </button>
            </div>
            <div className="modal-body">
              <div className="form-group">
                <label className="form-label">
                  Kết quả xử lý *
                </label>
                <textarea
                  className="form-control"
                  rows={4}
                  placeholder="Mô tả những gì đã được thực hiện..."
                  value={completeForm.resolution}
                  onChange={(e) =>
                    setCompleteForm({
                      ...completeForm,
                      resolution: e.target.value,
                    })
                  }
                />
              </div>
              <div className="form-group">
                <label className="form-label">
                  Chi phí thực tế (VND)
                </label>
                <input
                  className="form-control"
                  type="number"
                  placeholder="0"
                  value={completeForm.actualCost}
                  onChange={(e) =>
                    setCompleteForm({
                      ...completeForm,
                      actualCost: e.target.value,
                    })
                  }
                />
              </div>
            </div>
            <div className="modal-footer">
              <button
                className="btn btn-secondary"
                onClick={() => setShowCompleteModal(false)}
              >
                Huỷ
              </button>
              <button
                className="btn btn-success"
                onClick={() => {
                  setShowCompleteModal(false);
                  setSelectedReq(null);
                }}
              >
                ✓ Xác nhận hoàn thành
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Approve Modal */}
      {showApproveModal && selectedReq && (
        <div
          className="modal-overlay"
          onClick={() => setShowApproveModal(false)}
        >
          <div
            className="modal"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="modal-header">
              <span className="modal-title">
                Phê duyệt yêu cầu bảo trì
              </span>
              <button
                className="modal-close btn btn-ghost btn-sm btn-icon"
                onClick={() => setShowApproveModal(false)}
              >
                ✕
              </button>
            </div>
            <div className="modal-body">
              <p style={{ color: "var(--text-primary)", marginBottom: 16 }}>
                Bạn có chắc chắn muốn phê duyệt yêu cầu bảo trì "<strong>{selectedReq.title}</strong>" không?
              </p>
              <p style={{ color: "var(--text-muted)", fontSize: 14 }}>
                Sau khi phê duyệt, yêu cầu sẽ được chuyển sang trạng thái "Đang xử lý".
              </p>
              <div className="form-group">
                <label className="form-label">
                  Tên kỹ thuật viên *
                </label>
                <input
                  className="form-control"
                  placeholder="Nhập tên kỹ thuật viên..."
                  value={approveForm.technicianName}
                  onChange={(e) =>
                    setApproveForm({
                      ...approveForm,
                      technicianName: e.target.value,
                    })
                  }
                />
              </div>
              <div className="form-group">
                <label className="form-label">
                  Số điện thoại kỹ thuật viên *
                </label>
                <input
                  className="form-control"
                  placeholder="Nhập số điện thoại..."
                  value={approveForm.technicianPhone}
                  onChange={(e) =>
                    setApproveForm({
                      ...approveForm,
                      technicianPhone: e.target.value,
                    })
                  }
                />
              </div>
            </div>
            <div className="modal-footer">
              <button
                className="btn btn-secondary"
                onClick={() => setShowApproveModal(false)}
              >
                Hủy
              </button>
              <button
                className="btn btn-success"
                onClick={() => {
                  // Approve logic here
                  setShowApproveModal(false);
                  setSelectedReq(null);
                }}
              >
                ✓ Phê duyệt
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Reject Modal */}
      {showRejectModal && selectedReq && (
        <div
          className="modal-overlay"
          onClick={() => setShowRejectModal(false)}
        >
          <div
            className="modal"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="modal-header">
              <span className="modal-title">
                Từ chối yêu cầu bảo trì
              </span>
              <button
                className="modal-close btn btn-ghost btn-sm btn-icon"
                onClick={() => setShowRejectModal(false)}
              >
                ✕
              </button>
            </div>
            <div className="modal-body">
              <p style={{ color: "var(--text-primary)", marginBottom: 16 }}>
                Bạn đang từ chối yêu cầu bảo trì "<strong>{selectedReq.title}</strong>"
              </p>
              <div className="form-group">
                <label className="form-label">
                  Lý do từ chối *
                </label>
                <textarea
                  className="form-control"
                  rows={4}
                  placeholder="Nhập lý do từ chối yêu cầu này..."
                  value={rejectReason}
                  onChange={(e) => setRejectReason(e.target.value)}
                />
              </div>
            </div>
            <div className="modal-footer">
              <button
                className="btn btn-secondary"
                onClick={() => {
                  setShowRejectModal(false);
                  setRejectReason("");
                }}
              >
                Hủy
              </button>
              <button
                className="btn btn-danger"
                onClick={() => {
                  // Reject logic here
                  setShowRejectModal(false);
                  setSelectedReq(null);
                  setRejectReason("");
                }}
              >
                Từ chối
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

// Category Badge Helper
function getCategoryBadge(category: string) {
  const badges = {
    Plumbing: <span className="badge badge-purple">Plumbing</span>,
    Electrical: <span className="badge badge-info">Electrical</span>,
    Painting: <span className="badge badge-warning">Painting</span>,
    Appliance: <span className="badge badge-gray">Appliance</span>,
    Structural: <span className="badge badge-danger">Structural</span>,
    Cleaning: <span className="badge badge-success">Cleaning</span>,
    Other: <span className="badge badge-gray">Other</span>,
  };
  return badges[category] || <span className="badge badge-gray">{category}</span>;
}