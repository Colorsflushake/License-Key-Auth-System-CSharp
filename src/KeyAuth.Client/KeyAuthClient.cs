namespace KeyAuth.Client;

using KeyAuth.Client.Config;
using KeyAuth.Client.Models;
using KeyAuth.Client.Utils;
using KeyAuth.Client.Crypto;

public sealed class KeyAuthClient : IDisposable
{
    private readonly AuthConfig _config;
    private readonly HttpHelper _http;
    private readonly RequestSigner _signer;
    private readonly HwidGenerator _hwid;
    private UserSession? _currentSession;

    public bool IsAuthenticated => _currentSession is not null && !_currentSession.IsExpired;

    public KeyAuthClient(AuthConfig config)
    {
        _config = config;
        _signer = new RequestSigner(config.AppSecret);
        _http = new HttpHelper(config.ServerUrl, _signer);
        _hwid = new HwidGenerator();
    }

    public async Task<LicenseInfo?> AuthenticateAsync(string licenseKey)
    {
        var hwid = _hwid.Generate();
        var payload = new Dictionary<string, string>
        {
            ["key"] = licenseKey,
            ["hwid"] = hwid,
            ["app_id"] = _config.ApplicationId,
            ["version"] = _config.AppVersion
        };

        var response = await _http.PostAsync("/api/auth/validate", payload);
        if (response is null) return null;

        var license = LicenseInfo.FromJson(response);
        if (license is not null)
        {
            _currentSession = new UserSession(license.Username, hwid, license.ExpiresAt);
        }

        return license;
    }

    public async Task<bool> HeartbeatAsync()
    {
        if (_currentSession is null) return false;

        var payload = new Dictionary<string, string>
        {
            ["session_id"] = _currentSession.SessionId,
            ["hwid"] = _currentSession.HardwareId
        };

        var response = await _http.PostAsync("/api/auth/heartbeat", payload);
        return response is not null;
    }

    public void Dispose()
    {
        _http.Dispose();
    }
}
