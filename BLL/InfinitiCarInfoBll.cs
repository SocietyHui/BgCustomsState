using DAl;
using MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
   public class InfinitiCarInfoBll
    {

        //创建数据层对象
        InfinitiCarInfoDal miDal = new InfinitiCarInfoDal();

       //获取报关单号 根据批次 状态 报关单号
       public List<InfinitiCarInfo> GetList(InfinitiCarInfo carInfo)
        {
            //调用查询方法
            return miDal.GetList(carInfo);
        }


       //获取报关单号 根据批次 状态 报关单号
       public List<CarInfo> selectChecked(CarInfo carInfo)
       {
           //调用查询方法
           return miDal.selectChecked(carInfo);
       }



       //获取报关单号
       public List<InfinitiCarInfo> GetList()
       {
           //调用查询方法
           return miDal.GetList();
       }

       //获取detailId数据
       public List<CarInfo> GetListByDfno(CarInfo carInfo)
       {

           //调用查询方法
           return miDal.GetListByDfno(carInfo);
       }


       //获取vin数据
       public List<CarInfo> GetListByDetailId(CarInfo carInfo)
       {

           //调用查询方法
           return miDal.GetListByDetailId(carInfo);
       }



       public bool GetExsitList(CarInfo carInfo)
        {
            //调用查询方法
            return miDal.GetExsitList(carInfo);
        }

        public bool InsertCarInfo(CarInfo mi)
        {
            //调用dal层的insert方法，完成插入操作
            return miDal.InsertCarInfo(mi) > 0;
        }



        public bool UpdateHeadState(CarInfo mi)
        {
            return miDal.UpdateHeadState(mi) > 0;
        }

        public bool UpdateHeadState1(CarInfo mi)
        {
            return miDal.UpdateHeadState1(mi) > 0;
        }


        public bool UpdateCarInfo(InfinitiCarInfo mi)
        {
            return miDal.UpdateCarInfo(mi) > 0;
        }


        public bool UpdateCerNo(CarInfo mi)
        {
            return miDal.UpdateCerNo(mi) > 0;
        }



        public bool Remove(int id)
        {
            return miDal.Delete(id) > 0;
        }








        public bool UpdateCerInfo(CarInfo mi)
        {
            return miDal.UpdateCerInfo(mi) > 0;
        }
    }
}
