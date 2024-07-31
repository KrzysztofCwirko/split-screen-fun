using UnityEngine;

namespace FPS.Scripts.Extensions
{
    public static class TransformExtensions
    {
        public static T FindComponentInParent<T>(this GameObject root) where T: Component
        {
            var parent = root.transform.parent;

            while (parent != null)
            {
                if (parent.TryGetComponent(out T component))
                {
                    return component;
                }

                parent = parent.parent;
            }

            return null;
        }
    }
}