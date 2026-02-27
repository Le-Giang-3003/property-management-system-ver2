import { Outlet } from 'react-router';
import { Sidebar } from './Sidebar';

interface LayoutProps {
  role: 'tenant' | 'landlord' | 'admin';
}

interface HeaderProps {
  title: string;
}

export function Header({ title }: HeaderProps) {
  return (
    <header className="bg-[#1a1d2e] border-b border-gray-700 px-8 py-4">
      <div className="flex items-center justify-between">
        <h1 className="text-white text-2xl font-semibold">{title}</h1>
        <div className="flex items-center gap-4">
          {/* Add any header actions here if needed */}
        </div>
      </div>
    </header>
  );
}

export function Layout({ role }: LayoutProps) {
  return (
    <div className="flex min-h-screen bg-[#13161f]">
      <Sidebar role={role} />
      <main className="flex-1">
        <Outlet />
      </main>
    </div>
  );
}

export { Sidebar };