using ForzaAnalytics.Models.Core;
using System.Text.Json;

namespace ForzaAnalytics.Services.Serializers
{
    public static class MapSerializer
    {
        public static void PersistMap(string path, GroupedPositionalData data)
        {
            string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        public static GroupedPositionalData LoadMap(string path)
        {
            try
            {
                return JsonSerializer.Deserialize<GroupedPositionalData>(File.ReadAllText(path)) ?? new GroupedPositionalData();
            }
            catch (Exception)
            {
                return new GroupedPositionalData();
            }
        }
        public static void ExportPositionData(string path, GroupedExtendedPositionalData data)
        {
            string jsonString = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(path, jsonString);
        }
        public static GroupedExtendedPositionalData LoadPositionData(string path)
        {
            try
            {
                return JsonSerializer.Deserialize<GroupedExtendedPositionalData>(File.ReadAllText(path)) ?? new GroupedExtendedPositionalData();
            }
            catch (Exception)
            {
                return new GroupedExtendedPositionalData();
            }
        }
    }
}
