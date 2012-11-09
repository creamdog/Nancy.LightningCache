using System;
using System.Security.Cryptography;
using System.Text;
using Nancy.LightningCache.Projection;

namespace Nancy.LightningCache.CacheStore
{
    public class DiskCacheStore : ICacheStore
    {
        public static string Hash(string str)
        {
            var hasher = SHA256.Create();
            var inputBytes = Encoding.ASCII.GetBytes(str);
            var hashBytes = hasher.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            for (var i = 0; i < hashBytes.Length; i++)
                sb.Append(hashBytes[i].ToString("x2"));

            return sb.ToString();
        }

        public CachedResponse Get(string key)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, NancyContext context, DateTime absoluteExpiration)
        {
            throw new NotImplementedException();
        }
    }
}
