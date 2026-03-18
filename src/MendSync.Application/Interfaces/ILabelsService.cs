using MendSync.Application.DTOs.Labels;

namespace MendSync.Application.Interfaces;

public interface ILabelsService
{
    Task<IEnumerable<LabelDto>> GetLabelsAsync(string orgUuid);
    Task<LabelDto> AddLabelAsync(string orgUuid, CreateLabelDto request);
    Task RenameLabelAsync(string orgUuid, string labelUuid, RenameLabelDto request);
    Task RemoveLabelAsync(string orgUuid, string labelUuid);
}
