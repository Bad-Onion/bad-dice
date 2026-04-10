namespace _Project.Application.Interfaces
{
    /// <summary>
    /// Interface for objects that can be hovered over with a pointer (e.g., mouse cursor).
    /// </summary>
    public interface IHoverablePointerTarget
    {
        /// <summary>
        /// Sets the visual state of the object to indicate whether it is being hovered over or not.
        /// </summary>
        /// <param name="isHovered">True if the object is being hovered over; otherwise, false.</param>
        void SetHoverVisual(bool isHovered);
    }
}

