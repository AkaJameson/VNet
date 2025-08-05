using System.Collections.Concurrent;
using VNet.EventBus;

namespace VNet.EventBus.Imp
{
    public class CrossPluginEventBus : ICrossPluginEventBus
    {
        private readonly ConcurrentDictionary<Guid, Action<EventMessage>> _globalSubscribers = new();
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, Action<EventMessage>>> _topicSubscribers = new();
        public async Task Publish(EventMessage data)
        {
            if (data == null)
                return;
            if (_globalSubscribers.Count > 0)
            {
                var tasks = _globalSubscribers.Values.Select(subscriber => Task.Run(() => subscriber(data)));
                await Task.WhenAll(tasks);
            }
        }

        public async Task Publish(string topic, EventMessage data)
        {
            if (data == null) return;

            if (_topicSubscribers.TryGetValue(topic, out var topicSubscribers))
            {
                if (topicSubscribers.Count > 0)
                {
                    var tasks = topicSubscribers.Values.Select(subscriber => Task.Run(() => subscriber(data)));
                    await Task.WhenAll(tasks);
                }
            }

        }

        public Guid Subscriber(Action<EventMessage> action)
        {
            var id = Guid.NewGuid();
            _globalSubscribers.TryAdd(id, action);
            return id;
        }

        public Guid Subscriber(string topic, Action<EventMessage> action)
        {
            var id = Guid.NewGuid();
            var topicList = _topicSubscribers.GetOrAdd(topic, _ => new ConcurrentDictionary<Guid, Action<EventMessage>>());
            topicList.TryAdd(id, action);
            return id;
        }

        public void UnSubscriber(Guid id)
        {
            _globalSubscribers.TryRemove(id, out _);
        }

        public void UnSubscriber(string topic, Guid id)
        {
            if (_topicSubscribers.TryGetValue(topic, out var topicList))
            {
                topicList.TryRemove(id, out _);
                if (topicList.IsEmpty)
                {
                    _topicSubscribers.TryRemove(topic, out _);
                }
            }
        }
    }
}
