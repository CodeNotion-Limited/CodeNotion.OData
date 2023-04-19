using Microsoft.OData.Edm;

namespace CodeNotion.Odata.Resolvers.SmartEnums;

internal class PretendingPrimitiveEdmTypeReference : IEdmTypeReference
{
    public bool IsNullable { get; }
    public IEdmType Definition { get; }
    public IEdmTypeReference BaseTypeReference { get; }

    public PretendingPrimitiveEdmTypeReference(IEdmTypeReference baseTypeReference)
    {
        BaseTypeReference = baseTypeReference;
        IsNullable = baseTypeReference.IsNullable;
        Definition = new CustomEdmType(EdmTypeKind.Primitive);
    }
}
