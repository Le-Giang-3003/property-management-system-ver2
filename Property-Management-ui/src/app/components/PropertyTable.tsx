import { Trash2, Edit } from 'lucide-react';

interface Property {
  id: number;
  image: string;
  title: string;
  category: string;
  createdBy: string;
  status: string;
  price: string;
}

interface PropertyTableProps {
  properties: Property[];
}

export function PropertyTable({ properties }: PropertyTableProps) {
  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Đã thuê':
        return 'bg-red-500/20 text-red-400 border border-red-500/30';
      case 'Sẵn sàng':
        return 'bg-green-500/20 text-green-400 border border-green-500/30';
      case 'Đang dùng':
        return 'bg-blue-500/20 text-blue-400 border border-blue-500/30';
      case 'Phòng trống':
        return 'bg-cyan-500/20 text-cyan-400 border border-cyan-500/30';
      case 'Căn hộ quanh':
        return 'bg-blue-500/20 text-blue-400 border border-blue-500/30';
      default:
        return 'bg-gray-500/20 text-gray-400 border border-gray-500/30';
    }
  };

  return (
    <div className="bg-[#1e2333] rounded-lg overflow-hidden">
      <div className="overflow-x-auto">
        <table className="w-full">
          <thead>
            <tr className="border-b border-gray-700">
              <th className="text-left px-6 py-4 text-gray-400 font-medium text-sm">ID</th>
              <th className="text-left px-6 py-4 text-gray-400 font-medium text-sm">Tên nhà</th>
              <th className="text-left px-6 py-4 text-gray-400 font-medium text-sm">Loại</th>
              <th className="text-left px-6 py-4 text-gray-400 font-medium text-sm">Thứ thuộc</th>
              <th className="text-left px-6 py-4 text-gray-400 font-medium text-sm">Trạng thái</th>
              <th className="text-left px-6 py-4 text-gray-400 font-medium text-sm">Tiền thuê</th>
              <th className="text-left px-6 py-4 text-gray-400 font-medium text-sm">Hành động</th>
            </tr>
          </thead>
          <tbody>
            {properties.map((property, index) => (
              <tr key={property.id} className="border-b border-gray-700/50 hover:bg-gray-800/30 transition-colors">
                <td className="px-6 py-4">
                  <span className="text-gray-300 text-sm">#{property.id}</span>
                </td>
                <td className="px-6 py-4">
                  <div className="flex items-center gap-3">
                    <img
                      src={property.image}
                      alt={property.title}
                      className="w-12 h-12 rounded object-cover"
                    />
                    <span className="text-white text-sm font-medium">{property.title}</span>
                  </div>
                </td>
                <td className="px-6 py-4">
                  <span className={`text-xs px-3 py-1 rounded-full ${getStatusColor(property.category)}`}>
                    {property.category}
                  </span>
                </td>
                <td className="px-6 py-4">
                  <span className="text-gray-300 text-sm">{property.createdBy}</span>
                </td>
                <td className="px-6 py-4">
                  <span className={`text-xs px-3 py-1 rounded-full ${getStatusColor(property.status)}`}>
                    {property.status}
                  </span>
                </td>
                <td className="px-6 py-4">
                  <span className="text-green-400 font-semibold text-sm">{property.price}</span>
                </td>
                <td className="px-6 py-4">
                  <div className="flex gap-2">
                    <button className="w-8 h-8 bg-blue-600 hover:bg-blue-500 rounded flex items-center justify-center transition-colors">
                      <Edit className="w-4 h-4 text-white" />
                    </button>
                    <button className="w-8 h-8 bg-red-600 hover:bg-red-500 rounded-full flex items-center justify-center transition-colors">
                      <Trash2 className="w-4 h-4 text-white" />
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
