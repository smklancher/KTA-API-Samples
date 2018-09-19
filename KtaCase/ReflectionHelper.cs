using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KtaCase
{
    public static class  ReflectionHelper
    {
        public static string PropertyList(object obj)
        {
            return InternalPropertyList(obj, null);
        }
        public static string StaticPropertyList(Type T)
        {
            return InternalPropertyList(null, T);
        }

        private static string InternalPropertyList(object obj, Type T)
        {
            if (obj != null)
            {
                T = obj.GetType();
            }

            if (T == null) { return string.Empty; }

            var props = T.GetProperties();
            var sb = new StringBuilder();
            foreach (var p in props)
            {
                try
                {
                    object value=new object();
                    if (p.IsStatic())
                    {
                        value = p.GetValue(null);
                    } else {
                        if (obj == null)
                        {
                            continue;
                        }
                        if (p.GetIndexParameters().Length == 0)
                        {
                            value = p.GetValue(obj, null);
                        }
                    }

                    if (p.PropertyType !=typeof(string) && //don't treat string as IEnumerable
                        p.PropertyType.FindInterfaces((t, c) => t == typeof(IEnumerable), null).Length > 0)
                    {
                        var ienum = (IEnumerable)value;
                        sb.AppendLine($"{p.Name}: Collection of {p.PropertyType.ToString()}");
                        foreach (object item in ienum)
                        {
                            sb.AppendLine($"\tFirst item: {item.ToString()}");
                            break;
                        }
                    }
                    else if (p.GetIndexParameters().Length > 0)
                    {
                        sb.AppendLine($"{p.Name}: Collection of {p.PropertyType.ToString()}");
                        sb.AppendLine($"\tFirst item: {p.GetValue(obj, new object[] { 0 }).ToString()}");
                    }
                    else
                    {
                        sb.AppendLine(p.Name + ": " + p.GetValue(obj, null));
                    }
                        
                }
                catch (Exception ex)
                {
                    Debug.Print($"Error accessing {(p.IsStatic() ? "static" : "instance")} property of {T.ToString()}.{p.Name}: {ex.Message}");
                }
            }
            return sb.ToString();

            //for similar recursive approach see https://stackoverflow.com/a/4269473/221018
        }

        //public static string StaticPropertyList(Type T)
        //{
        //    var props = T.GetProperties();
        //    var sb = new StringBuilder();
        //    foreach (var p in props)
        //    {
        //        try
        //        {
        //            sb.AppendLine(p.Name + ": " + p.GetValue(null));
        //        }
        //        catch (Exception ex)
        //        {
        //            Debug.Print($"Error accessing static property of {T.ToString()}.{p.Name}: {ex.Message}");
        //        }
        //    }
        //    return sb.ToString();
        //}
        
    }

    public static class PropertyInfoExtensions
    {
        public static bool IsStatic(this PropertyInfo source, bool nonPublic = false)
            => source.GetAccessors(nonPublic).Any(x => x.IsStatic);
    }
}
