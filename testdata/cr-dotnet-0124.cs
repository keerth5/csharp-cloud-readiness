using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace CloudThreadAffinityExample
{
    // ❌ Violating Example: Explicitly setting processor affinity
    public class ThreadAffinityViolation
    {
        public void SetProcessorAffinity()
        {
            Process currentProcess = Process.GetCurrentProcess();

            // Violation: Attempt to manually bind the process to a specific CPU core
            // Cloud environments do not guarantee consistent core allocation.
            currentProcess.ProcessorAffinity = (IntPtr)1; // ❌ Bind to CPU 0
            Console.WriteLine("[Violation] Processor affinity set to CPU 0.");

            Thread thread = new Thread(() =>
            {
                Console.WriteLine($"[Violation] Running thread with fixed affinity: {Thread.CurrentThread.ManagedThreadId}");
            });

            thread.Start();
        }
    }

    // ❌ Violating Example: Making assumptions about available CPU cores
    public class CpuBindingAssumption
    {
        public void RunOnSpecificCore()
        {
            int targetCore = 2;
            Console.WriteLine($"[Violation] Attempting to bind work to specific core {targetCore}...");

            // Violation: Assuming predictable CPU core binding
            if (Environment.ProcessorCount < targetCore)
            {
                Console.WriteLine("[Violation] Not enough cores, but still assuming CPU layout.");
            }
        }
    }

    // ✅ Compliant Example: Letting the OS/cloud scheduler handle thread distribution
    public class CloudSafeThreadManager
    {
        public void RunCloudTasks()
        {
            Console.WriteLine("[Compliant] Running parallel tasks without explicit CPU binding...");

            Parallel.For(0, 5, i =>
            {
                Console.WriteLine($"[Compliant] Task {i} running on thread {Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(100);
            });

            Console.WriteLine("[Compliant] OS/cloud scheduler optimally distributed threads.");
        }
    }

    // ✅ Compliant Example: Using ThreadPool or Task-based parallelism
    public class ElasticThreadScheduler
    {
        public async Task RunElasticTasksAsync()
        {
            Console.WriteLine("[Compliant] Using task scheduler for CPU elasticity...");

            var tasks = new Task[5];
            for (int i = 0; i < 5; i++)
            {
                int taskId = i;
                tasks[i] = Task.Run(() =>
                {
                    Console.WriteLine($"[Compliant] Task {taskId} executing on thread {Thread.CurrentThread.ManagedThreadId}");
                });
            }

            await Task.WhenAll(tasks);
            Console.WriteLine("[Compliant] Tasks completed without fixed thread affinity.");
        }
    }
}
