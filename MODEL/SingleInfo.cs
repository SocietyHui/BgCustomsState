using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODEL
{
    /***
    原产地上传信息
    **/
    public class SingleInfo
    {

        //批次号
        public String batch { get; set; }

        //上传状态
        public String single_state { get; set; }

        //按元产值分组汇总数量
        public String single_count { get; set; }

        //原产地证号
        public String certificate_origin_no { get; set; }

        //原产值
        public String origin_standard { get; set; }

        //序号
        public String order_no { get; set; }

        //签证日期
        public String sign_date { get; set; }

        //提单号
        public String bill_no { get; set; }

        //海关编码
        public String hs_code { get; set; }

        //海关序号
        public String single_seq_no { get; set; }

        //原产地证号 -- 抓取单一窗口
        public String CERT_NO { get; set; }

        //优惠贸易协定代码 -- 抓取单一窗口
        public String AGREEMENT_ID { get; set; }

        //原产国 -- 抓取单一窗口
        public String ORIGIN_COUNTRY { get; set; }

        //签证日期 -- 抓取单一窗口
        public String ISSUE_DATE { get; set; }

        //海关商品编码 -- 抓取单一窗口
        public String CODE_TS { get; set; }

        //数量 -- 抓取单一窗口
        public String QTY { get; set; }

        //计量单位 -- 抓取单一窗口
        public String UNIT { get; set; }

        //原产地值 -- 抓取单一窗口
        public String ORIGIN_CRITERION { get; set; }

        //是否经过非协定成员方中转 -- 抓取单一窗口
        public String IS_TRANSFER_RADIO { get; set; }

        //是否有单份全程运输单证 -- 抓取单一窗口
        public String IS_TRANSPORT_DOC_RADIO { get; set; }

        //提单号 -- 抓取单一窗口
        public String TRANSPORT_DOC_NO_TEXT { get; set; }

        //项号 -- 抓取单一窗口
        public String NO { get; set; }

    }
}
