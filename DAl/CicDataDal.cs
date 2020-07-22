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
using System.Data.OleDb;

namespace DAL
{
  public   class CicDataDal
    {
      public List<CicData> GetList()
        {
            //构造要查询的sql语句
            //string sql = "select * from CIC_Data where date_Result > @p1 ";
            string sql = "SELECT * from  cic_data  ORDER BY date_Result desc LIMIT 500 ";
            //OleDbParameter[] ps = //使用数组初始化器
            //{
            //    new OleDbParameter("@p1", mi.date_Result)
            //};
            //使用helper进行查询，得到结果
            DataTable dt = accessHelper.GetDataTable(sql);
            //将dt中的数据转存到list中
            List<CicData> list = new List<CicData>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new CicData()
                {
                    OperType = row["OperType"].ToString(),
                    ClientSeqNo = row["ClientSeqNo"].ToString(),
                    PreNo = row["PreNo"].ToString(),
                    EntryId = row["EntryId"].ToString(),
                    GNo = row["GNo"].ToString(),
                    GSeqNo = row["GSeqNo"].ToString(),
                    Color = row["Color"].ToString(),
                    CarCoverNo = row["CarCoverNo"].ToString(),
                    EnergyType = row["EnergyType"].ToString(),
                    EngineNo = row["EngineNo"].ToString(),
                    
                });
            }
            //将集合返回
            return list;
        }


      public DataTable GetDataTable()
      {
          //构造要查询的sql语句
          //string sql = "select * from CIC_Data where date_Result > @p1 ";
          string sql = "SELECT TOP 500 * from  cic_data  ORDER BY date_Result desc";
          //OleDbParameter[] ps = //使用数组初始化器
          //{
          //    new OleDbParameter("@p1", mi.date_Result)
          //};
          //使用helper进行查询，得到结果
          DataTable dt = accessHelper.GetDataTable(sql);

         
          //将集合返回
          return dt;
      }

      public DataTable GetSysDbTime()
      {
          //构造要查询的sql语句
          string sql = "select max(dateupdate) from MSysObjects";

          DataTable dt = accessHelper.GetSysDbTime(sql);


          //将集合返回
          return dt;
      }


      public int InsertTable(DataTable dt)
      {
          int size = mysqlHelper.BulkInsert(dt);


          //将集合返回
          return size;
      }

        public bool GetExsitList(String bgNo)
        {
            //构造要查询的sql语句
            string sql = "select * from bg_customs_state where bg_no = " + bgNo + "";
            //使用helper进行查询，得到结果
            DataTable dt = accessHelper.GetDataTable(sql);
          
            //将集合返回
            return dt.Rows.Count>0;
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="mi">ManagerInfo类型的对象</param>
        /// <returns></returns>
        public int Insert(CicData mi)
        {
            //构造insert语句
            string sql = "insert into bg_customs_state(bg_no,electronic_declaration,electronic_declaration_time,computer_review,computer_review_time,scene_receipt,scene_receipt_time,documentary_release,documentary_release_time,release_of_goods,release_of_goods_time,update_date,create_date)";
            sql += " values(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13)";
            //构造sql语句的参数
            OleDbParameter[] ps = //使用数组初始化器
            {
                //new OleDbParameter("@p1", mi.bgNo),
                //new OleDbParameter("@p2", mi.electronicDeclaration ),
                //new OleDbParameter("@p3", mi.electronicDeclarationTime),
                //new OleDbParameter("@p4", mi.computerReview),
                //new OleDbParameter("@p5", mi.computerReviewTime ),
                //new OleDbParameter("@p6", mi.sceneReceipt),
                //new OleDbParameter("@p7", mi.sceneReceiptTime ),
                //new OleDbParameter("@p8", mi.documentaryRelease),
                //new OleDbParameter("@p9", mi.documentaryReleaseTime ),
                //new OleDbParameter("@p10", mi.releaseOfGoods),
                //new OleDbParameter("@p11", mi.releaseOfGoodsTime ),
                //new OleDbParameter("@p12", mi.utime),
                //new OleDbParameter("@p13", mi.ctime )
            };
            //执行插入操作
            return accessHelper.ExcuteNonQuery(sql, ps);
        }

        /// <summary>
        /// 修改管理员，特别注意：密码
        /// </summary>
        /// <param name="mi"></param>
        /// <returns></returns>
        public int Update(CicData mi)
        {
            //为什么要进行密码的判断：
            //答：因为密码值是经过md5加密存储的，当修改时，需要判断用户是否改了密码，如果没有改，则不变，如果改了，则重新进行md5加密

            //定义参数集合，可以动态添加元素
            List<OleDbParameter> listPs = new List<OleDbParameter>();
            //构造update的sql语句
            string sql = "update bg_customs_state set  electronic_declaration=@p1, electronic_declaration_time=@p2, computer_review=@p3, computer_review_time=@p4, scene_receipt=@p5, scene_receipt_time=@p6, documentary_release=@p7, documentary_release_time=@p8";
            //listPs.Add(new OleDbParameter("@p1", mi.electronicDeclaration));
            //listPs.Add(new OleDbParameter("@p2", mi.electronicDeclarationTime));
            //listPs.Add(new OleDbParameter("@p3", mi.computerReview));
            //listPs.Add(new OleDbParameter("@p4", mi.computerReviewTime));
            //listPs.Add(new OleDbParameter("@p5", mi.sceneReceipt));
            //listPs.Add(new OleDbParameter("@p6", mi.sceneReceiptTime));
            //listPs.Add(new OleDbParameter("@p7", mi.documentaryRelease));
            //listPs.Add(new OleDbParameter("@p8", mi.documentaryReleaseTime));
            //继续拼接语句
            sql += ",release_of_goods=@p9 ,release_of_goods_time =@p10 ,update_date=@p11 ,create_date =@p12 where bg_no=@id";
            //listPs.Add(new OleDbParameter("@p9", mi.releaseOfGoods));
            //listPs.Add(new OleDbParameter("@p10", mi.releaseOfGoodsTime));
            //listPs.Add(new OleDbParameter("@p11", mi.utime));
            //listPs.Add(new OleDbParameter("@p12", mi.ctime));
            //listPs.Add(new OleDbParameter("@id", mi.bgNo));

            //执行语句并返回结果
            return accessHelper.ExcuteNonQuery(sql, listPs.ToArray());
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
            OleDbParameter p = new OleDbParameter("@id", id);
            //执行操作
            return accessHelper.ExcuteNonQuery(sql, p);
        }


    }
}
