﻿using Vookaba.Services.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vookaba.Services.Abstractions
{
    public interface IStaffAggregationService
    {
        public Task<ICollection<StaffDTO>> GetStaffAsync();
    }
}