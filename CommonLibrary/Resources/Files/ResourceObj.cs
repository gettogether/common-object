using System;
using System.Collections.Generic;
using System.Text;
using CommonLibrary.Utility;

namespace CommonLibrary.Resources.Files
{
    public class ResourceObj
    {
        private string _en_us = "";
        public string en_us
        {
            get
            {
                return _en_us;
            }
            set
            {
                _en_us = value;
            }
        }

        private string _zh_cn = "";
        public string zh_cn
        {
            get
            {
                return _zh_cn;
            }
            set
            {
                _zh_cn = value;
            }
        }

        private string _zh_tw = "";
        public string zh_tw
        {
            get
            {
                return _zh_tw;
            }
            set
            {
                _zh_tw = value;
            }
        }

        public object Key;

        public string Value
        {
            get
            {
                switch (MutiLanguage.GetCultureType())
                {
                    case MutiLanguage.Languages.en_us:
                        return _en_us;
                    case MutiLanguage.Languages.zh_cn:
                        if (string.IsNullOrEmpty(_zh_cn))
                            return _en_us;
                        else
                            return _zh_cn;
                    case MutiLanguage.Languages.zh_tw:
                        if (!string.IsNullOrEmpty(_zh_tw))
                            return _zh_tw;
                        else if (!string.IsNullOrEmpty(_zh_tw))
                            return _zh_tw;
                        else
                            return _en_us;
                    default:
                        return _en_us;
                }
            }
        }

        public ResourceObj()
        {

        }


        public ResourceObj(string en_us, string zh_cn, string zh_tw)
        {
            _en_us = en_us;
            _zh_cn = zh_cn;
            _zh_tw = zh_tw;
        }

        public ResourceObj(object rn)
        {
            this.Key = rn;
        }
    }
}
