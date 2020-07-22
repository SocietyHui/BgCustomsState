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

namespace DAL
{
  public   class UserInfoDal
    {
      public List<UserInfo> GetList()
        {
            //构造要查询的sql语句
            string sql = "select df_no from bg_customs_state where release_of_goods is null or release_of_goods=''";
            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sql);
            //将dt中的数据转存到list中
            List<UserInfo> list = new List<UserInfo>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new UserInfo()
                {
                    dfNo = row["df_no"].ToString(),
                    
                });
            }
            //将集合返回
            return list;
        }

        public bool GetExsitList(String bgNo)
        {
            //构造要查询的sql语句
            string sql = "select * from bg_customs_state where df_no = " + bgNo + "";
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
        public int Insert(UserInfo mi)
        {
            //构造insert语句
            string sql = "insert into bg_customs_state(df_no,electronic_declaration,electronic_declaration_time,computer_review,computer_review_time,scene_receipt,scene_receipt_time,documentary_release,documentary_release_time,release_of_goods,release_of_goods_time,update_date,create_date)";
            sql += " values(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13)";
            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            {
                new MySqlParameter("@p1", mi.dfNo),
                new MySqlParameter("@p2", mi.electronicDeclaration ),
                new MySqlParameter("@p3", mi.electronicDeclarationTime),
                new MySqlParameter("@p4", mi.computerReview),
                new MySqlParameter("@p5", mi.computerReviewTime ),
                new MySqlParameter("@p6", mi.sceneReceipt),
                new MySqlParameter("@p7", mi.sceneReceiptTime ),
                new MySqlParameter("@p8", mi.documentaryRelease),
                new MySqlParameter("@p9", mi.documentaryReleaseTime ),
                new MySqlParameter("@p10", mi.releaseOfGoods),
                new MySqlParameter("@p11", mi.releaseOfGoodsTime ),
                new MySqlParameter("@p12", mi.utime),
                new MySqlParameter("@p13", mi.ctime )
            };
            //执行插入操作
            return mysqlHelper.ExcuteNonQuery(sql, ps);
        }

        /// <summary>
        /// 修改管理员，特别注意：密码
        /// </summary>
        /// <param name="mi"></param>
        /// <returns></returns>
        public int Update(UserInfo mi)
        {
            //为什么要进行密码的判断：
            //答：因为密码值是经过md5加密存储的，当修改时，需要判断用户是否改了密码，如果没有改，则不变，如果改了，则重新进行md5加密

            //定义参数集合，可以动态添加元素
            List<MySqlParameter> listPs = new List<MySqlParameter>();
            //构造update的sql语句
            string sql = @"update bg_customs_state set  electronic_declaration=@p1, electronic_declaration_time=@p2, computer_review=@p3, computer_review_time=@p4, scene_receipt=@p5,
                         scene_receipt_time=@p6, documentary_release=@p7, documentary_release_time=@p8";
            listPs.Add(new MySqlParameter("@p1", mi.electronicDeclaration));
            listPs.Add(new MySqlParameter("@p2", mi.electronicDeclarationTime));
            listPs.Add(new MySqlParameter("@p3", mi.computerReview));
            listPs.Add(new MySqlParameter("@p4", mi.computerReviewTime));
            listPs.Add(new MySqlParameter("@p5", mi.sceneReceipt));
            listPs.Add(new MySqlParameter("@p6", mi.sceneReceiptTime));
            listPs.Add(new MySqlParameter("@p7", mi.documentaryRelease));
            listPs.Add(new MySqlParameter("@p8", mi.documentaryReleaseTime));
            //继续拼接语句
            sql += ",release_of_goods=@p9 ,release_of_goods_time =@p10 ,update_date=@p11 ,create_date =@p12 where df_no=@id";
            listPs.Add(new MySqlParameter("@p9", mi.releaseOfGoods));
            listPs.Add(new MySqlParameter("@p10", mi.releaseOfGoodsTime));
            listPs.Add(new MySqlParameter("@p11", mi.utime));
            listPs.Add(new MySqlParameter("@p12", mi.ctime));
            listPs.Add(new MySqlParameter("@id", mi.dfNo));

            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql, listPs.ToArray());
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


    }
}
