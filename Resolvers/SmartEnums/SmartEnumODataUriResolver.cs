using Ardalis.SmartEnum;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Linq;

namespace CodeNotion.Odata.Resolvers.SmartEnums;

internal class SmartEnumODataUriResolver<TEntity> : ODataUriResolverDecorator
{
    private readonly Type _entityType;

    public SmartEnumODataUriResolver(ODataUriResolver decoratee) : base(decoratee)
    {
        _entityType = typeof(TEntity);
    }

    public override IEdmProperty ResolveProperty(IEdmStructuredType type, string propertyName)
    {
        var property = base.ResolveProperty(type, propertyName);

        // $orderby allows primitive & enums types only
        // we can't replace ODataQueryOptionParser.orderByClause property or
        // UriParser.OrderByBinder class with custom ones beacause they are sealed
        // so the only way is pretend that SmartEnum property is primitive type
        if (property != null && property.Type.Definition.TypeKind == EdmTypeKind.Complex)
        {
            Type? propertyClrType = null;

            if (_entityType.FullName == type.FullTypeName()) // todo: check also declarated types
            {
                propertyClrType = _entityType.GetProperty(property.Name)!.PropertyType;
            }

            if (propertyClrType != null && propertyClrType.GetInterfaces().Contains(typeof(ISmartEnum)))
            {
                return new SpecialSmartEnumEdmProperty(property);
            }
        }

        return property;
    }
}
