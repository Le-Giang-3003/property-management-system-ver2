import { useState } from "react";
import { useNavigate } from "react-router";
import { UserPlus, ArrowLeft } from "lucide-react";

export default function Register() {
  const navigate = useNavigate();
  const [form, setForm] = useState({
    fullName: "",
    email: "",
    password: "",
    confirmPassword: "",
    phone: "",
    role: "Tenant", // Default role
  });

  const [errors, setErrors] = useState({});

  const validateForm = () => {
    const newErrors: any = {};

    if (!form.fullName.trim()) {
      newErrors.fullName = "Vui lòng nhập họ tên";
    }

    if (!form.email.trim()) {
      newErrors.email = "Vui lòng nhập email";
    } else if (!/\S+@\S+\.\S+/.test(form.email)) {
      newErrors.email = "Email không hợp lệ";
    }

    if (!form.password) {
      newErrors.password = "Vui lòng nhập mật khẩu";
    } else if (form.password.length < 6) {
      newErrors.password = "Mật khẩu phải có ít nhất 6 ký tự";
    }

    if (form.password !== form.confirmPassword) {
      newErrors.confirmPassword = "Mật khẩu xác nhận không khớp";
    }

    if (!form.phone.trim()) {
      newErrors.phone = "Vui lòng nhập số điện thoại";
    } else if (!/^[0-9]{10}$/.test(form.phone)) {
      newErrors.phone = "Số điện thoại phải có 10 chữ số";
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    
    if (validateForm()) {
      // Here you would typically send data to backend
      console.log("Register form data:", form);
      
      // Show success message and redirect to login
      alert(`Đăng ký thành công với vai trò ${form.role === "Tenant" ? "Người thuê" : "Chủ nhà"}! Vui lòng đăng nhập.`);
      navigate("/login");
    }
  };

  const roleOptions = [
    {
      value: "Tenant",
      label: "Người thuê",
      icon: "👤",
      color: "#3b82f6",
      desc: "Tôi đang tìm kiếm bất động sản để thuê",
    },
    {
      value: "Landlord",
      label: "Chủ nhà",
      icon: "🏠",
      color: "#10b981",
      desc: "Tôi có bất động sản muốn cho thuê",
    },
  ];

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
          maxWidth: 520,
          position: "relative",
          zIndex: 1,
        }}
      >
        {/* Back to Login Link */}
        <button
          onClick={() => navigate("/login")}
          style={{
            display: "flex",
            alignItems: "center",
            gap: 8,
            background: "transparent",
            border: "none",
            color: "var(--text-muted)",
            fontSize: 14,
            cursor: "pointer",
            marginBottom: 20,
            padding: "8px 12px",
            borderRadius: 8,
            transition: "all 0.2s",
          }}
          onMouseEnter={(e) => {
            e.currentTarget.style.background = "var(--bg-hover)";
            e.currentTarget.style.color = "var(--text-primary)";
          }}
          onMouseLeave={(e) => {
            e.currentTarget.style.background = "transparent";
            e.currentTarget.style.color = "var(--text-muted)";
          }}
        >
          <ArrowLeft size={16} />
          Quay lại đăng nhập
        </button>

        {/* Logo */}
        <div style={{ textAlign: "center", marginBottom: 32 }}>
          <div
            style={{
              width: 60,
              height: 60,
              background: "linear-gradient(135deg, #6366f1, #8b5cf6)",
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
              background: "linear-gradient(135deg, #6366f1, #818cf8)",
              WebkitBackgroundClip: "text",
              WebkitTextFillColor: "transparent",
              marginBottom: 6,
            }}
          >
            Tạo tài khoản
          </h1>
          <p style={{ color: "var(--text-muted)", fontSize: 14 }}>
            Đăng ký để sử dụng PropertyMS
          </p>
        </div>

        {/* Register Form */}
        <form
          onSubmit={handleSubmit}
          style={{
            background: "var(--bg-secondary)",
            border: "1px solid var(--border)",
            borderRadius: 16,
            padding: 24,
          }}
        >
          {/* Role Selection */}
          <div className="form-group">
            <label className="form-label">Bạn là *</label>
            <div style={{ display: "flex", gap: 12 }}>
              {roleOptions.map((option) => (
                <button
                  key={option.value}
                  type="button"
                  onClick={() => setForm({ ...form, role: option.value })}
                  style={{
                    flex: 1,
                    display: "flex",
                    flexDirection: "column",
                    alignItems: "center",
                    gap: 8,
                    padding: 16,
                    background:
                      form.role === option.value
                        ? `${option.color}15`
                        : "var(--bg-card)",
                    border: `2px solid ${form.role === option.value ? option.color : "var(--border)"}`,
                    borderRadius: 12,
                    cursor: "pointer",
                    transition: "all 0.2s",
                  }}
                >
                  <div
                    style={{
                      fontSize: 32,
                      marginBottom: 4,
                    }}
                  >
                    {option.icon}
                  </div>
                  <div
                    style={{
                      fontWeight: 600,
                      fontSize: 14,
                      color: "var(--text-primary)",
                    }}
                  >
                    {option.label}
                  </div>
                  <div
                    style={{
                      fontSize: 11,
                      color: "var(--text-muted)",
                      textAlign: "center",
                      lineHeight: 1.4,
                    }}
                  >
                    {option.desc}
                  </div>
                </button>
              ))}
            </div>
          </div>

          {/* Full Name */}
          <div className="form-group">
            <label className="form-label">Họ và tên *</label>
            <input
              className="form-control"
              type="text"
              value={form.fullName}
              onChange={(e) =>
                setForm({ ...form, fullName: e.target.value })
              }
              placeholder="Nguyễn Văn A"
            />
            {errors.fullName && (
              <div
                style={{
                  color: "var(--danger)",
                  fontSize: 12,
                  marginTop: 6,
                }}
              >
                {errors.fullName}
              </div>
            )}
          </div>

          {/* Email */}
          <div className="form-group">
            <label className="form-label">Email *</label>
            <input
              className="form-control"
              type="email"
              value={form.email}
              onChange={(e) => setForm({ ...form, email: e.target.value })}
              placeholder="email@example.com"
            />
            {errors.email && (
              <div
                style={{
                  color: "var(--danger)",
                  fontSize: 12,
                  marginTop: 6,
                }}
              >
                {errors.email}
              </div>
            )}
          </div>

          {/* Phone */}
          <div className="form-group">
            <label className="form-label">Số điện thoại *</label>
            <input
              className="form-control"
              type="tel"
              value={form.phone}
              onChange={(e) => setForm({ ...form, phone: e.target.value })}
              placeholder="0912345678"
            />
            {errors.phone && (
              <div
                style={{
                  color: "var(--danger)",
                  fontSize: 12,
                  marginTop: 6,
                }}
              >
                {errors.phone}
              </div>
            )}
          </div>

          {/* Password */}
          <div className="form-group">
            <label className="form-label">Mật khẩu *</label>
            <input
              className="form-control"
              type="password"
              value={form.password}
              onChange={(e) =>
                setForm({ ...form, password: e.target.value })
              }
              placeholder="••••••••"
            />
            {errors.password && (
              <div
                style={{
                  color: "var(--danger)",
                  fontSize: 12,
                  marginTop: 6,
                }}
              >
                {errors.password}
              </div>
            )}
          </div>

          {/* Confirm Password */}
          <div className="form-group">
            <label className="form-label">Xác nhận mật khẩu *</label>
            <input
              className="form-control"
              type="password"
              value={form.confirmPassword}
              onChange={(e) =>
                setForm({ ...form, confirmPassword: e.target.value })
              }
              placeholder="••••••••"
            />
            {errors.confirmPassword && (
              <div
                style={{
                  color: "var(--danger)",
                  fontSize: 12,
                  marginTop: 6,
                }}
              >
                {errors.confirmPassword}
              </div>
            )}
          </div>

          {/* Submit Button */}
          <button
            type="submit"
            className="btn btn-primary"
            style={{
              width: "100%",
              justifyContent: "center",
              padding: "12px",
              fontSize: 14,
              marginTop: 8,
            }}
          >
            <UserPlus size={16} />
            Tạo tài khoản
          </button>

          {/* Login Link */}
          <div
            style={{
              textAlign: "center",
              marginTop: 16,
              fontSize: 13,
              color: "var(--text-muted)",
            }}
          >
            Đã có tài khoản?{" "}
            <span
              onClick={() => navigate("/login")}
              style={{
                color: "var(--accent)",
                fontWeight: 600,
                cursor: "pointer",
              }}
            >
              Đăng nhập ngay
            </span>
          </div>
        </form>

        <div
          style={{
            textAlign: "center",
            marginTop: 16,
            fontSize: 12,
            color: "var(--text-muted)",
          }}
        >
          © 2026 PropertyMS — PRN222 Demo Application
        </div>
      </div>
    </div>
  );
}
