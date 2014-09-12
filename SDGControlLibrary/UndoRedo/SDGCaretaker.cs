using System;
using System.Collections.Generic;
using System.Text;
using Sdgt;

namespace SDGControlLibrary.UndoRedo
{
    //Delegate that connects the Originators with the Caretaker
    public delegate void ActionOcurredEventHandler(IOriginator origin);

    public static class SDGCaretaker
    {
        /* Recuerda esto: Tienes que hacer una lista con 2 cosas: ORIGINATOR - MEMENTO y luego ir 
         * viendo que enviarle a cada uno de los originators en caso del undo */

        //A list (not stack really) that saves the Originator and its state
        private static List<object[]> lastActions = new List<object[]>();

        //A list of actions that have been undone recently
        private static List<object[]> redoActionList = new List<object[]>();

        //Maximium amount of total actions
        private static int MaxUndoActions = 100;

        //Maximium amount of user actions
        private static int MaxUndoPerUser = 20;

        //List that limits keeps how many actions per mouse are in the stack
        private static System.Collections.Hashtable mouseTotalActions = new System.Collections.Hashtable();

        public static void KeyboardHook(object sender, SdgKeyEventArgs e)
        {
            if (e.Control)            
            {
                switch(e.KeyCode)
                {
                    //UNDO
                    case System.Windows.Forms.Keys.Z:
                        {
                            e.Handled = true;
                            DoUndo(e.ID);
                            break;
                        }
                    //REDO
                    case System.Windows.Forms.Keys.Y:
                        {
                            e.Handled = true;
                            DoRedo(e.ID);
                            break;
                        }
                }                
            }
        }

        private static void DoUndo(int mouseID)
        {
            object[] lastAct = GetLastActionByID(mouseID, lastActions);
            
            if (lastAct != null)
            {
                IOriginator orig = (IOriginator)lastAct[0];
                Memento mement = (Memento)lastAct[1];

                List<object[]> onTopActions = GetOnTopLastActions(lastAct);
                List<Memento> onTopMemento = new List<Memento>();

                if (onTopActions != null)
                {
                    for (int i = 0; i < onTopActions.Count; i++)
                    {
                        object[] currentAction = onTopActions[i];
                        onTopMemento.Add((Memento)currentAction[1]);
                    }
                }

                Memento currentState = orig.SaveState;
                currentState.IdMouseOriginated = mement.IdMouseOriginated;

                bool[] listToRemove = orig.RestoredOldState(mement, onTopMemento);

                if (listToRemove[1])
                {
                    redoActionList.Add(new object[3] { orig, currentState, mement });
                }

                RemoveActions(listToRemove, lastAct, onTopActions);

                if (listToRemove[0])
                {
                    DoUndo(mouseID);
                }
            }
        }

        private static void DoRedo(int mouseID)
        {
            object[] lastRedoAct = GetLastActionByID(mouseID, redoActionList);

            if (lastRedoAct != null)
            {
                IOriginator orig = (IOriginator)lastRedoAct[0];
                Memento redoMement = (Memento)lastRedoAct[1];
                Memento oldMement = (Memento)lastRedoAct[2];

                orig.RestoredOldState(redoMement, null);
                redoActionList.Remove(lastRedoAct);

                AddActionToStack(orig, oldMement, true);
            }
        }

        private static object[] GetFirstActionByID(int ID,List<object[]> listFrom)
        {
            for (int i = 0; i < listFrom.Count; i++)
            {
                object[] currentAction = listFrom[i];
                if (((Memento)currentAction[1]).IdMouseOriginated == ID)
                {
                    return currentAction;
                }
            }

            return null;
        }

        private static object[] GetLastActionByID(int ID, List<object[]> listFrom)
        {
            for (int i = listFrom.Count - 1; i >= 0; i--)
            {
                object[] currentAction = listFrom[i];
                if (((Memento)currentAction[1]).IdMouseOriginated == ID)
                {
                    return currentAction;
                }
            }

            return null;
        }

        private static int GetUserWithMoreActivity()
        {
            int idMouse = -1;
            int maxActions = -1;

            foreach (object keyValue in mouseTotalActions.Keys)
            {
                int currentMaxActions = (int)mouseTotalActions[keyValue];

                if (currentMaxActions > maxActions)
                {
                    maxActions = currentMaxActions;
                    idMouse = (int)keyValue;
                }
            }

            return idMouse;
        }

        private static List<object[]> GetOnTopLastActions(object[] desiredAction)
        {
            List<object[]> onTopActions = null;

            if (lastActions.Contains(desiredAction))
            {
                int indexOfDesiredAction = lastActions.IndexOf(desiredAction);
                IOriginator currentOriginator = (IOriginator)desiredAction[0];

                for (int i = indexOfDesiredAction + 1; i < lastActions.Count; i++)
                {
                    object[] currentAction = lastActions[i];
                    if ((IOriginator)currentAction[0] == currentOriginator)
                    {
                        if (onTopActions == null) onTopActions = new List<object[]>();

                        onTopActions.Add(currentAction);
                    }
                }
            }

            return onTopActions;
        }

