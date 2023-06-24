using System.Collections;

namespace Colibri.Core;

public class StatementBlock : Node, IEnumerable<Node>
{
    public StatementBlock(IReadOnlyList<Node> nodes)
    {
        Nodes = nodes;
    }
    
    public IReadOnlyList<Node> Nodes { get; }

    public override string ToString() =>
        $"{{{Environment.NewLine}{string.Join(Environment.NewLine, Nodes)}{Environment.NewLine}}}";

    public IEnumerator<Node> GetEnumerator() => Nodes.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}