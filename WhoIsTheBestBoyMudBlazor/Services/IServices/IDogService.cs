using Common.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BestBoyClient.Services.IServices
{
    interface IDogService
    {
        public Task<IEnumerable<DogDTO>> GetUnapproved();
        public Task<int> GetNumberUnapproved();
        public Task<DogDTO> CreateDog(NewDogDTO newDogDTO);
        public Task<bool> Delete(int id);
        public Task<bool> SetApprovalStatus(int id, bool status);
        public Task<IEnumerable<DogDTO>> GetRandomDogs(int number);
        public Task<IEnumerable<DogDTO>> GetUserDogs();
        public Task<bool> ProcessMatch(int idFirstContender, int idSecondContender, int winnerId);
        public Task<Tuple<int,int>> GetPositionInRanking(int dogId);
        public Task<IEnumerable<DogDTO>> GetRankingTop();
    }
}
