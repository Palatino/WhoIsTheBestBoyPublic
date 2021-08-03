using Common.Models;
using Common.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using WhoIsTheBestBoyAPI.Authorization;
using WhoIsTheBestBoyAPI.Data.Repositories.IRepositories;
using WhoIsTheBestBoyAPI.Services.IServices;

namespace WhoIsTheBestBoyAPI.Controllers
{
    [Produces("application/json")]
    [ApiController]
    public class DogController : ControllerBase
    {
        private readonly IDogRepository _dogRepo;

        public DogController(IDogRepository dogRepo)
        {
            _dogRepo = dogRepo;

        }


        /// <summary>
        /// Retrieve dog by ID
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Returns dog with the matching Id</response>
        /// <response code="400">If the id is not valid</response>  
        /// <response code="404">If no entries with provided id</response>  
        [HttpGet]
        [Route("/api/dogs/{id:int}")]
        [ProducesResponseType(typeof(DogDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Get(int? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return BadRequest(new ErrorModel()
                    {
                        Title = "Error",
                        ErrorMessage = "Provide valid id",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                    });
                }
                var dogInDB = await _dogRepo.GetApproved(id.Value);

                if (dogInDB is null)
                {
                    return NotFound(new ErrorModel()
                    {
                        Title = "Error",
                        ErrorMessage = $"Dog with id {id.Value} not found",
                        StatusCode = (int)HttpStatusCode.NotFound,
                    });
                }

                return Ok(dogInDB);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorModel()
                {
                    ErrorMessage = $"Internal server error: {e.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Title = "Error"

                });
            }

        }

        /// <summary>
        /// Create new dog
        /// </summary>
        /// <response code="200">Create new dog</response>
        /// <response code="400">If invalid request</response>
        [HttpPost]
        [Authorize]
        [Route("/api/dogs")]
        [ProducesResponseType(typeof(DogDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create( [FromBody]NewDogDTO newDogDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new ErrorModel()
                    {
                        ErrorMessage = $"Provide valid dog object",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Title = "Error"

                    });
                }

                //Add owner id

                newDogDTO.CreatedById = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                var createdDog = await _dogRepo.Create(newDogDTO);
                return Created(createdDog.Id.ToString(), createdDog);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorModel()
                {
                    ErrorMessage = $"Internal server error: {e.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Title = "Error"

                });
            }

        }

        /// <summary>
        /// Delete existing dog
        /// </summary>
        /// <response code="204">Dog deleted</response>
        /// <response code="400">If invalid request</response>
        /// <response code="404">If dog not found request</response>
        [HttpDelete]
        [Authorize]
        [Route("/api/dogs/{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new ErrorModel()
                    {
                        ErrorMessage = $"Provide valid id",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Title = "Error"

                    });
                }

                //Check if user is the owner of the dog or if it is and admin
                var dogOwnerId = await _dogRepo.GetDogOwnerId(id.Value);
                if(!(User.HasClaim("permissions", Policy.DeleteAny) 
                    || dogOwnerId == this.User.FindFirst(ClaimTypes.NameIdentifier).Value))
                {
                    return StatusCode((int)HttpStatusCode.Forbidden, new ErrorModel()
                    {
                        ErrorMessage = $"No rights to delete this dog",
                        StatusCode = (int)HttpStatusCode.Forbidden,
                        Title = "Error"

                    });
                }

