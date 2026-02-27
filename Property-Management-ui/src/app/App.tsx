import { useState } from "react";
import {
  BrowserRouter,
  Routes,
  Route,
  Navigate,
} from "react-router";
import { Sidebar, Header } from "./components/Layout";
import Login from "./pages/Login";
import Register from "./pages/Register";

// Admin pages
import AdminDashboard from "./pages/admin/AdminDashboard";
import AdminProperties from "./pages/admin/AdminProperties";
import AdminUsers from "./pages/admin/AdminUsers";
import AdminReports from "./pages/admin/AdminReports";
import AdminConfig from "./pages/admin/AdminConfig";

// Landlord pages
import LandlordDashboard from "./pages/landlord/LandlordDashboard";
import LandlordProperties from "./pages/landlord/LandlordProperties";
import LandlordLeases from "./pages/landlord/LandlordLeases";

// Tenant pages
import TenantDashboard from "./pages/tenant/TenantDashboard";
import PropertySearch from "./pages/tenant/PropertySearch";
import MyProperties from "./pages/tenant/MyProperties";

// Shared pages
import Payments from "./pages/shared/Payments";
import Maintenance from "./pages/shared/Maintenance";
import Bookings from "./pages/shared/Bookings";
import Applications from "./pages/shared/Applications";
import Chat from "./pages/shared/Chat";
import Leases from "./pages/shared/Leases";

function AppLayout({ role, onLogout }) {
  const getTitle = (role) => {
    if (role === "admin") return "Admin Panel";
    if (role === "landlord") return "Chủ nhà";
    return "Người thuê";
  };
  
  const roleKey = role.toLowerCase();

  return (
    <div className="app-layout">
      <Sidebar role={roleKey} onLogout={onLogout} />
      <div className="main-content">
        <Header title={getTitle(roleKey)} />
        <div className="page-body">
          <Routes>
            {role === "Admin" && (
              <>
                <Route
                  path="/admin/dashboard"
                  element={<AdminDashboard />}
                />
                <Route
                  path="/admin/properties"
                  element={<AdminProperties />}
                />
                <Route
                  path="/admin/users"
                  element={<AdminUsers />}
                />
                <Route
                  path="/admin/leases"
                  element={<Leases role="Admin" />}
                />
                <Route
                  path="/admin/payments"
                  element={<Payments role="Admin" />}
                />
                <Route
                  path="/admin/maintenance"
                  element={<Maintenance role="Admin" />}
                />
                <Route
                  path="/admin/reports"
                  element={<AdminReports />}
                />
                <Route
                  path="/admin/config"
                  element={<AdminConfig />}
                />
                <Route
                  path="/admin/chat"
                  element={<Chat />}
                />
                <Route
                  path="*"
                  element={
                    <Navigate to="/admin/dashboard" replace />
                  }
                />
              </>
            )}

            {role === "Landlord" && (
              <>
                <Route
                  path="/landlord/dashboard"
                  element={<LandlordDashboard />}
                />
                <Route
                  path="/landlord/properties"
                  element={<LandlordProperties />}
                />
                <Route
                  path="/landlord/applications"
                  element={<Applications role="Landlord" />}
                />
                <Route
                  path="/landlord/leases"
                  element={<LandlordLeases />}
                />
                <Route
                  path="/landlord/payments"
                  element={<Payments role="Landlord" />}
                />
                <Route
                  path="/landlord/maintenance"
                  element={<Maintenance role="Landlord" />}
                />
                <Route
                  path="/landlord/bookings"
                  element={<Bookings role="Landlord" />}
                />
                <Route
                  path="/landlord/chat"
                  element={<Chat />}
                />
                <Route
                  path="*"
                  element={
                    <Navigate
                      to="/landlord/dashboard"
                      replace
                    />
                  }
                />
              </>
            )}

            {role === "Tenant" && (
              <>
                <Route
                  path="/tenant/dashboard"
                  element={<TenantDashboard />}
                />
                <Route
                  path="/tenant/search"
                  element={<PropertySearch />}
                />
                <Route
                  path="/tenant/my-properties"
                  element={<MyProperties />}
                />
                <Route
                  path="/tenant/applications"
                  element={<Applications role="Tenant" />}
                />
                <Route
                  path="/tenant/leases"
                  element={<Leases role="Tenant" />}
                />
                <Route
                  path="/tenant/payments"
                  element={<Payments role="Tenant" />}
                />
                <Route
                  path="/tenant/maintenance"
                  element={<Maintenance role="Tenant" />}
                />
                <Route
                  path="/tenant/bookings"
                  element={<Bookings role="Tenant" />}
                />
                <Route path="/tenant/chat" element={<Chat />} />
                <Route
                  path="*"
                  element={
                    <Navigate to="/tenant/dashboard" replace />
                  }
                />
              </>
            )}
          </Routes>
        </div>
      </div>
    </div>
  );
}

export default function App() {
  const [role, setRole] = useState(null); // null = not logged in

  return (
    <BrowserRouter>
      {!role ? (
        <Routes>
          <Route path="/login" element={<Login onLogin={(r) => setRole(r)} />} />
          <Route path="/register" element={<Register />} />
          <Route path="*" element={<Navigate to="/login" replace />} />
        </Routes>
      ) : (
        <AppLayout role={role} onLogout={() => setRole(null)} />
      )}
    </BrowserRouter>
  );
}