using IRepositories.Entity;
using IRepositories.IRepository;
using Services.Interfaces;
using ModelViews.Requests.VaccinePackage;
using ModelViews.Responses.VaccinePackage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;

namespace Services
{
    public class VaccinePackageService : IVaccinePackageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<VaccinePackage> _vaccinePackageRepository;

        public VaccinePackageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _vaccinePackageRepository = _unitOfWork.GetRepository<VaccinePackage>();
        }

        public async Task<IEnumerable<VaccinePackageResponseDTO>> GetAllPackagesAsync()
        {
            var packages = await _vaccinePackageRepository.GetAllAsync();
            return packages.Select(p => new VaccinePackageResponseDTO
            {
                Id = p.Id,
                PackageName = p.PackageName,
                PackageDescription = p.PackageDescription,
                PackageStatus = p.PackageStatus
            }).ToList();
        }

        public async Task<VaccinePackageResponseDTO?> GetPackageByIdAsync(string id)
        {
            var package = await _vaccinePackageRepository.GetByIdAsync(id);
            if (package == null) return null;
            return new VaccinePackageResponseDTO
            {
                Id = package.Id,
                PackageName = package.PackageName,
                PackageDescription = package.PackageDescription,
                PackageStatus = package.PackageStatus
            };
        }

        public async Task AddPackageAsync(VaccinePackageRequestDTO packageDto)
        {
            var package = new VaccinePackage
            {
                PackageName = packageDto.PackageName,
                PackageDescription = packageDto.PackageDescription,
                PackageStatus = packageDto.PackageStatus
            };
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _vaccinePackageRepository.InsertAsync(package);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task UpdatePackageAsync(string id, VaccinePackageRequestDTO packageDto)
        {
            var existingPackage = await _vaccinePackageRepository.GetByIdAsync(id);
            if (existingPackage == null)
            {
                throw new Exception("Vaccine package not found.");
            }
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                existingPackage.PackageName = packageDto.PackageName;
                existingPackage.PackageDescription = packageDto.PackageDescription;
                existingPackage.PackageStatus = packageDto.PackageStatus;
                await _vaccinePackageRepository.UpdateAsync(existingPackage);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task DeletePackageAsync(string id)
        {
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _vaccinePackageRepository.DeleteAsync(id);
                await _unitOfWork.SaveAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}