using System.Text;
using NoobsChess.UCI;
using NoobsEngine;

namespace NoobsChess.Search;

public class SearchInfo {
    public long StartTime;
    public long StopTime;
    public int DepthLimit;
    public bool IsDepthSet;
    public bool IsTimeSet;
    public int MovesToGo;

    public long Nodes;

    public bool IsQuit;
    public bool IsStopped;
    public bool IsInfinite;

    public float FailHigh;
    public float FailHighFirst;

    public Thread? SearchThread = null;

    public void CheckUp() {
        if (IsTimeSet && NoobsUtils.GetTimeInMs() > StopTime) {
            IsStopped = true;
        }
        if (IsStopped && SearchThread != null) {
            SearchThread.Interrupt();
            SearchThread = null;
        }
    }
}