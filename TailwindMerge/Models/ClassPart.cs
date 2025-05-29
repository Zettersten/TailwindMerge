namespace TailwindMerge.Models;

public sealed class ClassPart
{
    public Dictionary<string, ClassPart> NextPart { get; } = [];
    public List<ClassValidator> Validators { get; } = [];

    public string? ClassGroupId { get; private set; }

    public void SetClassGroupId(string? classGroupId = null)
    {
        this.ClassGroupId = classGroupId;
    }
}
