using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebFramework.Api;

namespace MyApi.Controllers.v1
{
    public class CspPost
    {
        [JsonPropertyName("csp-report")]
        public CspReport CspReport { get; set; }
    }

    public class CspReport
    {
        [JsonPropertyName("document-uri")]
        public string DocumentUri { get; set; }

        [JsonPropertyName("referrer")]
        public string Referrer { get; set; }

        [JsonPropertyName("violated-directive")]
        public string ViolatedDirective { get; set; }

        [JsonPropertyName("effective-directive")]
        public string EffectiveDirective { get; set; }

        [JsonPropertyName("original-policy")]
        public string OriginalPolicy { get; set; }

        [JsonPropertyName("disposition")]
        public string Disposition { get; set; }

        [JsonPropertyName("blocked-uri")]
        public string BlockedUri { get; set; }

        [JsonPropertyName("line-number")]
        public int LineNumber { get; set; }

        [JsonPropertyName("column-number")]
        public int ColumnNumber { get; set; }

        [JsonPropertyName("source-file")]
        public string SourceFile { get; set; }

        [JsonPropertyName("status-code")]
        public string StatusCode { get; set; }

        [JsonPropertyName("script-sample")]
        public string ScriptSample { get; set; }
    }

    [ApiVersion("1")]
    public class CspReportController : ControllerBase
    {
        private readonly ILogger<CspReportController> _logger;

        public CspReportController(ILogger<CspReportController> logger)
        {
            _logger = logger;
        }

        [HttpPost("[action]")]
        [IgnoreAntiforgeryToken]
        public async Task<ApiResult> Log()
        {
            /* a sample payload:
            {
              "csp-report": {
                "document-uri": "http://localhost:5000/untypedSha",
                "referrer": "",
                "violated-directive": "script-src",
                "effective-directive": "script-src",
                "original-policy": "default-src 'self'; style-src 'self'; script-src 'self'; font-src 'self'; img-src 'self' data:; connect-src 'self'; media-src 'self'; object-src 'self'; report-uri /api/Home/CspReport",
                "disposition": "enforce",
                "blocked-uri": "eval",
                "line-number": 21,
                "column-number": 8,
                "source-file": "http://localhost:5000/scripts.bundle.js",
                "status-code": 200,
                "script-sample": ""
              }
            }
            */

            using (var bodyReader = new StreamReader(HttpContext.Request.Body))
            {
                var body = await bodyReader.ReadToEndAsync();

                _logger.LogError($"Content Security Policy Error: {body}");

                HttpContext.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body));
                JsonSerializer.Deserialize<CspPost>(body);
            }

            return Ok();
        }
    }
}