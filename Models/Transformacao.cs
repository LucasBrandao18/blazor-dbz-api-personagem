using System.Text.Json.Serialization;

namespace BlazorDbzPersonagem.Models;

public class Transformacao
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string? Nome { get; set; }
}
