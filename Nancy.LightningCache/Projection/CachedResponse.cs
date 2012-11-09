using System;
using System.Globalization;
using System.IO;

namespace Nancy.LightningCache.Projection
{
    /// <summary>
    /// Cached Nancy Response
    /// </summary>
    public class CachedResponse : Response
    {
        public readonly string OldResponseOutput;

        public readonly DateTime Expiration;

        public CachedResponse(SerializableResponse response)
        {
            ContentType = response.ContentType;
            Headers = response.Headers;
            StatusCode = response.StatusCode;
            OldResponseOutput = response.Contents;
            Contents = GetContents(this.OldResponseOutput);
            Expiration = response.Expiration;
            
            Headers["X-Nancy-LightningCache-Expiration"] = response.Expiration.ToString(CultureInfo.InvariantCulture);
        }

        protected static Action<Stream> GetContents(string contents)
        {
            return stream =>
            {
                var writer = new StreamWriter(stream) { AutoFlush = true };
                writer.Write(contents);
            };
        }
    }
}
