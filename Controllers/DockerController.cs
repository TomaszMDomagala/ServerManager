using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ServManager.SSHConnection;

namespace ServManager.Docker
{

    public record class Container()
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public string? State { get; set; }
        public string? Status { get; set; }
    }

    public class DockerController
    {
        // https://docs.docker.com/engine/api/v1.42/#tag/Container/operation/ContainerRestart
        public async Task<List<Container>> GetRunningContainers(IPAddress Address)
        {
            HttpClient client = new();
            await using Stream stream =
                await client.GetStreamAsync($"http://{Address}:1111/containers/json?all=True");
            var containers = await JsonSerializer.DeserializeAsync<List<Container>>(stream);

            return containers ?? new();
        }

        public async Task StartContainer(IPAddress Address, string id)
        {
            HttpClient client = new();

            using HttpResponseMessage response = await client.PostAsync(
                $"http://{Address}:1111/containers/{id}/start",
                null
            );
        }

        public async Task StopContainer(IPAddress Address, string id)
        {
            HttpClient client = new();

            using HttpResponseMessage response = await client.PostAsync(
                $"http://{Address}:1111/containers/{id}/stop",
                null
            );
        }

        public async Task RestartContainer(IPAddress Address, string id)
        {
            HttpClient client = new();

            using HttpResponseMessage response = await client.PostAsync(
                $"http://{Address}:1111/containers/{id}/restart",
                null
            );
        }
    }
}