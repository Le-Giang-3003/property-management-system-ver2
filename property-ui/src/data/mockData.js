// ===================== MOCK DATA =====================

export const currentUser = {
  id: 1,
  email: "admin@propms.vn",
  fullName: "Nguyễn Văn Admin",
  phoneNumber: "0901234567",
  role: "Admin",
  address: "123 Nguyễn Huệ, Q.1, TP.HCM",
  avatarUrl: null,
  isActive: true,
  isTenant: false,
  isLandlord: false,
  isEmailVerified: true,
  isPhoneVerified: true,
  isIdentityVerified: true,
  createdAt: "2024-01-01T00:00:00Z",
  lastLoginAt: "2026-02-26T01:30:00Z",
};

// Roles: Admin, Landlord, Tenant
export const roleContext = "Admin"; // Change to "Landlord" or "Tenant" to switch view

export const users = [
  { id: 1, email: "admin@propms.vn", fullName: "Nguyễn Văn Admin", phoneNumber: "0901234567", role: "Admin", isActive: true, isTenant: false, isLandlord: false, isEmailVerified: true, isIdentityVerified: true, createdAt: "2024-01-01", lastLoginAt: "2026-02-26" },
  { id: 2, email: "landlord1@gmail.com", fullName: "Trần Thị Lan", phoneNumber: "0912345678", role: "Member", isActive: true, isTenant: false, isLandlord: true, isEmailVerified: true, isIdentityVerified: true, createdAt: "2024-02-15", lastLoginAt: "2026-02-25" },
  { id: 3, email: "landlord2@gmail.com", fullName: "Lê Minh Tuấn", phoneNumber: "0923456789", role: "Member", isActive: true, isTenant: false, isLandlord: true, isEmailVerified: true, isIdentityVerified: false, createdAt: "2024-03-10", lastLoginAt: "2026-02-20" },
  { id: 4, email: "tenant1@gmail.com", fullName: "Phạm Thị Hoa", phoneNumber: "0934567890", role: "Member", isActive: true, isTenant: true, isLandlord: false, isEmailVerified: true, isIdentityVerified: true, createdAt: "2024-04-01", lastLoginAt: "2026-02-24" },
  { id: 5, email: "tenant2@gmail.com", fullName: "Hoàng Văn Bình", phoneNumber: "0945678901", role: "Member", isActive: true, isTenant: true, isLandlord: false, isEmailVerified: false, isIdentityVerified: false, createdAt: "2024-05-20", lastLoginAt: "2026-02-10" },
  { id: 6, email: "tenant3@gmail.com", fullName: "Ngô Thị Cúc", phoneNumber: "0956789012", role: "Member", isActive: false, isTenant: true, isLandlord: false, isEmailVerified: true, isIdentityVerified: false, createdAt: "2024-06-15", lastLoginAt: "2025-12-01" },
];

