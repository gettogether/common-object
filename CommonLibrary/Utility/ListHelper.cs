using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using CommonLibrary.Utility.Dynamic;
using System.Collections;

namespace CommonLibrary.Utility
{
    public class ListHelper
    {
        public class Sorter
        {
            public delegate int Compare<T>(T value1, T value2);

            public static void DimidiateSort<T>(List<T> myList, Compare<T> myCompareMethod)
            {
                DimidiateSort<T>(myList, 0, myList.Count - 1, myCompareMethod);
            }
            public static void DimidiateSort<T>(List<T> myList, int left, int right, Compare<T> myCompareMethod)
            {
                if (left < right)
                {
                    T s = myList[(right + left) / 2];
                    int i = left - 1;
                    int j = right + 1;
                    T temp = default(T);
                    while (true)
                    {
                        do
                        {
                            i++;
                        }
                        while (i < right && myCompareMethod(myList[i], s) == -1);
                        do
                        {
                            j--;
                        }
                        while (j > left && myCompareMethod(myList[j], s) == 1);
                        if (i >= j)
                            break;
                        temp = myList[i];
                        myList[i] = myList[j];
                        myList[j] = temp;
                    }
                    DimidiateSort(myList, left, i - 1, myCompareMethod);
                    DimidiateSort(myList, j + 1, right, myCompareMethod);
                }
            }
        }

        interface ISort<T> where T : IComparable<T>
        {
            void Sort(T[] items);
        }

        public class Sort<T>
        {
            public static void Swap(T[] items, int left, int right)
            {
                T temp = items[right];
                items[right] = items[left];
                items[left] = temp;
            }
        }

        public static LTo ConvertListType<LFrom, From, LTo, To>(LFrom l)
            where LTo : IList<To>, new()
            where LFrom : IList<From>, new()
        {
            LTo ret = new LTo();
            foreach (From f in l)
            {
                To t = (To)Convert.ChangeType(f, typeof(To));
                ret.Add(t);
            }
            return ret;
        }

        public static List<To> ConvertListType<From, To>(List<From> f)
        {
            return ConvertListType<List<From>, From, List<To>, To>(f);
        }

        public static DataTable ListToDataTable(IList ResList)
        {
            DataTable ret = new DataTable();
            if (ResList == null || ResList.Count == 0) return ret;
            System.Reflection.PropertyInfo[] p = ResList[0].GetType().GetProperties();
            List<System.Reflection.PropertyInfo> trim = new List<System.Reflection.PropertyInfo>();
            foreach (System.Reflection.PropertyInfo pi in p)
            {
                if (pi.Name.ToUpper().Equals("CONNINFO") || pi.Name.ToUpper().Equals("ITEM"))
                {
                    continue;
                }
                trim.Add(pi);
                ret.Columns.Add(pi.Name, System.Type.GetType(pi.PropertyType.ToString()));
            }
            p = trim.ToArray();
            for (int i = 0; i < ResList.Count; i++)
            {
                IList TempList = new ArrayList();
                foreach (System.Reflection.PropertyInfo pi in p)
                {
                    object oo = pi.GetValue(ResList[i], null);
                    TempList.Add(oo);
                }

                object[] itm = new object[p.Length];
                for (int j = 0; j < TempList.Count; j++)
                {
                    itm.SetValue(TempList[j], j);
                }
                ret.LoadDataRow(itm, true);
            }
            return ret;
        }
    }
}
