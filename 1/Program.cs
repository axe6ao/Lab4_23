using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public delegate void EventHandler<T>(object sender, T args);

public class EventBus
{
    private Dictionary<string, List<Delegate>> _eventHandlers;
    private Dictionary<string, int> _throttleLimits;

    public EventBus()
    {
        _eventHandlers = new Dictionary<string, List<Delegate>>();
        _throttleLimits = new Dictionary<string, int>();
    }

    public void Register<T>(string eventName, EventHandler<T> handler)
    {
        if (!_eventHandlers.ContainsKey(eventName))
        {
            _eventHandlers[eventName] = new List<Delegate>();
        }

        _eventHandlers[eventName].Add(handler);
    }

    public void Unregister<T>(string eventName, EventHandler<T> handler)
    {
        if (_eventHandlers.ContainsKey(eventName))
        {
            _eventHandlers[eventName].Remove(handler);
        }
    }

    public void SetThrottleLimit(string eventName, int limit)
    {
        _throttleLimits[eventName] = limit;
    }

    public void Send<T>(string eventName, object sender, T args)
    {
        if (!_eventHandlers.ContainsKey(eventName))
        {
            return;
        }

        if (_throttleLimits.ContainsKey(eventName))
        {
            var handlers = _eventHandlers[eventName];

            for (int i = 0; i < handlers.Count; i++)
            {
                var handler = handlers[i];

                if (handler is EventHandler<T> eventHandler)
                {
                    var now = DateTime.UtcNow;
                    var lastSent = (DateTime?)handler.Target;

                    if (!lastSent.HasValue || (now - lastSent.Value).TotalMilliseconds >= _throttleLimits[eventName])
                    {
                        handler.Target = now;
                        eventHandler(sender, args);
                    }
                }
            }
        }
        else
        {
            foreach (var handler in _eventHandlers[eventName])
            {
                if (handler is EventHandler<T> eventHandler)
                {
                    eventHandler(sender, args);
                }
            }
        }
    }
}