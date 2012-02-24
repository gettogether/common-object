using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using MySqlDataAccess;
using MySql.Data.MySqlClient;

namespace MySqlDataAccess.Data
{
    public class UOObjectBase<T> : IUOObject<T>, IFormattable where T : class, new()
    {
        public DataObjectInfo DOInfo;
        #region Insert Functions
        public int Insert(T t)
        {
            return SqlUtil.ExecuteInsert<T>(DOInfo.ConnectionKey, DOInfo.TableName, t);
        }

        public int Insert()
        {
            return 0;
        }

        public int Insert(MySqlConnection cnn, MySqlTransaction tran, T t)
        {
            return SqlUtil.ExecuteInsert<T>(cnn, tran, DOInfo.TableName, t);
        }
        #endregion

        #region Update Functions
        public int Update(ParsList pars, T t)
        {
            return SqlUtil.ExecuteUpdate<T>(DOInfo.ConnectionKey, DOInfo.TableName, t, pars);
        }

        public int Update(MySqlConnection cnn, MySqlTransaction tran, ParsList pars, T t)
        {
            return SqlUtil.ExecuteUpdate<T>(cnn, tran, DOInfo.TableName, t, pars);
        }
        #endregion

        #region Other Functions
        public override string ToString()
        {
            return this.DOInfo.TableName;
        }

        string System.IFormattable.ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider != null)
            {
                ICustomFormatter fmt = formatProvider.GetFormat(this.GetType()) as ICustomFormatter;
                if (fmt != null)
                    return fmt.Format(format, this, formatProvider);
                switch (format)
                {
                    case "n": return ToString();
                    case "s": return DOInfo.DefaultSelect;
                    case "c": return DOInfo.ConnectionKey;
                    case "cns": return DOInfo.ConnectionKey + "|" + DOInfo.TableName + "|" + DOInfo.DefaultSelect;
                    default: return ToString();
                }
            }
            return ToString();
        }
        #endregion
    }
}
