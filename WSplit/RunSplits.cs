using System;
using System.Collections.Generic;

public class RunSplits
{
    public CompareType CompType;
    public bool NewBestTime;
    private bool oldFaster;
    public List<SplitSegment> segments = new List<SplitSegment>();

    public string RunTitle { set; get; }

    public string RunGoal { set; get; }

    public int AttemptsCount { set; get; }

    public int StartDelay { set; get; }

    public string RunFile { set; get; }

    public bool UnsavedSplit { set; get; }

    public void Add(SplitSegment segment)
    {
        segments.Add(segment);
        if ((segment.BestTime != 0.0) && ((segment.BestTime < segment.OldTime) || (segment.OldTime == 0.0)))
        {
            oldFaster = false;
        }
        else
        {
            oldFaster = true;
        }
    }

    public void Clear()
    {
        // ? detailPreferredSize = clockMinimumSize;
        segments.Clear();
        ResetParams();
        ResetSegments();
    }

    private void ResetParams()
    {
        RunTitle = string.Empty;
        RunGoal = string.Empty;
        StartDelay = 0;
        AttemptsCount = 0;
    }

    public double CompTime()
    {
        return CompTime(Math.Min(LiveIndex, LastIndex));
    }

    // Now has the ability to return the sum of bests as the comparison time
    public double CompTime(int index)
    {
        if ((index < 0) || (index > LastIndex))
        {
            return 0.0;
        }
        if (CompType != CompareType.Best)
        {
            if (CompType == CompareType.Old)
            {
                return segments[index].OldTime;
            }
            if (CompType == CompareType.SumOfBests)
            {
                return SumOfBests(index);
            }
            if (oldFaster)
            {
                return segments[index].OldTime;
            }
        }
        return segments[index].BestTime;
    }

    public double SumOfBests(int index)
    {
        double sum = 0.0;
        for (int i = 0; i <= index; ++i)
        {
            if (segments[i].BestSegment == 0.0)
                return 0.0;
            sum += segments[i].BestSegment;
        }
        return sum;
    }

    public void DoSplit(double time)
    {
        if (LiveRun && !Done)
        {
            segments[LiveIndex].LiveTime = time;
            if ((LiveIndex == LastIndex) && ((segments[LastIndex].LiveTime < segments[LastIndex].BestTime) || (segments[LastIndex].BestTime == 0.0)))
            {
                NewBestTime = true;
            }
            LiveIndex++;
        }
    }

    public double LastDelta(int index)
    {
        if ((index > 0) && (index <= LastIndex))
        {
            for (int i = index - 1; i >= 0; i--)
            {
                if ((segments[i].LiveTime != 0.0) && (CompTime(i) != 0.0))
                {
                    return (segments[i].LiveTime - CompTime(i));
                }
            }
        }
        return 0.0;
    }

    public double LiveSegment(int index)
    {
        if ((index >= 0) && (index <= LastIndex))
        {
            double liveTime = segments[index].LiveTime;
            if ((index <= 0) || (liveTime <= 0.0))
            {
                return liveTime;
            }
            if (segments[index - 1].LiveTime > 0.0)
            {
                return (liveTime - segments[index - 1].LiveTime);
            }
        }
        return 0.0;
    }

    public void LiveToOld()
    {
        foreach (SplitSegment segment in segments)
        {
            segment.OldTime = segment.LiveTime;
        }
    }

    public void Next()
    {
        if (LiveIndex < LastIndex)
        {
            DoSplit(0.0);
        }
    }

    public void Previous()
    {
        LiveIndex--;
        CurrentSegment.LiveTime = 0.0;
        NewBestTime = false;
    }

    public void ResetSegments()
    {
        foreach (SplitSegment segment in segments)
        {
            segment.BackupBest = segment.BestTime;
            segment.BackupBestSegment = segment.BestSegment;
            segment.LiveTime = 0.0;
        }
        LiveIndex = 0;
        NewBestTime = false;
    }

    public void RestoreBest()
    {
        foreach (SplitSegment segment in segments)
        {
            segment.BestTime = segment.BackupBest;
            segment.BestSegment = segment.BackupBestSegment;
        }
        if (LastIndex >= 0)
        {
            if ((segments[LastIndex].BestTime != 0.0) && ((segments[LastIndex].BestTime < segments[LastIndex].OldTime) || (segments[LastIndex].OldTime == 0.0)))
            {
                oldFaster = false;
            }
            else
            {
                oldFaster = true;
            }
        }
    }

