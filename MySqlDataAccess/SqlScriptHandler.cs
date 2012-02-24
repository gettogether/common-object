using System;
using System.Collections.Generic;
using System.Text;

namespace MySqlDataAccess
{
    public enum ParameterType
    {
        And,
        Or,
        Initial
    }
    public class Pars
    {
        #region Attributes
        private ParameterType _parType;
        private TokenTypes _tokentype;
        private string _column;
        private object _value;
        private string _columnpar;
        public ParameterType ParType
        {
            get { return _parType; }
            set { _parType = value; }
        }
        public TokenTypes TokType
        {
            get { return _tokentype; }
            set { _tokentype = value; }
        }
        public string Column
        {
            get { return _column; }
            set { _column = value; _columnpar = value; }
        }
        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public string ColumnPar
        {
            get { return _columnpar; }
            set { _columnpar = value; }
        }
        #endregion
        #region Constructors
        public Pars(ParameterType parType, TokenTypes tokType, string column, object value)
        {
            this.TokType = tokType;
            this.ParType = parType;
            this.Column = column;
            this.Value = value;
        }
        public Pars(ParameterType parType, TokenTypes tokType, object column, object value)
        {
            this.TokType = tokType;
            this.ParType = parType;
            this.Column = column.ToString();
            this.Value = value;
        }
        public Pars()
        {

        }
        #endregion
    }

    public class ParsList : List<Pars>
    {
        #region Constructors
        public ParsList()
        {

        }
        public ParsList(params Pars[] pars)
        {
            foreach (Pars p in pars) this.Add(p);
        }
        #endregion
    }
    public enum TokenTypes
    {
        Equal,
        Like,
        LeftLike,
        RightLike,
        LessThanEqual,
        LessThan,
        GreaterThan,
        GreaterThanEqual,
        Unknown
    }
    public static class SqlScriptHandler
    {
        public const string ColumnTempString = "{0}";
        public const string ValueTmepString = "@{0}";
        public const string UnInsert = ",UN-INSERT";
        public const string UnUpdate = ",UN-UPDATE";
        public readonly static string UpdateColumnTempString = string.Format("{0}={1}", ColumnTempString, ValueTmepString);

        #region Functions
        public static ParsList ProcessColumnPars(ParsList pars)
        {
            List<string> code_list = new List<string>();
            ParsList processed_pars = new ParsList();
            int i = 0;
            foreach (Pars par in pars)
            {
                if (code_list.Contains(par.ColumnPar))
                {
                    par.ColumnPar = (par.ColumnPar + (++i));
                    processed_pars.Add(par);
                }
                else
                {
                    processed_pars.Add(par);
                }
                code_list.Add(par.ColumnPar);
            }
            return processed_pars;
        }
        public static string GetParsedExpression(string defaultSql, string tableName, ParsList pars)
        {
            StringBuilder my_sql = new StringBuilder(string.Format(defaultSql, tableName));
            bool is_first = true;
            foreach (Pars p in ProcessColumnPars(pars))
            {
                if (is_first)
                {
                    my_sql.Append(string.Format(" {0} {1} {2} @{3}", "where", p.Column, GetTokenType(p.TokType), p.ColumnPar));
                    is_first = false;
                }
                else
                {
                    my_sql.Append(string.Format(" {0} {1} {2} @{3}", GetParameterType(p.ParType), p.Column, GetTokenType(p.TokType), p.ColumnPar));
                }
            }
            return my_sql.ToString();
        }
        public static string GetTokenType(TokenTypes tokType)
        {
            switch (tokType)
            {
                case TokenTypes.Equal:
                    return "=";
                case TokenTypes.Like:
                    return "like";
                case TokenTypes.GreaterThan:
                    return ">";
                case TokenTypes.LessThan:
                    return "<";
                case TokenTypes.GreaterThanEqual:
                    return "=>";
                case TokenTypes.LessThanEqual:
                    return "<=";
                default:
                    return "=";
            }
        }
        public static object ProcessValue(TokenTypes tokType, object value)
        {
            if (tokType == TokenTypes.Like)
                return string.Format("%{0}%", value);
            else if (tokType == TokenTypes.Like)
                return string.Format("{0}%", value);
            else if (tokType == TokenTypes.RightLike)
                return string.Format("%{0}", value);
            else
                return value;
        }
        public static string GetParameterType(ParameterType parType)
        {
            switch (parType)
            {
                case ParameterType.And:
                    return "and";
                case ParameterType.Or:
                    return "or";
                default:
                    return "and";
            }
        }
        public static string[] ObjectsToStrings(params object[] objs)
        {
            List<string> listStr = new List<string>();
            foreach (object o in objs) listStr.Add(o.ToString());
            return listStr.ToArray();
        }
        #endregion

