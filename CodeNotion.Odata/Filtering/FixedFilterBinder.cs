using CodeNotion.Odata.Extensions;
using Microsoft.AspNetCore.OData.Query.Expressions;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;
using System;
using System.Linq.Expressions;

namespace CodeNotion.Odata.Filtering;

internal class FixedFilterBinder : FilterBinder
{
    public override Expression BindBinaryOperatorNode(BinaryOperatorNode binary, QueryBinderContext context)
    {
        var operationKind = binary.OperatorKind;
        var left = binary.Left;
        var right = binary.Right;

        if (TryFixBinaryOperatorNode(operationKind, left, right, context, out var fixedBinary) ||
            TryFixBinaryOperatorNode(operationKind, right, left, context, out fixedBinary))
        {
            return base.BindBinaryOperatorNode(fixedBinary, context);
        }

        return base.BindBinaryOperatorNode(binary, context);
    }

    public override Expression BindSingleValueNode(SingleValueNode node, QueryBinderContext context)
    {
        if (node is ConvertNode convertNode && convertNode.Source is SpecialEdmDateNode edmDateNode)
        {
            return Expression.Constant(edmDateNode.EdmDate.ToDateOnly());
        }

        return base.BindSingleValueNode(node, context);
    }

    private bool TryFixBinaryOperatorNode(BinaryOperatorKind operatorKind, SingleValueNode left,
        SingleValueNode right, QueryBinderContext context, out BinaryOperatorNode fixedBinary)
        => TryFixDateOnlyComparingWithEdmDate(operatorKind, left, right, context, out fixedBinary) ||
           TryFixNullableDateOnlyComparingWithNullableEdmDate(operatorKind, left, right, context, out fixedBinary);

    private bool TryFixDateOnlyComparingWithEdmDate(BinaryOperatorKind operatorKind, SingleValueNode left, SingleValueNode right, QueryBinderContext context, out BinaryOperatorNode fixedBinary)
    {
        if (left is SingleValuePropertyAccessNode { Source: not SingleNavigationNode } leftPropertyNode &&
            context.ElementClrType.GetProperty(leftPropertyNode.Property.Name)!.PropertyType == typeof(DateOnly) &&
            right is ConstantNode rightConstantNode && rightConstantNode.Value is Date)
        {
            // replacing ConstantNode (Edm.Date) with SpecialEdmDateNode to handling it in BindSingleValueNode method
            var edmDateNode = new SpecialEdmDateNode(originalNode: rightConstantNode);
            var fixedRight = new ConvertNode(edmDateNode, rightConstantNode.TypeReference);
            fixedBinary = new BinaryOperatorNode(operatorKind, left, fixedRight);

            return true;
        }

        fixedBinary = null!;

        return false;
    }

    private bool TryFixNullableDateOnlyComparingWithNullableEdmDate(BinaryOperatorKind operatorKind, SingleValueNode left, SingleValueNode right, QueryBinderContext context, out BinaryOperatorNode fixedBinary)
    {
        if (left is SingleValuePropertyAccessNode { Source: not SingleNavigationNode } leftPropertyNode &&
            context.ElementClrType.GetProperty(leftPropertyNode.Property.Name)!.PropertyType == typeof(DateOnly?) &&
            right is ConvertNode rightConvertNode && right.TypeReference.Definition.FullTypeName() == "Edm.Date" &&
            rightConvertNode.Source is ConstantNode rightConstantNode && rightConstantNode.Value is Date)
        {
            // replacing ConstantNode (Edm.Date) with SpecialEdmDateNode to handling it in BindSingleValueNode method
            var edmDateNode = new SpecialEdmDateNode(originalNode: rightConstantNode);
            var fixedRight = new ConvertNode(edmDateNode, rightConvertNode.TypeReference);
            fixedBinary = new BinaryOperatorNode(operatorKind, left, fixedRight);

            return true;
        }

        fixedBinary = null!;

        return false;
    }
}