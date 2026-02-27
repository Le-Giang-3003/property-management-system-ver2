import { Search, Filter, Plus, Bell, User } from 'lucide-react';
import { PropertyCard } from '../components/PropertyCard';

const properties = [
  {
    id: 1,
    title: 'Căn hộ thứ đúc 8',
    category: 'Căn hộ',
    area: 30,
    bedrooms: 2,
    bathrooms: 1,
    price: 30000,
    status: 'Đã thuê',
    image: 'https://images.unsplash.com/photo-1594873604892-b599f847e859?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxtb2Rlcm4lMjBhcGFydG1lbnQlMjBpbnRlcmlvcnxlbnwxfHx8fDE3NzIxMTA3NjV8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
  },
  {
    id: 2,
    title: 'Căn hộ thứ đúc 6',
    category: 'Phòng trọ',
    area: 25,
    bedrooms: 2,
    bathrooms: 1,
    price: 10000000,
    status: 'Đã thuê',
    image: 'https://images.unsplash.com/photo-1561518801-3a7645cf0256?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxsdXh1cnklMjBhcGFydG1lbnQlMjBidWlsZGluZyUyMHBvb2x8ZW58MXx8fHwxNzcyMTY3MTg2fDA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
  },
  {
    id: 3,
    title: 'Căn hộ thứ đúc 7',
    category: 'Phòng trọ',
    area: 30,
    bedrooms: 2,
    bathrooms: 2,
    price: 8000000,
    status: 'Phòng trống',
    image: 'https://images.unsplash.com/photo-1561518801-3a7645cf0256?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxsdXh1cnklMjBhcGFydG1lbnQlMjBidWlsZGluZyUyMHBvb2x8ZW58MXx8fHwxNzcyMTY3MTg2fDA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
  },
  {
    id: 4,
    title: 'Căn hộ thứ đúc 8',
    category: 'Phòng trọ',
    area: 35,
    bedrooms: 2,
    bathrooms: 2,
    price: 20000000,
    status: 'Đang dùng',
    image: 'https://images.unsplash.com/photo-1652882860902-7c6b0f88ef23?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxhcGFydG1lbnQlMjBiZWRyb29tJTIwbWluaW1hbGlzdHxlbnwxfHx8fDE3NzIxNjcxODZ8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
  },
  {
    id: 5,
    title: 'Căn hộ thứ đúc 5',
    category: 'Căn hộ',
    area: 40,
    bedrooms: 3,
    bathrooms: 2,
    price: 30000000,
    status: 'Đã thuê',
    image: 'https://images.unsplash.com/photo-1688888019305-27d5de949222?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxjaXR5JTIwYXBhcnRtZW50JTIwZXh0ZXJpb3J8ZW58MXx8fHwxNzcyMDUyOTEyfDA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
  },
  {
    id: 6,
    title: 'Căn hộ thứ đúc 4',
    category: 'Phòng trọ',
    area: 28,
    bedrooms: 2,
    bathrooms: 1,
    price: 20000000,
    status: 'Đang dùng',
    image: 'https://images.unsplash.com/photo-1703929119947-72e4d83b00a8?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxoaWdoJTIwcmlzZSUyMGFwYXJ0bWVudHxlbnwxfHx8fDE3NzIxNjcxODd8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
  },
  {
    id: 7,
    title: 'Căn hộ thứ đúc 1',
    category: 'Phòng trọ',
    area: 32,
    bedrooms: 2,
    bathrooms: 1,
    price: 7000000,
    status: 'Phòng trống',
    image: 'https://images.unsplash.com/photo-1612419299101-6c294dc2901d?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxhcGFydG1lbnQlMjBsaXZpbmclMjByb29tJTIwY296eXxlbnwxfHx8fDE3NzIxNjcxODd8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
  },
  {
    id: 8,
    title: 'Căn hộ thứ đúc 2',
    category: 'Mái phẳt',
    area: 45,
    bedrooms: 3,
    bathrooms: 2,
    price: 12000000,
    status: 'Sẵn sàng',
    image: 'https://images.unsplash.com/photo-1594873604892-b599f847e859?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxtb2Rlcm4lMjBhcGFydG1lbnQlMjBpbnRlcmlvcnxlbnwxfHx8fDE3NzIxMTA3NjV8MA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
  },
  {
    id: 9,
    title: 'Căn hộ thứ đúc 3',
    category: 'Phòng trọ',
    area: 30,
    bedrooms: 2,
    bathrooms: 1,
    price: 3500000,
    status: 'Phòng trống',
    image: 'https://images.unsplash.com/photo-1561518801-3a7645cf0256?crop=entropy&cs=tinysrgb&fit=max&fm=jpg&ixid=M3w3Nzg4Nzd8MHwxfHNlYXJjaHwxfHxsdXh1cnklMjBhcGFydG1lbnQlMjBidWlsZGluZyUyMHBvb2x8ZW58MXx8fHwxNzcyMTY3MTg2fDA&ixlib=rb-4.1.0&q=80&w=1080&utm_source=figma&utm_medium=referral',
  },
];

