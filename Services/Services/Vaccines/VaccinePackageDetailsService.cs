//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using IRepositories.IRepository;
//using ModelViews.Requests.VaccinePackage;
//using ModelViews.Responses.VaccinePackage;
//using IServices.Interfaces.Vaccines;
//using IRepositories.Entity.Vaccines;



//namespace Services.Services.Vaccines
//{
//    public class VaccinePackageDetailsService : IVaccinePackageDetailsService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IGenericRepository<VaccinePackageDetail> _repository;

//        public VaccinePackageDetailsService(IUnitOfWork unitOfWork)
//        {
//            _unitOfWork = unitOfWork;
//            _repository = _unitOfWork.GetRepository<VaccinePackageDetail>();
//        }

//        public async Task<IEnumerable<VaccinePackageDetailsResponseDTO>> GetAllDetailsAsync()
//        {
//            var details = await _repository.GetAllAsync();
//            return details.Select(d => new VaccinePackageDetailsResponseDTO
//            {
//                Id = d.Id,
//                VaccineId = d.VaccineId,
//                VaccinePackageId = d.VaccinePackageId,
//                PackagePrice = d.PackagePrice
//            }).ToList();
//        }

//        public async Task<VaccinePackageDetailsResponseDTO?> GetDetailByIdAsync(string id)
//        {
//            var detail = await _repository.GetByIdAsync(id);
//            if (detail == null) return null;
//            return new VaccinePackageDetailsResponseDTO
//            {
//                Id = detail.Id,
//                VaccineId = detail.VaccineId,
//                VaccinePackageId = detail.VaccinePackageId,
//                PackagePrice = detail.PackagePrice
//            };
//        }

//        public async Task AddDetailAsync(VaccinePackageDetailsRequestDTO detailDto)
//        {
//            var detail = new VaccinePackageDetail
//            {
//                VaccineId = detailDto.VaccineId,
//                VaccinePackageId = detailDto.VaccinePackageId,
//                PackagePrice = detailDto.PackagePrice
//            };
//            await _repository.InsertAsync(detail);
//            await _unitOfWork.SaveAsync();
//        }

//        public async Task UpdateDetailAsync(string id, VaccinePackageDetailsRequestDTO detailDto)
//        {
//            var existingDetail = await _repository.GetByIdAsync(id);
//            if (existingDetail == null)
//                throw new Exception("Vaccine package detail not found.");

//            existingDetail.VaccineId = detailDto.VaccineId;
//            existingDetail.VaccinePackageId = detailDto.VaccinePackageId;
//            existingDetail.PackagePrice = detailDto.PackagePrice;
//            await _repository.UpdateAsync(existingDetail);
//            await _unitOfWork.SaveAsync();
//        }

//        public async Task DeleteDetailAsync(string id)
//        {
//            await _repository.DeleteAsync(id);
//            await _unitOfWork.SaveAsync();
//        }
//    }
//}
