using SqlFramework.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SqlFramework.Concrete.SqlRepository
{
    public class SqlRepository<T> : ISqlRepository<T> where T : class, new()
    {

        private SqlConnection _connection;
        private T GetT { get; }
        private string TableName { get; set; }
        private string Key { get; set; }
        private string Value { get; set; }
        Dictionary<object, object> datas { get; set; }

        //private Dictionary<object, object> datas { get; set; }
        private List<string> KeyAttributes { get; set; }
        public SqlRepository(string _connection)
        {
            GetTableNameAttribute(new T());
            Key = "";
            Value = "";
            GetT = new T();
            this._connection = new SqlConnection(_connection);
        }
        public SqlRepository()
        {

        }
        private void GetTableNameAttribute(T tableClass)
        {
            datas = new Dictionary<object, object>();
            var attrs = tableClass.GetType().GetCustomAttributes(false);
            foreach (Attribute attr in attrs)
            {
                TableAttribute tAttr = attr as TableAttribute;
                if (tAttr != null)
                {
                    if (tAttr.Schema == "dbo")
                    {
                        TableName = tAttr.Name;
                    }
                }
            }
        }
        public void Insert(T data)
        {
            CreateAddQuery(data);
            try
            {
                _connection.Open();
                string cmd = string.Format(@"Insert Into {0} ({1}) Values ({2})", TableName, Key, Value);
                SqlCommand command = new SqlCommand(cmd, _connection);
                foreach (var item in datas)
                {
                    command.Parameters.AddWithValue("@" + (string)item.Key, item.Value);
                }
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("[Error Occurred]  Error Message : {0}", e.Message);
            }

            finally
            {
                _connection.Close();
            }
        }
        private void CreateAddQuery(T data)
        {
            Key = "";
            Value = "";
            datas = new Dictionary<object, object>();
            KeyAttributes = new List<string>();
            foreach (var prop in data.GetType().GetProperties())
            {
                foreach (object keyAttr in prop.GetCustomAttributes(true))
                {

                    KeyAttribute kAttr = keyAttr as KeyAttribute;
                    if (kAttr != null)
                    {
                        KeyAttributes.Add(prop.Name);
                    }
                }
            }

            foreach (var item in data.GetType().GetProperties())
            {

                datas.Add(item.Name, item.GetValue(data));
            }

            foreach (var item in datas)
            {
                if (KeyAttributes.Contains(item.Key))
                {
                    Key += "";
                }
                else
                {
                    Key += item.Key;
                    Key += ",";
                    Value += "@";
                    Value += item.Key;
                    Value += ",";
                }
            }
            if (Key.Length > 0)
            {
                Key = Key.Substring(0, Key.Length - 1);
                Value = Value.Substring(0, Value.Length - 1);
            }
        }

        public List<T> GetAll()
        {
            CreateSelectQuery();
            string cmd = string.Format("Select {0} from {1}", Key, TableName);
            SqlCommand command = new SqlCommand(cmd, _connection);
            List<T> entities = new List<T>();
            try
            {
                _connection.Open();
                SqlDataReader dr = command.ExecuteReader();
                PropertyInfo[] properties = GetT.GetType().GetProperties();
                Hashtable hashtable = new Hashtable();

                foreach (PropertyInfo propertyInfo in properties)
                {
                    hashtable.Add(propertyInfo.Name, propertyInfo);
                }
                while (dr.Read())
                {
                    T entity = new T();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        PropertyInfo info = (PropertyInfo)hashtable[dr.GetName(i)];
                        if (info != null)
                        {
                            info.SetValue(entity, dr.GetValue(i), null);
                        }
                    }
                    entities.Add(entity);
                }

            }
            catch (Exception e)
            {

                Console.WriteLine("[Error Occurred]  Error Message : {0}", e.Message);

            }
            finally
            {
                _connection.Close();

            }
            return entities;

        }

        public List<T> GetAll(Expression<Func<T, bool>> filter)
        {
            BinaryExpression binaryExp = (BinaryExpression)filter.Body;
            MemberExpression memberLeft = (MemberExpression)binaryExp.Left;
            ConstantExpression memberRight = (ConstantExpression)binaryExp.Right;
            PropertyInfo[] infos = GetT.GetType().GetProperties();
            Hashtable hashtable = new Hashtable();
            List<T> entities = new List<T>();
            foreach (PropertyInfo property in infos)
            {
                hashtable.Add(property.Name, property);
            }
            string cmdText = string.Format(@"Select * from {0} Where {1}={2}", TableName, memberLeft.Member.Name, memberRight);

            SqlCommand cmd = new SqlCommand(cmdText, _connection);
            cmd.Parameters.AddWithValue("@" + memberLeft.Member.Name, memberRight.Value);
            try
            {
                _connection.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    T entity = new T();
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        PropertyInfo prop = (PropertyInfo)hashtable[dr.GetName(i)];
                        if (prop != null)
                        {
                            prop.SetValue(entity, dr.GetValue(i), null);
                        }
                    }
                    entities.Add(entity);
                }
            }
            catch (Exception e)
            {

                throw new Exception("[Error Occurred] Error Message: " + e.Message.ToString());
            }
            return entities;
        }

        public T Get(Expression<Func<T, bool>> filter)
        {
            Key = "";
            Value = "";
            BinaryExpression binary = (BinaryExpression)filter.Body;
            MemberExpression memberLeft = (MemberExpression)binary.Left;
            ConstantExpression memberRight = (ConstantExpression)binary.Right;
            Hashtable hashtable = new Hashtable();
            PropertyInfo[] infos = GetT.GetType().GetProperties();
            foreach (PropertyInfo info in infos)
            {
                hashtable.Add(info.Name, info);
            }

            string cmdText = string.Format(@"Select * From {0} Where {1} = {2}", TableName, memberLeft.Member.Name, memberRight);

            SqlCommand cmd = new SqlCommand(cmdText, _connection);
            cmd.Parameters.AddWithValue("@" + memberLeft.Member.Name, memberRight.Value);
            _connection.Open();
            SqlDataReader dr = cmd.ExecuteReader();
            T entity = new T();
            while (dr.Read())
            {
                for (int i = 0; i < dr.FieldCount; i++)
                {
                    PropertyInfo prop = (PropertyInfo)hashtable[dr.GetName(i)];
                    if (prop != null)
                    {
                        prop.SetValue(entity, dr.GetValue(i), null);
                    }
                }
            }
            return entity;
        }

        private void CreateSelectQuery()
        {
            Key = "";
            Value = "";
            datas = new Dictionary<object, object>();
            KeyAttributes = new List<string>();
            foreach (var prop in GetT.GetType().GetProperties())
            {
                foreach (object keyAttr in prop.GetCustomAttributes(true))
                {

                    KeyAttribute kAttr = keyAttr as KeyAttribute;
                    if (kAttr != null)
                    {
                        KeyAttributes.Add(prop.Name);
                    }
                }
            }
            foreach (var item in GetT.GetType().GetProperties())
            {

                datas.Add(item.Name, item.GetValue(GetT));
            }

            foreach (var item in datas)
            {
                Key += item.Key;
                Key += ",";
                Value += "@";
                Value += item.Key;
                Value += ",";
            }
            if (Key.Length > 0)
            {
                Key = Key.Substring(0, Key.Length - 1);
                Value = Value.Substring(0, Value.Length - 1);
            }
        }

        public void Update(T data)
        {
            CreateUpdateQuery(data);
            try
            {
                _connection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine("[Error Occurred] Error Message : {0}", e.Message);

            }
            string cmd = string.Format("Update {0} Set {1} where {2} = @{3}", TableName, Key, Value, Value);
            SqlCommand command = new SqlCommand(cmd, _connection);
            foreach (var item in datas)
            {
                command.Parameters.AddWithValue("@" + (string)item.Key, item.Value);
            }
            command.ExecuteNonQuery();
            _connection.Close();
        }

        private void CreateUpdateQuery(T data)
        {
            Key = "";
            Value = "";
            datas = new Dictionary<object, object>();
            KeyAttributes = new List<string>();
            foreach (var prop in data.GetType().GetProperties())
            {
                foreach (object keyAttr in prop.GetCustomAttributes(true))
                {

                    KeyAttribute kAttr = keyAttr as KeyAttribute;
                    if (kAttr != null)
                    {
                        KeyAttributes.Add(prop.Name);
                    }
                }
            }

            foreach (var item in data.GetType().GetProperties())
            {

                datas.Add(item.Name, item.GetValue(data));
            }

            foreach (var item in datas)
            {
                if (KeyAttributes.Contains(item.Key))
                {
                    Key += "";
                    Value += item.Key;
                }
                else
                {
                    Key += item.Key;
                    Key += "=";
                    Key += "@";

                    Key += item.Key;
                    Key += ",";
                }
            }
            if (Key.Length > 0)
            {
                Key = Key.Substring(0, Key.Length - 1);

            }


        }

        public void Delete(T data)
        {
            CreateDeleteQuery(data);
            try
            {
                _connection.Open();
                string cmd = string.Format("Delete From {0} Where {1}=@{2}", TableName, Key, Value);
                SqlCommand command = new SqlCommand(cmd, _connection);
                foreach (var item in datas)
                {
                    command.Parameters.AddWithValue("@" + (string)item.Key, item.Value);
                }
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("[Error Occurred]  Error Message : {0}", e.Message);

            }
            finally
            {
                _connection.Close();
            }

        }

        private void CreateDeleteQuery(T data)
        {
            Key = "";
            Value = "";
            datas = new Dictionary<object, object>();
            KeyAttributes = new List<string>();
            foreach (var prop in data.GetType().GetProperties())
            {
                foreach (object keyAttr in prop.GetCustomAttributes(true))
                {

                    KeyAttribute kAttr = keyAttr as KeyAttribute;
                    if (kAttr != null)
                    {
                        KeyAttributes.Add(prop.Name);
                    }
                }
            }
            foreach (var item in data.GetType().GetProperties())
            {

                datas.Add(item.Name, item.GetValue(data));
            }
            foreach (var item in datas)
            {
                if (KeyAttributes.Contains(item.Key))
                {
                    Key += item.Key;
                    Value += item.Key;
                }

            }


        }
    }
}