export const properties = [
  {
    id: 1, title: "Căn hộ cao cấp Vinhomes Central Park", description: "Căn hộ 2 phòng ngủ view sông thoáng đãng, đầy đủ nội thất cao cấp, an ninh 24/7.",
    propertyType: "Apartment", status: "Available", address: "720A Điện Biên Phủ", city: "TP.HCM", district: "Bình Thạnh", ward: "Phường 22",
    area: 75, bedrooms: 2, bathrooms: 2, floors: 25, monthlyRent: 18000000, depositAmount: 36000000, currency: "VND",
    amenities: '["Hồ bơi","Gym","Thang máy","Bãi đỗ xe","An ninh 24/7","Wifi"]',
    allowPets: false, allowSmoking: false, maxOccupants: 4, landlordId: 2,
    landlord: { id: 2, fullName: "Trần Thị Lan", phoneNumber: "0912345678", isIdentityVerified: true },
    images: [{ id: 1, imageUrl: "https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?w=800", isPrimary: true, displayOrder: 1 }],
    rejectionReason: null, createdAt: "2025-10-01", updatedAt: "2025-12-10", viewCount: 342,
  },
  {
    id: 2, title: "Nhà phố liền kề Thảo Điền", description: "Nhà phố 4 tầng, 4 phòng ngủ, khu vực yên tĩnh gần trường quốc tế.",
    propertyType: "House", status: "Rented", address: "45 Nguyễn Văn Hưởng", city: "TP.HCM", district: "Quận 2", ward: "Thảo Điền",
    area: 150, bedrooms: 4, bathrooms: 3, floors: 4, monthlyRent: 35000000, depositAmount: 70000000, currency: "VND",
    amenities: '["Sân thượng","Bãi đỗ xe","Camera an ninh"]',
    allowPets: true, allowSmoking: false, maxOccupants: 6, landlordId: 2,
    landlord: { id: 2, fullName: "Trần Thị Lan", phoneNumber: "0912345678", isIdentityVerified: true },
    images: [{ id: 2, imageUrl: "https://images.unsplash.com/photo-1449844908441-8829872d2607?w=800", isPrimary: true, displayOrder: 1 }],
    rejectionReason: null, createdAt: "2025-08-15", updatedAt: "2025-11-01", viewCount: 189,
  },
  {
    id: 3, title: "Phòng trọ giá rẻ Gò Vấp", description: "Phòng trọ sạch sẽ, có điều hoà, gần chợ Gò Vấp.",
    propertyType: "Room", status: "Pending", address: "12 Nguyễn Oanh", city: "TP.HCM", district: "Gò Vấp", ward: "Phường 17",
    area: 25, bedrooms: 1, bathrooms: 1, floors: 3, monthlyRent: 4500000, depositAmount: 9000000, currency: "VND",
    amenities: '["Điều hoà","Wifi","Giờ giấc tự do"]',
    allowPets: false, allowSmoking: false, maxOccupants: 2, landlordId: 3,
    landlord: { id: 3, fullName: "Lê Minh Tuấn", phoneNumber: "0923456789", isIdentityVerified: false },
    images: [{ id: 3, imageUrl: "https://images.unsplash.com/photo-1493809842364-78817add7ffb?w=800", isPrimary: true, displayOrder: 1 }],
    rejectionReason: null, createdAt: "2026-02-20", updatedAt: null, viewCount: 45,
  },
  {
    id: 4, title: "Studio hiện đại quận 7", description: "Studio mới xây, thiết kế thông minh, phù hợp cho 1-2 người.",
    propertyType: "Apartment", status: "Available", address: "88 Huỳnh Tấn Phát", city: "TP.HCM", district: "Quận 7", ward: "Phú Thuận",
    area: 40, bedrooms: 1, bathrooms: 1, floors: 12, monthlyRent: 8500000, depositAmount: 17000000, currency: "VND",
    amenities: '["Thang máy","Bãi đỗ xe","An ninh 24/7","Wifi"]',
    allowPets: false, allowSmoking: false, maxOccupants: 2, landlordId: 2,
    landlord: { id: 2, fullName: "Trần Thị Lan", phoneNumber: "0912345678", isIdentityVerified: true },
    images: [{ id: 4, imageUrl: "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=800", isPrimary: true, displayOrder: 1 }],
    rejectionReason: null, createdAt: "2025-12-01", updatedAt: "2026-01-15", viewCount: 210,
  },
  {
    id: 5, title: "Biệt thự vườn Bình Dương", description: "Biệt thự sân vườn rộng 500m2, hồ bơi riêng, phù hợp hộ gia đình lớn.",
    propertyType: "Villa", status: "Draft", address: "25 Huỳnh Văn Lũy", city: "Bình Dương", district: "Phú Mỹ", ward: null,
    area: 500, bedrooms: 5, bathrooms: 4, floors: 2, monthlyRent: 60000000, depositAmount: 120000000, currency: "VND",
    amenities: '["Hồ bơi","Sân vườn","Bếp BBQ","Garage","Camera an ninh"]',
    allowPets: true, allowSmoking: false, maxOccupants: 10, landlordId: 3,
    landlord: { id: 3, fullName: "Lê Minh Tuấn", phoneNumber: "0923456789", isIdentityVerified: false },
    images: [{ id: 5, imageUrl: "https://images.unsplash.com/photo-1564013799919-ab600027ffc6?w=800", isPrimary: true, displayOrder: 1 }],
    rejectionReason: null, createdAt: "2026-02-10", updatedAt: null, viewCount: 0,
  },
];

