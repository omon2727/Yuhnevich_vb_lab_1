using System.Net.Http;
using System.Net.Http.Json;
using System.Web;
using Microsoft.AspNetCore.Http;
using Yuhnevich_vb_lab.Domain.Entities;
using Yuhnevich_vb_lab.Domain.Models;
using Yuhnevich_vb_lab.Services.ProductService;

namespace Yuhnevich_vb_lab.UI.Services.ProductService
{
    public class ApiProductService(HttpClient httpClient) : IProductService
    {
        public async Task<ResponseData<ListModel<Dish>>> GetProductListAsync(string? categoryNormalizedName, int pageNo = 1)
        {
            var path = string.IsNullOrEmpty(categoryNormalizedName)
                ? (pageNo == 1 ? "" : $"page/{pageNo}")
                : $"{categoryNormalizedName}/{pageNo}";
            var fullUri = new Uri(httpClient.BaseAddress + path, UriKind.RelativeOrAbsolute);
            Console.WriteLine($"ApiProductService: Sending request to: {fullUri}");

            try
            {
                var result = await httpClient.GetAsync(fullUri);
                Console.WriteLine($"ApiProductService: StatusCode={result.StatusCode}, ReasonPhrase={result.ReasonPhrase}");
                if (result.IsSuccessStatusCode)
                {
                    var apiResponse = await result.Content.ReadFromJsonAsync<ResponseData<ListModel<Dish>>>();
                    if (apiResponse == null || apiResponse.Data == null)
                    {
                        Console.WriteLine("ApiProductService: apiResponse or apiResponse.Data is null");
                        return new ResponseData<ListModel<Dish>>
                        {
                            Success = false,
                            ErrorMessage = "API вернул пустой или некорректный ответ"
                        };
                    }

                    Console.WriteLine($"ApiProductService: ItemsCount={apiResponse.Data.Items?.Count ?? 0}, CurrentPage={apiResponse.Data.CurrentPage}, TotalPages={apiResponse.Data.TotalPages}");
                    return new ResponseData<ListModel<Dish>>
                    {
                        Data = new ListModel<Dish>
                        {
                            Items = apiResponse.Data.Items ?? new List<Dish>(),
                            CurrentPage = apiResponse.Data.CurrentPage,
                            TotalPages = apiResponse.Data.TotalPages
                        },
                        Success = apiResponse.Success,
                        ErrorMessage = apiResponse.ErrorMessage
                    };
                }

                Console.WriteLine($"ApiProductService: Error: {result.StatusCode} - {result.ReasonPhrase}");
                return new ResponseData<ListModel<Dish>>
                {
                    Success = false,
                    ErrorMessage = $"Ошибка чтения API: {result.StatusCode} - {result.ReasonPhrase}"
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiProductService: Exception: {ex.Message}");
                return new ResponseData<ListModel<Dish>>
                {
                    Success = false,
                    ErrorMessage = $"Исключение при вызове API: {ex.Message}"
                };
            }
        }



        public async Task<ResponseData<Dish>> GetProductByIdAsync(int id)
        {
            var result = await httpClient.GetAsync($"{httpClient.BaseAddress}{id}");

            if (result.IsSuccessStatusCode)
            {
                return new ResponseData<Dish>
                {
                    Data = await result.Content.ReadFromJsonAsync<Dish>(),
                    Success = true
                };
            }

            return new ResponseData<Dish>
            {
                Success = false,
                ErrorMessage = $"Ошибка получения блюда с ID {id}: {result.ReasonPhrase}"
            };
        }

        public async Task<ResponseData<Dish>> CreateProductAsync(Dish dish, IFormFile? formFile)
        {
            var formContent = new MultipartFormDataContent();

            // Добавляем данные блюда как JSON
            formContent.Add(new StringContent(System.Text.Json.JsonSerializer.Serialize(dish), System.Text.Encoding.UTF8, "application/json"), "dish");

            // Если есть файл изображения, добавляем его
            if (formFile != null)
            {
                var fileStreamContent = new StreamContent(formFile.OpenReadStream());
                fileStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(formFile.ContentType);
                formContent.Add(fileStreamContent, "formFile", formFile.FileName);
            }

            var result = await httpClient.PostAsync(httpClient.BaseAddress, formContent);

            if (result.IsSuccessStatusCode)
            {
                return new ResponseData<Dish>
                {
                    Data = await result.Content.ReadFromJsonAsync<Dish>(),
                    Success = true
                };
            }

            return new ResponseData<Dish>
            {
                Success = false,
                ErrorMessage = $"Ошибка создания блюда: {result.ReasonPhrase}"
            };
        }

        public async Task UpdateProductAsync(int id, Dish dish, IFormFile? formFile)
        {
            var formContent = new MultipartFormDataContent();

            // Добавляем данные блюда как JSON
            formContent.Add(new StringContent(System.Text.Json.JsonSerializer.Serialize(dish), System.Text.Encoding.UTF8, "application/json"), "dish");

            // Если есть файл изображения, добавляем его
            if (formFile != null)
            {
                var fileStreamContent = new StreamContent(formFile.OpenReadStream());
                fileStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(formFile.ContentType);
                formContent.Add(fileStreamContent, "formFile", formFile.FileName);
            }

            var result = await httpClient.PutAsync($"{httpClient.BaseAddress}{id}", formContent);

            if (!result.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Ошибка обновления блюда с ID {id}: {result.ReasonPhrase}");
            }
        }

        public async Task DeleteProductAsync(int id)
        {
            var result = await httpClient.DeleteAsync($"{httpClient.BaseAddress}{id}");

            if (!result.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Ошибка удаления блюда с ID {id}: {result.ReasonPhrase}");
            }
        }
    }
}