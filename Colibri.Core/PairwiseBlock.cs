using System.Collections;

namespace Colibri.Core;

public class PairwiseBlock : Pair, IEnumerable<Pair>
{
    public PairwiseBlock(IReadOnlyList<Pair> pairs)
    {
        Pairs = pairs;
        Car = pairs[0];
        Cdr = List.FromNodes(pairs.Skip(1));
    }
    
    public IReadOnlyList<Pair> Pairs { get; }

    public override string ToString() =>
        $"[{Environment.NewLine}{string.Join(Environment.NewLine, Pairs.Select(i => $"{i.Car} {i.Cdr}"))}{Environment.NewLine}]";

    public new IEnumerator<Pair> GetEnumerator() => Pairs.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}