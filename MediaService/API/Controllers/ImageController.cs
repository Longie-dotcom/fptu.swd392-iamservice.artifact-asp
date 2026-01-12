using Application.Interface;
using Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        #region Attributes
        private readonly IImageService imageService;
        #endregion

        #region Properties
        #endregion

        public ImageController(IImageService imageService)
        {
            this.imageService = imageService;
        }

        #region Methods
        [AllowAnonymous]
        [HttpPost("images")]
        public async Task<IActionResult> Upload(
            IFormFile file,
            [FromForm] string purpose,
            CancellationToken ct)
        {
            var result = await imageService.UploadAsync(
                file.OpenReadStream(),
                file.ContentType,
                Enum.Parse<ImagePurpose>(purpose, true),
                ct);

            return Ok(new { imageName = result.Value });
        }
        #endregion
    }
}
