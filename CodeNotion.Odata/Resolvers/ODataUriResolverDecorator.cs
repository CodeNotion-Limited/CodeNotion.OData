using System;
using System.Collections.Generic;
using Microsoft.OData.Edm;
using Microsoft.OData.Edm.Vocabularies;
using Microsoft.OData.UriParser;

namespace CodeNotion.Odata.Resolvers;

public class ODataUriResolverDecorator : ODataUriResolver
{
    protected readonly ODataUriResolver Decoratee;

    public ODataUriResolverDecorator(ODataUriResolver decoratee) =>
        Decoratee = decoratee;

    public override bool EnableCaseInsensitive
    {
        get => Decoratee.EnableCaseInsensitive;
        set => Decoratee.EnableCaseInsensitive = value;
    }

    public override bool EnableNoDollarQueryOptions
    {
        get => Decoratee.EnableNoDollarQueryOptions;
        set => Decoratee.EnableNoDollarQueryOptions = value;
    }

    public override IEnumerable<IEdmOperationImport> ResolveOperationImports(IEdmModel model, string identifier) =>
        Decoratee.ResolveOperationImports(model, identifier);

    public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(IEdmEntityType type, IDictionary<string, string> namedValues, Func<IEdmTypeReference, string, object> convertFunc) =>
        Decoratee.ResolveKeys(type, namedValues, convertFunc);

    public override IEnumerable<KeyValuePair<string, object>> ResolveKeys(IEdmEntityType type, IList<string> positionalValues, Func<IEdmTypeReference, string, object> convertFunc) =>
        Decoratee.ResolveKeys(type, positionalValues, convertFunc);

    public override IEdmProperty ResolveProperty(IEdmStructuredType type, string propertyName) =>
        Decoratee.ResolveProperty(type, propertyName);

    public override IEdmTerm ResolveTerm(IEdmModel model, string termName) =>
        Decoratee.ResolveTerm(model, termName);

    public override IEdmSchemaType ResolveType(IEdmModel model, string typeName) =>
        Decoratee.ResolveType(model, typeName);

    public override IEnumerable<IEdmOperation> ResolveBoundOperations(IEdmModel model, string identifier, IEdmType bindingType) =>
        Decoratee.ResolveBoundOperations(model, identifier, bindingType);

    public override IEdmNavigationSource ResolveNavigationSource(IEdmModel model, string identifier) =>
        Decoratee.ResolveNavigationSource(model, identifier);

    public override IDictionary<IEdmOperationParameter, SingleValueNode> ResolveOperationParameters(IEdmOperation operation, IDictionary<string, SingleValueNode> input) =>
        Decoratee.ResolveOperationParameters(operation, input);

    public override IEnumerable<IEdmOperation> ResolveUnboundOperations(IEdmModel model, string identifier) =>
        Decoratee.ResolveUnboundOperations(model, identifier);

    public override void PromoteBinaryOperandTypes(BinaryOperatorKind binaryOperatorKind, ref SingleValueNode leftNode, ref SingleValueNode rightNode, out IEdmTypeReference typeReference) =>
        Decoratee.PromoteBinaryOperandTypes(binaryOperatorKind, ref leftNode, ref rightNode, out typeReference);

    public override bool Equals(object? obj) =>
        Decoratee.Equals(obj);

    public override int GetHashCode() =>
        Decoratee.GetHashCode();

    public override string ToString() =>
        $"DECORATOR[{Decoratee}]";
}