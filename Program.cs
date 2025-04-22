using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

class RamCleaner
{
    [DllImport("psapi.dll")]
    static extern int EmptyWorkingSet(IntPtr hProcess);

    [DllImport("psapi.dll", SetLastError = true)]
    static extern bool GetPerformanceInfo(out PERFORMANCE_INFORMATION pInfo, int size);

    [StructLayout(LayoutKind.Sequential)]
    struct PERFORMANCE_INFORMATION
    {
        public int cb;
        public IntPtr CommitTotal;
        public IntPtr CommitLimit;
        public IntPtr CommitPeak;
        public IntPtr PhysicalTotal;
        public IntPtr PhysicalAvailable;
        public IntPtr SystemCache;
        public IntPtr KernelTotal;
        public IntPtr KernelPaged;
        public IntPtr KernelNonpaged;
        public IntPtr PageSize;
        public int HandleCount;
        public int ProcessCount;
        public int ThreadCount;
    }

    static ulong GetUsedMemoryInMB()
    {
        if (GetPerformanceInfo(out PERFORMANCE_INFORMATION pi, Marshal.SizeOf(typeof(PERFORMANCE_INFORMATION))))
        {
            ulong pageSize = (ulong)pi.PageSize;
            ulong total = (ulong)pi.PhysicalTotal * pageSize;
            ulong available = (ulong)pi.PhysicalAvailable * pageSize;
            return (total - available) / (1024 * 1024); // in MB
        }
        return 0;
    }

    static void CleanRam()
    {
        Console.WriteLine("Measuring RAM usage...");
        ulong ramBefore = GetUsedMemoryInMB();
        Console.WriteLine($"[RAM Before]: {ramBefore} MB\n");

        int trimmedCount = 0;
        foreach (Process process in Process.GetProcesses())
        {
            try
            {
                if (!process.HasExited)
                {
                    EmptyWorkingSet(process.Handle);
                    trimmedCount++;
                    Console.WriteLine($"[✓] Trimmed: {process.ProcessName}");
                }
            }
            catch
            {
                Console.WriteLine($"[x] Skipped: {process.ProcessName}");
            }
        }

        ulong ramAfter = GetUsedMemoryInMB();
        ulong freed = ramBefore > ramAfter ? ramBefore - ramAfter : 0;

        Console.WriteLine($"\n[RAM After]: {ramAfter} MB");
        Console.WriteLine($"[✓] Freed up: {freed} MB");
        Console.WriteLine($"\nDone. Trimmed memory from {trimmedCount} processes.");
    }

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // ASCII Header
        Console.WriteLine(@"
$$$$$$$\   $$$$$$\  $$\      $$\        $$$$$$\  $$\       $$$$$$$$\  $$$$$$\  $$\   $$\ $$$$$$$$\ $$$$$$$\  
$$  __$$\ $$  __$$\ $$$\    $$$ |      $$  __$$\ $$ |      $$  _____|$$  __$$\ $$$\  $$ |$$  _____|$$  __$$\ 
$$ |  $$ |$$ /  $$ |$$$$\  $$$$ |      $$ /  \__|$$ |      $$ |      $$ /  $$ |$$$$\ $$ |$$ |      $$ |  $$ |
$$$$$$$  |$$$$$$$$ |$$\$$\$$ $$ |      $$ |      $$ |      $$$$$\    $$$$$$$$ |$$ $$\$$ |$$$$$\    $$$$$$$  |
$$  __$$< $$  __$$ |$$ \$$$  $$ |      $$ |      $$ |      $$  __|   $$  __$$ |$$ \$$$$ |$$  __|   $$  __$$< 
$$ |  $$ |$$ |  $$ |$$ |\$  /$$ |      $$ |  $$\ $$ |      $$ |      $$ |  $$ |$$ |\$$$ |$$ |      $$ |  $$ |
$$ |  $$ |$$ |  $$ |$$ | \_/ $$ |      \$$$$$$  |$$$$$$$$\ $$$$$$$$\ $$ |  $$ |$$ | \$$ |$$$$$$$$\ $$ |  $$ |
\__|  \__|\__|  \__|\__|     \__|       \______/ \________|\________|\__|  \__|\__|  \__|\________|\__|  \__|  
                                                                                                             
by maestrodelfuego
        ");

        // Perform initial RAM cleaning
        CleanRam();

        // Periodically clean RAM every 2 minutes (120 seconds)
        while (true)
        {
            Console.WriteLine("\nNext cleaning in 2 minutes...");
            Thread.Sleep(120 * 1000); // Sleep for 2 minutes
            CleanRam();
        }
    }
}