                var numberOfOperations = await _dogRepo.Delete(id.Value);
                if (numberOfOperations == 0)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, new ErrorModel()
                    {
                        ErrorMessage = $"No entry with that id found",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Title = "Error"

                    });
                }
                return NoContent();
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorModel()
                {
                    ErrorMessage = $"Internal server error: {e.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Title = "Error"

                });
            }

        }

        /// <summary>
        /// Retrieve all dogs not yet approved
        /// </summary>
        /// <response code="200">Returns all dogs awaiting approval</response>
        [HttpGet]
        [Authorize(Policy = Authorization.Policy.DogAprove)]
        [Route("/api/dogs/notapproved")]
        [ProducesResponseType(typeof(IEnumerable<DogDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDogsAwaitingApproval()
        {
            try
            {
                var notApprovedDogs = await _dogRepo.GetDogsUnapproved();
                return Ok(notApprovedDogs);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorModel()
                {
                    ErrorMessage = $"Internal server error: {e.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Title = "Error"

                });
            }

        }

        /// <summary>
        /// Retrieve all dogs not yet approved
        /// </summary>
        /// <response code="200">Returns all dogs awaiting approval</response>
        [HttpGet]
        [Authorize(Policy = Authorization.Policy.DogAprove)]
        [Route("/api/dogs/notapprovednumber")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNumberOfDogsAwaitingApproval()
        {
            try
            {
                var numberOfUnapprovedDogs = await _dogRepo.GetNumberOfUnapprovedDogs();
                return Ok(numberOfUnapprovedDogs);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorModel()
                {
                    ErrorMessage = $"Internal server error: {e.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Title = "Error"

                });
            }

        }

        /// <summary>
        /// Change the approved value of dog
        /// </summary>
        /// <param name="dogApproval"></param>
        /// <returns></returns>
        /// <response code="200">Returns true if operation successful</response>
        /// <response code="400">If the id or boolean are not valid</response>  
        /// <response code="404">If no entries with provided id</response>  
        [HttpPatch]
        [Authorize(Policy = Authorization.Policy.DogAprove)]
        [Route("/api/dogs/approvedstatus")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetApprovalStatus([FromBody]DogApproval dogApproval)
        {
            try
            {
                if (dogApproval is null)
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, new ErrorModel()
                    {
                        ErrorMessage = $"Provide valid id and status",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                        Title = "Error"

                    });
                }

                if( (await _dogRepo.Get(dogApproval.Id)) is null)
                {
                    return StatusCode((int)HttpStatusCode.NotFound, new ErrorModel()
                    {
                        ErrorMessage = $"No dog with provided Id found",
                        StatusCode = (int)HttpStatusCode.NotFound,
                        Title = "Error"

                    });
                }


                var statusSet = await _dogRepo.SetApproveStatus(dogApproval.Id, dogApproval.Status);
                return Ok(statusSet);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorModel()
                {
                    ErrorMessage = $"Internal server error: {e.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Title = "Error"

                });
            }

        }

        /// <summary>
        /// Gets the position in the ranking of a given dog
        /// </summary>
        /// <param name="id"></param>
        /// <response code="200">Returns dog with the matching Id</response>
        /// <response code="400">If the id is not valid</response>  
        /// <response code="404">If no entries with provided id</response>  
        [HttpGet]
        [Authorize]
        [Route("/api/dogs/ranking/{id:int}")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPositionInRaking(int? id)
        {
            try
            {
                if (!id.HasValue)
                {
                    return BadRequest(new ErrorModel()
                    {
                        Title = "Error",
                        ErrorMessage = "Provide valid id",
                        StatusCode = (int)HttpStatusCode.BadRequest,
                    });
                }
                var dogInDB = await _dogRepo.GetApproved(id.Value);

                if (dogInDB is null)
                {
                    return NotFound(new ErrorModel()
                    {
                        Title = "Error",
                        ErrorMessage = $"Dog with id {id.Value} not found",
                        StatusCode = (int)HttpStatusCode.NotFound,
                    });
                }

                Tuple<int,int> positionInRanking = await _dogRepo.GetDogPositionInRaking(id.Value);
                return Ok(positionInRanking);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorModel()
                {
                    ErrorMessage = $"Internal server error: {e.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Title = "Error"

                });
            }

        }


        /// <summary>
        /// Process a given match
        /// </summary>
        /// <param name="idFirstContender"></param>
        /// <param name="idSecondContender"></param>
        /// <param name="firstContendertWon"></param>
        /// <response code="200">Returns true if match was processed succesfully/response>
        /// <response code="400">If worng input</response>  
        /// <response code="404">If at least on of the contenders is not found</response>  
        [HttpPost]
        [Route("/api/dogs/processmatch")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ProcessMatch([FromBody] MatchResult result)
        {

            try
            {
                var firstDog = await _dogRepo.GetApproved(result.FirstContenderId);
                var secondDog = await _dogRepo.GetApproved(result.SecondContenderId);

                if((firstDog is null) || (secondDog is null))
                {
                    return NotFound(new ErrorModel()
                    {
                        Title = "Error",
                        ErrorMessage = "At least one of the contenders does not exist",
                        StatusCode = (int)HttpStatusCode.NotFound,
                    });
                }

                bool processedOk = await _dogRepo.ProcessMatch(result.FirstContenderId, result.SecondContenderId, result.WinnerId);
                return Ok(processedOk);
            }
            catch (Exception e)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorModel()
                {
                    ErrorMessage = $"Internal server error: {e.Message}",
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Title = "Error"
                });
            }
        }

        [HttpGet]
        [Route("/api/dogs/random/{number:int}")]
        [ProducesResponseType(typeof(IEnumerable<DogDTO>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRandomDogs(int? number)
        {
            if (!number.HasValue) { number = 1; }
            var randomDogs = await _dogRepo.GetRandomDogs(number.Value);
            return Ok(randomDogs);
        }

        [Authorize]
        [HttpGet]
        [Route("/api/dogs/user")]
        [ProducesResponseType(typeof(IEnumerator<DogDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserDogs()
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            var userDogs = await _dogRepo.GetUserDogs(userId);
            return Ok(userDogs);
        }

        [HttpGet]
        [Route("/api/dogs/top")]
        [ProducesResponseType(typeof(IEnumerator<DogDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorModel), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopTen()
        {
            var topTen = await _dogRepo.GetTopEleven();
            return Ok(topTen);
        }
    }
}
