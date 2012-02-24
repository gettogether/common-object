using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.ObjectBase
{
    public class MySqlDataBaseList<T> : ListBase<T>
        where T : class, new()
    {
    }
}
