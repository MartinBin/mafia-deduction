using System.Collections.Generic;

namespace Mafia_server
{
    public class PlayerCaretaker
    {
        private readonly Dictionary<int, Stack<PlayerMemento>> _mementoStacks = new();

        public void Save(Player player)
        {
            if (!_mementoStacks.ContainsKey(player.ID))
            {
                _mementoStacks[player.ID] = new Stack<PlayerMemento>();
            }
            _mementoStacks[player.ID].Push(player.SaveState());
        }

        public void Restore(Player player)
        {
            if (_mementoStacks.ContainsKey(player.ID) && _mementoStacks[player.ID].Count > 0)
            {
                player.RestoreState(_mementoStacks[player.ID].Pop());
            }
        }
    }
}