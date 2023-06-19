using GPS_Project.Models;
using Shiny.Locations;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace GPS_Project.Helper
{
    public static class Utils
    {
        public static SQLiteAsyncConnection db;
        public static readonly string SERVICE_STATUS_KEY = "service_status";
        public static double Seconds { get; set; }
        public static IGpsManager Manager { get; set; }
        public static bool IsDistanceMode { get; set; }
        public static int Meters { get; set; }
       public static bool IsTracking { get; set; }
        public static GeolocationAccuracy GeolocationAccuracy { get; set; }

        public static async Task<SQLiteAsyncConnection> Init()
        {
            try
            {
                if (db != null)
                    return db;
                // Get an absolute path to the database file
                var databasePath = Path.Combine(FileSystem.AppDataDirectory, "GpsProject.db");

                db = new SQLiteAsyncConnection(databasePath);
               
                 if (!Utils.TableExists("GpsModel", db))
                await db.CreateTableAsync<GpsModel>();
                return db;
            }
            catch (Exception ex)
            {
               
            }
            return null;
        }
        public static bool TableExists(string tableName, SQLiteAsyncConnection connection)
        {
            try
            {
                var tableInfo = connection.GetConnection().GetTableInfo(tableName);
                if (tableInfo.Count > 0)
                    return true;
                else
                    return false;
            }
            catch (Exception ex) { return false; }
        }
    }
}
