using System;
using System.Linq;
using UnityEngine;

namespace RedSaw.MissionSystem
{
    public static class Utils
    {
        /// <summary>get default name of action</summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public static bool FetchAttribute<T>(this object action, out T attr) where T : System.Attribute
        {
            var type = action.GetType();
            var attrs = type.GetCustomAttributes(typeof(T), true);
            attr = attrs.Length > 0 ? (T)attrs[0] : null;
            return attr != null;
        }

        /// <summary>reset all [SeralizedField] field to defalut value</summary>
        public static void ResetObject<T>(T obj) where T : class
        {
            if(obj is null) return;

            /* get all fields */
            var type = obj.GetType();
            var fields = type.GetFields(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            /* reset all [SerializeField] field to default value */
            foreach (var field in fields.Where(x => x.IsDefined(typeof(SerializeField), true)))
                field.SetValue(obj, field.FieldType.IsValueType ? Activator.CreateInstance(field.FieldType) : null);
        }

        /// <summary>copy object simple implementation</summary>
        public static T CopyObject<T>(T obj) where T : class
        {
            if (obj is null) return null;
            if (obj is string) return obj;
            if (obj.GetType().IsAbstract) return null;

            /* get all fields */
            var type = obj.GetType();
            var fields = type.GetFields(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            /* create new instance */
            var newObj = Activator.CreateInstance(type);

            /* copy all [SerializeField] field to new object */
            foreach (var field in fields.Where(x => x.IsDefined(typeof(SerializeField), true)))
                field.SetValue(newObj, field.GetValue(obj));

            return newObj as T;
        }

        /// <summary>copy object datas from other object</summary>
        public static void CopyObjectFrom<T>(T self, T other) where T : class
        {
            if (self is null || other is null) return;
            if (self.GetType() != other.GetType()) return;

            /* get all fields */
            var type = self.GetType();
            var fields = type.GetFields(
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            /* copy all [SerializeField] field to new object */
            foreach (var field in fields.Where(x => x.IsDefined(typeof(SerializeField), true)))
                field.SetValue(self, field.GetValue(other));
        }
    }
}