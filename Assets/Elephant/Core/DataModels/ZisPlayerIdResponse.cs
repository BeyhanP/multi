using System;
using UnityEngine;

namespace ElephantSDK
{
    [Serializable]
    public class ZisPlayerIdResponse
    {
        private const string ZisPlayerId = "ZIS_PLAYER_ID_OBJ";
        public string player_id;
        public string app_id;
        public string zis_json;

        public ZisPlayerIdResponse()
        {
            player_id = "";
            app_id = "";
            zis_json = "";
        }

        public static void Save(ZisPlayerIdResponse zisPlayer)
        {
            var zisPlayerJsonString = JsonUtility.ToJson(zisPlayer);
            Utils.SaveToFile(ZisPlayerId, zisPlayerJsonString);
        }

        public static ZisPlayerIdResponse Get()
        {
            var zisPlayerJsonString = Utils.ReadFromFile(ZisPlayerId) ?? "";
            if (string.IsNullOrEmpty(zisPlayerJsonString)) return new ZisPlayerIdResponse();

            var zisPlayer = JsonUtility.FromJson<ZisPlayerIdResponse>(zisPlayerJsonString);
            return zisPlayer;
        }

        public string GetDataJsonRepresentation()
        {
            return zis_json;
        }

        public static void Flush()
        {
            Utils.SaveToFile(ZisPlayerId, "");
        }
    }
}