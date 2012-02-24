using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DataAccess.Data
{
    public abstract class StoreProcBase<T, R> : Interfaces.IStoreProcBase<T, R>
        where T : class, new()
        where R : class, new()
    {
        #region Store procedure information.

        private StoreProcInformation _StoreProcInfo;
        public StoreProcInformation StoreProcInfo
        {
            get { return _StoreProcInfo; }
            set { _StoreProcInfo = value; }
        }

        #endregion

        #region Query functions

        public abstract R GetResults(IDataParameter[] parameters);

        public abstract DataSet GetDataSet(IDataParameter[] parameters);

        public abstract IDataReader GetDataReader(IDataParameter[] parameters);

        #endregion
    }
}