        public static void AddAction(IOriginator originator)
        {
            Memento currentState = originator.SaveState;

            if (currentState != null)
            {
                AddActionToStack(originator, currentState,false);
            }
        }

        public static void ClearUndoRedo()
        {
            lastActions.Clear();
            redoActionList.Clear();
        }

        private static void AddActionToStack(IOriginator originator, Memento currentState, bool DoingRedo)
        {
            int totalActionsMouse = 1;

            if (!mouseTotalActions.Contains(currentState.IdMouseOriginated))
            {
                mouseTotalActions.Add(currentState.IdMouseOriginated, 1);
            }
            else
            {
                mouseTotalActions[currentState.IdMouseOriginated] = (int)mouseTotalActions[currentState.IdMouseOriginated] + 1;
                totalActionsMouse = (int)mouseTotalActions[currentState.IdMouseOriginated];
            }

            if (totalActionsMouse > MaxUndoPerUser)
            {
                RemoveFirstAction(currentState.IdMouseOriginated);
            }
            else if (lastActions.Count > MaxUndoActions)
            {
                RemoveFirstAction(GetUserWithMoreActivity());
            }

            object[] actionToAdd = new object[2] { originator, currentState };

            ValidateAllRedoActions(actionToAdd, true);            

            lastActions.Add(actionToAdd);
        }

        private static void RemoveActions(bool[] listToRemove, object[] lastAct, List<object[]> onTopActions)
        {
            int countOnTop = 0;

            if (onTopActions != null)
            {
                countOnTop = onTopActions.Count;
            }

            if (listToRemove != null && listToRemove.Length == countOnTop + 2)
            {
                if (listToRemove[1])
                {
                    RemoveFromStack(lastAct,true);
                }

                if (countOnTop != 0)
                {
                    for (int i = 0; i < listToRemove.Length - 2; i++)
                    {
                        if (listToRemove[i + 2])
                        {
                            //We must remove the value
                            RemoveFromStack(onTopActions[i],true);
                        }
                    }
                }
            }
        }

        private static void RemoveFromStack(object[] lastAct, bool toRedoList)
        {
            if (lastActions.Contains(lastAct))
            {
                Memento lastMemento = (Memento)lastAct[1];

                if (mouseTotalActions.Contains(lastMemento.IdMouseOriginated))
                {
                    mouseTotalActions[lastMemento.IdMouseOriginated] = (int)mouseTotalActions[lastMemento.IdMouseOriginated] - 1;
                }

                lastActions.Remove(lastAct);

                if (toRedoList)
                {
                    ValidateAllRedoActions(lastAct, false);
                }
            }
        }

        private static void RemoveFirstAction(int idMouseOriginated)
        {
            for (int i = 0; i < lastActions.Count; i++)
            {
                object[] currentAct = (object[])lastActions[i];
                Memento lastMemento = (Memento)currentAct[1];

                if (lastMemento.IdMouseOriginated == idMouseOriginated)
                {
                    IOriginator lastOriginator = (IOriginator)currentAct[0];
                    bool removeAllOtherActions = lastOriginator.StateRemoved(lastMemento);

                    if (removeAllOtherActions)
                    {
                        RemoveAllActionFromOriginator(lastOriginator);
                    }
                    else
                    {
                        RemoveFromStack(currentAct,false);
                    }

                    return;
                }
            }
        }

        private static void RemoveAllActionFromOriginator(IOriginator removeOriginator)
        {
            for (int i = 0; i < lastActions.Count; i++)
            {
                object[] currentAct = (object[])lastActions[i];
                IOriginator lastOriginator = (IOriginator)currentAct[0];

                if (lastOriginator == removeOriginator)
                {
                    RemoveFromStack(currentAct,false);
                }
            }
        }

        private static void ValidateAllRedoActions(object[] action, bool Added)
        {
            List<object[]> redoToRemove = new List<object[]>();

            for (int i = 0; i < redoActionList.Count; i++)
            {
                object[] redoAction = (object[])redoActionList[i];

                if (action[0] == redoAction[0])
                {
                    bool validateRedoAction = ((IOriginator)action[0]).ValidateRedoAction((Memento)redoAction[1], (Memento)action[1],Added);
                    
                    if (!validateRedoAction)
                    {
                        redoToRemove.Add(redoAction);
                    }
                }
            }

            for (int i = 0; i < redoToRemove.Count; i++)
            {
                redoActionList.Remove(redoToRemove[i]);
            }
        }
    }
}
