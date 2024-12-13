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
        private readonly IServiceManager _serviceManager;

        public TournamentDetailsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
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
            var tournamentDtos = await _serviceManager.TournamentService.GetTournamentsAsync(includeGames);
            return Ok(tournamentDtos);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentDto>> GetTournamentDetails(int id)
        {
            var tournamentDetails = await _serviceManager.TournamentService.GetTournamentById(id);

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

            try
            {
                var updatedTournament = await _serviceManager.TournamentService.UpdateTournamentAsync(id, tournamentDto);
                
                return Ok(updatedTournament);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (DbUpdateConcurrencyException)
            {
                 return StatusCode(500, "A concurrency error occurred.");
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
                var (Id, createdTournament) = await _serviceManager.TournamentService.CreateTournamentAsync(tournamentDto);

                return CreatedAtAction(nameof(GetTournamentDetails), new { Id }, createdTournament);
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
            try
            {
                var deletedTournament = await _serviceManager.TournamentService.DeleteTournamentAsync(id);

                return Ok(deletedTournament);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while deleting the tournament.");
            }

        }

        [HttpPatch("{tournamentId:int}")]
        public async Task<ActionResult<TournamentDto>> PatchTournament(int tournamentId, JsonPatchDocument<TournamentDto> patchDocument)
        {
            if (patchDocument == null) return BadRequest("Patch document cannot be null.");

            try
            {
                var tournament = await _serviceManager.TournamentService.GetTournamentById(tournamentId);
                if (tournament == null) return NotFound();

                patchDocument.ApplyTo(tournament, ModelState);
                TryValidateModel(tournament);

                // Validate the patched model
                if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

                var updatedTournament = await _serviceManager.TournamentService.UpdateTournamentAsync(tournamentId, tournament);
                return Ok(updatedTournament);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, "An error occurred while applying the patch.");
            }
        }

        //private async Task<bool> TournamentDetailsExists(int id)
        //{
        //    return await _unitOfWork.TournamentRepository.AnyAsync(id);
        //}
    }
}