export const leases = [
  {
    id: 1, leaseNumber: "HD-2025-001", propertyId: 2, propertyTitle: "Nhà phố liền kề Thảo Điền", propertyAddress: "45 Nguyễn Văn Hưởng, Thảo Điền, Q.2",
    landlordId: 2, landlordName: "Trần Thị Lan", tenantId: 4, tenantName: "Phạm Thị Hoa",
    status: "Active", startDate: "2025-11-01", endDate: "2026-10-31", monthlyRent: 35000000, depositAmount: 70000000, currency: "VND",
    paymentDueDay: 5, lateFeePercentage: 2.5,
    terms: "Thanh toán đúng hạn mỗi tháng. Không gây tiếng ồn sau 22h. Giữ gìn vệ sinh chung.",
    specialConditions: "Được phép nuôi 1 con chó nhỏ",
    landlordSigned: true, landlordSignedAt: "2025-10-28T10:00:00Z",
    tenantSigned: true, tenantSignedAt: "2025-10-30T14:30:00Z",
    terminationReason: null, createdAt: "2025-10-25",
  },
  {
    id: 2, leaseNumber: "HD-2025-002", propertyId: 1, propertyTitle: "Căn hộ cao cấp Vinhomes Central Park", propertyAddress: "720A Điện Biên Phủ, P.22, Bình Thạnh",
    landlordId: 2, landlordName: "Trần Thị Lan", tenantId: 5, tenantName: "Hoàng Văn Bình",
    status: "Pending", startDate: "2026-03-01", endDate: "2027-02-28", monthlyRent: 18000000, depositAmount: 36000000, currency: "VND",
    paymentDueDay: 1, lateFeePercentage: 2,
    terms: "Thanh toán đúng hạn hàng tháng. Không hút thuốc trong căn hộ.",
    specialConditions: null,
    landlordSigned: true, landlordSignedAt: "2026-02-20T09:00:00Z",
    tenantSigned: false, tenantSignedAt: null,
    terminationReason: null, createdAt: "2026-02-18",
  },
  {
    id: 3, leaseNumber: "HD-2024-001", propertyId: 4, propertyTitle: "Studio hiện đại quận 7", propertyAddress: "88 Huỳnh Tấn Phát, Phú Thuận, Q.7",
    landlordId: 2, landlordName: "Trần Thị Lan", tenantId: 6, tenantName: "Ngô Thị Cúc",
    status: "Expired", startDate: "2024-03-01", endDate: "2025-02-28", monthlyRent: 8500000, depositAmount: 17000000, currency: "VND",
    paymentDueDay: 5, lateFeePercentage: 1.5,
    terms: "Thanh toán đúng hạn hàng tháng.",
    specialConditions: null,
    landlordSigned: true, landlordSignedAt: "2024-02-25T10:00:00Z",
    tenantSigned: true, tenantSignedAt: "2024-02-26T11:00:00Z",
    terminationReason: null, createdAt: "2024-02-20",
  },
];

