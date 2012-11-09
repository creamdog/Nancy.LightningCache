using System;

namespace Nancy.LightningCache.Projection
{
    /// <summary>
    /// Cacheable Nancy Response
    /// </summary>
    public class CacheableResponse : Nancy.Response
    {
        private readonly Nancy.Response _response;

        public readonly DateTime Expiration;

        public CacheableResponse(Nancy.Response response, DateTime expiration)
        {
            _response = response;
            Expiration = expiration;
            this.ContentType = response.ContentType;
            this.Headers = response.Headers;
            this.StatusCode = response.StatusCode;
            this.Contents = _response.Contents;
        }
    }
}