        public static class Search
        {
            public const string SelectIdentityId = "select @@identity id";
            public const string InitiallySelectString = "select {0} from {1} {2}";
            public static readonly string SelectString = string.Format(InitiallySelectString, "*", "{0}", "{1}"); //"select * from {0} {1}";
            public const string OrderString = " order by {0} {1}";
            public const string AscString = " asc";
            public const string DescString = " desc";
            public const string TopString = "{1} limit {0} ";
            public static readonly string DefaultSelectString = string.Format(InitiallySelectString, "*", "", "{0}"); //select * from {1}";
            #region Functions
            #region Private Functions
            private static string ColumnArrToString(object[] columns)
            {
                if (columns == null || columns.Length == 0) return "*";
                StringBuilder sb = new StringBuilder();
                foreach (object s in columns)
                {
                    if (sb.Length <= 0)
                    {
                        sb.Append(s);
                    }
                    else
                    {
                        sb.Append("," + s);
                    }
                }
                return sb.ToString();
            }

            private static string ColumnArrToString(string[] columns)
            {
                if (columns == null || columns.Length == 0) return "*";
                StringBuilder sb = new StringBuilder();
                foreach (string s in columns)
                {
                    if (sb.Length <= 0)
                    {
                        sb.Append(s);
                    }
                    else
                    {
                        sb.Append("," + s);
                    }
                }
                return sb.ToString();
            }
            private static string GetSelectString(string tableName, ParsList pars)
            {
                return GetParsedExpression(DefaultSelectString, tableName, pars);
            }
            private static string GetSelectString(string tableName, ParsList pars, bool asc, params string[] orderBy)
            {
                return GetParsedExpression(DefaultSelectString, tableName, pars) + GetOrderString(asc, orderBy);
            }
            #endregion

            #region Public Functions
            public static string GetOrderString(bool asc, params string[] orderBy)
            {
                string orders = string.Empty;
                if (orderBy != null && orderBy.Length > 0)
                {
                    foreach (string s in orderBy)
                        orders += (string.IsNullOrEmpty(orders) ? "" : ",") + string.Format(ColumnTempString, s);
                    orders = string.Format(OrderString, orders, (asc ? AscString : DescString));
                }
                return orders;
            }

            public static string GetOrderString(bool asc, params object[] orderBy)
            {
                return GetOrderString(asc, ObjectsToStrings(orderBy));
            }

            public static string ProcessSelectTopString(int count, string orderString)
            {
                return string.Format(SelectString, "{0}", string.Format(TopString, count, orderString));
            }

            public static string ProcessSelectTopString(string columns, int count,string orderString)
            {
                return string.Format(InitiallySelectString, columns, "{0}", string.Format(TopString, count, orderString));
            }

            public static string GetSelectString(string tableName, object[] columns)
            {
                return string.Format(InitiallySelectString, ColumnArrToString(columns), "", tableName);
            }

            public static string GetSelectString(string tableName, string[] columns)
            {
                return string.Format(InitiallySelectString, ColumnArrToString(columns), "", tableName);
            }

            public static string GetSelectString(string tableName)
            {
                return string.Format(DefaultSelectString, tableName);
            }

            //public static string GetSelectTopString(string tableName, int count)
            //{
            //    return string.Format(ProcessSelectTopString(count, ""), tableName);
            //}

            public static string GetSelectTopString(string tableName, int count, string orderString)
            {
                return string.Format(ProcessSelectTopString(count, orderString), tableName);
            }

            public static string GetSelectTopString(string tableName, string[] columns, int count,string orderString)
            {
                return string.Format(ProcessSelectTopString(ColumnArrToString(columns), count, orderString), tableName);
            }

            public static string GetSelectTopString(string tableName, object[] columns, int count,string orderString)
            {
                return string.Format(ProcessSelectTopString(ColumnArrToString(columns), count, orderString), tableName);
            }

            public static string GetSelectOrderString(string tableName, object[] columns, bool asc, params object[] orderBy)
            {
                return GetSelectString(tableName, columns) + GetOrderString(asc, orderBy);
            }

            public static string GetSelectOrderString(string tableName, bool asc, params object[] orderBy)
            {
                return GetSelectString(tableName) + GetOrderString(asc, orderBy);
            }

            public static string GetSelectTopOrderString(string tableName, int count, bool asc, params object[] orderBy)
            {
                return GetSelectTopString(tableName, count, GetOrderString(asc, orderBy));
            }

            public static string GetSelectTopOrderString(string tableName, int count, bool asc, params string[] orderBy)
            {
                return GetSelectTopString(tableName, count, GetOrderString(asc, orderBy));
            }

            public static string GetSelectString(string tableName, ParsList pars, int count)
            {
                if (count > 0)
                    return GetParsedExpression(ProcessSelectTopString(count, ""), tableName, pars);
                else
                    return GetSelectString(tableName, pars);
            }

            public static string GetSelectString(string tableName, ParsList pars, bool asc, int count, params string[] orderBy)
            {
                if (count > 0)
                    return GetParsedExpression(ProcessSelectTopString(count, GetOrderString(asc, orderBy)), tableName, pars);
                else
                    return GetSelectString(tableName, pars, asc, orderBy);
            }
            #endregion
            #endregion
        }

        public static class Insert
        {
            public const string InsertString = "insert into {0} ({1}) values({2})";
        }

        public static class Delete
        {
            public const string InsertString = "delete from {0}";
            public static string GetDeleteString(string tableName, ParsList pars)
            {
                return GetParsedExpression(InsertString, tableName, pars);
            }
        }

        public static class Update
        {
            public const string UpdateString = "update {0} set {1}";
        }
    }
}
