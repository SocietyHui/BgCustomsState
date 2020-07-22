using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using System.Data.SqlClient;
using System.IO;

namespace SqlUtil
{
   public static class mysqlHelper
    {
       private static string connStr = ConfigurationManager.ConnectionStrings["mysql"].ConnectionString;


       public static int ExcuteNonQuery(string sql, params MySqlParameter[]  ps)
       {
           //创建连接对象
           using (MySqlConnection conn = new MySqlConnection(connStr))
           {
               //创建命令对象
               MySqlCommand cmd = new MySqlCommand(sql, conn);
             
               //添加参数
               cmd.Parameters.AddRange(ps);
               //打开连接
               conn.Open();
               //执行命令，并返回受影响的行数
               return cmd.ExecuteNonQuery();
           }
       }


       //获取首行首列值的方法
       public static object ExecuteScalar(string sql, params MySqlParameter[] ps)
       {
           using (MySqlConnection conn = new MySqlConnection(connStr))
           {
               MySqlCommand cmd = new MySqlCommand(sql, conn);
               cmd.Parameters.AddRange(ps);

               conn.Open();
               //执行命令，获取查询结果中的首行首列的值，返回
               return cmd.ExecuteScalar();
           }
       }

       //获取结果集
       public static DataTable GetDataTable(string sql, params MySqlParameter[] ps)
       {
           using (MySqlConnection conn = new MySqlConnection(connStr))
           {
               //构造适配器对象
               MySqlDataAdapter adapter = new MySqlDataAdapter(sql, conn);
               //构造数据表，用于接收查询结果
               DataTable dt = new DataTable();
               //添加参数
               adapter.SelectCommand.Parameters.AddRange(ps);
               //执行结果
               adapter.Fill(dt);
               //返回结果集
               return dt;
           }
       }

       /// <summary>
       ///使用MySqlDataAdapter批量更新数据
       /// </summary>
       /// <param name="connectionString">数据库连接字符串</param>
       /// <param name="table">数据表</param>
       public static void BatchUpdate(DataTable table)
       {

           MySqlConnection conn = new MySqlConnection(connStr);
           MySqlCommand command = conn.CreateCommand();
           command.CommandTimeout = 22;
           command.CommandType = CommandType.Text;
           MySqlDataAdapter adapter = new MySqlDataAdapter(command);
           MySqlCommandBuilder commandBulider = new MySqlCommandBuilder(adapter);
           commandBulider.ConflictOption = ConflictOption.OverwriteChanges;

           MySqlTransaction transaction = null;
           try
           {
               conn.Open();
               transaction = conn.BeginTransaction();
               //设置批量更新的每次处理条数
               adapter.UpdateBatchSize = table.Rows.Count;
               //设置事物
               adapter.SelectCommand.Transaction = transaction;

               if (table.ExtendedProperties["SQL"] != null)
               {
                   adapter.SelectCommand.CommandText = table.ExtendedProperties["SQL"].ToString();
               }
               adapter.Update(table);
               transaction.Commit();/////提交事务
           }
           catch (MySqlException ex)
           {
               if (transaction != null) transaction.Rollback();
               throw ex;
           }
           finally
           {
               conn.Close();
               conn.Dispose();
           }
       }



       /// <summary>
       ///大批量数据插入,返回成功插入行数
       /// </summary>
       /// <param name="connectionString">数据库连接字符串</param>
       /// <param name="table">数据表</param>
       /// <returns>返回成功插入行数</returns>
       public static int BulkInsert(DataTable table)
       {
           if (string.IsNullOrEmpty(table.TableName)) throw new Exception("请给DataTable的TableName属性附上表名称");
           if (table.Rows.Count == 0) return 0;
           int insertCount = 0;
           string tmpPath = Path.GetTempFileName();
           string csv = DataTableToCsv(table);
           File.WriteAllText(tmpPath, csv);
           using (MySqlConnection conn = new MySqlConnection(connStr))
           {
               MySqlTransaction tran = null;
               try
               {
                   conn.Open();
                   tran = conn.BeginTransaction();
                   MySqlBulkLoader bulk = new MySqlBulkLoader(conn)
                   {
                       FieldTerminator = ",",
                       FieldQuotationCharacter = '"',
                       EscapeCharacter = '"',
                       LineTerminator = "\r\n",
                       FileName = tmpPath,
                       NumberOfLinesToSkip = 0,
                       TableName = table.TableName,
                   };
                   bulk.Columns.AddRange(table.Columns.Cast<DataColumn>().Select(colum => colum.ColumnName).ToList());
                   insertCount = bulk.Load();
                   tran.Commit();
               }
               catch (MySqlException ex)
               {
                   if (tran != null) tran.Rollback();
                   throw ex;
               }
           }
           File.Delete(tmpPath);
           return insertCount;
       }


       /// <summary>
       ///将DataTable转换为标准的CSV
       /// </summary>
       /// <param name="table">数据表</param>
       /// <returns>返回标准的CSV</returns>
       private static string DataTableToCsv(DataTable table)
       {
           //以半角逗号（即,）作分隔符，列为空也要表达其存在。
           //列内容如存在半角逗号（即,）则用半角引号（即""）将该字段值包含起来。
           //列内容如存在半角引号（即"）则应替换成半角双引号（""）转义，并用半角引号（即""）将该字段值包含起来。
           StringBuilder sb = new StringBuilder();
           DataColumn colum;
           foreach (DataRow row in table.Rows)
           {
               for (int i = 0; i < table.Columns.Count; i++)
               {
                   colum = table.Columns[i];
                   if (i != 0) sb.Append(",");
                   if (colum.DataType == typeof(string) && row[colum].ToString().Contains(","))
                   {
                       sb.Append("\"" + row[colum].ToString().Replace("\"", "\"\"") + "\"");
                   }
                   else sb.Append(row[colum].ToString());
               }
               sb.AppendLine();
           }

           return sb.ToString();
       }

    }
}
