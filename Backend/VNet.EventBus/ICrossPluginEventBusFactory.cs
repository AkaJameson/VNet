namespace VNet.EventBus
{
    /// <summary>
    /// 事件工厂
    /// </summary>
    public interface ICrossPluginEventBusFactory
    {
        Task PublishAsync<T>(T data) where T : new();
        Task PublishAsync<T>(string topic, T data) where T : new();
        Guid Subscriber<T>(Action<T> action) where T : new();
        Guid Subscriber<T>(string topic, Action<T> action) where T : new();
        void UnSubscriber<T>(Action<T> action) where T : new();
        void UnSubscriber<T>(string topic, Action<T> action) where T : new();
    }
}