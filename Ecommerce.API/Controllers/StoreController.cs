using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Domain.DTOs.StoreDTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoreController : ControllerBase
    {
        private readonly IStoreService _storeService;

        public StoreController(IStoreService storeService)
        {
            _storeService = storeService;
        }

        /// <summary>
        /// Creates a new store for the authenticated user.
        /// </summary>
        [Authorize]
        [HttpPost("CreateStore")]
        public async Task<IActionResult> CreateStore([FromBody] NewStoreDto store)
        {
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(userIdStr, out var userId))
                    return Unauthorized();

                var result = await _storeService.CreateStore(store, userId);

                if (!result)
                    return BadRequest("Store creation failed.");

                return Ok("Store created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Soft deletes (deactivates) a store by name for the authenticated user.
        /// </summary>
        [Authorize]
        [HttpDelete("DeleteByName/{storeName}")]
        public async Task<IActionResult> DeleteStoreByName(string storeName)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var result = await _storeService.DeleteStoreByName(storeName, userId);
            if (!result)
                return NotFound("Store not found or not owned by you.");

            return Ok("Store deleted (deactivated) successfully.");
        }

        /// <summary>
        /// Updates store details for the authenticated user.
        /// </summary>
        [Authorize]
        [HttpPut("UpdateStore")]
        public async Task<IActionResult> UpdateStore([FromBody] UpdateStoreDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            try
            {
                var result = await _storeService.UpdateStore(dto, userId);
                if (!result)
                    return NotFound("Store not found or not owned by you.");

                return Ok("Store updated successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Unassigns a brand from a store for the authenticated user.
        /// </summary>
        [Authorize]
        [HttpPut("UnassignBrand/{storeName}")]
        public async Task<IActionResult> UnassignBrand(string storeName)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            var result = await _storeService.UnassignBrand(storeName, userId);

            if (!result)
                return NotFound("Store not found or not owned by you.");

            return Ok("Brand unassigned from store successfully.");
        }
    }
}
