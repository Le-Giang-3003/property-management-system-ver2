import {
  tenantDashboard,
  properties,
} from "../../utils/mockData";
import {
  formatMoney,
  formatDate,
  getStatusBadge,
  getPropertyTypeLabel,
} from "../../utils/helpers";
import {
  FileText,
  CreditCard,
  Wrench,
  Calendar,
  Home,
  AlertCircle,
  CheckCircle,
  Search,
  Clock,
  MapPin,
  Bed,
  Bath,
  Maximize2,
  ArrowRight,
  Zap,
  TrendingUp,
  Eye,
} from "lucide-react";
import { useState } from "react";

export default function TenantDashboard() {
  const {
    activeLease,
    nextPayment,
    openMaintenanceRequests,
    upcomingBookings,
  } = tenantDashboard;
  const availableProps = properties.filter(
    (p) => p.status === "Available",
  ).slice(0, 3);

  const [selectedProperty, setSelectedProperty] = useState(null);

  return (
    <div>
      {/* Header Section */}
      <div className="mb-32">
        <div className="page-title">Dashboard Người thuê</div>
        <div className="page-desc">
          Chào mừng trở lại! Quản lý thuê nhà và theo dõi các hoạt động của bạn
        </div>
      </div>

      {/* Quick Stats */}
      <div className="stat-grid mb-32">
        <div className="stat-card stat-card-gradient">
          <div className="stat-icon-wrapper green">
            <Home size={24} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Hợp đồng hiện tại</div>
            <div className="stat-value">
              {activeLease ? "Đang thuê" : "Chưa có"}
            </div>
            <div className="stat-change">
              {activeLease
                ? `Đến ${formatDate(activeLease.endDate)}`
                : "Tìm nhà mới"}
            </div>
          </div>
        </div>

        <div className="stat-card stat-card-gradient">
          <div className="stat-icon-wrapper yellow">
            <CreditCard size={24} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Thanh toán tiếp theo</div>
            <div className="stat-value" style={{ fontSize: 20 }}>
              {nextPayment ? formatMoney(nextPayment.amount) : "—"}
            </div>
            <div className="stat-change">
              {nextPayment
                ? `Đến hạn ${formatDate(nextPayment.dueDate)}`
                : "Không có"}
            </div>
          </div>
        </div>

        <div className="stat-card stat-card-gradient">
          <div className="stat-icon-wrapper red">
            <Wrench size={24} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Yêu cầu bảo trì</div>
            <div className="stat-value">{openMaintenanceRequests}</div>
            <div className="stat-change">
              {openMaintenanceRequests > 0 ? "Đang xử lý" : "Tất cả ổn"}
            </div>
          </div>
        </div>

        <div className="stat-card stat-card-gradient">
          <div className="stat-icon-wrapper blue">
            <Calendar size={24} />
          </div>
          <div className="stat-info">
            <div className="stat-label">Lịch hẹn</div>
            <div className="stat-value">{upcomingBookings.length}</div>
            <div className="stat-change">Sắp tới</div>
          </div>
        </div>
      </div>

      {/* Quick Actions */}
      <div className="quick-actions-grid mb-32">
        <div className="quick-action-card">
          <div className="quick-action-icon">
            <Search size={20} />
          </div>
          <div className="quick-action-content">
            <div className="quick-action-title">Tìm kiếm BDS</div>
            <div className="quick-action-desc">Tìm nhà mơ ước của bạn</div>
          </div>
          <ArrowRight size={18} className="quick-action-arrow" />
        </div>

        <div className="quick-action-card">
          <div className="quick-action-icon">
            <FileText size={20} />
          </div>
          <div className="quick-action-content">
            <div className="quick-action-title">Đơn xin thuê</div>
            <div className="quick-action-desc">Xem trạng thái đơn</div>
          </div>
          <ArrowRight size={18} className="quick-action-arrow" />
        </div>

        <div className="quick-action-card">
          <div className="quick-action-icon">
            <CreditCard size={20} />
          </div>
          <div className="quick-action-content">
            <div className="quick-action-title">Thanh toán</div>
            <div className="quick-action-desc">Lịch sử giao dịch</div>
          </div>
          <ArrowRight size={18} className="quick-action-arrow" />
        </div>

        <div className="quick-action-card">
          <div className="quick-action-icon">
            <Wrench size={20} />
          </div>
          <div className="quick-action-content">
            <div className="quick-action-title">Yêu cầu sửa chữa</div>
            <div className="quick-action-desc">Báo cáo sự cố</div>
          </div>
          <ArrowRight size={18} className="quick-action-arrow" />
        </div>
      </div>

      {/* Main Content Grid */}
      <div className="dashboard-grid mb-32">
        {/* Active Lease Card */}
        <div className="dashboard-card-full">
          <div className="card-header-fancy">
            <div className="card-title-with-icon">
              <Home size={20} />
              <span>Hợp đồng đang thuê</span>
            </div>
            {activeLease && getStatusBadge(activeLease.status)}
          </div>
          {activeLease ? (
            <div className="lease-content">
              <div className="lease-property-banner">
                <div className="lease-property-info">
                  <div className="lease-property-title">
                    {activeLease.propertyTitle}
                  </div>
                  <div className="lease-property-address">
                    <MapPin size={14} /> {activeLease.propertyAddress}
                  </div>
                </div>
                <div className="lease-rent-badge">
                  <div className="lease-rent-amount">
                    {formatMoney(activeLease.monthlyRent)}
                  </div>
                  <div className="lease-rent-label">/tháng</div>
                </div>
              </div>

              <div className="lease-details-grid">
                <div className="lease-detail-item">
                  <div className="lease-detail-label">
                    <FileText size={14} />
                    Mã hợp đồng
                  </div>
                  <div className="lease-detail-value">
                    {activeLease.leaseNumber}
                  </div>
                </div>
                <div className="lease-detail-item">
                  <div className="lease-detail-label">
                    <Home size={14} />
                    Chủ nhà
                  </div>
                  <div className="lease-detail-value">
                    {activeLease.landlordName}
                  </div>
                </div>
                <div className="lease-detail-item">
                  <div className="lease-detail-label">
                    <CreditCard size={14} />
                    Tiền cọc
                  </div>
                  <div className="lease-detail-value">
                    {formatMoney(activeLease.depositAmount)}
                  </div>
                </div>
                <div className="lease-detail-item">
                  <div className="lease-detail-label">
                    <Calendar size={14} />
                    Thời hạn
                  </div>
                  <div className="lease-detail-value">
                    {formatDate(activeLease.startDate)} → {formatDate(activeLease.endDate)}
                  </div>
                </div>
              </div>

              <div className="lease-progress-section">
                <div className="lease-progress-header">
                  <span className="lease-progress-label">Tiến độ hợp đồng</span>
                  <span className="lease-progress-percent">30%</span>
                </div>
                <div className="progress-bar">
                  <div className="progress-fill" style={{ width: "30%" }} />
                </div>
                <div className="lease-progress-footer">
                  <span>{formatDate(activeLease.startDate)}</span>
                  <span>{formatDate(activeLease.endDate)}</span>
                </div>
              </div>
            </div>
          ) : (
            <div className="empty-state">
              <div className="empty-icon">🏠</div>
              <div className="empty-title">Chưa có hợp đồng</div>
              <p className="empty-desc">
                Bạn chưa có hợp đồng thuê nào đang hiệu lực
              </p>
              <button className="btn btn-primary mt-16">
                <Search size={16} />
                Tìm kiếm BDS
              </button>
            </div>
          )}
        </div>

        {/* Next Payment Card */}
        <div className="dashboard-card">
          <div className="card-header-fancy">
            <div className="card-title-with-icon">
              <CreditCard size={20} />
              <span>Thanh toán sắp tới</span>
            </div>
          </div>
          {nextPayment ? (
            <div className="payment-content">
              <div className="payment-amount-section">
                <div className="payment-label">Số tiền cần thanh toán</div>
                <div className="payment-amount">
                  {formatMoney(nextPayment.amount)}
                </div>
                <div className="payment-due">
                  <Clock size={14} />
                  Đến hạn {formatDate(nextPayment.dueDate)}
                </div>
                <div className="payment-status">
                  {getStatusBadge(nextPayment.status)}
                </div>
              </div>

              <div className="payment-description">
                <div className="payment-desc-label">Mô tả</div>
                <div className="payment-desc-value">
                  {nextPayment.description}
                </div>
              </div>

              <div className="payment-actions">
                <button className="btn btn-primary" style={{ flex: 1 }}>
                  <Zap size={16} />
                  Thanh toán ngay
                </button>
                <button className="btn btn-ghost">
                  <Eye size={16} />
                </button>
              </div>
            </div>
          ) : (
            <div className="empty-state-small">
              <div className="empty-icon">✅</div>
              <p className="empty-desc">
                Không có thanh toán nào sắp đến hạn
              </p>
            </div>
          )}
        </div>
      </div>

      {/* Upcoming Bookings */}
      {upcomingBookings.length > 0 && (
        <div className="card mb-32">
          <div className="card-header-fancy">
            <div className="card-title-with-icon">
              <Calendar size={20} />
              <span>Lịch hẹn sắp tới</span>
            </div>
            <span className="badge-count">{upcomingBookings.length}</span>
          </div>
          <div className="bookings-grid">
            {upcomingBookings.map((b) => (
              <div key={b.id} className="booking-card">
                <div className="booking-header">
                  <div className="booking-icon">
                    <Home size={16} />
                  </div>
                  {getStatusBadge(b.status)}
                </div>
                <div className="booking-title">{b.propertyTitle}</div>
                <div className="booking-details">
                  <div className="booking-detail">
                    <Calendar size={12} />
                    {formatDate(b.scheduledDate)}
                  </div>
                  <div className="booking-detail">
                    <Clock size={12} />
                    {b.startTime} - {b.endTime}
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Available Properties */}
      <div className="card">
        <div className="card-header-fancy">
          <div className="card-title-with-icon">
            <TrendingUp size={20} />
            <span>BDS có thể thuê gần đây</span>
          </div>
          <button className="btn btn-ghost btn-sm">
            Xem tất cả <ArrowRight size={14} />
          </button>
        </div>
        <div className="property-grid">
          {availableProps.map((p) => (
            <div key={p.id} className="property-card-modern">
              {p.images[0] ? (
                <img
                  className="property-image"
                  src={p.images[0].imageUrl}
                  alt={p.title}
                />
              ) : (
                <div className="property-image-placeholder">
                  <Home size={32} />
                </div>
              )}
              <div className="property-body">
                <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: 8 }}>
                  <span className="property-type-badge">
                    {getPropertyTypeLabel(p.propertyType)}
                  </span>
                  {getStatusBadge(p.status)}
                </div>
                <div className="property-title">{p.title}</div>
                <div className="property-address">
                  <MapPin size={12} /> {p.district}, {p.city}
                </div>
                <div className="property-specs">
                  <span className="spec-item">
                    <Bed size={13} /> {p.bedrooms}
                  </span>
                  <span className="spec-item">
                    <Bath size={13} /> {p.bathrooms}
                  </span>
                  <span className="spec-item">
                    <Maximize2 size={13} /> {p.area}m²
                  </span>
                </div>
                <div className="property-footer">
                  <div>
                    <div className="property-price">
                      {formatMoney(p.monthlyRent)}
                    </div>
                    <div className="property-price-sub">/tháng</div>
                  </div>
                  <button className="btn btn-primary btn-sm">
                    <Eye size={14} />
                    Chi tiết
                  </button>
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
}