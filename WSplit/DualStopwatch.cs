using System;
using System.Diagnostics;

public class DualStopwatch
{
    private bool fallbackIsRunning;
    private long pauseTicks;
    private TimeSpan pauseTime = TimeSpan.Zero;
    private long startTicks;
    private DateTime startTime;
    private readonly Stopwatch systimer = new Stopwatch();
    public bool useFallback;

    // Used for when the timer starts at a time greater than 0 ticks
    private TimeSpan startedAt = TimeSpan.Zero;

    public DualStopwatch(bool useFallbackMethod)
    {
        useFallback = useFallbackMethod;
    }

    public void Reset()
    {
        systimer.Reset();
        pauseTime = TimeSpan.Zero;
        pauseTicks = 0L;
        startedAt = TimeSpan.Zero;
        fallbackIsRunning = false;
    }

    public void StartAt(TimeSpan offset)
    {
        // If the timer wasn't already running and it was not paused, apply the offset
        if (!systimer.IsRunning && !fallbackIsRunning && pauseTime.Ticks >= 0L)
            startedAt = offset;
        Start();
    }

    public void Start()
    {
        systimer.Start();
        if (!fallbackIsRunning && pauseTime.Ticks > 0L)
        {
            startTime = DateTime.UtcNow - pauseTime;
            pauseTime = TimeSpan.Zero;
            startTicks = Environment.TickCount - pauseTicks;
            pauseTicks = 0L;
        }
        else
        {
            startTime = DateTime.UtcNow;
            startTicks = Environment.TickCount;
        }
        fallbackIsRunning = true;
    }

    public void Stop()
    {
        systimer.Stop();
        if (fallbackIsRunning)
        {
            pauseTime = DateTime.UtcNow - startTime;
            pauseTicks = Environment.TickCount - startTicks;
            fallbackIsRunning = false;
        }
    }

    public double driftMilliseconds
    {
        get
        {
            return (fallbackElapsed.TotalMilliseconds - systimer.Elapsed.TotalMilliseconds);
        }
    }

    public TimeSpan Elapsed
    {
        get
        {
            if (useFallback)
            {
                return fallbackElapsed + startedAt;
            }
            return systimer.Elapsed + startedAt;
        }
    }

    public long ElapsedMilliseconds
    {
        get
        {
            if (useFallback)
            {
                return (long)Math.Truncate(fallbackElapsed.TotalMilliseconds + startedAt.TotalMilliseconds);
                //return (long) Math.Round(this.fallbackElapsed.TotalMilliseconds + startedAt.TotalMilliseconds);
            }
            return (long)Math.Truncate(systimer.ElapsedMilliseconds + startedAt.TotalMilliseconds);
            //return (long) Math.Round(this.systimer.ElapsedMilliseconds + startedAt.TotalMilliseconds);
        }
    }

    public long ElapsedTicks
    {
        get
        {
            return Elapsed.Ticks;
        }
    }

    private TimeSpan fallbackElapsed
    {
        get
        {
            if (!fallbackIsRunning)
            {
                return pauseTime;
            }
            TimeSpan span = DateTime.UtcNow - startTime;
            TimeSpan span2 = TimeSpan.FromMilliseconds(Environment.TickCount - startTicks);
            if (span2 < TimeSpan.Zero)
            {
                span2 += TimeSpan.FromMilliseconds(4294967295);
            }
            double num = Math.Abs((double)(span.TotalMilliseconds - span2.TotalMilliseconds));
            if ((num > 500.0) && (Math.Abs((double)(num / span.TotalMilliseconds)) > 0.00013888888888888889))
            {
                return span2;
            }
            return span;
        }
    }

    public bool IsRunning
    {
        get
        {
            if (useFallback)
            {
                return fallbackIsRunning;
            }
            return systimer.IsRunning;
        }
    }
}