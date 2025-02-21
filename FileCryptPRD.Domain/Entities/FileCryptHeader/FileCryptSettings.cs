namespace FileCryptPRD.Domain.Entities.FileCryptHeader;

public class FileCryptSettings
{
    public Uri BaseUrl { get; set; }
    public string LinkEndpoint { get; set; }
    public string CreateEndpoint { get; set; }

    public Uri CreateEndpointFullUrl => new Uri(BaseUrl, CreateEndpoint);
}
