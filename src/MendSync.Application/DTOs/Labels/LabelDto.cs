namespace MendSync.Application.DTOs.Labels;

public class LabelDto
{
    public string Uuid { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
}

public class CreateLabelDto
{
    public string Name { get; set; } = string.Empty;
    public string? Color { get; set; }
}

public class RenameLabelDto
{
    public string Name { get; set; } = string.Empty;
}
