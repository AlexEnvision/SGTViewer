using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SGTReader;
using ZedGraph;
using SourceGrid;
using GraphLib;

namespace SGTViewer
{
    public partial class MainForm : Form
    {
        ReadOperation ReadSGT;
        List<UInt32[]> Data;
        Size OldSize;

        private String CurExample = "TILED_VERTICAL_AUTO";
        private PrecisionTimer.Timer mTimer = null;
        private DateTime lastTimerTick = DateTime.Now;
        public MainForm()
        {
            InitializeComponent();
            OldSize = this.Size;

            mTimer = new PrecisionTimer.Timer();
            mTimer.Period = 40;                         // 20 fps
            mTimer.Tick += new EventHandler(OnTimerTick);
            lastTimerTick = DateTime.Now;
            mTimer.Start();       
        }

        private void OnTimerTick(object sender, EventArgs e)
        {
            if (CurExample == "ANIMATED_AUTO")
            {
                try
                {
                    TimeSpan dt = DateTime.Now - lastTimerTick;
                }
                catch (ObjectDisposedException ex)
                {
                    // we get this on closing of form
                }
                catch (Exception ex)
                {
                    Console.Write("exception invoking refreshgraph(): " + ex.Message);
                }


            }
        }

        private void openMenu_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(0);
        }
        private void btGridPanelSwitch_Click(object sender, EventArgs e)
        {
            tabControl1.SelectTab(1);
        }

        private void openSgtMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Signal Files(*.sgt)|*.sgt";
                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ReadSGT = new ReadOperation(openFileDialog.FileName);
                    List<string> ColumnNames = ReadSGT.GetDataColums();
                    List<UInt32[]> Data = ReadSGT.GetData();
                    this.Data = Data;

                    gridSgtFile.Rows.Clear();
                    gridSgtFile.Columns.Clear();

                    dgvSgtFile.Rows.Clear();
                    dgvSgtFile.Columns.Clear();

                    foreach (var _column in ColumnNames)
                    {
                        DataGridViewColumn dgvZedGraphColumn = new DataGridViewZedGraphColumn();
                        dgvZedGraphColumn.HeaderText = _column;
                        dgvSgtFile.Columns.Add(dgvZedGraphColumn);
  
                    }

                    dgvSgtFile.Rows.Add();
                    dgvSgtFile.Rows[0].Height = dgvSgtFile.Height - 30;

                    for (int i = 0; i < Data[0].Length; i++)
                    {
                        dgvSgtFile.Rows[0].Cells[i].Value = GetSignal(Data, i);
                    }
                                     
