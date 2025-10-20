using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBService.Model
{
    public class KPAMaster
    {
        public int CategoryId { get; set; }
        public int DataSourceId { get; set; }
        public string KpaName { get; set; }
        public string UnitOfKpa { get; set; }
        public string DataSourceText { get; set; }
        public string OtherSource { get; set; }
        public int ImportanceId { get; set; }
        public string Comment { get; set; }
    }

}
