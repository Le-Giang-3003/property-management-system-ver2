// Utility helper functions

export const formatMoney = (amount, currency = "VND") => {
  if (!amount && amount !== 0) return "—";
  return new Intl.NumberFormat("vi-VN", {
    style: "currency",
    currency,
  }).format(amount);
};

export const formatDate = (dateStr) => {
  if (!dateStr) return "—";
  return new Date(dateStr).toLocaleDateString("vi-VN");
};

export const formatDateTime = (dateStr) => {
  if (!dateStr) return "—";
  return new Date(dateStr).toLocaleString("vi-VN");
};

export const getMonthLabel = (year, month) => {
  return `${String(month).padStart(2, "0")}/${year}`;
};

export const getStatusBadge = (status) => {
  const map = {
    // Property
    Available: { label: "Đăng đăng", cls: "badge-success" },
    Rented: { label: "Đã thuê", cls: "badge-gray" },
    Pending: { label: "Chờ duyệt", cls: "badge-warning" },
    Draft: { label: "Tạm ẩn", cls: "badge-gray" },
    Rejected: { label: "Từ chối", cls: "badge-danger" },
    Inactive: { label: "Bị khoá", cls: "badge-gray" },
    // Lease
    Active: { label: "Hiệu lực", cls: "badge-success" },
    Expired: { label: "Hết hạn", cls: "badge-gray" },
    Terminated: { label: "Đã chấm dứt", cls: "badge-danger" },
    // Payment
    Completed: { label: "Đã thanh toán", cls: "badge-success" },
    Overdue: { label: "Quá hạn", cls: "badge-danger" },
    Cancelled: { label: "Đã huỷ", cls: "badge-gray" },
    Refunded: { label: "Đã hoàn tiền", cls: "badge-info" },
    // Maintenance
    Open: { label: "Mở", cls: "badge-warning" },
    InProgress: { label: "Đang xử lý", cls: "badge-info" },
    Resolved: { label: "Đã giải quyết", cls: "badge-success" },
    // Application
    Approved: { label: "Đã duyệt", cls: "badge-success" },
    // Booking
    Confirmed: { label: "Đã xác nhận", cls: "badge-success" },
  };
  const item = map[status] || {
    label: status,
    cls: "badge-gray",
  };
  return (
    <span className={`badge ${item.cls}`}>{item.label}</span>
  );
};

export const getPriorityBadge = (priority) => {
  const map = {
    Low: { label: "Thấp", cls: "badge-success" },
    Medium: { label: "Trung bình", cls: "badge-warning" },
    High: { label: "Cao", cls: "badge-danger" },
    Critical: { label: "Khẩn cấp", cls: "badge-danger" },
  };
  const item = map[priority] || {
    label: priority,
    cls: "badge-gray",
  };
  return (
    <span className={`badge ${item.cls}`}>{item.label}</span>
  );
};

export const getCategoryLabel = (cat) => {
  const map = {
    Plumbing: "Ống nước",
    Electrical: "Điện",
    Painting: "Sơn",
    Appliance: "Thiết bị",
    Structural: "Kết cấu",
    Cleaning: "Vệ sinh",
    Other: "Khác",
  };
  return map[cat] || cat;
};

export const getPropertyTypeLabel = (type) => {
  const map = {
    Apartment: "Căn hộ",
    House: "Nhà phố",
    Room: "Phòng trọ",
    Villa: "Biệt thự",
    Commercial: "Thương mại",
  };
  return map[type] || type;
};

export const getPaymentTypeLabel = (type) => {
  const map = {
    MonthlyRent: "Tiền thuê",
    Deposit: "Đặt cọc",
    LateFee: "Phí trễ",
    Maintenance: "Bảo trì",
    Refund: "Hoàn tiền",
  };
  return map[type] || type;
};

export const getRoleName = (role, isTenant, isLandlord) => {
  if (role === "Admin") return "Quản trị viên";
  if (isLandlord && isTenant) return "Chủ nhà & Người thuê";
  if (isLandlord) return "Chủ nhà";
  if (isTenant) return "Người thuê";
  return "Thành viên";
};