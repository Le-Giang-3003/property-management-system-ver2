import { useState } from "react";
import { properties } from "../../utils/mockData";
import { formatMoney } from "../../utils/helpers";
import { Heart, Edit, Trash2, Bed, Maximize2, Filter, MapPin } from "lucide-react";

// Mock data for tenant's properties (favorited/applied)
const myPropertyIds = [1, 2, 3, 4, 5, 6, 7, 8]; // IDs of properties tenant interacted with
const myProperties = properties.filter(p => myPropertyIds.includes(p.id));

// Status mapping
const statusMap = {
  "Available": { label: "Còn trống", color: "success" },
  "Rented": { label: "Đang thuê", color: "info" },
  "Pending": { label: "Chờ duyệt", color: "warning" },
};

export default function MyProperties() {
  const [favorites, setFavorites] = useState([1, 3, 5, 7]);
  const [selectedProp, setSelectedProp] = useState(null);
  const [filterStatus, setFilterStatus] = useState("all");

  const filtered = myProperties.filter(p => {
    if (filterStatus === "all") return true;
    return p.status === filterStatus;
  });

  const toggleFav = (id) => {
    setFavorites(prev => 
      prev.includes(id) ? prev.filter(f => f !== id) : [...prev, id]
    );
  };

  return (
    <div>
      {/* Header */}
      <div className="mb-20">
        <div className="page-title">Bất động sản của bạn</div>
        <div className="page-desc">
          Quản lý các BĐS đang quan tâm và đang thuê
        </div>
      </div>

      {/* Filter Bar */}
      <div style={{ 
        display: 'flex', 
        alignItems: 'center', 
        justifyContent: 'space-between',
        marginBottom: 24,
        flexWrap: 'wrap',
        gap: 12
      }}>
        <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap' }}>
          <button 
            className={`filter-tab ${filterStatus === 'all' ? 'active' : ''}`}
            onClick={() => setFilterStatus('all')}
          >
            Tất cả ({myProperties.length})
          </button>
          <button 
            className={`filter-tab ${filterStatus === 'Available' ? 'active' : ''}`}
            onClick={() => setFilterStatus('Available')}
          >
            Còn trống ({myProperties.filter(p => p.status === 'Available').length})
          </button>
          <button 
            className={`filter-tab ${filterStatus === 'Rented' ? 'active' : ''}`}
            onClick={() => setFilterStatus('Rented')}
          >
            Đang thuê ({myProperties.filter(p => p.status === 'Rented').length})
          </button>
          <button 
            className={`filter-tab ${filterStatus === 'Pending' ? 'active' : ''}`}
            onClick={() => setFilterStatus('Pending')}
          >
            Chờ duyệt ({myProperties.filter(p => p.status === 'Pending').length})
          </button>
        </div>

        <button className="btn btn-primary">
          + Thêm mới BĐS
        </button>
      </div>

      {/* Property Grid */}
      <div className="property-grid">
        {filtered.map(p => {
          const status = statusMap[p.status] || { label: p.status, color: "gray" };
          const isFavorite = favorites.includes(p.id);

          return (
            <div key={p.id} className="tenant-property-card">
              {/* Image */}
              <div className="tenant-property-image-wrapper">
                {p.images[0] ? (
                  <img 
                    className="tenant-property-image" 
                    src={p.images[0].imageUrl} 
                    alt={p.title}
                  />
                ) : (
                  <div className="tenant-property-image-placeholder">
                    🏠
                  </div>
                )}
                
                {/* Overlay Badge */}
                <div className="tenant-property-overlay">
                  <span className={`badge badge-${status.color}`}>
                    {status.label}
                  </span>
                </div>
              </div>

              {/* Content */}
              <div className="tenant-property-body">
                {/* Type & Title */}
                <div style={{ marginBottom: 8 }}>
                  <div className="tenant-property-type">
                    {p.propertyType === "Apartment" ? "CĂN HỘ STUDIO" : 
                     p.propertyType === "House" ? "NHÀ ĐỨT" :
                     p.propertyType === "Villa" ? "BIỆT THỰ" : 
                     p.propertyType.toUpperCase()}
                  </div>
                  <div className="tenant-property-title">
                    {p.title}
                  </div>
                </div>

                {/* Location */}
                <div className="tenant-property-location">
                  <MapPin size={12} />
                  {p.district}, {p.city}
                </div>

                {/* Specs */}
                <div className="tenant-property-specs">
                  <span className="spec-item">
                    <Bed size={12} /> {p.bedrooms}
                  </span>
                  <span className="spec-item">
                    <Maximize2 size={12} /> {p.area}m²
                  </span>
                </div>

                {/* Price & Actions */}
                <div className="tenant-property-footer">
                  <div>
                    <div className="tenant-property-price">
                      {formatMoney(p.monthlyRent)}
                    </div>
                    <div className="tenant-property-price-sub">
                      /tháng
                    </div>
                  </div>

                  <div style={{ display: 'flex', gap: 6, alignItems: 'center' }}>
                    <button className="btn-request">
                      Yêu cầu
                    </button>
                    
                    <button 
                      className="btn-icon-action"
                      onClick={() => toggleFav(p.id)}
                      title={isFavorite ? "Bỏ yêu thích" : "Yêu thích"}
                    >
                      <Heart 
                        size={14} 
                        fill={isFavorite ? "#ef4444" : "none"}
                        color={isFavorite ? "#ef4444" : "#9ca3af"}
                      />
                    </button>
                    
                    <button 
                      className="btn-icon-action"
                      title="Chỉnh sửa"
                    >
                      <Edit size={14} />
                    </button>
                    
                    <button 
                      className="btn-icon-action"
                      title="Xóa"
                    >
                      <Trash2 size={14} />
                    </button>
                  </div>
                </div>
              </div>
            </div>
          );
        })}
      </div>

      {filtered.length === 0 && (
        <div style={{ 
          textAlign: 'center', 
          padding: '4rem 2rem',
          color: '#9ca3af'
        }}>
          <div style={{ fontSize: '3rem', marginBottom: '1rem' }}>🏠</div>
          <div style={{ fontSize: '1.125rem', fontWeight: 600, marginBottom: '0.5rem' }}>
            Chưa có BĐS nào
          </div>
          <div style={{ fontSize: '0.875rem' }}>
            Hãy bắt đầu tìm kiếm BĐS phù hợp với bạn
          </div>
        </div>
      )}
    </div>
  );
}
