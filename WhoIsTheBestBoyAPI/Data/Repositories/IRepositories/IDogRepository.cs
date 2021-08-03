using Common.Models.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WhoIsTheBestBoyAPI.Data.Repositories.IRepositories
{
    public interface IDogRepository
    {
        Task<DogDTO> Create(NewDogDTO dogDTO);
        Task<int> Delete(int id);

        /// <summary>
        /// Get all approved dogs
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DogDTO>> GetApproved();

        /// <summary>
        /// Get 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DogDTO> GetApproved(int id);


        /// <summary>
        /// Get dog by Id if approved
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<DogDTO> Get(int id);

        /// <summary>
        /// Get all dogs not yet approved
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DogDTO>> GetDogsUnapproved();
        Task<int> GetNumberOfUnapprovedDogs();
        Task<IEnumerable<DogDTO>> GetUserDogs(string userId);
        Task<Tuple<int,int>> GetDogPositionInRaking(int id);
        Task<bool> ProcessMatch(int firstContenderId, int secondContenderId, int winnerId);
        Task<bool> SetApproveStatus(int id, bool status);
        Task<IEnumerable<DogDTO>> GetRandomDogs(int number);
        Task<string> GetDogOwnerId(int dogId);
        Task<IEnumerable<DogDTO>> GetTopEleven();


    }
}