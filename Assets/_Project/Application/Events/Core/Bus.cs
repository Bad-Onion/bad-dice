using _Project.Application.Interfaces;

namespace _Project.Application.Events.Core
{
    /// <summary>
    /// A simple event bus implementation for events of type T. It allows subscribers to listen for events of type T and
    /// publishers to raise events of type T. The event bus uses a delegate to define the event handler and an event to allow
    /// subscribers to register their handlers. When an event is raised, all registered handlers are invoked with the event data.
    /// </summary>
    /// <typeparam name="T">The type of event that this bus will handle. It must be a struct that implements the IEvent interface.</typeparam>
    public static class Bus<T> where T : struct, IEvent
    {
        public delegate void EventDelegate(T evt);
        public static event EventDelegate OnEvent;

        public static void Raise(T evt) => OnEvent?.Invoke(evt);
    }
}