using System.Security.Cryptography;
using System.Text;

namespace SocialFood.Web.Utils;

public class SHAUtil
{
    public static String CalculateHash(string value)
    {
        using SHA256 hash = SHA256.Create();
        return String.Concat(hash
          .ComputeHash(Encoding.UTF8.GetBytes(value))
          .Select(item => item.ToString("x2")));
    }
}

