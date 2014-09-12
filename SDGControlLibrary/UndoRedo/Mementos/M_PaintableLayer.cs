using System;
using System.Collections.Generic;
using System.Text;
using SDGControlLibrary.Misc_Controls.Layer_Types;

namespace SDGControlLibrary.UndoRedo
{
    public enum LastPaintableLayerAction { LayerCreated, Deletion, CurrentState };

    public class M_PaintableLayer: Memento
    {
        protected LastPaintableLayerAction lastAction = LastPaintableLayerAction.LayerCreated;
        public LastPaintableLayerAction LastAction
        {
            get { return lastAction; }
        }

        #region Layer Created

        protected BaseTypeLayer layer = null;
        public BaseTypeLayer Layer
        {
            get { return layer; }
        }

        #endregion

        #region Deletion

        protected List<BaseTypeLayer> affectedLayers = new List<BaseTypeLayer>();
        public List<BaseTypeLayer> AffectedLayers
        { 
            get
            {
                return affectedLayers;
            }
        }

        protected List<object> layerState = new List<object>();
        public List<object> LayerState
        {
            get
            {
                return layerState;
            }
        }

        #endregion

        public M_PaintableLayer(int mouseID, BaseTypeLayer btl)
            :base("",mouseID)
            
        {
            this.layer = btl;
            this.lastAction = LastPaintableLayerAction.LayerCreated;
        }

        public M_PaintableLayer(int mouseID)
            : base("", mouseID)
        {
            this.lastAction = LastPaintableLayerAction.Deletion;
        }

        public M_PaintableLayer()
            : base("", -1)
        {
            this.lastAction = LastPaintableLayerAction.CurrentState;
        }

        public void AddAffectedLayers(List<BaseTypeLayer> newAffectedLayers)
        {
            foreach (BaseTypeLayer btl in newAffectedLayers)
            {
                object newState = btl.SaveState();

                if (!affectedLayers.Contains(btl))
                {
                    affectedLayers.Add(btl);
                    layerState.Add(newState);
                }
                else
                {
                    int index = affectedLayers.IndexOf(btl);
                    layerState[index] = newState;
                }
            }
        }

        public void RemoveAffectedLayers(List<BaseTypeLayer> newAffectedLayers)
        {
            foreach (BaseTypeLayer btl in newAffectedLayers)
            {              
                if (affectedLayers.Contains(btl))
                {
                    int index = affectedLayers.IndexOf(btl);

                    affectedLayers.RemoveAt(index);
                    layerState.RemoveAt(index);
                }
             
            }
        }
    }
}
