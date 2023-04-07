using FlowerSpot.Models;
using FlowerSpot.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Security.Claims;

namespace FlowerSpot.Controllers
{
    [Authorize]
    [ApiController]
    [Route("sightings")]
    public class SightingController : Controller
    {
        private readonly ISightingService _sightingService;

        public SightingController(ISightingService sightingService)
        {
            _sightingService = sightingService;
        }

        // GET api/sighting
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sighting>>> GetAllSightings()
        {
            var sightings = await _sightingService.GetAllSightingsAsync();
            return Ok(sightings);
        }

        // GET api/sighting/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sighting>> GetSighting(int id)
        {
            var sighting = await _sightingService.GetSightingAsync(id);

            if (sighting == null)
            {
                return NotFound();
            }

            return Ok(sighting);
        }

        // POST api/sighting
        [HttpPost]
        public async Task<ActionResult<Sighting>> CreateSighting(Sighting sighting)
        {
            var createdSighting = await _sightingService.CreateSightingAsync(sighting);
            return CreatedAtAction(nameof(GetSighting), new { id = createdSighting.Id }, createdSighting);
        }

        // PATCH api/sighting/5
        /*[HttpPatch("{id}")]
        public async Task<IActionResult> UpdateSighting(int id, [FromBody] JsonPatchDocument<Sighting> patchDoc)
        {
            var sighting = await _sightingService.GetSightingAsync(id);
            if (sighting == null)
            {
                return NotFound();
            }

            patchDoc.ApplyTo(sighting);

            if (!TryValidateModel(sighting))
            {
                return BadRequest(ModelState);
            }

            await _sightingService.UpdateSightingAsync(sighting);

            return NoContent();
        }*/

        [HttpPatch("{user_id}/{sighting_id}")]
        public async Task<IActionResult> UpdateSighting(int user_id, int sighting_id, [FromBody] JObject json)
        {
            // Get the sighting to be updated
            var sighting = await _sightingService.GetSightingAsync(sighting_id);
            if (sighting == null)
            {
                return NotFound();
            }

            // Check if the user is the creator of the sighting
            if (sighting.User.Id != user_id)
            {
                return Unauthorized();
            }

            // Apply the patch to the sighting object, it does not make sense to change location of sighting (if it is immediate), but if they set it manually, and they want to update location, this can be handy, so we will leave it
            sighting.Longitude = (double?)json["longitude"] ?? sighting.Longitude;
            sighting.Latitude = (double?)json["latitude"] ?? sighting.Latitude;
            sighting.Flower = json["flower"]?.ToObject<Flower>() ?? sighting.Flower;


            // Validate the updated sighting object, this can be added additionally
            if (!TryValidateModel(sighting))
            {
                return BadRequest(ModelState);
            }

            // Update the sighting in the database
            await _sightingService.UpdateSightingAsync(sighting);

            return Ok(sighting);
        }

        [HttpDelete("{SightingId}/{UserId}")]
        public async Task<IActionResult> DeleteSighting(int SightingId, int UserId)
        {
            try
            {
                await _sightingService.DeleteSightingAsync(SightingId, UserId);
                return Ok();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }


    }
}