export function TenantPage() {
  return (
    <div className="min-h-screen">
      {/* Header */}
      <div className="bg-[#1a1d2e] border-b border-gray-700">
        <div className="px-8 py-4">
          <div className="flex items-center justify-between">
            <div>
              <h1 className="text-white text-2xl font-semibold mb-1">Quản lý Bất động sản</h1>
              <p className="text-gray-400 text-sm">Quản lý tất cả các bất động sản một cách dễ dàng</p>
            </div>
            
            <div className="flex items-center gap-4">
              <div className="flex items-center gap-2">
                <span className="text-gray-400 text-sm">80%</span>
                <button className="text-white hover:text-gray-300 transition-colors">
                  <Plus className="w-5 h-5" />
                </button>
              </div>
              <button className="px-4 py-2 bg-blue-600 hover:bg-blue-500 text-white rounded-lg text-sm font-medium transition-colors">
                Email
              </button>
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
          <h2 className="text-white text-xl font-semibold mb-1">Bất động sản của bạn</h2>
          <p className="text-gray-400 text-sm">Danh sách các BĐS bạn đang có kh 24/12/2025</p>
        </div>

        {/* Filters */}
        <div className="flex items-center justify-between mb-6">
          <div className="flex items-center gap-3">
            <button className="px-4 py-2 bg-blue-600 text-white rounded-lg text-sm font-medium hover:bg-blue-500 transition-colors">
              Tất cả (25)
            </button>
            <button className="px-4 py-2 bg-[#252a3d] text-gray-300 rounded-lg text-sm hover:bg-gray-700 transition-colors">
              Đang dùng (11)
            </button>
            <button className="px-4 py-2 bg-[#252a3d] text-gray-300 rounded-lg text-sm hover:bg-gray-700 transition-colors">
              Đã thuê (11)
            </button>
            <button className="px-4 py-2 bg-[#252a3d] text-gray-300 rounded-lg text-sm hover:bg-gray-700 transition-colors">
              Đã xoay (11)
            </button>
            <button className="px-4 py-2 bg-[#252a3d] text-gray-300 rounded-lg text-sm hover:bg-gray-700 transition-colors">
              Để duyệt (11)
            </button>
            <button className="px-4 py-2 bg-[#252a3d] text-gray-300 rounded-lg text-sm hover:bg-gray-700 transition-colors">
              Từ Chấm (11)
            </button>
            <button className="px-4 py-2 bg-[#252a3d] text-gray-300 rounded-lg text-sm hover:bg-gray-700 transition-colors">
              Bị khóa (11)
            </button>
          </div>

          <div className="flex items-center gap-3">
            <button className="px-4 py-2 bg-[#252a3d] text-white rounded-lg text-sm hover:bg-gray-700 transition-colors flex items-center gap-2">
              <Filter className="w-4 h-4" />
              Bộ lọc
            </button>
          </div>
        </div>

        {/* Property Grid */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
          {properties.map((property) => (
            <PropertyCard key={property.id} {...property} />
          ))}
        </div>
      </div>
    </div>
  );
}
