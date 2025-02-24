using System.Security.Cryptography;
using System.Text;

namespace Core.Utils
{
    public static class CoreHelper
    {
        public static DateTimeOffset SystemTimeNow => TimeHelper.ConvertToUtcPlus7(DateTimeOffset.Now);
    }
}