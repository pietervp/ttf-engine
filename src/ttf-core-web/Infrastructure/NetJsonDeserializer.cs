﻿using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using Nancy.Extensions;
using Nancy.ModelBinding;
using Nancy.Responses.Negotiation;

namespace Ttf.Server.Core.Web.Infrastructure
{
    public class NetJsonBodyDeserializer : IBodyDeserializer
    {
        private static readonly ConcurrentDictionary<Type, BindingMemberInfo[]> CachedBindingMemberInfo = new ConcurrentDictionary<Type, BindingMemberInfo[]>();
        private static readonly ConcurrentDictionary<Type, object> CachedDefaultValues = new ConcurrentDictionary<Type, object>();

        #region Implementation of IBodyDeserializer

        /// <summary>
        /// Whether the deserializer can deserialize the content type
        /// </summary>
        /// <param name="mediaRange">Content type to deserialize</param><param name="context">Current <see cref="T:Nancy.ModelBinding.BindingContext"/>.</param>
        /// <returns>
        /// True if supported, false otherwise
        /// </returns>
        public bool CanDeserialize(MediaRange mediaRange, BindingContext context)
        {
            return Helpers.IsJsonType($"{mediaRange.Type}/{mediaRange.Subtype}");
        }

        public object Deserialize(MediaRange mediaRange, Stream bodyStream, BindingContext context)
        {
            return Deserialize("application/json", bodyStream, context);
        }

        /// <summary>
        /// Deserialize the request body to a model
        /// </summary>
        /// <param name="contentType">Content type to deserialize</param><param name="bodyStream">Request body stream</param><param name="context">Current <see cref="T:Nancy.ModelBinding.BindingContext"/>.</param>
        /// <returns>
        /// Model instance
        /// </returns>
        public object Deserialize(string contentType, Stream bodyStream, BindingContext context)
        {
            object deserializedObject;
            using (var inputStream = new StreamReader(bodyStream))
            {
                // deserialize json
                deserializedObject = NetJSON.NetJSON.Deserialize(context.DestinationType, inputStream.ReadToEnd());

                // .. then, due to NancyFx's support for blacklisted properties, we need to get the propertyInfo first (read from cache if possible)
                var comparisonType = GetTypeForBlacklistComparison(context.DestinationType);

                BindingMemberInfo[] bindingMemberInfo;
                if (CachedBindingMemberInfo.TryGetValue(comparisonType, out bindingMemberInfo) == false)
                {
                    bindingMemberInfo = comparisonType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => new BindingMemberInfo(p)).ToArray();
                    // the following is somewhat dirty but oh well
                    SpinWait.SpinUntil(() => CachedBindingMemberInfo.ContainsKey(comparisonType) || CachedBindingMemberInfo.TryAdd(comparisonType, bindingMemberInfo));
                }

                // ... and then compare whether there's anything blacklisted
                if (bindingMemberInfo.Except(context.ValidModelBindingMembers).Any())
                {
                    // .. if so, take object and basically eradicated value(s) for the blacklisted properties.
                    // this is inspired by https://raw.githubusercontent.com/NancyFx/Nancy.Serialization.JsonNet/master/src/Nancy.Serialization.JsonNet/JsonNetBodyDeserializer.cs
                    // but again.. only *inspired*.
                    // The main difference is, that the instance Jil returned from the JSON.Deserialize() call will be wiped clean, no second/new instance will be created.
                    return CleanBlacklistedMembers(context, deserializedObject, bindingMemberInfo);
                }

                return deserializedObject;
            }
        }

        #endregion

        private static Type GetTypeForBlacklistComparison(Type destinationType)
        {
            // check whether the type is an array and if so, return its Element Type (the type that is being.. well.. array / []'ed)
            if (destinationType.IsArray)
                return destinationType.GetElementType();

            if (destinationType.IsCollection() && destinationType.IsConstructedGenericType)
                return destinationType.GetGenericArguments().FirstOrDefault() ?? destinationType;

            else
                return destinationType;
        }

        /// <summary>
        /// Cleans the blacklisted properties from the <see cref="deserializedObject"/>.
        /// If it is a an <see cref="IEnumerable"/>, the blacklisted properties from each element are cleaned.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="deserializedObject">The deserialized object.</param>
        /// <param name="cachedBindingMemberInfo">The cached property information.</param>
        /// <returns></returns>
        private static object CleanBlacklistedMembers(BindingContext context, object deserializedObject, BindingMemberInfo[] cachedBindingMemberInfo)
        {
            if (context.DestinationType.IsCollection())
            {
                foreach (var enumerableElement in (IEnumerable)deserializedObject)
                {
                    CleanPropertyValues(context, enumerableElement, cachedBindingMemberInfo);
                }
            }
            else
            {
                CleanPropertyValues(context, deserializedObject, cachedBindingMemberInfo);
            }

            return deserializedObject;
        }

        /// <summary>
        /// Cleans the property values for the provided <see cref="targetObject"/>.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="targetObject">The target object.</param>
        /// <param name="cachedBindingMemberInfo">The cached property information.</param>
        private static void CleanPropertyValues(BindingContext context, object targetObject, IEnumerable<BindingMemberInfo> cachedBindingMemberInfo)
        {
            foreach (var blacklistedProperty in cachedBindingMemberInfo.Except(context.ValidModelBindingMembers))
            {
                blacklistedProperty.SetValue(targetObject, GetDefaultForType(blacklistedProperty.PropertyType));
            }
        }

        /// <summary>
        /// Gets the default for the provided <see cref="targetType"/>.
        /// (see http://stackoverflow.com/questions/325426/programmatic-equivalent-of-defaulttype)
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns></returns>
        public static object GetDefaultForType(Type targetType)
        {
            object defaultValue;
            if (CachedDefaultValues.TryGetValue(targetType, out defaultValue) == false)
            {
                defaultValue = Activator.CreateInstance(targetType);

                // the following is somewhat dirty .. again.. but oh well.. again
                SpinWait.SpinUntil(() => CachedDefaultValues.ContainsKey(targetType) || CachedDefaultValues.TryAdd(targetType, defaultValue));
            }

            return defaultValue;
        }
    }
}