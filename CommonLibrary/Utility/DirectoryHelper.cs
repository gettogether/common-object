using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CommonLibrary.Utility
{
    public class DirectoryHelper
    {
        public struct CopyParameter
        {
            public string Destination;

            public string Source;

            public bool IsOverwrite;

            public string[] IgnoreFolders;
        }

        public static void AsyncCopy(CopyParameter cp)
        {
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(ThreadCopy));
            t.Start(cp);
        }

        private static void ThreadCopy(object cp)
        {
            Copy((CopyParameter)cp);
        }

        public static void Copy(string sourcePath, string destPath, bool isOverwrite, bool isAsync, params string[] ignoreFolders)
        {
            CopyParameter cp = new CopyParameter();
            cp.Source = sourcePath;
            cp.Destination = destPath;
            cp.IsOverwrite = isOverwrite;
            cp.IgnoreFolders = ignoreFolders;
            if (isAsync)
                AsyncCopy(cp);
            else
                Copy(cp);
        }

        public static void Copy(CopyParameter cp)
        {
            CopyParameter Info = cp;
            if (!Directory.Exists(Info.Source))
            {
                throw new FileNotFoundException();
            }
            if (!Directory.Exists(Info.Destination))
            {
                Directory.CreateDirectory(Info.Destination);
            }
            string[] DirFiles;
            string[] DirDirs;
            try
            {
                DirFiles = Directory.GetFiles(Info.Source);
                DirDirs = Directory.GetDirectories(Info.Source);
            }
            catch { throw new FileNotFoundException(); }
            foreach (string SingleDir in DirDirs)
            {
                bool isIgnore = false;
                if (cp.IgnoreFolders != null)
                {
                    foreach (string iFolder in cp.IgnoreFolders)
                    {
                        if (SingleDir.ToLower().IndexOf(string.Concat("\\", iFolder.ToLower())) > 0) { isIgnore = true; break; }
                    }
                }
                if (isIgnore) continue;
                string DirName = "\\";
                DirName = string.Concat(DirName, SingleDir.Split('\\')[SingleDir.Split('\\').Length - 1]);
                CopyParameter NextInfo = new CopyParameter();
                NextInfo.Destination = string.Concat(Info.Destination, DirName);
                NextInfo.Source = SingleDir;
                NextInfo.IgnoreFolders = cp.IgnoreFolders;
                NextInfo.IsOverwrite = cp.IsOverwrite;
                Copy(NextInfo);
            }
            foreach (string SingleFile in DirFiles)
            {
                try
                {
                    string FileName = SingleFile.Split('\\')[SingleFile.Split('\\').Length - 1];
                    string destFileName = string.Concat(Info.Destination, "\\", FileName);
                    if (!Info.IsOverwrite && File.Exists(destFileName)) continue;
                    File.Copy(SingleFile, destFileName, Info.IsOverwrite);
                }
                catch (Exception ex) { LogHelper.Write(LogHelper.LogTypes.Error, ex.ToString()); }
            }
        }

        public static void Copy(string sourcePath, string dstPath,bool isCopySubDir)
        {
            DirectoryInfo dSource = new DirectoryInfo(sourcePath);
            DirectoryInfo dDst = Directory.CreateDirectory(dstPath);
            foreach (FileInfo f in dSource.GetFiles())
            {
                f.CopyTo(dDst.FullName + "\\" + f.Name);
            }
            if (!isCopySubDir)
                return;
            else
            {
                foreach (DirectoryInfo d in dSource.GetDirectories())
                {
                    Copy(d.FullName, dstPath + d.FullName.Replace(sourcePath, ""), isCopySubDir);
                }
            }
        }
    }
}
