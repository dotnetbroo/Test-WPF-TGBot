using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using Test.Data.Repositories;
using Test.Domain.Entities;
using Test.Service.Commons.Configurations;
using Test.Service.Commons.Exceptions;
using Test.Service.DTOs;
using Test.Service.Helpers;
using Test.Service.Interfaces;

namespace Test.Service.Services;

public class ProductService : IProductService
{
    private readonly IRepository<Product> _productRepository;
    private readonly string _folder;
    private readonly string _folderVideos;
    private readonly string _folderPage;
    private readonly IMapper _mapper;


    public ProductService(IRepository<Product> productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _folder = WebHostEnviromentHelper.WebRootPath;
        _folderPage = "Media";
        _folderVideos = Path.Combine(_folderPage, "Products");

    }

    public async Task<ProductForResultDto> CreateAsync(ProductForCreationDto productForCreationDto)
    {
        var product = await _productRepository.SelectAll()
            .Where(n => n.Name == productForCreationDto.Name)
            .FirstOrDefaultAsync();
        if (product is not null)
            throw new CustomException(403, "Product is already exsit.");

        var wwwRootPath = Path.Combine(WebHostEnviromentHelper.WebRootPath, "Media", "Products");
        var assetsFolderPath = Path.Combine(wwwRootPath, "Media");
        var videosFolderPath = Path.Combine(assetsFolderPath, "Products");

        if (!Directory.Exists(assetsFolderPath))
        {
            Directory.CreateDirectory(assetsFolderPath);
        }

        if (!Directory.Exists(videosFolderPath))
        {
            Directory.CreateDirectory(videosFolderPath);
        }
        var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(productForCreationDto.Video.FileName);

        var fullPath = Path.Combine(wwwRootPath, fileName);

        using (var stream = File.OpenWrite(fullPath))
        {
            await productForCreationDto.Video.CopyToAsync(stream);
            await stream.FlushAsync();
            stream.Close();
        }

        string resultVideo = Path.Combine("Media", "Products", fileName);

        var mapped = _mapper.Map<Product>(productForCreationDto);
        mapped.Video = resultVideo;
        mapped.CreatedAt = DateTime.UtcNow;
        Random random = new Random();
        int randomNumber = random.Next(99999999);
        mapped.SortNumber = randomNumber;

        var result = await _productRepository.InsertAsync(mapped);

        return _mapper.Map<ProductForResultDto>(result);

    }

    public async Task<byte[]> DownloadAsync(string imageName)
    {
        var imagePath = Path.Combine(_folder, _folderVideos, imageName);
        if (File.Exists(imagePath))
            return await File.ReadAllBytesAsync(imagePath);

        throw new FileNotFoundException();
    }

    public async Task<ProductForResultDto> ModifyAsync(long id, ProductForUpdateDto productForUpdateDto)
    {
        var product = await _productRepository.SelectAll()
            .Where(n => n.Id == id)
            .FirstOrDefaultAsync();
        if (product is null)
            throw new CustomException(403, "Product is not found.");

        var fullPath = Path.Combine(WebHostEnviromentHelper.WebRootPath, product.Video);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(productForUpdateDto.Video.FileName);
        var rootPath = Path.Combine(WebHostEnviromentHelper.WebRootPath, "Media", "Products", fileName);
        using (var stream = new FileStream(rootPath, FileMode.Create))
        {
            await productForUpdateDto.Video.CopyToAsync(stream);
            await stream.FlushAsync();
            stream.Close();
        }
        string resultImage = Path.Combine("Media", "Products", fileName);

        var mapped = _mapper.Map(productForUpdateDto, product);
        mapped.Video = resultImage;
        mapped.UpdatedAt = DateTime.UtcNow;

        var result = await _productRepository.UpdateAsync(mapped);

        return _mapper.Map<ProductForResultDto>(result);
    }

    public async Task<bool> RemoveAsync(long id)
    {
        var product = await _productRepository.SelectAll()
            .Where(ea => ea.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (product is null)
            throw new CustomException(404, "Product is not found");

        var fullPath = Path.Combine(WebHostEnviromentHelper.WebRootPath, product.Video);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return await _productRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<ProductForResultDto>> RetrieveAllAsync(PaginationParams @params)
    {
        var products = await _productRepository.SelectAll()
            .AsNoTracking()
            .ToListAsync();

        return _mapper.Map<IEnumerable<ProductForResultDto>>(products);
    }

    public async Task<ProductForResultDto> RetrieveByIdAsync(long id)
    {
        var product = await _productRepository.SelectAll()
            .Where(ea => ea.Id == id)
            .AsNoTracking()
            .FirstOrDefaultAsync();

        if (product is null)
            throw new CustomException(404, "Event Asset is not found");

        return _mapper.Map<ProductForResultDto>(product);
    }
}