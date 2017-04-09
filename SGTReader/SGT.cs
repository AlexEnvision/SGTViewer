using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArchiveSerialization;

namespace SGTReader
{
    public abstract class SGTProduce
    {
        public virtual void Serialize(Archive ar) { }
        public virtual List<string> GetAllColumnNames() { return new List<string>(); }
        public virtual List<string> GetRealColumnNames() { return new List<string>(); }
        public virtual List<UInt32[]> GetData() { return new List<UInt32[]>(); }
    }

    internal class SGT : SGTProduce
    {
        Handle handle;
        ExtHandle extHandle;                  // F001
        List<CutChannel> cutChannels;        
        MathChannelHandle mathChannelsHandle; // F003
        List<MathChannel> mathChannels;
        LabVagonType labVagonType;            // F005
        RecordCurrentPass recordCurrentPass;  // F006
        FrameParameters frameParameters;
        List<DataSection> data;

        private int CountChannelsOfCut 
        {
            get { return Convert.ToInt32(extHandle.GetSize() / 32.0); }
        }

        private int CountChannelsOfMath
        {
            get { return Convert.ToInt32(mathChannelsHandle.GetMathSize() / 24.0); }
        }

        private int CountFrameOfCut
        {
            get { return handle.GetCutCount(); }
        }

        public SGT()   
        {
            handle = new Handle();
            cutChannels = new List<CutChannel>();
            mathChannels = new List<MathChannel>();
            labVagonType = new LabVagonType();
            recordCurrentPass = new RecordCurrentPass();
            frameParameters = new FrameParameters();
            data = new List<DataSection>();
        }

        public override void Serialize(Archive ar)
        {
            if (ar.IsStoring())
            {
                // Создание заголовка (шапки) файла
                handle.Serialize(ar);
                extHandle.Serialize(ar);

                foreach (var cutChannel in cutChannels)
                {
                     cutChannel.Serialize(ar);
                }

                mathChannelsHandle.Serialize(ar);
                foreach (var mathChannel in mathChannels)
                {
                    mathChannel.Serialize(ar);
                }

                labVagonType.Serialize(ar);
                recordCurrentPass.Serialize(ar);
                frameParameters.Serialize(ar);

                foreach (var dataSection in data)
                {
                    dataSection.Serialize(ar);
                }
            }
            else
            {
                // Чтение шапки файла
                handle.Serialize(ar);
                extHandle.Serialize(ar);            

                var cutChannel = new CutChannel();
                cutChannels.Clear();
                cutChannels = new List<CutChannel>();
                for (int ii = 0; ii < CountChannelsOfCut; ii++)
                {
                    cutChannel.Serialize(ar);
                    cutChannels.Add(cutChannel);
                }

                mathChannelsHandle.Serialize(ar);

                var mathChannel = new MathChannel();
                mathChannels.Clear();
                mathChannels = new List<MathChannel>();
                for (int ii = 0; ii < CountChannelsOfMath; ii++)
                {                 
                    mathChannel.Serialize(ar);
                    mathChannels.Add(mathChannel);
                }

                labVagonType.Serialize(ar);
                recordCurrentPass.Serialize(ar);
                frameParameters.Serialize(ar);

                //byte[] DataCorrection;
                //ar.Read(out DataCorrection, 4);

                var dataSection = new DataSection();
                data.Clear();
                data = new List<DataSection>();
                for (int ii = 0; ii < CountFrameOfCut; ii++)
                {
                    dataSection.CreateSection(frameParameters.Names, frameParameters.Offsets);
                    dataSection.Serialize(ar);
                    data.Add(dataSection);
                }
            }
        }

        public override List<string> GetAllColumnNames()
        {
            List<string> _columns = new List<string>();
            foreach (var _cutChannel in cutChannels)
            {
                _columns.Add(_cutChannel.GetName());
            }
            return _columns;
        }

        public override List<string> GetRealColumnNames()
        {
            List<string> _columns = new List<string>();
            foreach (var _name in frameParameters.Names)
            {
                _columns.Add(_name);
            }
            return _columns;
        }

        public override List<UInt32[]> GetData()
        {
            List<UInt32[]> data = new List<UInt32[]>();
            foreach (var _data in this.data)
            {           
                data.Add(_data.GetValues());
            }
            return data;            
        }
    }

    //----------------------------------------------- Заголовки SGT файла ----------------------------------------------------

