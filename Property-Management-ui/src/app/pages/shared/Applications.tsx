import { useState } from "react";
import { rentalApplications } from "../../utils/mockData";
import {
  formatMoney,
  formatDate,
  getStatusBadge,
} from "../../utils/helpers";
import {
  ClipboardList,
  Eye,
  CheckCircle,
  XCircle,
  FileCheck,
  Calendar,
  DollarSign,
  CreditCard,
  Clock,
  Percent,
  Building2,
  User,
  Phone,
  Mail,
} from "lucide-react";

export default function Applications({ role = "Landlord" }) {
  const [selectedApp, setSelectedApp] = useState(null);
  const [showRejectModal, setShowRejectModal] = useState(false);
  const [rejectReason, setRejectReason] = useState("");
  const [showCreateContractModal, setShowCreateContractModal] = useState(false);
  const [contractForm, setContractForm] = useState({
    startDate: '',
    endDate: '',
    monthlyRent: '',
    deposit: '',
    paymentDay: '5',
    lateFeePercent: '1',
    terms: '',
    notes: ''
  });

  const myApps =
    role === "Tenant"
      ? rentalApplications.filter((a) => a.tenantId === 4)
      : rentalApplications;

  return (
    <div>
      <div className="flex items-center justify-between mb-20">
        <div>
          <div className="page-title">Đơn xin thuê</div>
          <div className="page-desc">
            {role === "Tenant"
              ? "Các đơn xin thuê bạn đã nộp"
              : "Đơn xin thuê đang chờ xem xét từ người thuê"}
          </div>
        </div>
      </div>

      <div
        className="stat-grid"
        style={{
          gridTemplateColumns: "repeat(3, 1fr)",
          marginBottom: 20,
        }}
      >
        <div className="stat-card">
          <div className="stat-icon yellow">
            <ClipboardList size={20} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Chờ xem xét</div>
            <div className="stat-value">
              {
                myApps.filter((a) => a.status === "Pending")
                  .length
              }
            </div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon green">
            <ClipboardList size={20} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Đã duyệt</div>
            <div className="stat-value">
              {
                myApps.filter((a) => a.status === "Approved")
                  .length
              }
            </div>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon red">
            <ClipboardList size={20} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Từ chối</div>
            <div className="stat-value">
              {
                myApps.filter((a) => a.status === "Rejected")
                  .length
              }
            </div>
          </div>
        </div>
      </div>

      <div className="card">
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>BDS</th>
                {role !== "Tenant" && <th>Người nộp</th>}
                {role !== "Tenant" && <th>Liên hệ</th>}
                <th>Ngày dọn vào</th>
                <th>Thời hạn thuê</th>
                <th>Số người</th>
                <th>Thu nhập/tháng</th>
                <th>Nghề nghiệp</th>
                <th>Trạng thái</th>
                <th>Ngày nộp</th>
                <th>Thao tác</th>
              </tr>
            </thead>
            <tbody>
              {myApps.map((a) => (
                <tr key={a.id}>
                  <td>
                    <div
                      style={{
                        display: "flex",
                        alignItems: "center",
                        gap: 8,
                      }}
                    >
                      {a.propertyThumbnail && (
                        <img
                          src={a.propertyThumbnail}
                          alt=""
                          style={{
                            width: 36,
                            height: 36,
                            borderRadius: 4,
                            objectFit: "cover",
                          }}
                        />
                      )}
                      <div>
                        <strong style={{ fontSize: 12 }}>
                          {a.propertyTitle}
                        </strong>
                      </div>
                    </div>
                  </td>
                  {role !== "Tenant" && (
                    <td>
                      <strong>{a.tenantName}</strong>
                    </td>
                  )}
                  {role !== "Tenant" && (
                    <td className="text-muted text-sm">
                      {a.tenantEmail}
                      <br />
                      {a.tenantPhone}
                    </td>
                  )}
                  <td className="text-muted">
                    {formatDate(a.moveInDate)}
                  </td>
                  <td>{a.leaseDurationMonths} tháng</td>
                  <td>{a.numberOfOccupants} người</td>
                  <td className="text-green">
                    {a.monthlyIncome
                      ? formatMoney(a.monthlyIncome)
                      : "—"}
                  </td>
                  <td className="text-muted">
                    {a.occupation || "—"}
                  </td>
                  <td>{getStatusBadge(a.status)}</td>
                  <td className="text-muted">
                    {formatDate(a.createdAt)}
                  </td>
                  <td>
                    <div style={{ display: "flex", gap: 6 }}>
                      <button
                        className="btn btn-ghost btn-sm btn-icon"
                        onClick={() => setSelectedApp(a)}
                      >
                        <Eye size={13} />
                      </button>
                      {role === "Landlord" &&
                        a.status === "Pending" && (
                          <>
                            <button
                              className="btn btn-success btn-sm btn-icon"
                              title="Duyệt"
                            >
                              <CheckCircle size={13} />
                            </button>
                            <button
                              className="btn btn-danger btn-sm btn-icon"
                              title="Từ chối"
                              onClick={() => {
                                setSelectedApp(a);
                                setShowRejectModal(true);
                              }}
                            >
                              <XCircle size={13} />
                            </button>
                          </>
                        )}
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {selectedApp && !showRejectModal && (
        <div
          className="modal-overlay"
          onClick={() => setSelectedApp(null)}
        >
          <div
            className="modal"
            style={{ maxWidth: 660 }}
            onClick={(e) => e.stopPropagation()}
          >
            <div className="modal-header">
              <span className="modal-title">
                Đơn xin thuê #{selectedApp.id}
              </span>
              <button
                className="modal-close btn btn-ghost btn-sm btn-icon"
                onClick={() => setSelectedApp(null)}
              >
                ✕
              </button>
            </div>
            <div className="modal-body">
              {/* Applicant Header - Beautiful Design */}
              <div className="applicant-header">
                {selectedApp.propertyThumbnail && (
                  <img
                    src={selectedApp.propertyThumbnail}
                    alt=""
                    className="applicant-avatar"
                  />
                )}
                <div className="applicant-info">
                  <div className="applicant-name">
                    Người nộp: {selectedApp.tenantName} • {formatDate(selectedApp.createdAt)}
                  </div>
                  <div className="applicant-status">
                    {getStatusBadge(selectedApp.status)}
                  </div>
                </div>
              </div>

              {/* Property Info */}
              <div className="detail-section">
                <div className="detail-section-title">Thông tin bất động sản:</div>
                <div className="detail-section-content">{selectedApp.propertyTitle}</div>
              </div>

              <div className="grid-2">
                <div>
                  <div className="info-row">
                    <span className="info-label">Email</span>
                    <span className="info-value">
                      {selectedApp.tenantEmail}
                    </span>
                  </div>
                  <div className="info-row">
                    <span className="info-label">
                      Điện thoại
                    </span>
                    <span className="info-value">
                      {selectedApp.tenantPhone}
                    </span>
                  </div>
                  <div className="info-row">
                    <span className="info-label">
                      Nghề nghiệp
                    </span>
                    <span className="info-value">
                      {selectedApp.occupation || "—"}
                    </span>
                  </div>
                  <div className="info-row">
                    <span className="info-label">
                      Nơi làm việc
                    </span>
                    <span className="info-value">
                      {selectedApp.employerName || "—"}
                    </span>
                  </div>
                </div>
                <div>
                  <div className="info-row">
                    <span className="info-label">
                      Thu nhập/tháng
                    </span>
                    <span className="info-value text-green fw-700">
                      {selectedApp.monthlyIncome
                        ? formatMoney(selectedApp.monthlyIncome)
                        : "—"}
                    </span>
                  </div>
                  <div className="info-row">
                    <span className="info-label">
                      Ngày dọn vào
                    </span>
                    <span className="info-value">
                      {formatDate(selectedApp.moveInDate)}
                    </span>
                  </div>
                  <div className="info-row">
                    <span className="info-label">
                      Thời hạn thuê
                    </span>
                    <span className="info-value">
                      {selectedApp.leaseDurationMonths} tháng
                    </span>
                  </div>
                  <div className="info-row">
                    <span className="info-label">
                      Số người ở
                    </span>
                    <span className="info-value">
                      {selectedApp.numberOfOccupants} người
                    </span>
                  </div>
                </div>
              </div>
              {selectedApp.message && (
                <div className="info-row">
                  <span className="info-label">Lời nhắn</span>
                  <span
                    className="info-value"
                    style={{ whiteSpace: "pre-wrap" }}
                  >
                    {selectedApp.message}
                  </span>
                </div>
              )}
              {selectedApp.rejectionReason && (
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
                    Lý do từ chối:
                  </div>
                  <div className="text-sm">
                    {selectedApp.rejectionReason}
                  </div>
                </div>
              )}
              {selectedApp.reviewedAt && (
                <div className="info-row mt-8">
                  <span className="info-label">
                    Ngày xem xét
                  </span>
                  <span className="info-value">
                    {formatDate(selectedApp.reviewedAt)}
                  </span>
                </div>
              )}
            </div>
            <div className="modal-footer">
              <button
                className="btn btn-secondary"
                onClick={() => setSelectedApp(null)}
              >
                Đóng
              </button>
              {role === "Landlord" &&
                selectedApp.status === "Pending" && (
                  <>
                    <button 
                      className="btn btn-success"
                      onClick={() => {
                        setSelectedApp(null);
                        setShowCreateContractModal(true);
                      }}
                    >
                      <CheckCircle size={16} /> Duyệt
                    </button>
                    <button
                      className="btn btn-danger"
                      onClick={() => setShowRejectModal(true)}
                    >
                      <XCircle size={16} /> Từ chối
                    </button>
                  </>
                )}
            </div>
          </div>
        </div>
      )}

      {showRejectModal && selectedApp && (
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
              <span className="modal-title">
                Từ chối đơn xin thuê
              </span>
              <button
                className="modal-close btn btn-ghost btn-sm btn-icon"
                onClick={() => setShowRejectModal(false)}
              >
                ✕
              </button>
            </div>
            <div className="modal-body">
              <div className="form-group">
                <label className="form-label">
                  Lý do từ chối *
                </label>
                <textarea
                  className="form-control"
                  rows={4}
                  placeholder="Nhập lý do từ chối..."
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
                  setSelectedApp(null);
                  setRejectReason("");
                }}
              >
                Xác nhận từ chối
              </button>
            </div>
          </div>
        </div>
      )}

      {showCreateContractModal && selectedApp && (
        <div
          className="modal-overlay"
          onClick={() => setShowCreateContractModal(false)}
        >
          <div
            className="modal contract-modal"
            style={{ maxWidth: 720 }}
            onClick={(e) => e.stopPropagation()}
          >
            <div className="modal-header">
              <span className="modal-title">
                <FileCheck size={20} /> Tạo hợp đồng thuê
              </span>
              <button
                className="modal-close btn btn-ghost btn-sm btn-icon"
                onClick={() => setShowCreateContractModal(false)}
              >
                ✕
              </button>
            </div>
            <div className="modal-body">
              {/* Property Information */}
              <div className="contract-info-section">
                <div className="contract-info-header">
                  <Building2 size={18} />
                  <span>Thông tin bất động sản</span>
                </div>
                <div className="contract-info-content">
                  <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                    {selectedApp.propertyThumbnail && (
                      <img
                        src={selectedApp.propertyThumbnail}
                        alt=""
                        style={{
                          width: 60,
                          height: 60,
                          borderRadius: 8,
                          objectFit: 'cover',
                        }}
                      />
                    )}
                    <div>
                      <div className="fw-600">{selectedApp.propertyTitle}</div>
                      <div className="text-muted text-sm">Địa chỉ: {selectedApp.propertyAddress || 'Hà Nội, Việt Nam'}</div>
                    </div>
                  </div>
                </div>
              </div>

              {/* Tenant Information */}
              <div className="contract-info-section">
                <div className="contract-info-header">
                  <User size={18} />
                  <span>Thông tin người thuê (Bên B)</span>
                </div>
                <div className="contract-info-content">
                  <div className="grid-2" style={{ gap: 8 }}>
                    <div className="contract-info-item">
                      <User size={14} className="contract-info-icon" />
                      <span>{selectedApp.tenantName}</span>
                    </div>
                    <div className="contract-info-item">
                      <Phone size={14} className="contract-info-icon" />
                      <span>{selectedApp.tenantPhone}</span>
                    </div>
                    <div className="contract-info-item">
                      <Mail size={14} className="contract-info-icon" />
                      <span>{selectedApp.tenantEmail}</span>
                    </div>
                    <div className="contract-info-item">
                      <Building2 size={14} className="contract-info-icon" />
                      <span>{selectedApp.occupation || 'Không cung cấp'}</span>
                    </div>
                  </div>
                </div>
              </div>

              {/* Landlord Information */}
              <div className="contract-info-section">
                <div className="contract-info-header">
                  <User size={18} />
                  <span>Thông tin chủ nhà (Bên A)</span>
                </div>
                <div className="contract-info-content">
                  <div className="grid-2" style={{ gap: 8 }}>
                    <div className="contract-info-item">
                      <User size={14} className="contract-info-icon" />
                      <span>Nguyễn Văn Chủ Nhà</span>
                    </div>
                    <div className="contract-info-item">
                      <Phone size={14} className="contract-info-icon" />
                      <span>0912-345-678</span>
                    </div>
                    <div className="contract-info-item">
                      <Mail size={14} className="contract-info-icon" />
                      <span>landlord@propertyme.vn</span>
                    </div>
                    <div className="contract-info-item">
                      <CreditCard size={14} className="contract-info-icon" />
                      <span>CCCD: 001234567890</span>
                    </div>
                  </div>
                </div>
              </div>

              {/* Contract Details */}
              <div className="contract-section" style={{ marginTop: 24 }}>
                <div className="contract-section-title">Chi tiết hợp đồng</div>
              </div>

              {/* Date Range */}
              <div className="form-row">
                <div className="form-group-with-icon">
                  <label className="form-label">BẮT ĐẦU THUÊ TỪ <span className="text-red">*</span></label>
                  <div className="input-icon-wrapper">
                    <Calendar size={16} className="input-icon" />
                    <input 
                      className="form-control-with-icon" 
                      type="date"
                      value={contractForm.startDate}
                      onChange={e => setContractForm({...contractForm, startDate: e.target.value})}
                    />
                  </div>
                </div>
                <div className="form-group-with-icon">
                  <label className="form-label">NGÀY KẾT THÚC <span className="text-red">*</span></label>
                  <div className="input-icon-wrapper">
                    <Calendar size={16} className="input-icon" />
                    <input 
                      className="form-control-with-icon" 
                      type="date"
                      value={contractForm.endDate}
                      onChange={e => setContractForm({...contractForm, endDate: e.target.value})}
                    />
                  </div>
                </div>
              </div>

              {/* Money Fields */}
              <div className="form-row">
                <div className="form-group-with-icon">
                  <label className="form-label">TIỀN THUÊ HÀNG THÁNG (VNĐ) <span className="text-red">*</span></label>
                  <div className="input-icon-wrapper">
                    <DollarSign size={16} className="input-icon" />
                    <input 
                      className="form-control-with-icon" 
                      type="number"
                      placeholder="0"
                      value={contractForm.monthlyRent}
                      onChange={e => setContractForm({...contractForm, monthlyRent: e.target.value})}
                    />
                  </div>
                </div>
                <div className="form-group-with-icon">
                  <label className="form-label">TIỀN CỌC (VNĐ) <span className="text-red">*</span></label>
                  <div className="input-icon-wrapper">
                    <CreditCard size={16} className="input-icon" />
                    <input 
                      className="form-control-with-icon" 
                      type="number"
                      placeholder="0"
                      value={contractForm.deposit}
                      onChange={e => setContractForm({...contractForm, deposit: e.target.value})}
                    />
                  </div>
                </div>
              </div>

              {/* Payment Terms */}
              <div className="form-row">
                <div className="form-group-with-icon">
                  <label className="form-label">NGÀY THANH TOÁN HÀNG THÁNG (VD: MÙNG 5)</label>
                  <div className="input-icon-wrapper">
                    <Clock size={16} className="input-icon" />
                    <input 
                      className="form-control-with-icon" 
                      type="number"
                      min="1"
                      max="31"
                      placeholder="5"
                      value={contractForm.paymentDay}
                      onChange={e => setContractForm({...contractForm, paymentDay: e.target.value})}
                    />
                  </div>
                </div>
                <div className="form-group-with-icon">
                  <label className="form-label">PHÍ PHẠT TRẢ CHẬM (%)</label>
                  <div className="input-icon-wrapper">
                    <Percent size={16} className="input-icon" />
                    <input 
                      className="form-control-with-icon" 
                      type="number"
                      step="0.1"
                      placeholder="1"
                      value={contractForm.lateFeePercent}
                      onChange={e => setContractForm({...contractForm, lateFeePercent: e.target.value})}
                    />
                  </div>
                </div>
              </div>

              {/* Terms */}
              <div className="form-group-with-icon">
                <label className="form-label">ĐIỀU KHOẢN HỢP ĐỒNG <span className="text-red">*</span></label>
                <textarea 
                  className="form-control" 
                  rows={4}
                  placeholder="Nội dung điều khoản hợp đồng..."
                  value={contractForm.terms}
                  onChange={e => setContractForm({...contractForm, terms: e.target.value})}
                />
              </div>

              {/* Notes */}
              <div className="form-group-with-icon">
                <label className="form-label">LƯU Ý / ĐIỀU KIỆN ĐẶC BIỆT</label>
                <textarea 
                  className="form-control" 
                  rows={3}
                  placeholder="Nội dung lưu ý hoặc điều kiện đặc biệt..."
                  value={contractForm.notes}
                  onChange={e => setContractForm({...contractForm, notes: e.target.value})}
                />
              </div>
            </div>
            <div className="modal-footer">
              <button
                className="btn btn-secondary"
                onClick={() => setShowCreateContractModal(false)}
              >
                Hủy
              </button>
              <button
                className="btn btn-success"
                onClick={() => {
                  setShowCreateContractModal(false);
                  setSelectedApp(null);
                }}
              >
                <FileCheck size={16} /> Tạo hợp đồng
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}