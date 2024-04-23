using Microsoft.AspNetCore.Http;

namespace Test.Service.DTOs;

public record ProductForUpdateDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IFormFile Video { get; set; }
}
