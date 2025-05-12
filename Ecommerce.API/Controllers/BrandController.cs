using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Domain.DTOs.BrandDTOs;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        /// <summary>
        /// Creates a new brand.
        /// </summary>
        [HttpPost("CreateBrand")]
        public async Task<IActionResult> CreateBrand([FromBody] string brandName)
        {
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!int.TryParse(userIdStr, out var userId))
                    return Unauthorized();

                var result = await _brandService.CreateBrand(brandName);

                if (result == null)
                    return BadRequest("Brand creation failed.");

                return Ok("Brand created successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Updates the name of an existing brand.
        /// </summary>
        [Authorize]
        [HttpPut("EditBrand")]
        public async Task<IActionResult> EditBrand([FromBody] EditBrandDto dto)
        {
            try
            {
                var result = await _brandService.UpdateBrandName(dto.CurrentName, dto.NewName);

                if (!result)
                    return NotFound("Brand not found or update failed.");

                return Ok("Brand name updated successfully.");
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
    }
}