    internal struct Handle
    {
        ushort ID;                         // код типа файла
        UInt32 CutCount;                   // общее количество измерительных срезов в файле
        UInt16 HeaderSize;                 // размер начального заголовка
        UInt16 ListSize;                   // размер списка дополнительных заголовков
        UInt32 Engine;                     // номер путеизмерительного средства
        byte[] Date;                     // дата получения файла (формат даты описан ниже)
        float Period;                      // период между срезами измерений
        UInt16 MeasMode;                   // режим измерений: 0-по пути (период в метрах),1-по времени (период в секундах). Другие значения на данный момент не определены
        double Begin;                      // Координата начала записи сигнала в файла
        double dPeriod;                    // Период между срезами измерений в формате с повышенной точностью. Если это поле равно 0, необходимо использовать поле Period (файл старого формата)

        public void Serialize(Archive ar)
        {

            if (ar.IsStoring())
            {
                // Создание заголовка (шапки) 
                ar.Write(ID);
                ar.Write(CutCount);
                ar.Write(ListSize);
                ar.Write(Date);
                ar.Write(Period);
                ar.Write(MeasMode);
                ar.Write(Begin);
                ar.Write(dPeriod);
            }
            else
            {
                ushort id;
                UInt32 cutCount;
                UInt16 headerSize;
                UInt16 listSize;
                UInt16 engine;
                byte[] date;
                float period;
                UInt16 measMode;
                double begin;
                double dperiod;
                // Чтение шапки файла
                ar.Read(out id);
                //Проверка сигнатуры файла в заголовке
                if (id != 0x6A7F)
                    throw new Exception("Данный формат НЕ является файлом сигналов!");
                ar.Read(out cutCount);
                ar.Read(out headerSize);
                ar.Read(out listSize);
                ar.Read(out engine);
                ar.Read(out date, 4);
                ar.Read(out period);
                ar.Read(out measMode);
                ar.Read(out begin);
                ar.Read(out dperiod);

                this.ID = id;
                this.CutCount = cutCount;  // two-step storage is needed since m_etaLearningRate is "volatile"               
                this.HeaderSize = headerSize;
                this.Engine = engine;
                this.Date = date;
                this.Period = period;
                this.MeasMode = measMode;
                this.Begin = begin;
                this.dPeriod = dperiod;
            }
        }

        public int GetCutCount()
        {
            return (int)CutCount;
        }
    }

    internal struct ExtHandle
    {
        UInt16 ID;    //идентификатор дополнительного заголовка 
        UInt16 Size;  //размер заголовка (без учета размера постоянной шапки)

        public void Serialize(Archive ar)
        {
            if (ar.IsStoring())
            {
                ar.Write(ID);
                ar.Write(Size);
            }
            else
            {
                UInt16 id;
                UInt16 size;

                ar.Read(out id);
                ar.Read(out size);

                this.ID = id;
                this.Size = size;
            }
        }

        public int GetSize()
        {
            return Size;
        }
    }

    internal struct MathChannelHandle
    {
        UInt16 Separator;
        UInt16 SectionID;
        UInt16 MathSize;

        public void Serialize(Archive ar)
        {
            if (ar.IsStoring())
            {
                ar.Write(Separator);
                ar.Write(SectionID);
                ar.Write(MathSize);
            }
            else
            {
                UInt16 separator;
                UInt16 id;
                UInt16 size;

                ar.Read(out separator);
                ar.Read(out id);
                ar.Read(out size);

                this.Separator = separator;
                this.SectionID = id;
                this.MathSize = size;
            }
        }

        public int GetMathSize()
        {
            return MathSize;
        }
    }

    //___________________________________________________Описание структуры срезов_______________________________________________________

    internal struct CutChannel
    {
        UInt16 Code;          //не используется
        string Name;          //идентификатор параметра
        UInt16 Count;         //для типа бит размер поля в битах, для типа битовое поле размер поля в байтах, для всех остальных типов количество элементов данного типа в одном канале
        UInt16 Type;          //формат представления канала (тип данных) 
        UInt16 Scale;         //коэффициент приведения в систему единиц SI задается только для целочисленных параметров, если равен 0 коэффициент неопределен (например для основных параметров путеизмерителя равен 1e-4)
        byte[] Reserved;      //зарезервировано, должно равняться нулю

