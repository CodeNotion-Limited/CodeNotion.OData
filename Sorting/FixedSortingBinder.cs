using System.Dynamic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.OData.Query.Expressions;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace CodeNotion.Odata.Sorting;

internal class FixedSortingBinder : OrderByBinder
{
    public override OrderByBinderResult BindOrderBy(OrderByClause orderByClause, QueryBinderContext context)
    {
        // TODO this method is not called if I use a SmartEnum because it fails in ODataService.cs line 48
        var type = orderByClause.Expression.GetType();
        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(SmartEnumIntNode<>))
        {
            return base.BindOrderBy(orderByClause, context);
        }

        // TODO check if structuredType is correct type
        var structuredType = context.Model.FindDeclaredType(context.ElementClrType.FullName) as IEdmStructuredType;
        var orderByProperty = structuredType.FindProperty("Value"); // TODO nameof

        var parameterExpression = Expression.Parameter(context.ElementClrType, "x");
        var propertyExpression = Expression.Property(parameterExpression, orderByProperty.Name);
        var orderByExpression = Expression.Lambda(propertyExpression, parameterExpression);

        return new OrderByBinderResult(orderByExpression, orderByClause.Direction);
    }
    
    // only for debugging purposes
    public override Expression Bind(QueryNode node, QueryBinderContext context) {
        return base.Bind(node, context);
    }
    public override Expression BindCollectionNode(CollectionNode node, QueryBinderContext context) {
        return base.BindCollectionNode(node, context);
    }
    public override Expression BindSingleValueNode(SingleValueNode node, QueryBinderContext context) {
        return base.BindSingleValueNode(node, context);
    }
    public override Expression BindNavigationPropertyNode(QueryNode sourceNode, IEdmNavigationProperty navigationProperty, string propertyPath, QueryBinderContext context) {
        return base.BindNavigationPropertyNode(sourceNode, navigationProperty, propertyPath, context);
    }
    public override Expression BindCollectionResourceCastNode(CollectionResourceCastNode node, QueryBinderContext context) {
        return base.BindCollectionResourceCastNode(node, context);
    }
    public override Expression BindCollectionComplexNode(CollectionComplexNode collectionComplexNode, QueryBinderContext context) {
        return base.BindCollectionComplexNode(collectionComplexNode, context);
    }
    public override Expression BindCollectionPropertyAccessNode(CollectionPropertyAccessNode propertyAccessNode, QueryBinderContext context) {
        return base.BindCollectionPropertyAccessNode(propertyAccessNode, context);
    }
    public override Expression BindDynamicPropertyAccessQueryNode(SingleValueOpenPropertyAccessNode openNode, QueryBinderContext context) {
        return base.BindDynamicPropertyAccessQueryNode(openNode, context);
    }
    public override Expression BindPropertyAccessQueryNode(SingleValuePropertyAccessNode propertyAccessNode, QueryBinderContext context) {
        return base.BindPropertyAccessQueryNode(propertyAccessNode, context);
    }
    public override Expression BindSingleComplexNode(SingleComplexNode singleComplexNode, QueryBinderContext context) {
        return base.BindSingleComplexNode(singleComplexNode, context);
    }
    public override Expression BindRangeVariable(RangeVariable rangeVariable, QueryBinderContext context) {
        return base.BindRangeVariable(rangeVariable, context);
    }
    public override Expression BindBinaryOperatorNode(BinaryOperatorNode binaryOperatorNode, QueryBinderContext context) {
        return base.BindBinaryOperatorNode(binaryOperatorNode, context);
    }
    public override Expression BindConvertNode(ConvertNode convertNode, QueryBinderContext context) {
        return base.BindConvertNode(convertNode, context);
    }
    public override Expression BindCountNode(CountNode node, QueryBinderContext context) {
        return base.BindCountNode(node, context);
    }
    public override Expression BindInNode(InNode inNode, QueryBinderContext context) {
        return base.BindInNode(inNode, context);
    }
    public override Expression BindSingleResourceFunctionCallNode(SingleResourceFunctionCallNode node, QueryBinderContext context) {
        return base.BindSingleResourceFunctionCallNode(node, context);
    }
    public override Expression BindSingleResourceCastFunctionCall(SingleResourceFunctionCallNode node, QueryBinderContext context) {
        return base.BindSingleResourceCastFunctionCall(node, context);
    }
    public override Expression BindSingleResourceCastNode(SingleResourceCastNode node, QueryBinderContext context) {
        return base.BindSingleResourceCastNode(node, context);
    }
    public override Expression BindAllNode(AllNode allNode, QueryBinderContext context) {
        return base.BindAllNode(allNode, context);
    }
    public override Expression BindAnyNode(AnyNode anyNode, QueryBinderContext context) {
        return base.BindAnyNode(anyNode, context);
    }
    public override Expression BindUnaryOperatorNode(UnaryOperatorNode unaryOperatorNode, QueryBinderContext context) {
        return base.BindUnaryOperatorNode(unaryOperatorNode, context);
    }
    public override Expression BindConstantNode(ConstantNode constantNode, QueryBinderContext context) {
        return base.BindConstantNode(constantNode, context);
    }
    public override Expression BindCollectionConstantNode(CollectionConstantNode node, QueryBinderContext context) {
        return base.BindCollectionConstantNode(node, context);
    }
}