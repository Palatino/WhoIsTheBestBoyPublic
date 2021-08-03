using AutoMapper;
using Common.Extensions;
using Common.Models.BussinesModels;
using Common.Models.DTO;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WhoIsTheBestBoyAPI.Data.Repositories.IRepositories;
using WhoIsTheBestBoyAPI.Services.IServices;

namespace WhoIsTheBestBoyAPI.Data.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly BestBoyDbContext _context;
        private readonly IMapper _mapper;
        private readonly IDogScoreCalculator _calculator;
        private readonly IBlobService _blobService;
        private string containerName = Environment.GetEnvironmentVariable("BLOB_CONTAINER_NAME");

        public DogRepository(IMapper mapper, BestBoyDbContext contex, IDogScoreCalculator calculator, IBlobService blobService)
        {
            _mapper = mapper;
            _context = contex;
            _blobService = blobService;
            _calculator = calculator;
        }
        public async Task<DogDTO> Create(NewDogDTO dogDTO)
        {
            var blobName = await SaveAvatarAsync(dogDTO.Avatar);
            Dog dog = new Dog();
            dog.Name = dogDTO.Name;
            dog.AvatarName = blobName;
            dog.CreatedById = dogDTO.CreatedById;
            var newDog = _context.Dogs.Add(dog);
            await _context.SaveChangesAsync();

            var newDogDTO = _mapper.Map<Dog, DogDTO>(newDog.Entity);
            return newDogDTO;
        }
        public async Task<IEnumerable<DogDTO>> GetApproved()
        {
            var dogs = await _context.Dogs.Where(d=>d.Approved).ToListAsync();
            var dogDTOs = _mapper.Map<IEnumerable<Dog>, IEnumerable<DogDTO>>(dogs);
            return dogDTOs;
        }
        public async Task<IEnumerable<DogDTO>> GetDogsUnapproved()
        {
            var dogsNotApproved = await _context.Dogs.Where(d => !d.Approved).ToListAsync();
            var dogsNotApprovedDtos = _mapper.Map<IEnumerable<Dog>, IEnumerable<DogDTO>>(dogsNotApproved);

            return dogsNotApprovedDtos;
        }
        public async Task<int> GetNumberOfUnapprovedDogs()
        {
            var number = await _context.Dogs.Where(d => !d.Approved).CountAsync();
            return number;
        }
        public async Task<DogDTO> GetApproved(int id)
        {
            var dogInDB = await _context.Dogs.Where(d=>d.Approved && d.Id==id).FirstOrDefaultAsync();
            if (dogInDB is null) return null;
            var dogDTO = _mapper.Map<Dog, DogDTO>(dogInDB);
            return dogDTO;
        }
        public async Task<DogDTO> Get(int id)
        {
            var dogInDB = await _context.Dogs.FirstOrDefaultAsync();
            if (dogInDB is null) return null;
            var dogDTO = _mapper.Map<Dog, DogDTO>(dogInDB);
            return dogDTO;
        }
        public async Task<int> Delete(int id)
        {
            var dogInDB = await _context.Dogs.FindAsync(id);
            if (dogInDB is null) return 0;

            await _blobService.DeleteBlobAsync(
                dogInDB.AvatarName, 
                Environment.GetEnvironmentVariable("BLOB_CONTAINER_NAME"));

            _context.Dogs.Remove(dogInDB);
            var changes = await _context.SaveChangesAsync();
            return changes;
        }
        public async Task<bool> SetApproveStatus(int id, bool status)
        {
            var dogInDB = await _context.Dogs.FindAsync(id);
            if (dogInDB is null) return false;

            dogInDB.Approved = status;
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> ProcessMatch(int firstContenderId, int secondContenderId, int winnerId)
        {
            var firstContender = await _context.Dogs.FindAsync(firstContenderId);
            var secondContender = await _context.Dogs.FindAsync(secondContenderId);
            bool firstContenderWon = firstContenderId == winnerId;

            if (firstContender is null || secondContender is null)
            {
                return false;
            }

            var newScores = _calculator.CalculateMatchResult(firstContender.Score, secondContender.Score, firstContenderWon);
            firstContender.Score = newScores[0];
            firstContender.NumberOfMatches++;

            secondContender.Score = newScores[1];
            secondContender.NumberOfMatches++;

            if (firstContenderWon)
            {
                firstContender.NumberOfWins++;
            }
            else
            {
                secondContender.NumberOfWins++;
            }


            await _context.SaveChangesAsync();
            return true;

        }
        public async Task<Tuple<int,int>> GetDogPositionInRaking(int id)
        {
            var dog = await _context.Dogs.FindAsync(id);
            if (dog is null) return new Tuple<int, int>(-1,-1);

            var ranking = await _context.Dogs.Where(d => d.Approved).OrderByDescending(d => d.Score).ToListAsync();
            var totalNumberOfDogs = await _context.Dogs.Where(d => d.Approved).CountAsync();
            return new Tuple<int, int>(1 + ranking.IndexOf(dog),totalNumberOfDogs);
        }
        public async Task<IEnumerable<DogDTO>> GetRandomDogs(int number)
        {
            var rand = new Random();
            List<Dog> randomDogs = new List<Dog>();
            var allApprovedDogs = await _context.Dogs.Where(d => d.Approved).ToListAsync();

            //If total number of dogs less than requested return all of them
            if (allApprovedDogs.Count < number)
            {
                allApprovedDogs = allApprovedDogs.ToList().CreateShuffled();
                return _mapper.Map<IEnumerable<Dog>, IEnumerable<DogDTO>>(allApprovedDogs);
            }
            while (randomDogs.Count < number)
            {
                var selectedDog = allApprovedDogs[rand.Next(allApprovedDogs.Count())];
                randomDogs.Add(selectedDog);
                allApprovedDogs.Remove(selectedDog);

                if (allApprovedDogs.Count() == 0) { break; }

            }
            return _mapper.Map<IEnumerable<Dog>, IEnumerable<DogDTO>>(randomDogs);
        }
        private async Task<string> SaveAvatarAsync(byte[] Avatar)
        {
            string blobName = Guid.NewGuid().ToString();
            var blobNameWithExtension = await _blobService.UploadContentBlobAsync(Avatar,blobName, containerName);
            if (string.IsNullOrEmpty(blobNameWithExtension)) return null;
            return blobNameWithExtension;
        }
        public async Task<IEnumerable<DogDTO>> GetUserDogs(string userId)
        {
            var userDogs = await _context.Dogs.Where(d => d.CreatedById == userId).ToListAsync();
            return _mapper.Map<IEnumerable<Dog>, IEnumerable<DogDTO>>(userDogs);
        }
        public async Task<string> GetDogOwnerId(int dogId)
        {
            var dog = await _context.Dogs.FindAsync(dogId);
            if (dog is null) return "";
            return dog.CreatedById;
        }
        public async Task<IEnumerable<DogDTO>> GetTopEleven()
        {
            List<Dog> topEleven;
            if(await _context.Dogs.Where(d=>d.Approved).CountAsync() < 11)
            {
                topEleven = await _context.Dogs.Where(d => d.Approved).OrderByDescending(d => d.Score).ToListAsync();
            }
            else
            {
                topEleven = await _context.Dogs.Where(d => d.Approved).OrderByDescending(d => d.Score).Take(11).ToListAsync();
            }

            return _mapper.Map<IEnumerable<Dog>, IEnumerable<DogDTO>>(topEleven);
        }
    }
}
