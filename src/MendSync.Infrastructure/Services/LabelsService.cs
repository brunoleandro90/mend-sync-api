using MendSync.Application.DTOs.Labels;
using MendSync.Application.Interfaces;
using MendSync.Infrastructure.HttpClients;
using Microsoft.Extensions.Logging;

namespace MendSync.Infrastructure.Services;

public class LabelsService : ILabelsService
{
    private readonly MendApiClient _client;
    private readonly ILogger<LabelsService> _logger;

    public LabelsService(MendApiClient client, ILogger<LabelsService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<IEnumerable<LabelDto>> GetLabelsAsync(string orgUuid)
    {
        return await _client.GetAsync<IEnumerable<LabelDto>>($"/api/v3.0/orgs/{orgUuid}/labels") ?? [];
    }

    public async Task<LabelDto> AddLabelAsync(string orgUuid, CreateLabelDto request)
    {
        return await _client.PostAsync<CreateLabelDto, LabelDto>($"/api/v3.0/orgs/{orgUuid}/labels", request)
            ?? new LabelDto();
    }

    public async Task RenameLabelAsync(string orgUuid, string labelUuid, RenameLabelDto request)
    {
        await _client.PutRawAsync($"/api/v3.0/orgs/{orgUuid}/labels/{labelUuid}", request);
    }

    public async Task RemoveLabelAsync(string orgUuid, string labelUuid)
    {
        await _client.DeleteAsync($"/api/v3.0/orgs/{orgUuid}/labels/{labelUuid}");
    }
}
