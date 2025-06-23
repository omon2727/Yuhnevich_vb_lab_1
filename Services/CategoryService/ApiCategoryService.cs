using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using Yuhnevich_vb_lab.Domain.Entities;
using Yuhnevich_vb_lab.Domain.Models;
using Yuhnevich_vb_lab.Services.CategoryService;

namespace Yuhnevich_vb_lab.UI.Services.CategoryService
{
    public class ApiCategoryService(HttpClient httpClient) : ICategoryService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.Preserve,
            PropertyNameCaseInsensitive = true
        };
        public async Task<ResponseData<List<Category>>> GetCategoryListAsync()
        {
            var result = await _httpClient.GetAsync(_httpClient.BaseAddress);

            if (result.IsSuccessStatusCode)
            {
                var categories = await result.Content.ReadFromJsonAsync<ResponseData<IEnumerable<Category>>>(_jsonOptions);
                return new ResponseData<List<Category>>
                {
                    Data = categories.Data.ToList(),
                    Success = categories.Success,
                    ErrorMessage = categories.ErrorMessage
                };
            }

            return new ResponseData<List<Category>>
            {
                Success = false,
                ErrorMessage = "Ошибка чтения API"
            };
        }


        public async Task<ResponseData<Category>> GetCategoryByIdAsync(int id)
        {
            var result = await httpClient.GetAsync($"{httpClient.BaseAddress}{id}");

            if (result.IsSuccessStatusCode)
            {
                return new ResponseData<Category>
                {
                    Data = await result.Content.ReadFromJsonAsync<Category>(),
                    Success = true
                };
            }

            return new ResponseData<Category>
            {
                Success = false,
                ErrorMessage = $"Ошибка получения категории с ID {id}: {result.ReasonPhrase}"
            };
        }

        public async Task<ResponseData<Category>> CreateCategoryAsync(Category category)
        {
            var result = await httpClient.PostAsJsonAsync(httpClient.BaseAddress, category);

            if (result.IsSuccessStatusCode)
            {
                return new ResponseData<Category>
                {
                    Data = await result.Content.ReadFromJsonAsync<Category>(),
                    Success = true
                };
            }

            return new ResponseData<Category>
            {
                Success = false,
                ErrorMessage = $"Ошибка создания категории: {result.ReasonPhrase}"
            };
        }

        public async Task<ResponseData<bool>> UpdateCategoryAsync(int id, Category category)
        {
            var result = await httpClient.PutAsJsonAsync($"{httpClient.BaseAddress}{id}", category);

            if (result.IsSuccessStatusCode)
            {
                return new ResponseData<bool>
                {
                    Data = true,
                    Success = true
                };
            }

            return new ResponseData<bool>
            {
                Success = false,
                ErrorMessage = $"Ошибка обновления категории с ID {id}: {result.ReasonPhrase}"
            };
        }

        public async Task<ResponseData<bool>> DeleteCategoryAsync(int id)
        {
            var result = await httpClient.DeleteAsync($"{httpClient.BaseAddress}{id}");

            if (result.IsSuccessStatusCode)
            {
                return new ResponseData<bool>
                {
                    Data = true,
                    Success = true
                };
            }

            return new ResponseData<bool>
            {
                Success = false,
                ErrorMessage = $"Ошибка удаления категории с ID {id}: {result.ReasonPhrase}"
            };
        }
    }
}