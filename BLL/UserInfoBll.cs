using DAL;
using MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
   public partial class UserInfoBll
    {
        //创建数据层对象
        UserInfoDal miDal = new UserInfoDal();

        public List<UserInfo> GetList()
        {
            //调用查询方法
            return miDal.GetList();
        }

        public bool GetExsitList(String bgNo)
        {
            //调用查询方法
            return miDal.GetExsitList(bgNo);
        }

        public bool Add(UserInfo mi)
        {
            //调用dal层的insert方法，完成插入操作
            return miDal.Insert(mi) > 0;
        }

        public bool Edit(UserInfo mi)
        {
            return miDal.Update(mi) > 0;
        }

        public bool Remove(int id)
        {
            return miDal.Delete(id) > 0;
        }
    }
}
