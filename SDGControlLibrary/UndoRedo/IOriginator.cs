using System;
using System.Collections.Generic;
using System.Text;

namespace SDGControlLibrary.UndoRedo
{
    public interface IOriginator
    {
        /// <summary>
        /// Event which is triggered when a control is about to change
        /// </summary>
        event ActionOcurredEventHandler ActionWillHappen;

        /// <summary>
        /// Checks if the reDoState of this is still valid even if a new State has been ended
        /// </summary>
        /// <param name="reDoState">It the old ReDoState</param>
        /// <param name="newState">The new State</param>
        /// <param name="added">If it is added or removed</param>
        /// <returns>If the old redo state is valid</returns>
        bool ValidateRedoAction(Memento reDoState, Memento newState, bool added);

        /// <summary>
        /// Tells the IOriginator that an state of his was removed from the stack
        /// </summary>
        /// <param name="removedMemento">The state that has been removed</param>
        /// <returns>Boolean that tells if all other IOriginator changes should be removed from the stack</returns>
        bool StateRemoved(Memento removedMemento);

        /// <summary>
        /// Method that saves the current state of the object
        /// </summary>
        Memento SaveState
        { get; }

        /// <summary>
        /// Method that restores a old state and checks if the new actions performed to that 
        /// control can be changed or not and tells that they should be removed
        /// </summary>
        /// <param name="desiredState">State that is desired to be restored</param>
        /// <param name="onTopStackList">Other states that have been done after the desired one, and that should be decided if they should be removed or not</param>
        /// <returns>List of booleans that must be the lenght of the onTopStackList + 2, the first one telling if next state should be loaded , the second telling if the desired State should be removed and each one after represents if the corresponding Memento (index - 1) should be removed or not</returns>
        bool[] RestoredOldState(Memento desiredState,List<Memento> onTopStackList);

        //new bool[onTopStackList.Count + 2]{Next State Should Load, Desired State Removal, new bool[onTopStackList.Count]}
    }
}
