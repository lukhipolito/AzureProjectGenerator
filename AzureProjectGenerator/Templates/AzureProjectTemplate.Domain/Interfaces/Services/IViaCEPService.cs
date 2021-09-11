using System.Threading.Tasks;
using AzureProjectTemplate.Domain.Models.Services;

namespace AzureProjectTemplate.Domain.Interfaces.Services
{
    public interface IViaCEPService
    {
        Task<ViaCEP> GetByCEPAsync(string cep);
    }
}
