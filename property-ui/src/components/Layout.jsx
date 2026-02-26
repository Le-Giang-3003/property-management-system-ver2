import { useNavigate, useLocation } from 'react-router-dom';
import { currentUser } from '../data/mockData';
import {
  LayoutDashboard, Building2, FileText, CreditCard, Wrench,
  Calendar, Users, Settings, MessageSquare, Bell, LogOut,
  ClipboardList, BarChart3, ShieldCheck
} from 'lucide-react';

const adminNav = [
  { label: 'Dashboard', icon: LayoutDashboard, path: '/admin/dashboard' },
  { label: 'Quản lý BDS', icon: Building2, path: '/admin/properties' },
  { label: 'Quản lý Users', icon: Users, path: '/admin/users' },
  { label: 'Hợp đồng', icon: FileText, path: '/admin/leases' },
  { label: 'Thanh toán', icon: CreditCard, path: '/admin/payments' },
  { label: 'Bảo trì', icon: Wrench, path: '/admin/maintenance' },
  { label: 'Báo cáo', icon: BarChart3, path: '/admin/reports' },
  { label: 'Cấu hình hệ thống', icon: Settings, path: '/admin/config' },
];

const landlordNav = [
  { label: 'Dashboard', icon: LayoutDashboard, path: '/landlord/dashboard' },
  { label: 'BDS của tôi', icon: Building2, path: '/landlord/properties' },
  { label: 'Đơn xin thuê', icon: ClipboardList, path: '/landlord/applications', badge: 2 },
  { label: 'Hợp đồng', icon: FileText, path: '/landlord/leases' },
  { label: 'Thanh toán', icon: CreditCard, path: '/landlord/payments' },
  { label: 'Bảo trì', icon: Wrench, path: '/landlord/maintenance', badge: 2 },
  { label: 'Lịch xem nhà', icon: Calendar, path: '/landlord/bookings', badge: 1 },
  { label: 'Tin nhắn', icon: MessageSquare, path: '/landlord/chat', badge: 3 },
];

const tenantNav = [
  { label: 'Dashboard', icon: LayoutDashboard, path: '/tenant/dashboard' },
  { label: 'Tìm kiếm BDS', icon: Building2, path: '/tenant/search' },
  { label: 'Đơn xin thuê', icon: ClipboardList, path: '/tenant/applications' },
  { label: 'Hợp đồng', icon: FileText, path: '/tenant/leases' },
  { label: 'Thanh toán', icon: CreditCard, path: '/tenant/payments' },
  { label: 'Bảo trì', icon: Wrench, path: '/tenant/maintenance' },
  { label: 'Lịch xem nhà', icon: Calendar, path: '/tenant/bookings' },
  { label: 'Tin nhắn', icon: MessageSquare, path: '/tenant/chat', badge: 2 },
];

function Sidebar({ role }) {
  const navigate = useNavigate();
  const location = useLocation();
  const nav = role === 'Admin' ? adminNav : role === 'Landlord' ? landlordNav : tenantNav;

  return (
    <aside className="sidebar">
      <div className="sidebar-logo">
        <div className="sidebar-logo-icon">🏢</div>
        <div>
          <div className="sidebar-logo-text">PropMS</div>
          <div className="sidebar-logo-sub">Property Management</div>
        </div>
      </div>

      <nav className="sidebar-nav">
        <div className="nav-group">
          <div className="nav-group-label">
            {role === 'Admin' ? 'Quản trị' : role === 'Landlord' ? 'Chủ nhà' : 'Người thuê'}
          </div>
          {nav.map(item => (
            <div
              key={item.path}
              className={`nav-item ${location.pathname === item.path ? 'active' : ''}`}
              onClick={() => navigate(item.path)}
            >
              <item.icon size={18} />
              <span>{item.label}</span>
              {item.badge && <span className="nav-badge">{item.badge}</span>}
            </div>
          ))}
        </div>
      </nav>

      <div className="sidebar-footer">
        <div className="sidebar-user">
          <div className="user-avatar">{currentUser.fullName[0]}</div>
          <div className="user-info">
            <div className="user-name">{currentUser.fullName}</div>
            <div className="user-role">{role}</div>
          </div>
          <LogOut size={16} style={{ color: 'var(--text-muted)', flexShrink: 0 }} />
        </div>
      </div>
    </aside>
  );
}

function Header({ title, children }) {
  return (
    <div className="page-header">
      <h1 className="header-title">{title}</h1>
      <div className="header-actions">
        {children}
        <div className="icon-btn">
          <Bell size={16} />
          <div className="notification-dot" />
        </div>
        <div className="icon-btn">
          <div className="user-avatar" style={{ width: 28, height: 28, fontSize: 12 }}>
            {currentUser.fullName[0]}
          </div>
        </div>
      </div>
    </div>
  );
}

export { Sidebar, Header };
