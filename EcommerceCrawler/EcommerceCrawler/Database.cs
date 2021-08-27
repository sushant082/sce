using Npgsql;
using System;
using System.Data;
using EcommerceCrawler.Log;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace EcommerceCrawler
{
    public abstract class Database
    {
        public abstract IDbConnection Connect();
        public abstract int Execute(string sql, CommandType cmdType);
        public abstract int ExecuteScalar(string sql, CommandType cmdType);
        public abstract IDataReader ExecuteReader(string sql, CommandType cmdType);
    }
    public class NpgsqlDatabase : Database, IDisposable
    {
        IDbConnection con;
        IDbCommand cmd;
        private static string _connectionString;
        public NpgsqlDatabase()
        {
            cmd = new NpgsqlCommand();
        }
        public NpgsqlDatabase(string connectionString)
        {
            _connectionString = connectionString;
            cmd = new NpgsqlCommand();
        }
        public override IDbConnection Connect()
        {
            try
            {
                con = new NpgsqlConnection(_connectionString);
                con.Open();
                if (con.State != ConnectionState.Open) con.Open();
                return con;
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex.ToString());
                LogTrace.WriteDebugLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.ToString());
            }
            return null;
        }
        public override int Execute(string sql, CommandType cmdType = CommandType.Text)
        {
            try
            {
                cmd.CommandText = sql;
                cmd.Connection = con;
                int result = cmd.ExecuteNonQuery();
                return result;
            }

            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex.ToString());
                LogTrace.WriteDebugLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.ToString());
                LogTrace.WriteDebugLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, cmd.CommandText);
            }
            return -1;
        }
        public override int ExecuteScalar(string sql, CommandType cmdType = CommandType.Text)
        {
            try
            {
                cmd.CommandText = sql;
                cmd.Connection = con;
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                return result;
            }

            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex.ToString());
                LogTrace.WriteDebugLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.ToString());
                LogTrace.WriteDebugLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, cmd.CommandText);
            }
            return -1;
        }
        public override IDataReader ExecuteReader(string sql, CommandType cmdType = CommandType.Text)
        {
            try
            {
                cmd.CommandText = sql;
                cmd.Connection = con;
                IDataReader dr = cmd.ExecuteReader();
                return dr;
            }
            catch (NpgsqlException ex)
            {
                Console.WriteLine(ex.ToString());
                LogTrace.WriteDebugLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, ex.ToString());
                LogTrace.WriteDebugLog(this.GetType().Name, MethodBase.GetCurrentMethod().Name, cmd.CommandText);
            }
            return null;
        }

        public void Dispose()
        {
            if (con.State == ConnectionState.Open)
                con.Close();
        }

        public static List<Setting> LoadConn()
        {
            using (StreamReader r = new StreamReader("connection_string.json"))
            {
                string json = r.ReadToEnd();
                List<Setting> items = JsonConvert.DeserializeObject<List<Setting>>(json);
                return items;
            }
        }
    }
}
