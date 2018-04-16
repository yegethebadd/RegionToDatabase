using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;

namespace RegionToDatabase
{
    class Program
    {
        static void Main(string[] args)
        {
            //先把数据读取出来
            string jsonstring = File.ReadAllText(@"D:\全国数据.json");
            var regionOrigins = JsonConvert.DeserializeObject<List<RegionOrigin>>(jsonstring);

            List<Region> regions = new List<Region>();
            foreach (var ro in regionOrigins)
            {
                regions.AddRange(ParseRegion(ro));
            }

            //数据库连接字段
            string connectionStr = @"Data Source=.;Initial CataLog=DB;User ID=sa;Password=123456";
            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                con.Open();
                foreach(var item in regions)
                {
                    //按需选择，此处不保存居委会信息
                    if (item.Level != 5)
                    {
                        if (SaveToDatabase(con, item))
                        {
                            Console.WriteLine("Success : " + item.Name);
                        }
                        else
                        {
                            Console.WriteLine("Error : " + item.Name);
                        }
                    }
                }
            }
        }

        private static bool SaveToDatabase(SqlConnection con, Region region)
        {
            try
            {
                string insertQuery = @"Insert into Region(Name,Code,ParentCode,Type,Level) VALUES(@Name,@Code,@ParentCode,@Type,@Level)";
                using(SqlCommand cmd=new SqlCommand(insertQuery, con))
                {
                    cmd.Parameters.Add(new SqlParameter("@Name", region.Name));
                    cmd.Parameters.Add(new SqlParameter("@Code", region.Code));
                    cmd.Parameters.Add(new SqlParameter("@ParentCode", region.ParentCode));
                    cmd.Parameters.Add(new SqlParameter("@Type", region.Type));
                    cmd.Parameters.Add(new SqlParameter("@Level", region.Level));
                    cmd.ExecuteNonQuery();
                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// 将原始格式的数据转换为数据库Region格式
        /// </summary>
        /// <param name="ro"></param>
        /// <param name="parentCode"></param>
        /// <returns></returns>
        private static IEnumerable<Region> ParseRegion(RegionOrigin ro, string parentCode = "")
        {
            List<Region> regions = new List<Region>();

            Region region = new Region();
            region.Code = ro.Code;
            region.Level = ro.Level;
            region.Name = ro.Name;
            region.Type = ro.Type;
            region.ParentCode = parentCode;
            regions.Add(region);

            if (ro.Children != null)
            {
                foreach(var roc in ro.Children)
                {
                    regions.AddRange(ParseRegion(roc, ro.Code));
                }
            }
            return regions;
        }
    }
}
