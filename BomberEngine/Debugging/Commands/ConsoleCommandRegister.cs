using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Debugging.Commands
{
    public class ConsoleCommandRegister
    {
        private Dictionary<char, LinkedList<ConsoleCommand>> commandsLookup;

        public ConsoleCommandRegister()
        {
            commandsLookup = new Dictionary<char, LinkedList<ConsoleCommand>>();
        }

        public bool RegisterCommand(ConsoleCommand command)
        {
            LinkedList<ConsoleCommand> commandList = FindList(command);
            return AddCommand(commandList, command);
        }

        public void GetSuggested(String token, LinkedList<ConsoleCommand> outList)
        {
            LinkedList<ConsoleCommand> list = FindList(token);

            foreach (ConsoleCommand command in list)
            {
                if (command.GetName().StartsWith(token))
                {
                    outList.AddLast(command);
                }
            }
        }

        private LinkedList<ConsoleCommand> FindList(ConsoleCommand command)
        {
            return FindList(command.GetName());
        }

        private LinkedList<ConsoleCommand> FindList(String token)
        {
            char firstChar = token[0];

            LinkedList<ConsoleCommand> commandList;
            bool found = commandsLookup.TryGetValue(firstChar, out commandList);
            if (!found)
            {
                commandList = new LinkedList<ConsoleCommand>();
                commandsLookup[firstChar] = commandList;
            }

            return commandList;
        }

        private bool AddCommand(LinkedList<ConsoleCommand> commandList, ConsoleCommand command)
        {
            String name = command.GetName();
            for (LinkedListNode<ConsoleCommand> node = commandList.First; node != null; node = node.Next)
            {
                ConsoleCommand other = node.Value;
                if (command == other)
                {
                    return false; // no duplicates
                }

                String otherName = other.GetName();
                if (name.CompareTo(otherName) < 0)
                {
                    commandList.AddBefore(node, command);
                    return true;
                }
            }

            commandList.AddLast(command);
            return true;
        }
    }
}