        public void Serialize(Archive ar)
        {
            if (ar.IsStoring())
            {
                ar.Write(Code);
                ar.Write(Name);
                ar.Write(Count);
                ar.Write(Type);
                ar.Write(Scale);
                ar.Write(Reserved);      
            }
            else
            {
                UInt16 code;
                string name; 
                UInt16 count;
                UInt16 type;
                UInt16 scale;
                byte[] reserved = new byte[8];

                ar.Read(out code);
                ar.Read(out name, 16);
                ar.Read(out count);
                ar.Read(out type);
                ar.Read(out scale);
                ar.Read(out reserved, 8);

                this.Code = code;
                this.Name = name.Split('\0')[0];
                this.Count = count;
                this.Type = type;
                this.Scale = scale;
                this.Reserved = reserved;
            }
        }

        public string GetName()
        {
            return this.Name;
        }
    }

    //+++++++++++++++++++++++++++++   Описание записи констант математической модели измерений   ++++++++++++++++++++++++++++++++

    internal struct MathChannel
    {
        double Value;        //Значение константы 
        string Name;	     //Имя константы

        public void Serialize(Archive ar)
        {
            if (ar.IsStoring())
            {
                ar.Write(Value);
                ar.Write(Name);   
            }
            else
            {
                double value;
                string name;

                ar.Read(out value);
                ar.Read(out name, 16);

                this.Value = value;
                this.Name = name.Split('\0')[0];
            }
        }
    }

    /// <summary>
    /// Считывает тип вагона F005
    /// </summary>
    internal struct LabVagonType
    {
        UInt16 Separator;    //Разделитель секций в файле
        string Model;	     //Имя константы

        public void Serialize(Archive ar)
        {
            if (ar.IsStoring())
            {
                ar.Write(Separator);
                ar.Write(Model);
            }
            else
            {
                UInt16 separator;
                string model;

                ar.Read(out separator);
                ar.Read(out model, 42);

                this.Separator = separator;
                this.Model = model.Split('\0')[0];
            }
        }
    }

    /// <summary>
    /// Запись текущего проезда F006
    /// </summary>
    internal struct RecordCurrentPass
    {
        UInt16 Separator;     //Разделитель секций в файле
        string UnusageSpace;         
        string ModelID;       //Внутренний идентификатор модели вагона (например: M_P2_1)
        string HardwareID;    //Внутренний идентификатор исполнения вагона (например: P2_1Acl)
        UInt32 FileCode;      //Код файла сигналов 0x6a7f
        byte[] Dt;            //Дата время проезда                                                !!!!!!!!!!!!!!!!!!!! Писать надо спецструктуры
        UInt16 LapNum;        //Номер проезда
        string RailWayCode;   //Идентификатор железной дороги
        byte[] Voayge;        //Информация об отчетном периоде                                    !!!!!!!!!!!!!!!!!!!!
        byte[] GUID;          // Guid проезда                                                     !!!!!!!!!!!!!!!!!!!!
        byte[] Version;       // Версия ПО, создавшего файл                                       !!!!!!!!!!!!!!!!!!!!
        UInt16 Specification; // Код варианта расшифровки
        byte[] Reserved;      //зарезервировано, должно равняться нулю

        public void Serialize(Archive ar)
        {
            if (ar.IsStoring())
            {
                ar.Write(Separator);
                ar.Write(UnusageSpace);
                ar.Write(ModelID);
                ar.Write(HardwareID);
                ar.Write(FileCode);
                ar.Write(Dt);
                ar.Write(LapNum);
                ar.Write(RailWayCode);
                ar.Write(Voayge);
                ar.Write(GUID);
                ar.Write(Version);
                ar.Write(Specification);
                ar.Write(Reserved);
            }
            else
            {
                UInt16 separator;
                string unusageSpace;
                string modelID;
                string hardwareID;
                UInt32 fileCode;
                byte[] dt = new byte[7];
                UInt16 lapNum;
                string railWayCode;
                byte[] voyage = new byte[8];
                byte[] guid = new byte[16];
                byte[] version = new byte[32];
                UInt16 specification;
                byte[] reserved = new byte[32];

                ar.Read(out separator);
                ar.Read(out unusageSpace, 22);
                ar.Read(out modelID, 20);
                ar.Read(out hardwareID, 20);
                ar.Read(out fileCode);
                ar.Read(out dt, 7);
                ar.Read(out lapNum);
                ar.Read(out railWayCode, 1);
                ar.Read(out voyage, 8);
                ar.Read(out guid, 16);
                ar.Read(out version, 32);
                ar.Read(out specification);
                ar.Read(out reserved, 32);

                this.Separator = separator;
                this.UnusageSpace = unusageSpace.Split('\0')[0];
                this.ModelID = modelID.Split('\0')[0];
                this.HardwareID = hardwareID.Split('\0')[0];
                this.FileCode = fileCode;
                this.Dt = dt;
                this.LapNum = lapNum;
                this.RailWayCode = railWayCode;
                this.Voayge = voyage;
                this.GUID = guid;
                this.Version = version;
                this.Specification = specification;
                this.Reserved = reserved;
            }
        }
    }

