using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using VNet.EventBus;

namespace VNet.EventBus.Imp
{
    /// <summary>
    /// 事件工厂，所有事件通过 EventMessage 传递，自动桥接类型转换
    /// </summary>
    public class CrossPluginEventBusFactory : ICrossPluginEventBusFactory
    {
        private readonly ICrossPluginEventBus _eventBus;
        private readonly ConcurrentDictionary<Delegate, (Guid Id, Action<EventMessage> Wrapper)> _actionMappings = new();
        private readonly ConcurrentDictionary<(Type source, Type target), List<(Func<object, object> getter, Action<object, object> setter)>> _compiledMappings = new();
        public CrossPluginEventBusFactory()
        {
            _eventBus = new CrossPluginEventBus();
        }

        public async Task PublishAsync<T>(T data) where T : new()
        {
            if (data == null) return;
            var message = new EventMessage(data);
            await _eventBus.Publish(message);
        }

        public async Task PublishAsync<T>(string topic, T data) where T : new()
        {
            if (data == null) return;
            var message = new EventMessage(data);
            await _eventBus.Publish(topic, message);
        }

        public Guid Subscriber<T>(Action<T> action) where T : new()
        {
            Action<EventMessage> wrapper = CreateWrapper(action);
            var id = _eventBus.Subscriber(wrapper);
            _actionMappings.TryAdd(action, (id, wrapper));
            return id;
        }

        public Guid Subscriber<T>(string topic, Action<T> action) where T : new()
        {
            Action<EventMessage> wrapper = CreateWrapper(action);
            var id = _eventBus.Subscriber(topic, wrapper);
            _actionMappings.TryAdd(action, (id, wrapper));
            return id;
        }

        public void UnSubscriber<T>(Action<T> action) where T : new()
        {
            if (_actionMappings.TryRemove(action, out var mapping))
            {
                _eventBus.UnSubscriber(mapping.Id);
            }
        }

        public void UnSubscriber<T>(string topic, Action<T> action) where T : new()
        {
            if (_actionMappings.TryRemove(action, out var mapping))
            {
                _eventBus.UnSubscriber(topic, mapping.Id);
            }
        }

        private Action<EventMessage> CreateWrapper<T>(Action<T> action) where T : new()
        {
            Action<EventMessage> wrapper = msg =>
            {
                if (msg.TypeName == typeof(T).Name)
                {
                    try
                    {
                        object dyn = msg.Payload;
                        var sourceType = dyn.GetType();
                        var targetType = typeof(T);

                        var mappings = _compiledMappings.GetOrAdd((sourceType, targetType), key =>
                        {
                            var result = new List<(Func<object, object> getter, Action<object, object> setter)>();
                            var sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                            var tartgetProperties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                            foreach (var sourceProp in sourceProperties)
                            {
                                var targetProp = tartgetProperties.FirstOrDefault(p =>
                                p.Name == sourceProp.Name &&
                                p.PropertyType == sourceProp.PropertyType &&
                                p.CanWrite);
                                if (targetProp != null)
                                {
                                    var getter = CreateGetter(sourceProp);
                                    var setter = CreateSetter(targetProp);
                                    result.Add((getter, setter));
                                }
                            }
                            return result;
                        });
                        var proxy = new T();
                        foreach (var (getter, setter) in mappings)
                        {
                            var value = getter(dyn);
                            setter(proxy, value);
                        }
                        action(proxy);
                        return;
                    }
                    catch
                    {
                        return;
                    }

                }
                else if (msg.Payload is T typed)
                {
                    action(typed);
                    return;
                }
            };
            return wrapper;
        }


        private static Func<object, object> CreateGetter(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var convertInstance = Expression.Convert(instance, property.DeclaringType!);
            var propertyAccess = Expression.Property(convertInstance, property);
            var convertResult = Expression.Convert(propertyAccess, typeof(object));
            var lambda = Expression.Lambda<Func<object, object>>(convertResult, instance);
            return lambda.Compile();
        }

        private static Action<object, object> CreateSetter(PropertyInfo property)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var value = Expression.Parameter(typeof(object), "value");
            var convertInstance = Expression.Convert(instance, property.DeclaringType!);
            var convertValue = Expression.Convert(value, property.PropertyType);
            var propertyAccess = Expression.Property(convertInstance, property);
            var assign = Expression.Assign(propertyAccess, convertValue);
            var lambda = Expression.Lambda<Action<object, object>>(assign, instance, value);
            return lambda.Compile();
        }
    }

}
