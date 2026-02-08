using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDBExplorer.Utils
{
    public class ListViewItemComparer : IComparer
    {
        private int _col;
        private SortOrder _order;

        public ListViewItemComparer(int column, SortOrder order)
        {
            _col = column;
            _order = order;
        }

        public int Compare(object? x, object? y)
        {
            int returnVal = -1;
            var itemX = (ListViewItem)x;
            var itemY = (ListViewItem)y;

            if (_col == 2 && itemX.SubItems[_col].Tag is long sizeX && itemY.SubItems[_col].Tag is long sizeY)
            {
                returnVal = sizeX.CompareTo(sizeY);
            }
            else
            {
                returnVal = string.Compare(itemX.SubItems[_col].Text, itemY.SubItems[_col].Text);
            }

            if (_order == SortOrder.Descending)
                returnVal *= -1;

            return returnVal;
        }
    }
}
