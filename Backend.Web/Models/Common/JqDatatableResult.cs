using System.Collections.Generic;

namespace Backend.Web.Models.Common
{
    public class JqDatatableResult<T>
    {
        public JqDatatableResult()
        {
            Data = new List<T>();
        }

        public int Draw { get; set; }

        public int RecordsTotal { get; set; }

        public int RecordsFiltered { get; set; }

        public List<T> Data { get; set; }
    }
}