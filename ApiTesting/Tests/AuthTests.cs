using ApiTesting.Helpers;

namespace ApiTesting.Tests;

[TestFixture]
public class AuthTests
{
    private ApiClient _api = null!;

    [SetUp]
    public async Task SetUp()
    {
        _api = new ApiClient();
        await _api.InitializeAsync();
    }

    [TearDown]
    public async Task TearDown() => await _api.DisposeAsync();

    [Test]
    public async Task CreateToken_WithValidCredentials_ShouldReturnToken()
    {
        var response = await _api.Request.PostAsync("/auth", new()
        {
            DataObject = new { username = "admin", password = "password123" }
        });

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();
        var token = json.Value.GetProperty("token").GetString();

        Assert.That(token, Is.Not.Null.And.Not.Empty);
    }

    [Test]
    public async Task CreateToken_WithInvalidCredentials_ShouldReturnBadCredentials()
    {
        var response = await _api.Request.PostAsync("/auth", new()
        {
            DataObject = new { username = "invalid", password = "wrong" }
        });

        Assert.That(response.Status, Is.EqualTo(200));

        var json = await response.JsonAsync();
        var reason = json.Value.GetProperty("reason").GetString();

        Assert.That(reason, Is.EqualTo("Bad credentials"));
    }

    [Test]
    public async Task CreateToken_WithMissingUsername_ShouldReturnBadCredentials()
    {
        var response = await _api.Request.PostAsync("/auth", new()
        {
            DataObject = new { password = "password123" }
        });

        var json = await response.JsonAsync();
        var reason = json.Value.GetProperty("reason").GetString();

        Assert.That(reason, Is.EqualTo("Bad credentials"));
    }

    [Test]
    public async Task CreateToken_WithMissingPassword_ShouldReturnBadCredentials()
    {
        var response = await _api.Request.PostAsync("/auth", new()
        {
            DataObject = new { username = "admin" }
        });

        var json = await response.JsonAsync();
        var reason = json.Value.GetProperty("reason").GetString();

        Assert.That(reason, Is.EqualTo("Bad credentials"));
    }

    [Test]
    public async Task CreateToken_WithEmptyBody_ShouldReturnBadCredentials()
    {
        var response = await _api.Request.PostAsync("/auth", new()
        {
            DataObject = new { }
        });

        var json = await response.JsonAsync();
        var reason = json.Value.GetProperty("reason").GetString();

        Assert.That(reason, Is.EqualTo("Bad credentials"));
    }
}
