﻿// Copyright (c) 2018 Ark S.r.l. All rights reserved.
// Licensed under the MIT License. See LICENSE file for license information. 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Ark.Tools.Core
{
    /// <summary>
    /// Reflection utilities
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Get the `Attribute` object of the specified type associated with a member.
        /// </summary>
        /// <typeparam name="TAttribute">Type of attribute to get.</typeparam>
        /// <param name="memberInfo">The member to look for the attribute on.</param>
        public static TAttribute GetAttribute<TAttribute>(MemberInfo memberInfo)
        {
            var attributes = from a in memberInfo.GetCustomAttributes(true)
                             where a is TAttribute
                             select a;

            return (TAttribute)attributes.FirstOrDefault();
        }

        /// <summary>
        /// Get the `Attribute` object of the specified type associated with a class.
        /// </summary>
        /// <typeparam name="TAttribute">Type of attribute to get.</typeparam>
        /// <param name="type">The class to look for the attribute on.</param>
        public static TAttribute GetAttribute<TAttribute>(Type type)
        {
            var attributes = from a in type.GetCustomAttributes(true)
                             where a is TAttribute
                             select a;

            return (TAttribute)attributes.FirstOrDefault();
        }


        /// <summary>
        /// Get the item type of a type that implements `IEnumerable`.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetEnumerableItemType(Type type)
        {
            // If the type passed IS the interface type, success!
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return type.GetGenericArguments()[0];

            // Otherwise, loop through the interfaces until we find IEnumerable (if it exists).
            Type[] interfaces = type.GetInterfaces();
            foreach (Type i in interfaces)
            {
                if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return i.GetGenericArguments()[0];
                }
            }

            return null;
        }

        /// <summary>
        /// Get a field or property value from an object.
        /// </summary>
        /// <param name="obj">The object whose property we want.</param>
        /// <param name="name">The name of the field or property we want.</param>
        public static object GetFieldOrPropertyValue(object obj, string name)
        {
            if (obj == null) return null;

            var type = obj.GetType();
            var member = type.GetField(name) ?? type.GetProperty(name) as MemberInfo;

            if (member == null) return null;

            object value;

            switch (member.MemberType)
            {
                case MemberTypes.Property:
                    value = ((PropertyInfo)member).GetValue(obj, null);
                    break;
                case MemberTypes.Field:
                    value = ((FieldInfo)member).GetValue(obj);
                    break;
                default:
                    value = null;
                    break;
            }

            return value;
        }

        /// <summary>
        /// Get a field or property value from an object.
        /// </summary>
        /// <param name="obj">The object whose property we want.</param>
        /// <param name="name">The name of the field or property we want.</param>
        public static T GetFieldOrPropertyValue<T>(object obj, string name)
        {
            if (obj == null) return default(T);

            var type = obj.GetType();
            var member = type.GetField(name) ?? type.GetProperty(name) as MemberInfo;

            if (member == null) return default(T);

            var value = GetFieldOrPropertyValue(obj, name);

            return (T)value;
        }

        /// <summary>
        /// Return the name of the type as it is written in CS code
        /// </summary>
        /// <param name="type">the type</param>
        /// <returns></returns>
        public static string GetCSTypeName(this Type type)
        {
            if (type == typeof(string))
            {
                return "string";
            }
            else if (type == typeof(object)) { return "object"; }
            else if (type == typeof(bool)) { return "bool"; }
            else if (type == typeof(char)) { return "char"; }
            else if (type == typeof(int)) { return "int"; }
            else if (type == typeof(float)) { return "float"; }
            else if (type == typeof(double)) { return "double"; }
            else if (type == typeof(long)) { return "long"; }
            else if (type == typeof(ulong)) { return "ulong"; }
            else if (type == typeof(uint)) { return "uint"; }
            else if (type == typeof(byte)) { return "byte"; }
            else if (type == typeof(Int64)) { return "Int64"; }
            else if (type == typeof(short)) { return "short"; }
            else if (type == typeof(decimal)) { return "decimal"; }
            else if (type.IsGenericType)
            {
                return $"{ToGenericTypeString(type)}";
            }
            else if (type.IsArray)
            {
                List<string> arrayLength = new List<string>();
                for (int i = 0; i < type.GetArrayRank(); i++)
                {
                    arrayLength.Add("[]");
                }
                return GetCSTypeName(type.GetElementType()) + string.Join("", arrayLength).Replace("+", ".");
            }
            else
            {
                return type.FullName.Replace("+", ".");
            }
        }

        private static string ToCSReservatedWord(this Type type, bool fullName)
        {
            if (type == typeof(string))
            {
                return "string";
            }
            else if (type == typeof(object)) { return "object"; }
            else if (type == typeof(bool)) { return "bool"; }
            else if (type == typeof(char)) { return "char"; }
            else if (type == typeof(int)) { return "int"; }
            else if (type == typeof(float)) { return "float"; }
            else if (type == typeof(double)) { return "double"; }
            else if (type == typeof(long)) { return "long"; }
            else if (type == typeof(ulong)) { return "ulong"; }
            else if (type == typeof(uint)) { return "uint"; }
            else if (type == typeof(byte)) { return "byte"; }
            else if (type == typeof(Int64)) { return "Int64"; }
            else if (type == typeof(short)) { return "short"; }
            else if (type == typeof(decimal)) { return "decimal"; }
            else
            {
                if (fullName)
                {
                    return type.FullName;
                }
                else
                {
                    return type.Name;
                }

            }
        }

        private static string ToGenericTypeString(this Type t, params Type[] arg)
        {
            if (t.IsGenericParameter || t.FullName == null) return t.FullName;//Generic argument stub
            bool isGeneric = t.IsGenericType || t.FullName.IndexOf('`') >= 0;//an array of generic types is not considered a generic type although it still have the genetic notation
            bool isArray = !t.IsGenericType && t.FullName.IndexOf('`') >= 0;
            Type genericType = t;
            while (genericType.IsNested && genericType.DeclaringType.GetGenericArguments().Count() == t.GetGenericArguments().Count())//Non generic class in a generic class is also considered in Type as being generic
            {
                genericType = genericType.DeclaringType;
            }
            if (!isGeneric) return ToCSReservatedWord(t, true).Replace('+', '.');

            var arguments = arg.Any() ? arg : t.GetGenericArguments();//if arg has any then we are in the recursive part, note that we always must take arguments from t, since only t (the last one) will actually have the constructed type arguments and all others will just contain the generic parameters
            string genericTypeName = genericType.ToCSReservatedWord(true);
            if (genericType.IsNested)
            {
                var argumentsToPass = arguments.Take(genericType.DeclaringType.GetGenericArguments().Count()).ToArray();//Only the innermost will return the actual object and only from the GetGenericArguments directly on the type, not on the on genericDfintion, and only when all parameters including of the innermost are set
                arguments = arguments.Skip(argumentsToPass.Count()).ToArray();
                genericTypeName = genericType.DeclaringType.ToGenericTypeString(argumentsToPass) + "." + ToCSReservatedWord(genericType, false);//Recursive
            }
            if (isArray)
            {
                genericTypeName = t.GetElementType().ToGenericTypeString() + "[]";//this should work even for multidimensional arrays
            }
            if (genericTypeName.IndexOf('`') >= 0)
            {
                genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));
                string genericArgs = string.Join(", ", arguments.Select(a => a.ToGenericTypeString()).ToArray());
                //Recursive
                genericTypeName = genericTypeName + "<" + genericArgs + ">";
                if (isArray) genericTypeName += "[]";
            }
            if (t != genericType)
            {
                genericTypeName += t.FullName.Replace(genericType.ToCSReservatedWord(true), "").Replace('+', '.');
            }
            if (genericTypeName.IndexOf("[") >= 0 && genericTypeName.IndexOf("]") != genericTypeName.IndexOf("[") + 1) genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf("["));//For a non generic class nested in a generic class we will still have the type parameters at the end 
            return genericTypeName;
        }
    }
}
