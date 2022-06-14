using UnityEngine;

namespace Extensions
{
    public static class GameObjectExtensions
    {

        public static T GetOrAddComponent<T>(this GameObject gameObject) 
            where T : Component => gameObject.GetComponent<T>() ?? gameObject.AddComponent<T>();

        public static bool HasComponent<T>(this GameObject gameObject) 
            where T : Component => gameObject.GetComponent<T>() != null;
            
        public static TComponent AddComponent<TComponent, TFirstArgument>
            (this GameObject gameObject, TFirstArgument firstArgument)
            where TComponent : MonoBehaviour
        {
            Argument<TFirstArgument>.First = firstArgument;
         
            var _component = gameObject.AddComponent<TComponent>();
 
            Argument<TFirstArgument>.First = default;
 
            return _component;
        }
        
        public static TComponent AddComponent<TComponent, TFirstArgument, TSecondArgument>
            (this GameObject gameObject, TFirstArgument firstArgument, TSecondArgument secondArgument)
            where TComponent : MonoBehaviour
        {
            Arguments<TFirstArgument, TSecondArgument>.First = firstArgument;
            Arguments<TFirstArgument, TSecondArgument>.Second = secondArgument;
         
            var _component = gameObject.AddComponent<TComponent>();
 
            Arguments<TFirstArgument, TSecondArgument>.First = default;
            Arguments<TFirstArgument, TSecondArgument>.Second = default;
 
            return _component;
        }
    }
    
    public static class Argument<TFirstArgument>
    {
        public static TFirstArgument First { get; internal set; }
    }
    
    public static class Arguments<TFirstArgument, TSecondArgument>
    {
        public static TFirstArgument First { get; internal set; }
        public static TSecondArgument Second { get; internal set; }
    }

}