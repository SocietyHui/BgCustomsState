using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.OleDb;

namespace SqlUtil
{
   public static class accessHelper
    {
       private static string conStr = ConfigurationManager.ConnectionStrings["access"].ConnectionString;


       public static int ExcuteNonQuery(string sql, params OleDbParameter[]  ps)
       {
           //创建连接对象
           using (OleDbConnection conn = new OleDbConnection(conStr))
           {
               //创建命令对象
               OleDbCommand cmd = new OleDbCommand(sql, conn);
             
               //添加参数
               cmd.Parameters.AddRange(ps);
               //打开连接
               conn.Open();
               //执行命令，并返回受影响的行数
               return cmd.ExecuteNonQuery();
           }
       }


       //获取首行首列值的方法
       public static object ExecuteScalar(string sql, params OleDbParameter[] ps)
       {
           using (OleDbConnection conn = new OleDbConnection(conStr))
           {
               OleDbCommand cmd = new OleDbCommand(sql, conn);
               cmd.Parameters.AddRange(ps);

               conn.Open();
               //执行命令，获取查询结果中的首行首列的值，返回
               return cmd.ExecuteScalar();
           }
       }

       //获取结果集
       //public static DataTable GetDataTable(string sql, params OleDbParameter[] ps)
       //{
       //    using (OleDbConnection conn = new OleDbConnection(conStr))
       //    {

       //        //构造适配器对象
       //        OleDbDataAdapter adapter = new OleDbDataAdapter(sql, conn);
       //        //构造数据表，用于接收查询结果
       //        DataTable dt = new DataTable();
       //        //添加参数
       //        adapter.SelectCommand.Parameters.AddRange(ps);
       //        //执行结果
       //        adapter.Fill(dt);
       //        //返回结果集
       //        return dt;
       //    }
       //}


       //获取结果集
       public static DataTable GetDataTable(string sql, params OleDbParameter[] ps)
       {
           using (OleDbConnection conn = new OleDbConnection(conStr))
           {
               try
               {
                   //构造适配器对象
                   OleDbDataAdapter adapter = new OleDbDataAdapter(sql, conn);
                   //构造数据表，用于接收查询结果
                   DataTable dt = new DataTable();
                   //添加参数
                   adapter.SelectCommand.Parameters.AddRange(ps);
                   //执行结果
                   adapter.Fill(dt);
                   //返回结果集
                   return dt;
               }
               catch (Exception ex)
               {
                   throw ex;
               }
               finally
               {
                   if (conn.State == ConnectionState.Open)
                       conn.Close();
               }

           }
       }


        //获取结果集
       public static DataTable GetSysDbTime(string sql, params OleDbParameter[] ps)
       {
           using (OleDbConnection conn = new OleDbConnection(conStr))
           {
               try
               {
                   //构造适配器对象
                   OleDbDataAdapter adapter = new OleDbDataAdapter(sql, conn);
                   //构造数据表，用于接收查询结果
                   DataTable dt = new DataTable();
                   //添加参数
                   adapter.SelectCommand.Parameters.AddRange(ps);
                   //执行结果
                   adapter.Fill(dt);
                   //返回结果集
                   return dt;
               }
               catch (Exception ex)
               {
                   throw ex;
               }
               finally
               {
                   if (conn.State == ConnectionState.Open)
                       conn.Close();
               }

           }
       }
       
    }
}
