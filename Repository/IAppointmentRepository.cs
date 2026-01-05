using HMSSystem.Models;

namespace HMSSystem.Repository
{
    public interface IAppointmentRepository
    {
        Task CreateAsync(Appointment appointment);
        Task<List<Appointment>> GetByUserIdAsync(int userId);
        Task<List<Appointment>> GetAllAsync();
        Task<Appointment> GetByIdAsync(int id);
        Task UpdateAsync(Appointment appointment);
    }

}
