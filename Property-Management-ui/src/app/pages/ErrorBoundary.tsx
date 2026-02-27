import { AlertTriangle } from 'lucide-react';
import { useRouteError } from 'react-router';

export function ErrorBoundary() {
  const error = useRouteError() as any;
  
  return (
    <div className="min-h-screen flex items-center justify-center bg-[#13161f]">
      <div className="text-center max-w-md">
        <div className="w-24 h-24 bg-red-600/20 rounded-full flex items-center justify-center mx-auto mb-6">
          <AlertTriangle className="w-12 h-12 text-red-500" />
        </div>
        <h1 className="text-white text-3xl font-bold mb-3">Đã xảy ra lỗi</h1>
        <p className="text-gray-400 text-lg mb-6">
          {error?.message || error?.statusText || 'Đã xảy ra lỗi không mong muốn'}
        </p>
        <a href="/" className="px-6 py-3 bg-blue-600 hover:bg-blue-500 text-white rounded-lg font-medium transition-colors inline-block">
          Về trang chủ
        </a>
      </div>
    </div>
  );
}