                    FillGrid(this.gridSgtFile, Data, ColumnNames);
                }
            }
        }

        private UInt32[] GetSignal(List<UInt32[]> Data, int sigNum)
        {
            UInt32[] Sig = new UInt32[Data.Count];
            for (int j = 0; j < Data.Count; j++)
            {
                Sig[j] = Data[j][sigNum];
            }
            return Sig;
        }

        private void FillGrid(Grid grid, List<UInt32[]> Data, List<string> ColumnNames)
        {
            grid.BorderStyle = BorderStyle.FixedSingle;

            grid.ColumnsCount = ColumnNames.Count;
            grid.FixedRows = 1;
            grid.Rows.Insert(0);           

            SourceGrid.Cells.Editors.ComboBox cbEditor = new SourceGrid.Cells.Editors.ComboBox(typeof(string));
            cbEditor.StandardValues = new string[] { "Value 1", "Value 2", "Value 3" };
            cbEditor.EditableMode = SourceGrid.EditableMode.Focus | SourceGrid.EditableMode.SingleClick | SourceGrid.EditableMode.AnyKey;            

            int p = 0;
            foreach (var _column in ColumnNames)
            {
                grid[0, p] = new SourceGrid.Cells.ColumnHeader(_column);
                grid.Columns.SetWidth(p, 100);
                p++;
            }
             
            grid.Rows.Insert(1);
            grid.Rows[1].Height = dgvSgtFile.Height - 50;
            for (int q = 0; q < ColumnNames.Count; q++)
            {
                //ZedGraphControl ZDC = new ZedGraphControl();
                //ZDC.GraphPane.Title.Text =  "";
                //ZDC.GraphPane.XAxis.Title.Text =  "";
                //ZDC.GraphPane.YAxis.Title.Text =  "";

                //CreateGraph(ZDC, GetSignal(Data, q));

                PlotterDisplayEx PDE = new PlotterDisplayEx();
                PDE.Smoothing = System.Drawing.Drawing2D.SmoothingMode.None;
                PDE.Refresh();

                BuildGraph(PDE, GetSignal(Data, q));

                grid[1, q] = new SourceGrid.Cells.CellControl(PDE);
           }
           //grid.AutoSizeCells();
        }

        // Построение графика Кепстральных коэффициентов
        public void CreateGraph(ZedGraphControl zgc, UInt32[] sig)       //double[] FactFreq
        {
            try
            {
                GraphPane myPane;
                // get a reference to the GraphPane
                myPane = zgc.GraphPane;
                myPane.CurveList.Clear();
                PointPairList list1 = new PointPairList();
                double K = sig.Length;
                for (int i = 0; i < K - 1; i++)
                {
                    list1.Add(sig[i], i*(-1));     
                }

                // Рисуем график красной линией
                LineItem myCurve = myPane.AddCurve("", list1, Color.Red, SymbolType.None);

                //myPane.YAxis.Scale.Min = 0;
                //myPane.XAxis.Scale.Min = 0;
                zgc.AxisChange();
                zgc.Invalidate();
            }
            catch (NullReferenceException e)
            {

            }
        }

        private void BuildGraph(PlotterDisplayEx display, UInt32[] sig)
        {
            this.SuspendLayout();

            display.DataSources.Clear();
            display.SetDisplayRangeX(sig.Min(), sig.Max());

            display.DataSources.Add(new DataSource());
            display.DataSources[0].Name = "Graph " + (0 + 1);
            display.DataSources[0].OnRenderXAxisLabel += RenderXLabel;

            this.Text = "Tiled Graphs (vertical prefered) autoscaled";
            display.PanelLayout = PlotterGraphPaneEx.LayoutMode.TILES_HOR;
            display.DataSources[0].Length = sig.Length;
            display.DataSources[0].AutoScaleY = true;
            display.DataSources[0].SetDisplayRangeY(0, sig.Length);
            display.DataSources[0].SetGridDistanceY(100);
            CalcGraph(display.DataSources[0], sig);
        }

        protected void CalcGraph(DataSource src, UInt32[] sig)
        {
            for (int i = 0; i < src.Length; i++)
            {
                src.Samples[i].y = i;

                src.Samples[i].x = sig[i];
            }
            src.OnRenderYAxisLabel = RenderYLabel;
        }

        private String RenderYLabel(DataSource s, float value)
        {
            return String.Format("{0:0.0}", value);
        }


        private String RenderXLabel(DataSource s, int idx)
        {
            //if (s.AutoScaleX)
            //{
            //    //if (idx % 2 == 0)
            //    {
            //        int Value = (int)(s.Samples[idx].x);
            //        return "" + Value;
            //    }
            //    return "";
            //}
            //else
            //{
            //    int Value = (int)(s.Samples[idx].x / 200);
            //    String Label = "" + Value + "\"";
            //    return Label;
            //}
            return "";
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            Size delta = this.Size - OldSize;
            double kw = this.Size.Width / OldSize.Width * 1.0;
            double kh = this.Size.Height / OldSize.Height * 1.0;
            tabControl1.Width = tabControl1.Width + delta.Width;
            tabControl1.Height = tabControl1.Height + delta.Height;
            dgvSgtFile.Width = dgvSgtFile.Width + delta.Width;     //Convert.ToInt32(dgvSgtFile.Width * kw);
            dgvSgtFile.Height = dgvSgtFile.Height + delta.Height;  //Convert.ToInt32(dgvSgtFile.Height * kh);
            gridSgtFile.Width = gridSgtFile.Width + delta.Width;
            gridSgtFile.Height = gridSgtFile.Height + delta.Height;

            OldSize = this.Size;
        }
    }
}

// Из private void openSgtMenuItem_Click(object sender, EventArgs e) 
//int p = 0;
//foreach (UInt32[] Row in Data)
//{
//    dgvSgtFile.Rows.Add();                   
//    for (int i = 0; i < Row.Length; i++)
//    {
//        dgvSgtFile.Rows[p].Cells[i].Value = Row[i];
//    }
//    p++;
//}

//for (int i = 10; i < Data[0].Length; i++)
//{
//    ZedGraphControl ZDC = new ZedGraphControl();
//    ZDC.Location = new Point(((i - 10) * dgvSgtFile.Columns[2].Width) + 1, 23);
//    ZDC.Size = new Size(dgvSgtFile.Columns[2].Width, dgvSgtFile.Height);
//    ZDC.GraphPane.Title.Text =  "";
//    ZDC.GraphPane.XAxis.Title.Text =  "";
//    ZDC.GraphPane.YAxis.Title.Text =  "";

//    dgvSgtFile.Controls.Add(ZDC);
//    PaintGraph(ZDC, Data, i);
//}  