    public double RunDelta(double time, int index)
    {
        if ((CompTime(index) > 0.0) && (time > 0.0))
        {
            return (time - CompTime(index));
        }
        return 0.0;
    }

    public double RunDeltaAt(int index)
    {
        if ((index >= 0) && (index <= LastIndex))
        {
            return RunDelta(segments[index].LiveTime, index);
        }
        return 0.0;
    }

    public double SegDelta(double time, int index)
    {
        if (CompTime(index) > 0.0)
        {
            return (RunDelta(time, index) - LastDelta(index));
        }
        return 0.0;
    }

    public bool NeedUpdate(bool bestOverall)
    {
        // This will never return true if no run is loaded.
        if (NewBestTime)
            return true;
        for (int i = 0; i <= LastIndex; ++i)
        {
            if (((!bestOverall && segments[i].LiveTime != 0.0) && (segments[i].LiveTime < segments[i].BestTime || segments[i].BestTime == 0.0)) // If !bestoverall and the split is faster than before
                || (segments[i].LiveTime != 0.0 && (i == 0 || segments[i - 1].LiveTime != 0.0) && (segments[i].BestSegment == 0.0 || LiveSegment(i) < segments[i].BestSegment)))   // Or if it's a new Best Segment
                return true;
        }
        return false;
    }

    public void UpdateBest(bool bestOverall)
    {
        // Updates each segment if needed
        foreach (SplitSegment segment in segments)
        {
            if ((bestOverall && NewBestTime) || ((!bestOverall && (segment.LiveTime != 0.0)) && ((segment.LiveTime < segment.BestTime) || (segment.BestTime == 0.0))))
            {
                segment.BestTime = segment.LiveTime;
            }
        }

        // Gets rid of incoherences
        double bestTime = 0.0;
        for (int i = LastIndex; i >= 0; i--)
        {
            if (segments[i].BestTime != 0.0)
            {
                if (bestTime == 0.0)
                {
                    bestTime = segments[LastIndex].BestTime;
                }
                if (segments[i].BestTime > bestTime)
                {
                    segments[i].BestTime = 0.0;
                }
                else
                {
                    bestTime = segments[i].BestTime;
                }
            }
        }

        // Updates bests if needed
        for (int j = 0; j <= LastIndex; j++)
        {
            if (((segments[j].LiveTime != 0.0) && ((j == 0) || (segments[j - 1].LiveTime != 0.0))) && ((LiveSegment(j) < segments[j].BestSegment) || (segments[j].BestSegment == 0.0)))
            {
                segments[j].BestSegment = LiveSegment(j);
            }
        }
        if (LastIndex >= 0)
        {
            if ((segments[LastIndex].BestTime != 0.0) && ((segments[LastIndex].BestTime < segments[LastIndex].OldTime) || (segments[LastIndex].OldTime == 0.0)))
            {
                oldFaster = false;
            }
            else
            {
                oldFaster = true;
            }
        }

        NewBestTime = false;
    }

    public CompareType ComparingType
    {
        get
        {
            if (CompType != CompareType.Fastest)
            {
                return CompType;
            }
            if (oldFaster)
            {
                return CompareType.Old;
            }
            return CompareType.Best;
        }
    }

    public int Count
    {
        get
        {
            return segments.Count;
        }
    }

    public SplitSegment CurrentSegment
    {
        get
        {
            if (LiveRun && !Done)
            {
                return segments[LiveIndex];
            }
            return new SplitSegment(null);
        }
    }

    public bool Done
    {
        get
        {
            return (LiveRun && (LiveIndex > LastIndex));
        }
    }

    public int LastIndex
    {
        get
        {
            return (segments.Count - 1);
        }
    }

    public SplitSegment LastSegment
    {
        get
        {
            if (LiveRun)
            {
                return segments[LastIndex];
            }
            return new SplitSegment(null);
        }
    }

    public int LiveIndex { get; private set; }

    public bool LiveRun
    {
        get
        {
            return (segments.Count > 0);
        }
    }

    public enum CompareType
    {
        Fastest,
        Old,
        Best,
        SumOfBests
    }
}