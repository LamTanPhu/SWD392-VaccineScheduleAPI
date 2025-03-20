using AutoMapper;
using IRepositories.Entity.Vaccines;
using IRepositories.IRepository;
using IRepositories.IRepository.Vaccines;
using IServices.Interfaces.Vaccines;
using Microsoft.EntityFrameworkCore;
using ModelViews.Requests.VaccinePackage;
using ModelViews.Responses.Vaccine;
using ModelViews.Responses.VaccinePackage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Services.Services.Vaccines
{
    public class VaccinePackageService : IVaccinePackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVaccinePackageRepository _packageRepository;
        private readonly IVaccinePackageDetailsRepository _detailRepository;
        private readonly IVaccineRepository _vaccineRepository;
        private readonly IMapper _mapper;

        public VaccinePackageService(
            IUnitOfWork unitOfWork,
            IVaccinePackageRepository packageRepository,
            IVaccinePackageDetailsRepository detailRepository,
            IVaccineRepository vaccineRepository,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _packageRepository = packageRepository ?? throw new ArgumentNullException(nameof(packageRepository));
            _detailRepository = detailRepository ?? throw new ArgumentNullException(nameof(detailRepository));
            _vaccineRepository = vaccineRepository ?? throw new ArgumentNullException(nameof(vaccineRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<VaccinePackageResponseDTO>> GetAllPackagesAsync()
        {
            var packages = await _packageRepository.Entities
                .Where(p => p.PackageStatus == true) // Dùng bool
                .Include(p => p.PackageDetails)
                .ThenInclude(pd => pd.Vaccine)
                .ToListAsync();

            return _mapper.Map<IEnumerable<VaccinePackageResponseDTO>>(packages);
        }

        public async Task<VaccinePackageResponseDTO?> GetPackageByIdAsync(string id)
        {
            var package = await _packageRepository.GetByIdAsync(id);
            if (package == null || package.PackageStatus == false) // Dùng bool
                return null;

            await _packageRepository.Entities
                .Where(p => p.Id == id)
                .Include(p => p.PackageDetails)
                .ThenInclude(pd => pd.Vaccine)
                .FirstOrDefaultAsync();

            return _mapper.Map<VaccinePackageResponseDTO>(package);
        }

        public async Task<VaccinePackageResponseDTO> AddPackageAsync(VaccinePackageRequestDTO packageDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingPackage = await _packageRepository.Entities
                    .FirstOrDefaultAsync(p => p.PackageName == packageDto.PackageName && p.PackageStatus == true); // Dùng bool
                if (existingPackage != null)
                    throw new Exception("A package with this name already exists.");

                var package = _mapper.Map<VaccinePackage>(packageDto);
                package.PackageStatus = true; // Dùng bool

                await _packageRepository.InsertAsync(package);

                foreach (var vaccineDose in packageDto.VaccineDoses)
                {
                    var vaccine = await _vaccineRepository.GetByIdAsync(vaccineDose.VaccineId);
                    if (vaccine == null || vaccine.Status == "0")
                        throw new Exception($"Vaccine with ID {vaccineDose.VaccineId} not found or inactive.");

                    var packageDetail = _mapper.Map<VaccinePackageDetail>(vaccineDose);
                    packageDetail.VaccinePackageId = package.Id;
                    await _detailRepository.InsertAsync(packageDetail);
                }

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                var createdPackage = await _packageRepository.Entities
                    .Where(p => p.Id == package.Id)
                    .Include(p => p.PackageDetails)
                    .ThenInclude(pd => pd.Vaccine)
                    .FirstOrDefaultAsync();

                return _mapper.Map<VaccinePackageResponseDTO>(createdPackage);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to add package.", ex);
            }
        }

        public async Task<VaccinePackageResponseDTO?> UpdatePackageAsync(string id, VaccinePackageRequestDTO packageDto)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var existingPackage = await _packageRepository.GetByIdAsync(id);
                if (existingPackage == null || existingPackage.PackageStatus == false) // Dùng bool
                    return null;

                var duplicatePackage = await _packageRepository.Entities
                    .FirstOrDefaultAsync(p => p.PackageName == packageDto.PackageName && p.Id != id && p.PackageStatus == true); // Dùng bool
                if (duplicatePackage != null)
                    throw new Exception("Another package with this name already exists.");

                _mapper.Map(packageDto, existingPackage);

                var oldDetails = await _detailRepository.Entities
                    .Where(pd => pd.VaccinePackageId == id)
                    .ToListAsync();
                foreach (var detail in oldDetails)
                    await _detailRepository.DeleteAsync(detail.Id);

                foreach (var vaccineDose in packageDto.VaccineDoses)
                {
                    var vaccine = await _vaccineRepository.GetByIdAsync(vaccineDose.VaccineId);
                    if (vaccine == null || vaccine.Status == "0")
                        throw new Exception($"Vaccine with ID {vaccineDose.VaccineId} not found or inactive.");

                    var packageDetail = _mapper.Map<VaccinePackageDetail>(vaccineDose);
                    packageDetail.VaccinePackageId = id;
                    await _detailRepository.InsertAsync(packageDetail);
                }

                await _packageRepository.UpdateAsync(existingPackage);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();

                var updatedPackage = await _packageRepository.Entities
                    .Where(p => p.Id == id)
                    .Include(p => p.PackageDetails)
                    .ThenInclude(pd => pd.Vaccine)
                    .FirstOrDefaultAsync();

                return _mapper.Map<VaccinePackageResponseDTO>(updatedPackage);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to update package: " + ex.Message, ex);
            }
        }

        public async Task AddVaccineToPackageAsync(string packageId, VaccinePackageUpdateRequestDTO request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var package = await _packageRepository.GetByIdAsync(packageId);
                if (package == null || package.PackageStatus == false) // Dùng bool
                    throw new Exception("Vaccine package not found or inactive.");

                var vaccine = await _vaccineRepository.GetByIdAsync(request.VaccineId);
                if (vaccine == null || vaccine.Status == "0")
                    throw new Exception($"Vaccine with ID {request.VaccineId} not found or inactive.");

                var existingDetail = await _detailRepository.Entities
                    .FirstOrDefaultAsync(pd => pd.VaccinePackageId == packageId && pd.VaccineId == request.VaccineId);
                if (existingDetail != null)
                    throw new Exception("Vaccine already exists in this package.");

                var packageDetail = _mapper.Map<VaccinePackageDetail>(request);
                packageDetail.VaccinePackageId = packageId;
                await _detailRepository.InsertAsync(packageDetail);

                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to add vaccine to package.", ex);
            }
        }

        public async Task RemoveVaccineFromPackageAsync(string packageId, VaccinePackageUpdateRequestDTO request)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var package = await _packageRepository.GetByIdAsync(packageId);
                if (package == null || package.PackageStatus == false) // Dùng bool
                    throw new Exception("Vaccine package not found or inactive.");

                var detail = await _detailRepository.Entities
                    .FirstOrDefaultAsync(pd => pd.VaccinePackageId == packageId && pd.VaccineId == request.VaccineId);
                if (detail == null)
                    throw new Exception("Vaccine not found in this package.");

                await _detailRepository.DeleteAsync(detail.Id);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to remove vaccine from package.", ex);
            }
        }

        public async Task DeletePackageAsync(string id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var package = await _packageRepository.GetByIdAsync(id);
                if (package == null || package.PackageStatus == false) // Dùng bool
                    throw new Exception("Vaccine package not found.");

                package.PackageStatus = false; // Dùng bool
                await _packageRepository.UpdateAsync(package);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to delete package.", ex);
            }
        }

        public async Task<CombinedVaccineResponseDTO> GetAllVaccinesAndPackagesAsync()
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var vaccines = await _vaccineRepository.Entities
                    .Where(v => v.Status != "0")
                    .ToListAsync();

                var packages = await _packageRepository.Entities
                    .Where(p => p.PackageStatus == true) // Dùng bool
                    .Include(p => p.PackageDetails)
                    .ThenInclude(pd => pd.Vaccine)
                    .ToListAsync();

                await _unitOfWork.CommitTransactionAsync();

                return new CombinedVaccineResponseDTO
                {
                    Vaccines = _mapper.Map<List<VaccineResponseDTO>>(vaccines), // Map trực tiếp sang List
                    VaccinePackages = _mapper.Map<List<VaccinePackageResponseDTO>>(packages) // Map trực tiếp sang List
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Failed to get vaccines and packages.", ex);
            }
        }
    }
}