using _Project.Application.Interfaces;

namespace _Project.Application.Events
{
    public static class Bus<T> where T : struct, IEvent
    {
        public delegate void EventDelegate(T evt);
        public static event EventDelegate OnEvent;

        public static void Raise(T evt) => OnEvent?.Invoke(evt);
    }
}