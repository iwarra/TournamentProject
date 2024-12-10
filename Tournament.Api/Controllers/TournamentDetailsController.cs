using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tournament.Data.Data;
using Tournament.Core.Entities;
using Tournament.Core.Repositories;
using AutoMapper;
using Tournament.Core.Dto;
using Microsoft.AspNetCore.JsonPatch;
using Humanizer;

namespace Tournament.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TournamentDetailsController : ControllerBase
    {
        private readonly IUoW _unitOfWork;
        private readonly IMapper _mapper;

        public TournamentDetailsController(IUoW unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //// GET: api/TurnamentDetails
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournamentDetails()
        //{
        //    var detailsDto = await _unitOfWork.TournamentRepository.GetAllAsync();
        //    return Ok(detailsDto);
        //}

        //Version with optional game inclusion with context
        // GET: api/TurnamentDetails?includeGames=false or true
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TournamentDto>>> GetTournamentDetails(bool includeGames)
        {
            var tournamentDtos = includeGames ? _mapper.Map<IEnumerable<TournamentDto>>(await _unitOfWork.TournamentRepository.GetAllAsync(true)) :
                                                _mapper.Map<IEnumerable<TournamentDto>>(await _unitOfWork.TournamentRepository.GetAllAsync());

                return Ok(tournamentDtos);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<TournamentDto>> GetTournamentDetails(int id)
        {
            var tournamentDetails = await _unitOfWork.TournamentRepository.GetAsync(id);

            if (tournamentDetails == null)
            {
                return NotFound();
            }

            var tournamentDto = _mapper.Map<TournamentDto>(tournamentDetails);

            return Ok(tournamentDto);

            //return turnamentDetails;
        }


        // PUT: api/TurnamentDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTournamentDetails(int id, TournamentDto tournamentDto)
        {
            
            if (id != tournamentDto.Id)
            {
                return BadRequest("Provided ID does not match the DTO ID.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var tournamentDetails = _mapper.Map<TournamentDetails>(tournamentDto);
                _unitOfWork.TournamentRepository.Update(tournamentDetails);
                
                await _unitOfWork.CompleteAsync();
                
                return Ok(tournamentDto);
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
                catch (Exception ex) 
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


                return CreatedAtAction(nameof(GetTournamentDetails), new { id = tournamentDto.Id }, createdDto);
            }
            catch (Exception ex)
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
            catch (Exception ex)
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

            // Retrieve the entity from the database
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
            catch (Exception ex)
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
