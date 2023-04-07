using FlowerSpot.Models;
using FlowerSpot.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace FlowerSpot.Controllers
{
    [Authorize]
    [ApiController]
    [Route("flowers")]
    public class FlowersController : ControllerBase
    {
        private readonly IFlowerService _flowerService;

        public FlowersController(IFlowerService flowerService)
        {
            _flowerService = flowerService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Flower>>> GetFlowers()
        {
            var flowers = await _flowerService.GetFlowersAsync();
            return Ok(flowers);
        }

        [HttpPost]
        public async Task<ActionResult<Flower>> CreateFlower(Flower flower)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var createdFlower = await _flowerService.CreateFlowerAsync(flower);
                    return CreatedAtAction(nameof(CreateFlower), new { id = createdFlower.Id }, createdFlower);
                }
                catch (Exception ex)
                {
                    // Handle any exceptions here
                    return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create flower");
                }
            }
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFlower(int id)
        {
            try
            {
                await _flowerService.DeleteFlowerAsync(id);
                return NoContent();
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFlowerById(int id)
        {
            try
            {
                var flower = await _flowerService.GetFlowerByIdAsync(id);
                return Ok(flower);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<Flower>> PatchFlower(int id, [FromBody] JObject patchDoc)
        {
            var flowerToUpdate = await _flowerService.GetFlowerByIdAsync(id);

            if (flowerToUpdate == null)
            {
                return NotFound();
            }
            flowerToUpdate.Name = (string)patchDoc["name"] ?? flowerToUpdate.Name;
            flowerToUpdate.ImageRef = (string)patchDoc["imageRef"] ?? flowerToUpdate.ImageRef;
            flowerToUpdate.Description = (string)patchDoc["description"] ?? flowerToUpdate.Description;

            var updatedFlower = await _flowerService.UpdateFlowerAsync(flowerToUpdate);

            if (updatedFlower == null)
            {
                return BadRequest(ModelState);
            }

            return updatedFlower;
        }
    }
}