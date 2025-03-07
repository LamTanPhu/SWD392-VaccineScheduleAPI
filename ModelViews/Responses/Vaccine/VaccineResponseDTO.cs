﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelViews.Responses.Vaccine
{
    public class VaccineResponseDTO
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int QuantityAvailable { get; set; }
        public int Price { get; set; }
        public string Status { get; set; }
        public string VaccineCategoryId { get; set; }
        public string BatchId { get; set; }
    }
}
