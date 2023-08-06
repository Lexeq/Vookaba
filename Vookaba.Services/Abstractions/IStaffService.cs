namespace Vookaba.Services.Abstractions
{
    public interface IStaffAggregationService
    {
        public Task<ICollection<StaffDTO>> GetStaffAsync();
    }
}
