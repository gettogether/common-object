using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Collections;

namespace CommonLibrary.WebObject
{
    public class TableHelper
    {
        public static TableCell BuiltTableCell(string text)
        {
            return BuiltTableCell(text, null);
        }

        public static TableCell BuiltTableCell(string text, string cssClass)
        {
            return BuiltTableCell(text, cssClass, null, 1, null);
        }

        public static TableCell BuiltTableCell(int borderWidth, string text)
        {
            //string style_str = string.Concat("border-width:", borderWidth, "; border-style:solid;");
            string style_str = string.Concat("border: ", borderWidth, "px solid #C1DAD7;padding: 3px 3px 3px 3px;");
            return BuiltTableCell(text, null, null, 1, style_str);
        }

        public static TableCell BuiltTableCell(string text, string cssClass, string id, int columnspan, string style_str)
        {
            TableCell tc = new TableCell();
            tc.Text = text;
            tc.CssClass = cssClass;
            tc.ID = id;
            if (columnspan != 1)
                tc.ColumnSpan = columnspan;
            tc.Attributes.Add("style", style_str);
            return tc;
        }

        public static TableRow BuiltTableRow(params TableCell[] tableCells)
        {
            TableRow tr = new TableRow();
            foreach (TableCell tc in tableCells)
            {
                tr.Cells.Add(tc);
            }
            return tr;
        }

        public static TableRow BuiltTableRow(params string[] text)
        {
            TableRow tr = new TableRow();
            foreach (string s in text)
            {
                tr.Cells.Add(BuiltTableCell(s));
            }
            return tr;
        }

        public static TableRow BuiltTableRow(int borderWidth, params string[] text)
        {
            TableRow tr = new TableRow();
            foreach (string s in text)
            {
                tr.Cells.Add(BuiltTableCell(borderWidth, s));
            }
            return tr;
        }

        public static TableRow BuiltTableRow(string tcCssClass, params string[] text)
        {
            return BuiltTableRow(null, null, tcCssClass, null, null, 1, text);
        }

        public static TableRow BuiltTableRow(string trCssClass, string trstyle_str, string tcCssClass, string tcstyle_str, string tcid, int tccolumnspan, params string[] text)
        {
            TableRow tr = new TableRow();
            if (text != null && text.Length != 0)
            {
                int count = 0;
                foreach (string s in text)
                {
                    if (!string.IsNullOrEmpty(tcid) && text.Length > 1)
                    {
                        tcid = string.Concat(tcid, "_", count);
                        ++count;
                    }
                    tr.Cells.Add(BuiltTableCell(s, tcCssClass, tcid, tccolumnspan, tcstyle_str));
                }
            }
            else
            {
                tr.Cells.Add(BuiltTableCell(null, tcCssClass, tcid, tccolumnspan, tcstyle_str));
            }
            tr.CssClass = trCssClass;
            tr.Attributes.Add("style", trstyle_str);
            return tr;
        }

        public static TableRow BuiltTableRow(int borderWidth, string trCssClass, params string[] text)
        {
            TableRow tr = BuiltTableRow(borderWidth, text);
            tr.CssClass = trCssClass;
            return tr;
        }

        public static void MergeRows(Table t, params int[] cols_to_merge)
        {
            if (t == null || t.Rows.Count == 0) return;
            string[] last_vals = new string[t.Rows[0].Cells.Count];
            TableCell[] last_cells = new TableCell[last_vals.Length];
            bool[] can_merge = new bool[last_vals.Length];

            for (int i = 0; i < can_merge.Length; i++) can_merge[i] = false;
            if (cols_to_merge != null)
            {
                foreach (int i in cols_to_merge)
                    if (i < can_merge.Length) can_merge[i] = true;
            }
            for (int i = 0; i < t.Rows.Count; i++)
            {
                TableRow tr = t.Rows[i];
                for (int j = 0, idx = 0; j < tr.Cells.Count; j++, idx++)
                {
                    if (!can_merge[idx]) continue;
                    TableCell td = tr.Cells[j];
                    string s = td.Text;

                    if (last_cells[idx] == null) last_cells[idx] = td;

                    if (s == last_vals[idx])
                    {
                        if (last_cells[idx].RowSpan == 0) last_cells[idx].RowSpan = 1;
                        last_cells[idx].RowSpan++;
                        tr.Cells.Remove(td);
                        j--;
                    }
                    else
                    {
                        last_vals[idx] = s;
                        last_cells[idx] = td;
                    }
                }
            }
        }
        public static Table GenListToTable(IList list)
        {
            return GenListToTable(list, null, null, null, false, null);
        }
        public static Table GenListToTable(IList list, string table_css, string header_css, string rows_css, bool merge_all_columns, params int[] merge_columns)
        {
            return GenListToTable(list, table_css, header_css, rows_css, false, null, merge_all_columns, merge_columns);
        }
        public static Table GenListToTable(IList list, string table_css, string header_css, string rows_css, bool keep_column, int[] exist_columns, bool merge_all_columns, params int[] merge_columns)
        {
            return GenListToTable(list, table_css, header_css, rows_css, false, null, false, null, merge_all_columns, merge_columns);
        }

