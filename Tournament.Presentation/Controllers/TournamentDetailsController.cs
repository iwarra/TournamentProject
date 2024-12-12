using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using AutoMapper;
using Tournament.Core.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Service.Contracts;


namespace Tournament.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentDetailsController : ControllerBase
    {
        private readonly IServiceManager serviceManager;
        private readonly IUoW _unitOfWork;
        private readonly IMapper _mapper;

        public TournamentDetailsController(IUoW unitOfWork, IMapper mapper, IServiceManager serviceManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            this.serviceManager = serviceManager;
        }

        //// GET: api/TurnamentDetails
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournamentDetails()
        //{
        //    var detailsDto = await _unitOfWork.TournamentRepository.GetAllAsync();
        //    return Ok(detailsDto);
        //}


        //Version with optional game inclusion 
        // GET: api/TurnamentDetails?includeGames=false or true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournamentDetails(bool includeGames)
        {
            var tournamentDtos = await serviceManager.TournamentService.GetTournamentsAsync(includeGames);
            return Ok(tournamentDtos);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentDto>> GetTournamentDetails(int id)
        {
            var tournamentDetails = await serviceManager.TournamentService.GetTournamentById(id);

            if (tournamentDetails == null)
            {
                return NotFound();
            }

            //var tournamentDto = _mapper.Map<TournamentDto>(tournamentDetails);

            return Ok(tournamentDetails);
        }


        // PUT: api/TurnamentDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTournamentDetails(int id, TournamentDto tournamentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var existingTournament = await _unitOfWork.TournamentRepository.GetAsync(id);
            if (existingTournament == null)
            {
                return NotFound();
            }

            try
            {
                _mapper.Map(tournamentDto, existingTournament);
                _unitOfWork.TournamentRepository.Update(existingTournament);
                
                await _unitOfWork.CompleteAsync();
                
                return Ok(_mapper.Map<TournamentDto>(existingTournament));
            }
            catch (DbUpdateConcurrencyException)
            {
                bool isFound = await TournamentDetailsExists(id);
                if (!isFound)
                {
                    return NotFound();
                }
                else
                {
                    return StatusCode(500, "A concurrency error occurred.");
                }
            }
                catch (Exception) 
                {
                    return StatusCode(500, "An error occurred while updating the tournament.");
                }
        }

        // POST: api/TurnamentDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TournamentDto>> PostTournamentDetails(TournamentDto tournamentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var tournamentDetails = _mapper.Map<TournamentDetails>(tournamentDto);
                _unitOfWork.TournamentRepository.Add(tournamentDetails);
                await _unitOfWork.CompleteAsync();

                var createdDto = _mapper.Map<TournamentDto>(tournamentDetails);


                return CreatedAtAction(nameof(GetTournamentDetails), new { id = tournamentDetails.Id }, createdDto);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while saving the tournament.");
            }
        }

        // DELETE: api/TurnamentDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTurnamentDetails(int id)
        {
            var tournamentDetails = await _unitOfWork.TournamentRepository.GetAsync(id);
            if (tournamentDetails == null)
            {
                return NotFound();
            }

            try
            {
                _unitOfWork.TournamentRepository.Remove(tournamentDetails);
                await _unitOfWork.CompleteAsync();

                var deletedDto = _mapper.Map<TournamentDto>(tournamentDetails);

                //Return the deleted Dto in case we want to see which tournament was deleted
                return Ok(deletedDto); 
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while deleting the tournament.");
            }

        }

        [HttpPatch("{tournamentId:int}")]
        public async Task<ActionResult<TournamentDto>> PatchTournament(int tournamentId, JsonPatchDocument<TournamentDto> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest("Patch document cannot be null.");
            }

            var tournamentDetails = await _unitOfWork.TournamentRepository.GetAsync(tournamentId);
            if (tournamentDetails == null)
            {
                return NotFound();
            }

            var tournamentDto = _mapper.Map<TournamentDto>(tournamentDetails);

            // Apply the patch
            patchDocument.ApplyTo(tournamentDto, ModelState);
            TryValidateModel(tournamentDto);

            // Validate the patched model
            if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

            try
            {
                var updatedTournamentDetails = _mapper.Map(tournamentDto, tournamentDetails);

                _unitOfWork.TournamentRepository.Update(updatedTournamentDetails);
                await _unitOfWork.CompleteAsync();

                return Ok(tournamentDto);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while applying the patch.");
            }
        }

        private async Task<bool> TournamentDetailsExists(int id)
        {
            return await _unitOfWork.TournamentRepository.AnyAsync(id);
        }
    }
}
