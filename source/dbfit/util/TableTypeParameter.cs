using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dbfit.util
{
    public class TableTypeParameter
    {
        String _tabletype;
        DataTable _datatable;

        public TableTypeParameter(String tabletype, DataTable datatable)
        {
            _tabletype = tabletype;
            _datatable = datatable;
        }

        public String Tabletype { get { return _tabletype; } }

        public DataTable Datatable { get { return _datatable; }  }
         
    }
}
