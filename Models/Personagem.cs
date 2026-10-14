using System.Text.Json.Serialization;

namespace BlazorDbzPersonagem.Models;

public class Personagem
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Nome { get; set; }

    [JsonPropertyName("originPlanet")]
    public Planeta? PlanetaOrigem { get; set; }

    [JsonPropertyName("transformations")]
    public List<Transformacao?>? Transformacoes { get; set; }
}
