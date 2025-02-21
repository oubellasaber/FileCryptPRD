using FileCryptPRD.Domain.Entities.FileCryptHeader.ValueObjects;
using System.Globalization;

namespace FileCryptPRD.Domain.Entities.FileCryptHeader;

public class FileCryptHeader
{
    private FileCryptHeader(
        string phpSessionCookie)
    {
        PhpSessionCookie = new CookieHeader($"PHPSESSID={phpSessionCookie}");
    }

    public FileCryptHeader(
        string phpSessionCookie,
        DateTime sessionExpirationDate) : this(phpSessionCookie)
    {
        SessionExpirationDate = sessionExpirationDate;
    }

    public FileCryptHeader(
        string phpSessionCookie,
        string expirationDate,
        FileCryptHeaderConfig config) : this(phpSessionCookie)
    {
        if (!DateTime.TryParseExact(
            expirationDate,
            config.DateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal,
            out DateTime parsedSessionexpirationDate))
        {
            throw new ArgumentException("Invalid date format", nameof(expirationDate));
        }

        SessionExpirationDate = parsedSessionexpirationDate;
    }

    public HttpHeader PhpSessionCookie { get; private set; }
    public DateTime SessionExpirationDate { get; private set; }
}