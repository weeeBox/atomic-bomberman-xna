using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Consoles.Commands
{
    public class CCommandRegister
    {
        private Dictionary<char, LinkedList<CCommand>> commandsLookup;

        public CCommandRegister()
        {
            commandsLookup = new Dictionary<char, LinkedList<CCommand>>();
        }

        public bool RegisterCommand(CCommand command)
        {
            LinkedList<CCommand> commandList = FindList(command);
            return AddCommand(commandList, command);
        }

        public bool UnregisterCommand(CCommand command)
        {
            LinkedList<CCommand> list = FindList(command);
            for (LinkedListNode<CCommand> node = list.First; node != null; node = node.Next)
            {   
                if (command == node.Value)
                {
                    list.Remove(node);
                    return true;
                }
            }

            return false;
        }

        public List<CCommand> ListCommands()
        {
            List<CCommand> list = new List<CCommand>();
            foreach (KeyValuePair<char, LinkedList<CCommand>> e in commandsLookup)
            {
                list.AddRange(e.Value);
            }
            return list;
        }

        public CCommand FindCommand(String name)
        {
            LinkedList<CCommand> list = FindList(name);
            foreach (CCommand command in list)
            {
                if (command.name.Equals(name))
                {
                    return command;
                }
            }

            return null;
        }

        public void GetSuggested(String token, LinkedList<CCommand> outList)
        {
            LinkedList<CCommand> list = FindList(token);

            foreach (CCommand command in list)
            {
                if (command.name.StartsWith(token))
                {
                    outList.AddLast(command);
                }
            }
        }

        private LinkedList<CCommand> FindList(CCommand command)
        {
            return FindList(command.name);
        }

        private LinkedList<CCommand> FindList(String token)
        {
            char firstChar = token[0];

            LinkedList<CCommand> commandList;
            bool found = commandsLookup.TryGetValue(firstChar, out commandList);
            if (!found)
            {
                commandList = new LinkedList<CCommand>();
                commandsLookup[firstChar] = commandList;
            }

            return commandList;
        }

        private bool AddCommand(LinkedList<CCommand> commandList, CCommand command)
        {
            String name = command.name;
            for (LinkedListNode<CCommand> node = commandList.First; node != null; node = node.Next)
            {
                CCommand other = node.Value;
                if (command == other)
                {
                    return false; // no duplicates
                }

                String otherName = other.name;
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
