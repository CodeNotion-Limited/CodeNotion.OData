using Ardalis.SmartEnum;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Linq;
using System.Reflection;

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
                return new SpecialSmartEnumEdmProperty(property/*, propertyClrType*/);
            }
        }

        return property;
    }

    //public override void PromoteBinaryOperandTypes(BinaryOperatorKind binaryOperatorKind, ref SingleValueNode leftNode, ref SingleValueNode rightNode, out IEdmTypeReference typeReference)
    //{
    //    TryFixNodes(ref leftNode, ref rightNode);
    //    TryFixNodes(ref rightNode, ref leftNode);

    //    base.PromoteBinaryOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode, out typeReference);
    //}

    //private bool TryFixNodes(ref SingleValueNode leftNode, ref SingleValueNode rightNode)
    //{
    //    if (leftNode is not SingleValuePropertyAccessNode propertyNode)
    //    {
    //        return false;
    //    }

    //    if (propertyNode.Property is not SpecialSmartEnumEdmProperty smartEnumProperty)
    //    {
    //        return false;
    //    }

    //    if (rightNode is not ConstantNode constantNode)
    //    {
    //        return false;
    //    }

    //    if (constantNode.Value is null)
    //    {
    //        return false;
    //    }

    //    if (constantNode.Value is string smartEnumMemberName)
    //    {
    //        var fromNameMethod = smartEnumProperty.ClrType.GetMethod("FromName", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy, new[] { typeof(string), typeof(bool) })!;
    //        var smartEnumMember = fromNameMethod.Invoke(null, new object[] { smartEnumMemberName, EnableCaseInsensitive });
    //        rightNode = new ConstantNode(smartEnumMember);
    //        leftNode = new SingleValuePropertyAccessNode(propertyNode.Source, smartEnumProperty.BaseProperty);
    //    }

    //    return true;
    //}
}
