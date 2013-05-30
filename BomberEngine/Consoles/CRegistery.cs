using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BomberEngine.Consoles
{
    public class CRegistery
    {
        private Dictionary<char, LinkedList<CCommand>> commandsLookup;

        public CRegistery()
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

        /* Дублеж, пиздешь и провокация */

        public List<CCommand> ListCommands()
        {
            List<CCommand> list = new List<CCommand>();
            foreach (KeyValuePair<char, LinkedList<CCommand>> e in commandsLookup)
            {
                LinkedList<CCommand> commands = e.Value;
                foreach (CCommand command in commands)
                {
                    if (!(command is CVarCommand))
                    {
                        list.Add(command);
                    }
                }
            }
            return list;
        }

        public List<CCommand> ListCommands(String prefix)
        {
            List<CCommand> list = new List<CCommand>();
            foreach (KeyValuePair<char, LinkedList<CCommand>> e in commandsLookup)
            {
                LinkedList<CCommand> commands = e.Value;
                foreach (CCommand command in commands)
                {
                    if (!(command is CVarCommand) && command.StartsWith(prefix))
                    {
                        list.Add(command);
                    }
                }
            }
            return list;
        }

        public List<CVar> ListVars()
        {
            List<CVar> list = new List<CVar>();
            foreach (KeyValuePair<char, LinkedList<CCommand>> e in commandsLookup)
            {
                LinkedList<CCommand> commands = e.Value;
                foreach (CCommand command in commands)
                {
                    CVarCommand varCommand = command as CVarCommand;
                    if (varCommand != null)
                    {
                        list.Add(varCommand.cvar);
                    }
                }
            }
            return list;
        }

        public List<CVar> ListVars(String prefix)
        {
            List<CVar> list = new List<CVar>();
            foreach (KeyValuePair<char, LinkedList<CCommand>> e in commandsLookup)
            {
                LinkedList<CCommand> commands = e.Value;
                foreach (CCommand command in commands)
                {
                    CVarCommand varCommand = command as CVarCommand;
                    if (varCommand != null && varCommand.StartsWith(prefix))
                    {
                        list.Add(varCommand.cvar);
                    }
                }
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
                if (command.StartsWith(token))
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
