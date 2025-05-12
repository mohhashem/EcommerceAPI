using Domain.DTOs.ProductDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Security.Claims;

namespace Ecommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Adds a new product to the specified store and brand.
        /// </summary>
        [Authorize]
        [HttpPost("CreateNewProduct")]
        public async Task<IActionResult> AddProduct([FromBody] NewProductDTO dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            try
            {
                var result = await _productService.AddProduct(dto, userId);
                return result ? Ok("Product added.") : BadRequest("Could not add product.");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Soft deletes a product by name.
        /// </summary>
        [Authorize]
        [HttpPut("DeleteByName")]
        public async Task<IActionResult> SoftDeleteProductByName([FromBody] ProductIdentifierDTO dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            try
            {
                var result = await _productService.SoftDeleteProductByName(dto, userId);
                return result ? Ok("Product soft deleted.") : NotFound("Product not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Recovers a soft deleted product by name.
        /// </summary>
        [Authorize]
        [HttpPut("RecoverByName")]
        public async Task<IActionResult> RecoverProductByName([FromBody] ProductIdentifierDTO dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            try
            {
                var result = await _productService.RecoverProductByName(dto, userId);
                return result ? Ok("Product recovered.") : NotFound("Product not found.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Edits an existing product's details.
        /// </summary>
        [Authorize]
        [HttpPut("EditProduct")]
        public async Task<IActionResult> EditProduct([FromBody] EditProductDTO dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            try
            {
                var result = await _productService.EditProduct(dto, userId);
                return result ? Ok("Product updated.") : NotFound("Product not found or update failed.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        /// <summary>
        /// Retrieves all products for a given store and brand.
        /// </summary>
        [Authorize]
        [HttpGet("GetAllByStoreAndBrand")]
        public async Task<IActionResult> GetAllProductsByStoreAndBrand([FromQuery] string storeName, [FromQuery] string brandName)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized();

            try
            {
                var result = await _productService.GetAllProductsByStoreAndBrand(storeName, brandName, userId);

                if (result == null || !result.Any())
                    return NotFound("No products found.");

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