export const payments = [
  { id: 1, leaseId: 1, leaseNumber: "HD-2025-001", propertyTitle: "Nhà phố liền kề Thảo Điền", tenantName: "Phạm Thị Hoa", landlordName: "Trần Thị Lan", paymentType: "MonthlyRent", status: "Completed", paymentMethod: "BankTransfer", amount: 35000000, currency: "VND", dueDate: "2026-02-05", paidDate: "2026-02-03", billingMonth: 2, billingYear: 2026, lateFeeAmount: null, description: "Tiền thuê tháng 2/2026", transactionId: "TXN20260203001", createdAt: "2026-01-25" },
  { id: 2, leaseId: 1, leaseNumber: "HD-2025-001", propertyTitle: "Nhà phố liền kề Thảo Điền", tenantName: "Phạm Thị Hoa", landlordName: "Trần Thị Lan", paymentType: "MonthlyRent", status: "Pending", paymentMethod: null, amount: 35000000, currency: "VND", dueDate: "2026-03-05", paidDate: null, billingMonth: 3, billingYear: 2026, lateFeeAmount: null, description: "Tiền thuê tháng 3/2026", transactionId: null, createdAt: "2026-02-25" },
  { id: 3, leaseId: 2, leaseNumber: "HD-2025-002", propertyTitle: "Căn hộ Vinhomes Central Park", tenantName: "Hoàng Văn Bình", landlordName: "Trần Thị Lan", paymentType: "Deposit", status: "Completed", paymentMethod: "Cash", amount: 36000000, currency: "VND", dueDate: "2026-02-28", paidDate: "2026-02-20", billingMonth: null, billingYear: null, lateFeeAmount: null, description: "Tiền đặt cọc hợp đồng HD-2025-002", transactionId: null, createdAt: "2026-02-18" },
  { id: 4, leaseId: 1, leaseNumber: "HD-2025-001", propertyTitle: "Nhà phố liền kề Thảo Điền", tenantName: "Phạm Thị Hoa", landlordName: "Trần Thị Lan", paymentType: "MonthlyRent", status: "Overdue", paymentMethod: null, amount: 35000000, currency: "VND", dueDate: "2026-01-05", paidDate: null, billingMonth: 1, billingYear: 2026, lateFeeAmount: 875000, description: "Tiền thuê tháng 1/2026", transactionId: null, createdAt: "2025-12-25" },
  { id: 5, leaseId: 3, leaseNumber: "HD-2024-001", propertyTitle: "Studio hiện đại quận 7", tenantName: "Ngô Thị Cúc", landlordName: "Trần Thị Lan", paymentType: "MonthlyRent", status: "Completed", paymentMethod: "BankTransfer", amount: 8500000, currency: "VND", dueDate: "2025-02-05", paidDate: "2025-02-04", billingMonth: 2, billingYear: 2025, lateFeeAmount: null, description: "Tiền thuê tháng 2/2025", transactionId: "TXN20250204001", createdAt: "2025-01-25" },
];

export const maintenanceRequests = [
  { id: 1, propertyId: 2, propertyTitle: "Nhà phố liền kề Thảo Điền", leaseId: 1, requestedBy: 4, requesterName: "Phạm Thị Hoa", status: "InProgress", priority: "High", category: "Plumbing", title: "Rò rỉ đường ống nước nhà bếp", description: "Đường ống phía dưới bồn rửa chén bị rò nước, cần sửa gấp.", imageUrls: null, assignedTo: null, assignedToName: null, assignedAt: null, estimatedCost: 500000, actualCost: null, resolution: null, resolvedAt: null, scheduledDate: "2026-02-28", rating: null, feedback: null, createdAt: "2026-02-24" },
  { id: 2, propertyId: 1, propertyTitle: "Căn hộ Vinhomes Central Park", leaseId: 2, requestedBy: 5, requesterName: "Hoàng Văn Bình", status: "Open", priority: "Medium", category: "Electrical", title: "Ổ cắm điện phòng khách bị chập", description: "Ổ cắm điện góc phòng khách phát ra tiếng kêu lạ khi cắm thiết bị.", imageUrls: null, assignedTo: null, assignedToName: null, assignedAt: null, estimatedCost: 200000, actualCost: null, resolution: null, resolvedAt: null, scheduledDate: null, rating: null, feedback: null, createdAt: "2026-02-23" },
  { id: 3, propertyId: 2, propertyTitle: "Nhà phố liền kề Thảo Điền", leaseId: 1, requestedBy: 4, requesterName: "Phạm Thị Hoa", status: "Resolved", priority: "Low", category: "Painting", title: "Tường phòng ngủ bị bong tróc sơn", description: "Một mảng tường khoảng 1m² bị bong sơn do ẩm mốc.", imageUrls: null, assignedTo: null, assignedToName: "Thợ Minh", assignedAt: "2026-01-10", estimatedCost: 300000, actualCost: 350000, resolution: "Đã cạo sơn cũ, xử lý ẩm và sơn lại toàn bộ khu vực.", resolvedAt: "2026-01-15", scheduledDate: "2026-01-12", rating: 5, feedback: "Thợ làm rất cẩn thận, sạch sẽ.", createdAt: "2026-01-08" },
  { id: 4, propertyId: 4, propertyTitle: "Studio hiện đại quận 7", leaseId: 3, requestedBy: 6, requesterName: "Ngô Thị Cúc", status: "Cancelled", priority: "Low", category: "Other", title: "Thay bóng đèn hành lang", description: "Bóng đèn hành lang tầng trệt bị hỏng.", imageUrls: null, assignedTo: null, assignedToName: null, assignedAt: null, estimatedCost: 50000, actualCost: null, resolution: null, resolvedAt: null, scheduledDate: null, rating: null, feedback: null, createdAt: "2025-12-20" },
];

