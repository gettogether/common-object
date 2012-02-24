using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.ObjectBase
{
    public class ConfigBase<T> where T : class, new()
    {
        public ConfigBase()
        {

        }

        public virtual void ReadSetting()
        {

        }

        public virtual void InitSetting()
        {

        }

        public virtual void SaveSetting()
        {

        }
    }
}
