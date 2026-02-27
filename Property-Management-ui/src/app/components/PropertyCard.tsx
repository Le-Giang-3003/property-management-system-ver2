import { Heart, Edit, Eye, Copy } from 'lucide-react';
import { ImageWithFallback } from './figma/ImageWithFallback';

interface PropertyCardProps {
  id: number;
  title: string;
  category: string;
  area: number;
  bedrooms: number;
  bathrooms: number;
  price: number;
  status: string;
  image: string;
  showActions?: boolean;
}

export function PropertyCard({
  title,
  category,
  area,
  bedrooms,
  bathrooms,
  price,
  status,
  image,
  showActions = true,
}: PropertyCardProps) {
  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Đã thuê':
        return 'bg-red-500';
      case 'Sẵn sàng':
        return 'bg-green-500';
      case 'Đang dùng':
        return 'bg-blue-500';
      case 'Phòng trống':
        return 'bg-cyan-500';
      default:
        return 'bg-gray-500';
    }
  };

  return (
    <div className="bg-[#252a3d] rounded-lg overflow-hidden group hover:ring-2 hover:ring-blue-500 transition-all">
      <div className="relative h-48">
        <ImageWithFallback
          src={image}
          alt={title}
          className="w-full h-full object-cover"
        />
        <button className="absolute top-3 right-3 w-8 h-8 bg-white/90 rounded-full flex items-center justify-center hover:bg-white transition-colors">
          <Heart className="w-4 h-4 text-gray-700" />
        </button>
        <div className="absolute top-3 left-3">
          <span className={`text-xs px-2 py-1 rounded text-white ${getStatusColor(status)}`}>
            {status}
          </span>
        </div>
        <div className="absolute bottom-3 left-3">
          <span className="text-xs px-2 py-1 rounded bg-blue-600 text-white">
            {category}
          </span>
        </div>
      </div>

      <div className="p-4">
        <h3 className="text-white font-medium mb-2 line-clamp-1">{title}</h3>
        
        <div className="flex items-center gap-4 text-gray-400 text-sm mb-3">
          <span className="flex items-center gap-1">
            🏠 {bedrooms}
          </span>
          <span className="flex items-center gap-1">
            🚿 {bathrooms} m²
          </span>
          <span className="flex items-center gap-1">
            📐 {area} m²
          </span>
        </div>

        <div className="flex items-center justify-between">
          <div>
            <p className="text-orange-400 font-bold text-lg">
              {price.toLocaleString('vi-VN')} VNĐ
            </p>
          </div>
          
          {showActions && (
            <div className="flex gap-2">
              <button className="w-8 h-8 bg-gray-700 hover:bg-gray-600 rounded flex items-center justify-center transition-colors">
                <Edit className="w-4 h-4 text-white" />
              </button>
              <button className="w-8 h-8 bg-gray-700 hover:bg-gray-600 rounded flex items-center justify-center transition-colors">
                <Eye className="w-4 h-4 text-white" />
              </button>
              <button className="w-8 h-8 bg-blue-600 hover:bg-blue-500 rounded flex items-center justify-center transition-colors">
                <Copy className="w-4 h-4 text-white" />
              </button>
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
