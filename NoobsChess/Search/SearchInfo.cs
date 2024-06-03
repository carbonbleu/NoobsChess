namespace NoobsChess.Search;

public class SearchInfo {
    public long StartTime;
    public long StopTime;
    public int DepthLimit;
    public int DepthSet;
    public int TimeSet;
    public int MovesToGo;

    public long Nodes;

    public bool IsQuit;
    public bool IsStopped;
    public bool IsInfinite;

    public float FailHigh;
    public float FailHighFirst;
}