namespace EFCoreStringPKAndChildCollection;

public class ParentWithStringPK
{
    public string Id { get; set; }
    public List<Child> Children { get; set; }
}

public class ParentWithGuidPK
{
    public Guid Id { get; set; }
    public List<Child> Children { get; set; }
}

public record Child(Guid id);