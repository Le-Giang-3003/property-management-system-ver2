import { createBrowserRouter } from 'react-router';
import { Layout } from './components/Layout';
import { TenantPage } from './pages/TenantPage';
import { LandlordPage } from './pages/LandlordPage';
import { AdminPage } from './pages/AdminPage';
import { PlaceholderPage } from './pages/PlaceholderPage';
import { NotFoundPage } from './pages/NotFoundPage';
import { ErrorBoundary } from './pages/ErrorBoundary';

export const router = createBrowserRouter([
  {
    path: '/',
    element: <Layout role="tenant" />,
    errorElement: <ErrorBoundary />,
    children: [
      {
        index: true,
        element: <TenantPage />,
      },
    ],
  },
  {
    path: '/tenant',
    element: <Layout role="tenant" />,
    errorElement: <ErrorBoundary />,
    children: [
      {
        index: true,
        element: <TenantPage />,
      },
      {
        path: 'requests',
        element: <PlaceholderPage title="Gửi yêu cầu" description="Trang này đang được phát triển" />,
      },
      {
        path: 'payments',
        element: <PlaceholderPage title="Trả tiền" description="Trang này đang được phát triển" />,
      },
      {
        path: 'chat',
        element: <PlaceholderPage title="Chat" description="Trang này đang được phát triển" />,
      },
    ],
  },
  {
    path: '/landlord',
    element: <Layout role="landlord" />,
    errorElement: <ErrorBoundary />,
    children: [
      {
        index: true,
        element: <LandlordPage />,
      },
      {
        path: 'properties',
        element: <PlaceholderPage title="Gửi của tôi" description="Trang này đang được phát triển" />,
      },
      {
        path: 'contracts',
        element: <PlaceholderPage title="Hợp đồng" description="Trang này đang được phát triển" />,
      },
      {
        path: 'payments',
        element: <PlaceholderPage title="Thanh toán" description="Trang này đang được phát triển" />,
      },
      {
        path: 'requests',
        element: <PlaceholderPage title="Yêu cầu từ UI" description="Trang này đang được phát triển" />,
      },
      {
        path: 'chat',
        element: <PlaceholderPage title="Chat" description="Trang này đang được phát triển" />,
      },
    ],
  },
  {
    path: '/admin',
    element: <Layout role="admin" />,
    errorElement: <ErrorBoundary />,
    children: [
      {
        index: true,
        element: <AdminPage />,
      },
      {
        path: 'users',
        element: <PlaceholderPage title="Quản lý Users" description="Trang này đang được phát triển" />,
      },
      {
        path: 'payments',
        element: <PlaceholderPage title="Thanh toán" description="Trang này đang được phát triển" />,
      },
      {
        path: 'reports',
        element: <PlaceholderPage title="Báo cáo" description="Trang này đang được phát triển" />,
      },
      {
        path: 'settings',
        element: <PlaceholderPage title="Cấu hình hệ thống" description="Trang này đang được phát triển" />,
      },
    ],
  },
  {
    path: '*',
    element: <NotFoundPage />,
  },
]);