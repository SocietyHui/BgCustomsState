using Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SqlUtil;
using MODEL;
using MySql.Data.MySqlClient;
namespace DAl
{
   public class InfinitiCarInfoDal
    {
       public List<InfinitiCarInfo> GetList(InfinitiCarInfo carInfo)
        {
            //构造要查询的sql语句
            string sql = @"select * from bg_yf_licence_upload where batch='" + carInfo.batch + "' and (serial_no is null or serial_no='') ";
          
            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sql);
            //将dt中的数据转存到list中
            List<InfinitiCarInfo> list = new List<InfinitiCarInfo>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new InfinitiCarInfo()
                {
                    Id =  row["id"].ToString(),
                    contract = row["contract"].ToString(),
                    commodity_code = row["commodity_code"].ToString(),
                    goods_no = row["goods_no"].ToString(),

                    quantity = row["quantity"].ToString(),
                    unit_price = row["unit_price"].ToString(),
                    import_application = row["import_application"].ToString(),
                    contract_no = row["contract_no"].ToString(),

                    contract_total_num = row["contract_total_num"].ToString(),
                    contract_signing_date = row["contract_signing_date"].ToString(),
                    contract_amount = row["contract_amount"].ToString(),
                    estimated_arrival_date = row["estimated_arrival_date"].ToString(),

                    specific_model = row["specific_model"].ToString(),
                    specific_goods_no = row["specific_goods_no"].ToString(),
                    batch = row["batch"].ToString()
                });
            }
            //将集合返回
            return list;
        }



       public List<CarInfo> selectChecked(CarInfo carInfo)
       {
           StringBuilder sb = new StringBuilder();
           //构造要查询的sql语句
           string sql = @"select id,df_no,single_state from bg_cust_dec_head where batch='" + carInfo.Batch + "'";
           sb.Append(sql);
           if (carInfo.Df_no != null && carInfo.Df_no != "")
           {
               sb.Append("and df_no in (" + carInfo.Df_no + ")");
           }
           if (carInfo.SingleState != null && carInfo.SingleState != "")
           {
               sb.Append("and single_state<'" + carInfo.SingleState + "'");
           }
           //使用helper进行查询，得到结果
           DataTable dt = mysqlHelper.GetDataTable(sb.ToString());
           //将dt中的数据转存到list中
           List<CarInfo> list = new List<CarInfo>();
           foreach (DataRow row in dt.Rows)
           {
               list.Add(new CarInfo()
               {
                   Id = row["id"].ToString(),
                   Df_no = row["df_no"].ToString(),
                   Batch = carInfo.Batch.ToString(),
                   SingleState = row["single_state"].ToString(),
               });
           }
           //将集合返回
           return list;
       }



       public List<InfinitiCarInfo> GetList()
       {
           StringBuilder sb = new StringBuilder();
           //构造要查询的sql语句
           string sql = @"select batch from bg_yf_licence_upload GROUP BY batch ORDER BY create_date desc limit 50";
           
           //使用helper进行查询，得到结果
           DataTable dt = mysqlHelper.GetDataTable(sql);
           //将dt中的数据转存到list中
           List<InfinitiCarInfo> list = new List<InfinitiCarInfo>();
           foreach (DataRow row in dt.Rows)
           {
               list.Add(new InfinitiCarInfo()
               {
                   batch = row["batch"].ToString()
               });
           }
           //将集合返回
           return list;
       }

          public List<CarInfo> GetListByDfno(CarInfo carInfo)
          {
              //构造要查询的sql语句
              string sql = @"select id,batch,cust_dec_head_id from bg_cust_dec_detail
                            where batch='" + carInfo.Batch + "' and cust_dec_head_id='" + carInfo.Id + "'";
              //使用helper进行查询，得到结果
              DataTable dt = mysqlHelper.GetDataTable(sql);
              //将dt中的数据转存到list中
              List<CarInfo> list = new List<CarInfo>();
              foreach (DataRow row in dt.Rows)
              {
                  list.Add(new CarInfo()
                  {
                      Detail_id = row["id"].ToString(),
                      Batch = carInfo.Batch.ToString(),
                      Cust_dec_head_id = row["cust_dec_head_id"].ToString(),
                  });
              }
              //将集合返回
              return list;
          }




          public List<CarInfo> GetListByDetailId(CarInfo carInfo)
          {
              //构造要查询的sql语句
              string sql = @"select CAST((@i:=@i+1) AS CHAR) as rowno , A.seq_no,B.barcode,B.color,B.info1 as spec,B.engine_type,REPLACE(B.displacement,'CC','') as displacement
		          ,B.motor_no,B.motor_power,DATE_FORMAT(production_date,'%Y%m') as production_date from bg_cust_dec_detail A 
                  left JOIN bg_cust_dec_list B on A.id = B.cust_dec_detail_id ,(select @i:=0) as it
                       where A.batch='" + carInfo.Batch + "' AND A.id='" + carInfo.Detail_id + "'";
              //使用helper进行查询，得到结果
              DataTable dt = mysqlHelper.GetDataTable(sql);
              //将dt中的数据转存到list中
              List<CarInfo> list = new List<CarInfo>();
              foreach (DataRow row in dt.Rows)
              {
                  list.Add(new CarInfo()
                  {
                      Row_no = row["rowno"].ToString(),
                      Seq_no = row["seq_no"].ToString(),
                      Barcode = row["barcode"].ToString(),
                      Color = row["color"].ToString(),
                      Spec = row["spec"].ToString(),
                      Engine_type = row["engine_type"].ToString(),
                      Displacement = row["displacement"].ToString(),
                      Motor_no = row["motor_no"].ToString(),
                      Motor_power = row["motor_power"].ToString(),
                      Production_date = row["production_date"].ToString(),

                  });
              }
              //将集合返回
              return list;
          }







          public bool GetExsitList(CarInfo carInfo)
        {
            //构造要查询的sql语句
            string sql = "select * from bg_cust_dec_carinfo where seq_no = '" + carInfo.Seq_no + "' and cust_dec_detail_id = '" + carInfo.Detail_id + "' and df_no = '" + carInfo.Df_no + "'";
            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sql);
          
            //将集合返回
            return dt.Rows.Count>0;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="mi">ManagerInfo类型的对象</param>
        /// <returns></returns>
        public int InsertCarInfo(CarInfo mi)
        {
            //构造insert语句
            string sql = @"insert into bg_cust_dec_carinfo(seq_no,cust_dec_detail_id,cust_dec_head_id,batch,cop_no,car_type,spec,engine_type,
                          displacement,motor_no,motor_power,color,barcode,production_date,statu,detail_sum,create_date,update_date,df_no,model,brand)";
            sql += " values(@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16,@p17,@p18,@p19,@p20)";
            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            { 
                new MySqlParameter("@p0", mi.Seq_no),
                new MySqlParameter("@p1", mi.Detail_id),
                new MySqlParameter("@p2", mi.Cust_dec_head_id ),
                new MySqlParameter("@p3", mi.Batch),
                new MySqlParameter("@p4", mi.CopNo),
                new MySqlParameter("@p5", mi.CarType ),
                new MySqlParameter("@p6", mi.Spec),
                new MySqlParameter("@p7", mi.Engine_type ),
                new MySqlParameter("@p8", mi.Displacement),
                new MySqlParameter("@p9", mi.Motor_no ),
                new MySqlParameter("@p10", mi.Motor_power),
                new MySqlParameter("@p11", mi.Color ),
                new MySqlParameter("@p12", mi.Barcode),
                new MySqlParameter("@p13", mi.Production_date ),
                new MySqlParameter("@p14", mi.Statu),
                new MySqlParameter("@p15", mi.detailSum ),
                new MySqlParameter("@p16", mi.create_date),
                new MySqlParameter("@p17", mi.update_date ),
                new MySqlParameter("@p18", mi.Df_no ),
                new MySqlParameter("@p19", mi.Model ),
                new MySqlParameter("@p20", mi.Brand )
            };
            //执行插入操作
            return mysqlHelper.ExcuteNonQuery(sql, ps);
        }

        /// <summary>
        /// 修改管理员，特别注意：密码
        /// </summary>
        /// <param name="mi"></param>
        /// <returns></returns>
        public int UpdateCarInfo(InfinitiCarInfo mi)
        {
            //为什么要进行密码的判断：
            //答：因为密码值是经过md5加密存储的，当修改时，需要判断用户是否改了密码，如果没有改，则不变，如果改了，则重新进行md5加密

            //构造update的sql语句
            string sql = @"update bg_yf_licence_upload set serial_no=@p2  where batch=@batch and id=@id ";
            
            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            { 

                new MySqlParameter("@p2", mi.serial_no),
                new MySqlParameter("@batch", mi.batch ),
                new MySqlParameter("@id", mi.Id )
            };

            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql, ps);
        }


       //更新head状态
        public int UpdateHeadState(CarInfo mi)
        {
            //为什么要进行密码的判断：
            //答：因为密码值是经过md5加密存储的，当修改时，需要判断用户是否改了密码，如果没有改，则不变，如果改了，则重新进行md5加密

            //构造update的sql语句
            string sql = @"update bg_cust_dec_head set single_state=@p1 where batch=@batch and df_no=@df_no and  id=@headid ";

            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            { 

                new MySqlParameter("@p1", mi.SingleState),
                new MySqlParameter("@df_no", mi.Df_no ),
                new MySqlParameter("@batch", mi.Batch ),
                new MySqlParameter("@headid", mi.Id )
            };

            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql, ps);
        }

        //更新head状态
        public int UpdateHeadState1(CarInfo mi)
        {
            //为什么要进行密码的判断：
            //答：因为密码值是经过md5加密存储的，当修改时，需要判断用户是否改了密码，如果没有改，则不变，如果改了，则重新进行md5加密

            //构造update的sql语句
            string sql = @"update bg_cust_dec_head set single_state='" + mi.SingleState + "' where batch='" + mi.Batch + "' and df_no in ( " + mi.Df_no + ") ";


            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql);
        }



        /// <summary>
        /// 根据编号删除管理员
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public int Delete(int id)
        {
            //构造删除的sql语句
            string sql = "delete from ManagerInfo where mid=@id";
            //根据语句构造参数
            MySqlParameter p = new MySqlParameter("@id", id);
            //执行操作
            return mysqlHelper.ExcuteNonQuery(sql, p);
        }





       
        /// <summary>
        /// 修改管理员，特别注意：密码
        /// </summary>
        /// <param name="mi"></param>
        /// <returns></returns>
        public int UpdateCerInfo(CarInfo mi)
        {
            //为什么要进行密码的判断：
            //答：因为密码值是经过md5加密存储的，当修改时，需要判断用户是否改了密码，如果没有改，则不变，如果改了，则重新进行md5加密

            //构造update的sql语句
            string sql = @"update bg_certifacation_compare set file_name=@p0,file_path=@p1,certifacation2=@p2  where batch=@batch and vin=@vin ";

            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            { 
                new MySqlParameter("@p0", mi.Motor_no),
                new MySqlParameter("@p1", mi.Model),
                new MySqlParameter("@p2", mi.Df_no),
                new MySqlParameter("@batch", mi.Batch ),
                new MySqlParameter("@vin", mi.Barcode )
            };

            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql, ps);
        }


        /// <summary>
        /// 更新证明书编号
        /// </summary>
        /// <param name="mi"></param>
        /// <returns></returns>
        public int UpdateCerNo(CarInfo mi)
        {
            //为什么要进行密码的判断：
            //答：因为密码值是经过md5加密存储的，当修改时，需要判断用户是否改了密码，如果没有改，则不变，如果改了，则重新进行md5加密

            //构造update的sql语句
            string sql = @"update bg_cust_dec_carinfo set cer_no=@p2,statu=@p12,update_date=@p13 where batch=@batch and df_no=@df_no and barcode=@p10";

            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            { 

                new MySqlParameter("@p2", mi.cer_no),
                new MySqlParameter("@p10", mi.Barcode),
                new MySqlParameter("@p12", mi.Statu),
                new MySqlParameter("@p13", mi.update_date ),
                new MySqlParameter("@df_no", mi.Df_no ),
                new MySqlParameter("@batch", mi.Batch )
            };

            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql, ps);
        }


    }
}
