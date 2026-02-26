using PropertyManagementSystemVer2.BLL.DTOs;
using PropertyManagementSystemVer2.BLL.Services.Interfaces;
using PropertyManagementSystemVer2.DAL.Entities;
using PropertyManagementSystemVer2.DAL.Enums;
using PropertyManagementSystemVer2.DAL.Repositories.Interfaces;

namespace PropertyManagementSystemVer2.BLL.Services.Implementations
{
    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PropertyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // BR6: Tao Property moi
        // 1. Tao listing: title, description, address, price, property type, so phong, dien tich, tien ich
        // 2. Toi thieu 3 anh, toi da 20 anh (max 5MB/anh, JPG/PNG/WebP) - validate o tang Web
        // 3. Anh dau tien la thumbnail
        // 4. Property mac dinh trang thai Draft
        // 5. Landlord submit de Admin xet duyet -> trang thai chuyen sang Pending Approval
        public async Task<ServiceResultDto<PropertyDetailDto>> CreatePropertyAsync(int landlordId, CreatePropertyDto dto)
        {
            var landlord = await _unitOfWork.Users.GetByIdAsync(landlordId);
            if (landlord == null || !landlord.IsLandlord)
                return ServiceResultDto<PropertyDetailDto>.Failure("Chi Landlord moi co the tao property.");

            var property = new Property
            {
                Title = dto.Title,
                Description = dto.Description,
                PropertyType = dto.PropertyType,
                Status = PropertyStatus.Pending, // Yêu cầu duyệt ngay
                Address = dto.Address,
                City = dto.City,
                District = dto.District,
                Ward = dto.Ward,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Area = dto.Area,
                Bedrooms = dto.Bedrooms,
                Bathrooms = dto.Bathrooms,
                Floors = dto.Floors,
                MonthlyRent = dto.MonthlyRent,
                DepositAmount = dto.DepositAmount,
                Amenities = dto.Amenities,
                AllowPets = dto.AllowPets,
                AllowSmoking = dto.AllowSmoking,
                MaxOccupants = dto.MaxOccupants,
                LandlordId = landlordId,
                IsAvailable = true,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Properties.AddAsync(property);
            await _unitOfWork.SaveChangesAsync();

            var result = await GetByIdAsync(property.Id);
            return result;
        }

        // BR7: Chinh sua Property
        // 1. Chi chinh sua khi property o trang thai Draft hoac Available
        // 2. Neu dang co Lease active -> chi sua description, anh, tien ich (khong sua gia, dia chi)
        // 3. Moi thay doi ghi audit log
        public async Task<ServiceResultDto<PropertyDetailDto>> UpdatePropertyAsync(int userId, int propertyId, UpdatePropertyDto dto)
        {
            var property = await _unitOfWork.Properties.GetByIdWithDetailsAsync(propertyId);
            if (property == null)
                return ServiceResultDto<PropertyDetailDto>.Failure("Khong tim thay property.");

            // Kiem tra quyen so huu
            if (property.LandlordId != userId)
                return ServiceResultDto<PropertyDetailDto>.Failure("Ban khong co quyen chinh sua property nay.");

            // BR7.1: Chi chinh sua khi o Pending, Approved hoac Available/Unavailable/Rejected
            if (property.Status != PropertyStatus.Approved && property.Status != PropertyStatus.Pending && property.Status != PropertyStatus.Available && property.Status != PropertyStatus.Unavailable && property.Status != PropertyStatus.Rejected)
                return ServiceResultDto<PropertyDetailDto>.Failure("Chi co the chinh sua property o trang thai Pending, Approved, Available, Unavailable hoac Rejected.");

            // BR7.2: Neu co Lease active, chi cho sua mot so truong
            var hasActiveLease = await _unitOfWork.Properties.HasActiveLeaseAsync(propertyId);
            bool statusChangedToPending = false;
            if (hasActiveLease)
            {
                // Chi cho sua description va amenities
                if (dto.Description != null) property.Description = dto.Description;
                if (dto.Amenities != null) property.Amenities = dto.Amenities;
            }
            else
            {
                if (dto.Title != null) property.Title = dto.Title;
                if (dto.Description != null) property.Description = dto.Description;
                if (dto.PropertyType.HasValue) property.PropertyType = dto.PropertyType.Value;
                if (dto.Address != null) property.Address = dto.Address;
                if (dto.City != null) property.City = dto.City;
                if (dto.District != null) property.District = dto.District;
                if (dto.Ward != null) property.Ward = dto.Ward;
                if (dto.Latitude.HasValue) property.Latitude = dto.Latitude.Value;
                if (dto.Longitude.HasValue) property.Longitude = dto.Longitude.Value;
                if (dto.Area.HasValue) property.Area = dto.Area.Value;
                if (dto.Bedrooms.HasValue) property.Bedrooms = dto.Bedrooms.Value;
                if (dto.Bathrooms.HasValue) property.Bathrooms = dto.Bathrooms.Value;
                if (dto.Floors.HasValue) property.Floors = dto.Floors.Value;
                if (dto.MonthlyRent.HasValue) property.MonthlyRent = dto.MonthlyRent.Value;
                if (dto.DepositAmount.HasValue) property.DepositAmount = dto.DepositAmount.Value;
                if (dto.Amenities != null) property.Amenities = dto.Amenities;
                property.AllowPets = dto.AllowPets;
                property.AllowSmoking = dto.AllowSmoking;
                if (dto.MaxOccupants.HasValue) property.MaxOccupants = dto.MaxOccupants.Value;

                // Tự động chuyển về Pending nếu không phải đang bị từ chối
                if (property.Status != PropertyStatus.Rejected && property.Status != PropertyStatus.Pending)
                {
                    property.Status = PropertyStatus.Pending;
                    statusChangedToPending = true;
                }
            }

            property.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Properties.Update(property);
            await _unitOfWork.SaveChangesAsync();

            // Return updated data with appropriate message
            var updatedProperty = await GetByIdAsync(propertyId);
            var successMessage = statusChangedToPending 
                ? "Cập nhật thành công. Bất động sản đã chuyển về trạng thái Chờ duyệt để Admin phê duyệt lại."
                : "Cập nhật thông tin thành công.";
                
            var res = ServiceResultDto<PropertyDetailDto>.Success(updatedProperty.Data);
            res.Message = successMessage;
            return res;
        }

        // BR6.5: Landlord submit de Admin xet duyet -> trang thai Pending Approval
        public async Task<ServiceResultDto> SubmitForApprovalAsync(int landlordId, int propertyId)
        {
            var property = await _unitOfWork.Properties.GetByIdWithDetailsAsync(propertyId);
            if (property == null)
                return ServiceResultDto.Failure("Khong tim thay property.");

            if (property.LandlordId != landlordId)
                return ServiceResultDto.Failure("Ban khong co quyen thuc hien thao tac nay.");

            if (property.Status == PropertyStatus.Pending)
                return ServiceResultDto.Success("Property dang o trang thai cho duyet.");

            if (property.Status != PropertyStatus.Rejected && property.Status != PropertyStatus.Pending)
            {
                // This means the property is not Rejected. We should only allow submitting from Rejected.
                // Wait, initially properties are created at Pending status directly. SubmitForApprovalAsync is only needed for RESUBMISSION from Rejected status.
                if (property.Status != PropertyStatus.Rejected)
                     return ServiceResultDto.Failure("Chi co the gui xet duyet lai khi bi tu choi.");
            }

            // Yêu cầu (1): Bỏ điều kiện "Property phải có tối thiểu 3 ảnh trước khi submit." khi gửi lại duyệt
            // var imageCount = await _unitOfWork.PropertyImages.CountByPropertyIdAsync(propertyId);
            // if (imageCount < 3)
            //     return ServiceResultDto.Failure("Property phai co toi thieu 3 anh truoc khi submit.");

            property.Status = PropertyStatus.Pending;
            property.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Properties.Update(property);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success("Da gui yeu cau duyet property.");
        }

        // BR8: Admin duyet/tu choi Property
        // 1. Admin review property dang o trang thai Pending Approval
        // 2. Approve -> Property chuyen sang Available (hien thi cho thue)
        // 3. Reject -> Property chuyen ve Draft kem ly do tu choi
        // 4. Landlord nhan notification ket qua duyet
        // 5. Landlord co the sua va submit lai neu bi reject
        // 6. Admin co the xem danh sach property Pending Approval
        public async Task<ServiceResultDto> ApproveRejectPropertyAsync(int adminId, ApproveRejectPropertyDto dto)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(dto.PropertyId);
            if (property == null)
                return ServiceResultDto.Failure("Khong tim thay property.");

            if (property.Status != PropertyStatus.Pending)
                return ServiceResultDto.Failure("Property khong o trang thai cho duyet.");

            if (dto.IsApproved)
            {
                // BR8.2: Approve -> Available
                property.Status = PropertyStatus.Approved;
                property.ApprovedAt = DateTime.UtcNow;
                property.ApprovedBy = adminId;
                property.RejectionReason = null;
            }
            else
            {
                // BR8.3: Reject -> Rejected kem ly do
                if (string.IsNullOrWhiteSpace(dto.RejectionReason))
                    return ServiceResultDto.Failure("Phai neu ly do tu choi.");

                property.Status = PropertyStatus.Rejected;
                property.RejectionReason = dto.RejectionReason;
            }

            property.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Properties.Update(property);
            await _unitOfWork.SaveChangesAsync();

            // BR8.4: TODO - Gui notification cho Landlord
            return ServiceResultDto.Success(dto.IsApproved ? "Da duyet property." : "Da tu choi property.");
        }

