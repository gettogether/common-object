using System;
using System.Reflection.Emit;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Utility.Dynamic
{
    public class DynamicComparer<T> : IComparer<T>
    {
        #region Private Fields
        protected DynamicMethod method;
        protected CompareMethodInvoker comparer;
        #endregion
        #region Private Delegates
        protected delegate int CompareMethodInvoker(T x, T y);
        #endregion
        #region Constructors
        public DynamicComparer(string orderBy)
        {
            Initialize(orderBy);
        }
        public DynamicComparer(string orderBy, bool asc)
        {
            if (!asc)
                orderBy += " DESC";
            Initialize(orderBy);
        }
        public DynamicComparer(Dictionary<string, bool> sortBy)
        {
            StringBuilder orderBy = new StringBuilder();
            foreach (KeyValuePair<string, bool> sort in sortBy)
            {
                orderBy.Append(string.Concat(sort.Key, sort.Value ? "" : " DESC", ","));
            }
            Initialize(ParseOrderBy(orderBy.ToString().TrimEnd(',')));
        }
        public DynamicComparer(SortProperty[] sortProperties)
        {
            Initialize(sortProperties);
        }
        #endregion
        #region Public Methods
        public void Initialize(string orderBy)
        {
            Initialize(ParseOrderBy(orderBy));
        }
        public void Initialize(SortProperty[] sortProperties)
        {
            CheckSortProperties(sortProperties);
            method = CreateDynamicCompareMethod(sortProperties);
            comparer = (CompareMethodInvoker)method.CreateDelegate(typeof(CompareMethodInvoker));
        }
        public int Compare(T x, T y)
        {
            if (x == null && y == null)
                return 0;
            else if (x == null)
                return -1;
            else if (y == null)
                return 1;
            return comparer.Invoke(x, y);
        }
        #endregion
        #region Private Methods
        protected SortProperty[] ParseOrderBy(string orderBy)
        {
            if (orderBy == null || orderBy.Trim().Length == 0)
                throw new ArgumentException("The \"order by\" string must not be null or empty.");

            string[] props = orderBy.Split(',');
            SortProperty[] sortProps = new SortProperty[props.Length];
            string prop;
            bool descending;

            for (int i = 0; i < props.Length; i++)
            {
                descending = false;
                prop = props[i].Trim();

                if (prop.ToUpper().EndsWith(" DESC"))
                {
                    descending = true;
                    prop = prop.Substring(0, prop.ToUpper().LastIndexOf(" DESC"));
                }
                else if (prop.ToUpper().EndsWith(" ASC"))
                {
                    prop = prop.Substring(0, prop.ToUpper().LastIndexOf(" ASC"));
                }

                prop = prop.Trim();

                sortProps[i] = new SortProperty(prop, descending);
            }

            return sortProps;
        }

        protected DynamicMethod CreateDynamicCompareMethod(SortProperty[] sortProperties)
        {
            DynamicMethod dm = new DynamicMethod("DynamicComparison", typeof(int), new Type[] { typeof(T), typeof(T) }, typeof(DynamicComparer<T>));

            #region Generate IL for dynamic method
            ILGenerator ilGen = dm.GetILGenerator();

            Label lbl = ilGen.DefineLabel();
            ilGen.DeclareLocal(typeof(int));
            Dictionary<Type, LocalBuilder> localVariables = new Dictionary<Type, LocalBuilder>();

            ilGen.Emit(OpCodes.Ldc_I4_0);
            ilGen.Emit(OpCodes.Stloc_0);

            string propertyName;
            PropertyInfo propertyInfo;
            foreach (SortProperty property in sortProperties)
            {
                propertyName = property.Name;
                propertyInfo = typeof(T).GetProperty(propertyName);

                ilGen.Emit(OpCodes.Ldloc_0);
                ilGen.Emit(OpCodes.Brtrue_S, lbl);
                ilGen.Emit(OpCodes.Ldarg_0);
                ilGen.EmitCall(OpCodes.Callvirt, propertyInfo.GetGetMethod(), null);

                if (propertyInfo.PropertyType.IsValueType)
                {
                    if (!localVariables.ContainsKey(propertyInfo.PropertyType))
                        localVariables.Add(propertyInfo.PropertyType, ilGen.DeclareLocal(propertyInfo.PropertyType));

                    int localIndex = localVariables[propertyInfo.PropertyType].LocalIndex;

                    ilGen.Emit(OpCodes.Stloc, localIndex);
                    ilGen.Emit(OpCodes.Ldloca_S, localIndex);
                }

                ilGen.Emit(OpCodes.Ldarg_1);
                ilGen.EmitCall(OpCodes.Callvirt, propertyInfo.GetGetMethod(), null);
                ilGen.EmitCall(OpCodes.Callvirt, propertyInfo.PropertyType.GetMethod("CompareTo", new Type[] { propertyInfo.PropertyType }), null);

                if (property.Descending)
                    ilGen.Emit(OpCodes.Neg);

                ilGen.Emit(OpCodes.Stloc_0);
            }

            ilGen.MarkLabel(lbl);
            ilGen.Emit(OpCodes.Ldloc_0);
            ilGen.Emit(OpCodes.Ret);
            #endregion

            return dm;
        }

        protected void CheckSortProperties(SortProperty[] sortProperties)
        {
            if (sortProperties == null)
                sortProperties = new SortProperty[0];

            Type instanceType = typeof(T);
            //if (!instanceType.IsPublic)
            //    throw new ArgumentException(string.Format("Type \"{0}\" is not public.", typeof(T).FullName));

            foreach (SortProperty sProp in sortProperties)
            {
                PropertyInfo pInfo = instanceType.GetProperty(sProp.Name);

                if (pInfo == null)
                    throw new ArgumentException(string.Format("No public property named \"{0}\" was found.", sProp.Name));

                if (!pInfo.CanRead)
                    throw new ArgumentException(string.Format("The property \"{0}\" is write-only.", sProp.Name));

                if (!typeof(IComparable).IsAssignableFrom(pInfo.PropertyType))
                    throw new ArgumentException(string.Format("The type \"{1}\" of the property \"{0}\" does not implement IComparable.", sProp.Name, pInfo.PropertyType.FullName));
            }
        }

        #endregion

    }
}