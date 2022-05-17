using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Framework.Helper
{
    public static class SingletonPool
    {
        private static Dictionary<Type, System.Object> _singletons = new Dictionary<Type, System.Object>();

        public static void Add<T>(System.Object instance) where T : class, IDisposable
        {
            _singletons[typeof(Singleton<T>)] = instance;
        }

        public static void Remove<T>() where T : class, IDisposable
        {
            _singletons.Remove(typeof(Singleton<T>));
        }

        public static T Get<T>() where T : class, IDisposable
        {
            System.Object instance;
            if (_singletons.TryGetValue(typeof(Singleton<T>), out instance))
            {
                return (T)instance;
            }

            return null;
        }

        public static void DestroyAll(params Type[] except)
        {
            foreach (var type in _singletons.Keys.ToArray())
            {
                if (!except.ArrayContains<Type>(type))
                {
                    try
                    {
                        Debug.Log($"Singleton Type: {type}");
                        MethodInfo mi = type.GetMethod("Destroy", BindingFlags.Public | BindingFlags.Static, null, new Type[] { }, null);
                        mi.Invoke(null, null);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError($"Singleton destroy exception: {e.ToString()}");
                    }
                }
            }
            _singletons.Clear();
        }

        public static bool ArrayContains<T>(this T[] src, T elem)
        {
            return Array.IndexOf(src, elem) != -1;
        }
    }

    public static class Singleton<T> where T : class, IDisposable
    {
        private static T _instance;
        private static bool _inCreate;

        public static T Get()
        {
            if (!Valid() && !_inCreate)
            {
                Create();
            }

            return _instance;
        }

        public static void Create()
        {
            if (!Valid())
            {
                _inCreate = true;
                var type = typeof(T);
                var constructor = type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[0], new ParameterModifier[0]);
                _instance = (T)constructor.Invoke(new object[0]);
                SingletonPool.Add<T>(_instance);
                _inCreate = false;
            }
        }

        public static void Destroy()
        {
            if (Valid())
            {
                _instance.Dispose();
                SingletonPool.Remove<T>();
                _instance = null;
            }
        }

        public static bool Valid()
        {
            return _instance != null;
        }
    }

    public static class MonoSingleton<T> where T : MonoBehaviour
    {
        private static T _instance;

        public static T Get()
        {
            if (_instance == null)
            {
                _instance = UnityEngine.Object.FindObjectOfType<T>();
            }

            return _instance;
        }

        public static bool Valid()
        {
            var instance = Get();
            return instance != null;
        }

        public static void Set(T instance)
        {
            _instance = instance;
        }
    }
}