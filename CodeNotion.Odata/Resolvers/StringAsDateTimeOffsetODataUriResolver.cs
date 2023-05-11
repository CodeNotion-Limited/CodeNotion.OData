using System;
using System.Linq;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.UriParser;

namespace CodeNotion.Odata.Resolvers;

public class StringAsDateTimeOffsetODataUriResolver : ODataUriResolverDecorator
{
    public StringAsDateTimeOffsetODataUriResolver(ODataUriResolver decoratee) : base(decoratee) { }

    public override void PromoteBinaryOperandTypes(BinaryOperatorKind binaryOperatorKind, ref SingleValueNode leftNode, ref SingleValueNode rightNode, out IEdmTypeReference typeReference)
    {
        var @fixed = TryFixDateTimeOffsetNode(leftNode, ref rightNode);

        if (!@fixed)
        {
            TryFixDateTimeOffsetNode(rightNode, ref leftNode);
        }

        base.PromoteBinaryOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode, out typeReference);
    }

    private static bool TryFixDateTimeOffsetNode(SingleValueNode leftNode, ref SingleValueNode rightNode)
    {
        if (leftNode is SingleValuePropertyAccessNode propertyNode && propertyNode.TypeReference.IsDateTimeOffset() &&
            rightNode is ConstantNode constantNode && constantNode.Value is string rightString && DateTimeOffset.TryParse(rightString, out var rightDateTimeOffset))
        {
            rightNode = new ConstantNode(rightDateTimeOffset);
            return true;
        }

        return false;
    }
}