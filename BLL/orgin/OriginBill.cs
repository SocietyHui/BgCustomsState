using DAl;
using MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
   public class OriginBill
    {

        //创建数据层对象
        OriginDal miDal = new OriginDal();

       //获取报关单号 根据批次 状态 报关单号
       public List<SingleInfo> GetList(SingleInfo carInfo)
        {
            //调用查询方法
            return miDal.GetList(carInfo);
        }


       //获取报关单号 根据批次 状态 报关单号
       public List<SingleInfo> selectChecked(SingleInfo carInfo)
       {
           //调用查询方法
           return miDal.selectChecked(carInfo);
       }



       //获取报关单号
       public List<SingleInfo> GetList()
       {
           //调用查询方法
           return miDal.GetList();
       }

       //获取detailId数据
       public List<SingleInfo> GetListByDfno(SingleInfo carInfo)
       {

           //调用查询方法
           return miDal.GetListByDfno(carInfo);
       }

        //根据批次号+原产地证号，获取项数信息
        public List<SingleInfo> GetListDetail(SingleInfo carInfo)
        {

            //调用查询方法
            return miDal.GetListDetail(carInfo);
        }
        


        public bool UpdateHeadState(SingleInfo mi)
        {
            return miDal.UpdateHeadState(mi) > 0;
        }

        public bool UpdateHeadStateAndNo(SingleInfo mi)
        {
            return miDal.UpdateHeadStateAndNo(mi) > 0;
        }

        public bool UpdateCertificateDetail(SingleInfo mi)
        {
            return miDal.UpdateCertificateDetail(mi) > 0;
        }
        

        public bool UpdateHeadState1(SingleInfo mi)
        {
            return miDal.UpdateHeadState1(mi) > 0;
        }


        public bool UpdateCerNo(SingleInfo mi)
        {
            return miDal.UpdateCerNo(mi) > 0;
        }

    }
}
