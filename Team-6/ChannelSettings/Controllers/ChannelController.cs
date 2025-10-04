using AutoMapper;
using ChannelSettings.Core.Core;
using ChannelSettings.Core.IServices;
using ChannelSettings.Core.Models;
using ChannelSettings.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChannelSettings.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChannelController(IChannelService channelService, IMapper mapper,
        ILogger<ChannelController> logger) : ControllerBase
    {

        private readonly IChannelService _service = channelService;
        private readonly IMapper _mapper = mapper;
        private readonly ILogger<ChannelController> _logger = logger;

        [HttpGet("channels")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = _mapper.Map<List<ChannelDto>>(await _service.GetAllAsync(cancellationToken));
            return Ok(response);
        }

        [HttpGet("channel")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAsync([FromQuery] int id, CancellationToken cancellationToken)
        {
            var channel = await _service.GetByIdAsync(id, cancellationToken);
            return Ok(_mapper.Map<ChannelModel, ChannelDto>(channel));
        }

        [HttpPost("channel")]
        public async Task<IActionResult> CreateAsync([FromBody] CreatingChannelDto creatingChannelDto)
        {
            return Ok(await _service.CreateAsync(_mapper.Map<CreatingChannel>(creatingChannelDto)));
        }

        [HttpPut("channel")]
        public async Task<IActionResult> EditAsync([FromQuery] int id, [FromBody] UpdatingChannelDto updatingChannelDto)
        {
            await _service.UpdateAsync(id, _mapper.Map<UpdatingChannelDto, UpdatingChannel>(updatingChannelDto));
            return Ok();
        }

        [HttpDelete("channel")]
        public async Task<IActionResult> DeleteAsync([FromQuery] int id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
    }
}
