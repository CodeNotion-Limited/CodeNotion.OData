using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace CodeNotion.Odata.Filtering;

internal class SpecialEdmDateNode : SingleValueNode
{
    private readonly ConstantNode _originalNode;

    public override IEdmTypeReference TypeReference => _originalNode.TypeReference;
    public override QueryNodeKind Kind => QueryNodeKind.Constant;
    public Date EdmDate { get; }

    public SpecialEdmDateNode(ConstantNode originalNode)
    {
        _originalNode = originalNode;
        EdmDate = (Date)_originalNode.Value;
    }
}