        public async Task<ServiceResultDto> BlockPropertyAsync(int adminId, int propertyId, string reason)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            if (property == null)
                return ServiceResultDto.Failure("Khong tim thay property.");

            if (property.Status == PropertyStatus.Blocked)
                return ServiceResultDto.Failure("Bất động sản này đã bị khóa từ trước.");

            if (property.Status != PropertyStatus.Available)
                return ServiceResultDto.Failure("Chỉ được khóa bài đăng khi trạng thái đang là 'Đang đăng'.");

            if (string.IsNullOrWhiteSpace(reason))
                return ServiceResultDto.Failure("Phải nêu lý do khóa bất động sản.");

            property.Status = PropertyStatus.Blocked;
            property.IsAvailable = false;
            property.RejectionReason = reason; // Reuse this field to store the block reason
            property.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Properties.Update(property);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success("Đã khóa bài đăng bất động sản thành công.");
        }

        // BR8.6: Xem danh sach property Pending Approval
        public async Task<ServiceResultDto<List<PropertyListDto>>> GetPendingApprovalAsync()
        {
            var properties = await _unitOfWork.Properties.GetByStatusAsync(PropertyStatus.Pending);
            var result = properties.Select(MapToPropertyListDto).ToList();
            return ServiceResultDto<List<PropertyListDto>>.Success(result);
        }

        // BR9: Unpublish Property
        // 1. Chuyen property Available -> Inactive (Unavailable)
        // 2. Khi unpublish: huy tat ca Booking pending, notify applicants
        // 3. Khong unpublish khi co Lease active
        // 4. Muon publish lai phai submit duyet lai tu dau (Pending -> Approve)
        public async Task<ServiceResultDto> UnpublishPropertyAsync(int userId, int propertyId)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            if (property == null)
                return ServiceResultDto.Failure("Khong tim thay property.");

            if (property.LandlordId != userId)
                return ServiceResultDto.Failure("Ban khong co quyen thuc hien thao tac nay.");

            if (property.Status != PropertyStatus.Available)
                return ServiceResultDto.Failure("Chi co the unpublish property dang Available.");

            // BR9.3: Khong unpublish khi co Lease active
            if (await _unitOfWork.Properties.HasActiveLeaseAsync(propertyId))
                return ServiceResultDto.Failure("Khong the unpublish property dang co hop dong thue active.");

            // BR9.1: Chuyen sang Unavailable
            property.Status = PropertyStatus.Unavailable;
            property.IsAvailable = false;
            property.UpdatedAt = DateTime.UtcNow;

            // BR9.2: Huy tat ca booking pending
            var pendingBookings = await _unitOfWork.Bookings.GetByPropertyIdAsync(propertyId, BookingStatus.Pending);
            foreach (var booking in pendingBookings)
            {
                booking.Status = BookingStatus.Cancelled;
                booking.CancellationReason = "Property da bi unpublish.";
                booking.CancelledAt = DateTime.UtcNow;
                _unitOfWork.Bookings.Update(booking);
            }

            _unitOfWork.Properties.Update(property);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success("Da unpublish property.");
        }

        public async Task<ServiceResultDto> PublishPropertyAsync(int userId, int propertyId)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            if (property == null)
                return ServiceResultDto.Failure("Khong tim thay property.");

            if (property.LandlordId != userId)
                return ServiceResultDto.Failure("Ban khong co quyen thuc hien thao tac nay.");

            if (property.Status != PropertyStatus.Approved && property.Status != PropertyStatus.Unavailable)
                return ServiceResultDto.Failure("Chi co the publish property da duoc Approved hoac dang Unavailable.");

            property.Status = PropertyStatus.Available;
            property.IsAvailable = true;
            property.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Properties.Update(property);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success("Da publish property len danh sach cho thue.");
        }

        // BR10: Soft Delete Property
        // 1. Chi xoa khi khong co Lease active hoac Payment pending
        // 2. Soft delete, giu data cho reporting
        // 3. Admin co the hard delete sau 90 ngay
        public async Task<ServiceResultDto> SoftDeletePropertyAsync(int userId, int propertyId)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            if (property == null)
                return ServiceResultDto.Failure("Khong tim thay property.");

            if (property.LandlordId != userId)
                return ServiceResultDto.Failure("Ban khong co quyen xoa property nay.");

            // BR10.1: Kiem tra Lease active va Payment pending
            if (await _unitOfWork.Properties.HasActiveLeaseAsync(propertyId))
                return ServiceResultDto.Failure("Khong the xoa property dang co hop dong thue active.");

            if (await _unitOfWork.Properties.HasPendingPaymentAsync(propertyId))
                return ServiceResultDto.Failure("Khong the xoa property dang co payment pending.");

            // BR10.2: Soft delete
            property.IsAvailable = false;
            property.Status = PropertyStatus.Unavailable;
            property.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Properties.Update(property);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success("Da xoa property.");
        }

        // BR11: Tim kiem va loc Property
        // 1. Search theo keyword (title, address, description)
        // 2. Filter: price range, property type, so phong, dien tich, tien ich, khu vuc
        // 3. Sort: gia, ngay dang, rating
        // 5. Pagination mac dinh 20 items/page
        public async Task<ServiceResultDto<PagedResultDto<PropertyListDto>>> SearchPropertiesAsync(PropertySearchDto searchDto)
        {
            var properties = await _unitOfWork.Properties.SearchAsync(
                searchDto.Keyword, searchDto.PropertyType, searchDto.Status, searchDto.MinPrice, searchDto.MaxPrice,
                searchDto.City, searchDto.District, searchDto.MinBedrooms, searchDto.MinArea,
                searchDto.PageNumber, searchDto.PageSize, searchDto.SortBy);

            var totalCount = await _unitOfWork.Properties.CountSearchAsync(
                searchDto.Keyword, searchDto.PropertyType, searchDto.Status, searchDto.MinPrice, searchDto.MaxPrice,
                searchDto.City, searchDto.District, searchDto.MinBedrooms, searchDto.MinArea);

            var result = new PagedResultDto<PropertyListDto>
            {
                Items = properties.Select(MapToPropertyListDto).ToList(),
                TotalCount = totalCount,
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize
            };

            return ServiceResultDto<PagedResultDto<PropertyListDto>>.Success(result);
        }

        // BR12: Xem chi tiet Property
        // 1. Hien thi day du thong tin, gallery anh, map, thong tin Landlord (limited), reviews
        // 2. Tang view count (simplified - khong co ViewCount column hien tai)
        // 3. Tenant da login thay nut Apply / Book Viewing (xu ly o tang Web)
        public async Task<ServiceResultDto<PropertyDetailDto>> GetByIdAsync(int propertyId)
        {
            var property = await _unitOfWork.Properties.GetByIdWithDetailsAsync(propertyId);
            if (property == null)
                return ServiceResultDto<PropertyDetailDto>.Failure("Khong tim thay property.");

            return ServiceResultDto<PropertyDetailDto>.Success(MapToPropertyDetailDto(property));
        }

        // BR14: Danh sach Property cua Landlord
        // 1. Xem tat ca property minh so huu
        // 2. Filter theo status (Draft/Available/Rented/Inactive)
        // 3. Dashboard summary: tong property, dang trong, dang cho thue, tong revenue
        public async Task<ServiceResultDto<List<PropertyListDto>>> GetByLandlordIdAsync(int landlordId, PropertyStatus? status = null)
        {
            var properties = await _unitOfWork.Properties.GetByLandlordIdAsync(landlordId);

            if (status.HasValue)
                properties = properties.Where(p => p.Status == status.Value);

            var result = properties.Select(MapToPropertyListDto).ToList();
            return ServiceResultDto<List<PropertyListDto>>.Success(result);
        }

        // BR14.3: Property Summary cho Landlord Dashboard
        public async Task<ServiceResultDto<PropertySummaryDto>> GetPropertySummaryAsync(int landlordId)
        {
            var properties = await _unitOfWork.Properties.GetByLandlordIdAsync(landlordId);
            var propertyList = properties.ToList();

            var summary = new PropertySummaryDto
            {
                TotalProperties = propertyList.Count,
                AvailableCount = propertyList.Count(p => p.Status == PropertyStatus.Approved),
                RentedCount = propertyList.Count(p => p.Status == PropertyStatus.Rented),
                PendingCount = propertyList.Count(p => p.Status == PropertyStatus.Pending),
                InactiveCount = propertyList.Count(p => p.Status == PropertyStatus.Unavailable)
            };

            return ServiceResultDto<PropertySummaryDto>.Success(summary);
        }

        // BR13: Quan ly anh Property
        // 1. Upload anh (max 5MB/anh, JPG/PNG/WebP) - validate o tang Web
        // 3. Drag-drop sap xep thu tu
        // 4. Xoa anh khong anh huong anh khac
        public async Task<ServiceResultDto<PropertyImageDto>> AddImageAsync(int landlordId, int propertyId, string imageUrl, string? caption, bool isPrimary)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            if (property == null || property.LandlordId != landlordId)
                return ServiceResultDto<PropertyImageDto>.Failure("Khong co quyen them anh cho property nay.");

            var imageCount = await _unitOfWork.PropertyImages.CountByPropertyIdAsync(propertyId);
            if (imageCount >= 20)
                return ServiceResultDto<PropertyImageDto>.Failure("Da dat toi da 20 anh cho property.");

            // BR6.3: Anh dau tien la thumbnail
            if (isPrimary || imageCount == 0)
            {
                var existingPrimary = await _unitOfWork.PropertyImages.GetPrimaryImageAsync(propertyId);
                if (existingPrimary != null)
                {
                    existingPrimary.IsPrimary = false;
                    _unitOfWork.PropertyImages.Update(existingPrimary);
                }
                isPrimary = true;
            }

            var image = new PropertyImage
            {
                PropertyId = propertyId,
                ImageUrl = imageUrl,
                Caption = caption,
                IsPrimary = isPrimary,
                DisplayOrder = imageCount + 1,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.PropertyImages.AddAsync(image);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto<PropertyImageDto>.Success(new PropertyImageDto
            {
                Id = image.Id,
                ImageUrl = image.ImageUrl,
                Caption = image.Caption,
                IsPrimary = image.IsPrimary,
                DisplayOrder = image.DisplayOrder
            });
        }

        // BR13.4: Xoa anh
        public async Task<ServiceResultDto> RemoveImageAsync(int landlordId, int propertyId, int imageId)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            if (property == null || property.LandlordId != landlordId)
                return ServiceResultDto.Failure("Khong co quyen xoa anh.");

            var image = await _unitOfWork.PropertyImages.GetByIdAsync(imageId);
            if (image == null || image.PropertyId != propertyId)
                return ServiceResultDto.Failure("Khong tim thay anh.");

            _unitOfWork.PropertyImages.Delete(image);
            await _unitOfWork.SaveChangesAsync();

            return ServiceResultDto.Success("Da xoa anh.");
        }

        public async Task<ServiceResultDto> SetPrimaryImageAsync(int landlordId, int propertyId, int imageId)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            if (property == null || property.LandlordId != landlordId)
                return ServiceResultDto.Failure("Không có quyền thiết lập ảnh cho bất động sản này.");

            var images = await _unitOfWork.PropertyImages.GetByPropertyIdAsync(propertyId);
            var targetImage = images.FirstOrDefault(i => i.Id == imageId);

            if (targetImage == null)
                return ServiceResultDto.Failure("Không tìm thấy ảnh.");

            // Đặt tất cả ảnh của property này thành phụ
            foreach (var img in images)
            {
                img.IsPrimary = false;
                _unitOfWork.PropertyImages.Update(img);
            }

            // Đặt ảnh được chọn thành primary
            targetImage.IsPrimary = true;
            _unitOfWork.PropertyImages.Update(targetImage);

            await _unitOfWork.SaveChangesAsync();
            return ServiceResultDto.Success("Đã thiết lập ảnh đại diện thành công.");
        }

        // BR13.3: Sap xep thu tu anh
        public async Task<ServiceResultDto> ReorderImagesAsync(int landlordId, int propertyId, List<int> imageIds)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            if (property == null || property.LandlordId != landlordId)
                return ServiceResultDto.Failure("Khong co quyen sap xep anh.");

            var images = await _unitOfWork.PropertyImages.GetByPropertyIdAsync(propertyId);
            var imageList = images.ToList();

            for (int i = 0; i < imageIds.Count; i++)
            {
                var img = imageList.FirstOrDefault(x => x.Id == imageIds[i]);
                if (img != null)
                {
                    img.DisplayOrder = i + 1;
                    _unitOfWork.PropertyImages.Update(img);
                }
            }

            await _unitOfWork.SaveChangesAsync();
            return ServiceResultDto.Success("Da cap nhat thu tu anh.");
        }

        // Mapping helpers
        private static PropertyListDto MapToPropertyListDto(Property p)
        {
            return new PropertyListDto
            {
                Id = p.Id,
                Title = p.Title,
                PropertyType = p.PropertyType,
                Status = p.Status,
                Address = p.Address,
                Description = p.Description,
                City = p.City,
                District = p.District,
                Ward = p.Ward,
                Area = p.Area,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                Floors = p.Floors,
                MonthlyRent = p.MonthlyRent,
                DepositAmount = p.DepositAmount,
                Amenities = p.Amenities,
                AllowPets = p.AllowPets,
                AllowSmoking = p.AllowSmoking,
                Currency = p.Currency,
                ThumbnailUrl = p.Images?.FirstOrDefault(i => i.IsPrimary)?.ImageUrl ?? p.Images?.FirstOrDefault()?.ImageUrl,
                CreatedAt = p.CreatedAt,
                RejectionReason = p.RejectionReason,
                Landlord = p.Landlord != null ? new LandlordSummaryDto
                {
                    Id = p.Landlord.Id,
                    FullName = p.Landlord.FullName,
                    AvatarUrl = p.Landlord.AvatarUrl,
                    PhoneNumber = p.Landlord.PhoneNumber,
                    IsIdentityVerified = p.Landlord.IsIdentityVerified
                } : null
            };
        }

        private static PropertyDetailDto MapToPropertyDetailDto(Property p)
        {
            return new PropertyDetailDto
            {
                Id = p.Id,
                Title = p.Title,
                Description = p.Description,
                PropertyType = p.PropertyType,
                Status = p.Status,
                Address = p.Address,
                City = p.City,
                District = p.District,
                Ward = p.Ward,
                Latitude = p.Latitude,
                Longitude = p.Longitude,
                Area = p.Area,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                Floors = p.Floors,
                MonthlyRent = p.MonthlyRent,
                DepositAmount = p.DepositAmount,
                Currency = p.Currency,
                Amenities = p.Amenities,
                AllowPets = p.AllowPets,
                AllowSmoking = p.AllowSmoking,
                MaxOccupants = p.MaxOccupants,
                LandlordId = p.LandlordId,
                Landlord = p.Landlord != null ? new LandlordSummaryDto
                {
                    Id = p.Landlord.Id,
                    FullName = p.Landlord.FullName,
                    AvatarUrl = p.Landlord.AvatarUrl,
                    PhoneNumber = p.Landlord.PhoneNumber,
                    IsIdentityVerified = p.Landlord.IsIdentityVerified
                } : null,
                Images = p.Images?.Select(i => new PropertyImageDto
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    Caption = i.Caption,
                    IsPrimary = i.IsPrimary,
                    DisplayOrder = i.DisplayOrder
                }).ToList() ?? new List<PropertyImageDto>(),
                RejectionReason = p.RejectionReason,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt
            };
        }
    }
}
