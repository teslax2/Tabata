using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace Tabata.ViewModel
{
    internal static class Serializer
    {
        private static readonly string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "tabataTemp.txt");

        public static bool Serialize<T>(T objectToSerialize)
        {
            try
            {                
                var serializedObject = JsonConvert.SerializeObject(objectToSerialize);
                File.WriteAllText(path, serializedObject);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to serialize settings! - {ex}");
                return false;
            }

        }

        public static T Deserialize<T>()
        {
            if (!File.Exists(path))
                return default(T);

            try
            {
                var serializedObject = File.ReadAllText(path);
                var deserializedObject = JsonConvert.DeserializeObject<T>(serializedObject);
                return deserializedObject;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to deserialize settings! - {ex}");
                return default(T);
            }

        }
    }
}
