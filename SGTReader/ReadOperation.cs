using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGTReader
{
    /// <summary>
    /// Класс-клиент абстрактной фабрики
    /// </summary>
    public class ReadOperation
	{
	    private SGTProduce sgt;
 
        // Конструктор
	    public ReadOperation(string FileDir)
	    {
            ReaderFactory reader = new Reader();
            sgt = reader.CreateSGT(FileDir);
        }

        public List<string> GetAllColumns()
        {
            return sgt.GetAllColumnNames();
        }
        public List<string> GetDataColums()
	    {
            return sgt.GetRealColumnNames();
	    }
        public List<UInt32[]> GetData()
        {
            return sgt.GetData();
        }

	}
}
