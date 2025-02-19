﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Base;

namespace IRepositories.Entity
{

    public class VaccinePackageDetail : BaseEntity
    {
        public string VaccineId { get; set; }
        public string VaccinePackageId { get; set; }
        public int PackagePrice { get; set; }

        // Navigation properties
        [ForeignKey("VaccineId")]
        public virtual Vaccine Vaccine { get; set; }

        [ForeignKey("VaccinePackageId")]
        public virtual VaccinePackage VaccinePackage { get; set; }
    }

}
