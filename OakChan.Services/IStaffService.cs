using OakChan.Services.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OakChan.Services
{
    public interface IStaffAggregationService
    {
        public Task<ICollection<StaffDTO>> GetStaffAsync();
    }
}
