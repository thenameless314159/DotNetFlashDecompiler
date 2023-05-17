namespace FlazzySpan.ABC.AVM2.Instructions.Containers;

public interface IPropertyContainer
{
    int PropertyNameIndex { get; set; }
    ASMultiname PropertyName { get; }
}
