using System;
using System.Threading;
using System.Threading.Tasks;

namespace CloudTimeExample
{
    // ❌ Violating Example: Using local server time (DateTime.Now, TimeZone.CurrentTimeZone)
    public class LocalTimeScheduler
    {
        public void RunDailyTask()
        {
            // Violation: Uses server-local time, which varies by region
            DateTime localNow = DateTime.Now;
            Console.WriteLine($"[Violation] Local time: {localNow}");

            // Violation: Uses TimeZone.CurrentTimeZone, which is server-dependent
            TimeZone zone = TimeZone.CurrentTimeZone;
            Console.WriteLine($"[Violation] Server time zone: {zone.StandardName}");

            // Violation: Scheduled operations without timezone normalization
            if (localNow.Hour == 0)
            {
                Console.WriteLine("Running daily cleanup task (local time)...");
            }
        }
    }

    // ✅ Compliant Example: Using UTC and explicit timezone handling
    public class UtcTimeScheduler
    {
        public void RunDailyTask()
        {
            // Correct: Use Coordinated Universal Time (UTC)
            DateTime utcNow = DateTime.UtcNow;
            Console.WriteLine($"[Compliant] UTC time: {utcNow}");

            // Convert UTC to specific timezone safely
            TimeZoneInfo targetZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            DateTime targetTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, targetZone);

            Console.WriteLine($"[Compliant] Target zone time: {targetTime}");

            if (targetTime.Hour == 0)
            {
                Console.WriteLine("Running daily cleanup task (PST)...");
            }
        }
    }

    // ❌ Violating Example: Using Timer without UTC consideration
    public class LocalTimerJob
    {
        public void StartTimer()
        {
            Timer timer = new Timer(
                e => Console.WriteLine($"[Violation] Timer tick at {DateTime.Now}"),
                null,
                TimeSpan.Zero,
                TimeSpan.FromHours(1)
            );
        }
    }

    // ✅ Compliant Example: Using UTC-aware scheduling
    public class UtcTimerJob
    {
        public async Task StartTimerAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                Console.WriteLine($"[Compliant] Timer tick at {DateTime.UtcNow}");
                await Task.Delay(TimeSpan.FromHours(1), token);
            }
        }
    }
}
