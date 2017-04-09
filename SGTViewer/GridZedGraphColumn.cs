using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SourceGrid;
using SourceGrid.Cells;

namespace SGTViewer
{
    class GridZedGraphColumn: GridVirtual
    {
        public override bool EnableSort
        {
            get;
            set;
        }

        public GridZedGraphColumn(string Name)
        {
             this.Name = Name;
        }

        public override ICellVirtual GetCell(int p_iRow, int p_iCol)
        {
            return null;
        }

        public override ICellVirtual[] GetCellsAtRow(int p_RowIndex)
        {
            return base.GetCellsAtRow(p_RowIndex);
        }

        protected override ColumnsBase CreateColumnsObject()
        {
            return null;
        }

        protected override RowsBase CreateRowsObject()
        {
            return null;
        }
    }
}
