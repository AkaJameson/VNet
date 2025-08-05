namespace VNet.EventBus
{
    public class EventMessage
    {
        public string TypeName { get; set; }
        public object Payload { get; set; }
        /// <summary>
        /// 中间层转化
        /// </summary>
        public EventMessage(object payload)
        {
            TypeName = payload.GetType().Name!;
            Payload = payload;
        }
    }

   
}
