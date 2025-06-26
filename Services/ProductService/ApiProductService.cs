using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Yuhnevich_vb_lab.Domain.Entities;
using Yuhnevich_vb_lab.Domain.Models;
using Yuhnevich_vb_lab.Services.ProductService;

namespace Yuhnevich_vb_lab.UI.Services.ProductService
{
    public class ApiProductService(HttpClient httpClient) : IProductService
    {
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.Preserve,
            PropertyNameCaseInsensitive = true
        };

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
                    var apiResponse = await result.Content.ReadFromJsonAsync<ResponseData<ListModel<Dish>>>(_jsonOptions);
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
            var result = await httpClient.GetAsync($"{httpClient.BaseAddress}id/{id}");

            if (result.IsSuccessStatusCode)
            {
                return new ResponseData<Dish>
                {
                    Data = await result.Content.ReadFromJsonAsync<Dish>(_jsonOptions),
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
            var responseData = new ResponseData<Dish>();

            try
            {
                // Валидация входных данных
                if (string.IsNullOrEmpty(dish.Name))
                {
                    responseData.Success = false;
                    responseData.ErrorMessage = "Название блюда не указано";
                    Console.WriteLine("ApiProductService.CreateProductAsync: Validation failed - Name is empty");
                    return responseData;
                }
                if (dish.CategoryId <= 0)
                {
                    responseData.Success = false;
                    responseData.ErrorMessage = "Категория не выбрана";
                    Console.WriteLine("ApiProductService.CreateProductAsync: Validation failed - CategoryId is invalid");
                    return responseData;
                }
                if (formFile != null)
                {
                    Console.WriteLine($"ApiProductService.CreateProductAsync: File detected - Name={formFile.FileName}, Size={formFile.Length}, Type={formFile.ContentType}");
                    if (!formFile.ContentType.StartsWith("image/"))
                    {
                        responseData.Success = false;
                        responseData.ErrorMessage = "Файл должен быть изображением";
                        Console.WriteLine("ApiProductService.CreateProductAsync: Validation failed - Invalid file type");
                        return responseData;
                    }
                    if (formFile.Length > 5 * 1024 * 1024)
                    {
                        responseData.Success = false;
                        responseData.ErrorMessage = "Файл слишком большой, максимум 5 МБ";
                        Console.WriteLine("ApiProductService.CreateProductAsync: Validation failed - File too large");
                        return responseData;
                    }
                }
                else
                {
                    Console.WriteLine("ApiProductService.CreateProductAsync: No file provided");
                }

                // Шаг 1: Отправить JSON-запрос на создание блюда
                Console.WriteLine($"ApiProductService.CreateProductAsync: Sending POST to {httpClient.BaseAddress}json, Dish={JsonSerializer.Serialize(dish)}");
                var createResponse = await httpClient.PostAsJsonAsync($"{httpClient.BaseAddress}json", dish, _jsonOptions);
                if (!createResponse.IsSuccessStatusCode)
                {
                    var errorContent = await createResponse.Content.ReadAsStringAsync();
                    responseData.Success = false;
                    responseData.ErrorMessage = $"Не удалось создать блюдо: {createResponse.StatusCode} - {createResponse.ReasonPhrase}. Details: {errorContent}";
                    Console.WriteLine($"ApiProductService.CreateProductAsync: Create failed, StatusCode={createResponse.StatusCode}, Reason={createResponse.ReasonPhrase}, Content={errorContent}");
                    return responseData;
                }

                // Получить созданное блюдо
                var createdDish = await createResponse.Content.ReadFromJsonAsync<Dish>(_jsonOptions);
                if (createdDish == null)
                {
                    responseData.Success = false;
                    responseData.ErrorMessage = "API вернул пустой или некорректный ответ для созданного блюда";
                    Console.WriteLine("ApiProductService.CreateProductAsync: Created dish is null");
                    return responseData;
                }

                responseData.Data = createdDish;
                responseData.Success = true;

                // Шаг 2: Если передан файл изображения, отправить запрос на сохранение изображения
                if (formFile != null)
                {
                    var imageUrl = $"{httpClient.BaseAddress}{createdDish.Id}/image";
                    Console.WriteLine($"ApiProductService.CreateProductAsync: Sending POST to {imageUrl}, File={formFile.FileName}");
                    var imageContent = new MultipartFormDataContent();
                    var fileStreamContent = new StreamContent(formFile.OpenReadStream());
                    fileStreamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(formFile.ContentType);
                    imageContent.Add(fileStreamContent, "image", formFile.FileName);

                    var imageResponse = await httpClient.PostAsync(imageUrl, imageContent);
                    if (!imageResponse.IsSuccessStatusCode)
                    {
                        var imageErrorContent = await imageResponse.Content.ReadAsStringAsync();
                        responseData.ErrorMessage = $"Блюдо создано, но не удалось сохранить изображение: {imageResponse.StatusCode} - {imageResponse.ReasonPhrase}. Details: {imageErrorContent}";
                        Console.WriteLine($"ApiProductService.CreateProductAsync: Image upload failed, StatusCode={imageResponse.StatusCode}, Reason={imageResponse.ReasonPhrase}, Content={imageErrorContent}");
                    }
                    else
                    {
                        var imageResult = await imageResponse.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                        if (imageResult != null && imageResult.TryGetValue("imageUrl", out var imageUrlValue))
                        {
                            createdDish.Image = imageUrlValue;
                            Console.WriteLine($"ApiProductService.CreateProductAsync: Image uploaded, ImageUrl={imageUrlValue}");
                        }
                        else
                        {
                            responseData.ErrorMessage = "Блюдо создано, но не удалось получить URL изображения";
                            Console.WriteLine("ApiProductService.CreateProductAsync: Image uploaded, but imageUrl not found in response");
                        }
                    }
                }

                Console.WriteLine($"ApiProductService.CreateProductAsync: Dish created, Id={createdDish.Id}, Image={(formFile != null ? "Uploaded" : "Not provided")}");
                return responseData;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ApiProductService.CreateProductAsync: Exception: {ex.Message}, StackTrace: {ex.StackTrace}");
                return new ResponseData<Dish>
                {
                    Success = false,
                    ErrorMessage = $"Исключение при создании блюда: {ex.Message}"
                };
            }
        }

        public async Task UpdateProductAsync(int id, Dish dish, IFormFile? formFile)
        {
            var formContent = new MultipartFormDataContent();

            // Добавляем данные блюда как JSON
            formContent.Add(new StringContent(JsonSerializer.Serialize(dish, _jsonOptions), System.Text.Encoding.UTF8, "application/json"), "dish");

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