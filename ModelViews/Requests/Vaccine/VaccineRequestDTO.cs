using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ModelViews.Requests.Vaccine
{
    public class VaccineRequestDTO
    {
        [Required]
        public string Name { get; set; }

        public string? IngredientsDescription { get; set; }

        [Required]
        public int UnitOfVolume { get; set; }

        [Required]
        public int MinAge { get; set; }

        [Required]
        public int MaxAge { get; set; }

        [Required]
        public DateTime BetweenPeriod { get; set; }

        [Required]
        public int QuantityAvailable { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public DateTime ProductionDate { get; set; }

        [Required]
        public DateTime ExpirationDate { get; set; }

        [Required]
        public string VaccineCategoryId { get; set; }

        [Required]
        public string BatchId { get; set; }

        public IFormFile Image { get; set; } // Tùy chọn để upload ảnh
    }
}