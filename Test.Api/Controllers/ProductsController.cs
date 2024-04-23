using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Test.Service.Commons.Configurations;
using Test.Service.DTOs;
using Test.Service.Interfaces;

namespace Test.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /*[HttpPost]
        public async Task<IActionResult> AddAsync([FromForm] ProductForCreationDto productForCreationDto)
            => Ok(await _productService.CreateAsync(productForCreationDto));*/

        /*[HttpPut]
        public async Task<IActionResult> ModifyAsync(long id, [FromForm] ProductForUpdateDto productForUpdate)
            => Ok(await _productService.ModifyAsync(id, productForUpdate));*/

        /*[HttpGet("download-video-by-path")]
        public async Task<IActionResult> DownloadAsync(string video)
            => File(await this._productService.DownloadAsync(video), "application/octet-stream", video);*/

        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParams @params)
            => Ok(await _productService.RetrieveAllAsync(@params));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync([FromRoute(Name ="id")] long id)
            => Ok(await _productService.RetrieveByIdAsync(id));

        /*[HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute(Name ="id")] long id)
            => Ok(await _productService.RemoveAsync(id));

        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<ProductForResultDto>>> SearchProducts(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Query parameter is required.");
            }

            var products = await _productService.SearchAsync(query);
            return Ok(products);
        }*/
    }
}
