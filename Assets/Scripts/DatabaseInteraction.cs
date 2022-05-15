using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using UnityEditor;
using UnityEngine;
namespace DefaultNamespace
{
    [CreateAssetMenu(fileName = "Resources/ScriptableObjects", menuName = "Create Item", order = 0)]
    public class soItem : ScriptableObject
    {
        public int ItemID;
        public string ItemName;
        public int Strength;
    }

    /// <summary>
    /// This class is a placeholder for an Entity Framework class.
    /// </summary>
    public class tblItemsItem
    {
        public int ItemID;
        public string ItemName;
        public int Strength;
    }
    
    public class DatabaseInteraction : MonoBehaviour
    {
        [SerializeField] private static string dbPath = Application.streamingAssetsPath + "/ItemDatabase.db";
        [SerializeField] private static Uri dbUri;
        [SerializeField] private static string scriptableObjectLocation = "Assets/Resources/ScriptableObjects";
    
        public static void setupDB()
        {
            dbPath = Application.streamingAssetsPath + "/ItemDatabase.db";
            dbUri = new Uri(dbPath);
        }

        [MenuItem("Tools/Import Item Database")]
        public static void ClickCreateScriptableObjects()
        {
            DeleteExistingScriptableObjects();
            List<tblItemsItem> myItems = LoadAllItems();
            foreach (var currentItem in myItems)
            {
                CreateScriptableObject(currentItem);
            }
            AssetDatabase.SaveAssets();
        }
        private static List<tblItemsItem> LoadAllItems()
        {
            List<tblItemsItem> myItems = new List<tblItemsItem>();
            if (dbUri == null)
            {
                setupDB();
            }
            using SqliteConnection connection = new SqliteConnection("Data Source=" + dbUri.AbsolutePath);
            {
                connection.Open();
                using SqliteCommand command = connection.CreateCommand();
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT ItemID, ItemName, Strength FROM tblItems";
                    SqliteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        tblItemsItem newItem = new tblItemsItem();
                        newItem.ItemID = reader.GetInt32(0);
                        newItem.ItemName = reader.GetString(1);
                        newItem.Strength = reader.GetInt32(2);
                        myItems.Add(newItem);
                    }
                }
            }
            return myItems;
        }

        private static void CreateScriptableObject(tblItemsItem currentItem)
        {
            soItem newItem = ScriptableObject.CreateInstance<soItem>();
            newItem.ItemName = currentItem.ItemName;
            newItem.Strength = currentItem.Strength;
            newItem.ItemID = currentItem.ItemID;
            newItem.name = currentItem.ItemID + currentItem.ItemName;
            AssetDatabase.CreateAsset(newItem, scriptableObjectLocation + "/" + newItem.name + ".asset");
        }

        private static void DeleteExistingScriptableObjects()
        {
            if (Directory.Exists(scriptableObjectLocation))
            {
                Directory.Delete(scriptableObjectLocation, true);
            }
            Directory.CreateDirectory(scriptableObjectLocation);
        }


    }

}
