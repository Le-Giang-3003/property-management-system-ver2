import { Home, Search, Settings, LayoutDashboard, Building2, MessageSquare, LogOut, FileText, CreditCard, Wrench, Users, DollarSign, BarChart3, Cog } from 'lucide-react';
import { Link, useLocation } from 'react-router';

interface SidebarProps {
  role: 'tenant' | 'landlord' | 'admin';
  onLogout?: () => void;
}

export function Sidebar({ role, onLogout }: SidebarProps) {
  const location = useLocation();
  
  const tenantMenuItems = [
    { icon: LayoutDashboard, label: 'Dashboard', path: '/tenant/dashboard', id: 'tenant-dashboard' },
    { icon: Search, label: 'Tìm Thuê Nhà', path: '/tenant/search', id: 'tenant-search' },
    { icon: FileText, label: 'Đơn xin thuê', path: '/tenant/applications', id: 'tenant-applications' },
    { icon: Building2, label: 'Hợp đồng', path: '/tenant/leases', id: 'tenant-leases' },
    { icon: CreditCard, label: 'Thanh toán', path: '/tenant/payments', id: 'tenant-payments' },
    { icon: Wrench, label: 'Yêu cầu bảo trì UI', path: '/tenant/maintenance', id: 'tenant-maintenance' },
    { icon: MessageSquare, label: 'Chat', path: '/tenant/chat', id: 'tenant-chat' },
  ];

  const landlordMenuItems = [
    { icon: LayoutDashboard, label: 'Dashboard', path: '/landlord/dashboard', id: 'landlord-dashboard' },
    { icon: Building2, label: 'Bất động sản', path: '/landlord/properties', id: 'landlord-properties' },
    { icon: FileText, label: 'Đơn xin thuê', path: '/landlord/applications', id: 'landlord-applications' },
    { icon: FileText, label: 'Hợp đồng', path: '/landlord/leases', id: 'landlord-contracts' },
    { icon: CreditCard, label: 'Thanh toán', path: '/landlord/payments', id: 'landlord-payments' },
    { icon: Wrench, label: 'Yêu cầu bảo trì', path: '/landlord/maintenance', id: 'landlord-requests' },
    { icon: MessageSquare, label: 'Chat', path: '/landlord/chat', id: 'landlord-chat' },
  ];

  const adminMenuItems = [
    { icon: LayoutDashboard, label: 'Dashboard', path: '/admin/dashboard', id: 'admin-dashboard' },
    { icon: Building2, label: 'Quản lý BĐS', path: '/admin/properties', id: 'admin-properties' },
    { icon: Users, label: 'Quản lý Users', path: '/admin/users', id: 'admin-users' },
    { icon: DollarSign, label: 'Thanh toán', path: '/admin/payments', id: 'admin-payments' },
    { icon: BarChart3, label: 'Báo cáo', path: '/admin/reports', id: 'admin-reports' },
    { icon: MessageSquare, label: 'Chat', path: '/admin/chat', id: 'admin-chat' },
    { icon: Cog, label: 'Cấu hình', path: '/admin/config', id: 'admin-settings' },
  ];

  const menuItems = role === 'tenant' ? tenantMenuItems : role === 'landlord' ? landlordMenuItems : adminMenuItems;

  return (
    <div className="w-64 bg-[#1a1d2e] min-h-screen flex flex-col">
      <div className="p-6 border-b border-gray-700">
        <div className="flex items-center gap-3">
          <div className="w-10 h-10 bg-blue-600 rounded-lg flex items-center justify-center">
            <Building2 className="w-6 h-6 text-white" />
          </div>
          <div>
            <h1 className="text-white font-semibold text-lg">PropertyME</h1>
          </div>
        </div>
      </div>

      <nav className="flex-1 p-4">
        <div className="space-y-1">
          {menuItems.map((item) => {
            const Icon = item.icon;
            const isActive = location.pathname === item.path;
            
            return (
              <Link
                key={item.id}
                to={item.path}
                className={`flex items-center gap-3 px-4 py-3 rounded-lg transition-colors ${
                  isActive
                    ? 'bg-blue-600 text-white'
                    : 'text-gray-400 hover:bg-gray-800 hover:text-white'
                }`}
              >
                <Icon className="w-5 h-5" />
                <span className="text-sm">{item.label}</span>
              </Link>
            );
          })}
        </div>
      </nav>

      <div className="p-4 border-t border-gray-700">
        <div className="flex items-center gap-3 px-4 py-3 mb-3">
          <div className="w-8 h-8 bg-blue-600 rounded-full flex items-center justify-center text-white text-sm font-semibold">
            {role === 'tenant' ? 'LM' : role === 'landlord' ? 'TU' : 'AS'}
          </div>
          <div className="flex-1 min-w-0">
            <p className="text-white text-sm font-medium truncate">
              {role === 'tenant' ? 'LÂM MIỀN Chí4' : role === 'landlord' ? 'Tariq-Iqbab Th...' : 'Admin System'}
            </p>
            <p className="text-gray-400 text-xs truncate">
              {role === 'tenant' ? 'thttps://localhost:7184-7' : role === 'landlord' ? 'https://localhost:7184-7' : 'admin@property.me'}
            </p>
          </div>
        </div>
        <button
          onClick={onLogout}
          className="w-full flex items-center gap-3 px-4 py-3 rounded-lg text-gray-400 hover:bg-red-900/20 hover:text-red-400 transition-colors"
        >
          <LogOut className="w-5 h-5" />
          <span className="text-sm font-medium">Đăng xuất</span>
        </button>
      </div>
    </div>
  );
}