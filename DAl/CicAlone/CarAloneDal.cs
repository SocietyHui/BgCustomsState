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

namespace DAl.CicAlone
{
    public class CarAloneDal
    {
        public List<CarInfo> GetList(CarInfo carInfo)
        {
            StringBuilder sb = new StringBuilder();
            //构造要查询的sql语句
            string sql = @"select df_no as id,df_no,single_state from cic_input_data where batch='" + carInfo.Batch + "'";
            sb.Append(sql);
            if (carInfo.Df_no != null && carInfo.Df_no != "")
            {
                sb.Append("and df_no='" + carInfo.Df_no + "'");
            }
            if (carInfo.SingleState != null && carInfo.SingleState != "")
            {
                sb.Append("and single_state<'" + carInfo.SingleState + "'");
            }
            sb.Append(" group by df_no ");
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

        //查询导入明细
        public List<CarInfo> GetListAll(CarInfo carInfo)
        {
            StringBuilder sb = new StringBuilder();
            //构造要查询的sql语句
            string sql = @"select * from cic_input_data where batch='" + carInfo.Batch + "'";
            sb.Append(sql);
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
                    Detail_id = row["g_no"].ToString(),
                    Batch = carInfo.Batch.ToString(),
                    Seq_no = row["seq_no"].ToString(),
                    Barcode = row["barcode"].ToString(),
                    Color = row["color"].ToString(),
                    Spec = row["spec"].ToString(),
                    Engine_type = row["engine_type"].ToString(),
                    Displacement = row["displacement"].ToString(),
                    Motor_no = row["motor_no"].ToString(),
                    Motor_power = row["motor_power"].ToString(),
                    Production_date = row["production_date"].ToString()
                });
            }
            //将集合返回
            return list;
        }


        public List<CarInfo> selectChecked(CarInfo carInfo)
        {
            StringBuilder sb = new StringBuilder();
            //构造要查询的sql语句
            string sql = @"select df_no as id,df_no,single_state from cic_input_data where batch='" + carInfo.Batch + "'";
            sb.Append(sql);
            if (carInfo.Df_no != null && carInfo.Df_no != "")
            {
                sb.Append("and df_no in (" + carInfo.Df_no + ")");
            }
            if (carInfo.SingleState != null && carInfo.SingleState != "")
            {
                sb.Append("and single_state<'" + carInfo.SingleState + "'");
            }
            sb.Append(" group by df_no ");
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



        public List<CarInfo> GetList()
        {
            StringBuilder sb = new StringBuilder();
            //构造要查询的sql语句
            string sql = @"select batch from cic_input_data GROUP BY batch ORDER BY create_date desc limit 50";

            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sql);
            //将dt中的数据转存到list中
            List<CarInfo> list = new List<CarInfo>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new CarInfo()
                {
                    Batch = row["batch"].ToString()
                });
            }
            //将集合返回
            return list;
        }

        public List<CarInfo> GetListByDfno(CarInfo carInfo)
        {
            //构造要查询的sql语句
            string sql = @"select g_no as id,batch,df_no as cust_dec_head_id from cic_input_data
                            where batch='" + carInfo.Batch + "' and df_no='" + carInfo.Id + "'";
            sql += "group by g_no";
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
            string sql = @"select df_no,g_no,id as cust_dec_detail_id, a.seq_no as rowno,
                            A.g_no as seq_no,barcode,color, spec, engine_type,
                            displacement, motor_no, motor_power,production_date
                            from cic_input_data A  
                            where A.batch = '" + carInfo.Batch + "' and A.g_no ='" + carInfo.Detail_id + "' and df_no ='" + carInfo.Cust_dec_head_id + "'";
                     
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
            string sql = "select * from cic_input_data where seq_no = '" + carInfo.Seq_no + "' and cust_dec_detail_id = '" + carInfo.Detail_id + "' and df_no = '" + carInfo.Df_no + "'";
            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sql);

            //将集合返回
            return dt.Rows.Count > 0;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="mi">ManagerInfo类型的对象</param>
        /// <returns></returns>
        public int InsertCarInfo(CarInfo mi)
        {
            //构造insert语句
            string sql = @"insert into cic_input_data(batch,g_no,seq_no,df_no,single_state,
                            barcode,color,spec,engine_type,displacement,motor_no,motor_power,production_date)";
            sql += " values(@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12)";
            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            {
                new MySqlParameter("@p0", mi.Batch),
                new MySqlParameter("@p1", mi.Detail_id),
                new MySqlParameter("@p2", mi.Seq_no),
                new MySqlParameter("@p3", mi.Df_no),
                new MySqlParameter("@p4", "0"),
                new MySqlParameter("@p5", mi.Barcode ),
                new MySqlParameter("@p6", mi.Color),
                new MySqlParameter("@p7", mi.Spec ),
                new MySqlParameter("@p8", mi.Engine_type),
                new MySqlParameter("@p9", mi.Displacement ),
                new MySqlParameter("@p10", mi.Motor_no),
                new MySqlParameter("@p11", mi.Motor_power ),                
                new MySqlParameter("@p12", mi.Production_date )
            };
            //执行插入操作
            return mysqlHelper.ExcuteNonQuery(sql, ps);
        }

        /// <summary>
        /// 修改管理员，特别注意：密码
        /// </summary>
        /// <param name="mi"></param>
        /// <returns></returns>
        public int UpdateCarInfo(CarInfo mi)
        {
            //为什么要进行密码的判断：
            //答：因为密码值是经过md5加密存储的，当修改时，需要判断用户是否改了密码，如果没有改，则不变，如果改了，则重新进行md5加密

            //构造update的sql语句
            string sql = @"update cic_input_data set cop_no_cic=@p2,car_type_cic=@p3,spec_cic=@p4,engine_type_cic=@p5,displacement_cic=@p6,motor_no_cic=@p7,
                           motor_power_cic=@p8,color_cic=@p9,barcode_cic=@p10,production_date_cic=@p11,statu_cic=@p12,update_date=@p13,
                            model_cic=@p14,brand_cic=@p15 where batch=@batch and df_no=@df_no and seq_no=@seq_no and g_no=@detailId ";

            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            {

                new MySqlParameter("@p2", mi.CopNo),
                new MySqlParameter("@p3", mi.CarType ),
                new MySqlParameter("@p4", mi.Spec),
                new MySqlParameter("@p5", mi.Engine_type ),
                new MySqlParameter("@p6", mi.Displacement),
                new MySqlParameter("@p7", mi.Motor_no ),
                new MySqlParameter("@p8", mi.Motor_power),
                new MySqlParameter("@p9", mi.Color ),
                new MySqlParameter("@p10", mi.Barcode),
                new MySqlParameter("@p11", mi.Production_date ),
                new MySqlParameter("@p12", mi.Statu),
                new MySqlParameter("@p13", mi.update_date ),
                new MySqlParameter("@p14", mi.Model ),
                new MySqlParameter("@p15", mi.Brand ),
                new MySqlParameter("@df_no", mi.Df_no ),
                new MySqlParameter("@batch", mi.Batch ),
                new MySqlParameter("@seq_no", mi.Seq_no ),
                new MySqlParameter("@detailId", mi.Detail_id )
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
            string sql = @"update cic_input_data set single_state=@p1 where batch=@batch and df_no=@df_no ";

            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            {

                new MySqlParameter("@p1", mi.SingleState),
                new MySqlParameter("@df_no", mi.Df_no ),
                new MySqlParameter("@batch", mi.Batch )
                //new MySqlParameter("@headid", mi.Id )
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
            string sql = @"update cic_input_data set single_state='" + mi.SingleState + "' where batch='" + mi.Batch + "' and df_no in ( " + mi.Df_no + ") ";


            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql);
        }

        /// <summary>
        /// 根据编号删除管理员
        /// </summary>
        /// <param name="batch"></param>
        /// <returns></returns>
        public int DeleteCicData(String batch)
        {
            //构造删除的sql语句
            string sql = "delete from cic_input_data where batch='" + batch + "'";
            //执行操作
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
            string sql = @"update cic_input_data set cer_no=@p2,statu=@p12,update_date=@p13 where batch=@batch and df_no=@df_no and barcode=@p10";

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


        //查询导入明细
        public List<CarInfo> Compare(CarInfo carInfo)
        {
            //构造要查询的sql语句
            string sql = @"select * from cic_input_data where 1=1 and 
                            spec_cic<>spec or engine_type_cic<> engine_type or displacement_cic<>displacement 
                            or motor_no_cic<>motor_no or motor_power_cic<>motor_power
                            or color_cic<>color or barcode_cic<> barcode or production_date_cic<>production_date
                            and batch='" + carInfo.Batch + "'";
            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sql);
            //将dt中的数据转存到list中
            List<CarInfo> list = new List<CarInfo>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new CarInfo()
                {
                    Id = row["id"].ToString(),
                    Df_no = row["df_no"].ToString(),
                    Detail_id = row["g_no"].ToString(),
                    Batch = carInfo.Batch.ToString(),
                    Seq_no = row["seq_no"].ToString(),
                    Barcode = row["barcode"].ToString(),
                    Color = row["color"].ToString(),
                    Spec = row["spec"].ToString(),
                    Engine_type = row["engine_type"].ToString(),
                    Displacement = row["displacement"].ToString(),
                    Motor_no = row["motor_no"].ToString(),
                    Motor_power = row["motor_power"].ToString(),
                    Production_date = row["production_date"].ToString(),
                   
                    barcode_cic = row["barcode_cic"].ToString(),
                    color_cic=row["color_cic"].ToString(),
                    displacement_cic=row["displacement_cic"].ToString(),
                    engine_type_cic=row["engine_type_cic"].ToString(),
                    motor_no_cic=row["motor_no_cic"].ToString(),
                    motor_power_cic=row["motor_power_cic"].ToString(),
                    production_date_cic=row["production_date_cic"].ToString(),
                    spec_cic=row["spec_cic"].ToString()
                });
            }
            //将集合返回
            return list;
        }

    }

}
