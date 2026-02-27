import { useState, useRef } from "react";
import { useLocation } from "react-router";
import { leases } from "../../utils/mockData";
import { formatMoney, formatDate, getStatusBadge } from "../../utils/helpers";
import { 
  FileText, 
  Eye, 
  Plus, 
  User, 
  CreditCard, 
  Calendar, 
  DollarSign, 
  Clock, 
  Percent, 
  FileCheck,
  Building2,
  Phone,
  Mail,
  Hash,
  PenTool,
  Eraser,
  Download
} from "lucide-react";

export default function Leases({ role = 'Admin' }) {
  const location = useLocation();
  const [selectedLease, setSelectedLease] = useState(null);
  const [statusFilter, setStatusFilter] = useState('All');
  const [showCreateModal, setShowCreateModal] = useState(location.state?.createContract || false);
  const [showSignModal, setShowSignModal] = useState(false);
  const applicationData = location.state?.applicationData || null;
  
  // Canvas refs for signatures
  const canvasRef = useRef(null);
  const [isDrawing, setIsDrawing] = useState(false);
  
  const [contractForm, setContractForm] = useState({
    tenantName: applicationData?.tenantName || '',
    tenantPhone: applicationData?.tenantPhone || '',
    tenantAddress: applicationData?.propertyTitle || '',
    startDate: applicationData?.moveInDate || '',
    endDate: '',
    monthlyRent: '',
    deposit: '',
    paymentDay: '5',
    lateFeePercent: '',
    terms: '',
    notes: ''
  });

  const myLeases = role === 'Tenant'
    ? leases.filter(l => l.tenantId === 4)
    : role === 'Landlord'
    ? leases.filter(l => l.landlordId === 2)
    : leases;

  const filtered = statusFilter === 'All' ? myLeases : myLeases.filter(l => l.status === statusFilter);

  // Canvas drawing functions
  const startDrawing = (e) => {
    if (!canvasRef.current) return;
    setIsDrawing(true);
    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    const rect = canvas.getBoundingClientRect();
    ctx.beginPath();
    ctx.moveTo(e.clientX - rect.left, e.clientY - rect.top);
  };

  const draw = (e) => {
    if (!isDrawing || !canvasRef.current) return;
    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    const rect = canvas.getBoundingClientRect();
    ctx.lineTo(e.clientX - rect.left, e.clientY - rect.top);
    ctx.strokeStyle = '#3b82f6';
    ctx.lineWidth = 2;
    ctx.lineCap = 'round';
    ctx.lineJoin = 'round';
    ctx.stroke();
  };

  const stopDrawing = () => {
    setIsDrawing(false);
  };

  const clearCanvas = () => {
    if (!canvasRef.current) return;
    const canvas = canvasRef.current;
    const ctx = canvas.getContext('2d');
    ctx.clearRect(0, 0, canvas.width, canvas.height);
  };

  const saveSignature = () => {
    setShowSignModal(false);
    setSelectedLease(null);
  };

  return (
    <div>
      <div className="flex items-center justify-between mb-20">
        <div>
          <div className="page-title">Hợp đồng thuê</div>
          <div className="page-desc">{role === 'Admin' ? 'Tất cả hợp đồng trong hệ thống' : role === 'Landlord' ? 'Các hợp đồng của BDS bạn cho thuê' : 'Các hợp đồng thuê của bạn'}</div>
        </div>
        {role === 'Landlord' && <button className="btn btn-primary" onClick={() => setShowCreateModal(true)}><Plus size={16}/> Tạo hợp đồng</button>}
      </div>

      <div className="stat-grid" style={{ gridTemplateColumns: 'repeat(4, 1fr)', marginBottom: 20 }}>
        {['Active', 'Pending', 'Expired', 'Terminated'].map((s, i) => (
          <div key={s} className="stat-card">
            <div className={`stat-icon ${['green','yellow','blue','red'][i]}`}><FileText size={20}/></div>
            <div className="stat-info">
              <div className="stat-label">{s === 'Active' ? 'Hiệu lực' : s === 'Pending' ? 'Chờ ký' : s === 'Expired' ? 'Hết hạn' : 'Chấm dứt'}</div>
              <div className="stat-value">{myLeases.filter(l => l.status === s).length}</div>
            </div>
          </div>
        ))}
      </div>

      <div className="filter-bar">
        {['All','Active','Pending','Expired','Terminated'].map(s => (
          <button key={s} className={`btn ${statusFilter === s ? 'btn-primary' : 'btn-ghost'} btn-sm`} onClick={() => setStatusFilter(s)}>
            {s === 'All' ? 'Tất cả' : s === 'Active' ? '🟢 Hiệu lực' : s === 'Pending' ? '🟡 Chờ ký' : s === 'Expired' ? '⚫ Hết hạn' : '🔴 Chấm dứt'}
          </button>
        ))}
      </div>

      <div className="card">
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>Mã HĐ</th>
                <th>BDS</th>
                {role !== 'Tenant' && <th>Người thuê</th>}
                {role !== 'Landlord' && <th>Chủ nhà</th>}
                <th>Trạng thái</th>
                <th>Chữ ký</th>
                <th>Tiền thuê</th>
                <th>Thời hạn</th>
                <th>Xem</th>
              </tr>
            </thead>
            <tbody>
              {filtered.map(l => (
                <tr key={l.id}>
                  <td><strong style={{ color: 'var(--accent-light)' }}>{l.leaseNumber}</strong></td>
                  <td style={{ maxWidth: 160, fontSize: 12 }}>{l.propertyTitle}</td>
                  {role !== 'Tenant' && <td>{l.tenantName}</td>}
                  {role !== 'Landlord' && <td>{l.landlordName}</td>}
                  <td>{getStatusBadge(l.status)}</td>
                  <td>
                    <div style={{ fontSize: 11 }}>
                      {l.landlordSigned && l.tenantSigned ? '✅ Cả hai' : !l.landlordSigned ? '⏳ Chủ nhà' : '⏳ Người thuê'}
                    </div>
                  </td>
                  <td className="text-green fw-600">{formatMoney(l.monthlyRent)}</td>
                  <td className="text-muted" style={{ fontSize: 12 }}>{formatDate(l.startDate)}<br/>→ {formatDate(l.endDate)}</td>
                  <td><button className="btn btn-ghost btn-sm btn-icon" onClick={() => setSelectedLease(l)}><Eye size={13}/></button></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {selectedLease && !showSignModal && (
        <div className="modal-overlay" onClick={() => setSelectedLease(null)}>
          <div className="modal contract-modal" style={{ maxWidth: 720 }} onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">
                <FileText size={20} /> Chi tiết hợp đồng
              </span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setSelectedLease(null)}>✕</button>
            </div>
            <div className="modal-body">
              {/* Contract Number Header */}
              <div style={{ 
                display: 'flex', 
                justifyContent: 'space-between', 
                alignItems: 'center',
                marginBottom: 20, 
                padding: 16, 
                background: 'linear-gradient(135deg, rgba(59, 130, 246, 0.15) 0%, rgba(99, 102, 241, 0.15) 100%)',
                border: '1px solid rgba(99, 102, 241, 0.3)',
                borderRadius: 8 
              }}>
                <div>
                  <div className="text-muted text-sm mb-4" style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                    <Hash size={14} />
                    <span>MÃ HỢP ĐỒNG</span>
                  </div>
                  <div className="fw-700" style={{ fontSize: 22, color: 'var(--accent-light)' }}>{selectedLease.leaseNumber}</div>
                </div>
                <div>
                  {getStatusBadge(selectedLease.status)}
                </div>
              </div>

              {/* Property Information */}
              <div className="contract-info-section">
                <div className="contract-info-header">
                  <Building2 size={18} />
                  <span>Thông tin bất động sản</span>
                </div>
                <div className="contract-info-content">
                  <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                    {selectedLease.propertyThumbnail && (
                      <img
                        src={selectedLease.propertyThumbnail}
                        alt=""
                        style={{
                          width: 60,
                          height: 60,
                          borderRadius: 8,
                          objectFit: 'cover',
                        }}
                      />
                    )}
                    <div>
                      <div className="fw-600">{selectedLease.propertyTitle}</div>
                      <div className="text-muted text-sm">Địa chỉ: {selectedLease.propertyAddress || 'Hà Nội, Việt Nam'}</div>
                    </div>
                  </div>
                </div>
              </div>

              {/* Tenant Information */}
              <div className="contract-info-section">
                <div className="contract-info-header">
                  <User size={18} />
                  <span>Thông tin người thuê (Bên B)</span>
                </div>
                <div className="contract-info-content">
                  <div className="grid-2" style={{ gap: 8 }}>
                    <div className="contract-info-item">
                      <User size={14} className="contract-info-icon" />
                      <span>{selectedLease.tenantName}</span>
                    </div>
                    <div className="contract-info-item">
                      <Phone size={14} className="contract-info-icon" />
                      <span>{selectedLease.tenantPhone || '0909-XXX-XXX'}</span>
                    </div>
                    <div className="contract-info-item">
                      <Mail size={14} className="contract-info-icon" />
                      <span>{selectedLease.tenantEmail || 'tenant@email.com'}</span>
                    </div>
                    <div className="contract-info-item">
                      <CreditCard size={14} className="contract-info-icon" />
                      <span>CCCD: {selectedLease.tenantIdCard || '001234567891'}</span>
                    </div>
                  </div>
                </div>
              </div>

              {/* Landlord Information */}
              <div className="contract-info-section">
                <div className="contract-info-header">
                  <User size={18} />
                  <span>Thông tin chủ nhà (Bên A)</span>
                </div>
                <div className="contract-info-content">
                  <div className="grid-2" style={{ gap: 8 }}>
                    <div className="contract-info-item">
                      <User size={14} className="contract-info-icon" />
                      <span>{selectedLease.landlordName}</span>
                    </div>
                    <div className="contract-info-item">
                      <Phone size={14} className="contract-info-icon" />
                      <span>0912-345-678</span>
                    </div>
                    <div className="contract-info-item">
                      <Mail size={14} className="contract-info-icon" />
                      <span>landlord@propertyme.vn</span>
                    </div>
                    <div className="contract-info-item">
                      <CreditCard size={14} className="contract-info-icon" />
                      <span>CCCD: 001234567890</span>
                    </div>
                  </div>
                </div>
              </div>

              {/* Contract Details */}
              <div className="contract-section" style={{ marginTop: 24 }}>
                <div className="contract-section-title">Chi tiết hợp đồng</div>
              </div>

              <div className="grid-2">
                <div>
                  <div className="info-row">
                    <span className="info-label"><Calendar size={14} /> Ngày bắt đầu</span>
                    <span className="info-value">{formatDate(selectedLease.startDate)}</span>
                  </div>
                  <div className="info-row">
                    <span className="info-label"><Calendar size={14} /> Ngày kết thúc</span>
                    <span className="info-value">{formatDate(selectedLease.endDate)}</span>
                  </div>
                  <div className="info-row">
                    <span className="info-label"><DollarSign size={14} /> Tiền thuê</span>
                    <span className="info-value text-green fw-700">{formatMoney(selectedLease.monthlyRent)}</span>
                  </div>
                </div>
                <div>
                  <div className="info-row">
                    <span className="info-label"><CreditCard size={14} /> Tiền cọc</span>
                    <span className="info-value">{formatMoney(selectedLease.depositAmount)}</span>
                  </div>
                  <div className="info-row">
                    <span className="info-label"><Clock size={14} /> Ngày thanh toán</span>
                    <span className="info-value">Ngày {selectedLease.paymentDueDay}/tháng</span>
                  </div>
                  <div className="info-row">
                    <span className="info-label"><Percent size={14} /> Phí trễ hạn</span>
                    <span className="info-value">{selectedLease.lateFeePercentage}%/tháng</span>
                  </div>
                </div>
              </div>

              {selectedLease.terms && (
                <div className="info-row">
                  <span className="info-label">Điều khoản hợp đồng</span>
                  <span className="info-value text-sm" style={{ whiteSpace: 'pre-wrap' }}>{selectedLease.terms}</span>
                </div>
              )}

              {selectedLease.specialConditions && (
                <div className="info-row">
                  <span className="info-label">Điều kiện đặc biệt</span>
                  <span className="info-value text-sm" style={{ whiteSpace: 'pre-wrap' }}>{selectedLease.specialConditions}</span>
                </div>
              )}

              {/* Signature Status */}
              <div style={{ padding: 16, background: 'var(--bg-primary)', borderRadius: 8, marginTop: 20, border: '1px solid var(--border)' }}>
                <div className="fw-600 mb-12 text-sm" style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                  <PenTool size={16} />
                  <span>TÌNH TRẠNG CHỮ KÝ</span>
                </div>
                <div style={{ display: 'flex', gap: 12 }}>
                  <div style={{ 
                    flex: 1, 
                    padding: 14, 
                    background: selectedLease.landlordSigned ? 'rgba(16, 185, 129, 0.1)' : 'var(--bg-secondary)',
                    border: `2px solid ${selectedLease.landlordSigned ? 'var(--success)' : 'var(--border)'}`, 
                    borderRadius: 8, 
                    textAlign: 'center' 
                  }}>
                    <div style={{ fontSize: 28, marginBottom: 8 }}>{selectedLease.landlordSigned ? '✅' : '⏳'}</div>
                    <div className="text-sm fw-600 mb-4">Chủ nhà</div>
                    <div className="text-muted" style={{ fontSize: 11 }}>
                      {selectedLease.landlordSigned ? formatDate(selectedLease.landlordSignedAt) : 'Chưa ký'}
                    </div>
                  </div>
                  <div style={{ 
                    flex: 1, 
                    padding: 14, 
                    background: selectedLease.tenantSigned ? 'rgba(16, 185, 129, 0.1)' : 'var(--bg-secondary)',
                    border: `2px solid ${selectedLease.tenantSigned ? 'var(--success)' : 'var(--border)'}`, 
                    borderRadius: 8, 
                    textAlign: 'center' 
                  }}>
                    <div style={{ fontSize: 28, marginBottom: 8 }}>{selectedLease.tenantSigned ? '✅' : '⏳'}</div>
                    <div className="text-sm fw-600 mb-4">Người thuê</div>
                    <div className="text-muted" style={{ fontSize: 11 }}>
                      {selectedLease.tenantSigned ? formatDate(selectedLease.tenantSignedAt) : 'Chưa ký'}
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setSelectedLease(null)}>Đóng</button>
              {((role === 'Tenant' && !selectedLease.tenantSigned) || (role === 'Landlord' && !selectedLease.landlordSigned)) && (
                <button 
                  className="btn btn-primary"
                  onClick={() => setShowSignModal(true)}
                >
                  <PenTool size={16} /> Ký hợp đồng
                </button>
              )}
            </div>
          </div>
        </div>
      )}

      {showSignModal && (
        <div className="modal-overlay" onClick={() => setShowSignModal(false)}>
          <div className="modal" style={{ maxWidth: 580 }} onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">Ký hợp đồng</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setShowSignModal(false)}>✕</button>
            </div>
            <div className="modal-body">
              <div className="contract-section">
                <div className="contract-section-title">Hợp đồng thuê: {selectedLease.leaseNumber}</div>
                <div className="contract-info-content">
                  <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                    {selectedLease.propertyThumbnail && (
                      <img
                        src={selectedLease.propertyThumbnail}
                        alt=""
                        style={{
                          width: 60,
                          height: 60,
                          borderRadius: 8,
                          objectFit: 'cover',
                        }}
                      />
                    )}
                    <div>
                      <div className="fw-600">{selectedLease.propertyTitle}</div>
                      <div className="text-muted text-sm">Địa chỉ: {selectedLease.propertyAddress || 'Hà Nội, Việt Nam'}</div>
                    </div>
                  </div>
                </div>
              </div>

              <div className="contract-section">
                <div className="contract-section-title">Thông tin người thuê (Bên B)</div>
                <div className="contract-info-content">
                  <div className="grid-2" style={{ gap: 8 }}>
                    <div className="contract-info-item">
                      <User size={14} className="contract-info-icon" />
                      <span>{selectedLease.tenantName}</span>
                    </div>
                    <div className="contract-info-item">
                      <Phone size={14} className="contract-info-icon" />
                      <span>{selectedLease.tenantPhone || '0909-XXX-XXX'}</span>
                    </div>
                    <div className="contract-info-item">
                      <Mail size={14} className="contract-info-icon" />
                      <span>{selectedLease.tenantEmail || 'tenant@email.com'}</span>
                    </div>
                    <div className="contract-info-item">
                      <CreditCard size={14} className="contract-info-icon" />
                      <span>CCCD: {selectedLease.tenantIdCard || '001234567891'}</span>
                    </div>
                  </div>
                </div>
              </div>

              <div className="contract-section">
                <div className="contract-section-title">Thông tin chủ nhà (Bên A)</div>
                <div className="contract-info-content">
                  <div className="grid-2" style={{ gap: 8 }}>
                    <div className="contract-info-item">
                      <User size={14} className="contract-info-icon" />
                      <span>{selectedLease.landlordName}</span>
                    </div>
                    <div className="contract-info-item">
                      <Phone size={14} className="contract-info-icon" />
                      <span>0912-345-678</span>
                    </div>
                    <div className="contract-info-item">
                      <Mail size={14} className="contract-info-icon" />
                      <span>landlord@propertyme.vn</span>
                    </div>
                    <div className="contract-info-item">
                      <CreditCard size={14} className="contract-info-icon" />
                      <span>CCCD: 001234567890</span>
                    </div>
                  </div>
                </div>
              </div>

              <div className="contract-section">
                <div className="contract-section-title">Chi tiết hợp đồng</div>
                <div className="grid-2">
                  <div>
                    <div className="info-row">
                      <span className="info-label"><Calendar size={14} /> Ngày bắt đầu</span>
                      <span className="info-value">{formatDate(selectedLease.startDate)}</span>
                    </div>
                    <div className="info-row">
                      <span className="info-label"><Calendar size={14} /> Ngày kết thúc</span>
                      <span className="info-value">{formatDate(selectedLease.endDate)}</span>
                    </div>
                    <div className="info-row">
                      <span className="info-label"><DollarSign size={14} /> Tiền thuê</span>
                      <span className="info-value text-green fw-700">{formatMoney(selectedLease.monthlyRent)}</span>
                    </div>
                  </div>
                  <div>
                    <div className="info-row">
                      <span className="info-label"><CreditCard size={14} /> Tiền cọc</span>
                      <span className="info-value">{formatMoney(selectedLease.depositAmount)}</span>
                    </div>
                    <div className="info-row">
                      <span className="info-label"><Clock size={14} /> Ngày thanh toán</span>
                      <span className="info-value">Ngày {selectedLease.paymentDueDay}/tháng</span>
                    </div>
                    <div className="info-row">
                      <span className="info-label"><Percent size={14} /> Phí trễ hạn</span>
                      <span className="info-value">{selectedLease.lateFeePercentage}%/tháng</span>
                    </div>
                  </div>
                </div>
              </div>

              <div className="contract-section">
                <div className="contract-section-title">Ký tên</div>
                <div className="contract-signature">
                  <canvas
                    ref={canvasRef}
                    width={500}
                    height={100}
                    style={{ border: '1px solid #ccc' }}
                    onMouseDown={startDrawing}
                    onMouseMove={draw}
                    onMouseUp={stopDrawing}
                    onMouseLeave={stopDrawing}
                  />
                  <div className="contract-signature-buttons">
                    <button className="btn btn-ghost btn-sm" onClick={clearCanvas}>Xóa</button>
                    <button className="btn btn-primary btn-sm" onClick={saveSignature}>Lưu chữ ký</button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

      {showCreateModal && (
        <div className="modal-overlay" onClick={() => setShowCreateModal(false)}>
          <div className="modal" style={{ maxWidth: 580 }} onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <span className="modal-title">Chấp nhận đơn & Tạo hợp đồng</span>
              <button className="modal-close btn btn-ghost btn-sm btn-icon" onClick={() => setShowCreateModal(false)}>✕</button>
            </div>
            <div className="modal-body">
              {/* Thông tin khách thuê */}
              <div className="contract-section">
                <div className="contract-section-title">Thông tin khách thuê:</div>
                <div className="contract-tenant-info">
                  <div className="fw-600 text-sm">{contractForm.tenantName}</div>
                  <div className="text-muted text-sm">SĐT: {contractForm.tenantPhone} | Bắt động sản: {contractForm.tenantAddress}</div>
                </div>
              </div>

              {/* Bắt đầu thuê từ & Ngày kết thúc */}
              <div className="form-row">
                <div className="form-group-with-icon">
                  <label className="form-label">BẮT ĐẦU THUÊ TỪ <span className="text-red">*</span></label>
                  <div className="input-icon-wrapper">
                    <Calendar size={16} className="input-icon" />
                    <input 
                      className="form-control-with-icon" 
                      type="date"
                      value={contractForm.startDate}
                      onChange={e => setContractForm({...contractForm, startDate: e.target.value})}
                    />
                  </div>
                </div>
                <div className="form-group-with-icon">
                  <label className="form-label">NGÀY KẾT THÚC <span className="text-red">*</span></label>
                  <div className="input-icon-wrapper">
                    <Calendar size={16} className="input-icon" />
                    <input 
                      className="form-control-with-icon" 
                      type="date"
                      value={contractForm.endDate}
                      onChange={e => setContractForm({...contractForm, endDate: e.target.value})}
                    />
                  </div>
                </div>
              </div>

              {/* Tiền thuê hàng tháng & Tiền cọc */}
              <div className="form-row">
                <div className="form-group-with-icon">
                  <label className="form-label">TIỀN THUÊ HÀNG THÁNG (VNĐ) <span className="text-red">*</span></label>
                  <div className="input-icon-wrapper">
                    <DollarSign size={16} className="input-icon" />
                    <input 
                      className="form-control-with-icon" 
                      type="number"
                      placeholder="0"
                      value={contractForm.monthlyRent}
                      onChange={e => setContractForm({...contractForm, monthlyRent: e.target.value})}
                    />
                  </div>
                </div>
                <div className="form-group-with-icon">
                  <label className="form-label">TIỀN CỌC (VNĐ) <span className="text-red">*</span></label>
                  <div className="input-icon-wrapper">
                    <CreditCard size={16} className="input-icon" />
                    <input 
                      className="form-control-with-icon" 
                      type="number"
                      placeholder="0"
                      value={contractForm.deposit}
                      onChange={e => setContractForm({...contractForm, deposit: e.target.value})}
                    />
                  </div>
                </div>
              </div>

              {/* Ngày thanh toán hàng tháng & Phí phạt trả chậm */}
              <div className="form-row">
                <div className="form-group-with-icon">
                  <label className="form-label">NGÀY THANH TOÁN HÀNG THÁNG (VD: MÙNG 5)</label>
                  <div className="input-icon-wrapper">
                    <Clock size={16} className="input-icon" />
                    <input 
                      className="form-control-with-icon" 
                      type="number"
                      min="1"
                      max="31"
                      placeholder="5"
                      value={contractForm.paymentDay}
                      onChange={e => setContractForm({...contractForm, paymentDay: e.target.value})}
                    />
                  </div>
                </div>
                <div className="form-group-with-icon">
                  <label className="form-label">PHÍ PHẠT TRẢ CHẬM (%)</label>
                  <div className="input-icon-wrapper">
                    <Percent size={16} className="input-icon" />
                    <input 
                      className="form-control-with-icon" 
                      type="number"
                      step="0.1"
                      placeholder="0"
                      value={contractForm.lateFeePercent}
                      onChange={e => setContractForm({...contractForm, lateFeePercent: e.target.value})}
                    />
                  </div>
                </div>
              </div>

              {/* Điều khoản hợp đồng */}
              <div className="form-group-with-icon">
                <label className="form-label">ĐIỀU KHOẢN HỢP ĐỒNG <span className="text-red">*</span></label>
                <textarea 
                  className="form-control" 
                  rows={4}
                  placeholder="Nội dung điều khoản..."
                  value={contractForm.terms}
                  onChange={e => setContractForm({...contractForm, terms: e.target.value})}
                />
              </div>

              {/* Lưu ý / Điều kiện đặc biệt */}
              <div className="form-group-with-icon">
                <label className="form-label">LƯU Ý / ĐIỀU KIỆN ĐẶC BIỆT</label>
                <textarea 
                  className="form-control" 
                  rows={3}
                  placeholder="Nội dung lưu ý hoặc điều kiện đặc biệt..."
                  value={contractForm.notes}
                  onChange={e => setContractForm({...contractForm, notes: e.target.value})}
                />
              </div>
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setShowCreateModal(false)}>Hủy</button>
              <button className="btn btn-primary" onClick={() => setShowCreateModal(false)}>
                <FileCheck size={16} /> Lưu và tạo hợp đồng
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}