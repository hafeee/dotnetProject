using FlowerSpot.Models;
using FlowerSpot.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using OpenQA.Selenium;
using System.Security.Claims;

namespace FlowerSpot.Controllers
{
    [ApiController]
    [Route("likes")]
    [Authorize]
    public class LikesController : Controller
    {
        private readonly ILikeService _likeService;

        public LikesController(ILikeService likeService)
        {
            _likeService = likeService;
        }

        // GET api/likes/{likeId}
        [HttpGet("{likeId}")]
        public async Task<IActionResult> GetLike(int likeId)
        {
            try
            {
                var like = await _likeService.GetLikeAsync(likeId);

                return Ok(like);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        // GET /likes/sightings/{sightingId}
        [HttpGet("sightings/{sightingId}")]
        public async Task<IActionResult> GetLikesBySightingId(int sightingId)
        {
            try
            {
                var likes = await _likeService.GetLikesBySightingIdAsync(sightingId);

                return Ok(likes);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
        }

        // POST /likes/sightings/{sightingId}
        [HttpPost("sightings/{sightingId}")]
        public async Task<IActionResult> CreateLike(int sightingId)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var like = await _likeService.CreateLikeAsync(sightingId, userId);

                return CreatedAtAction(nameof(GetLike), new { likeId = like.Id }, like);
            }
            catch (NotFoundException)
            {
                return NotFound();
            }
            catch (AlreadyExistsException)
            {
                return Conflict();
            }
        }

        // DELETE /likes/sightings/{sightingId}
        [HttpDelete("sightings/{sightingId}")]
        public async Task<IActionResult> DeleteLike(int sightingId)
        {
            try
            {
                // Get the authenticated user
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                // Get the like associated with the user and sighting
                var like = await _likeService.GetLikeAsync(sightingId, userId);

                // If like is not found, return NotFound status code
                if (like == null)
                {
                    return NotFound();
                }

                // Delete the like
                await _likeService.DeleteLikeAsync(like.Id, userId);

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

    }
}
