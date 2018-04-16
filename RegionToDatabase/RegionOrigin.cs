using System;
using System.Collections.Generic;
using System.Text;

namespace RegionToDatabase
{
    public class RegionOrigin
    {
        public string Name { get; set; }
        public string Href { get; set; }
        public string Code { get; set; }
        public string Type { get; set; }
        public int Level { get; set; }
        public List<RegionOrigin> Children { get; set; }
    }

    /// <summary>
    /// 数据库中的表
    /// </summary>
    public class Region
    {
        //名称
        public string Name { get; set; }

        //代码
        public string Code { get; set; }

        //父节点代码
        public string ParentCode { get; set; }

        //类型
        public string Type { get; set; }

        //层
        public int Level { get; set; }
    }
}
