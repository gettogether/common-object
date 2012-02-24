using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace CommonLibrary.WebObject
{
    public class DataGridHelper
    {
        public static void CombinColumns(DataGrid dataGrid)
        {
            int rowsCount = dataGrid.Items.Count;
            for (int i = rowsCount - 1; i > 0; i--)
            {
                int colCount = dataGrid.Items[0].Cells.Count;

                for (int j = colCount - 1; j >= 0; j--)
                {
                    int span = dataGrid.Items[i].Cells[j].RowSpan;
                    if (String.Equals(dataGrid.Items[i - 1].Cells[j].Text, dataGrid.Items[i].Cells[j].Text))
                    {
                        span = (span == 0 ? 1 : span);
                        span += 1;
                        dataGrid.Items[i - 1].Cells[j].RowSpan = span;
                        dataGrid.Items[i].Cells[j].Visible = false;

                    }
                }
            }
        }
        public static void SpanRow(DataGrid dg, int GroupColumn, int compareColumn)
        {
            int i = 0;
            int j = 0;
            int rowSpan;
            string strTemp = "";

            for (i = 0; i < dg.Items.Count; i++)
            {
                rowSpan = 1;
                strTemp = dg.Items[i].Cells[compareColumn].Text;
                for (j = i + 1; j < dg.Items.Count; j++)
                {
                    if (string.Compare(strTemp, dg.Items[j].Cells[compareColumn].Text) == 0)
                    {
                        rowSpan += 1;
                        dg.Items[i].Cells[GroupColumn].RowSpan = rowSpan;
                        dg.Items[j].Cells[GroupColumn].Visible = false;
                    }
                    else
                    {
                        break;
                    }
                }
                i = j - 1;
            }
        }
    }
}
