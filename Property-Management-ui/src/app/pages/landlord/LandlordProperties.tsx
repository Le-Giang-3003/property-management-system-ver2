import { useState } from "react";
import { properties } from "../../utils/mockData";
import {
  formatMoney,
  formatDate,
  getStatusBadge,
  getPropertyTypeLabel,
} from "../../utils/helpers";
import {
  Plus,
  Edit,
  Trash,
  Eye,
  Home,
  Bed,
  Bath,
  Maximize2,
  Copy,
  Search,
  FileText,
  Building2,
  MapPin,
  DollarSign,
  Layers,
  Dog,
  Cigarette,
  Image as ImageIcon,
  Upload,
  Star,
  ChevronLeft,
  ChevronRight,
  X,
} from "lucide-react";
import useEmblaCarousel from "embla-carousel-react";
import PropertyDetailModal from "../../components/PropertyDetailModal";
import ImageManagementModal from "../../components/ImageManagementModal";

const myProps = properties.filter((p) => p.landlordId === 2);

export default function LandlordProperties() {
  const [statusFilter, setStatusFilter] = useState("All");
  const [searchQuery, setSearchQuery] = useState("");
  const [showModal, setShowModal] = useState(false);
  const [selectedProp, setSelectedProp] = useState(null);
  const [viewDetail, setViewDetail] = useState(null);
  const [imageManagementProp, setImageManagementProp] = useState(null);
  const [form, setForm] = useState({
    title: "",
    description: "",
    propertyType: "Apartment",
    address: "",
    city: "TP.HCM",
    district: "",
    ward: "",
    area: "",
    bedrooms: 1,
    bathrooms: 1,
    floors: "",
    monthlyRent: "",
    depositAmount: "",
    amenities: "",
    allowPets: false,
    allowSmoking: false,
    maxOccupants: "",
  });

  const filteredProps = myProps.filter((p) => {
    const matchesStatus =
      statusFilter === "All" ||
      (statusFilter === "Available" && p.status === "Available") ||
      (statusFilter === "Rented" && p.status === "Rented") ||
      (statusFilter === "Pending" && p.status === "Pending") ||
      (statusFilter === "Draft" && p.status === "Draft") ||
      (statusFilter === "Inactive" && p.status === "Inactive");

    const matchesSearch =
      !searchQuery ||
      p.title.toLowerCase().includes(searchQuery.toLowerCase()) ||
      p.address.toLowerCase().includes(searchQuery.toLowerCase());

    return matchesStatus && matchesSearch;
  });

  const getStatusColor = (status: string) => {
    switch (status) {
      case "Available":
        return "#10b981"; // green
      case "Rented":
        return "#6b7280"; // gray
      case "Pending":
        return "#f59e0b"; // yellow
      case "Draft":
        return "#6b7280"; // gray
      case "Rejected":
        return "#ef4444"; // red
      case "Inactive":
        return "#6b7280"; // gray
      default:
        return "#6b7280";
    }
  };

  const getStatusLabel = (status: string) => {
    switch (status) {
      case "Available":
        return "Đang đăng";
      case "Rented":
        return "Đã thuê";
      case "Pending":
        return "Chờ duyệt";
      case "Draft":
        return "Tạm ẩn";
      case "Rejected":
        return "Từ chối";
      case "Inactive":
        return "Bị khoá";
      default:
        return status;
    }
  };

  const openCreate = () => {
    setSelectedProp(null);
    setForm({
      title: "",
      description: "",
      propertyType: "Apartment",
      address: "",
      city: "TP.HCM",
      district: "",
      ward: "",
      area: "",
      bedrooms: 1,
      bathrooms: 1,
      floors: "",
      monthlyRent: "",
      depositAmount: "",
      amenities: "",
      allowPets: false,
      allowSmoking: false,
      maxOccupants: "",
    });
    setShowModal(true);
  };
  const openEdit = (p) => {
    setSelectedProp(p);
    setForm({
      title: p.title,
      description: p.description,
      propertyType: p.propertyType,
      address: p.address,
      city: p.city,
      district: p.district,
      ward: p.ward || "",
      area: p.area,
      bedrooms: p.bedrooms,
      bathrooms: p.bathrooms,
      floors: p.floors || "",
      monthlyRent: p.monthlyRent,
      depositAmount: p.depositAmount,
      amenities: p.amenities
        ? JSON.parse(p.amenities).join(", ")
        : "",
      allowPets: p.allowPets,
      allowSmoking: p.allowSmoking,
      maxOccupants: p.maxOccupants || "",
    });
    setShowModal(true);
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-20">
        <div>
          <div className="page-title">Bất động sản của bạn</div>
          <div className="page-desc">
            Quản lý danh sách bất động sản bạn đang cho thuê
          </div>
        </div>
        <button
          className="btn btn-primary"
          onClick={openCreate}
        >
          <Plus size={16} /> Thêm BĐS mới
        </button>
      </div>

      {/* Search and Filter */}
      <div style={{ marginBottom: 24 }}>
        <div style={{ position: "relative", marginBottom: 16 }}>
          <Search
            size={18}
            style={{
              position: "absolute",
              left: 16,
              top: "50%",
              transform: "translateY(-50%)",
              color: "#6b7280",
            }}
          />
          <input
            className="form-control"
            placeholder="Tìm theo tên, địa chỉ..."
            value={searchQuery}
            onChange={(e) => setSearchQuery(e.target.value)}
            style={{
              paddingLeft: 48,
              background: "#0f111a",
            }}
          />
        </div>

        <div className="filter-bar">
          {[
            { label: "Tất cả", value: "All", count: myProps.length },
            {
              label: "Đang đăng",
              value: "Available",
              count: myProps.filter((p) => p.status === "Available").length,
            },
            {
              label: "Đã thuê",
              value: "Rented",
              count: myProps.filter((p) => p.status === "Rented").length,
            },
            {
              label: "Chờ duyệt",
              value: "Pending",
              count: myProps.filter((p) => p.status === "Pending").length,
            },
            {
              label: "Tạm ẩn",
              value: "Draft",
              count: myProps.filter((p) => p.status === "Draft").length,
            },
            {
              label: "Bị khoá",
              value: "Inactive",
              count: myProps.filter((p) => p.status === "Inactive").length,
            },
          ].map((tab) => (
            <button
              key={tab.value}
              className={`filter-tab ${statusFilter === tab.value ? "active" : ""}`}
              onClick={() => setStatusFilter(tab.value)}
            >
              {tab.label} ({tab.count})
            </button>
          ))}
        </div>
      </div>

      {/* Property Grid */}
      <div className="landlord-properties-grid">
        {filteredProps.map((p) => (
          <div key={p.id} className="landlord-property-card-new">
            <div className="property-card-image-wrapper">
              <img
                src={p.images[0]?.imageUrl}
                alt={p.title}
                className="property-card-image-new"
              />
              <div className="property-card-badges">
                <span className="property-type-badge">
                  {getPropertyTypeLabel(p.propertyType)}
                </span>
                {getStatusBadge(p.status)}
              </div>
            </div>
            <div className="property-card-content-new">
              <h3 className="property-card-title-new">{p.title}</h3>
              <div className="property-card-location">
                <MapPin size={14} />
                <span>{p.district}, {p.city}</span>
              </div>
              <div className="property-card-specs-new">
                <div className="spec-item-new">
                  <Bed size={16} />
                  <span>{p.bedrooms}</span>
                </div>
                <div className="spec-item-new">
                  <Maximize2 size={16} />
                  <span>{p.area} m²</span>
                </div>
              </div>
              <div className="property-card-footer-new">
                <div className="property-card-price-new">
                  <div className="price-amount-new">
                    {formatMoney(p.monthlyRent)}
                  </div>
                  <div className="price-period-new">/ tháng</div>
                </div>
                <div className="property-card-actions-new">
                  <button
                    className="action-btn-new action-warning"
                    title="Tạm ẩn"
                  >
                    Tạm ẩn
                  </button>
                  <button
                    className="action-btn-new action-icon"
                    onClick={() => setViewDetail(p)}
                    title="Xem chi tiết"
                  >
                    <Eye size={16} />
                  </button>
                  <button
                    className="action-btn-new action-icon"
                    onClick={() => openEdit(p)}
                    title="Chỉnh sửa"
                  >
                    <Edit size={16} />
                  </button>
                  <button
                    className="action-btn-new action-icon action-danger"
                    title="Quản lý hình ảnh"
                    onClick={() => setImageManagementProp(p)}
                  >
                    <ImageIcon size={16} />
                  </button>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Create/Edit Modal */}
      {showModal && (
        <div
          className="modal-overlay"
          onClick={() => setShowModal(false)}
        >
          <div
            className="modal-create-property"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="modal-header">
              <span className="modal-title">
                {selectedProp
                  ? "Chỉnh sửa BDS"
                  : "Đăng tin BDS mới"}
              </span>
              <button
                className="modal-close btn btn-ghost btn-sm btn-icon"
                onClick={() => setShowModal(false)}
              >
                ✕
              </button>
            </div>
            <div className="modal-body">
              <div className="form-group-with-icon">
                <label className="form-label">
                  Tiêu đề <span className="text-red">*</span>
                </label>
                <div className="input-icon-wrapper">
                  <FileText size={16} className="input-icon" />
                  <input
                    className="form-control-with-icon"
                    placeholder="VD: Căn hộ 2PN Vinhomes Central Park"
                    value={form.title}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        title: e.target.value,
                      })
                    }
                  />
                </div>
              </div>

              <div className="form-group-with-icon">
                <label className="form-label">
                  Loại BDS <span className="text-red">*</span>
                </label>
                <div className="input-icon-wrapper">
                  <Building2 size={16} className="input-icon" />
                  <select
                    className="form-control-with-icon"
                    value={form.propertyType}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        propertyType: e.target.value,
                      })
                    }
                  >
                    {[
                      "Apartment",
                      "House",
                      "Room",
                      "Villa",
                      "Commercial",
                    ].map((t) => (
                      <option key={t} value={t}>
                        {getPropertyTypeLabel(t)}
                      </option>
                    ))}
                  </select>
                </div>
              </div>

              <div className="form-row">
                <div className="form-group-with-icon">
                  <label className="form-label">
                    Thành phố <span className="text-red">*</span>
                  </label>
                  <div className="input-icon-wrapper">
                    <MapPin size={16} className="input-icon" />
                    <input
                      className="form-control-with-icon"
                      placeholder="TP.HCM"
                      value={form.city}
                      onChange={(e) =>
                        setForm({ ...form, city: e.target.value })
                      }
                    />
                  </div>
                </div>
                <div className="form-group-with-icon">
                  <label className="form-label">
                    Quận/Huyện <span className="text-red">*</span>
                  </label>
                  <div className="input-icon-wrapper">
                    <MapPin size={16} className="input-icon" />
                    <input
                      className="form-control-with-icon"
                      placeholder="Quận Bình Thạnh"
                      value={form.district}
                      onChange={(e) =>
                        setForm({
                          ...form,
                          district: e.target.value,
                        })
                      }
                    />
                  </div>
                </div>
              </div>

              <div className="form-row">
                <div className="form-group-with-icon">
                  <label className="form-label">
                    Phường/Xã
                  </label>
                  <div className="input-icon-wrapper">
                    <MapPin size={16} className="input-icon" />
                    <input
                      className="form-control-with-icon"
                      placeholder="Phường 12"
                      value={form.ward}
                      onChange={(e) =>
                        setForm({ ...form, ward: e.target.value })
                      }
                    />
                  </div>
                </div>
                <div className="form-group-with-icon">
                  <label className="form-label">
                    Địa chỉ <span className="text-red">*</span>
                  </label>
                  <div className="input-icon-wrapper">
                    <MapPin size={16} className="input-icon" />
                    <input
                      className="form-control-with-icon"
                      placeholder="Số nhà, tên đường..."
                      value={form.address}
                      onChange={(e) =>
                        setForm({
                          ...form,
                          address: e.target.value,
                        })
                      }
                    />
                  </div>
                </div>
              </div>

              <div className="form-row">
                <div className="form-group-with-icon">
                  <label className="form-label">
                    Diện tích (m²) <span className="text-red">*</span>
                  </label>
                  <div className="input-icon-wrapper">
                    <Maximize2 size={16} className="input-icon" />
                    <input
                      className="form-control-with-icon"
                      type="number"
                      placeholder="80"
                      value={form.area}
                      onChange={(e) =>
                        setForm({ ...form, area: e.target.value })
                      }
                    />
                  </div>
                </div>
                <div className="form-group-with-icon">
                  <label className="form-label">Số tầng</label>
                  <div className="input-icon-wrapper">
                    <Layers size={16} className="input-icon" />
                    <input
                      className="form-control-with-icon"
                      type="number"
                      placeholder="1"
                      value={form.floors}
                      onChange={(e) =>
                        setForm({
                          ...form,
                          floors: e.target.value,
                        })
                      }
                    />
                  </div>
                </div>
              </div>

              <div className="form-row">
                <div className="form-group-with-icon">
                  <label className="form-label">
                    Phòng ngủ <span className="text-red">*</span>
                  </label>
                  <div className="input-icon-wrapper">
                    <Bed size={16} className="input-icon" />
                    <input
                      className="form-control-with-icon"
                      type="number"
                      min={0}
                      placeholder="2"
                      value={form.bedrooms}
                      onChange={(e) =>
                        setForm({
                          ...form,
                          bedrooms: e.target.value,
                        })
                      }
                    />
                  </div>
                </div>
                <div className="form-group-with-icon">
                  <label className="form-label">
                    Phòng tắm <span className="text-red">*</span>
                  </label>
                  <div className="input-icon-wrapper">
                    <Bath size={16} className="input-icon" />
                    <input
                      className="form-control-with-icon"
                      type="number"
                      min={1}
                      placeholder="2"
                      value={form.bathrooms}
                      onChange={(e) =>
                        setForm({
                          ...form,
                          bathrooms: e.target.value,
                        })
                      }
                    />
                  </div>
                </div>
              </div>

              <div className="form-row">
                <div className="form-group-with-icon">
                  <label className="form-label">
                    Tiền thuê/tháng (VND) <span className="text-red">*</span>
                  </label>
                  <div className="input-icon-wrapper">
                    <DollarSign size={16} className="input-icon" />
                    <input
                      className="form-control-with-icon"
                      type="number"
                      placeholder="15000000"
                      value={form.monthlyRent}
                      onChange={(e) =>
                        setForm({
                          ...form,
                          monthlyRent: e.target.value,
                        })
                      }
                    />
                  </div>
                </div>
                <div className="form-group-with-icon">
                  <label className="form-label">
                    Tiền đặt cọc (VND) <span className="text-red">*</span>
                  </label>
                  <div className="input-icon-wrapper">
                    <DollarSign size={16} className="input-icon" />
                    <input
                      className="form-control-with-icon"
                      type="number"
                      placeholder="30000000"
                      value={form.depositAmount}
                      onChange={(e) =>
                        setForm({
                          ...form,
                          depositAmount: e.target.value,
                        })
                      }
                    />
                  </div>
                </div>
              </div>

              <div className="form-group-with-icon">
                <label className="form-label">Mô tả</label>
                <div className="input-icon-wrapper">
                  <FileText size={16} className="input-icon textarea-icon" />
                  <textarea
                    className="form-control-with-icon"
                    rows={3}
                    placeholder="Mô tả chi tiết về BDS..."
                    value={form.description}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        description: e.target.value,
                      })
                    }
                  />
                </div>
              </div>

              <div className="form-group">
                <label className="form-label">
                  Tiện ích (phân cách bởi dấu phẩy)
                </label>
                <textarea
                  className="form-control"
                  rows={2}
                  placeholder="VD: Wifi, Điều hoà, Thang máy, Bãi xe..."
                  value={form.amenities}
                  onChange={(e) =>
                    setForm({
                      ...form,
                      amenities: e.target.value,
                    })
                  }
                />
              </div>

              <div className="form-checkboxes">
                <label className="custom-checkbox">
                  <input
                    type="checkbox"
                    checked={form.allowPets}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        allowPets: e.target.checked,
                      })
                    }
                  />
                  <span className="custom-checkbox-box">
                    <svg
                      className="custom-checkbox-check"
                      viewBox="0 0 12 10"
                      fill="none"
                    >
                      <path
                        d="M1 5L4.5 8.5L11 1.5"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                      />
                    </svg>
                  </span>
                  <span className="custom-checkbox-content">
                    <Dog size={16} />
                    <span>Được nuôi thú cưng</span>
                  </span>
                </label>
                <label className="custom-checkbox">
                  <input
                    type="checkbox"
                    checked={form.allowSmoking}
                    onChange={(e) =>
                      setForm({
                        ...form,
                        allowSmoking: e.target.checked,
                      })
                    }
                  />
                  <span className="custom-checkbox-box">
                    <svg
                      className="custom-checkbox-check"
                      viewBox="0 0 12 10"
                      fill="none"
                    >
                      <path
                        d="M1 5L4.5 8.5L11 1.5"
                        stroke="currentColor"
                        strokeWidth="2"
                        strokeLinecap="round"
                        strokeLinejoin="round"
                      />
                    </svg>
                  </span>
                  <span className="custom-checkbox-content">
                    <Cigarette size={16} />
                    <span>Được hút thuốc</span>
                  </span>
                </label>
              </div>
            </div>
            <div className="modal-footer">
              <button
                className="btn btn-secondary"
                onClick={() => setShowModal(false)}
              >
                Huỷ
              </button>
              <button
                className="btn btn-primary"
                onClick={() => setShowModal(false)}
              >
                {selectedProp ? "Lưu thay đổi" : "Đăng tin"}
              </button>
            </div>
          </div>
        </div>
      )}

      {/* Detail view modal */}
      {viewDetail && (
        <PropertyDetailModal
          property={viewDetail}
          onClose={() => setViewDetail(null)}
          onEdit={(property) => {
            setViewDetail(null);
            openEdit(property);
          }}
          showImageManagement={true}
        />
      )}

      {/* Image management modal */}
      {imageManagementProp && (
        <ImageManagementModal
          property={imageManagementProp}
          onClose={() => setImageManagementProp(null)}
          onSave={(images, thumbnailIndex) => {
            console.log("Saved images:", images, "Thumbnail:", thumbnailIndex);
            // TODO: Update property images
            setImageManagementProp(null);
          }}
        />
      )}
    </div>
  );
}