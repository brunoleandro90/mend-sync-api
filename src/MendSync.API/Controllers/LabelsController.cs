using MendSync.Application.DTOs.Common;
using MendSync.Application.DTOs.Labels;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.Token;
using Microsoft.AspNetCore.Mvc;

namespace MendSync.API.Controllers;

[ApiController]
[Route("api/labels")]
public class LabelsController : ControllerBase
{
    private readonly ILabelsService _service;
    private readonly TokenStore _tokenStore;

    public LabelsController(ILabelsService service, TokenStore tokenStore)
    {
        _service = service;
        _tokenStore = tokenStore;
    }

    private string OrgUuid => _tokenStore.GetOrgUuid()
        ?? throw new InvalidOperationException("Not authenticated. Please login first.");

    [HttpGet]
    public async Task<IActionResult> GetLabels()
    {
        var result = await _service.GetLabelsAsync(OrgUuid);
        return Ok(ApiResponse<IEnumerable<LabelDto>>.Ok(result));
    }

    [HttpPost]
    public async Task<IActionResult> AddLabel([FromBody] CreateLabelDto request)
    {
        var result = await _service.AddLabelAsync(OrgUuid, request);
        return Ok(ApiResponse<LabelDto>.Ok(result));
    }

    [HttpPut("{labelUuid}")]
    public async Task<IActionResult> RenameLabel(string labelUuid, [FromBody] RenameLabelDto request)
    {
        await _service.RenameLabelAsync(OrgUuid, labelUuid, request);
        return NoContent();
    }

    [HttpDelete("{labelUuid}")]
    public async Task<IActionResult> RemoveLabel(string labelUuid)
    {
        await _service.RemoveLabelAsync(OrgUuid, labelUuid);
        return NoContent();
    }
}
