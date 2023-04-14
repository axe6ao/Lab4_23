using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3 { }
public class RetryPolicy
{
    private int _maxRetries;
    private TimeSpan _initialDelay;
    private TimeSpan _maxDelay;
    private Random _random;

    public RetryPolicy(int maxRetries, TimeSpan initialDelay, TimeSpan maxDelay)
    {
        _maxRetries = maxRetries;
        _initialDelay = initialDelay;
        _maxDelay = maxDelay;
        _random = new Random();
    }

    public async Task ExecuteAsync(Func<Task> action)
    {
        int retries = 0;
        TimeSpan delay = _initialDelay;
        while (true)
        {
            try
            {
                await action();
                return;
            }
            catch
            {
                retries++;
                if (retries > _maxRetries)
                {
                    throw;
                }
                await Task.Delay(delay);
                delay = TimeSpan.FromTicks(Math.Min(delay.Ticks * 2, _maxDelay.Ticks));
                int jitter = _random.Next(-(int)delay.TotalMilliseconds, (int)delay.TotalMilliseconds);
                delay += TimeSpan.FromMilliseconds(jitter);
            }
        }
    }
}
