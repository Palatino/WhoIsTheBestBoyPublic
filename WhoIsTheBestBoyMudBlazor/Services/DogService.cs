using Common.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BestBoyClient.Services.IServices;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BestBoyClient.Services
{
    public class DogService : IDogService
    {
        private readonly HttpClient _authorizedClient;
        private readonly HttpClient _anonymousClient;
        public DogService(IHttpClientFactory factory)
        {
            _authorizedClient = factory.CreateClient("AuthenticatedClient");
            _anonymousClient = factory.CreateClient("AnonymousClient");
        }

        public async Task<DogDTO> CreateDog(NewDogDTO newDogDTO)
        {
            var dogDtoJson = JsonSerializer.Serialize(newDogDTO, typeof(NewDogDTO));
            var content = new StringContent(dogDtoJson, Encoding.UTF8, "application/json");
            var response = await _authorizedClient.PostAsync("/api/dogs", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var dog= JsonSerializer.Deserialize<DogDTO>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return dog;
        }
        public async Task<IEnumerable<DogDTO>> GetUnapproved()
        {
            var response = await _authorizedClient.GetAsync("/api/dogs/notapproved");
            var responseString = await response.Content.ReadAsStringAsync();
            var dogs = JsonSerializer.Deserialize<IEnumerable<DogDTO>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return dogs;
        }
        public async Task<bool> Delete(int id)
        {
            var response = await _authorizedClient.DeleteAsync($"/api/dogs/{id}");
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent) return true;
            return false;
        }
        public async Task<IEnumerable<DogDTO>> GetRandomDogs(int number)
        {
            var response = await _anonymousClient.GetAsync($"/api/dogs/random/{number}");
            var responseString = await response.Content.ReadAsStringAsync();
            var dogs = JsonSerializer.Deserialize<IEnumerable<DogDTO>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return dogs;
        }
        public async Task<bool> ProcessMatch(int idFirstContender, int idSecondContender, int winnerId)
        {
            var matchResult = new MatchResult()
            {
                FirstContenderId = idFirstContender,
                SecondContenderId = idSecondContender,
                WinnerId = winnerId
            };
            var bodyJson = JsonSerializer.Serialize(matchResult, typeof(object));
            var content = new StringContent(bodyJson, Encoding.UTF8, "application/json");
            var response = await _anonymousClient.PostAsync("/api/dogs/processmatch", content);
            var responseString = await response.Content.ReadAsStringAsync();
            var matchUpdated = JsonSerializer.Deserialize<bool>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return matchUpdated;
        }
        public async Task<bool> SetApprovalStatus(int id, bool status)
        {
            var dogStatus = new DogApproval() { Id = id, Status = status };
            var dogStatusString = JsonSerializer.Serialize(dogStatus, typeof(DogApproval));
            var content = new StringContent(dogStatusString, Encoding.UTF8, "application/json");
            var response = await _authorizedClient.PatchAsync($"/api/dogs/approvedstatus", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            return Convert.ToBoolean(responseContent);
        }
        public async Task<IEnumerable<DogDTO>> GetUserDogs()
        {
            var response = await _authorizedClient.GetAsync("/api/dogs/user");
            var responseString = await response.Content.ReadAsStringAsync();
            var dogs = JsonSerializer.Deserialize<IEnumerable<DogDTO>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return dogs;
        }
        public async Task<Tuple<int, int>> GetPositionInRanking(int dogId)
        {
            var response = await _authorizedClient.GetAsync($"/api/dogs/ranking/{dogId}");
            var responseString = await response.Content.ReadAsStringAsync();
            var rankingInfo = JsonSerializer.Deserialize<Tuple<int,int>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return rankingInfo;
        }
        public async Task<int> GetNumberUnapproved()
        {
            var response = await _authorizedClient.GetAsync("/api/dogs/notapprovednumber");
            var responseString = await response.Content.ReadAsStringAsync();
            var numberOfUnapproved = JsonSerializer.Deserialize<int>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return numberOfUnapproved;
        }
        public async Task<IEnumerable<DogDTO>> GetRankingTop()
        {
            var response = await _anonymousClient.GetAsync($"/api/dogs/top");
            var responseString = await response.Content.ReadAsStringAsync();
            var dogs = JsonSerializer.Deserialize<IEnumerable<DogDTO>>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return dogs;
        }
    }
}
