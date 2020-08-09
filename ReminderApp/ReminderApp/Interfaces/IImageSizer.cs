using System.Threading.Tasks;
using Xamarin.Forms;

namespace ReminderApp.Interfaces
{
    public interface IImageSizer
    {
        Task<Size> GetSizeForImage(string fileName);
    }
}
