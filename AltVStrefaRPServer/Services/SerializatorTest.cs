using AltV.Net;
using MessagePack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace AltVStrefaRPServer.Services
{
    public class SerializatorTest
    {
        public TestObject TestObject { get; set; }
        public JsonTestClass Json { get; set; }
        public MessagePackStringClass MessageString { get; set; }

        public SerializatorTest()
        {
            GenerateData();
        }

        private void GenerateData()
        {
            TestObject = new TestObject
            {
                Id = 10021231,
                Name = "Some way longer string dsaaaaaaaaaaaaaa77dsadsa7td7saagh12gi312i321b321h3h1b123h132",
                SomeCollection = Enumerable.Range(1, 100).ToList(),
                SomeDictionary = GetSomeDictionary()
            };
        }

        private Dictionary<int, string> GetSomeDictionary()
        {
            var returnDictionary = new Dictionary<int, string>();
            foreach (var i in Enumerable.Range(1, 100))
            {
                returnDictionary.Add(i, $"item:{i}");
            }
            return returnDictionary;
        }

        public void CalculateSize(object objectToCalucalate)
        {
            int size = GetSizeOfObject(objectToCalucalate);
            Console.WriteLine($"Size of the object is {size}");
        }

        /// <summary>
        /// Gets the size of object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="avgStringSize">Average size of the string.</param>
        /// <returns>An approximation of the size of the object in bytes</returns>
        public static int GetSizeOfObject(object obj, int avgStringSize = -1)
        {
            int pointerSize = IntPtr.Size;
            int size = 0;
            Type type = obj.GetType();
            var info = type.GetFields(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
            foreach (var field in info)
            {
                if (field.FieldType.IsValueType)
                {
                    size += Marshal.SizeOf(field.FieldType);
                }
                else
                {
                    size += pointerSize;
                    if (field.FieldType.IsArray)
                    {
                        var array = field.GetValue(obj) as Array;
                        if (array != null)
                        {
                            var elementType = array.GetType().GetElementType();
                            if (elementType.IsValueType)
                            {
                                size += Marshal.SizeOf(field.FieldType) * array.Length;
                            }
                            else
                            {
                                size += pointerSize * array.Length;
                                if (elementType == typeof(string) && avgStringSize > 0)
                                {
                                    size += avgStringSize * array.Length;
                                }
                            }
                        }
                    }
                    else if (field.FieldType == typeof(string) && avgStringSize > 0)
                    {
                        size += avgStringSize;
                    }
                }
            }
            return size;
        }

        public void ConvertToJson(TestObject testObject)
        {
            Json = new JsonTestClass();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Json.Json = JsonConvert.SerializeObject(testObject);
            stopwatch.Stop();
            Console.WriteLine($"Converted to JSON in {stopwatch.Elapsed}");
        }

        public void ConvertToMessagePack(TestObject testObject)
        {
            MessageString = new MessagePackStringClass();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            MessageString.MessagePack = MessagePackSerializer.Serialize(testObject);
            stopwatch.Stop();
            Console.WriteLine($"Converted to MessagePack in {stopwatch.Elapsed}");
        }
    }

    //[MessagePackObject(keyAsPropertyName: true)]
    [MessagePackObject]
    public class TestObject : IWritable
    {
        [Key(0)]
        public int Id { get; set; }
        [Key(1)]
        public string Name { get; set; }
        [Key(2)]
        public List<int> SomeCollection { get; set; }
        [Key(3)]
        public Dictionary<int, string> SomeDictionary { get; set; }

        public void OnWrite(IMValueWriter writer)
        {
            writer.BeginObject();

            writer.Name("id");
            writer.Value(Id);

            writer.Name("name");
            writer.Value(Name);

            writer.Name("someCollection");
            writer.BeginArray();
            foreach (var i in SomeCollection)
            {
                writer.Value(i);
            }
            writer.EndArray();

            writer.EndObject();
        }
    }

    public class JsonTestClass
    {
        public string Json { get; set; }
    }

    public class MessagePackStringClass
    {
        public byte[] MessagePack { get; set; }
    }
}
