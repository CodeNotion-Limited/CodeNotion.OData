using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace CodeNotion.Odata.Resolvers;

public class IntAsEnumODataUriResolver : ODataUriResolverDecorator
{
    public IntAsEnumODataUriResolver(ODataUriResolver decoratee) : base(decoratee) { }

    public override void PromoteBinaryOperandTypes(BinaryOperatorKind binaryOperatorKind, ref SingleValueNode leftNode, ref SingleValueNode rightNode, out IEdmTypeReference typeReference)
    {
        var leftEnumType = GetEnumType(leftNode, out var isLeftNodeNullable);
        if (leftEnumType != null)
        {
            TryChangeValue(leftEnumType, isLeftNodeNullable, ref rightNode);
        }

        var rightNodeEnumType = GetEnumType(rightNode, out var isRightNodeNullable);
        if (rightNodeEnumType != null)
        {
            TryChangeValue(rightNodeEnumType, isRightNodeNullable, ref leftNode);
        }

        base.PromoteBinaryOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode, out typeReference);
    }

    private static EdmEnumType? GetEnumType(SingleValueNode node, out bool isNullable)
    {
        if (node is not SingleValuePropertyAccessNode propertyAccessNode)
        {
            isNullable = false;
            return null;
        }

        var propertyType = propertyAccessNode.Property.Type;
        isNullable = propertyType.IsNullable;

        return propertyType.Definition as EdmEnumType;
    }

    private static void TryChangeValue(IEdmEnumType enumType, bool isNullable, ref SingleValueNode valueNode)
    {
        if (valueNode is not ConstantNode constantNode)
        {
            return;
        }

        if (constantNode.Value is string || constantNode.Value == null && isNullable)
        {
            return;
        }

        var value = constantNode.LiteralText;
        if (isNullable && value == "null")
        {
            return;
        }

        var intValue = (int) constantNode.Value!;
        var enumName = enumType.Members.SingleOrDefault(e => e.Value.Value == intValue)?.Name;
        if (!string.IsNullOrWhiteSpace(enumName))
        {
            valueNode = new ConstantNode(enumName, enumName);
        }
    }
}