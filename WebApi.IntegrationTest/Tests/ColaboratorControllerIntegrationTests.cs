using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Application.DTO;
using DataModel.Repository;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;
using WebApi.IntegrationTests.Helpers;

namespace WebApi.IntegrationTests
{
    public class SomeIntegrationTests : IClassFixture<IntegrationTestsWebApplicationFactory<Program>>
    {
        private readonly IntegrationTestsWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public SomeIntegrationTests(IntegrationTestsWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = true
            });
        }

        [Theory]
        [InlineData("/api/colaborator")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Act
            var response = await _client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Fact]
        public async Task GetColaborators_ReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AbsanteeContext>();

                Utilities.ReinitializeDbForTests(db);
                Utilities.InitializeDbForTests(db);
            }

            // Act
            var response = await _client.GetAsync("/api/colaborator");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(responseBody), "Response body is null or empty");

            var jsonDocument = JsonDocument.Parse(responseBody);
            var jsonArray = jsonDocument.RootElement;

            Assert.True(jsonArray.ValueKind == JsonValueKind.Array, "Response body is not a JSON array");
            Assert.Equal(3, jsonArray.GetArrayLength());
        }

        [Fact]
        public async Task GetColaborators_ReturnsEmptyArrayWhenNoData()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AbsanteeContext>();

                Utilities.ReinitializeDbForTests(db); 
            }

            // Act
            var response = await _client.GetAsync("/api/colaborator");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var colaborators = JsonConvert.DeserializeObject<List<ColaboratorDTO>>(responseBody);

            Assert.Empty(colaborators);
        }

        [Fact]
        public async Task GetColaborators_ReturnsSeedData()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AbsanteeContext>();

                Utilities.ReinitializeDbForTests(db);
                Utilities.InitializeDbForTests(db);
            }

            // Act
            var response = await _client.GetAsync("/api/colaborator");

            // Assert
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var colaborators = JsonConvert.DeserializeObject<List<ColaboratorDTO>>(responseBody);

            Assert.Equal(3, colaborators.Count);
            Assert.Equal("Catarina Moreira", colaborators[0].Name);
            Assert.Equal("a", colaborators[1].Name);
            Assert.Equal("kasdjflkadjf lkasdfj laksdjf alkdsfjv alkdsfjv asl", colaborators[2].Name);
        }

        [Theory]
        [InlineData("/api/invalidendpoint")]
        public async Task Get_InvalidEndpoint_ReturnsNotFound(string url)
        {
            // Act
            var response = await _client.GetAsync(url);

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetColaborators_ResponseTimeIsAcceptable()
        {
            // Arrange
            var maxResponseTime = TimeSpan.FromSeconds(2);

            // Act
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var response = await _client.GetAsync("/api/colaborator");
            stopwatch.Stop();

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(stopwatch.Elapsed < maxResponseTime, $"Response time exceeded {maxResponseTime.TotalSeconds} seconds");
        }
    }
}
