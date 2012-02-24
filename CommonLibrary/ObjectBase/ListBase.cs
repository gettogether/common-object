using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Data;
using CommonLibrary.Utility;

namespace CommonLibrary.ObjectBase
{
    public class ListBase<T> : List<T>
        where T : class, new()
    {

        public DataTable ToDataTable()
        {
            if (this != null)
                return ListHelper.ListToDataTable(this);
            return null;
        }

        public Table ToTable(params int[] merge_columns)
        {
            if (this != null && this.Count > 0)
                return WebObject.TableHelper.GenListToTable(this, null, null, null, false, merge_columns);
            return null;
        }

        public Table ToTable(string table_css, string header_css, string rows_css, params int[] merge_columns)
        {
            if (this != null && this.Count > 0)
                return WebObject.TableHelper.GenListToTable(this, table_css, header_css, rows_css, false, merge_columns);
            return null;
        }

        public Table ToTable(bool merge_all_columns)
        {
            if (this != null && this.Count > 0)
                return WebObject.TableHelper.GenListToTable(this, null, null, null, merge_all_columns);
            return null;
        }

        public List<O> FieldValuesToList<O>(object fieldName)
        {
            return FieldValuesToList<O>(fieldName.ToString());
        }

        public List<O> FieldValuesToList<O>(string fieldName)
        {
            List<O> ret = new List<O>();
            foreach (T t in this)
            {
                ret.Add((O)DataMapping.ObjectHelper.GetValue<T>(t, fieldName));
            }
            return ret;
        }

        #region Sort Functions

        public void SortBy(string column)
        {
            this.Sort(new Utility.Dynamic.DynamicComparer<T>(column));
        }

        public void SortBy(string column, bool asc)
        {
            this.Sort(new Utility.Dynamic.DynamicComparer<T>(column, asc));
        }

        public void SortBy(Dictionary<string, bool> sortList)
        {
            this.Sort(new Utility.Dynamic.DynamicComparer<T>(sortList));
        }

        public void SortBy(object column)
        {
            SortBy(column.ToString());
        }

        public void SortBy(object column, bool asc)
        {
            SortBy(column.ToString(), asc);

        }

        public void SortBy(Dictionary<object, bool> sortList)
        {
            Dictionary<string, bool> d = new Dictionary<string, bool>();
            foreach (KeyValuePair<object, bool> sort in sortList)
            {
                d.Add(sort.Key.ToString(),sort.Value);
            }
            SortBy(d);
        }
        #endregion

        #region Search Functions

        public C Search<C>(Predicate<T> match) where C : ICollection<T>, new()
        {
            C ret = new C();
            foreach (T t in base.FindAll(match))
                ret.Add(t);
            return ret;
        }

        public C Search<C, V>(string column, V v) where C : ICollection<T>, new()
        {
            string key = new StringBuilder(column).Append(v).ToString();
            PredicateHelper<V> ph = CacheHelper.GetCache(key) as PredicateHelper<V>;
            if (ph == null)
            {
                ph = new PredicateHelper<V>(column, v);
                CacheHelper.SetCache(key, ph);
            }
            return Search<C>(ph.Find<T>);
        }

        public C Search<C, V>(string column, V v, TokenTypes token) where C : ICollection<T>, new()
        {
            string key = new StringBuilder(column).Append(v).Append(token).ToString();
            PredicateHelper<V> ph = CacheHelper.GetCache(key) as PredicateHelper<V>;
            if (ph == null)
            {
                ph = new PredicateHelper<V>(column, v, token);
                CacheHelper.SetCache(key, ph);
            }
            return Search<C>(ph.Find<T>);
        }

        public C Search<C, V>(object column, V v) where C : ICollection<T>, new()
        {
            return Search<C, V>(column.ToString(), v);
        }

        public C Search<C, V>(object column, V v, TokenTypes token) where C : ICollection<T>, new()
        {
            return Search<C, V>(column.ToString(), v, token);
        }

        #endregion

        #region Other Functions

        public ListBase<T> GetPaging(int pageSize, int pageIndex)
        {
            return GetPaging(pageSize, pageIndex, "", true);
        }

        public ListBase<T> GetPaging(int pageSize, int pageIndex, object sortColumn, bool isAsc)
        {
            return GetPaging(pageSize, pageIndex, sortColumn.ToString(), true);
        }

        public ListBase<T> GetPaging(int pageSize, int pageIndex, string sortColumn, bool isAsc)
        {
            ListBase<T> ret = new ListBase<T>();
            if (!string.IsNullOrEmpty(sortColumn))
            {
                this.SortBy(sortColumn, isAsc);
            }
            int index;
            if (this.Count > pageSize)
            {
                for (index = (pageIndex - 1) * pageSize; index < pageSize * pageIndex && index < this.Count; index++)
                {
                    ret.Add(this[index]);
                }
                return ret;
            }
            else
                return this;
        }

        #endregion
    }
}
