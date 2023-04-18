using Microsoft.OData.Edm;

namespace CodeNotion.Odata.Resolvers.SmartEnums;

internal class CustomEdmType : IEdmType
{
    public EdmTypeKind TypeKind { get; }

    public CustomEdmType(EdmTypeKind typeKind)
    {
        TypeKind = typeKind;
    }
}
