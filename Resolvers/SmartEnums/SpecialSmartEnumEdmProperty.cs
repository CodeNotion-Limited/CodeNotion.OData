using Microsoft.OData.Edm;
using System;

namespace CodeNotion.Odata.Resolvers.SmartEnums;

internal class SpecialSmartEnumEdmProperty : EdmStructuralProperty
{
    public IEdmProperty BaseProperty { get; }

    public SpecialSmartEnumEdmProperty(IEdmProperty baseProperty)
        : base(baseProperty.DeclaringType, baseProperty.Name, new PretendingPrimitiveEdmTypeReference(baseProperty.Type))
    {
        BaseProperty = baseProperty;
    }
}
