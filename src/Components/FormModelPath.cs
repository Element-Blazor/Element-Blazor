using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Element
{
    internal static class FormModelPath
    {
        internal static bool TryGetValue(object model, string path, out object value)
        {
            value = null;
            if (model == null || string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            object current = model;
            foreach (var segment in Split(path))
            {
                if (!TryGetSegmentValue(current, segment, out current))
                {
                    return false;
                }
            }

            value = current;
            return true;
        }

        internal static bool TrySetValue(object model, string path, object value, Func<object, Type, object> converter)
        {
            if (!TryResolveTarget(model, path, createMissingParents: true, out var target, out var segment, out var targetType))
            {
                return false;
            }

            if (target is IDictionary<string, object> objectDictionary)
            {
                objectDictionary[segment] = value;
                return true;
            }

            if (target is IDictionary stringDictionary)
            {
                stringDictionary[segment] = value;
                return true;
            }

            if (target is IList list && int.TryParse(segment, out var index))
            {
                if (index < 0 || index >= list.Count)
                {
                    return false;
                }

                list[index] = value;
                return true;
            }

            if (targetType == null)
            {
                return false;
            }

            var convertedValue = converter == null ? value : converter(value, targetType);
            var property = GetProperty(target.GetType(), segment);
            if (property == null || !property.CanWrite)
            {
                return false;
            }

            property.SetValue(target, convertedValue);
            return true;
        }

        internal static bool TryGetPropertyInfo(Type modelType, string path, out PropertyInfo property)
        {
            property = null;
            if (modelType == null || string.IsNullOrWhiteSpace(path))
            {
                return false;
            }

            var currentType = modelType;
            foreach (var segment in Split(path))
            {
                if (int.TryParse(segment, out _))
                {
                    currentType = GetEnumerableElementType(currentType);
                    if (currentType == null)
                    {
                        return false;
                    }
                    continue;
                }

                property = GetProperty(currentType, segment);
                if (property == null)
                {
                    return false;
                }

                currentType = property.PropertyType;
            }

            return property != null;
        }

        internal static bool TryGetParentObject(object model, string path, out object parent)
        {
            parent = null;
            if (!TryResolveTarget(model, path, createMissingParents: false, out parent, out _, out _))
            {
                return false;
            }

            return true;
        }

        private static bool TryResolveTarget(object model, string path, bool createMissingParents, out object target, out string finalSegment, out Type targetType)
        {
            target = model;
            finalSegment = null;
            targetType = null;
            var segments = Split(path).ToArray();
            if (model == null || segments.Length == 0)
            {
                return false;
            }

            for (var i = 0; i < segments.Length - 1; i++)
            {
                var segment = segments[i];
                var nextSegment = segments[i + 1];
                if (target == null)
                {
                    return false;
                }

                if (TryGetSegmentValue(target, segment, out var next))
                {
                    if (next == null && createMissingParents && !int.TryParse(nextSegment, out _) && TryCreateNestedProperty(target, segment, out next))
                    {
                        target = next;
                        continue;
                    }

                    target = next;
                    continue;
                }

                if (createMissingParents && TryCreateNestedProperty(target, segment, out next))
                {
                    target = next;
                    continue;
                }

                return false;
            }

            finalSegment = segments[^1];
            targetType = ResolveSegmentType(target, finalSegment);
            return target != null;
        }

        private static bool TryGetSegmentValue(object source, string segment, out object value)
        {
            value = null;
            if (source == null)
            {
                return false;
            }

            if (source is IDictionary<string, object> objectDictionary)
            {
                return objectDictionary.TryGetValue(segment, out value);
            }

            if (source is IDictionary stringDictionary)
            {
                if (!stringDictionary.Contains(segment))
                {
                    return false;
                }

                value = stringDictionary[segment];
                return true;
            }

            if (source is IList list && int.TryParse(segment, out var index))
            {
                if (index < 0 || index >= list.Count)
                {
                    return false;
                }

                value = list[index];
                return true;
            }

            var property = GetProperty(source.GetType(), segment);
            if (property == null || !property.CanRead)
            {
                return false;
            }

            value = property.GetValue(source);
            return true;
        }

        private static bool TryCreateNestedProperty(object source, string segment, out object value)
        {
            value = null;
            var property = GetProperty(source.GetType(), segment);
            if (property == null || !property.CanRead || !property.CanWrite)
            {
                return false;
            }

            var type = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
            if (type.IsValueType || type == typeof(string) || type.GetConstructor(Type.EmptyTypes) == null)
            {
                return false;
            }

            value = Activator.CreateInstance(type);
            property.SetValue(source, value);
            return true;
        }

        private static Type ResolveSegmentType(object target, string segment)
        {
            if (target == null)
            {
                return null;
            }

            if (target is IDictionary<string, object> || target is IDictionary)
            {
                return null;
            }

            if (target is IList list && int.TryParse(segment, out _))
            {
                return GetEnumerableElementType(target.GetType());
            }

            return GetProperty(target.GetType(), segment)?.PropertyType;
        }

        private static PropertyInfo GetProperty(Type type, string name)
        {
            if (type == null || string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            return type.GetProperty(name, BindingFlags.Public | BindingFlags.Instance)
                ?? type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        private static IEnumerable<string> Split(string path)
        {
            return path.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x));
        }

        private static Type GetEnumerableElementType(Type type)
        {
            if (type == null || type == typeof(string))
            {
                return null;
            }

            if (type.IsArray)
            {
                return type.GetElementType();
            }

            return type.GetInterfaces()
                .Concat(new[] { type })
                .Where(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(x => x.GetGenericArguments()[0])
                .FirstOrDefault();
        }
    }
}
