using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace BgCustomsState
{
    public partial class ShunFeng : Form
    {
        public static string jsonStr = "";
        public ShunFeng()
        {
            
            InitializeComponent();
        }

        private void ShunFeng_Load(object sender, EventArgs e)
        {
            
        }
        
        private static void WayBillPrinterTools()
        {
            /*********2联单**************/
            /**
             * 调用打印机 不弹出窗口 适用于批量打印【二联单】
             */
            String url1 = "http://localhost:4040/sf/waybill/print?type=V2.0_poster_100mm150mm&output=noAlertPrint";
            /**
             * 调用打印机 弹出窗口 可选择份数 适用于单张打印【二联单】
             */
            String url2 = "http://localhost:4040/sf/waybill/print?type=V2.0_poster_100mm150mm&output=print";

            /**
             * 直接输出图片的BASE64编码字符串 可以使用html标签直接转换成图片【二联单】
             */
            String url3 = "http://localhost:4040/sf/waybill/print?type=V2.0_poster_100mm150mm&output=image";


            /*********3联单**************/
            /**
             * 调用打印机 不弹出窗口 适用于批量打印【三联单】
             */
            String url4 = "http://localhost:4040/sf/waybill/print?type=V3.0_poster_100mm210mm&output=noAlertPrint";
            /**
             * 调用打印机 弹出窗口 可选择份数 适用于单张打印【三联单】
             */
            String url5 = "http://localhost:4040/sf/waybill/print?type=V3.0_poster_100mm210mm&output=print";

            /**
             * 直接输出图片的BASE64编码字符串 可以使用html标签直接转换成图片【三联单】
             */
            String url6 = "http://localhost:4040/sf/waybill/print?type=V3.0_poster_100mm210mm&output=image";


            /*********2联150 丰密运单**************/
            /**
             * 调用打印机 不弹出窗口 适用于批量打印【二联单】
             */
            String url7 = "http://localhost:4040/sf/waybill/print?type=V2.0.FM_poster_100mm150mm&output=noAlertPrint";
            /**
             * 调用打印机 弹出窗口 可选择份数 适用于单张打印【二联单】
             */
            String url8 = "http://localhost:4040/sf/waybill/print?type=V2.0.FM_poster_100mm150mm&output=print";

            /**
             * 直接输出图片的BASE64编码字符串 可以使用html标签直接转换成图片【二联单】
             */
            String url9 = "http://localhost:4040/sf/waybill/print?type=V2.0.FM_poster_100mm150mm&output=image";


            /*********3联210 丰密运单**************/
            /**
             * 调用打印机 不弹出窗口 适用于批量打印【三联单】
             */
            String url10 = "http://localhost:4040/sf/waybill/print?type=V3.0.FM_poster_100mm210mm&output=noAlertPrint";
            /**
             * 调用打印机 弹出窗口 可选择份数 适用于单张打印【三联单】
             */
            String url11 = "http://localhost:4040/sf/waybill/print?type=V3.0.FM_poster_100mm210mm&output=print";

            /**
             * 直接输出图片的BASE64编码字符串 可以使用html标签直接转换成图片【三联单】
             */
            String url12 = "http://localhost:4040/sf/waybill/print?type=V3.0.FM_poster_100mm210mm&output=image";
            //String url12 = "http://10.118.65.124:9166/waybill-print-service/sf/waybill/print?type=V2.0.FM_poster_100mm150mm&output=image";

            //根据业务需求确定请求地址
            String reqURL = url7;

            //组装参数  true设置丰密参数 false 不设置
            //String jsonParam = AssemblyParameters(true);//true设置丰密参数 false 不设置
            string jsonParam = jsonStr;
            Console.WriteLine("param :" + jsonParam);
           // Console.ReadKey(true);



            //电子面单顶部是否需要logo 
            Boolean notTopLogo = false;//true 不需要  false 需要
            if (reqURL.Contains("V2.0") && notTopLogo)
            {
                reqURL = reqURL.Replace("V2.0", "V2.1");
            }

            if (reqURL.Contains("V3.0") && notTopLogo)
            {
                reqURL = reqURL.Replace("V3.0", "V3.1");
            }



            //发送请求
            String result = postJson(reqURL, jsonParam);

            Console.WriteLine("最终msg:" + result);
            //Console.ReadKey(true);

            if (result.Contains("\",\""))
            {

                // 如子母单及签回单需要打印两份或者以上
                //String[] arr = result.Split("\",\"");

                //// 输出图片到本地 支持.jpg、.png格式
                //for (int i = 0; i < arr.Length; i++)
                //{
                //    generateImage(arr[i].ToString(), "D:\\qiaoWaybill201811102-" + i + ".jpg");
                //}
            }
            else
            {
                generateImage(result, "D:\\qiaoWaybill201811102-1.jpg");
            }
            Console.WriteLine("结束");
            //Console.ReadKey(true);


        }

        //组装参数
        private static string AssemblyParameters(Boolean isFengMi)
        {
            IList<WaybillDto> waybillDtoList = new List<WaybillDto>();
            WaybillDto dto = new WaybillDto();



            //这个必填 
            //dto.appId = "SLKJ2019"; //对应丰桥平台获取的clientCode
            //dto.appKey = "FBIqMkZjzxbsZgo7jTpeq7PD8CVzLT4Q"; //对应丰桥平台获取的checkWord

            dto.appId = "DZGJWL"; //对应丰桥平台获取的clientCode
            dto.appKey = "nU1EfRimuiBTgKfMlAzSg5xXNYdtuoKB"; //对应丰桥平台获取的checkWord


            dto.mailNo = "755123457777";
            //  dto.setMailNo("755123457778,001000000002,001000000003");//子母单方式



            //收件人信息  
            dto.consignerProvince = "广东省";
            dto.consignerCity = "深圳市";
            dto.consignerCounty = "南山区";
            dto.consignerAddress = "学府路软件产业基地2B12楼5200708号"; //详细地址建议最多30个字  字段过长影响打印效果
            dto.consignerCompany = "神一样的科技";
            dto.consignerMobile = "15893799999";
            dto.consignerName = "风一样的旭哥";
            dto.consignerShipperCode = "518052";
            dto.consignerTel = "0755-33123456";


            //寄件人信息
            dto.deliverProvince = "浙江省";
            dto.deliverCity = "杭州市";
            dto.deliverCounty = "拱墅区";
            dto.deliverCompany = "神罗科技集团有限公司";
            dto.deliverAddress = "舟山东路708号古墩路北（玉泉花园旁）百花苑西区7-2-201室"; //详细地址建议最多30个字  字段过长影响打印效果
            dto.deliverName = "艾丽斯";
            dto.deliverMobile = "15881234567";
            dto.deliverShipperCode = "310000";
            dto.deliverTel = "0571-26508888";


            dto.destCode = "755"; //目的地代码 参考顺丰地区编号
            dto.zipCode = "571"; //原寄地代码 参考顺丰地区编号

            //签回单号  签单返回服务POD 会打印两份快单 其中第二份作为返寄的单==如有签回单业务需要传此字段值
            //dto.setReturnTrackingNo("755123457778");

            //陆运E标示
            //业务类型为"电商特惠、顺丰特惠、电商专配、陆运件"则必须打印E标识，用以提示中转场分拣为陆运件
            dto.electric = "E";



            //1 ：标准快递   2.顺丰特惠   3： 电商特惠   5：顺丰次晨  6：顺丰即日  7.电商速配   15：生鲜速配		
            dto.expressType = 1;

            ///addedService				
            //   COD代收货款价值 单位元   此项和月结卡号绑定的增值服务相关
            dto.codValue = "999.9";
            //dto.codMonthAccount = ""; //代收货款卡号 -如有代收货款专用卡号必传

            dto.insureValue = "501"; //声明保价价值  单位元

            dto.monthAccount = "7550385912"; //月结卡号
            dto.orderNo = "";
            dto.payMethod = 1; // 1寄方付 2收方付 3第三方月结支付

            dto.childRemark = "子单号备注";
            dto.mainRemark = "这是主运单的备注";
            dto.returnTrackingRemark = "迁回单备注";
            //dto.custLogo = "";
            //dto.logo = "";
            //dto.insureFee = "";
            //dto.payArea = "";
            //加密项
            dto.encryptCustName = true;//加密寄件人及收件人名称
            dto.encryptMobile = true;//加密寄件人及收件人联系手机	



            ArrayList rlsInfoDtoList = new ArrayList();


            RlsInfoDto rlsMain = new RlsInfoDto();
            rlsMain.abFlag = "A";
            rlsMain.codingMapping = "F33";
            rlsMain.codingMappingOut = "1A";
            rlsMain.destRouteLabel = "755WE-571A3";
            rlsMain.destTeamCode = "012345678";
            rlsMain.printIcon = "11110000";
            rlsMain.proCode = "T4";
            rlsMain.qrcode = "MMM={'k1':'755WE','k2':'021WT','k3':'','k4':'T4','k5':'755123456789','k6':''}";
            rlsMain.sourceTransferCode = "021WTF";
            rlsMain.waybillNo = "755123456789";
            rlsMain.xbFlag = "XB";
            rlsInfoDtoList.Add(rlsMain);

            if (null != dto.returnTrackingNo)
            {
                RlsInfoDto rlsBack = new RlsInfoDto();
                rlsBack.waybillNo = dto.returnTrackingNo;
                rlsBack.destRouteLabel = "021WTF";
                rlsBack.printIcon = "11110000";
                rlsBack.proCode = "T4";
                rlsBack.abFlag = "A";
                rlsBack.xbFlag = "XB";
                rlsBack.codingMapping = "1A";
                rlsBack.codingMappingOut = "F33";
                rlsBack.destTeamCode = "87654321";
                rlsBack.sourceTransferCode = "755WE-571A3";
                //对应下订单设置路由标签返回字段twoDimensionCode 该参
                rlsBack.qrcode = "MMM={'k1':'21WT','k2':'755WE','k3':'','k4':'T4','k5':'443123456789','k6':''}";
                rlsInfoDtoList.Add(rlsBack);
            }




            CargoInfoDto cargo = new CargoInfoDto();
            cargo.cargo = "苹果7S";
            cargo.cargoCount = 2;
            cargo.cargoUnit = "件";
            cargo.sku = "00015645";
            cargo.remark = "手机贵重物品 小心轻放";

            CargoInfoDto cargo2 = new CargoInfoDto();
            cargo2.cargo = "苹果macbook pro";
            cargo2.cargoCount = 10;
            cargo2.cargoUnit = "件";
            cargo2.sku = "00015646";
            cargo2.remark = "笔记本贵重物品 小心轻放";

            ArrayList cargoInfoList = new ArrayList();
            cargoInfoList.Add(cargo2);
            cargoInfoList.Add(cargo);

            dto.cargoInfoDtoList = cargoInfoList;
            dto.rlsInfoDtoList = rlsInfoDtoList;


            if (isFengMi)
            {
                dto.rlsInfoDtoList = rlsInfoDtoList;

            }

            waybillDtoList.Add(dto);

            return JsonConvert.SerializeObject(waybillDtoList);
        }
           
        //向服务传递参数
        private static string postJson(string reqURL, string jsonParm)
        {
           string httpResult = "";
           
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(reqURL);
                //req.ContentType = "application/json";
                //req.ContentType = "application/x-www-form-urlencoded";
                req.ContentType = "application/json;charset=utf-8";

                req.Method = "POST";
                req.Timeout = 20000;

                byte[] bs = System.Text.Encoding.UTF8.GetBytes(jsonParm);


                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                }

                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    //在这里对接收到的页面内容进行处理
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.Default))
                    {
                        httpResult = sr.ReadToEnd().ToString();
                    }
                }


                if (httpResult.Contains("["))
                {
                    httpResult = httpResult.Substring(httpResult.IndexOf("[") + 1, httpResult.IndexOf("]") - httpResult.IndexOf("[") - 1);
                }

                if (httpResult.StartsWith("\""))
                {

                    httpResult = httpResult.Substring(1, httpResult.Length - 1);
                }
                if (httpResult.EndsWith("\""))
                {
                    httpResult = httpResult.Substring(0, httpResult.Length - 1);
                }

                // 将换行全部替换成空
                httpResult = httpResult.Replace("\\n", "");

            }
            catch (Exception ex)
            {
                return ex.Message;
            }

            return httpResult;
        }

        //将图片文件写入本地
        public static Boolean generateImage(String imgStr, String imgFilePath)
        {
            if (imgStr == null)
                return false;
            try
            {

                byte[] bytes = Convert.FromBase64String(imgStr);


                int x = 256;
                byte a = (byte)x;

                for (int i = 0; i < bytes.Length; i++)
                {
                    if (bytes[i] < 0)
                    {
                        bytes[i] += a;
                    }
                }

                //FileStream write = File.OpenWrite(imgFilePath);

                using (FileStream fs = new FileStream(imgFilePath, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(bytes, 0, bytes.Length);
                    //fs.Flush();

                    fs.Close();
                }


            }
            catch (Exception e)
            {
                MessageBox.Show(imgStr+"-------"+e);
                return false;
            }
            return true;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            jsonStr = textBox1.Text;
            for (int i = 0; i < 1; i++)
            {
                WayBillPrinterTools();
            }
        }
    }

    class CargoInfoDto
    {

        public string cargo;
        public int parcelQuantity;
        public int cargoCount;
        public string cargoUnit;
        public double cargoWeight;
        public double cargoAmount;
        public double cargoTotalWeight;
        public string remark;
        public string sku;

    }
    class RlsInfoDto
    {
        public String abFlag;
        public String codingMapping;
        public String codingMappingOut;
        public String destRouteLabel;
        public String destTeamCode;
        public String printIcon;
        public String proCode;
        public String qrcode;
        public String sourceTransferCode;
        public String waybillNo;
        public String xbFlag;

    }
    class WaybillDto
    {

        public string mailNo;
        public int expressType;
        public int payMethod;
        public string returnTrackingNo;
        public string monthAccount;
        public string orderNo;
        public string zipCode;
        public string destCode;
        public string payArea;
        public string deliverCompany;
        public string deliverName;
        public string deliverMobile;
        public string deliverTel;
        public string deliverProvince;
        public string deliverCity;
        public string deliverCounty;
        public string deliverAddress;
        public string deliverShipperCode;
        public string consignerCompany;
        public string consignerName;
        public string consignerMobile;
        public string consignerTel;
        public string consignerProvince;
        public string consignerCity;
        public string consignerCounty;
        public string consignerAddress;
        public string consignerShipperCode;
        public string logo;
        public string sftelLogo;
        public string topLogo;
        public string topsftelLogo;
        public string appId;
        public string appKey;
        public string electric;
        public ArrayList cargoInfoDtoList;
        public ArrayList rlsInfoDtoList;
        public string insureValue;
        public string codValue;
        public string codMonthAccount;


        public string mainRemark;
        public string returnTrackingRemark;
        public string childRemark;
        public string custLogo;
        public string insureFee;

        public Boolean encryptCustName; //加密寄件人及收件人名称
        public Boolean encryptMobile; //加密寄件人及收件人联系手机
    }
}
