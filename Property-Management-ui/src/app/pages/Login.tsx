import { useState } from "react";
import { useNavigate } from "react-router";

export default function Login({ onLogin }) {
  const navigate = useNavigate();
  const [email, setEmail] = useState("admin@propms.vn");
  const [password, setPassword] = useState("admin123");
  const [selectedRole, setSelectedRole] = useState("Admin");

  const roles = [
    {
      role: "Admin",
      email: "admin@propms.vn",
      name: "Nguyễn Văn Admin",
      desc: "Quản trị viên hệ thống",
      icon: "🛡️",
      color: "#6366f1",
    },
    {
      role: "Landlord",
      email: "landlord1@gmail.com",
      name: "Trần Thị Lan",
      desc: "Chủ nhà · 4 BDS đang cho thuê",
      icon: "🏠",
      color: "#10b981",
    },
    {
      role: "Tenant",
      email: "tenant1@gmail.com",
      name: "Phạm Thị Hoa",
      desc: "Người thuê · HĐ đang hiệu lực",
      icon: "👤",
      color: "#3b82f6",
    },
  ];

  const handleQuickLogin = (role) => {
    setSelectedRole(role.role);
    setEmail(role.email);
    onLogin(role.role);
  };

  return (
    <div
      style={{
        minHeight: "100vh",
        background: "var(--bg-primary)",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        padding: 20,
      }}
    >
      {/* Background Orbs */}
      <div
        style={{
          position: "fixed",
          top: "10%",
          left: "15%",
          width: 300,
          height: 300,
          background:
            "radial-gradient(circle, rgba(99,102,241,0.15) 0%, transparent 70%)",
          pointerEvents: "none",
        }}
      />
      <div
        style={{
          position: "fixed",
          bottom: "20%",
          right: "10%",
          width: 400,
          height: 400,
          background:
            "radial-gradient(circle, rgba(16,185,129,0.1) 0%, transparent 70%)",
          pointerEvents: "none",
        }}
      />

      <div
        style={{
          width: "100%",
          maxWidth: 480,
          position: "relative",
          zIndex: 1,
        }}
      >
        {/* Logo */}
        <div style={{ textAlign: "center", marginBottom: 32 }}>
          <div
            style={{
              width: 60,
              height: 60,
              background:
                "linear-gradient(135deg, #6366f1, #8b5cf6)",
              borderRadius: 16,
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              fontSize: 28,
              margin: "0 auto 12px",
              boxShadow: "0 8px 32px rgba(99,102,241,0.4)",
            }}
          >
            🏢
          </div>
          <h1
            style={{
              fontSize: 28,
              fontWeight: 800,
              background:
                "linear-gradient(135deg, #6366f1, #818cf8)",
              WebkitBackgroundClip: "text",
              WebkitTextFillColor: "transparent",
              marginBottom: 6,
            }}
          >
            PropertyMS
          </h1>
          <p
            style={{ color: "var(--text-muted)", fontSize: 14 }}
          >
            Hệ thống quản lý bất động sản
          </p>
        </div>

        {/* Quick Login Demo */}
        <div
          style={{
            background: "var(--bg-secondary)",
            border: "1px solid var(--border)",
            borderRadius: 16,
            padding: 24,
            marginBottom: 24,
          }}
        >
          <div
            style={{
              fontSize: 12,
              fontWeight: 600,
              color: "var(--text-muted)",
              textTransform: "uppercase",
              letterSpacing: 1,
              marginBottom: 14,
            }}
          >
            🎮 Đăng nhập nhanh (Demo)
          </div>
          <div
            style={{
              display: "flex",
              flexDirection: "column",
              gap: 10,
            }}
          >
            {roles.map((r) => (
              <button
                key={r.role}
                onClick={() => handleQuickLogin(r)}
                style={{
                  display: "flex",
                  alignItems: "center",
                  gap: 12,
                  padding: "14px 16px",
                  background:
                    selectedRole === r.role
                      ? `rgba(99,102,241,0.1)`
                      : "var(--bg-card)",
                  border: `1px solid ${selectedRole === r.role ? "var(--accent)" : "var(--border)"}`,
                  borderRadius: 10,
                  cursor: "pointer",
                  transition: "all 0.2s",
                  textAlign: "left",
                  width: "100%",
                  color: "var(--text-primary)",
                }}
              >
                <div
                  style={{
                    width: 40,
                    height: 40,
                    borderRadius: 10,
                    background: `${r.color}20`,
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    fontSize: 18,
                    flexShrink: 0,
                  }}
                >
                  {r.icon}
                </div>
                <div style={{ flex: 1 }}>
                  <div
                    style={{ fontWeight: 600, fontSize: 14 }}
                  >
                    {r.name}
                  </div>
                  <div
                    style={{
                      fontSize: 12,
                      color: "var(--text-muted)",
                    }}
                  >
                    {r.desc}
                  </div>
                </div>
                <div
                  style={{
                    fontSize: 11,
                    color: r.color,
                    fontWeight: 700,
                    padding: "3px 8px",
                    background: `${r.color}20`,
                    borderRadius: 6,
                  }}
                >
                  {r.role}
                </div>
              </button>
            ))}
          </div>
        </div>

        {/* Manual Login */}
        <div
          style={{
            background: "var(--bg-secondary)",
            border: "1px solid var(--border)",
            borderRadius: 16,
            padding: 24,
          }}
        >
          <div
            style={{
              fontSize: 12,
              fontWeight: 600,
              color: "var(--text-muted)",
              textTransform: "uppercase",
              letterSpacing: 1,
              marginBottom: 16,
            }}
          >
            Đăng nhập thủ công
          </div>
          <div className="form-group">
            <label className="form-label">Email</label>
            <input
              className="form-control"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="email@example.com"
            />
          </div>
          <div className="form-group">
            <label className="form-label">Mật khẩu</label>
            <input
              className="form-control"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="••••••••"
            />
          </div>
          <button
            onClick={() => onLogin(selectedRole)}
            className="btn btn-primary"
            style={{
              width: "100%",
              justifyContent: "center",
              padding: "12px",
              fontSize: 14,
            }}
          >
            Đăng nhập →
          </button>
          
          {/* Register Link */}
          <div
            style={{
              textAlign: "center",
              marginTop: 16,
              fontSize: 13,
              color: "var(--text-muted)",
            }}
          >
            Chưa có tài khoản?{" "}
            <span
              onClick={() => navigate("/register")}
              style={{
                color: "var(--accent)",
                fontWeight: 600,
                cursor: "pointer",
              }}
            >
              Tạo tài khoản ngay
            </span>
          </div>
        </div>
      </div>
    </div>
  );
}