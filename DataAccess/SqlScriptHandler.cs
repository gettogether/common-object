using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace DataAccess
{
    public static class DateTimeValues
    {
        public static readonly DateTime Null = DateTime.MinValue;
        public static readonly DateTime DbTime = DateTime.MaxValue;
    }

    public class Parameter
    {
        #region Attributes
        private ParameterType _ParType = ParameterType.Initial;
        private TokenTypes _TokenType;
        private string _Column;
        private object _Value;
        private string _ColumnPar;
        private bool _IsStartParenthesis;

        public bool IsStartParenthesis
        {
            get { return _IsStartParenthesis; }
            set { _IsStartParenthesis = value; }
        }
        private bool _IsEndParenthesis;

        public bool IsEndParenthesis
        {
            get { return _IsEndParenthesis; }
            set { _IsEndParenthesis = value; }
        }

        public ParameterType ParType
        {
            get { return _ParType; }
            set { _ParType = value; }
        }
        public TokenTypes TokType
        {
            get { return _TokenType; }
            set { _TokenType = value; }
        }
        public string Column
        {
            get { return _Column; }
            set { _Column = value; _ColumnPar = value; }
        }
        public object Value
        {
            get { return _Value; }
            set { _Value = value; }
        }
        public string ColumnPar
        {
            get { return _ColumnPar; }
            set { _ColumnPar = value; }
        }
        #endregion

        #region Constructors
        public Parameter(ParameterType parType, TokenTypes tokType, string column, object value)
        {
            this.TokType = tokType;
            this.ParType = parType;
            this.Column = column;
            this.Value = value;
        }
        public Parameter(ParameterType parType, TokenTypes tokType, object column, object value)
        {
            this.TokType = tokType;
            this.ParType = parType;
            this.Column = column.ToString();
            this.Value = value;
        }
        public Parameter(TokenTypes tokType, object column, object value)
        {
            this.TokType = tokType;
            this.ParType = ParameterType.Initial;
            this.Column = column.ToString();
            this.Value = value;
        }
        public Parameter(object column, object value)
        {
            this.Column = column.ToString();
            this.Value = value;
        }

        public Parameter()
        {

        }
        #endregion
    }

    public class ParameterCollection : List<Parameter>
    {
        #region Constructors
        public ParameterCollection()
        {

        }
        public ParameterCollection(params Parameter[] pars)
        {
            foreach (Parameter p in pars) this.Add(p);
        }
        #endregion

        #region Parenthesis

        public void AddStartParenthesis()
        {
            Parameter p = new Parameter();
            p.IsStartParenthesis = true;
            this.Add(p);
        }
        public void AddEndParenthesis()
        {
            Parameter p = new Parameter();
            p.IsEndParenthesis = true;
            this.Add(p);
        }
        public void AddStartParenthesisWithAnd()
        {
            Parameter p = new Parameter();
            p.IsStartParenthesis = true;
            p.ParType = ParameterType.And;
            this.Add(p);
        }
        public void AddStartParenthesisWithOr()
        {
            Parameter p = new Parameter();
            p.IsStartParenthesis = true;
            p.ParType = ParameterType.Or;
            this.Add(p);
        }

        #endregion

        public void AddValue(object column, object value)
        {
            this.Add(new Parameter(column, value));
        }

        public void AddValue(string column, object value)
        {
            this.Add(new Parameter(column, value));
        }

        public void AddCondition(ParameterType parType, TokenTypes tokType, object column, object value)
        {
            this.Add(new Parameter(parType, tokType, column, value));
        }

        public void AddCondition(ParameterType parType, TokenTypes tokType, string column, object value)
        {
            this.Add(new Parameter(parType, tokType, column, value));
        }
    }

    public static class SqlScriptHandler
    {
        public const string GetDBTime = "GetDate()";
        public const string ColumnTempString = "[{0}]";
        public const string ValueTmepString = "@{0}";
        public const string UnInsert = ",UN-INSERT";
        public const string UnUpdate = ",UN-UPDATE";
        public readonly static string ColumnWithValueTempString = string.Format("{0}={1}", ColumnTempString, ValueTmepString);
        public const string ColumnWithValueNotParameter = "[{0}]={1}";

        #region Functions

        public static string ArrayToString(string[] array, string separate)
        {
            return ArrayToString(array, separate, true);
        }

        public static string ArrayToString(string[] array, string separate, bool isColumns)
        {
            if (array == null) return string.Empty;
            StringBuilder sbReturn = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                string s = array[i];
                if (isColumns) s = string.Concat(string.Format(ColumnTempString, s));
                if (sbReturn.Length == 0)
                    sbReturn.Append(s);
                else
                    sbReturn.Append(separate).Append(s);
            }
            return sbReturn.ToString();
        }

        public static string ArrayToString(object[] array, string separate)
        {
            return ArrayToString(array, separate, true);
        }

        public static string ArrayToString(object[] array, string separate, bool isColumns)
        {
            if (array == null) return string.Empty;
            StringBuilder sbReturn = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                string s = array[i].ToString();
                if (isColumns) s = string.Concat(string.Format(ColumnTempString, s));
                if (sbReturn.Length == 0)
                    sbReturn.Append(s);
                else
                    sbReturn.Append(separate).Append(s);
            }
            return sbReturn.ToString();
        }

        public static ParameterCollection ProcessColumnPars(ParameterCollection pc)
        {
            List<string> codes = new List<string>();
            ParameterCollection pcProcessed = new ParameterCollection();
            int i = 0;
            foreach (Parameter par in pc)
            {
                if (codes.Contains(par.ColumnPar))
                {
                    par.ColumnPar = (par.ColumnPar + (++i));
                    pcProcessed.Add(par);
                }
                else
                {
                    pcProcessed.Add(par);
                }
                codes.Add(par.ColumnPar);
            }
            return pcProcessed;
        }

        public static string GetParsedExpression(string cmdText, string tableName, ParameterCollection pc)
        {
            return new StringBuilder(string.Format(cmdText, tableName)).Append(GetCondition(pc, false)).ToString();
        }

        public static string GetCondition(ParameterCollection pc, bool isAppend)
        {
            if (pc == null || pc.Count == 0) return string.Empty;
            pc[0].ParType = ParameterType.Initial;
            StringBuilder sbParameters = new StringBuilder();
            foreach (Parameter p in ProcessColumnPars(pc))
            {
                if (!isAppend)
                {
                    sbParameters.Append(" where ");
                    isAppend = true;
                }
                if (p.ParType != ParameterType.Initial)
                {
                    sbParameters.Append(" ").Append(SqlScriptHandler.GetParameterType(p.ParType)).Append(" ");
                }
                if (p.IsStartParenthesis)
                {
                    sbParameters.Append("( ");
                    continue;
                }
                else if (p.IsEndParenthesis)
                {
                    sbParameters.Append(" )");
                    continue;
                }
                sbParameters.Append(string.Format(ColumnTempString, p.Column)).Append(" ").Append(SqlScriptHandler.GetTokenType(p.TokType, p.Value)).Append(" ");
                if (p.TokType == DataAccess.TokenTypes.IsNotNull || p.TokType == DataAccess.TokenTypes.IsNull || p.TokType == TokenTypes.In || p.TokType == TokenTypes.NotIn) continue;
                sbParameters.Append("@").Append(p.ColumnPar);
            }
            return sbParameters.ToString();
        }

        public static string GetTokenType(TokenTypes tokType, object value)
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
                    return ">=";
                case TokenTypes.LessThanEqual:
                    return "<=";
                case TokenTypes.IsNotNull:
                    return "is not null";
                case TokenTypes.IsNull:
                    return "is null";
                case TokenTypes.LeftLike:
                    return "like";
                case TokenTypes.RightLike:
                    return "like";
                case TokenTypes.In:
                    return ProcessInNotIn(tokType, value);
                case TokenTypes.NotIn:
                    return ProcessInNotIn(tokType, value);
                case TokenTypes.NotEqual:
                    return "<>";
                default:
                    return "=";
            }
        }

        private static string ProcessInNotIn(TokenTypes tokType, object value)
        {
            StringBuilder sbValues = new StringBuilder();
            if (value.GetType() == typeof(System.String[]))
            {
                foreach (object v in (string[])value)
                {
                    if (sbValues.Length > 0) sbValues.Append(",");
                    sbValues.Append("'").Append(v).Append("'");
                }
            }
            else if (value.GetType() == typeof(List<string>))
            {
                foreach (object v in (List<string>)value)
                {
                    if (sbValues.Length > 0) sbValues.Append(",");
                    sbValues.Append("'").Append(v).Append("'");
                }
            }
            else
            {
                if (value.GetType() == typeof(int[]))
                {
                    foreach (object v in (int[])value)
                    {
                        if (sbValues.Length > 0) sbValues.Append(",");
                        sbValues.Append(v);
                    }
                }
                else if (value.GetType() == typeof(decimal[]))
                {
                    foreach (object v in (decimal[])value)
                    {
                        if (sbValues.Length > 0) sbValues.Append(",");
                        sbValues.Append(v);
                    }
                }
                else if (value.GetType() == typeof(double[]))
                {
                    foreach (object v in (double[])value)
                    {
                        if (sbValues.Length > 0) sbValues.Append(",");
                        sbValues.Append(v);
                    }
                }
            }
            if (tokType == TokenTypes.In)
                sbValues.Insert(0, "in (");
            else
                sbValues.Insert(0, "not in (");
            sbValues.Append(")");
            return sbValues.ToString();
        }

        public static object ProcessValue(TokenTypes tokType, object value)
        {
            if (tokType == TokenTypes.Like)
                return string.Format("%{0}%", value);
            else if (tokType == TokenTypes.Like)
                return string.Format("{0}%", value);
            else if (tokType == TokenTypes.RightLike)
                return string.Format("%{0}", value);
            else if (tokType == TokenTypes.LeftLike)
                return string.Format("{0}%", value);
            else
                return value;
        }

        public static string GetConditionsFromParsList(ParameterCollection pc)
        {
            if (pc == null || pc.Count == 0) return string.Empty;
            pc[0].ParType = ParameterType.Initial;
            StringBuilder sbParameters = new StringBuilder();
            foreach (Parameter p in pc)
            {
                if (p.ParType != ParameterType.Initial)
                {
                    if (p.IsStartParenthesis || p.IsEndParenthesis)
                        sbParameters.Append(" ");
                    sbParameters.Append(SqlScriptHandler.GetParameterType(p.ParType)).Append(" ");
                }
                if (p.IsStartParenthesis)
                {
                    sbParameters.Append("(");
                    continue;
                }
                else if (p.IsEndParenthesis)
                {
                    sbParameters.Append(") ");
                    continue;
                }
                sbParameters.Append(string.Format(ColumnTempString, p.Column)).Append(" ").Append(SqlScriptHandler.GetTokenType(p.TokType, p.Value)).Append(" ");
                if (p.TokType == DataAccess.TokenTypes.IsNotNull || p.TokType == DataAccess.TokenTypes.IsNull || p.TokType == TokenTypes.In || p.TokType == TokenTypes.NotIn) continue;
                if (p.Value.GetType() == typeof(string))
                {
                    if (p.TokType == TokenTypes.LeftLike || p.TokType == TokenTypes.Like || p.TokType == TokenTypes.RightLike) sbParameters.Append("N");
                    sbParameters.Append("'").Append(SqlScriptHandler.ProcessValue(p.TokType, p.Value).ToString().Replace("'", "''")).Append("'").Append(" ");
                }
                else if (p.Value.GetType() == typeof(DateTime))
                {
                    sbParameters.Append("'").Append(p.Value).Append("'").Append(" ");
                }
                else if (p.Value.GetType() == typeof(Boolean))
                {
                    sbParameters.Append((int)((bool)p.Value ? 1 : 0));
                }
                else
                {
                    sbParameters.Append(p.Value).Append(" ");
                }

            }
            return sbParameters.ToString();
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
            public const string NewID = "newid()";
            public const string RowCount = "@@rowcount";
            public const string SelectIdentityId = "select isnull(@@identity,@@rowcount)";
            public const string InitiallySelectString = "select {0}{1} from [{2}](nolock)";
            public const string SelectRecordCountString = "select count(*) from [{0}](nolock)" ;
            public static readonly string SelectString = string.Format(InitiallySelectString, "{0}", "*", "{1}"); //"select {0}* from [{1}](nolock)";
            public const string OrderString = " order by {0} {1}";
            public const string AscString = " asc";
            public const string DescString = " desc";
            public const string TopString = "top {0} " ;
            public static readonly string DefaultSelectString = string.Format(InitiallySelectString, "*", "", "{0}"); //select * from [{1}](nolock)";
            //public const string SelectPagingString = "select top {1} * from [{0}](nolock) where [{3}] not in(select top {2} [{3}] from [{0}](nolock) where 1=1{4} order by [{3}] asc){4}";
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
            private static string GetSelectString(string tableName, ParameterCollection pc)
            {
                return GetParsedExpression(DefaultSelectString, tableName, pc);
            }
            private static string GetSelectString(string tableName, ParameterCollection pc, bool asc, params string[] orderBy)
            {
                return GetParsedExpression(DefaultSelectString, tableName, pc) + GetOrderString(asc, orderBy);
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

            public static string ProcessSelectTopString(int count)
            {
                return string.Format(SelectString, string.Format(TopString, count), "{0}");
            }

            public static string ProcessSelectTopString(string columns, int count)
            {
                return string.Format(InitiallySelectString, string.Format(TopString, count), columns, "{0}");
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

            public static string GetSelectTopString(string tableName, int count)
            {
                return string.Format(ProcessSelectTopString(count), tableName);
            }

            public static string GetSelectTopString(string tableName, string[] columns, int count)
            {
                return string.Format(ProcessSelectTopString(ColumnArrToString(columns), count), tableName);
            }

            public static string GetSelectTopString(string tableName, object[] columns, int count)
            {
                return string.Format(ProcessSelectTopString(ColumnArrToString(columns), count), tableName);
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
                return GetSelectTopString(tableName, count) + GetOrderString(asc, orderBy);
            }

            public static string GetSelectTopOrderString(string tableName, int count, bool asc, params string[] orderBy)
            {
                return GetSelectTopString(tableName, count) + GetOrderString(asc, orderBy);
            }

            public static string GetSelectString(string tableName, ParameterCollection pc, int count)
            {
                if (count > 0)
                    return GetParsedExpression(ProcessSelectTopString(count), tableName, pc);
                else
                    return GetSelectString(tableName, pc);
            }

            public static string GetSelectString(string tableName, ParameterCollection pc, bool asc, int count, params string[] orderBy)
            {
                if (count > 0)
                    return GetParsedExpression(ProcessSelectTopString(count), tableName, pc) + GetOrderString(asc, orderBy);
                else
                    return GetSelectString(tableName, pc, asc, orderBy);
            }

            public static string CreateSelectCommandText(DataTable tb)
            {
                StringBuilder sb = new StringBuilder("select ");
                foreach (DataColumn c in tb.Columns)
                {
                    sb.Append(c.ColumnName);
                    sb.Append(", ");
                }
                sb.Remove(sb.Length - 2, 2);
                sb.Append(" from ");
                sb.Append(tb.TableName);
                return sb.ToString();
            }

            #endregion
            #endregion
        }

        public static class Insert
        {
            public const string InsertString = "insert into [{0}] ({1}) values({2})";
        }

        public static class Delete
        {
            public const string InsertString = "delete from [{0}]";
            public static string GetDeleteString(string tableName, ParameterCollection pc)
            {
                return GetParsedExpression(InsertString, tableName, pc);
            }
        }

        public static class Update
        {
            public const string UpdateString = "update [{0}] set {1}";
        }
    }
}