export const bookings = [
  { id: 1, propertyId: 1, propertyTitle: "Căn hộ cao cấp Vinhomes Central Park", propertyThumbnail: "https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?w=400", tenantId: 5, tenantName: "Hoàng Văn Bình", landlordName: "Trần Thị Lan", status: "Confirmed", scheduledDate: "2026-02-28", startTime: "10:00:00", endTime: "11:00:00", message: "Tôi muốn xem căn hộ để tìm hiểu trước khi ký hợp đồng.", confirmationNotes: "Vui lòng đến đúng giờ, liên hệ qua SĐT trước khi đến.", cancellationReason: null, createdAt: "2026-02-22" },
  { id: 2, propertyId: 3, propertyTitle: "Phòng trọ giá rẻ Gò Vấp", propertyThumbnail: "https://images.unsplash.com/photo-1493809842364-78817add7ffb?w=400", tenantId: 4, tenantName: "Phạm Thị Hoa", landlordName: "Lê Minh Tuấn", status: "Pending", scheduledDate: "2026-03-01", startTime: "14:00:00", endTime: "15:00:00", message: "Muốn xem phòng cho người thân.", confirmationNotes: null, cancellationReason: null, createdAt: "2026-02-25" },
  { id: 3, propertyId: 4, propertyTitle: "Studio hiện đại quận 7", propertyThumbnail: "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=400", tenantId: 5, tenantName: "Hoàng Văn Bình", landlordName: "Trần Thị Lan", status: "Cancelled", scheduledDate: "2026-02-18", startTime: "09:00:00", endTime: "10:00:00", message: null, confirmationNotes: null, cancellationReason: "Bận công việc đột xuất, xin lỗi.", createdAt: "2026-02-15" },
];

export const rentalApplications = [
  { id: 1, propertyId: 1, propertyTitle: "Căn hộ cao cấp Vinhomes Central Park", propertyThumbnail: "https://images.unsplash.com/photo-1545324418-cc1a3fa10c00?w=400", tenantId: 5, tenantName: "Hoàng Văn Bình", tenantEmail: "tenant2@gmail.com", tenantPhone: "0945678901", status: "Approved", moveInDate: "2026-03-01", leaseDurationMonths: 12, numberOfOccupants: 1, message: "Tôi là kỹ sư IT, thu nhập ổn định, đảm bảo thanh toán đúng hạn.", occupation: "Kỹ sư phần mềm", monthlyIncome: 30000000, employerName: "FPT Software", rejectionReason: null, reviewedAt: "2026-02-19", createdAt: "2026-02-18" },
  { id: 2, propertyId: 3, propertyTitle: "Phòng trọ giá rẻ Gò Vấp", propertyThumbnail: "https://images.unsplash.com/photo-1493809842364-78817add7ffb?w=400", tenantId: 4, tenantName: "Phạm Thị Hoa", tenantEmail: "tenant1@gmail.com", tenantPhone: "0934567890", status: "Pending", moveInDate: "2026-03-15", leaseDurationMonths: 6, numberOfOccupants: 2, message: "Sinh viên đại học, cần phòng giá rẻ gần trường.", occupation: "Sinh viên", monthlyIncome: 5000000, employerName: null, rejectionReason: null, reviewedAt: null, createdAt: "2026-02-24" },
  { id: 3, propertyId: 4, propertyTitle: "Studio hiện đại quận 7", propertyThumbnail: "https://images.unsplash.com/photo-1502672260266-1c1ef2d93688?w=400", tenantId: 6, tenantName: "Ngô Thị Cúc", tenantEmail: "tenant3@gmail.com", tenantPhone: "0956789012", status: "Rejected", moveInDate: "2025-01-01", leaseDurationMonths: 12, numberOfOccupants: 1, message: null, occupation: "Nhân viên văn phòng", monthlyIncome: 10000000, employerName: "Company A", rejectionReason: "Thu nhập không đủ đáp ứng yêu cầu tối thiểu 3x tiền thuê.", reviewedAt: "2025-02-05", createdAt: "2025-01-30" },
];

