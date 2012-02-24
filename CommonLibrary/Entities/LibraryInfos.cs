using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLibrary.Entities
{
    public class LibraryInfos : CommonLibrary.ObjectBase.ListBase<LibraryInfos.LibraryInfo>
    {
        public LibraryInfos()
        {

        }

        public LibraryInfos(string path, string searchParttern)
        {
            string[] files = System.IO.Directory.GetFiles(path, searchParttern);
            foreach (string f in files)
            {
                System.IO.FileInfo fi = new System.IO.FileInfo(f);
                System.Diagnostics.FileVersionInfo vi = System.Diagnostics.FileVersionInfo.GetVersionInfo(f);
                this.Add(new LibraryInfos.LibraryInfo(fi.Name, fi.CreationTime, fi.LastWriteTime, vi.FileVersion));
            }
        }

        public override string ToString()
        {
            System.Web.UI.WebControls.Table tb = this.ToTable(false);
            if (tb != null)
            {
                tb.Rows[0].Cells[0].Text = "File Name";
                tb.Rows[0].Cells[1].Text = "Create Time";
                tb.Rows[0].Cells[2].Text = "Last Update Time";
                tb.Rows[0].Cells[3].Text = "Version";
                tb.Style.Add("style", "font-size:10pt;font-family: \"ËÎÌו\", \"Tahoma\", \"Geneva\", sans-serif;");
                return WebObject.ControlHelper.ControlToHtml(tb);
            }
            else
            {
                return base.ToString();
            }
        }

        public class LibraryInfo
        {
            private string _FileName;

            public string FileName
            {
                get { return _FileName; }
                set { _FileName = value; }
            }

            private DateTime _CreateOn;

            public DateTime CreateOn
            {
                get { return _CreateOn; }
                set { _CreateOn = value; }
            }

            private DateTime _UpdateOn;

            public DateTime UpdateOn
            {
                get { return _UpdateOn; }
                set { _UpdateOn = value; }
            }

            private string _Version;

            public string Version
            {
                get { return _Version; }
                set { _Version = value; }
            }

            public LibraryInfo()
            {

            }
            public LibraryInfo(string fileName, DateTime createOn, DateTime updateOn, string version)
            {
                this.FileName = fileName;
                this.CreateOn = createOn;
                this.UpdateOn = updateOn;
                this.Version = version;
            }
        }
    }
}
