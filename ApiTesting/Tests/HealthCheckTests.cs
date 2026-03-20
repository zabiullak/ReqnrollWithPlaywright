using ApiTesting.Helpers;

namespace ApiTesting.Tests;

[TestFixture]
public class HealthCheckTests
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
    public async Task Ping_ShouldReturn201()
    {
        var response = await _api.Request.GetAsync("/ping");

        Assert.That(response.Status, Is.EqualTo(201));
    }
}
