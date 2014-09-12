using System;
using System.Collections.Generic;
using System.Text;

namespace SDGControlLibrary.UndoRedo
{
    /// <summary>
    /// Really simple class, only containing a string. Meant for inheriting
    /// </summary>
    public class Memento
    {
        private int idMouseOriginated = -1;
        public int IdMouseOriginated
        {
            get 
            {
                return idMouseOriginated;
            }
            set
            {
                if (IdMouseOriginated == -1)
                {
                    idMouseOriginated = value;
                }
            }
        }

        public string CurrentState
        {
            get;
            set;
        }

        public Memento(string state, int idMouseOriginated)
        {
            this.CurrentState = state;
            this.idMouseOriginated = idMouseOriginated;
        }
    }
}