    /// <summary>
    /// Параметры кадра F008
    /// </summary>
    internal struct FrameParameters
    {
        UInt16 Separator;         //Разделитель секций в файле
        UInt16 ParametersSize;    //Размер блока параметров в байтах
        string ProgamName;        //Тип программы записывающей файл
        List<string> Parameters;  //Строки параметров кадра

        public string[] Names;
        public int[] Offsets;

        private void Parsing(List<string> Parameters)
        {
            int shiftparam = 4;
            Names = new string[Parameters.Count - shiftparam - 1];
            Offsets = new int[Parameters.Count - shiftparam - 1];
            for (int i = 0; i < Names.Length; i++)
            {
                Names[i] = Parameters[i + shiftparam].Split('=')[0];
                if (i != Names.Length - 1)
                    Offsets[i] = CalcOffset(Parameters[i + shiftparam], Parameters[i + shiftparam + 1]);
                else 
                {
                    Offsets[i] = CalcLastOffset(Parameters[i + shiftparam], Parameters[1]);
                }
            }
        }

        private int CalcOffset(string a, string b)
        {
            int first = Convert.ToInt32(a.Split('=')[1].Split(':')[0]);
            int second = Convert.ToInt32(b.Split('=')[1].Split(':')[0]);
            return second - first;
        }

        private int CalcLastOffset(string prevLast, string framelength)
        {
            int first = Convert.ToInt32(prevLast.Split('=')[1].Split(':')[0]);
            int second = Convert.ToInt32(framelength.Split('=')[1]);
            return second - first;
        }

        public void Serialize(Archive ar)
        {
            if (ar.IsStoring())
            {
                ar.Write(Separator);
                ar.Write(ParametersSize);
                ar.Write(ProgamName);
            }
            else
            {
                UInt16 separator;
                UInt16 parametersSize;
                string programName;
                List<string> parameters;               

                ar.Read(out separator);
                ar.Read(out parametersSize);
                ar.Read(out programName, 16);

                string parameter;
                parameters = new List<string>();
                do
                {
                    ar.Read(out parameter, char.MinValue);
                    if (parameter.Length > 1) parameters.Add(parameter.Split('\0')[0]);
                }
                while (parameter.Length > 1);             

                this.Separator = separator;
                this.ParametersSize = parametersSize;
                this.ProgamName = programName.Split('\0')[0];
                this.Parameters = parameters;

                Parsing(Parameters);
            }
        }      
    }

    /// <summary>
    /// Секция данных
    /// </summary>
    internal struct DataSection
    {
        string[] Names;
        int[] Offsets;
        UInt32[] Values;

        public void CreateSection(string[] Names, int[] offsets)
        {
            this.Names = Names;
            this.Offsets = offsets;
            this.Values = new UInt32[Names.Length];
        }

        public void Serialize(Archive ar)
        {
            if (ar.IsStoring())
            {            
                //foreach (var _value in Values)
                //   ar.Write(_value);
            }
            else
            {
                object[] objects = new object[Values.Length];
                UInt32[] values = new UInt32[Values.Length];

                byte[] _temp;
                for (int i = 0; i < Offsets.Length; i++)
                {
                    ar.Read(out _temp, Offsets[i]);
                    switch (Offsets[i])
                    {
                        case 1: byte[] twobyte = new byte[2];
                                twobyte[0] = _temp[0];
                                objects[i] = BitConverter.ToUInt16(twobyte, 0); 
                            break;
                        case 2: objects[i] = BitConverter.ToUInt16(_temp, 0);
                            break;
                        case 4: objects[i] = BitConverter.ToUInt32(_temp, 0);
                            break;
                    }
                    
                }

                for (int i = 0; i < Offsets.Length; i++)
                {
                    values[i] = Convert.ToUInt32(objects[i]);
                }

                this.Values = values;
            }
        }

        public UInt32[] GetValues()
        {
            return Values;
        }
    }
}
