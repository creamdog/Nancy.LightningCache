using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Nancy.Json;
using Nancy.LightningCache.Projection;

namespace Nancy.LightningCache.CacheStore
{
    /// <summary>
    /// Stores Responses serialized as JSON.
    /// File names are SHA256 hashes of the supplied key with each response sent to this cache store.
    /// </summary>
    public class DiskCacheStore : ICacheStore
    {
        private readonly string _cacheDirectory = string.Empty;
        private readonly JavaScriptSerializer _javaScriptSerializer; 
        private readonly object _lock = new object();
        private static readonly Dictionary<string, DateTime> FileKeyExpirationRecord = new Dictionary<string, DateTime>();
        private static TimeSpan _expiredFilesDeletionOffset;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cacheDirectory">absolute path to directory where to store cached files</param>
        /// <param name="expiredFilesDeletionOffset">extra time to keep expired files before deleting them</param>
        public DiskCacheStore(string cacheDirectory, TimeSpan expiredFilesDeletionOffset)
        {
            if (!Path.IsPathRooted(cacheDirectory))
                throw new ArgumentException("cache directory must contain a root, it must be an absolute path", "cacheDirectory");

            if (!Directory.Exists(cacheDirectory))
                Directory.CreateDirectory(cacheDirectory);

            File.WriteAllText(Path.Combine(cacheDirectory, GetType().FullName), GetType().FullName);
            File.Delete(Path.Combine(cacheDirectory, GetType().FullName));

            _cacheDirectory = cacheDirectory;
            _javaScriptSerializer = new JavaScriptSerializer();

            _expiredFilesDeletionOffset = expiredFilesDeletionOffset;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="cacheDirectory">absolute path to directory where to store cached files</param>
        public DiskCacheStore(string cacheDirectory)
        {
            if (!Path.IsPathRooted(cacheDirectory))
                throw new ArgumentException("cache directory must contain a root, it must be an absolute path", "cacheDirectory");

            if (!Directory.Exists(cacheDirectory))
                Directory.CreateDirectory(cacheDirectory);

            File.WriteAllText(Path.Combine(cacheDirectory, GetType().FullName), GetType().FullName);
            File.Delete(Path.Combine(cacheDirectory, GetType().FullName));

            _cacheDirectory = cacheDirectory;
            _javaScriptSerializer = new JavaScriptSerializer();

            _expiredFilesDeletionOffset = new TimeSpan(0,0,24,0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static string Hash(string str)
        {
            var hasher = SHA256.Create();
            var inputBytes = Encoding.ASCII.GetBytes(str);
            var hashBytes = hasher.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            for (var i = 0; i < hashBytes.Length; i++)
                sb.Append(hashBytes[i].ToString("x2"));

            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        private void DeleteExpiredCacheFiles()
        {
            var files = FileKeyExpirationRecord
            .Where(record => DateTime.Now >= record.Value.Add(_expiredFilesDeletionOffset))
            .Select(record => record.Key)
            .ToArray();
            foreach (var file in files)
            {
                if(File.Exists(Path.Combine(_cacheDirectory, file)))
                {
                    File.Delete(Path.Combine(_cacheDirectory, file));
                }
                FileKeyExpirationRecord.Remove(file);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public CachedResponse Get(string key)
        {
            lock(_lock)
            {
                var fileName = Hash(key);
                if (File.Exists(Path.Combine(_cacheDirectory, fileName)))
                {
                    var json = File.ReadAllText(Path.Combine(_cacheDirectory, fileName));
                    var serializedResponse = _javaScriptSerializer.Deserialize<SerializableResponse>(json);
                    return new CachedResponse(serializedResponse);
                } 
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            lock(_lock)
            {
                var fileName = Hash(key);
                if(File.Exists(Path.Combine(_cacheDirectory, fileName)))
                    File.Delete(Path.Combine(_cacheDirectory, fileName));
                if (FileKeyExpirationRecord.ContainsKey(fileName))
                    FileKeyExpirationRecord.Remove(fileName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="context"></param>
        /// <param name="absoluteExpiration"></param>
        public void Set(string key, NancyContext context, DateTime absoluteExpiration)
        {
            lock (_lock)
            {
               
                var fileName = Hash(key);
                if (File.Exists(Path.Combine(_cacheDirectory, fileName)))
                    File.Delete(Path.Combine(_cacheDirectory, fileName));
                var serializedResponse = new SerializableResponse(context.Response, absoluteExpiration);
                var json = _javaScriptSerializer.Serialize(serializedResponse);
                File.WriteAllText(Path.Combine(_cacheDirectory, fileName), json);

                DeleteExpiredCacheFiles();
                FileKeyExpirationRecord[fileName] = absoluteExpiration;
            }
        }
    }
}
