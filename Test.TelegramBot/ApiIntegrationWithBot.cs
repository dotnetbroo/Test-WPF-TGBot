using Newtonsoft.Json;
using System.Net.Http.Headers;
using Test.Service.DTOs;
using Test.Service.Helpers;

namespace Test.TelegramBot;

public class ApiIntegrationWithBot
{
    private string url = "https://localhost:7181/api/";

    public async Task<List<(long id, long sortNumber, string video)>> GetValuesAsync()
    {
        var resultDto = new List<ProductForResultDto>();

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.GetAsync("Product");
            response.EnsureSuccessStatusCode();
            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                resultDto = JsonConvert.DeserializeObject<List<ProductForResultDto>>(result);

                return resultDto.Select(p => (p.Id, p.SortNumber, p.Video)).ToList();
            }
            else
            {
                throw new Exception($"Response error: {response.RequestMessage}");
            }
        }
    }

    public Stream GetStreamAsync(string video)
    {
        try
        {
            string path = "../../wwwroot/";
            string videoPath = Path.Combine(path, video);
            FileStream stream = File.OpenRead(videoPath);

            return stream;
        }
        catch (Exception e)
        {
            throw;
        }

    }

    public async Task<(string videoData, string videoFileName)> GetVideoDatasOfProduct(long id)
    {
        var product = new ProductForResultDto();

        using (var client = new HttpClient())
        {
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage getData = await client.GetAsync($"Product/{id}");
            getData.EnsureSuccessStatusCode();

            if (getData.IsSuccessStatusCode)
            {
                string results = getData.Content.ReadAsStringAsync().Result;
                product = JsonConvert.DeserializeObject<ProductForResultDto>(results);
                string videoFileName = Path.GetFileName(product!.Video);

                return (product.Video, videoFileName);
            }
            else
            {
                return (null, null);
            }
        }
    }

}