        public static Table GenListToTable(IList list, string table_css, string header_css, string rows_css, bool chang_color, string rows_change_css, bool keep_column, int[] exist_columns, bool merge_all_columns, params int[] merge_columns)
        {
            int list_count = 0;
            Table ret = new Table();
            System.Reflection.PropertyInfo[] p = list[0].GetType().GetProperties();
            List<string> columns = new List<string>();
            foreach (System.Reflection.PropertyInfo pi in p)
            {
                if (exist_columns != null && exist_columns.Length != 0)
                {
                    if (keep_column)
                    {
                        if (Array.IndexOf(exist_columns, list_count) < 0)
                        {
                            list_count++;
                            continue;
                        }
                    }
                    else
                    {
                        if (Array.IndexOf(exist_columns, list_count) >= 0)
                        {
                            list_count++;
                            continue;
                        }
                    }
                }
                if (pi.Name.ToUpper().Equals("CONNINFO") || pi.Name.ToUpper().Equals("ITEM"))
                {
                    list_count++;
                    continue;
                }
                columns.Add(pi.Name);
                list_count++;
            }
            TableRow trHeader = null;
            trHeader = CommonLibrary.WebObject.TableHelper.BuiltTableRow(columns.ToArray());
            if (!string.IsNullOrEmpty(header_css))
                trHeader.CssClass = header_css;
            else
                trHeader.Attributes.Add("style", "border:1px solid #C1DAD7;padding: 3px 3px 3px 3px;font-weight: bold;");
            if (!string.IsNullOrEmpty(table_css))
            {
                ret.CssClass = table_css;
            }
            else
            {
                ret.Attributes.Add("style", "width:100%;font: normal 11px auto \"Trebuchet MS\", Verdana, Arial, Helvetica, sans-serif;font-size:11px;");
                //ret.Attributes.Add("bordercolor", "#dedfde");
                //ret.BorderWidth = 1;
                //ret.CellPadding = 3;
                //ret.CellSpacing = 0;
                //ret.Attributes.Add("rules", "cols");
            }
            ret.Rows.Add(trHeader);
            for (int i = 0; i < list.Count; i++)
            {
                List<string> values = new List<string>();
                list_count = 0;
                foreach (System.Reflection.PropertyInfo pi in p)
                {
                    if (exist_columns != null && exist_columns.Length != 0)
                    {
                        if (keep_column)
                        {
                            if (Array.IndexOf(exist_columns, list_count) < 0)
                            {
                                list_count++;
                                continue;
                            }
                        }
                        else
                        {
                            if (Array.IndexOf(exist_columns, list_count) >= 0)
                            {
                                list_count++;
                                continue;
                            }
                        }
                    }
                    if (pi.Name.ToUpper().Equals("CONNINFO") || pi.Name.ToUpper().Equals("ITEM"))
                    {
                        list_count++;
                        continue;
                    }
                    object oo = pi.GetValue(list[i], null);
                    if (oo != null)
                        values.Add(oo.ToString());
                    else
                        values.Add("");
                    list_count++;
                }
                if (string.IsNullOrEmpty(rows_css))
                    ret.Rows.Add(CommonLibrary.WebObject.TableHelper.BuiltTableRow(1, values.ToArray()));
                else
                {
                    if (chang_color && i % 2 == 1)
                        ret.Rows.Add(CommonLibrary.WebObject.TableHelper.BuiltTableRow(rows_change_css, values.ToArray()));
                    else
                        ret.Rows.Add(CommonLibrary.WebObject.TableHelper.BuiltTableRow(rows_css, values.ToArray()));
                }
            }
            if (merge_all_columns)
            {
                List<int> cols = new List<int>();
                for (int i = 0; i < ret.Rows[0].Cells.Count; i++)
                    cols.Add(i);
                MergeRows(ret, cols.ToArray());
            }
            return ret;
        }

        public static Table ColumnFormat(Table tb, int[] columns_indexs, string[] columns_format)
        {
            if (columns_format.Length != columns_indexs.Length) return tb;
            for (int i = 1; i < tb.Rows.Count; i++)
            {
                for (int cindex = 0; cindex < columns_indexs.Length; cindex++)
                {
                    if (columns_indexs[cindex] >= tb.Rows[i].Cells.Count) continue;
                    string scell = tb.Rows[i].Cells[columns_indexs[cindex]].Text;
                    if (!string.IsNullOrEmpty(scell))
                        tb.Rows[i].Cells[columns_indexs[cindex]].Text = string.Format(columns_format[cindex], scell);
                }
            }
            return tb;
        }

        public static Table ColumnInterchange(Table tb, int column_one, int column_two)
        {
            List<string> cells_one = new List<string>();
            List<string> cells_two = new List<string>();
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                cells_one.Add(tb.Rows[i].Cells[column_one].Text);
                cells_two.Add(tb.Rows[i].Cells[column_two].Text);
            }
            for (int i = 0; i < tb.Rows.Count; i++)
            {
                tb.Rows[i].Cells[column_one].Text = cells_two[i];
                tb.Rows[i].Cells[column_two].Text = cells_one[i];
            }
            return tb;
        }
    }
}
