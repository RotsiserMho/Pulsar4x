﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Pulsar4X.ECSLib
{
    /// <summary>
    /// The following is a special dictionay. It should be used whenever you have a dictionary 
    /// that will be serialized/deserialized by json.
    /// This is because by default JSON.net will save the key as a string only or will fail to deserialized, 
    /// this Dictionay forces it to save the full type (if it is a custom type) or to serilize as a Json array of key/value pairs.
    /// </summary>
    [JsonArrayAttribute]
    public class JDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        /// <summary>
        /// Initializes a new instance of Pulsar4X.ECSLib.JDictionary class that is empty, has the default initial capacity, and uses the default equality comparer for the key type.
        /// </summary>
        public JDictionary()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Pulsar4X.ESCLib.JDictionary class that contains elements copied from the specified System.Collections.Generic.IDictionary and uses the default equality comparer for the key type.
        /// </summary>
        public JDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
        }

        /// <summary>
        /// Deserializes the dictionary from a json list.
        /// </summary>
        [UsedImplicitly]
        public JDictionary(SerializationInfo info, StreamingContext context)
        {
            var list = (List<KeyValuePair<TKey, TValue>>)info.GetValue("List", typeof(List<KeyValuePair<TKey, TValue>>));

            foreach (KeyValuePair<TKey, TValue> item in list)
            {
                Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Serializes the Dictionary to a json list.
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("List", this.ToList());
        }
    }

    public static class JDictionaryExtension
    {
        [PublicAPI]
        public static void SafeValueAdd<TKey>(this JDictionary<TKey, int> jDict, TKey key, int toAdd)
        {
            if(!jDict.ContainsKey(key))
                jDict.Add(key, toAdd);
            else
            {
                jDict[key] += toAdd;
            }
        }

        [PublicAPI]
        public static void SafeValueAdd<TKey>(this JDictionary<TKey, float> jDict, TKey key, float toAdd)
        {
            if (!jDict.ContainsKey(key))
                jDict.Add(key, toAdd);
            else
            {
                jDict[key] += toAdd;
            }
        }

        [PublicAPI]
        public static void SafeValueAdd<TKey>(this JDictionary<TKey, double> jDict, TKey key, double toAdd)
        {
            if (!jDict.ContainsKey(key))
                jDict.Add(key, toAdd);
            else
            {
                jDict[key] += toAdd;
            }
        }
    }

    /// <summary>
    /// This class can be used by a Json.net serializer to get around the problem of it ignoreing ISerializable in favor of IEnumarble.
    /// </summary>
    public class ForceUseISerializable : DefaultContractResolver
    {
        protected override JsonContract CreateContract(Type objectType)
        {
            // make sure we use default Json datTime converter
            // otherwise import will fail.
            if (typeof(DateTime).IsAssignableFrom(objectType))  
            {
                return base.CreateContract(objectType);
            }

            // now we can force iserialible to be used.
            if (typeof(ISerializable).IsAssignableFrom(objectType))
            {
                return CreateISerializableContract(objectType);
            }

            // if we don't match any of the above just use the default.
            return base.CreateContract(objectType);
        }
    }
}