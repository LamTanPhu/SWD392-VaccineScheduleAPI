
﻿using ModelViews.Responses.Vaccine;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.VaccineCategory
{
    public class VaccineCategoryResponseDTO
    {
        public string Id { get; set; }
        public string CategoryName { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
        public string? ParentCategoryId { get; set; }

        public List<VaccineResponseDTO> Vaccines { get; set; } = new List<VaccineResponseDTO>();

    }
}
