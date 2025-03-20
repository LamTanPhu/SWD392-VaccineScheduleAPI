namespace Services.Services.Vaccines
{
    using IRepositories.IRepository;
    using IRepositories.Entity.Vaccines;
    using IServices.Interfaces.Vaccines;
    using ModelViews.Requests.VaccinePackage;
    using ModelViews.Responses.Vaccine;
    using ModelViews.Responses.VaccinePackage;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class VaccinePackageService : IVaccinePackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<VaccinePackage> _vaccinePackageRepository;
        private readonly IGenericRepository<VaccinePackageDetail> _vaccinePackageDetailRepository;
        private readonly IGenericRepository<Vaccine> _vaccineRepository;

        public VaccinePackageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _vaccinePackageRepository = _unitOfWork.GetRepository<VaccinePackage>();
            _vaccinePackageDetailRepository = _unitOfWork.GetRepository<VaccinePackageDetail>();
            _vaccineRepository = _unitOfWork.GetRepository<Vaccine>();
        }

        public async Task<IEnumerable<VaccinePackageResponseDTO>> GetAllPackagesAsync()
        {
            var packages = await _vaccinePackageRepository.Entities
                .Where(p => p.PackageStatus)
                .Include(p => p.PackageDetails)
                .ThenInclude(pd => pd.Vaccine)
                .ToListAsync();

            return packages.Select(p => new VaccinePackageResponseDTO
            {
                Id = p.Id,
                PackageName = p.PackageName,
                PackageDescription = p.PackageDescription,
                PackageStatus = p.PackageStatus,
                Vaccines = p.PackageDetails.Select(pd => new VaccineResponseDTO
                {
                    Id = pd.Vaccine.Id,
                    Name = pd.Vaccine.Name,
                    IngredientsDescription = pd.Vaccine.IngredientsDescription,
                    UnitOfVolume = pd.Vaccine.UnitOfVolume,
                    MinAge = pd.Vaccine.MinAge,
                    MaxAge = pd.Vaccine.MaxAge,
                    BetweenPeriod = pd.Vaccine.BetweenPeriod,
                    QuantityAvailable = pd.Vaccine.QuantityAvailable,
                    Price = pd.Vaccine.Price,
                    ProductionDate = pd.Vaccine.ProductionDate,
                    ExpirationDate = pd.Vaccine.ExpirationDate,
                    Status = pd.Vaccine.Status,
                    VaccineCategoryId = pd.Vaccine.VaccineCategoryId,
                    BatchId = pd.Vaccine.BatchId,
                    Image = pd.Vaccine.Image
                }).ToList()
            });
        }

        public async Task<VaccinePackageResponseDTO?> GetPackageByIdAsync(string id)
        {
            var package = await _vaccinePackageRepository.Entities
                .Where(p => p.Id == id && p.PackageStatus)
                .Include(p => p.PackageDetails)
                .ThenInclude(pd => pd.Vaccine)
                .FirstOrDefaultAsync();

            if (package == null) return null;

            return new VaccinePackageResponseDTO
            {
                Id = package.Id,
                PackageName = package.PackageName,
                PackageDescription = package.PackageDescription,
                PackageStatus = package.PackageStatus,
                Vaccines = package.PackageDetails.Select(pd => new VaccineResponseDTO
                {
                    Id = pd.Vaccine.Id,
                    Name = pd.Vaccine.Name,
                    IngredientsDescription = pd.Vaccine.IngredientsDescription,
                    UnitOfVolume = pd.Vaccine.UnitOfVolume,
                    MinAge = pd.Vaccine.MinAge,
                    MaxAge = pd.Vaccine.MaxAge,
                    BetweenPeriod = pd.Vaccine.BetweenPeriod,
                    QuantityAvailable = pd.Vaccine.QuantityAvailable,
                    Price = pd.Vaccine.Price,
                    ProductionDate = pd.Vaccine.ProductionDate,
                    ExpirationDate = pd.Vaccine.ExpirationDate,
                    Status = pd.Vaccine.Status,
                    VaccineCategoryId = pd.Vaccine.VaccineCategoryId,
                    BatchId = pd.Vaccine.BatchId,
                    Image = pd.Vaccine.Image
                }).ToList()
            };
        }

        // Phương thức đã sửa: AddPackageAsync
        public async Task<VaccinePackageResponseDTO> AddPackageAsync(VaccinePackageRequestDTO packageDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var package = new VaccinePackage
                {
                    Id = Guid.NewGuid().ToString(),
                    PackageName = packageDto.PackageName,
                    PackageDescription = packageDto.PackageDescription,
                    PackageStatus = true,
                    PackagePrice = packageDto.PackagePrice // Giả định DTO có PackagePrice
                };

                await _vaccinePackageRepository.InsertAsync(package);

                foreach (var vaccineId in packageDto.VaccineIds)
                {
                    var vaccine = await _vaccineRepository.GetByIdAsync(vaccineId);
                    if (vaccine == null || vaccine.Status == "0")
                        throw new Exception($"Vaccine with ID {vaccineId} not found or inactive.");

                    var packageDetail = new VaccinePackageDetail
                    {
                        Id = Guid.NewGuid().ToString(),
                        VaccineId = vaccineId,
                        VaccinePackageId = package.Id
                        // Loại bỏ PackagePrice vì nó không còn trong VaccinePackageDetail
                    };
                    await _vaccinePackageDetailRepository.InsertAsync(packageDetail);
                }

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                var createdPackage = await _vaccinePackageRepository.Entities
                    .Where(p => p.Id == package.Id)
                    .Include(p => p.PackageDetails)
                    .ThenInclude(pd => pd.Vaccine)
                    .FirstAsync();

                return new VaccinePackageResponseDTO
                {
                    Id = createdPackage.Id,
                    PackageName = createdPackage.PackageName,
                    PackageDescription = createdPackage.PackageDescription,
                    PackageStatus = createdPackage.PackageStatus,
                    Vaccines = createdPackage.PackageDetails.Select(pd => new VaccineResponseDTO
                    {
                        Id = pd.Vaccine.Id,
                        Name = pd.Vaccine.Name,
                        IngredientsDescription = pd.Vaccine.IngredientsDescription,
                        UnitOfVolume = pd.Vaccine.UnitOfVolume,
                        MinAge = pd.Vaccine.MinAge,
                        MaxAge = pd.Vaccine.MaxAge,
                        BetweenPeriod = pd.Vaccine.BetweenPeriod,
                        QuantityAvailable = pd.Vaccine.QuantityAvailable,
                        Price = pd.Vaccine.Price,
                        ProductionDate = pd.Vaccine.ProductionDate,
                        ExpirationDate = pd.Vaccine.ExpirationDate,
                        Status = pd.Vaccine.Status,
                        VaccineCategoryId = pd.Vaccine.VaccineCategoryId,
                        BatchId = pd.Vaccine.BatchId,
                        Image = pd.Vaccine.Image
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to create package: {ex.Message}");
            }
        }

        // Phương thức đã sửa: UpdatePackageAsync
        public async Task UpdatePackageAsync(string id, VaccinePackageRequestDTO packageDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingPackage = await _vaccinePackageRepository.GetByIdAsync(id);
                if (existingPackage == null || !existingPackage.PackageStatus)
                    throw new Exception("Vaccine package not found or inactive.");

                existingPackage.PackageName = packageDto.PackageName;
                existingPackage.PackageDescription = packageDto.PackageDescription;
                existingPackage.PackagePrice = packageDto.PackagePrice; // Giả định DTO có PackagePrice

                var oldDetails = await _vaccinePackageDetailRepository.Entities
                    .Where(pd => pd.VaccinePackageId == id)
                    .ToListAsync();
                foreach (var detail in oldDetails)
                    await _vaccinePackageDetailRepository.DeleteAsync(detail.Id);

                foreach (var vaccineId in packageDto.VaccineIds)
                {
                    var vaccine = await _vaccineRepository.GetByIdAsync(vaccineId);
                    if (vaccine == null || vaccine.Status == "0")
                        throw new Exception($"Vaccine with ID {vaccineId} not found or inactive.");

                    var packageDetail = new VaccinePackageDetail
                    {
                        Id = Guid.NewGuid().ToString(),
                        VaccineId = vaccineId,
                        VaccinePackageId = id
                        // Loại bỏ PackagePrice
                    };
                    await _vaccinePackageDetailRepository.InsertAsync(packageDetail);
                }

                await _vaccinePackageRepository.UpdateAsync(existingPackage);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to update package: {ex.Message}");
            }
        }

        // Phương thức đã sửa: AddVaccineToPackageAsync
        public async Task AddVaccineToPackageAsync(string packageId, VaccinePackageUpdateRequestDTO request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var package = await _vaccinePackageRepository.GetByIdAsync(packageId);
                if (package == null || !package.PackageStatus)
                    throw new Exception("Vaccine package not found or inactive.");

                var vaccine = await _vaccineRepository.GetByIdAsync(request.VaccineId);
                if (vaccine == null || vaccine.Status == "0")
                    throw new Exception($"Vaccine with ID {request.VaccineId} not found or inactive.");

                var existingDetail = await _vaccinePackageDetailRepository.Entities
                    .FirstOrDefaultAsync(pd => pd.VaccinePackageId == packageId && pd.VaccineId == request.VaccineId);
                if (existingDetail != null)
                    throw new Exception("Vaccine already exists in this package.");

                var packageDetail = new VaccinePackageDetail
                {
                    Id = Guid.NewGuid().ToString(),
                    VaccineId = request.VaccineId,
                    VaccinePackageId = packageId
                    // Loại bỏ PackagePrice
                };
                await _vaccinePackageDetailRepository.InsertAsync(packageDetail);

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to add vaccine to package: {ex.Message}");
            }
        }

        // Các phương thức không thay đổi: RemoveVaccineFromPackageAsync, DeletePackageAsync, GetAllVaccinesAndPackagesAsync
        public async Task RemoveVaccineFromPackageAsync(string packageId, VaccinePackageUpdateRequestDTO request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var package = await _vaccinePackageRepository.GetByIdAsync(packageId);
                if (package == null || !package.PackageStatus)
                    throw new Exception("Vaccine package not found or inactive.");

                var detail = await _vaccinePackageDetailRepository.Entities
                    .FirstOrDefaultAsync(pd => pd.VaccinePackageId == packageId && pd.VaccineId == request.VaccineId);
                if (detail == null)
                    throw new Exception("Vaccine not found in this package.");

                await _vaccinePackageDetailRepository.DeleteAsync(detail.Id);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to remove vaccine from package: {ex.Message}");
            }
        }

        public async Task DeletePackageAsync(string id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var package = await _vaccinePackageRepository.GetByIdAsync(id);
                if (package == null || !package.PackageStatus)
                    throw new Exception("Vaccine package not found or inactive.");

                package.PackageStatus = false;
                await _vaccinePackageRepository.UpdateAsync(package);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception($"Failed to delete package: {ex.Message}");
            }
        }

        public async Task<CombinedVaccineResponseDTO> GetAllVaccinesAndPackagesAsync()
        {
            var vaccines = await _vaccineRepository.Entities
                .Where(v => v.Status != "0")
                .Select(v => new VaccineResponseDTO
                {
                    Id = v.Id,
                    Name = v.Name,
                    IngredientsDescription = v.IngredientsDescription,
                    UnitOfVolume = v.UnitOfVolume,
                    MinAge = v.MinAge,
                    MaxAge = v.MaxAge,
                    BetweenPeriod = v.BetweenPeriod,
                    QuantityAvailable = v.QuantityAvailable,
                    Price = v.Price,
                    ProductionDate = v.ProductionDate,
                    ExpirationDate = v.ExpirationDate,
                    Status = v.Status,
                    VaccineCategoryId = v.VaccineCategoryId,
                    BatchId = v.BatchId,
                    Image = v.Image
                }).ToListAsync();

            var packages = await _vaccinePackageRepository.Entities
                .Where(p => p.PackageStatus)
                .Include(p => p.PackageDetails)
                .ThenInclude(pd => pd.Vaccine)
                .Select(p => new VaccinePackageResponseDTO
                {
                    Id = p.Id,
                    PackageName = p.PackageName,
                    PackageDescription = p.PackageDescription,
                    PackageStatus = p.PackageStatus,
                    Vaccines = p.PackageDetails.Select(pd => new VaccineResponseDTO
                    {
                        Id = pd.Vaccine.Id,
                        Name = pd.Vaccine.Name,
                        IngredientsDescription = pd.Vaccine.IngredientsDescription,
                        UnitOfVolume = pd.Vaccine.UnitOfVolume,
                        MinAge = pd.Vaccine.MinAge,
                        MaxAge = pd.Vaccine.MaxAge,
                        BetweenPeriod = pd.Vaccine.BetweenPeriod,
                        QuantityAvailable = pd.Vaccine.QuantityAvailable,
                        Price = pd.Vaccine.Price,
                        ProductionDate = pd.Vaccine.ProductionDate,
                        ExpirationDate = pd.Vaccine.ExpirationDate,
                        Status = pd.Vaccine.Status,
                        VaccineCategoryId = pd.Vaccine.VaccineCategoryId,
                        BatchId = pd.Vaccine.BatchId,
                        Image = pd.Vaccine.Image
                    }).ToList()
                }).ToListAsync();

            return new CombinedVaccineResponseDTO
            {
                Vaccines = vaccines,
                VaccinePackages = packages
            };
        }
    }
}