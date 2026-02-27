import { useState, useCallback, useRef } from "react";
import useEmblaCarousel from "embla-carousel-react";
import {
  X,
  ChevronLeft,
  ChevronRight,
  Upload,
  Star,
  Trash2,
  ImageIcon,
  MapPin,
  Maximize2,
  Bed,
  Bath,
  DollarSign,
  Home,
  Dog,
  Cigarette,
} from "lucide-react";
import { formatMoney, getStatusBadge } from "../utils/helpers";

interface PropertyDetailModalProps {
  property: any;
  onClose: () => void;
  onEdit?: (property: any) => void;
  onApprove?: () => void;
  onReject?: () => void;
  showImageManagement?: boolean;
  showApplyButton?: boolean;
  onApply?: () => void;
}

export default function PropertyDetailModal({
  property,
  onClose,
  onEdit,
  onApprove,
  onReject,
  showImageManagement = false,
  showApplyButton = false,
  onApply,
}: PropertyDetailModalProps) {
  const [emblaRef, emblaApi] = useEmblaCarousel({ loop: true });
  const [selectedIndex, setSelectedIndex] = useState(0);
  const [uploadedImages, setUploadedImages] = useState(
    property.images || []
  );
  const [thumbnailIndex, setThumbnailIndex] = useState(0);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const scrollPrev = useCallback(() => {
    if (emblaApi) emblaApi.scrollPrev();
  }, [emblaApi]);

  const scrollNext = useCallback(() => {
    if (emblaApi) emblaApi.scrollNext();
  }, [emblaApi]);

  const onSelect = useCallback(() => {
    if (!emblaApi) return;
    setSelectedIndex(emblaApi.selectedScrollSnap());
  }, [emblaApi]);

  useState(() => {
    if (!emblaApi) return;
    onSelect();
    emblaApi.on("select", onSelect);
    emblaApi.on("reInit", onSelect);
  });

  const handleFileUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (!files) return;

    const newImages = Array.from(files).map((file, index) => ({
      id: uploadedImages.length + index + 1,
      imageUrl: URL.createObjectURL(file),
      isThumbnail: false,
    }));

    setUploadedImages([...uploadedImages, ...newImages]);
  };

  const handleSetThumbnail = (index: number) => {
    setThumbnailIndex(index);
    // Update thumbnail logic here
  };

  const handleDeleteImage = (index: number) => {
    const newImages = uploadedImages.filter((_, i) => i !== index);
    setUploadedImages(newImages);
    if (thumbnailIndex === index) {
      setThumbnailIndex(0);
    } else if (thumbnailIndex > index) {
      setThumbnailIndex(thumbnailIndex - 1);
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div
        className="modal-detail"
        onClick={(e) => e.stopPropagation()}
      >
        <button
          className="modal-detail-close"
          onClick={onClose}
        >
          <X size={24} />
        </button>

        {/* Image Slider */}
        <div className="property-detail-slider">
          {uploadedImages.length > 0 ? (
            <>
              <div className="embla" ref={emblaRef}>
                <div className="embla__container">
                  {uploadedImages.map((img, index) => (
                    <div className="embla__slide" key={img.id}>
                      <img
                        src={img.imageUrl}
                        alt=""
                        className="property-detail-image"
                      />
                      {index === thumbnailIndex && (
                        <div className="thumbnail-badge">
                          <Star size={14} fill="currentColor" />
                          Ảnh đại diện
                        </div>
                      )}
                    </div>
                  ))}
                </div>
              </div>
              {uploadedImages.length > 1 && (
                <>
                  <button
                    className="embla__prev"
                    onClick={scrollPrev}
                  >
                    <ChevronLeft size={24} />
                  </button>
                  <button
                    className="embla__next"
                    onClick={scrollNext}
                  >
                    <ChevronRight size={24} />
                  </button>
                  <div className="embla__dots">
                    {uploadedImages.map((_, index) => (
                      <button
                        key={index}
                        className={`embla__dot ${index === selectedIndex ? "active" : ""}`}
                        onClick={() => emblaApi?.scrollTo(index)}
                      />
                    ))}
                  </div>
                </>
              )}
            </>
          ) : (
            <div className="property-detail-no-image">
              <ImageIcon size={48} />
              <p>Chưa có hình ảnh</p>
            </div>
          )}
        </div>

        {/* Image Management */}
        {showImageManagement && (
          <div className="property-image-management">
            <div className="image-management-header">
              <h3>Quản lý hình ảnh ({uploadedImages.length})</h3>
              <button
                className="btn btn-primary btn-sm"
                onClick={() => fileInputRef.current?.click()}
              >
                <Upload size={14} />
                Tải ảnh lên
              </button>
              <input
                ref={fileInputRef}
                type="file"
                accept="image/*"
                multiple
                style={{ display: "none" }}
                onChange={handleFileUpload}
              />
            </div>
            <div className="image-thumbnails">
              {uploadedImages.map((img, index) => (
                <div
                  key={img.id}
                  className={`image-thumbnail-item ${index === thumbnailIndex ? "is-thumbnail" : ""}`}
                >
                  <img src={img.imageUrl} alt="" />
                  <div className="image-thumbnail-actions">
                    <button
                      className="thumbnail-action-btn"
                      onClick={() => handleSetThumbnail(index)}
                      title="Đặt làm ảnh đại diện"
                    >
                      <Star
                        size={14}
                        fill={
                          index === thumbnailIndex
                            ? "currentColor"
                            : "none"
                        }
                      />
                    </button>
                    <button
                      className="thumbnail-action-btn danger"
                      onClick={() => handleDeleteImage(index)}
                      title="Xóa ảnh"
                    >
                      <Trash2 size={14} />
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Property Info */}
        <div className="property-detail-info">
          <div className="property-detail-header">
            <div>
              <h2 className="property-detail-title">
                {property.title}
              </h2>
              <p className="property-detail-address">
                <MapPin size={16} /> {property.address}, {property.district},{" "}
                {property.city}
              </p>
            </div>
            {getStatusBadge(property.status)}
          </div>

          <div className="property-detail-price">
            <div className="price-main">
              {formatMoney(property.monthlyRent)}
            </div>
            <div className="price-label">/ tháng</div>
          </div>

          <div className="property-detail-specs">
            <div className="spec-box">
              <div className="spec-icon">
                <Maximize2 size={20} />
              </div>
              <div className="spec-label">Diện tích</div>
              <div className="spec-value">{property.area} m²</div>
            </div>
            <div className="spec-box">
              <div className="spec-icon">
                <Bed size={20} />
              </div>
              <div className="spec-label">Phòng ngủ</div>
              <div className="spec-value">{property.bedrooms}</div>
            </div>
            <div className="spec-box">
              <div className="spec-icon">
                <Bath size={20} />
              </div>
              <div className="spec-label">Phòng tắm</div>
              <div className="spec-value">{property.bathrooms}</div>
            </div>
          </div>

          <div className="property-detail-section">
            <h3 className="section-title">Mô tả</h3>
            <p className="section-content">{property.description}</p>
          </div>

          <div className="property-detail-section">
            <h3 className="section-title">Thông tin chi tiết</h3>
            <div className="detail-grid">
              <div className="detail-item">
                <span className="detail-label">
                  <DollarSign size={16} />
                  Tiền thuê:
                </span>
                <span className="detail-value detail-value-primary">
                  {formatMoney(property.monthlyRent)}
                </span>
              </div>
              <div className="detail-item">
                <span className="detail-label">
                  <DollarSign size={16} />
                  Tiền cọc:
                </span>
                <span className="detail-value">
                  {formatMoney(property.depositAmount)}
                </span>
              </div>
              <div className="detail-item">
                <span className="detail-label">
                  <Dog size={16} />
                  Thú cưng:
                </span>
                <span className="detail-value">
                  {property.allowPets ? "✅ Cho phép" : "❌ Không"}
                </span>
              </div>
              <div className="detail-item">
                <span className="detail-label">
                  <Cigarette size={16} />
                  Hút thuốc:
                </span>
                <span className="detail-value">
                  {property.allowSmoking
                    ? "✅ Cho phép"
                    : "❌ Không"}
                </span>
              </div>
            </div>
          </div>

          {property.amenities && (
            <div className="property-detail-section">
              <h3 className="section-title">Tiện ích</h3>
              <div className="amenities-list">
                {JSON.parse(property.amenities).map((a: string) => (
                  <span key={a} className="amenity-badge">
                    ✓ {a}
                  </span>
                ))}
              </div>
            </div>
          )}
        </div>

        <div className="modal-footer">
          <button className="btn btn-secondary" onClick={onClose}>
            Đóng
          </button>
          {onEdit && (
            <button
              className="btn btn-primary"
              onClick={() => {
                onClose();
                onEdit(property);
              }}
            >
              Chỉnh sửa
            </button>
          )}
          {onApprove && (
            <button
              className="btn btn-success"
              onClick={() => {
                onClose();
                onApprove();
              }}
            >
              Duyệt
            </button>
          )}
          {onReject && (
            <button
              className="btn btn-danger"
              onClick={() => {
                onClose();
                onReject();
              }}
            >
              Từ chối
            </button>
          )}
          {showApplyButton && (
            <button
              className="btn btn-primary"
              onClick={() => {
                onClose();
                onApply?.();
              }}
            >
              Nộp đơn
            </button>
          )}
        </div>
      </div>
    </div>
  );
}