namespace MathAppApi.Shared.Cookies;

public class CookieConfig
{
    public string Name { get; set; } = "";
    public string Domain { get; set; } = "";
    public string Path { get; set; } = "/";
    public bool Secure { get; set; } = true;
    public bool HttpOnly { get; set; } = true;
    public string SameSite { get; set; } = "Lax";
    public int ExpirationMinutes { get; set; }= 30;
}

public class CookieSettings
{
    public Dictionary<string, CookieConfig> CookieConfigs { get; set; } = [];
}