using DAL;
using MODEL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class CicDataBll
    {

        //创建数据层对象
        CicDataDal miDal = new CicDataDal();

        public List<CicData> GetList()
        {
            //调用查询方法
            return miDal.GetList();
        }

        public DataTable GetDataTable()
        {
            //调用查询方法
            return miDal.GetDataTable();
        }

        public DataTable GetSysDbTime()
        {
            //调用查询方法
            return miDal.GetSysDbTime();
        }
        

        public int insertDataTable(DataTable dt)
        {
            //调用查询方法
            return miDal.InsertTable(dt);
        }

        public bool GetExsitList(String bgNo)
        {
            //调用查询方法
            return miDal.GetExsitList(bgNo);
        }

        public bool Add(CicData mi)
        {
            //调用dal层的insert方法，完成插入操作
            return miDal.Insert(mi) > 0;
        }

        public bool Edit(CicData mi)
        {
            return miDal.Update(mi) > 0;
        }

        public bool Remove(int id)
        {
            return miDal.Delete(id) > 0;
        }


    }
}
