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
   public class OriginDal
    {
       public List<SingleInfo> GetList(SingleInfo carInfo)
        {
           StringBuilder sb = new StringBuilder();
            //构造要查询的sql语句
            string sql = @"select id,certificate_origin_no,single_state from bg_bm_certificate_origin_single where batch='" + carInfo.batch + "' ";
            sb.Append(sql);
            if (carInfo.certificate_origin_no != null && carInfo.certificate_origin_no != "")
            {
                sb.Append("and certificate_origin_no='" + carInfo.certificate_origin_no + "'");
            }
            if (carInfo.single_state != null && carInfo.single_state != "")
            {
                sb.Append("and single_state<'" + carInfo.single_state + "'");
            }
            sb.Append(" group by certificate_origin_no; ");
            
            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sb.ToString());
            //将dt中的数据转存到list中
            List<SingleInfo> list = new List<SingleInfo>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SingleInfo()
                {
                    certificate_origin_no = row["certificate_origin_no"].ToString(),
                    batch = carInfo.batch.ToString(),
                    single_state = row["single_state"].ToString(),
                });
            }
            //将集合返回
            return list;
        }



       public List<SingleInfo> selectChecked(SingleInfo carInfo)
       {
           StringBuilder sb = new StringBuilder();
           //构造要查询的sql语句
           string sql = @"select id,certificate_origin_no,single_state from bg_bm_certificate_origin_single where batch='" + carInfo.batch + "'";
           sb.Append(sql);
            if (carInfo.certificate_origin_no != null && carInfo.certificate_origin_no != "")
            {
                sb.Append("and certificate_origin_no in (" + carInfo.certificate_origin_no + ")");
            }
            if (carInfo.single_state != null && carInfo.single_state != "")
            {
                sb.Append("and single_state<'" + carInfo.single_state + "'");
            }
            sb.Append(" group by certificate_origin_no; ");
            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sb.ToString());
           //将dt中的数据转存到list中
           List<SingleInfo> list = new List<SingleInfo>();
           foreach (DataRow row in dt.Rows)
           {
               list.Add(new SingleInfo()
               {
                   certificate_origin_no= row["certificate_origin_no"].ToString(),
                   batch = carInfo.batch.ToString(),
                   single_state = row["single_state"].ToString(),
               });
           }
           //将集合返回
           return list;
       }



       public List<SingleInfo> GetList()
       {
           StringBuilder sb = new StringBuilder();
            //构造要查询的sql语句
            string sql = @"select batch from bg_bm_certificate_origin_single GROUP BY batch ORDER BY create_date desc limit 50";
            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sql);
           //将dt中的数据转存到list中
           List<SingleInfo> list = new List<SingleInfo>();
           foreach (DataRow row in dt.Rows)
           {
               list.Add(new SingleInfo()
               {
                   batch = row["batch"].ToString()
               });
           }
           //将集合返回
           return list;
       }

          public List<SingleInfo> GetListByDfno(SingleInfo carInfo)
          {
              //构造要查询的sql语句
              string sql = @"SELECT bg_bm_certificate_origin_single.batch,bg_bm_certificate_origin_single.single_state,COUNT(1) AS single_count,
                        bg_bm_certificate_origin_single.certificate_origin_no,bg_bm_certificate_origin_single.origin_standard,
                        bg_bm_certificate_origin_single.sign_date,bg_bm_backup_data.b_l as bill_no,bg_bm_backup_data.hs_code as hs_code
                        FROM bg_bm_certificate_origin_single LEFT JOIN bg_bm_backup_data ON bg_bm_certificate_origin_single.vin=bg_bm_backup_data.vin
                        WHERE bg_bm_certificate_origin_single.batch='" + carInfo .batch+ "' AND bg_bm_certificate_origin_single.certificate_origin_no='"+ carInfo .certificate_origin_no+ "'";
            sql += "    GROUP BY bg_bm_certificate_origin_single.batch ";
                    
                            
              //使用helper进行查询，得到结果
              DataTable dt = mysqlHelper.GetDataTable(sql);
              //将dt中的数据转存到list中
              List<SingleInfo> list = new List<SingleInfo>();
              foreach (DataRow row in dt.Rows)
              {
                  list.Add(new SingleInfo()
                  {
                      batch = row["batch"].ToString(),
                      single_state = row["single_state"].ToString(),
                      single_count = row["single_count"].ToString(),
                      certificate_origin_no = row["certificate_origin_no"].ToString(),
                      origin_standard = row["origin_standard"].ToString(),
                      sign_date = row["sign_date"].ToString(),
                      bill_no = row["bill_no"].ToString(),
                      hs_code = row["hs_code"].ToString()
                  });
              }
              //将集合返回
              return list;
          }

        public List<SingleInfo> GetListDetail(SingleInfo carInfo)
        {
            //构造要查询的sql语句
            string sql = @"SELECT bg_bm_certificate_origin_single.batch,bg_bm_certificate_origin_single.single_state,1 AS single_count,
                        bg_bm_certificate_origin_single.certificate_origin_no,bg_bm_certificate_origin_single.origin_standard,
                        bg_bm_certificate_origin_single.order_no,
                        bg_bm_certificate_origin_single.sign_date,bg_bm_backup_data.b_l as bill_no,bg_bm_backup_data.hs_code as hs_code
                        FROM bg_bm_certificate_origin_single LEFT JOIN bg_bm_backup_data ON bg_bm_certificate_origin_single.vin=bg_bm_backup_data.vin
                        WHERE bg_bm_certificate_origin_single.batch='" + carInfo.batch + "' AND bg_bm_certificate_origin_single.certificate_origin_no='" + carInfo.certificate_origin_no + "'";
            sql += "    ORDER BY bg_bm_certificate_origin_single.order_no asc ";


            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sql);
            //将dt中的数据转存到list中
            List<SingleInfo> list = new List<SingleInfo>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SingleInfo()
                {
                    batch = row["batch"].ToString(),
                    single_state = row["single_state"].ToString(),
                    single_count = row["single_count"].ToString(),
                    certificate_origin_no = row["certificate_origin_no"].ToString(),
                    origin_standard = row["origin_standard"].ToString(),
                    sign_date = row["sign_date"].ToString(),
                    bill_no = row["bill_no"].ToString(),
                    hs_code = row["hs_code"].ToString()                   
                });
            }
            //将集合返回
            return list;
        }


        //更新head状态
        public int UpdateHeadState(SingleInfo mi)
        {
            //为什么要进行密码的判断：
            //答：因为密码值是经过md5加密存储的，当修改时，需要判断用户是否改了密码，如果没有改，则不变，如果改了，则重新进行md5加密

            //构造update的sql语句
            string sql = @"update bg_bm_certificate_origin_single set single_state=@single_state where batch=@batch and certificate_origin_no=@certificate_origin_no  ";

            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            { 

                new MySqlParameter("@single_state", mi.single_state),
                new MySqlParameter("@certificate_origin_no", mi.certificate_origin_no ),
                new MySqlParameter("@batch", mi.batch)
            };

            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql, ps);
        }

        //更新head状态
        public int UpdateHeadStateAndNo(SingleInfo mi)
        {
            //为什么要进行密码的判断：
            //答：因为密码值是经过md5加密存储的，当修改时，需要判断用户是否改了密码，如果没有改，则不变，如果改了，则重新进行md5加密

            //构造update的sql语句
            string sql = @"update bg_bm_certificate_origin_single set single_state=@single_state,single_seq_no=@single_seq_no where batch=@batch and certificate_origin_no=@certificate_origin_no  ";

            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            {

                new MySqlParameter("@single_state", mi.single_state),
                new MySqlParameter("@certificate_origin_no", mi.certificate_origin_no ),
                 new MySqlParameter("@single_seq_no", mi.single_seq_no ),
                new MySqlParameter("@batch", mi.batch)
            };

            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql, ps);
        }

        //根据批次号，项号更新
        public int UpdateCertificateDetail(SingleInfo mi)
        {
            //为什么要进行密码的判断：
            //答：因为密码值是经过md5加密存储的，当修改时，需要判断用户是否改了密码，如果没有改，则不变，如果改了，则重新进行md5加密

            //构造update的sql语句
            string sql = @"update bg_bm_certificate_origin_single 
                            set single_state=@single_state,
                                CERT_NO=@CERT_NO,
                                AGREEMENT_ID=@AGREEMENT_ID,
                                ORIGIN_COUNTRY=@ORIGIN_COUNTRY,
                                ISSUE_DATE=@ISSUE_DATE,
                                CODE_TS=@CODE_TS,
                                QTY=@QTY,
                                UNIT=@UNIT,
                                ORIGIN_CRITERION=@ORIGIN_CRITERION,
                                IS_TRANSFER_RADIO=@IS_TRANSFER_RADIO,
                                IS_TRANSPORT_DOC_RADIO=@IS_TRANSPORT_DOC_RADIO,
                                TRANSPORT_DOC_NO_TEXT=@TRANSPORT_DOC_NO_TEXT,
                                NO=@NO,
                                single_seq_no=@SEQ_NO
                                where batch=@batch and certificate_origin_no=@certificate_origin_no and item_no=@order_no  ";

            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            {

                new MySqlParameter("@single_state", mi.single_state),
                new MySqlParameter("@CERT_NO", mi.CERT_NO ),
                new MySqlParameter("@AGREEMENT_ID", mi.AGREEMENT_ID ),
                new MySqlParameter("@ORIGIN_COUNTRY", mi.ORIGIN_COUNTRY ),
                new MySqlParameter("@ISSUE_DATE", mi.ISSUE_DATE ),
                new MySqlParameter("@CODE_TS", mi.CODE_TS ),
                new MySqlParameter("@QTY", mi.QTY ),
                new MySqlParameter("@UNIT", mi.UNIT ),
                new MySqlParameter("@ORIGIN_CRITERION", mi.ORIGIN_CRITERION ),
                new MySqlParameter("@IS_TRANSFER_RADIO", mi.IS_TRANSFER_RADIO ),
                new MySqlParameter("@IS_TRANSPORT_DOC_RADIO", mi.IS_TRANSPORT_DOC_RADIO ),
                new MySqlParameter("@TRANSPORT_DOC_NO_TEXT", mi.TRANSPORT_DOC_NO_TEXT ),
                new MySqlParameter("@NO", mi.NO ),                
                new MySqlParameter("@SEQ_NO",mi.single_seq_no),
                new MySqlParameter("@batch", mi.batch),
                new MySqlParameter("@certificate_origin_no", mi.certificate_origin_no),
                new MySqlParameter("@order_no", mi.order_no)
            };

            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql, ps);
        }

        //更新head状态
        public int UpdateHeadState1(SingleInfo mi)
        {
            //为什么要进行密码的判断：
            //答：因为密码值是经过md5加密存储的，当修改时，需要判断用户是否改了密码，如果没有改，则不变，如果改了，则重新进行md5加密

            //构造update的sql语句
            string sql = @"update bg_bm_certificate_origin_single set single_state='" + mi.single_state + "' where batch='" + mi.batch + "' and certificate_origin_no in ( " + mi.certificate_origin_no + ") ";


            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql);
        }
       

        /// <summary>
        /// 更新证明书编号
        /// </summary>
        /// <param name="mi"></param>
        /// <returns></returns>
        public int UpdateCerNo(SingleInfo mi)
        {
            //为什么要进行密码的判断：
            //答：因为密码值是经过md5加密存储的，当修改时，需要判断用户是否改了密码，如果没有改，则不变，如果改了，则重新进行md5加密

            //构造update的sql语句
            string sql = @"update bg_bm_certificate_origin_single set cer_no=@p2,statu=@p12,update_date=@p13 where batch=@batch and df_no=@df_no and barcode=@p10";
            sql = "";
            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            { 
                new MySqlParameter("@p12", mi.single_state),
                new MySqlParameter("@df_no", mi.certificate_origin_no ),
                new MySqlParameter("@batch", mi.batch )
            };

            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql, ps);
        }


    }
}
