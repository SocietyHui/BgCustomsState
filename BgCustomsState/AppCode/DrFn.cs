using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Collections;
using System.Text;
using System.Configuration;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data.SqlClient;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;


/// <summary>
/// DrFn 的摘要说明
/// </summary>
public class DrFn
{
    #region 构造函数区域
    /// <summary>
    /// 构造函数区域：Construct Function
    /// </summary>
    public DrFn()
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //
    }

    public DrFn(string ConnStr)
    {
        //
        //TODO: 在此处添加构造函数逻辑
        //
    }
    #endregion //构造函数区域：Construct Function

    #region 创建默认数据对象
    public DrSf.LinkSql objSql = new DrSf.LinkSql(DsCg.GetDbConnectionString());
    #endregion //创建数据对象：Created Link Sql Object

    #region 创建新的数据库数据对象，调用LinkSql构造有参方法string ServerIP, string DBName, string UserName, string Password链接到另一个数据库
    public DrSf.LinkSql objSql2 = new DrSf.LinkSql("10.8.20.215","dzys","sa","til2016?");
    #endregion //创建数据对象：Created Link Sql Object

    #region 系统账号操作

    #region User Login Fuction
    /// <summary>
    /// User Login Fuction
    /// </summary>
    /// <param name="userID">User Name</param>
    /// <param name="passWord">Password</param>
    /// <returns>If Success:true:Success;false:failed</returns>
    public bool UserLogin(string userID, string passWord)
    {
        try
        {
            bool bolIfRight = false;

            bolIfRight = WriteUserID(userID);

            return bolIfRight;
        }
        catch
        {
            return false;
        }
    }

    public bool MemberLogin(string MemberId)
    {
        try
        {
            bool bolIfRight = false;

            bolIfRight = DsCk.WriteInfo2Cookie("MBID", "MBID", MemberId);

            return bolIfRight;
        }
        catch
        {
            return false;
        }
    }
    #endregion //User Login Fuction

    #region Write user ID to cookie
    /// <summary>
    /// Write user ID to cookie
    /// </summary>
    /// <param name="UserID"></param>
    /// <returns>True:Successful;False:Failed</returns>
    public bool WriteUserID(string UserID)
    {
        bool bolIfRight = false;

        try
        {
            System.Web.Security.FormsAuthentication.SetAuthCookie(UserID, false);
            bolIfRight = true;
        }
        catch
        {

        }
        return bolIfRight;
    }

    #endregion //Write user ID to cookie

    #region Get user ID from cookie
    /// <summary>
    /// Get user ID from cookie
    /// </summary>
    /// <returns>User ID</returns>
    public string GetUserID()
    {
        return System.Web.HttpContext.Current.User.Identity.Name.ToString();
    }

    public string GetMemberID()
    {
        return DsCk.ReadInfo2Cookie("MBID", "MBID");
    }
    #endregion //Get user ID from cookie

    #region Clear user ID from cookie
    /// <summary>
    /// Clear user ID from cookie
    /// </summary>
    /// <returns></returns>
    public void LogOutUserID()
    {
        System.Web.Security.FormsAuthentication.SignOut();
    }

    public void LogOutMemberID()
    {
        DsCk.RemoveInfo4AllCookie("MBID");
        //DsCk.RemoveInfoByCookieItemId("MBID","MBID");
    }
    #endregion //Clear user ID from cookie

    #region Write ARID to cookie
    /// <summary>
    /// Write ARID ID to cookie
    /// </summary>
    /// <param name="ARID">Area ID</param>
    /// <returns>True:Successful;False:Failed</returns>
    public bool SetArid(string ARID)
    {
        bool bolIfRight = false;

        try
        {
            DsCk.SetArid4Srs(ARID);
            bolIfRight = true;
        }
        catch
        {

        }
        return bolIfRight;
    }
    #endregion //Write user ID to cookie

    #region Get ARID from cookie
    /// <summary>
    /// Get ARID from cookie
    /// </summary>
    /// <returns>ARID</returns>
    public string GetArid()
    {
        return DsCk.ReadArid4Srs();
    }
    #endregion //Get ARID from cookie

    #region Clear ARID from cookie
    /// <summary>
    /// Clear ARID ID from cookie
    /// </summary>
    /// <returns></returns>
    public void ClearArid()
    {
        DsCk.RemoveArid();
    }
    #endregion //Clear ARID ID from cookie

    #region Check user password
    /// <summary>
    /// 验证前台用户名和密码
    /// </summary>
    /// <param name="UserID">User ID</param>
    /// <param name="Password">Password</param>
    /// <returns>1：用户名密码正确；2：用户名错误；3：密码错误；</returns>
    public string ChkUrPsw(string UserID, string Password)
    {
        try
        {
            DataTable dt = objSql.GetDataTable(UserID);
            string strResult = "";

            if (dt.Rows.Count != 0)
            {
                strResult = "3";

                string strPassword = dt.Rows[0]["PSWD"].ToString().Trim();

                Password = getMd5Of16(Password);

                if (strPassword == Password)
                {
                    strResult = "1";
                }
            }
            else
            {
                strResult = "2";
            }
            return strResult;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    #endregion //Check user password

    #region Check user password2
    /// <summary>
    /// 验证前台用户名和密码
    /// </summary>
    /// <param name="UserID">User ID</param>
    /// <param name="Password">Password</param>
    /// <returns>1：用户名密码正确；2：用户名错误；3：密码错误；</returns>
    public string ChkUrPswd(string UserID, string Password)
    {
        try
        {
            DataTable dt = objSql.GetDataTable("select * from INSP_EMP where USNM='"+UserID+"'");
            string strResult = "";

            if (dt.Rows.Count != 0)
            {
                strResult = "3";

                string strPassword = dt.Rows[0]["PASS"].ToString().Trim();

                //Password = getMd5Of16(Password);

                if (strPassword == Password)
                {
                    strResult = "1";
                }
            }
            else
            {
                strResult = "2";
            }
            return strResult;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    #endregion //Check user password

    #region Check user password1
    /// <summary>
    /// 验证前台用户名和密码
    /// </summary>
    /// <param name="UserID">User ID</param>
    /// <param name="Password">Password</param>
    /// <returns>1：用户名密码正确；2：用户名错误；3：密码错误；</returns>
    public string ChkUrPsw(string UserID, string Password, string strROID)
    {
        try
        {
            DataTable dt = objSql.GetUserInfro(UserID);//传入参数userID查询
            string strResult = "";

            if (dt.Rows.Count != 0)
            {
                strResult = "3";

                string strPassword = dt.Rows[0]["PSWD"].ToString().Trim();

                Password = getMd5Of16(Password);

                if (strPassword == Password)
                {
                    if (strROID == "01" || strROID == "02" || strROID == "00" || strROID == "03")
                    {
                        strResult = "0";
                    }
                    else if (strROID == "A1" || strROID == "B1" || strROID == "B2" || strROID == "B3")
                    {
                        strResult = "1";
                    }
                }
            }
            else
            {
                strResult = "2";
            }
            return strResult;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    #endregion //Check user password

    #region 得到用户序号
    /// <summary>
    /// 得到用户序号
    /// </summary>
    /// <param name="UserInfo">传入的用户信息：用户序号，用户账号，昵称，电话，微信，QQ等</param>
    /// <param name="InfoType">传入信息类型：URSN：用户序号；URID：用户账号；STNA：昵称:；PHNO：电话:；WXID：微信；QQID：QQ；</param>
    /// <param name="ReturnValues">需要传出的信息：URSN：用户序号；URID：用户账号；STNA：昵称:；PHNO：电话:；WXID：微信；QQID：QQ；</param>
    /// <returns></returns>
    public string GetURSN(string UserInfo, string InfoType, string ReturnValues)
    {
        string strURSN = "";

        strURSN = QueryFiled("MA12UH", InfoType, UserInfo, ReturnValues);

        return strURSN;
    }
    #endregion //得到用户序号

    #region 检测用户是否登录
    /// <summary>
    /// 检测用户是否登录
    /// </summary>
    /// <returns>:登录：true;没有登录:false</returns>
    public bool CheckUserLogin()
    {
        bool bolIfLgin = false;
        string strURSN = GetUserID();

        if (!String.IsNullOrEmpty(strURSN))
        {
            bolIfLgin = true;
        }

        return bolIfLgin;
    }
    //LTTPWH页面定义的方法插入系统日志
    public bool LgSave(string LGTY, string LGMS)
    {
        bool bolIfOk = false;

        #region 变量定义
        string strURID = GetUserID();

        string strTableName = "OS61LG";
        ArrayList aryInsertFiled = new ArrayList();
        ArrayList aryInsertValues = new ArrayList();
        string strLGID = "";
        string strLGFG = "";
        string strLGMS = "";
        string strURSN = "";
        string strLGTY = "";



        string strCRID = "";
        string strCGID = "";
        #endregion //变量定义

        #region 变量赋值
        strLGID = GetFlowNo("LB2B", "LGMG", "LGID");
        strURSN = strURID;
        strLGFG = "Y";
        strLGMS = LGMS;
        strCRID = strURID;
        strCGID = strURID;
        strLGTY = LGTY;
        #endregion //变量赋值

        #region 变量处理
        strLGID = FmatStr(strLGID);
        strURSN = FmatStr(strURSN);
        strLGFG = FmatStr(strLGFG);
        strLGMS = FmatStr(strLGMS);
        strLGTY = FmatStr(strLGTY);

        #endregion //变量处理

        #region 变量验证

        #region 非空验证
        //主键的非空验证
        //if (strTPNA == "")
        //{
        //    lblWarning.Text = "单品图片 不允许为空！";
        //    return;
        //}
        #endregion //非空验证

        #region 整型验证
        //if (!objDsFt.CheckInt(变量名))
        //{
        //	lblWarning.Text = "变量名 必须是整型数据";
        //	return;
        //}
        #endregion //整型验证

        #region 浮点验证
        //if (!objDsFt.CheckDeciaml(变量名))
        //{
        //	lblWarning.Text = "变量名 必须是数值型数据";
        //	return;
        //}
        #endregion //浮点验证

        #region 逻辑验证
        //if (strPLFG.Length >= 30)
        //{
        //ClientScript.RegisterStartupScript(typeof(string), "alert", "<script>alert('图片描述最多只能输入30个字符，您超过了限定长度！')</script>");
        //lblWarning.Text = "图片描述最多只能输入30个字符，您超过了限定长度！";
        //return;
        //}

        #endregion //逻辑验证

        #region 主键验证
        //if (objDsFt.CheckCodeExist(strTableName, "SYID+TPID+TPTY+URSN", strSYID + strTPID + strTPTY + strURSN))
        //{
        //    lblWarning.Text = "主键重复,请检查!";
        //    return;
        //}
        #endregion //主键验证

        #endregion //变量验证

        #region 变量组合
        aryInsertFiled.Add("LGID");
        aryInsertFiled.Add("URSN");
        aryInsertFiled.Add("LGFG");
        aryInsertFiled.Add("LGMS");
        aryInsertFiled.Add("CRID");
        aryInsertFiled.Add("CGID");
        aryInsertFiled.Add("ATFG");
        aryInsertFiled.Add("LGTY");


        aryInsertValues.Add(strLGID);
        aryInsertValues.Add(strURSN);
        aryInsertValues.Add(strLGFG);
        aryInsertValues.Add(strLGMS);
        aryInsertValues.Add(strCRID);
        aryInsertValues.Add(strCGID);
        aryInsertValues.Add("Y");
        aryInsertValues.Add(strLGTY);
        #endregion //变量组合

        #region 执行保存
        bolIfOk = InsertRecode(strTableName, aryInsertFiled, aryInsertValues);

        #endregion //执行保存

        UpdateMaxFlowNo("LB2B", "LGMG", "LGID");

        return bolIfOk;
        //UpdateMaxFlowNo("LB2B", "LGMG", "LGID");


    }
    #endregion //检测用户是否登录

    #endregion //系统账号操作

    #region 公用数据操作 *

    #region 查询数据

    /// <summary>
    /// 查询数据
    /// </summary>
    /// <param name="TableName">查询的表名</param>
    /// <param name="WhereFiled">查询的字段名</param>
    /// <param name="WhereType">查询类别：1:单值查询;2:多值查询;3:模糊查询;4:右模糊查询;5:左模糊查询;6:区间查询;7:空值查询,8:非空查询,9:排除查询; A: 组合好的条件，如 （ ATFG = 'Y' ）等;</param>
    /// <param name="WhereValues">查询的值</param>
    /// <returns></returns>
    public DataTable QueryTable(string TableName, ArrayList WhereFiled, ArrayList WhereType, ArrayList WhereValues)
    {
        try
        {
            DataTable dt = objSql.QueryTable(TableName, WhereFiled, WhereType, WhereValues);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 查询数据
    /// </summary>
    /// <param name="TableName">查询的表名</param>
    /// <param name="WhereFiled">查询的字段名</param>
    /// <param name="WhereType">查询类别：1:单值查询;2:多值查询;3:模糊查询;4:右模糊查询;5:左模糊查询;6:区间查询;7:空值查询,8:非空查询,9:排除查询; A: 组合好的条件，如 （ ATFG = 'Y' ）等;</param>
    /// <param name="WhereValues">查询的值</param>
    /// <returns></returns>
    public DataTable QueryTable(string TableName, string WhereFiled, string WhereType, string WhereValues)
    {
        try
        {
            DataTable dt = objSql.QueryTable(TableName, WhereFiled, WhereType, WhereValues);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    #region 得到字段值
    /// <summary>
    /// 根据字段值得到字段文本
    /// </summary>
    /// <param name="TableName">要得到的字段对应的</param>
    /// <param name="QueryFiledName">查询条件的字段名</param>
    /// <param name="QueryFiledValue">查询条件的字段值</param>
    /// <param name="GetFiledName">要得到的字段的字段名</param>
    /// <returns>要得到的字段的字段值</returns>
    public string QueryFiled(string TableName, string QueryFiledName, string QueryFiledValue, string GetFiledName)
    {
        string strText = "";

        StringBuilder sbSql = new StringBuilder("");
        sbSql.AppendFormat(" SELECT TOP 1 {0} FROM {1} WHERE {2} = '{3}' ", GetFiledName, TableName, QueryFiledName, QueryFiledValue);

        strText = objSql.GetDataFiled(sbSql.ToString());

        return strText;
    }
    #endregion //得到字段值

    #endregion //查询数据

    #region 插入数据
    /// <summary>
    /// 插入数据
    /// </summary>
    /// <param name="TableName">表名</param>
    /// <param name="WhereFiled">字段列表</param>
    /// <param name="WhereValues">字段值</param>
    /// <returns></returns>
    public bool InsertRecode(string TableName, ArrayList InsertFiled, ArrayList InsertValues)
    {
        try
        {
            bool bolIfSuccess = false;
            bolIfSuccess = objSql.InsertRecode(TableName, InsertFiled, InsertValues);
            return bolIfSuccess;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion //插入数据

    #region 更改数据
    /// <summary>
    /// 更改数据
    /// </summary>
    /// <param name="TableName">更改的表名</param>
    /// <param name="UpdateFiled">更改的字段名</param>
    /// <param name="UpdateValues">更改的值</param>
    /// <param name="WhereFiled">条件字段列表</param>
    /// <param name="WhereValues">条件字段值</param>
    /// <returns>更改完成的行数</returns>
    public bool UpdateRecode(string TableName, ArrayList UpdateFiled, ArrayList UpdateValues, ArrayList WhereFiled, ArrayList WhereValues)
    {
        try
        {
            bool bolIfUpdateOk = false;

            bolIfUpdateOk = objSql.UpdateRecode(TableName, UpdateFiled, UpdateValues, WhereFiled, WhereValues);

            return bolIfUpdateOk;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 更改数据
    /// </summary>
    /// <param name="TableName">更改的表名</param>
    /// <param name="UpdateFiled">更改的字段名</param>
    /// <param name="UpdateValues">更改的值</param>
    /// <param name="WhereFiled">条件字段列表</param>
    /// <param name="WhereValues">条件字段值</param>
    /// <returns>更改完成的行数</returns>
    public bool UpdateRecode(string TableName, string UpdateFiled, string UpdateValues, string WhereFiled, string WhereValues)
    {
        try
        {
            bool bol = objSql.UpdateRecode(TableName, UpdateFiled, UpdateValues, WhereFiled, WhereValues);

            return bol;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    /// <summary>
    /// 更改数据到另一个数据库
    /// </summary>
    /// <param name="TableName">更改的表名</param>
    /// <param name="UpdateFiled">更改的字段名</param>
    /// <param name="UpdateValues">更改的值</param>
    /// <param name="WhereFiled">条件字段列表</param>
    /// <param name="WhereValues">条件字段值</param>
    /// <returns>更改完成的行数</returns>
    public bool UpdateRecode2(string TableName, string UpdateFiled, string UpdateValues, string WhereFiled, string WhereValues)
    {
        try
        {
            bool bol = objSql2.UpdateRecode(TableName, UpdateFiled, UpdateValues, WhereFiled, WhereValues);

            return bol;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    #endregion //更改数据



    #region 删除数据
    /// <summary>
    /// 删除数据
    /// </summary>
    /// <param name="TableName">表名</param>
    /// <param name="KeyFiledName">主键</param>
    /// <param name="KeyFieldValues">条件</param>
    /// <returns></returns>
    public bool DeleteRecode(string TableName, string KeyFiledName, string KeyFieldValues)
    {
        try
        {
            bool bolIfDelete = false;

            bolIfDelete = objSql.DeleteRecode(TableName, KeyFiledName, KeyFieldValues);

            return bolIfDelete;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    /// <summary>
    /// 删除记录
    /// </summary>
    /// <param name="TableName">表名</param>
    /// <param name="DeleteWhereFiled">条件字段列表</param>
    /// <param name="DeleteWhereValues">条件字段值</param>
    /// <returns></returns>
    public bool DeleteRecode(string TableName, ArrayList DeleteWhereFiled, ArrayList DeleteWhereValues)
    {
        try
        {
            bool bolIfDelete = false;

            bolIfDelete = objSql.DeleteRecode(TableName, DeleteWhereFiled, DeleteWhereValues);

            return bolIfDelete;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion //更改数据

    #region 验证字段是否存在
    /// <summary>
    /// 验证表中字段值是否存在
    /// </summary>
    /// <param name="TBID">表名</param>
    /// <param name="FLID">字段名</param>
    /// <param name="FLVA">字段值</param>
    /// <returns>If Exist: Existed; false; No Existed: true;</returns>
    public bool CheckCodeExist(string TBID, string FLID, string FLVA)
    {
        try
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("Select Distinct {0} ", FLID)
            .AppendFormat("From {0} Where {1}='{2}'  ", TBID, FLID, FLVA);

            DataTable dt = objSql.GetDataTable(sbSql.ToString());

            if (dt.Rows.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    #endregion


    #region 根据时间验证字段是否存在
    /// <summary>
    /// 验证表中字段值是否存在
    /// </summary>
    /// <param name="TBID">表名</param>
    /// <param name="FLID">字段名</param>
    /// <param name="FLVA">字段值</param>
    /// <returns>If Exist: Existed; false; No Existed: true;</returns>
    public bool CheckCodeExistNew(string TBID, string FLID, string FLVA, string FLVI, string FLVIN, string FLVK, string FLVAK, string FLTM)
    {
        try
        {
            StringBuilder sbSql = new StringBuilder();
           // string sbSql = "Select Distinct FLID From (Select * From INSP_REC Where CGTM > 'FLTM') T  Where t.FLID='FLVA'and t.FLVI='FLVIN'  ";
            sbSql.AppendFormat("Select Distinct {0} From (Select * From {1} Where CRTM > '{2}' and ATFG='Y') T Where t.{3}='{4}'and t.{5}='{6}' and t.{7}='{8}' ", FLID, TBID, FLTM, FLID, FLVA, FLVI, FLVIN, FLVK, FLVAK);

           // Select Distinct 操作编号 From (Select * From INSP_REC Where CGTM > '2017-03-29 ') T  Where t.自动编号='46' and t.库别编号='01'
            DataTable dt = objSql.GetDataTable(sbSql.ToString());

            if (dt.Rows.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }

    #endregion


    #region nico新建验证字段是否存在
    /// <summary>
    /// 验证表中字段值是否存在
    /// </summary>
    /// <param name="TBID">表名</param>
    /// <param name="FLID">字段名</param>
    /// <param name="FLVA">字段值</param>
    /// <returns>If Exist: Existed; false; No Existed: true;</returns>
    public bool CheckCodeExistNew(string TBID, string FLID, string FLVA, string FLVI, string FLVIN)
    {
        try
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("Select Distinct {0} ", FLID)
            .AppendFormat("From {0} Where {1}='{2}'and {3}='{4}'  ", TBID, FLID, FLVA, FLVI, FLVIN);

            DataTable dt = objSql.GetDataTable(sbSql.ToString());

            if (dt.Rows.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch
        {
            return false;
        }
    }
    #endregion //Check key filed in table

    #region 得到流水号
    /// <summary>
    /// 得到流水号
    /// </summary>
    /// <param name="SYID">系统编号</param>
    /// <param name="MDID">模块代码</param>
    /// <param name="TPID">类别代码</param>
    /// <returns>当前单证流水号</returns>
    public string GetFlowNo(string SYID, string MDID, string TPID)
    {
        try
        {
            StringBuilder SQLString = new StringBuilder();

            SQLString.AppendFormat("SELECT FNTT + FNYM + MXNO FROM CA13NR WHERE  SYID = '{0}' AND MDID = '{1}' AND TPID = '{2}'", SYID, MDID, TPID);

            string MaxNo = objSql.GetDataFiled(SQLString.ToString().Trim()).ToString().Trim();

            return MaxNo;
        }
        catch (Exception)
        {
            throw;
        }
    }
    public string GetFlowNUM(string SYID, string MDID, string TPID)
    {
        try
        {
            StringBuilder SQLString = new StringBuilder();

            SQLString.AppendFormat("SELECT FRNO FROM CA13NR WHERE  SYID = '{0}' AND MDID = '{1}' AND TPID = '{2}'", SYID, MDID, TPID);

            string MaxNo = objSql.GetDataFiled(SQLString.ToString().Trim()).ToString().Trim();

            return MaxNo;
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion //得到流水号

    #region 更新流水号
    /// <summary>
    /// 更新周鹤流水0005编号
    /// </summary>
    /// <param name="MDID">模块名</param>
    /// <param name="TPID">ID名</param>
    /// <returns>所需流水号</returns>
    public bool UpdateFlowNo(string SYID, string MDID, string TPID)
    {
        try
        {
            StringBuilder SQLString = new StringBuilder();

            SQLString.Append("Update CA13NR  Set MXNO= REPLICATE('0', LEN(MXNO) - LEN((CASE WHEN FNDR = '-' ")
                .Append("THEN ((CONVERT(int, MXNO) - CONVERT(int, FNST))) ELSE ((CONVERT(int, MXNO) + CONVERT(int, FNST))) END)))")
                .Append(" + CAST((CASE WHEN FNDR = '-' THEN ((CONVERT(int, MXNO) - CONVERT(int, FNST))) ELSE ((CONVERT(int, MXNO)")
                .Append(" + CONVERT(int, FNST))) END) AS varchar) ,CRTM =GETDATE() FROM CA13NR WHERE ")
                .AppendFormat(" (SYID = '{0}') AND (MDID = '{1}') AND (TPID = '{2}') ", SYID, MDID, TPID);

            bool bolIfOk = objSql.ExecSQL(SQLString.ToString());
            return bolIfOk;
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion //更新流水号


    #region 更新流水号
    /// <summary>
    /// 更新nico流水5编号
    /// </summary>
    /// <param name="MDID">模块名</param>
    /// <param name="TPID">ID名</param>
    /// <returns>所需流水号</returns>
    public bool UpdateMaxFlowNo(string SYID, string MDID, string TPID)
    {
        try
        {
            StringBuilder SQLString = new StringBuilder();

            SQLString.Append("Update CA13NR  Set MXNO= ")
                .Append("  CAST((CASE WHEN FNDR = '-' THEN ((CONVERT(int, MXNO) - CONVERT(int, FNST))) ELSE ((CONVERT(int, MXNO)")
                .Append(" + CONVERT(int, FNST))) END) AS varchar) ,CRTM =GETDATE() FROM CA13NR WHERE ")
                .AppendFormat(" (SYID = '{0}') AND (MDID = '{1}') AND (TPID = '{2}') ", SYID, MDID, TPID);

            bool bolIfOk = objSql.ExecSQL(SQLString.ToString());
            return bolIfOk;
        }
        catch (Exception)
        {
            throw;
        }
    }
    #endregion //更新流水号


    public bool UpdateWGID(string WGID, string MDID, string NUM)
    {
        try
        {
            if (NUM=="1")
            {
                string SQLString = "update CA13NR set FRNO ='" + WGID + "' where MDID='" + MDID + "'";
                bool bolIfOk = objSql.ExecSQL(SQLString.ToString());
                return bolIfOk;
            }
            else
            {
                string SQLString = "update CA13NR set MXNO ='" + WGID + "' where MDID='" + MDID + "'";
                bool bolIfOk = objSql.ExecSQL(SQLString.ToString());
                return bolIfOk;
            }


            
        }
        catch (Exception)
        {
            throw;
        }
    }
    #region 得到明细号
    /// <summary>
    /// 根据单号得到当前明细号
    /// </summary>
    /// <param name="TableName">需要得到的明细号对应的表名</param>
    /// <param name="KeyFiledName">主键的字段名</param>
    /// <param name="KeyFiledValue">主键的字段值</param>
    /// <param name="ItemFiledName">明细字段的字段名</param>
    /// <param name="ItemLength">明细的长度：如果为0代表不计字段长度，当前单号有多长算多长</param>
    /// <returns>明细单号</returns>
    public string GetItemNoById(string TableName, string KeyFiledName, string KeyFiledValue, string ItemFiledName, int ItemLength)
    {
        string strText = "";

        StringBuilder sbSql = new StringBuilder("");
        //sbSql.AppendFormat(" SELECT {0} FROM {1} WHERE {2} = '{3}' ", GetFiledName, TableName, QueryFiledName, QueryFiledValue);

        if (ItemLength == 0)
        {
            sbSql.AppendFormat("SELECT ISNULL(MAX([{0}]),0) + 1 FROM {1} WHERE {2} = '{3}'", ItemFiledName, TableName, KeyFiledName, KeyFiledValue);
        }
        else
        {
            sbSql.AppendFormat("SELECT RIGHT('0000000000' + CONVERT(VARCHAR,(ISNULL(MAX([{0}]),0) + 1)),{4}) FROM {1} WHERE {2} = '{3}'", ItemFiledName, TableName, KeyFiledName, KeyFiledValue, ItemLength);
        }

        strText = objSql.GetDataFiled(sbSql.ToString());

        return strText;


        //string strPlNo = ""; SELECT ISNULL(MAX([OUIT]),0) + 1 FROM OWI1OI WHERE INID = 'IN000001'
        //string strSql = "SELECT RIGHT('000' + CONVERT(VARCHAR,(ISNULL(MAX(PLIT),0) + 1)),3) FROM ODOI WHERE PLNO = '" + PlNo + "'  ";
        //strPlNo = objSql.GetDataFiled(strSql);
        //return strPlNo;
    }
    #endregion //得到明细号

    #region 得到字段值
    /// <summary>
    /// 根据单号得到当前明细号
    /// </summary>
    /// <param name="TableName">需要得到的明细号对应的表名</param>
    /// <param name="KeyFiledName">主键的字段名</param>
    /// <param name="KeyFiledValue">主键的字段值</param>
    /// <param name="ItemFiledName">明细字段的字段名</param>
    /// <param name="ItemLength">明细的长度：如果为0代表不计字段长度，当前单号有多长算多长</param>
    /// <returns>明细单号</returns>
    public string GetFiledById(string TableName, string KeyFiledName, string KeyFiledValue, string ReturnFiled)
    {
        string strText = "";

        StringBuilder sbSql = new StringBuilder("");

        try
        {
            sbSql.AppendFormat("SELECT TOP 1 {0} FROM {1} WHERE ATFG ='Y' AND {2} ='{3}' ", ReturnFiled, TableName, KeyFiledName, KeyFiledValue);

            strText = objSql.GetDataFiled(sbSql.ToString());
        }
        catch
        {
        }
        return strText;


        //string strPlNo = ""; SELECT ISNULL(MAX([OUIT]),0) + 1 FROM OWI1OI WHERE INID = 'IN000001'
        //string strSql = "SELECT RIGHT('000' + CONVERT(VARCHAR,(ISNULL(MAX(PLIT),0) + 1)),3) FROM ODOI WHERE PLNO = '" + PlNo + "'  ";
        //strPlNo = objSql.GetDataFiled(strSql);
        //return strPlNo;
    }
    #endregion //得到明细号

    #endregion //公用数据操作

    #region 基本数据操作 *

    #region 执行SQL语句返回DataSet
    /// <summary>
    /// 执行SQL语句返回DataSet
    /// </summary>
    /// <param name="SQL">要查询的SQL语句</param>
    /// <returns>返回SQL语句查询的结果</returns>
    public DataSet GetDataSet(string strSQL)
    {


        try
        {
            DataSet dstTemp = objSql.GetDataSet(strSQL);

            return dstTemp;
        }
        catch (Exception ex)
        {
            throw (new Exception("查询数据库失败！　失败原因：" + ex.Message));
        }
    }
    #endregion //执行SQL语句返回DataSet

    #region 执行SQL语句返回DataSet访问另一个dzys数据库
    /// <summary>
    /// 执行SQL语句返回DataSet
    /// </summary>
    /// <param name="SQL">要查询的SQL语句</param>
    /// <returns>返回SQL语句查询的结果</returns>
    public DataSet GetDataSet2(string strSQL)
    {


        try
        {
            DataSet dstTemp = objSql2.GetDataSet(strSQL);

            return dstTemp;
        }
        catch (Exception ex)
        {
            throw (new Exception("查询数据库失败！　失败原因：" + ex.Message));
        }
    }
    #endregion //执行SQL语句返回DataSet

    #region 执行SQL语句返回DataTable访问另一个dzys数据库
    /// <summary>
    /// 执行SQL语句返回DataTable
    /// </summary>
    /// <param name="SQL">要查询的SQL语句</param>
    /// <returns>返回SQL语句执行的结果</returns>
    public DataTable GetDataTable2(string strSQL)
    {

        try
        {
            DataSet dstTemp = GetDataSet2(strSQL);

            DataTable dtTemp = dstTemp.Tables[0];

            return dtTemp;
        }
        catch (Exception ex)
        {
            throw (new Exception("查询数据库失败！　失败原因：" + ex.Message));
        }
    }
    #endregion //执行SQL语句返回


    #region 执行SQL语句返回DataTable
    /// <summary>
    /// 执行SQL语句返回DataTable
    /// </summary>
    /// <param name="SQL">要查询的SQL语句</param>
    /// <returns>返回SQL语句执行的结果</returns>
    public DataTable GetDataTable(string strSQL)
    {

        try
        {
            DataSet dstTemp = GetDataSet(strSQL);

            DataTable dtTemp = dstTemp.Tables[0];

            return dtTemp;
        }
        catch (Exception ex)
        {
            throw (new Exception("查询数据库失败！　失败原因：" + ex.Message));
        }
    }
    #endregion //执行SQL语句返回

    #region 执行SQL语句返回字符串
    /// <summary>
    /// 执行SQL语句返回字符串
    /// </summary>
    /// <param name="SQL">要查询的SQL语句</param>
    /// <returns>返回SQL语句执行的结果</returns>
    public string GetDataFiled(string strSQL)
    {
        string strFiled = "";

        try
        {
            DataTable dt = GetDataTable(strSQL);

            if (dt.Rows.Count != 0)
            {
                strFiled = dt.Rows[0][0].ToString().Trim();
            }
            return strFiled;
        }
        catch (Exception ex)
        {
            return "查询数据库失败";
            //throw (new Exception("查询数据库失败！　失败原因：" + ex.Message));
        }
    }
    #endregion //执行SQL语句返回DataTable

    #region 执行无返回值的SQL语句
    /// <summary>
    /// 执行无返回值的SQL语句
    /// </summary>
    /// <param name="SQL">要执行的SQL语句</param>
    /// <returns>是否执行成功</returns>
    public bool ExecSQL(string SQL)
    {
        bool bol = false;

        try
        {
            bol = objSql.ExecSQL(SQL);
        }
        catch (Exception ex)
        {

            throw (new Exception("SQL语句执行失败！　失败原因：" + ex.Message));
        }

        return bol;
    }
    #endregion //执行无返回值的SQL语句

    #region 执行无返回值的SQL ArrayList
    /// <summary>
    /// 执行无返回值的SQL ArrayList
    /// </summary>
    /// <param name="arySQL">要执行的ArrayList SQL</param>
    /// <returns>是否执行成功</returns>
    public bool ExecArrayListSQL(ArrayList arySQL)
    {

        bool bol = false;

        try
        {
            bol = objSql.ExecArrayListSQL(arySQL);
        }
        catch (Exception ex)
        {
            throw (new Exception("SQL语句执行失败！　失败原因：" + ex.Message));
        }

        return bol;
    }
    #endregion //执行无返回值的SQL ArrayList

    #endregion //基本数据操作

    #region 常用数据处理 *

    #region 字符串格式化
    /// <summary>
    ///  字符串格式化：防止注入式攻击
    /// </summary>
    /// <param name="InputString">需要格式化的字符串</param>
    /// <returns>处理后的字符串</returns>
    public string FmatStr(string InputString)
    {
        try
        {
            InputString = InputString.Replace("'", "''").Trim();
            InputString = InputString.Replace("*", " ").Trim();

            return InputString;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion //字符串格式化

    #region 数据整型验证
    /// <summary>
    /// 数据整型验证
    /// </summary>
    /// <param name="InputStr">输入字符串</param>
    /// <returns>验证结果：True:是整型;False:非整型</returns>
    public bool CheckInt(string InputStr)
    {
        bool bolIfInt = false;
        try
        {
            int intTemp = Int32.Parse(InputStr);
            bolIfInt = true;
        }
        catch
        {
            bolIfInt = false;
        }
        return bolIfInt;
    }
    #endregion //数据整型验证

    #region 数据浮点验证
    /// <summary>
    /// 数据浮点验证
    /// </summary>
    /// <param name="InputStr">输入字符串</param>
    /// <returns>过滤后decimal数值</returns>
    public bool CheckDeciaml(string InputStr)
    {
        bool bolIfDec = false;
        string OutputStr;
        OutputStr = InputStr.Trim().Replace("\'", "").Replace("--", "").Replace(@"/*", "")
            .Replace(@"*/", "").Replace(@"'", "").Replace("，", ",").Replace("。", ".")
            .Replace("　", "").Replace(" ", "").ToUpper();
        try
        {
            decimal OutputDeciaml = Convert.ToDecimal(OutputStr);
            bolIfDec = true;
        }
        catch
        {
            bolIfDec = false;
        }
        return bolIfDec;
    }
    #endregion //数据浮点验证

    #region 得到的文件名
    /// <summary>
    /// 得到的文件名
    /// </summary>
    /// <param name="FileAllPath">文件全路径</param>
    /// <returns>文件名</returns>
    public string GetFileName(string FileAllPath)
    {
        try
        {
            string strFileName = "";

            string[] strID = FileAllPath.Split(new char[] { '\\' });

            if (strID.Length > 0)
            {
                strFileName = strID[(strID.Length - 1)].ToString().Trim();

                string[] strTemp = strFileName.Split(new char[] { '.' });

                if (strTemp.Length > 0)
                {
                    strFileName = strTemp[0].ToString().Trim();
                }
            }

            return strFileName;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion //得到的文件名

    #region 得到文件扩展名
    /// <summary>
    /// 得到文件扩展名
    /// </summary>
    /// <param name="FileAllPath">文件的物理路径</param>
    /// <returns>文件扩展名</returns>
    public string GetFileExName(string FileAllPath)
    {
        try
        {
            string strFileExName = "";

            string[] strID = FileAllPath.Split(new char[] { '.' });

            if (strID.Length > 0)
            {
                strFileExName = strID[(strID.Length - 1)].ToString().Trim();
            }

            return strFileExName;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion //得到文件扩展名

    #endregion //常用数据处理

    #region 系统菜单操作

    #region 得到菜单信息
    /// <summary>
    /// 根据用户名得到菜单信息
    /// </summary>
    /// <param name="URSN">用户名</param>
    /// <returns>菜单信息</returns>
    public DataTable GetMenuInfo(string URSN)
    {
        try
        {
            DataTable dt = new DataTable();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT * FROM vmMenu WHERE (VISI = 'Y') AND  AUTP <>'' AND (URSN = '{0}') ORDER BY LAYE,PARE,SORT ", URSN);

            dt = GetDataTable(sb.ToString());
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 根据用户编号和父级菜单编号得到菜单信息
    /// </summary>
    /// <param name="URSN">用户编号.</param>
    /// <param name="PARE">父级菜单</param>
    /// <returns>菜单信息</returns>
    public DataTable GetMenuInfo(string URSN, string PARE)
    {
        try
        {
            DataTable dt = new DataTable();

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT * FROM vmMenu WHERE (VISI = 'Y') AND  AUTP <>'' AND (URSN = '{0}') AND PARE LIKE '{1}%' ORDER BY LAYE,PARE,SORT ", URSN, PARE);

            dt = GetDataTable(sb.ToString());
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// 根据菜单编号得到父级菜单名称
    /// </summary>
    /// <param name="MUID">菜单编号</param>
    /// <returns>父级菜单名称</returns>
    public string GetPatxInfo(string MUID)
    {
        string strPatx = "";

        try
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("SELECT MUTX FROM MA11MH WHERE MUID IN (SELECT PARE FROM MA11MH WHERE MUID ='{0}') ", MUID);
            strPatx = GetDataFiled(sb.ToString());
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return strPatx;
    }

    /// <summary>
    /// 根据菜单编号得到菜单描述
    /// </summary>
    /// <param name="MUID">菜单编号</param>
    /// <returns>菜单描述</returns>
    public string GetMutxInfo(string MUID)
    {
        string strPatx = "";

        strPatx = DsCg.GetSysTitle();

        try
        {
            strPatx = strPatx + " + " + QueryFiled("MA11MH", "MUID", MUID, "MUTX");
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return strPatx;
    }

    #endregion //得到菜单信息

    #region 验证用户权限
    /// <summary>
    /// Check User authorization by Menu ID
    /// </summary>
    /// <param name="MenuId"></param>
    /// <param name="UserId"></param>
    /// <returns></returns>
    public bool ChkUserAur(string MUID, string URSN)
    {

        try
        {
            bool bol = false;

            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("SELECT * FROM vmMenu WHERE (MUID = '{0}') AND (URSN = '{1}') AND (VISI = 'Y') AND AUTP <>'' ", MUID, URSN);

            DataTable dt = objSql.GetDataTable(sbSql.ToString());

            if (dt.Rows.Count != 0)
            {
                bol = true;
            }
            return bol;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion //验证用户权限



    #endregion //系统菜单操作

    #region 数据加密操作

    #region Get 32 character MD5 String
    /// <summary>
    /// Get a 32 character MD5 String
    /// </summary>
    /// <param name="Input">Need encipher string </param>
    /// <returns>a 32 character MD5 String</returns>
    public string getMd5Of32(string SourceString)
    {
        // Create a new instance of the MD5CryptoServiceProvider object.
        System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();
        // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
        //byte[] bytData = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(cl));
        // Convert the input string to a byte array and compute the hash.
        byte[] bytData = md5Hasher.ComputeHash(Encoding.Default.GetBytes(SourceString));
        // Create a new Stringbuilder to collect the bytes and create a string.
        StringBuilder sbBuilder = new StringBuilder();
        // Loop through each byte of the hashed data and format each one as a hexadecimal string.
        for (int i = 0; i < bytData.Length; i++)
        {
            sbBuilder.Append(bytData[i].ToString("x2"));
        }
        // Return the hexadecimal string.
        return sbBuilder.ToString();
    }
    #endregion //Get 32 character MD5 String

    #region Get 16 character MD5 String
    /// <summary>
    /// Get a 16 character MD5 String
    /// </summary>
    /// <param name="Input">Need encipher string </param>
    /// <returns>a 16 character MD5 String</returns>
    public string getMd5Of16(string SourceString)
    {
        // Create a new instance of the MD5CryptoServiceProvider object.
        System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();
        byte[] bytData = md5Hasher.ComputeHash(UTF8Encoding.Default.GetBytes(SourceString));
        string strOutput = BitConverter.ToString(bytData, 4, 8);
        strOutput = strOutput.Replace("-", "");
        return strOutput;
    }
    #endregion //Get 16 character MD5 String

    #endregion //数据加密操作：Encipherment Area

    #region 配置信息管理

    #region 得到配置信息字段值

    /// <summary>
    /// Get Ctrl Filed Values
    /// </summary>
    /// <param name="SYID">System ID</param>
    /// <param name="CLID">CLID</param>
    /// <param name="CLNA">CLNA</param>
    /// <param name="ReqValu">Request Filed ID</param>
    /// <returns>Request Filed values</returns>
    public string GetCtrlFild(string SYID, string CLID, string CLNA, string ReqValu)
    {
        try
        {
            StringBuilder sbSql = new StringBuilder();

            sbSql.AppendFormat("SELECT {0} FROM CA11FG WHERE (CLID = '{1}') AND (CLNA = '{2}') AND (SYID = '{3}')",
                ReqValu, CLID, CLNA, SYID);

            string strFiledValues = objSql.GetDataFiled(sbSql.ToString());

            return strFiledValues;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public string GetCtrlFild(string CLID, string CLNA, string ReqValu)
    {
        try
        {
            StringBuilder sbSql = new StringBuilder();

            sbSql.AppendFormat("SELECT {0} FROM CA11FG WHERE (CLID = '{1}') AND (CLNA = '{2}')",
                ReqValu, CLID, CLNA);

            string strFiledValues = objSql.GetDataFiled(sbSql.ToString());

            return strFiledValues;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    /// <summary>
    /// Get Ctrl Filed Values
    /// </summary>
    /// <param name="SYID">System ID</param>
    /// <param name="CLID">CLID</param>
    /// <param name="ConditionFiled">Condition Filed Name</param>
    /// <param name="ConditionValues">Condition Filed Values</param>
    /// <param name="ReqValu">Request Filed ID</param>
    /// <returns>Request Filed values</returns>
    public string GetCtrlFildCndi(string SYID, string CLID, string ConditionFiled, string ConditionValues, string ReqValu)
    {
        try
        {
            StringBuilder sbSql = new StringBuilder();

            sbSql.AppendFormat("SELECT {0} FROM CA11FG WHERE (CLID = '{1}') AND ({2} = '{3}') AND (SYID = '{4}')",
                ReqValu, CLID, ConditionFiled, ConditionValues, SYID);

            string strFiledValues = objSql.GetDataFiled(sbSql.ToString());

            return strFiledValues;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public string GetCtrlFildCndi(string CLID, string ConditionFiled, string ConditionValues, string ReqValu)
    {
        try
        {
            StringBuilder sbSql = new StringBuilder();

            sbSql.AppendFormat("SELECT {0} FROM CA11FG WHERE (CLID = '{1}') AND ({2} = '{3}'))",
                ReqValu, CLID, ConditionFiled, ConditionValues);

            string strFiledValues = objSql.GetDataFiled(sbSql.ToString());

            return strFiledValues;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion //得到配置信息字段值：

    #region 得到配置信息清单
    /// <summary>
    /// Get Ctrl List Info
    /// </summary>
    /// <param name="SYID">SYID</param>
    /// <param name="CLID">CLID</param>
    /// <param name="FieldValue">Filed Values for list </param>
    /// <param name="FieldText">Filed Text for List</param>
    /// <returns>Ctrl List</returns>
    public DataTable GetCtrlList(string SYID, string CLID, string FieldValue, string FieldText)
    {
        try
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.AppendFormat("Select {0} as Field_Value, {1} as Field_Text ", FieldValue, FieldText)
               .AppendFormat("From CA11FG Where SYID='{0}' and CLID='{1}' AND ATFG <> 'D' ORDER BY SQEN", SYID, CLID);

            DataTable dt = objSql.GetDataTable(sbSql.ToString());
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion //得到配置信息清单：

    #region 得到特定代码的列表信息
    /// <summary>
    /// 得到特定代码的列表信息
    /// </summary>
    /// <param name="DBID">数据库编号</param>
    /// <param name="ComponentType">组件类别</param>
    /// <param name="ComponectID">特殊控件编号</param>
    /// <returns></returns>
    public DataTable GetCtrlSpecial(string SYID, string ComponectID)
    {
        try
        {
            //定义变量
            StringBuilder sbSql = new StringBuilder();
            //根据代码名称设定SQL语句
            sbSql.AppendFormat("SELECT CPVL FROM CA12SC WHERE (SYID = '{0}') AND (CPID = '{1}')", SYID, ComponectID);
            string strSQL = objSql.GetDataFiled(sbSql.ToString());
            //查询满足条件的数据
            DataTable dt = objSql.GetDataTable(strSQL);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    
    #endregion //得到特定代码的列表信息

    #region 得到autocompelet下拉列表信息
    /// <summary>
    /// 得到特定代码的列表信息
    /// </summary>
    /// <param name="DBID">数据库编号</param>
    /// <param name="ComponentType">组件类别</param>
    /// <param name="ComponectID">特殊控件编号</param>
    /// <returns></returns>
    /// 
   
    public DataTable GetCtrlSpecialAuto(string SXNA, string Table)
    {
        try
        {
            //定义变量
            StringBuilder sbSql = new StringBuilder();
            //根据代码名称设定SQL语句
            String stSql= sbSql.AppendFormat("SELECT {0} FROM {1} WHERE ATFG = 'Y'", SXNA, Table).ToString();
            //查询满足条件的数据
            DataTable dt = objSql.GetDataTable(stSql);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetCtrlAuto(string SXNA, string Table)
    {
        try
        {
            //定义变量
            StringBuilder sbSql = new StringBuilder();
            //根据代码名称设定SQL语句
            String stSql = sbSql.AppendFormat("SELECT {0} FROM {1} WHERE 1 = 1", SXNA, Table).ToString();
            //查询满足条件的数据
            DataTable dt = objSql.GetDataTable(stSql);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public DataTable GetLossRecc(string Vin, string Table)
    {
        try
        {
            //定义变量
            StringBuilder sbSql = new StringBuilder();
            //根据代码名称设定SQL语句
            String stSql = sbSql.AppendFormat("SELECT distinct {0} FROM {1} WHERE brand <> '保时捷'", Vin, Table).ToString();
            //查询满足条件的数据
            DataTable dt = objSql.GetDataTable(stSql);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }


    public DataTable GetCtrlSpecial(string ComponectID)
    {
        try
        {
            //定义变量
            StringBuilder sbSql = new StringBuilder();
            //根据代码名称设定SQL语句
            sbSql.AppendFormat("SELECT CPVL FROM CA12SC WHERE CPID = '{0}'", ComponectID);
            string strSQL = objSql.GetDataFiled(sbSql.ToString());
            //查询满足条件的数据
            DataTable dt = objSql.GetDataTable(strSQL);
            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion //得到特定代码的列表信息


    #region 得到特定代码的列表信息
    /// <summary>
    /// 得到特定代码的列表信息
    /// </summary>
    /// <param name="DBID">数据库编号</param>
    /// <param name="ComponentType">组件类别</param>
    /// <param name="ComponectID">特殊控件编号</param>
    /// <returns></returns>
    public DataTable GetCtrlSpecialByPram(string SYID, string ComponectID, string Pram)
    {
        try
        {
            //定义变量
            StringBuilder sbSql = new StringBuilder();
            //根据代码名称设定SQL语句
            sbSql.AppendFormat("SELECT CPVL FROM CA12SC WHERE (SYID = '{0}') AND  (CPID = '{1}')", SYID, ComponectID);

            string strSQL = objSql.GetDataFiled(sbSql.ToString());

            strSQL = strSQL.Replace(" 1=1 ", Pram);

            //查询满足条件的数据
            DataTable dt = objSql.GetDataTable(strSQL);

            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public DataTable GetCtrlSpecialByPram(string ComponectID, string Pram)
    {
        try
        {
            //定义变量
            StringBuilder sbSql = new StringBuilder();
            //根据代码名称设定SQL语句
            sbSql.AppendFormat("SELECT CPVL FROM CA12SC WHERE CPID = '{0}'", ComponectID);

            string strSQL = objSql.GetDataFiled(sbSql.ToString());

            strSQL = strSQL.Replace(" 1=1 ", Pram);

            //查询满足条件的数据
            DataTable dt = objSql.GetDataTable(strSQL);

            return dt;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion //得到特定代码的列表信息

    #region 得到GridView默认的分页行数
    /// <summary>
    /// 得到GridView默认的分页行数
    /// </summary>
    /// <returns></returns>
    public string GetGridViewDefaltLine()
    {
        try
        {
            string strFiledValues = GetCtrlFild("GVLN", "GVLN", "CLTX");

            return strFiledValues;
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion //得到GridView默认的分页行数

    #endregion //配置信息管理:Get CA11FG Info

    #region 设置控件数据

    #region 设置通用下拉列表框

    #region 设置通用下拉列表框(Value + Text)
    /// <summary>
    /// 设置通用下拉列表框(Value + Text)
    /// </summary>
    /// <param name="DropDownListName">下拉类表框名称</param>
    /// <param name="SYID">系统名称</param>
    /// <param name="CLID">配置代码</param>
    /// <param name="FieldValue">配置值</param>
    /// <param name="FieldText">显示文本</param>
    public void SetDdl(DropDownList DropDownListName, string SYID, string CLID, string FieldValue, string FieldText)
    {
        try
        {
            DataTable dt = GetCtrlList(SYID, CLID, FieldValue, FieldText);
            DropDownListName.DataSource = dt;
           // DropDownListName.DataSource = GetCtrlList(SYID, CLID, FieldValue, FieldText);
            DropDownListName.DataTextField = "Field_Text";
            DropDownListName.DataValueField = "Field_Value";
            DropDownListName.DataBind();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion //设置通用下拉列表框(Value + Text)

    #region 设置通用下拉列表框(Value + Text)--含全选项
    /// <summary>
    /// 设置通用下拉列表框(Value + Text)--含全选项
    /// </summary>
    /// <param name="DropDownListName">下拉类表框名称</param>
    /// <param name="SYID">系统名称</param>
    /// <param name="CLID">配置代码</param>
    /// <param name="FieldValue">配置值</param>
    /// <param name="FieldText">显示文本</param>
    public void SetDdlAll(DropDownList DropDownListName, string SYID, string CLID, string FieldValue, string FieldText)
    {
        try
        {
            DataTable dt = GetCtrlList(SYID, CLID, FieldValue, FieldText);
            DataRow dr = dt.NewRow();
            dr["Field_Text"] = "";
            dr["Field_Value"] = "";
            dt.Rows.Add(dr);
            DropDownListName.DataSource = dt;
            DropDownListName.DataTextField = "Field_Text";
            DropDownListName.DataValueField = "Field_Value";
            DropDownListName.SelectedValue = "";
            DropDownListName.DataBind();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion //设置通用下拉列表框(Value + Text)--含全选项

    #endregion //设置通用下拉列表框

    #region 设定特定代码下拉类表框

    #region 设定特定代码下拉列表框
    /// <summary>
    /// 设定特定代码下拉列表框
    /// </summary>
    /// <param name="DropDownListName">列表框名称</param>
    /// <param name="DBID">数据库编号</param>
    /// <param name="ComponectID">特殊控件编号：参考特殊控件配置数据表</param>
    public void SetDdlSpecial(DropDownList DropDownListName, string SYID, string ComponectID)
    {
        try
        {
            DropDownListName.DataSource = GetCtrlSpecial(SYID, ComponectID);
            DropDownListName.DataTextField = "Field_Text";
            DropDownListName.DataValueField = "Field_Value";
            DropDownListName.DataBind();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion //设定特定代码下拉列表框

    #region 设定特定代码下拉列表框--包含全选
    /// <summary>
    /// 设定特定代码下拉列表框--包含全选
    /// </summary>
    /// <param name="DropDownListName">列表框名称</param>
    /// <param name="DBID">数据库编号</param>
    /// <param name="ComponectID">特殊控件编号：参考特殊控件配置数据表</param>
    public void SetDdlSpecialAll(DropDownList DropDownListName, string DBID, string ComponectID)
    {
        try
        {
            DataTable dt = GetCtrlSpecial(DBID, ComponectID);
            DataRow dr = dt.NewRow();
            dr["Field_Text"] = "";
            dr["Field_Value"] = "";
            dt.Rows.Add(dr);
            DropDownListName.DataSource = dt;
            DropDownListName.DataTextField = "Field_Text";
            DropDownListName.DataValueField = "Field_Value";
            DropDownListName.SelectedValue = "";
            DropDownListName.DataBind();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    #endregion //设定特定代码下拉列表框--包含全选

    #region 设定特定代码下拉列表框
    /// <summary>
    /// 设定特定代码下拉列表框
    /// </summary>
    /// <param name="DropDownListName">列表框名称</param>
    /// <param name="DBID">数据库编号</param>
    /// <param name="ComponectID">特殊控件编号：参考特殊控件配置数据表</param>
    public void SetDdlSpecialByPram(DropDownList DropDownListName, string SYID, string ComponectID, string Pram)
    {
        try
        {
            DropDownListName.DataSource = GetCtrlSpecialByPram(SYID, ComponectID, Pram);
            DropDownListName.DataTextField = "Field_Text";
            DropDownListName.DataValueField = "Field_Value";
            DropDownListName.DataBind();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion //设定特定代码下拉列表框

    #region 设定特定代码下拉列表框--包含全选
    /// <summary>
    /// 设定特定代码下拉列表框--包含全选
    /// </summary>
    /// <param name="DropDownListName">列表框名称</param>
    /// <param name="DBID">数据库编号</param>
    /// <param name="ComponectID">特殊控件编号：参考特殊控件配置数据表</param>
    public void SetDdlSpecialByPramAll(DropDownList DropDownListName, string SYID, string ComponectID, string Pram)
    {
        try
        {
            DataTable dt = GetCtrlSpecialByPram(SYID, ComponectID, Pram);
            DataRow dr = dt.NewRow();
            dr["Field_Text"] = "";
            dr["Field_Value"] = "";
            dt.Rows.Add(dr);
            DropDownListName.DataSource = dt;
            DropDownListName.DataTextField = "Field_Text";
            DropDownListName.DataValueField = "Field_Value";
            DropDownListName.SelectedValue = "";
            DropDownListName.DataBind();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion //设定特定代码下拉列表框--包含全选

    #region 设定月份下拉列表框--当前月和上月
    /// <summary>
    /// 设定月份下拉列表框--当前月和上月
    /// </summary>
    /// <param name="DropDownListName"></param>
    public void SetDropDownList_TowMonth(DropDownList DropDownListName)
    {
        try
        {
            DataTable dt = new DataTable();
            DataColumn col = new DataColumn();

            col = dt.Columns.Add("Field_Text", typeof(string));
            col.MaxLength = 20;
            col = dt.Columns.Add("Field_Value", typeof(string));
            col.MaxLength = 20;

            //DataRow dr = dt.NewRow();
            //dr["Field_Text"] = "";
            //dr["Field_Value"] = "";
            //dt.Rows.Add(dr);

            for (int i = 0; i < 2; i++)
            {
                string strMonth = DateTime.Now.AddMonths(-i).ToString("yyyyMM");
                DataRow drM = dt.NewRow();
                drM["Field_Text"] = strMonth;
                drM["Field_Value"] = strMonth;
                dt.Rows.Add(drM);
            }

            DropDownListName.DataSource = dt;
            DropDownListName.DataTextField = "Field_Text";
            DropDownListName.DataValueField = "Field_Value";
            DropDownListName.DataBind();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
    #endregion //设定月份下拉列表框

    #endregion //设定特定代码下拉列表框

    public string GetChkFlag(CheckBox chk)
    {
        string strFlag = "N";

        if (chk.Checked == true)
        {
            strFlag = "Y";
        }

        return strFlag;
    }

    public void SetChkFlag(CheckBox chk,string Flag)
    {

        if (Flag == "Y")
        {
            chk.Checked = true;
        }
        else
        {
            chk.Checked = false;
        }

        
    }

    #endregion //设置控件数据

    #region 系统信息

    public string GetSysTitle()
    {
        string strSysTitle = "";

        strSysTitle = DsCg.GetSysTitle();

        return strSysTitle;
    }

    #endregion //系统信息

    #region 图片处理

    /// <summary>
    /// 把图片存储为数据库二进制数据
    /// </summary>
    /// <param name="fileName">文件路径</param>
    private void SaveImage(string fileName)
    {
        // Read the file into a byte array            
        using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
        {
            byte[] imageData = new Byte[fs.Length];
            fs.Read(imageData, 0, (int)fs.Length);
            string strCon = "Data Source=(local);Initial Catalog=TEST;Integrated Security=SSPI";

            using (SqlConnection conn = new SqlConnection(strCon))
            {
                string sql = "INSERT INTO IMGI (FLNA,IMDT) VALUES (@filename,@blobdata)";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.Add("@filename", SqlDbType.Text);
                cmd.Parameters["@filename"].Direction = ParameterDirection.Input;
                cmd.Parameters.Add("@blobdata", SqlDbType.Image);
                cmd.Parameters["@blobdata"].Direction = ParameterDirection.Input;
                // Store the byte array within the image field                   
                cmd.Parameters["@filename"].Value = fileName;
                cmd.Parameters["@blobdata"].Value = imageData;
                conn.Open();
                if (cmd.ExecuteNonQuery() == 1)
                {
                    //
                }
            }
        }
    }

    /// <summary>
    /// 把数据库中的二进制数据读出写成文件
    /// </summary>
    /// <param name="fileName">文件路径</param>
    private void LoadImage(string fileName)
    {
        string strCon = "Data Source=(local);Initial Catalog=TEST;Integrated Security=SSPI";
        using (SqlConnection conn = new SqlConnection(strCon))
        {
            string sql = "SELECT IMDT FROM IMGI WHERE FLNA LIKE @filename";
            SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@filename", SqlDbType.Text);
            cmd.Parameters["@filename"].Value = fileName;
            conn.Open();
            object objImage = cmd.ExecuteScalar();
            byte[] buffer = (byte[])objImage;
            BinaryWriter bw = new BinaryWriter(new FileStream("D:\\abcd.png", FileMode.Create));
            bw.Write(buffer);
            bw.Close();
            MemoryStream ms = new MemoryStream(buffer);
            System.Drawing.Image bgImage = System.Drawing.Image.FromStream(ms);
            ms.Close();
            //image1 = bgImage;
            //img = (System.Web.UI.WebControls.Image)bgImage;       
            //img.ImageUrl = @"D:\abcd.png";
        }
    } 

    #endregion // 图片处理

    #region 业务模块

    #region 点评管理

    #region FnDp01 - 保存点评信息
    /// <summary>
    /// 保存点评信息(FnDp0)
    /// </summary>
    /// <param name="GSID">业务编号</param>
    /// <param name="GSTT">业务描述</param>
    /// <param name="PLNR">点评内容</param>
    /// <returns></returns>
    public bool FnDp01Save(string GSID,string GSTT,string PLNR)
    {
        bool bolIfOk = false;

        #region 变量定义
        string strURID = GetUserID();

        string strTableName = "OS12DP";
        ArrayList aryInsertFiled = new ArrayList();
        ArrayList aryInsertValues = new ArrayList();
        string strSYID = "";
        string strARID = "";
        string strPLID = "";
        string strURSN = "";
        string strGSID = "";
        string strGSTT = "";
        string strPLNR = "";
        string strPLFG = "";
        string strPLTM = "";
        string strPLIT = "";
        string strPLSN = "";
        string strCRID = "";
        string strCGID = "";
        #endregion //变量定义

        #region 变量赋值
        strSYID = DsCg.GetSyid();
        strPLID = GSID;
        strURSN = strURID;
        strGSID = GSID;
        strGSTT = GSTT;
        strPLNR = PLNR;
        strPLFG = "Y";
        strPLTM = strURSN;
        strPLIT = GetItemNoById(strTableName, "PLID", strPLID, "PLIT", 3);
        strPLSN = strPLIT;
        strCRID = strURID;
        strCGID = strURID;
        #endregion //变量赋值

        #region 变量处理
        strSYID = FmatStr(strSYID);
        strARID =FmatStr(strARID);
        strPLID =FmatStr(strPLID);
        strURSN =FmatStr(strURSN);
        strGSID =FmatStr(strGSID);
        strGSTT =FmatStr(strGSTT);
        strPLNR =FmatStr(strPLNR);
        strPLFG =FmatStr(strPLFG);
        strPLTM =FmatStr(strPLTM);
        strPLIT =FmatStr(strPLIT);
        strPLSN =FmatStr(strPLSN);
        #endregion //变量处理

        #region 变量验证

        #region 非空验证
        //主键的非空验证
        //if (strTPNA == "")
        //{
        //    lblWarning.Text = "单品图片 不允许为空！";
        //    return;
        //}
        #endregion //非空验证

        #region 整型验证
        //if (!objDsFt.CheckInt(变量名))
        //{
        //	lblWarning.Text = "变量名 必须是整型数据";
        //	return;
        //}
        #endregion //整型验证

        #region 浮点验证
        //if (!objDsFt.CheckDeciaml(变量名))
        //{
        //	lblWarning.Text = "变量名 必须是数值型数据";
        //	return;
        //}
        #endregion //浮点验证

        #region 逻辑验证
        if (strPLFG.Length >= 30)
        {
            //ClientScript.RegisterStartupScript(typeof(string), "alert", "<script>alert('图片描述最多只能输入30个字符，您超过了限定长度！')</script>");
            //lblWarning.Text = "图片描述最多只能输入30个字符，您超过了限定长度！";
            //return;
        }

        #endregion //逻辑验证

        #region 主键验证
        //if (objDsFt.CheckCodeExist(strTableName, "SYID+TPID+TPTY+URSN", strSYID + strTPID + strTPTY + strURSN))
        //{
        //    lblWarning.Text = "主键重复,请检查!";
        //    return;
        //}
        #endregion //主键验证

        #endregion //变量验证

        #region 变量组合
        aryInsertFiled.Add("SYID");
        aryInsertFiled.Add("ARID");
        aryInsertFiled.Add("PLID");
        aryInsertFiled.Add("URSN");
        aryInsertFiled.Add("GSID");
        aryInsertFiled.Add("GSTT");
        aryInsertFiled.Add("PLNR");
        aryInsertFiled.Add("PLFG");
        aryInsertFiled.Add("PLTM");
        aryInsertFiled.Add("PLIT");
        aryInsertFiled.Add("PLSN");
        aryInsertFiled.Add("CRID");
        aryInsertFiled.Add("CGID");
        aryInsertFiled.Add("ATFG");


        aryInsertValues.Add(strSYID);
        aryInsertValues.Add(strARID);
        aryInsertValues.Add(strPLID);
        aryInsertValues.Add(strURSN);
        aryInsertValues.Add(strGSID);
        aryInsertValues.Add(strGSTT);
        aryInsertValues.Add(strPLNR);
        aryInsertValues.Add(strPLFG);
        aryInsertValues.Add(strPLTM);
        aryInsertValues.Add(strPLIT);
        aryInsertValues.Add(strPLSN);
        aryInsertValues.Add(strCRID);
        aryInsertValues.Add(strCGID);
        aryInsertValues.Add("Y");
        #endregion //变量组合

        #region 执行保存
        bolIfOk = InsertRecode(strTableName, aryInsertFiled, aryInsertValues);
        #endregion //执行保存       

        return bolIfOk;
    }


    #endregion //FnDp01 - 保存点评信息

    #endregion //点评管理


    #endregion //业务模块

}