export const conversations = [
  { id: 1, otherUserId: 2, otherUserName: "Trần Thị Lan", otherUserAvatar: null, title: null, propertyId: 1, propertyTitle: "Căn hộ Vinhomes Central Park", lastMessageAt: "2026-02-25T10:30:00Z", lastMessageContent: "Tôi sẽ xem nhà vào thứ 6 nhé", unreadCount: 2, isActive: true },
  { id: 2, otherUserId: 4, otherUserName: "Phạm Thị Hoa", otherUserAvatar: null, title: null, propertyId: 2, propertyTitle: "Nhà phố Thảo Điền", lastMessageAt: "2026-02-24T15:00:00Z", lastMessageContent: "Ok, tôi sẽ liên hệ thợ sửa nước", unreadCount: 0, isActive: true },
  { id: 3, otherUserId: 3, otherUserName: "Lê Minh Tuấn", otherUserAvatar: null, title: null, propertyId: null, propertyTitle: null, lastMessageAt: "2026-02-20T08:00:00Z", lastMessageContent: "Bạn có thể duyệt BDS của tôi không?", unreadCount: 1, isActive: true },
];

export const messages = [
  { id: 1, conversationId: 1, senderId: 4, senderName: "Phạm Thị Hoa", senderAvatar: null, messageType: "Text", content: "Chào chị, căn hộ còn trống không ạ?", isRead: true, isEdited: false, isDeleted: false, sentAt: "2026-02-24T09:00:00Z" },
  { id: 2, conversationId: 1, senderId: 2, senderName: "Trần Thị Lan", senderAvatar: null, messageType: "Text", content: "Chào em, còn trống đây. Em muốn đặt lịch xem không?", isRead: true, isEdited: false, isDeleted: false, sentAt: "2026-02-24T09:05:00Z" },
  { id: 3, conversationId: 1, senderId: 4, senderName: "Phạm Thị Hoa", senderAvatar: null, messageType: "Text", content: "Dạ em muốn xem vào thứ 6 tuần này được không ạ?", isRead: true, isEdited: false, isDeleted: false, sentAt: "2026-02-24T09:10:00Z" },
  { id: 4, conversationId: 1, senderId: 2, senderName: "Trần Thị Lan", senderAvatar: null, messageType: "Text", content: "Được nha em. Thứ 6 ngày 28 lúc 10h sáng nhé.", isRead: true, isEdited: false, isDeleted: false, sentAt: "2026-02-25T10:28:00Z" },
  { id: 5, conversationId: 1, senderId: 4, senderName: "Phạm Thị Hoa", senderAvatar: null, messageType: "Text", content: "Tôi sẽ xem nhà vào thứ 6 nhé", isRead: false, isEdited: false, isDeleted: false, sentAt: "2026-02-25T10:30:00Z" },
];

export const adminDashboard = {
  totalUsers: 156,
  totalProperties: 48,
  activeLeases: 32,
  totalRevenue: 2450000000,
  pendingApprovals: 7,
  revenueTrend: [
    { year: 2025, month: 3, revenue: 145000000 },
    { year: 2025, month: 4, revenue: 162000000 },
    { year: 2025, month: 5, revenue: 178000000 },
    { year: 2025, month: 6, revenue: 195000000 },
    { year: 2025, month: 7, revenue: 210000000 },
    { year: 2025, month: 8, revenue: 225000000 },
    { year: 2025, month: 9, revenue: 198000000 },
    { year: 2025, month: 10, revenue: 214000000 },
    { year: 2025, month: 11, revenue: 231000000 },
    { year: 2025, month: 12, revenue: 245000000 },
    { year: 2026, month: 1, revenue: 258000000 },
    { year: 2026, month: 2, revenue: 192000000 },
  ],
};

