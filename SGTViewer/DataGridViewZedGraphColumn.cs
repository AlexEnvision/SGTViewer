using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZedGraph;
using System.Drawing;

namespace System.Windows.Forms
{
    public class DataGridViewZedGraphColumn : DataGridViewColumn
    {
        public DataGridViewZedGraphColumn()
            : base(new ZedGraphCell())
        {
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                // Ensure that the cell used for the template is a ZedGraphCell.
                if (value != null &&
                    !value.GetType().IsAssignableFrom(typeof(ZedGraphControl)))
                {
                    throw new InvalidCastException("Must be a ZedGraphCell");
                }
                base.CellTemplate = value;
            }
        }
    }

    public class ZedGraphCell : DataGridViewTextBoxCell
    {
        ZedGraphEditingControl ctl;

        public ZedGraphCell()
            : base()
        {
            // Use the short date format.
            this.Style.Format = "d";
            //this.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            //this.Style.ForeColor = Color.WhiteSmoke;
            //this.Style.BackColor = Color.WhiteSmoke;
            //this.Style.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            //this.Style.SelectionForeColor = Color.Blue;
            //this.Style.SelectionBackColor = Color.Blue;

        }

        public override void InitializeEditingControl(int rowIndex, object
            initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            // Set the value of the editing control to the current cell value.
            base.InitializeEditingControl(rowIndex, initialFormattedValue,
                dataGridViewCellStyle);           

            ctl =
                DataGridView.EditingControl as ZedGraphEditingControl;
            // Use the default row value when Value property is null.
            if (this.Value == null)
            {
                ctl.NValue = (UInt32[])this.DefaultNewRowValue;
            }
            else
            {
                try 
                { 
                    ctl.NValue = (UInt32[])this.Value;
                }
                catch (Exception) { ctl.NValue = (UInt32[])this.DefaultNewRowValue; }
            }
        }

        //protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds,
        //   int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue,
        //   string errorText, DataGridViewCellStyle cellStyle,
        //   DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        //{
        //    if (ctl != null)
        //    {
        //        graphics.DrawImage(ctl.GetImage(), cellBounds);
        //    }
        //    //В методе Paint я предполагал разместить код рисующий ZedGraph.
        //}

        public override Type EditType
        {
            get
            {
                // Return the type of the editing control that ZedGraphCell uses.
                return typeof(ZedGraphEditingControl);
            }
        }

        public override Type ValueType
        {
            get
            {
                // Return the type of the value that ZedGraphCell contains.
                return typeof(ZedGraphControl);
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                // Use the current date and time as the default value.
                return null;
            }
        }

       
    }

    //Делегат изменения значения ZedGraphEditingControl
    public delegate void ValueChangedDel(); 

    class ZedGraphEditingControl : ZedGraphControl, IDataGridViewEditingControl
    {
        DataGridView dataGridView;
        private bool valueChanged = false;
        int chType = 0;
        int rowIndex;
        private UInt32[] val = null;
        GraphPane myPane;

        UInt32[] Value = null;

        //Эвент измения значения NValue
        public event ValueChangedDel ValueChanged;

        public UInt32[] NValue
        {
            get { return val; }
            set
            {
                val = value;
                chType = 1;
                UInt32[] t = new UInt32[]{};
                try 
                {
                    t = (UInt32[])val;
                    //генерирование события измения значения NValue
                    if (ValueChanged != null)
                        ValueChanged();
                }
                catch (Exception) { }
                chType = 0;
            }
        }       

        public ZedGraphEditingControl()
        {
            this.GraphPane.Title.Text = "";
            this.GraphPane.XAxis.Title.Text = "";
            this.GraphPane.YAxis.Title.Text = "";

            this.ValueChanged += ZedGraphEditingControl_ValueChanged;
        }

        void ZedGraphEditingControl_ValueChanged()  //object sender, EventArgs e
        {
            if (chType == 0)
            {
                val = this.Value;
                CreateGraph(this.val);
            }
        }

        // Implements the IDataGridViewEditingControl.EditingControlFormattedValue 
        // property.
        public object EditingControlFormattedValue
        {
            get
            {
                CreateGraph(this.val);
                return this.Value;
            }
            set
            {
                if (value is String)
                {
                    try
                    {
                        // This will throw an exception of the string is 
                        // null, empty, or not in the format of a date.

                        //this.Value = DateTime.Parse((String)value);
                    }
                    catch
                    {
                        // In the case of an exception, just use the 
                        // default value so we're not left with a null
                        // value.

                        //this.Value = DateTime.Now;
                    }
                }
            }
        }

        // Implements the 
        // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
        public object GetEditingControlFormattedValue(
            DataGridViewDataErrorContexts context)
        {
            return EditingControlFormattedValue;
        }

        // Implements the 
        // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.ForeColor = dataGridViewCellStyle.ForeColor;
            this.BackColor = dataGridViewCellStyle.BackColor;
        }

        // Implements the IDataGridViewEditingControl.EditingControlRowIndex 
        // property.
        public int EditingControlRowIndex
        {
            get
            {
                return rowIndex;
            }
            set
            {
                rowIndex = value;
            }
        }

        // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey 
        // method.
        public bool EditingControlWantsInputKey(
            Keys key, bool dataGridViewWantsInputKey)
        {
            // Let the DateTimePicker handle the keys listed.
            switch (key & Keys.KeyCode)
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit 
        // method.
        public void PrepareEditingControlForEdit(bool selectAll)
        {
            // No preparation needs to be done.
        }

        // Implements the IDataGridViewEditingControl
        // .RepositionEditingControlOnValueChange property.
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlDataGridView property.
        public DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridView;
            }
            set
            {
                dataGridView = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlValueChanged property.
        public bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }
            set
            {
                valueChanged = value;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingPanelCursor property.
        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        // Построение графика 
        public void CreateGraph(UInt32[] sig)     
        {
            try
            {
                // get a reference to the GraphPane
                myPane = this.GraphPane;
                myPane.CurveList.Clear();
                PointPairList list1 = new PointPairList();
                double K = sig.Length;
                for (int i = 0; i < K - 1; i++)
                {
                    list1.Add(sig[i], i * (-1));
                }

                // Рисуем график красной линией
                LineItem myCurve = myPane.AddCurve("", list1, Color.Red, SymbolType.None);

                this.AxisChange();
                this.Invalidate();
            }
            catch (NullReferenceException e)
            {

            }
        }
    }
}
