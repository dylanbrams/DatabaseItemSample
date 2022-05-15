using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using Mono.Data.Sqlite;
using UnityEditor;
using UnityEngine;

namespace PreDemo
{
    [CreateAssetMenu(fileName = "Resources/ScriptableObjects", menuName = "Create Item Old", order = 0)]
    public class soItemOld : ScriptableObject
    {
        public int ItemID;
        public string Name;
        public int Strength;
    }


    public class tblItemsItem
    {
        public int ItemID;
        public string Name;
        public int Strength;
    }

    public class DatabaseInteractionOld : MonoBehaviour
    {
        [SerializeField] private static string dbPath = Application.streamingAssetsPath + "/ItemDatabase.db";
        [SerializeField] private static Uri dbURI;
        [SerializeField] private static string scriptableObjectLocation = "Assets/Resources/ScriptableObjects";

        public static void Start()
        {
            setupDB();
        }

        private static void setupDB()
        {
            dbPath = Application.streamingAssetsPath + "/ItemDatabase.db";
            dbURI = new Uri(dbPath);
        }

        [MenuItem("Tools/Import Item Database Pre-Demo")]
        public static void ClickCreateScriptableObjects()
        {
            DeleteExistingScriptableObjects();
            List<tblItemsItem> myItems = LoadAllItems();
            foreach (tblItemsItem currentItem in myItems)
            {
                CreateScriptableObject(currentItem);
            }
            AssetDatabase.SaveAssets();
        }

        public static List<tblItemsItem> LoadAllItems()
        {
            if (dbURI == null)
            {
                setupDB();
            }
            List<tblItemsItem> myItems = new List<tblItemsItem>();
            using (SqliteConnection connection = new SqliteConnection("Data Source=" + dbURI.AbsolutePath))
            {
                connection.Open();

                using SqliteCommand command = connection.CreateCommand();
                {
                    command.CommandType = CommandType.Text;
                    command.CommandText = "SELECT ItemID, Name, Strength FROM tblItems";
                    SqliteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        tblItemsItem newItem = new tblItemsItem();
                        newItem.ItemID = reader.GetInt32(0);
                        newItem.Name = reader.GetString(1);
                        newItem.Strength = reader.GetInt32(2);
                        myItems.Add(newItem);
                    }
                }
            }
            return myItems;
        }

        private static void CreateScriptableObject(tblItemsItem currentItem)
        {
            soItemOld newItem = ScriptableObject.CreateInstance<soItemOld>();
            newItem.Name = currentItem.Name;
            newItem.Strength = currentItem.Strength;
            newItem.ItemID = currentItem.ItemID;
            newItem.name = currentItem.Name;
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