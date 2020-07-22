using MODEL;
using MySql.Data.MySqlClient;
using SqlUtil;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAl
{
    public class SingleWindowDal
    {
        public List<SingleWindow> GetList(SingleWindow singleWindow)
        {
            StringBuilder sb = new StringBuilder();
            //构造要查询的sql语句
            string sql = @"select id,seq_no,batch,client_seq_no from bg_cust_dec_head_singlewindow where 1=1  ";
            sb.Append(sql);
            
            if (singleWindow.Batch != null && singleWindow.Batch != "")
            {
                sb.Append("and batch='" + singleWindow.Batch + "'");
            }
            if (singleWindow.SeqNo != null && singleWindow.SeqNo != "")
            {
                sb.Append("and seq_no='" + singleWindow.SeqNo + "'");
            }
            if (singleWindow.ClientSeqNo != null && singleWindow.ClientSeqNo != "")
            {
                sb.Append("and  client_seq_no < " + singleWindow.ClientSeqNo + "");
            }
            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sb.ToString());
            //将dt中的数据转存到list中
            List<SingleWindow> list = new List<SingleWindow>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SingleWindow()
                {
                    Id = row["id"].ToString(),
                    SeqNo = row["seq_no"].ToString(),
                    Batch = row["batch"].ToString(),
                    ClientSeqNo = row["client_seq_no"].ToString(),
                });
            }
            //将集合返回
            return list;
        }


        public List<SingleWindow> selectChecked(SingleWindow singleWindow)
        {
            StringBuilder sb = new StringBuilder();
            //构造要查询的sql语句
            string sql = @"select id,seq_no,batch,client_seq_no from bg_cust_dec_head_singlewindow where 1=1  ";
            sb.Append(sql);
           
            if (singleWindow.Batch != null && singleWindow.Batch != "")
            {
                sb.Append("and batch='" + singleWindow.Batch + "'");
            }
            if (singleWindow.SeqNo != null && singleWindow.SeqNo != "")
            {
                sb.Append("and seq_no in (" + singleWindow.SeqNo + ")");
            }
            if (singleWindow.ClientSeqNo != null && singleWindow.ClientSeqNo != "")
            {
                sb.Append("and  client_seq_no < " + singleWindow.ClientSeqNo + "");
            }
            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sb.ToString());
            //将dt中的数据转存到list中
            List<SingleWindow> list = new List<SingleWindow>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SingleWindow()
                {
                    Id = row["id"].ToString(),
                    SeqNo = row["seq_no"].ToString(),
                    Batch = row["batch"].ToString(),
                    ClientSeqNo = row["client_seq_no"].ToString(),
                });
            }
            //将集合返回
            return list;
        }



        //更新head状态
        public int UpdateHeadState(SingleWindow mi)
        {
            //为什么要进行密码的判断：
            //答：因为密码值是经过md5加密存储的，当修改时，需要判断用户是否改了密码，如果没有改，则不变，如果改了，则重新进行md5加密

            //构造update的sql语句
            string sql = @"update bg_cust_dec_head_singlewindow set client_seq_no='" + mi.ClientSeqNo + "' where batch='" + mi.Batch + "' and seq_no in ( " + mi.SeqNo + ") ";


            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql);
        }


        public List<SingleWindow> GetCiqList(SingleWindow singleWindow)
        {
            StringBuilder sb = new StringBuilder();
            //构造要查询的sql语句
            string sql = @"SELECT A.seq_no,B.ciq_decl_no  from bg_cust_dec_head A LEFT JOIN  (SELECT bill_no,ciq_decl_no  from bg_cust_dec_head WHERE batch = '" + singleWindow.Batch + "' AND ciq_decl_no is NOT NULL GROUP BY bill_no) B on A.bill_no=B.bill_no WHERE batch =  '" + singleWindow.Batch + "' and (A.ciq_decl_no is NULL or A.ciq_decl_no='') and A.seq_no is not NULL and B.ciq_decl_no is not NULL";
            sb.Append(sql);
           
            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sb.ToString());
            //将dt中的数据转存到list中
            List<SingleWindow> list = new List<SingleWindow>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SingleWindow()
                {
                    SeqNo = row["seq_no"].ToString(),
                    CiqDeclNo = row["ciq_decl_no"].ToString(),
                });
            }
            //将集合返回
            return list;
        }


        public List<SingleWindow> GetDecDocList(SingleWindow singleWindow)
        {
            StringBuilder sb = new StringBuilder();
            //构造要查询的sql语句
            string sql = @"SELECT seq_no from bg_cust_dec_head WHERE batch =  '" + singleWindow.Batch + "' and seq_no is not NULL and pro_df_no is NULL or pro_df_no=''";
            sb.Append(sql);
           
            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sb.ToString());
            //将dt中的数据转存到list中
            List<SingleWindow> list = new List<SingleWindow>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SingleWindow()
                {
                    SeqNo = row["seq_no"].ToString(),
                });
            }
            //将集合返回
            return list;
        }
        


        public List<SingleWindow> GetListByDfno(SingleWindow singleWindow)
        {
            //构造要查询的sql语句
            string sql = @"select id,batch,cust_dec_head_id from bg_cust_dec_detail
                            where batch='" + singleWindow.Batch + "' and cust_dec_head_id='" + singleWindow.Id + "'";
            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sql);
            //将dt中的数据转存到list中
            List<SingleWindow> list = new List<SingleWindow>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SingleWindow()
                {
                    //Detail_id = row["id"].ToString(),
                    //Batch = singleWindow.Batch.ToString(),
                    //Cust_dec_head_id = row["cust_dec_head_id"].ToString(),
                });
            }
            //将集合返回
            return list;
        }




        public List<SingleWindow> GetListByDetailId(SingleWindow singleWindow)
        {
            //构造要查询的sql语句
            string sql = @"select A.seq_no,B.barcode,B.color,B.info1 as spec,B.engine_type,REPLACE(B.displacement,'CC','') as displacement
		          ,B.motor_no,B.motor_power,DATE_FORMAT(production_date,'%Y%m') as production_date from bg_cust_dec_detail A 
                  left JOIN bg_cust_dec_list B on A.id = B.cust_dec_detail_id 
                       where A.batch='" + singleWindow.Batch + "' ";
            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sql);
            //将dt中的数据转存到list中
            List<SingleWindow> list = new List<SingleWindow>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SingleWindow()
                {
                    //Seq_no = row["seq_no"].ToString(),
                    //Barcode = row["barcode"].ToString(),
                    //Color = row["color"].ToString(),
                    //Spec = row["spec"].ToString(),
                    //Engine_type = row["engine_type"].ToString(),
                    //Displacement = row["displacement"].ToString(),
                    //Motor_no = row["motor_no"].ToString(),
                    //Motor_power = row["motor_power"].ToString(),
                    //Production_date = row["production_date"].ToString(),

                });
            }
            //将集合返回
            return list;
        }






        #region  //表体信息detail
        //查询detail是否存在
        public List<SingleDetail> GetDetailList(SingleDetail singleDetail)
        {
            //构造要查询的sql语句
            string sql = "select * from bg_cust_dec_detail_singlewindow where batch = '" + singleDetail.Batch + "' and cust_dec_head_id = '" + singleDetail.CustDecHeadId + "' and g_no = '" + singleDetail.GNo + "'";
            //使用helper进行查询，得到结果
           
            DataTable dt = mysqlHelper.GetDataTable(sql);
            //将dt中的数据转存到list中
            List<SingleDetail> list = new List<SingleDetail>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SingleDetail()
                {
                    Id = row["id"].ToString(),
                    CustDecHeadId = row["cust_dec_head_id"].ToString(),
                    Batch = row["batch"].ToString(),
                });
            }
            //将集合返回
            return list;
        }




        //更新detail
        public int UpdateDetail(SingleDetail mi)
        {
            //构造要查询的sql语句
            string sql = @"update bg_cust_dec_detail_singlewindow set  
                         g_no=@p0,custom_code=@p1, append_code=@p2, cust_dec_head_id=@p3, batch=@p4, ciq_code=@p5, ciq_name=@p6, goods_name=@p7, spec=@p8, 
                         valuation_qty=@p9 ,valuation_unit_code =@p10 , valuation_unit_name=@p11 ,unit_price =@p12 ,unit_total_price=@p13 ,currency_code =@p14 ,currency_name=@p15 ,
                            legal_qty_1=@p16 ,legal_unit_1_code =@p17 ,legal_unit_1_name=@p18 ,destination_country_std =@p19 ,destination_country_std_name=@p20 ,legal_qty_2 =@p21 ,legal_unit_2_code=@p22 ,legal_unit_2_name=@p23 ,
                           origin_country_std=@p24 ,origin_country_std_name =@p25 ,district_code=@p26 ,district_name =@p27 ,dest_code=@p28 ,dest_name=@p29 ,nc_detail_code =@p30 ,nc_detail_name=@p31 ,goods_spec =@p32 ,goods_attr=@p33 ,
                           goods_attr_name=@p34 ,purpose =@p35 ,purpose_name=@p36 ,trade_curr_std =@p37  ,gross_weight=@p38 ,net_weight =@p39 ,goods_brand=@p40 ,prod_batch_no =@p41 ,country_of_origin_code=@p42 ,country_of_origin_name =@p43 ,car_ccc_no =@p44 ,update_date=@p45 
                         where batch = '" + mi.Batch + "' and cust_dec_head_id = '" + mi.CustDecHeadId + "' and g_no = @p0 ";
           
            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            { 
                new MySqlParameter("@p0", mi.GNo),
                new MySqlParameter("@p1", mi.CustomCode),
                new MySqlParameter("@p2", mi.AppendCode),
                new MySqlParameter("@p3", mi.CustDecHeadId ),
                new MySqlParameter("@p4", mi.Batch),
                new MySqlParameter("@p5", mi.CiqCode ),
                new MySqlParameter("@p6", mi.CiqName),
                new MySqlParameter("@p7", mi.GoodsName ),
                new MySqlParameter("@p8", mi.Spec),


                new MySqlParameter("@p9", mi.ValuationQty ),
                new MySqlParameter("@p10", mi.ValuationUnitCode),
                new MySqlParameter("@p11", mi.ValuationUnitName ),
                new MySqlParameter("@p12", mi.UnitPrice),
                new MySqlParameter("@p13", mi.UnitTotalPrice ),
                new MySqlParameter("@p14", mi.CurrencyCode),
                new MySqlParameter("@p15", mi.CurrencyName ),


                new MySqlParameter("@p16", mi.LegalQty1),
                new MySqlParameter("@p17", mi.LegalUnit1Code ),
                new MySqlParameter("@p18", mi.LegalUnit1Name ),
                new MySqlParameter("@p19", mi.DestinationCountryStd),
                new MySqlParameter("@p20", mi.DestinationCountryStdName ),
                new MySqlParameter("@p21", mi.LegalQty2),
                new MySqlParameter("@p22", mi.LegalUnit2Code ),
                new MySqlParameter("@p23", mi.LegalUnit2Name),

                new MySqlParameter("@p24", mi.OriginCountryStd ),
                new MySqlParameter("@p25", mi.OriginCountryStdName),
                new MySqlParameter("@p26", mi.DistrictCode ),
                new MySqlParameter("@p27", mi.DistrictName ),
                 new MySqlParameter("@p28", mi.DestCode),
                new MySqlParameter("@p29", mi.DestName ),
                new MySqlParameter("@p30", mi.NcDetailCode),
                new MySqlParameter("@p31", mi.NcDetailName ),
                new MySqlParameter("@p32", mi.GoodsSpec),
                new MySqlParameter("@p33", mi.GoodsAttr ),

                new MySqlParameter("@p34", mi.GoodsAttrName),
                new MySqlParameter("@p35", mi.Purpose ),
                new MySqlParameter("@p36", mi.PurposeName ),
                new MySqlParameter("@p37", mi.TradeCurrStd),
                new MySqlParameter("@p38", mi.GrossWeight ),
                new MySqlParameter("@p39", mi.NetWeight ),
                new MySqlParameter("@p40", mi.GoodsBrand ),
                new MySqlParameter("@p41", mi.ProdBatchNo ),
                new MySqlParameter("@p42", mi.CountryOfOriginCode ),
                new MySqlParameter("@p43", mi.CountryOfOriginName),
                new MySqlParameter("@p44", mi.CarCccNo),

                new MySqlParameter("@p45", mi.UpdateDate )
            };

            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql, ps.ToArray());
           
           
        }
       


        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="mi">ManagerInfo类型的对象</param>
        /// <returns></returns>
        public string InsertSingleDetail(SingleDetail mi)
        {
            //构造insert语句
            string sql = @"insert into bg_cust_dec_detail_singlewindow(g_no,custom_code,append_code,cust_dec_head_id,batch,ciq_code,ciq_name,goods_name,spec,";
            sql +="valuation_qty,valuation_unit_code,valuation_unit_name,unit_price,unit_total_price,currency_code,currency_name,";
            sql += "legal_qty_1,legal_unit_1_code,legal_unit_1_name,destination_country_std,destination_country_std_name,legal_qty_2,legal_unit_2_code,legal_unit_2_name,";
            sql += "origin_country_std,origin_country_std_name,district_code,district_name,dest_code,dest_name,nc_detail_code,nc_detail_name,goods_spec,goods_attr,";
            sql += "goods_attr_name,purpose,purpose_name,trade_curr_std,gross_weight,net_weight,goods_brand,prod_batch_no,country_of_origin_code,country_of_origin_name,create_date,update_date)";
            sql += @" values(@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16,@p17,@p18,@p19,@p20,@p21,@p22,
                   @p23,@p24,@p25,@p26,@p27,@p28,@p29,@p30,@p31,@p32,@p33,@p34,@p35,@p36,@p37,@p38,@p39,@p40,@p41,@p42,@p43,@p44,@p45);SELECT @@Identity;";
            
            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            { 
                new MySqlParameter("@p0", mi.GNo),
                new MySqlParameter("@p1", mi.CustomCode),
                new MySqlParameter("@p2", mi.AppendCode),
                new MySqlParameter("@p3", mi.CustDecHeadId ),
                new MySqlParameter("@p4", mi.Batch),
                new MySqlParameter("@p5", mi.CiqCode ),
                new MySqlParameter("@p6", mi.CiqName),
                new MySqlParameter("@p7", mi.GoodsName ),
                new MySqlParameter("@p8", mi.Spec),


                new MySqlParameter("@p9", mi.ValuationQty ),
                new MySqlParameter("@p10", mi.ValuationUnitCode),
                new MySqlParameter("@p11", mi.ValuationUnitName ),
                new MySqlParameter("@p12", mi.UnitPrice),
                new MySqlParameter("@p13", mi.UnitTotalPrice ),
                new MySqlParameter("@p14", mi.CurrencyCode),
                new MySqlParameter("@p15", mi.CurrencyName ),


                new MySqlParameter("@p16", mi.LegalQty1),
                new MySqlParameter("@p17", mi.LegalUnit1Code ),
                new MySqlParameter("@p18", mi.LegalUnit1Name ),
                new MySqlParameter("@p19", mi.DestinationCountryStd),
                new MySqlParameter("@p20", mi.DestinationCountryStdName ),
                new MySqlParameter("@p21", mi.LegalQty2),
                new MySqlParameter("@p22", mi.LegalUnit2Code ),
                new MySqlParameter("@p23", mi.LegalUnit2Name),

                new MySqlParameter("@p24", mi.OriginCountryStd ),
                new MySqlParameter("@p25", mi.OriginCountryStdName),
                new MySqlParameter("@p26", mi.DistrictCode ),
                new MySqlParameter("@p27", mi.DistrictName ),
                new MySqlParameter("@p28", mi.DestCode),
                new MySqlParameter("@p29", mi.DestName ),
                new MySqlParameter("@p30", mi.NcDetailCode),
                new MySqlParameter("@p31", mi.NcDetailName ),
                new MySqlParameter("@p32", mi.GoodsSpec),
                new MySqlParameter("@p33", mi.GoodsAttr ),

                new MySqlParameter("@p34", mi.GoodsAttrName),
                new MySqlParameter("@p35", mi.Purpose ),
                new MySqlParameter("@p36", mi.PurposeName ),
                new MySqlParameter("@p37", mi.TradeCurrStd),
                new MySqlParameter("@p38", mi.GrossWeight ),
                new MySqlParameter("@p39", mi.NetWeight ),
                new MySqlParameter("@p40", mi.GoodsBrand ),
                new MySqlParameter("@p41", mi.ProdBatchNo ),
                new MySqlParameter("@p42", mi.CountryOfOriginCode ),
                new MySqlParameter("@p43", mi.CountryOfOriginName),

                new MySqlParameter("@p44", mi.CreateDate ),
                new MySqlParameter("@p45", mi.UpdateDate)
            };

            //执行插入操作
           
             return mysqlHelper.ExecuteScalar(sql, ps).ToString();
        }
        #endregion



        #region  // 许可证信息

        //查询Goodslimit是否存在
        public List<SingleGoodslimit> GetGoodslimitList(SingleGoodslimit mi)
        {
            //构造要查询的sql语句
            string sql = "select * from bg_cust_dec_goodslimit_singlewindow where batch = '" + mi.Batch + "' and cust_dec_head_id = '" + mi.CustDecHeadId + "' and cust_dec_detail_id = '" + mi.CustDecDetailId + "' and goods_no = '" + mi.GoodsNo + "' and lic_type_code = '" + mi.LicTypeCode + "'";
            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sql);
            //将dt中的数据转存到list中
            List<SingleGoodslimit> list = new List<SingleGoodslimit>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SingleGoodslimit()
                {
                    Id = row["id"].ToString()
                });
            }
            //将集合返回
            return list;
        }




        /// <summary>
        /// 插入许可证数据
        /// </summary>
        /// <param name="mi">ManagerInfo类型的对象</param>
        /// <returns></returns>
        public int InsertSingleGoodslimit(SingleGoodslimit mi)
        {
            //构造insert语句
            string sql = @"insert into bg_cust_dec_goodslimit_singlewindow(goods_no,cust_dec_detail_id,cust_dec_head_id,batch,lic_type_code,lic_type_name,licence_no,
                          create_date,update_date)";
            sql += " values(@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8)";
            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            { 
                new MySqlParameter("@p0", mi.GoodsNo),
                new MySqlParameter("@p1", mi.CustDecDetailId),
                new MySqlParameter("@p2", mi.CustDecHeadId ),
                new MySqlParameter("@p3", mi.Batch),
                new MySqlParameter("@p4", mi.LicTypeCode),
                new MySqlParameter("@p5", mi.LicTypeName ),
                new MySqlParameter("@p6", mi.LicenceNo),
                new MySqlParameter("@p7", mi.CreateDate ),
                new MySqlParameter("@p8", mi.UpdateDate)
            };
            //执行插入操作

            return mysqlHelper.ExcuteNonQuery(sql, ps);
        }




        //更新Goodslimit
        public int UpdateSingleGoodslimit(SingleGoodslimit mi)
        {
            //构造要查询的sql语句
            string sql = @"update bg_cust_dec_goodslimit_singlewindow set  
                         lic_type_code=@p0, lic_type_name=@p1, licence_no=@p2 ,update_date=@p3 
                         where batch = '" + mi.Batch + "' and cust_dec_head_id = '" + mi.CustDecHeadId + "' and cust_dec_detail_id = '" + mi.CustDecDetailId + "' and goods_no = '" + mi.GoodsNo + "' and lic_type_code = '" + mi.LicTypeCode + "'";

            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            { 
                new MySqlParameter("@p0", mi.LicTypeCode),
                new MySqlParameter("@p1", mi.LicTypeName),
                new MySqlParameter("@p2", mi.LicenceNo ),
                new MySqlParameter("@p3", mi.UpdateDate)
            };
            //执行插入操作

            return mysqlHelper.ExcuteNonQuery(sql, ps);


        }




        #endregion



        #region  //许可证VIN信息

        //查询GoodslimitListVin是否存在
        public List<SingleGoodslimitVin> GetGoodslimitListVin(SingleGoodslimitVin singleGoodslimitVin)
        {
            //构造要查询的sql语句
            string sql = "select * from bg_cust_dec_goodslimitvin_singlewindow where batch = '" + singleGoodslimitVin.Batch + "' and vin_code = '" + singleGoodslimitVin.VinCode + "' and cust_dec_head_id = '" + singleGoodslimitVin.CustDecHeadId + "' and cust_dec_detail_id = '" + singleGoodslimitVin.CustDecDetailId + "'";
            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sql);
            //将dt中的数据转存到list中
            List<SingleGoodslimitVin> list = new List<SingleGoodslimitVin>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SingleGoodslimitVin()
                {
                    Id = row["id"].ToString()
                });
            }
            //将集合返回
            return list;
        }




        /// <summary>
        /// 插入许可证VIN数据
        /// </summary>
        /// <param name="mi">ManagerInfo类型的对象</param>
        /// <returns></returns>
        public int InsertSingleGoodslimitVin(SingleGoodslimitVin mi)
        {
            //构造insert语句
            string sql = @"insert into bg_cust_dec_goodslimitvin_singlewindow(vin_no,cust_dec_detail_id,cust_dec_head_id,batch,bill_lad_date,quality_qgp,motor_no,vin_code,
                          chassis_no,invoice_no,invoice_num,prod_cnnm,prod_ennm,price_per_unit,licence_no,lic_type_name,lic_type_code,create_date,update_date,model_en)";
            sql += " values(@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16,@p17,@p18,@p19)";
            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            {
                new MySqlParameter("@p0", mi.VinNo),
                new MySqlParameter("@p1", mi.CustDecDetailId),
                new MySqlParameter("@p2", mi.CustDecHeadId ),
                new MySqlParameter("@p3", mi.Batch),
                new MySqlParameter("@p4", mi.BillLadDate),
                new MySqlParameter("@p5", mi.QualityQgp ),
                new MySqlParameter("@p6", mi.MotorNo),
                new MySqlParameter("@p7", mi.VinCode ),
                new MySqlParameter("@p8", mi.ChassisNo),
                new MySqlParameter("@p9", mi.InvoiceNo ),
                new MySqlParameter("@p10", mi.InvoiceNum),
                new MySqlParameter("@p11", mi.ProdCnnm ),
                new MySqlParameter("@p12", mi.ProdEnnm),
                new MySqlParameter("@p13", mi.PricePerUnit ),

                new MySqlParameter("@p14", mi.LicenceNo),
                new MySqlParameter("@p15", mi.LicTypeName ),
                new MySqlParameter("@p16", mi.LicTypeCode ),

                new MySqlParameter("@p17", mi.CreateDate),
                new MySqlParameter("@p18", mi.UpdateDate ),
                new MySqlParameter("@p19",mi.ModelEn)
            };
            //执行插入操作

            return mysqlHelper.ExcuteNonQuery(sql, ps);
        }






        //更新许可证VIN数据GoodslimitVin
        public int UpdateSingleGoodslimitVin(SingleGoodslimitVin mi)
        {
            //构造要查询的sql语句
            string sql = @"update bg_cust_dec_goodslimitvin_singlewindow set  
                          bill_lad_date=@p4, quality_qgp=@p5,
                           motor_no=@p6, vin_code=@p7, chassis_no=@p8,invoice_no=@p9 ,invoice_num =@p10  ,
                           prod_cnnm=@p11 ,prod_ennm =@p12 ,price_per_unit=@p13 ,licence_no =@p14 ,lic_type_name=@p15 ,lic_type_code=@p16 ,update_date =@p17,model_en=@p19  
                         where batch = '" + mi.Batch + "' and vin_code = '" + mi.VinCode + "' and cust_dec_head_id = '" + mi.CustDecHeadId + "' and cust_dec_detail_id = '" + mi.CustDecDetailId + "'";

            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            { 
                new MySqlParameter("@p4", mi.BillLadDate),
                new MySqlParameter("@p5", mi.QualityQgp ),
                new MySqlParameter("@p6", mi.MotorNo),
                new MySqlParameter("@p7", mi.VinCode ),
                new MySqlParameter("@p8", mi.ChassisNo),
                new MySqlParameter("@p9", mi.InvoiceNo ),
                new MySqlParameter("@p10", mi.InvoiceNum),
                new MySqlParameter("@p11", mi.ProdCnnm ),
                new MySqlParameter("@p12", mi.ProdEnnm),
                new MySqlParameter("@p13", mi.PricePerUnit ),
                new MySqlParameter("@p14", mi.LicenceNo),
                new MySqlParameter("@p15", mi.LicTypeName ),
                new MySqlParameter("@p16", mi.LicTypeCode ),

                new MySqlParameter("@p17", mi.UpdateDate ),
                new MySqlParameter("@p19",mi.ModelEn)
            };

            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql, ps.ToArray());


        }

        #endregion


        #region  //集装箱信息

        //查询SingleContainer是否存在
        public List<SingleContainer> GetSingleContainer(SingleContainer singleContainer)
        {
            //构造要查询的sql语句
            string sql = "select * from bg_cust_dec_container_singlewindow where batch = '" + singleContainer.Batch + "' and container_id = '" + singleContainer.ContainerId + "' and cust_dec_head_id = '" + singleContainer.CustDecHeadId + "' ";
            //使用helper进行查询，得到结果
            DataTable dt = mysqlHelper.GetDataTable(sql);
            //将dt中的数据转存到list中
            List<SingleContainer> list = new List<SingleContainer>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SingleContainer()
                {
                    Id = row["id"].ToString()
                });
            }
            //将集合返回
            return list;
        }




        /// <summary>
        /// 插入集装箱信息
        /// </summary>
        /// <param name="mi">ManagerInfo类型的对象</param>
        /// <returns></returns>
        public int InsertSingleContainer(SingleContainer mi)
        {
            //构造insert语句
            string sql = @"insert into bg_cust_dec_container_singlewindow(cust_dec_head_id,container_id,container_md,goods_no,lcl_flag,container_wt,batch,create_date,update_date)";
            sql += " values(@p0,@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8)";
            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            { 
                new MySqlParameter("@p0", mi.CustDecHeadId),
                new MySqlParameter("@p1", mi.ContainerId),
                new MySqlParameter("@p2", mi.ContainerMd ),
                new MySqlParameter("@p3", mi.GoodsNo),
                new MySqlParameter("@p4", mi.LclFlag),
                new MySqlParameter("@p5", mi.ContainerWt ),
                new MySqlParameter("@p6", mi.Batch),
                
                new MySqlParameter("@p7", mi.CreateDate),
                new MySqlParameter("@p8", mi.UpdateDate )
            };
            //执行插入操作

            return mysqlHelper.ExcuteNonQuery(sql, ps);
        }






        //更新许可证VIN数据GoodslimitVin
        public int UpdateSingleContainer(SingleContainer mi)
        {
            //构造要查询的sql语句
            string sql = @"update bg_cust_dec_container_singlewindow set container_md=@p4, goods_no=@p5,lcl_flag=@p6,container_wt=@p7,update_date=@p8
                         where batch = '" + mi.Batch + "' and container_id = '" + mi.ContainerId + "' and cust_dec_head_id = '" + mi.CustDecHeadId + "' ";

            //构造sql语句的参数
            MySqlParameter[] ps = //使用数组初始化器
            { 
                new MySqlParameter("@p4", mi.ContainerMd),
                new MySqlParameter("@p5", mi.GoodsNo ),
                new MySqlParameter("@p6", mi.LclFlag),
                new MySqlParameter("@p7", mi.ContainerWt ),
                new MySqlParameter("@p8", mi.UpdateDate)
            };

            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql, ps.ToArray());


        }

        #endregion




        #region //表头信息
        /// <summary>
        /// 更新单一窗口表头
        /// </summary>
        /// <param name="mi"></param>
        /// <returns></returns>
        public int Update(SingleWindow mi)
        {
            //为什么要进行密码的判断：
            //答：因为密码值是经过md5加密存储的，当修改时，需要判断用户是否改了密码，如果没有改，则不变，如果改了，则重新进行md5加密

            //定义参数集合，可以动态添加元素
            List<MySqlParameter> listPs = new List<MySqlParameter>();
            //构造update的sql语句
            string sql = @"update bg_cust_dec_head_singlewindow set  
                           port_of_ie_code=@p1, port_of_ie_name=@p2, customs_status=@p3, customs_status_detail=@p4, seq_no=@p5,
                           pre_entry_no=@p6, df_no=@p7, ie_port=@p8,ie_port_name=@p9 ,contr_no =@p10 ,
                           date_of_ie=@p11 ,date_of_declaration =@p12 ,trade_co_scc=@p13 ,proprietor_company_code =@p14 ,trade_ciq_code=@p15 ,
                           proprietor_company_name=@p16 ,overseas_consignor_code =@p17 ,overseas_consignor_ename=@p18 ,owner_code_scc =@p19 ,owner_code=@p20 ,
                           owner_ciq_code=@p21 ,owner_name =@p22 ,agent_code_scc=@p23 ,agent_code =@p24 ,decl_ciq_code=@p25 ,
                           agent_name=@p26 ,traf_mode_std =@p27 ,traf_mode_std_name=@p28 ,traf_name =@p29 ,voyage_no=@p30  ,
                           bill_no=@p31 ,trade_method_code =@p32 ,trade_method_name=@p33 ,nc_code =@p34 ,nc_name=@p35 ,
                           licence_no=@p36 ,trade_country_std =@p37 ,trade_country_std_name=@p38 ,distinate_port_std =@p39 ,distinate_port_std_name=@p40 ,
                           terms_of_delivery_code=@p41 ,terms_of_delivery_name =@p42 ,total_piece=@p43 ,wrap_type_std =@p44 ,wrap_type_std_name=@p45 ,
                           total_gross_weight=@p46 ,total_net_weight =@p47 ,trade_country_code=@p48 ,trade_country_name =@p49 ,doc_type_code1=@p50 ,
                           enty_port_code=@p51 ,enty_port_name =@p52 ,goods_place=@p53 ,desp_port_code =@p54 ,desp_port_name=@p55 ,
                           dec_type=@p56 ,dec_type_name =@p57 ,label_remark=@p58 ,mark_no =@p59 ,org_code=@p60 ,
                           org_name=@p61 ,declaration_material_code =@p62 ,ent_qualif_no=@p63 ,vsa_org_code =@p64 ,vsa_org_name=@p65 ,
                           insp_org_code=@p66 ,insp_org_name =@p67 ,desp_date=@p68 ,bl_line_no =@p69 ,purp_org_code=@p70 ,
                           purp_org_name=@p71 ,correlation_no =@p72 ,correlation_reason_flag=@p73 ,use_org_person_code =@p74 ,use_org_person_tel=@p75 ,
                           file_type=@p76 ,file_type_name =@p77 ,rpr_flag=@p78 ,relation_flag =@p79 ,price_flag=@p80 ,
                           royalty_flag=@p81,file_type=@p82 ,file_type_name=@p83, appl_ori=@p84, appl_copy_quan=@p85 ,
                           domestic_consignee_ename=@p86,overseas_consignor_cname=@p87 ,overseas_consignor_addr=@p88, cmpl_dschrg_dt=@p89, pack_type=@p90 ,pack_type_name=@p91, doc_no1=@p92, 
                           doc_type_code2=@p93,doc_no2=@p94, dec_g_no=@p95,eco_g_no=@p96,
                            client_seq_no=1, batch=@batch, update_date =@update   ";
            #region//参数
            listPs.Add(new MySqlParameter("@p1", mi.PortOfIeCode));
            listPs.Add(new MySqlParameter("@p2", mi.PortOfIeName));
            listPs.Add(new MySqlParameter("@p3", mi.CustomsStatus));
            listPs.Add(new MySqlParameter("@p4", mi.CustomsStatusDetail));
            listPs.Add(new MySqlParameter("@p5", mi.SeqNo));

            listPs.Add(new MySqlParameter("@p6", mi.PreEntryNo));
            listPs.Add(new MySqlParameter("@p7", mi.DfNo));
            listPs.Add(new MySqlParameter("@p8", mi.IePort));
            listPs.Add(new MySqlParameter("@p9", mi.IePortName));
            listPs.Add(new MySqlParameter("@p10", mi.ContrNo));
            
            listPs.Add(new MySqlParameter("@p11", mi.DateOfIe));
            listPs.Add(new MySqlParameter("@p12", mi.DateOfDeclaration));
            listPs.Add(new MySqlParameter("@p13", mi.TradeCoScc));
            listPs.Add(new MySqlParameter("@p14", mi.ProprietorCompanyCode));
            listPs.Add(new MySqlParameter("@p15", mi.TradeCiqCode));


            listPs.Add(new MySqlParameter("@p16", mi.ProprietorCompanyName));
            listPs.Add(new MySqlParameter("@p17", mi.OverseasConsignorCode));
            listPs.Add(new MySqlParameter("@p18", mi.OverseasConsignorEname));
            listPs.Add(new MySqlParameter("@p19", mi.OwnerCodeScc));
            listPs.Add(new MySqlParameter("@p20", mi.OwnerCode));

            listPs.Add(new MySqlParameter("@p21", mi.OwnerCiqCode));
            listPs.Add(new MySqlParameter("@p22", mi.OwnerName));
            listPs.Add(new MySqlParameter("@p23", mi.AgentCodeScc));
            listPs.Add(new MySqlParameter("@p24", mi.AgentCode));
            listPs.Add(new MySqlParameter("@p25", mi.DeclCiqCode));

            listPs.Add(new MySqlParameter("@p26", mi.AgentName));
            listPs.Add(new MySqlParameter("@p27", mi.TrafModeStd));
            listPs.Add(new MySqlParameter("@p28", mi.TrafModeStdName));
            listPs.Add(new MySqlParameter("@p29", mi.TrafName));
            listPs.Add(new MySqlParameter("@p30", mi.VoyageNo));

            listPs.Add(new MySqlParameter("@p31", mi.BillNo));
            listPs.Add(new MySqlParameter("@p32", mi.TradeMethodCode));
            listPs.Add(new MySqlParameter("@p33", mi.TradeMethodName));
            listPs.Add(new MySqlParameter("@p34", mi.NcCode));
            listPs.Add(new MySqlParameter("@p35", mi.NcName));
            
            listPs.Add(new MySqlParameter("@p36", mi.LicenceNo));
            listPs.Add(new MySqlParameter("@p37", mi.TradeCountryStd));
            listPs.Add(new MySqlParameter("@p38", mi.TradeCountryStdName));
            listPs.Add(new MySqlParameter("@p39", mi.DistinatePortCode));
            listPs.Add(new MySqlParameter("@p40", mi.DistinatePortName));
            
            listPs.Add(new MySqlParameter("@p41", mi.TermsOfDeliveryCode));
            listPs.Add(new MySqlParameter("@p42", mi.TermsOfDeliveryName));
            listPs.Add(new MySqlParameter("@p43", mi.TotalPiece));
            listPs.Add(new MySqlParameter("@p44", mi.WrapTypeStd));
            listPs.Add(new MySqlParameter("@p45", mi.WrapTypeStdName));

            listPs.Add(new MySqlParameter("@p46", mi.TotalGrossWeight));
            listPs.Add(new MySqlParameter("@p47", mi.TotalNetWeight));
            listPs.Add(new MySqlParameter("@p48", mi.TradeCountryCode));
            listPs.Add(new MySqlParameter("@p49", mi.TradeCountryName));
            listPs.Add(new MySqlParameter("@p50", mi.DocTypeCode1));

            listPs.Add(new MySqlParameter("@p51", mi.EntyPortCode));
            listPs.Add(new MySqlParameter("@p52", mi.EntyPortName));
            listPs.Add(new MySqlParameter("@p53", mi.GoodsPlace));
            listPs.Add(new MySqlParameter("@p54", mi.DespPortCode));
            listPs.Add(new MySqlParameter("@p55", mi.DespPortName));

            listPs.Add(new MySqlParameter("@p56", mi.DecType));
            listPs.Add(new MySqlParameter("@p57", mi.DecTypeName));
            listPs.Add(new MySqlParameter("@p58", mi.LabelRemark));
            listPs.Add(new MySqlParameter("@p59", mi.MarkNo));
            listPs.Add(new MySqlParameter("@p60", mi.OrgCode));
           
            listPs.Add(new MySqlParameter("@p61", mi.OrgName));
            listPs.Add(new MySqlParameter("@p62", mi.DeclarationMaterialCode));
            listPs.Add(new MySqlParameter("@p63", mi.EntQualifNo));
            listPs.Add(new MySqlParameter("@p64", mi.VsaOrgCode));
            listPs.Add(new MySqlParameter("@p65", mi.VsaOrgName));

            listPs.Add(new MySqlParameter("@p66", mi.InspOrgCode));
            listPs.Add(new MySqlParameter("@p67", mi.InspOrgName));
            listPs.Add(new MySqlParameter("@p68", mi.DespDate));
            listPs.Add(new MySqlParameter("@p69", mi.BlLineNo));
            listPs.Add(new MySqlParameter("@p70", mi.PurpOrgCode));
            
            listPs.Add(new MySqlParameter("@p71", mi.PurpOrgName));
            listPs.Add(new MySqlParameter("@p72", mi.CorrelationNo));
            listPs.Add(new MySqlParameter("@p73", mi.CorrelationReasonFlag));
            listPs.Add(new MySqlParameter("@p74", mi.UseOrgPersonCode));
            listPs.Add(new MySqlParameter("@p75", mi.UseOrgPersonTel));

            listPs.Add(new MySqlParameter("@p76", mi.FileType));
            listPs.Add(new MySqlParameter("@p77", mi.FileTypeName));
            listPs.Add(new MySqlParameter("@p78", mi.RprFlag));
            listPs.Add(new MySqlParameter("@p79", mi.RelationFlag));
            listPs.Add(new MySqlParameter("@p80", mi.PriceFlag));

            listPs.Add(new MySqlParameter("@p81", mi.RoyaltyFlag));
            listPs.Add(new MySqlParameter("@p82", mi.FileType));
            listPs.Add(new MySqlParameter("@p83", mi.FileTypeName));
            listPs.Add(new MySqlParameter("@p84", mi.ApplOri));
            listPs.Add(new MySqlParameter("@p85", mi.ApplCopyQuan));

            listPs.Add(new MySqlParameter("@p86", mi.DomesticConsigneeEname));
            listPs.Add(new MySqlParameter("@p87", mi.OverseasConsignorCname));
            listPs.Add(new MySqlParameter("@p88", mi.OverseasConsignorAddr));
            listPs.Add(new MySqlParameter("@p89", mi.CmplDschrgDt));
            listPs.Add(new MySqlParameter("@p90", mi.PackType));
            listPs.Add(new MySqlParameter("@p91", mi.PackTypeName));
            listPs.Add(new MySqlParameter("@p92", mi.DocNo1));

            //原产地证
            listPs.Add(new MySqlParameter("@p93", mi.DocTypeCode2));
            listPs.Add(new MySqlParameter("@p94", mi.DocNo2));
            listPs.Add(new MySqlParameter("@p95", mi.DecGNo));
            listPs.Add(new MySqlParameter("@p96", mi.EcoGNo));

            #endregion

            listPs.Add(new MySqlParameter("@batch", mi.Batch));
            listPs.Add(new MySqlParameter("@update", mi.UpdateDate));
            listPs.Add(new MySqlParameter("@id", mi.Id));
            listPs.Add(new MySqlParameter("@seqno", mi.SeqNo));
            //继续拼接语句
            sql += "where id=@id and seq_no = @seqno ";
            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql, listPs.ToArray());
        }


        #endregion


        //保存电子委托书
        public int UpadteHeadInfo(SingleWindow mi)
        {

            List<MySqlParameter> listPs = new List<MySqlParameter>();
            //构造update的sql语句
            string sql = @"update bg_cust_dec_head set  
                           pro_df_no=@p1 where seq_no=@p2";

            listPs.Add(new MySqlParameter("@p1", mi.ProDfNo));
            listPs.Add(new MySqlParameter("@p2", mi.SeqNo));

            //执行语句并返回结果
            return mysqlHelper.ExcuteNonQuery(sql, listPs.ToArray());
        }

        /// <summary>
        /// 根据编号删除
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
