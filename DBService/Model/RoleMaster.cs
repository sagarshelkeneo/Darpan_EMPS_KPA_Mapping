using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBService.Model
{
    public class RoleMaster
    {
        public int? WorkId { get; set; }
        public int? RoleTypeId { get; set; }
        public string RoleName { get; set; }
        public string RoleCode { get; set; }
        public int? ShopId { get; set; }
        public string ShopName { get; set; }
        public int? FunctionId { get; set; }
        public string FunctionName { get; set; }
        public int? SectionId { get; set; }
        public string SectionName { get; set; }
    }
}
