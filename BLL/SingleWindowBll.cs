using DAl;
using MODEL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
   public class SingleWindowBll
    {

        //创建数据层对象
       SingleWindowDal miDal = new SingleWindowDal();

        //获取报关单号
        public List<SingleWindow> GetList(SingleWindow singleWindow)
        {
            //调用查询方法
            return miDal.GetList(singleWindow);
        }


        //获取报关单号
        public List<SingleWindow> selectChecked(SingleWindow singleWindow)
        {
            //调用查询方法
            return miDal.selectChecked(singleWindow);
        }




        //获取报关单号
        public int UpdateHeadState(SingleWindow singleWindow)
        {
            //调用查询方法
            return miDal.UpdateHeadState(singleWindow);
        }



        //获取报检号
        public List<SingleWindow> GetCiqList(SingleWindow singleWindow)
        {
            //调用查询方法
            return miDal.GetCiqList(singleWindow);
        }


        //获取报关委托协议号数据
        public List<SingleWindow> GetDecDocList(SingleWindow singleWindow)
        {
            //调用查询方法
            return miDal.GetDecDocList(singleWindow);
        }




        //获取detailId数据
        public List<SingleWindow> GetListByDfno(SingleWindow singleWindow)
        {

            //调用查询方法
            return miDal.GetListByDfno(singleWindow);
        }


        //获取vin数据
        public List<SingleWindow> GetListByDetailId(SingleWindow singleWindow)
        {

            //调用查询方法
            return miDal.GetListByDetailId(singleWindow);
        }




        #region //detail数据操作
        //查询detail是否存在
        public List<SingleDetail> GetDetailList(SingleDetail singleDetail)
        {
            //调用查询方法
            return miDal.GetDetailList(singleDetail);
        }



       //查询detail是否存在
        public bool UpdateDetail(SingleDetail singleDetail)
        {
            //调用查询方法
            return miDal.UpdateDetail(singleDetail)>0;
        }

        public string InsertSingleDetail(SingleDetail mi)
        {
            //调用dal层的insert方法，完成插入操作
            return miDal.InsertSingleDetail(mi);
        }

        #endregion



        #region //Goodslimit 数据操作
        //查询Goodslimit是否存在
        public List<SingleGoodslimit> GetGoodslimitList(SingleGoodslimit singleGoodslimit)
        {
            //调用查询方法
            return miDal.GetGoodslimitList(singleGoodslimit);
        }


        public bool InsertSingleGoodslimit(SingleGoodslimit mi)
        {
            //调用dal层的insert方法，完成插入操作
            return miDal.InsertSingleGoodslimit(mi) > 0;
        }


        //更新许可证信息
        public bool UpdateSingleGoodslimit(SingleGoodslimit singleGoodslimit)
        {
            //调用查询方法
            return miDal.UpdateSingleGoodslimit(singleGoodslimit) > 0;
        }
        #endregion



        #region //GoodslimitVin 数据操作
        public bool InsertSingleGoodslimitVin(SingleGoodslimitVin mi)
        {
            //调用dal层的insert方法，完成插入操作
            return miDal.InsertSingleGoodslimitVin(mi) > 0;
        }


        //查询Goodslimit是否存在
        public List<SingleGoodslimitVin> GetGoodslimitListVin(SingleGoodslimitVin singleGoodslimitVin)
        {
            //调用查询方法
            return miDal.GetGoodslimitListVin(singleGoodslimitVin);
        }



        //更新许可证信息
        public bool UpdateSingleGoodslimitVin(SingleGoodslimitVin singleGoodslimitVin)
        {
            //调用查询方法
            return miDal.UpdateSingleGoodslimitVin(singleGoodslimitVin) > 0;
        }
        #endregion





        public bool InsertSingleContainer(SingleContainer mi)
        {
            //调用dal层的insert方法，完成插入操作
            return miDal.InsertSingleContainer(mi) > 0;
        }


        //查询SingleContainer是否存在
        public List<SingleContainer> GetSingleContainer(SingleContainer singleContainer)
        {
            //调用查询方法
            return miDal.GetSingleContainer(singleContainer);
        }



        //更新集装箱信息
        public bool UpdateSingleContainer(SingleContainer singleContainer)
        {
            //调用查询方法
            return miDal.UpdateSingleContainer(singleContainer) > 0;
        }



       //保存Headsinglewindow
        public bool Edit(SingleWindow mi)
        {
            return miDal.Update(mi) > 0;
        }

       //保存head表
        public bool UpadteHeadInfo(SingleWindow mi)
        {
            return miDal.UpadteHeadInfo(mi) > 0;
        }


        public bool Remove(int id)
        {
            return miDal.Delete(id) > 0;
        }

    }
}
