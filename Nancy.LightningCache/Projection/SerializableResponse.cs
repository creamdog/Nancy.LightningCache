using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Nancy.LightningCache.Projection
{
    /// <summary>
    /// Persistable Nancy Response
    /// </summary>
    public class SerializableResponse
    {
        public SerializableResponse()
        {
        }

        public SerializableResponse(Response response, DateTime expiration)
        {
            this.Expiration = expiration;
            this.ContentType = response.ContentType;
            this.Headers = response.Headers;
            this.StatusCode = response.StatusCode;

            using (var memoryStream = new MemoryStream())
            {
                response.Contents(memoryStream);
                this.Contents = Encoding.UTF8.GetString(memoryStream.GetBuffer().Where(a => a != 0).ToArray());
            }
        }
        public string ContentType { get; set; }
        public IDictionary<string, string> Headers { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Contents { get; set; }
        public DateTime Expiration { get; set; }
    }
}
