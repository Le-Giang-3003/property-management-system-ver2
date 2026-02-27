import { useState, useRef } from "react";
import { X, Upload, Star, Trash2, ImageIcon } from "lucide-react";

interface ImageManagementModalProps {
  property: any;
  onClose: () => void;
  onSave: (images: any[], thumbnailIndex: number) => void;
}

export default function ImageManagementModal({
  property,
  onClose,
  onSave,
}: ImageManagementModalProps) {
  const [images, setImages] = useState(property.images || []);
  const [thumbnailIndex, setThumbnailIndex] = useState(0);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileUpload = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (!files) return;

    const newImages = Array.from(files).map((file, index) => ({
      id: Date.now() + index,
      imageUrl: URL.createObjectURL(file),
      isThumbnail: false,
    }));

    setImages([...images, ...newImages]);
  };

  const handleSetThumbnail = (index: number) => {
    setThumbnailIndex(index);
  };

  const handleDeleteImage = (index: number) => {
    const newImages = images.filter((_: any, i: number) => i !== index);
    setImages(newImages);
    
    if (thumbnailIndex === index) {
      setThumbnailIndex(0);
    } else if (thumbnailIndex > index) {
      setThumbnailIndex(thumbnailIndex - 1);
    }
  };

  const handleSave = () => {
    onSave(images, thumbnailIndex);
    onClose();
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div
        className="modal-image-management"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="modal-header">
          <div className="modal-title-section">
            <ImageIcon size={24} />
            <div>
              <h2 className="modal-title">Quản lý hình ảnh</h2>
              <p className="modal-subtitle">{property.title}</p>
            </div>
          </div>
          <button
            className="modal-close btn btn-ghost btn-sm btn-icon"
            onClick={onClose}
          >
            <X size={20} />
          </button>
        </div>

        <div className="modal-body">
          {/* Upload Section */}
          <div className="upload-section">
            <button
              className="btn-upload-image"
              onClick={() => fileInputRef.current?.click()}
            >
              <Upload size={24} />
              <span className="upload-text">
                Tải ảnh lên
              </span>
              <span className="upload-hint">
                Hỗ trợ: JPG, PNG, WEBP (Tối đa 10MB)
              </span>
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

          {/* Images Grid */}
          {images.length > 0 ? (
            <div className="images-grid-section">
              <div className="section-header">
                <h3 className="section-title">
                  Hình ảnh ({images.length})
                </h3>
                <p className="section-hint">
                  Click vào ngôi sao để đặt ảnh đại diện
                </p>
              </div>
              <div className="images-grid">
                {images.map((img: any, index: number) => (
                  <div
                    key={img.id}
                    className={`image-grid-item ${index === thumbnailIndex ? "is-thumbnail" : ""}`}
                  >
                    <img src={img.imageUrl} alt="" />
                    
                    {index === thumbnailIndex && (
                      <div className="thumbnail-badge-overlay">
                        <Star size={12} fill="currentColor" />
                        Ảnh đại diện
                      </div>
                    )}

                    <div className="image-grid-actions">
                      <button
                        className={`image-action-btn ${index === thumbnailIndex ? "active" : ""}`}
                        onClick={() => handleSetThumbnail(index)}
                        title="Đặt làm ảnh đại diện"
                      >
                        <Star
                          size={16}
                          fill={
                            index === thumbnailIndex
                              ? "currentColor"
                              : "none"
                          }
                        />
                      </button>
                      <button
                        className="image-action-btn danger"
                        onClick={() => handleDeleteImage(index)}
                        title="Xóa ảnh"
                      >
                        <Trash2 size={16} />
                      </button>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          ) : (
            <div className="empty-images-state">
              <ImageIcon size={64} />
              <p className="empty-title">Chưa có hình ảnh</p>
              <p className="empty-desc">
                Tải lên ảnh để hiển thị bất động sản của bạn
              </p>
            </div>
          )}
        </div>

        <div className="modal-footer">
          <button className="btn btn-secondary" onClick={onClose}>
            Hủy
          </button>
          <button className="btn btn-primary" onClick={handleSave}>
            <Upload size={16} />
            Lưu thay đổi
          </button>
        </div>
      </div>
    </div>
  );
}
