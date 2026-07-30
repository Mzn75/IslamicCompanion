using System.Threading.Tasks;
using IslamicCompanion.Models;

namespace IslamicCompanion.Services
{
    public interface IQuranApiService
    {
        Task<QuranVerse> GetDailyVerseAsync();
        Task<QuranVerse> GetRandomVerseAsync();
    }
}