export const landlordDashboard = {
  totalProperties: 4,
  occupancyRate: 50,
  monthlyRevenue: 35000000,
  pendingPayments: 35875000,
  activeMaintenanceRequests: 2,
  revenueTrend: [
    { year: 2025, month: 3, revenue: 35000000 },
    { year: 2025, month: 4, revenue: 35000000 },
    { year: 2025, month: 5, revenue: 43500000 },
    { year: 2025, month: 6, revenue: 43500000 },
    { year: 2025, month: 7, revenue: 43500000 },
    { year: 2025, month: 8, revenue: 43500000 },
    { year: 2025, month: 9, revenue: 35000000 },
    { year: 2025, month: 10, revenue: 35000000 },
    { year: 2025, month: 11, revenue: 35000000 },
    { year: 2025, month: 12, revenue: 35000000 },
    { year: 2026, month: 1, revenue: 35000000 },
    { year: 2026, month: 2, revenue: 35000000 },
  ],
};

export const tenantDashboard = {
  activeLease: leases[0],
  nextPayment: payments[1],
  openMaintenanceRequests: 1,
  upcomingBookings: [bookings[0]],
};

export const systemConfigs = [
  { id: 1, key: "MaxImagesPerProperty", value: "10", type: "System", description: "Số ảnh tối đa cho mỗi BDS", unit: null, isActive: true },
  { id: 2, key: "LateFeePercentage", value: "2.5", type: "Payment", description: "Phần trăm phí trả chậm mặc định", unit: "%", isActive: true },
  { id: 3, key: "MaxBookingsPerDay", value: "3", type: "Booking", description: "Số lượt đặt lịch tối đa mỗi ngày", unit: null, isActive: true },
  { id: 4, key: "LeaseMinDuration", value: "3", type: "Lease", description: "Thời hạn hợp đồng tối thiểu", unit: "tháng", isActive: true },
  { id: 5, key: "DepositMaxMonths", value: "3", type: "Payment", description: "Tiền cọc tối đa theo tháng", unit: "tháng", isActive: true },
  { id: 6, key: "AutoApproveListings", value: "false", type: "System", description: "Tự động duyệt đăng tin BDS", unit: null, isActive: true },
];

export const auditLogs = [
  { id: 1, userId: 1, userName: "Nguyễn Văn Admin", action: "APPROVE_PROPERTY", details: "Duyệt BDS #1 - Căn hộ Vinhomes Central Park", createdAt: "2026-02-25T08:30:00Z" },
  { id: 2, userId: 2, userName: "Trần Thị Lan", action: "CREATE_PROPERTY", details: "Tạo BDS mới #5 - Biệt thự vườn Bình Dương", createdAt: "2026-02-10T14:00:00Z" },
  { id: 3, userId: 4, userName: "Phạm Thị Hoa", action: "SUBMIT_APPLICATION", details: "Nộp đơn xin thuê BDS #3 - Phòng trọ Gò Vấp", createdAt: "2026-02-24T10:00:00Z" },
  { id: 4, userId: 1, userName: "Nguyễn Văn Admin", action: "DEACTIVATE_USER", details: "Vô hiệu hoá tài khoản user #6 - Ngô Thị Cúc", createdAt: "2026-01-15T09:00:00Z" },
  { id: 5, userId: 5, userName: "Hoàng Văn Bình", action: "MAKE_PAYMENT", details: "Thanh toán tiền cọc HĐ HD-2025-002", createdAt: "2026-02-20T16:00:00Z" },
];

export const revenueReports = [
  { propertyId: 1, propertyTitle: "Căn hộ cao cấp Vinhomes Central Park", totalRentCollected: 18000000, totalLateFees: 0, totalMaintenanceCost: 0, grossRevenue: 18000000, netRevenue: 18000000 },
  { propertyId: 2, propertyTitle: "Nhà phố liền kề Thảo Điền", totalRentCollected: 35000000, totalLateFees: 875000, totalMaintenanceCost: 350000, grossRevenue: 35875000, netRevenue: 35525000 },
  { propertyId: 4, propertyTitle: "Studio hiện đại quận 7", totalRentCollected: 8500000, totalLateFees: 0, totalMaintenanceCost: 0, grossRevenue: 8500000, netRevenue: 8500000 },
];
