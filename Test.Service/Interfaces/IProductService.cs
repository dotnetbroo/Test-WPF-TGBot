using Test.Service.Commons.Configurations;
using Test.Service.DTOs;

namespace Test.Service.Interfaces;

public interface IProductService
{
    Task<bool> RemoveAsync(long id);
    Task<byte[]> DownloadAsync(string videoPath);
    Task<ProductForResultDto> RetrieveByIdAsync(long id);
    Task<IEnumerable<ProductForResultDto>> RetrieveAllAsync(PaginationParams @params);
    Task<ProductForResultDto> CreateAsync(ProductForCreationDto productForCreationDto);
    Task<ProductForResultDto> ModifyAsync(long id, ProductForUpdateDto productForUpdateDto);
}
