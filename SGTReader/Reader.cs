using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ArchiveSerialization;
using System.Windows.Forms;

namespace SGTReader
{
    public abstract class ReaderFactory 
    {
        public abstract SGTProduce CreateSGT();
        public abstract SGTProduce CreateSGT(string SGTPath);
    }

    internal class Reader : ReaderFactory 
    {
        public override SGTProduce CreateSGT()
        {
            return new SGT();
        }
        public override SGTProduce CreateSGT(string SGTPath)
        {
            SGT sgt = new SGT();
            OpenFile(sgt, SGTPath);
            return sgt;
        }

        private void OpenFile(SGTProduce sgt, string SGTPath)
        {
            try
            {
                string filename = SGTPath; 
                using (FileStream fileStream = File.OpenRead(filename))
                {
                    ArchiveSerialization.Archive Ar = new ArchiveSerialization.Archive(fileStream, ArchiveSerialization.ArchiveOp.load);
                    sgt.Serialize(Ar);
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("При загрузке файла произошла ошибка:" + "\n" + Ex.Message);
            }
        }
    }
}
