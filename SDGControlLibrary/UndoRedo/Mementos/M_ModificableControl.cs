using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace SDGControlLibrary.UndoRedo
{
    public enum LastModificableActionType { LocationSizeChanged, ControlDeletion, ControlCreation, CurrentState };

    public class M_ModificableControl: Memento
    {
        protected LastModificableActionType lastAction;
        public LastModificableActionType LastAction
        {
            get { return lastAction; }
        }

        protected Point location = Point.Empty;
        public Point Location
        {
            get { return location; }
        }

        protected Size size = Size.Empty;
        public Size Size
        {
            get { return size; }
        }

        public M_ModificableControl(int id,Point p, Size s,LastModificableActionType type)
            :base("",id)
        {
            location = p;
            size = s;
            lastAction = type;
        }
    }
}
