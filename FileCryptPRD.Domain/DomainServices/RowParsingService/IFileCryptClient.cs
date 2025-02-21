using FileCryptPRD.Domain.Entities.FileCryptHeader;

namespace FileCryptPRD.Domain.DomainServices.RowParsingService;
public interface IFileCryptClient
{
    Task<HttpResponseMessage> GetHeadersAsync(Uri url);
    Task<string> GetAsync(string url, StringContent? content);
    Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage);
}