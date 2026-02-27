import { Search, Filter, Plus, Bell, User, FileText, Trash2 } from 'lucide-react';
import { PropertyTable } from '../components/PropertyTable';

const properties = [
  {
    id: 9,
    image: 'https://images.unsplash.com/photo-1594873604892-b599f847e859?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxtb2Rlcm4lMjBhcGFydG1lbnQlMjBpbnRlcmlvcnxlbnwxfHx8fDE3NzIxMTA3NjV8MA&ixlib=rb-4.1.0&q=80&w=400',
    title: 'Căn hộ thứ đúc 9',
    category: 'Căn hộ',
    createdBy: 'LÂM MIẾN Chí4',
    status: 'Đã thuê',
    price: '28 đ',
  },
  {
    id: 8,
    image: 'https://images.unsplash.com/photo-1561518801-3a7645cf0256?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxsdXh1cnklMjBhcGFydG1lbnQlMjBidWlsZGluZyUyMHBvb2x8ZW58MXx8fHwxNzcyMTY3MTg2fDA&ixlib=rb-4.1.0&q=80&w=400',
    title: 'Căn hộ thứ đúc 8',
    category: 'Phòng trọ',
    createdBy: 'LÂM MIẾN Chí4',
    status: 'Đang dùng',
    price: '10,000,000 đ',
  },
  {
    id: 7,
    image: 'https://images.unsplash.com/photo-1561518801-3a7645cf0256?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxsdXh1cnklMjBhcGFydG1lbnQlMjBidWlsZGluZyUyMHBvb2x8ZW58MXx8fHwxNzcyMTY3MTg2fDA&ixlib=rb-4.1.0&q=80&w=400',
    title: 'Căn hộ thứ đúc 7',
    category: 'Căn hộ quanh',
    createdBy: 'LÂM MIẾN Chí4',
    status: 'Đang dùng',
    price: '8,000,000 đ',
  },
  {
    id: 6,
    image: 'https://images.unsplash.com/photo-1688888019305-27d5de949222?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxjaXR5JTIwYXBhcnRtZW50JTIwZXh0ZXJpb3J8ZW58MXx8fHwxNzcyMDUyOTEyfDA&ixlib=rb-4.1.0&q=80&w=400',
    title: 'Căn hộ thứ đúc 6',
    category: 'Phòng trọ',
    createdBy: 'LÂM MIẾN Chí4',
    status: 'Đang dùng',
    price: '20,000,000 đ',
  },
  {
    id: 5,
    image: 'https://images.unsplash.com/photo-1652882860902-7c6b0f88ef23?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxhcGFydG1lbnQlMjBiZWRyb29tJTIwbWluaW1hbGlzdHxlbnwxfHx8fDE3NzIxNjcxODZ8MA&ixlib=rb-4.1.0&q=80&w=400',
    title: 'Căn hộ thứ đúc 5',
    category: 'Căn hộ',
    createdBy: 'LÂM MIẾN Chí4',
    status: 'Đang dùng',
    price: '30,000,000 đ',
  },
  {
    id: 4,
    image: 'https://images.unsplash.com/photo-1703929119947-72e4d83b00a8?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxoaWdoJTIwcmlzZSUyMGFwYXJ0bWVudHxlbnwxfHx8fDE3NzIxNjcxODd8MA&ixlib=rb-4.1.0&q=80&w=400',
    title: 'Căn hộ thứ đúc 4',
    category: 'Phòng trọ',
    createdBy: 'LÂM MIẾN Chí4',
    status: 'Đang dùng',
    price: '20,000,000 đ',
  },
  {
    id: 3,
    image: 'https://images.unsplash.com/photo-1612419299101-6c294dc2901d?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxhcGFydG1lbnQlMjBsaXZpbmclMjByb29tJTIwY296eXxlbnwxfHx8fDE3NzIxNjcxODd8MA&ixlib=rb-4.1.0&q=80&w=400',
    title: 'Căn hộ thứ đúc 1',
    category: 'Căn hộ quanh',
    createdBy: 'LÂM MIẾN Chí4',
    status: 'Đang dùng',
    price: '7,000,000 đ',
  },
  {
    id: 2,
    image: 'https://images.unsplash.com/photo-1594873604892-b599f847e859?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxtb2Rlcm4lMjBhcGFydG1lbnQlMjBpbnRlcmlvcnxlbnwxfHx8fDE3NzIxMTA3NjV8MA&ixlib=rb-4.1.0&q=80&w=400',
    title: 'Căn hộ thứ đúc 2',
    category: 'Mái phẳt',
    createdBy: 'LÂM MIẾN Chí4',
    status: 'Đang dùng',
    price: '12,000,000 đ',
  },
  {
    id: 1,
    image: 'https://images.unsplash.com/photo-1561518801-3a7645cf0256?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxsdXh1cnklMjBhcGFydG1lbnQlMjBidWlsZGluZyUyMHBvb2x8ZW58MXx8fHwxNzcyMTY3MTg2fDA&ixlib=rb-4.1.0&q=80&w=400',
    title: 'Căn hộ thứ đúc 3',
    category: 'Phòng trọ',
    createdBy: 'LÂM MIẾN Chí4',
    status: 'Phòng trống',
    price: '3,500,000 đ',
  },
];

