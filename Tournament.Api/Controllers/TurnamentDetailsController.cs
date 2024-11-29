using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tournament.Data.Data;
using Tournament.Core.Entities;

namespace Tournament.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurnamentDetailsController : ControllerBase
    {
        private readonly TournamentApiContext _context;

        public TurnamentDetailsController(TournamentApiContext context)
        {
            _context = context;
        }

        // GET: api/TurnamentDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TurnamentDetails>>> GetTurnamentDetails()
        {
            return await _context.TurnamentDetails.ToListAsync();
        }

        // GET: api/TurnamentDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TurnamentDetails>> GetTurnamentDetails(int id)
        {
            var turnamentDetails = await _context.TurnamentDetails.FindAsync(id);

            if (turnamentDetails == null)
            {
                return NotFound();
            }

            return turnamentDetails;
        }

        // PUT: api/TurnamentDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTurnamentDetails(int id, TurnamentDetails turnamentDetails)
        {
            if (id != turnamentDetails.Id)
            {
                return BadRequest();
            }

            _context.Entry(turnamentDetails).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TurnamentDetailsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/TurnamentDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<TurnamentDetails>> PostTurnamentDetails(TurnamentDetails turnamentDetails)
        {
            _context.TurnamentDetails.Add(turnamentDetails);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTurnamentDetails", new { id = turnamentDetails.Id }, turnamentDetails);
        }

        // DELETE: api/TurnamentDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTurnamentDetails(int id)
        {
            var turnamentDetails = await _context.TurnamentDetails.FindAsync(id);
            if (turnamentDetails == null)
            {
                return NotFound();
            }

            _context.TurnamentDetails.Remove(turnamentDetails);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TurnamentDetailsExists(int id)
        {
            return _context.TurnamentDetails.Any(e => e.Id == id);
        }
    }
}
