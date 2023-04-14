using Ardalis.SmartEnum;
using Microsoft.OData.Edm;
using Microsoft.OData.UriParser;

namespace CodeNotion.Odata.Sorting;

internal class SmartEnumIntNode<TEnum> : SingleValueNode where TEnum : SmartEnum<TEnum, int>
{
    private readonly ConstantNode _originalNode;

    public override IEdmTypeReference TypeReference => _originalNode.TypeReference;
    public override QueryNodeKind Kind => QueryNodeKind.Constant;
    public SmartEnum<TEnum> SmartEnum { get; }

    public SmartEnumIntNode(ConstantNode originalNode)
    {
        _originalNode = originalNode;
        SmartEnum = (SmartEnum<TEnum>)typeof(SmartEnum<TEnum>).GetMethod(nameof(SmartEnum.FromValue))!.Invoke(null, new object[] { (int)_originalNode.Value })!;
    }
}