export function AdminPage() {
  return (
    <div className="min-h-screen">
      {/* Header */}
      <div className="bg-[#1a1d2e] border-b border-gray-700">
        <div className="px-8 py-4">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-white text-2xl font-semibold">Quản lý Bất động sản</h1>
            </div>
            
            <div className="flex items-center gap-4">
              <button className="relative text-white hover:text-gray-300 transition-colors">
                <Bell className="w-5 h-5" />
              </button>
              <button className="text-white hover:text-gray-300 transition-colors">
                <User className="w-5 h-5" />
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Content */}
      <div className="p-8">
        {/* Subtitle */}
        <div className="mb-6">
          <h2 className="text-white text-xl font-semibold mb-1">Quản lý Bất động sản</h2>
          <p className="text-gray-400 text-sm">Quản lý tất cả các bất động sản có trong hệ thống</p>
        </div>

        {/* Search and Actions */}
        <div className="flex items-center justify-between mb-6">
          <div className="flex-1 max-w-2xl">
            <div className="relative">
              <Search className="absolute left-4 top-1/2 transform -translate-y-1/2 w-5 h-5 text-gray-400" />
              <input
                type="text"
                placeholder="Tìm kiếm theo tên nhà, địa chỉ..."
                className="w-full pl-12 pr-4 py-3 bg-[#252a3d] text-white rounded-lg border border-gray-700 focus:outline-none focus:ring-2 focus:ring-blue-500"
              />
            </div>
          </div>
          
          <div className="flex items-center gap-3 ml-4">
            <button className="px-4 py-3 bg-blue-600 hover:bg-blue-500 text-white rounded-lg font-medium transition-colors flex items-center gap-2">
              <Plus className="w-4 h-4" />
              Tạo BĐS
            </button>
            <button className="px-4 py-3 bg-[#252a3d] hover:bg-gray-700 text-white rounded-lg transition-colors flex items-center gap-2">
              <FileText className="w-4 h-4" />
              Cấm dữ liệu
            </button>
            <button className="px-4 py-3 bg-[#252a3d] hover:bg-gray-700 text-white rounded-lg transition-colors flex items-center gap-2">
              <Filter className="w-4 h-4" />
              Bộ lọc
            </button>
            <button className="px-4 py-3 bg-red-600/20 hover:bg-red-600/30 text-red-400 rounded-lg transition-colors flex items-center gap-2">
              <Trash2 className="w-4 h-4" />
              Xoá hàng
            </button>
            <button className="px-4 py-3 bg-[#252a3d] hover:bg-gray-700 text-white rounded-lg transition-colors">
              Tệ chế
            </button>
          </div>
        </div>

        {/* Filter Tabs */}
        <div className="flex items-center gap-3 mb-6">
          <button className="px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-500 transition-colors">
            Tất cả
          </button>
          <button className="px-4 py-2 bg-[#252a3d] text-gray-300 rounded-lg text-sm hover:bg-gray-700 transition-colors">
            Căm dùng
          </button>
          <button className="px-4 py-2 bg-[#252a3d] text-gray-300 rounded-lg text-sm hover:bg-gray-700 transition-colors">
            Đã thuê
          </button>
          <button className="px-4 py-2 bg-[#252a3d] text-gray-300 rounded-lg text-sm hover:bg-gray-700 transition-colors">
            Cho thuê lại
          </button>
          <button className="px-4 py-2 bg-[#252a3d] text-gray-300 rounded-lg text-sm hover:bg-gray-700 transition-colors">
            Từ chẩm
          </button>
          <button className="px-4 py-2 bg-[#252a3d] text-gray-300 rounded-lg text-sm hover:bg-gray-700 transition-colors">
            Bị khóa
          </button>
        </div>

        {/* Property Table */}
        <PropertyTable properties={properties} />
      </div>
    </div>
  );
}
