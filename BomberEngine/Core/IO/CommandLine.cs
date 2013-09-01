using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BomberEngine.Debugging;

namespace BomberEngine.Core.IO
{
    public class CommandLineException : Exception
    {
        public CommandLineException(String message)
            : base(message)
        {
        }

        public CommandLineException(String message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class CommandLine
    {
        private IDictionary<String, CommandLineEntry> m_shortLookup;
        private IDictionary<String, CommandLineEntry> m_longLookup;

        private CommandLineEntry m_defaultEntry;

        public CommandLine()
        {
            m_shortLookup = new Dictionary<String, CommandLineEntry>();
            m_longLookup = new Dictionary<String, CommandLineEntry>();
        }

        public void Parse(String[] args)
        {
            ArgsIterator iter = new ArgsIterator(args);
            while (iter.HasNext())
            {
                String arg = iter.Next();
                if (arg.StartsWith("-"))
                {
                    String shortParam = arg.Substring(1);
                    if (shortParam.Length == 0)
                    {
                        throw new CommandLineException("Short param expected");
                    }

                    CommandLineEntry entry = FindEntry(shortParam, true);
                    if (entry == null)
                    {
                        throw new CommandLineException("Unknown param: " + arg);
                    }

                    entry.Parse(iter);
                }
                else if (arg.StartsWith("--"))
                {
                    String longParam = arg.Substring(2);
                    if (longParam.Length == 0)
                    {
                        throw new CommandLineException("Long param expected");
                    }

                    CommandLineEntry entry = FindEntry(longParam, false);
                    if (entry == null)
                    {
                        throw new CommandLineException("Unknown param: " + arg);
                    }

                    entry.Parse(iter);
                }
                else
                {
                    if (m_defaultEntry == null)
                    {
                        throw new CommandLineException("Default entry is not set");
                    }

                    m_defaultEntry.Parse(iter);
                }
            }
        }

        public void SetDefaultEntry(CommandLineEntry entry)
        {
            m_defaultEntry = entry;
        }

        public void Register(CommandLineEntry entry)
        {
            String shortName = entry.ShortName;
            String longName = entry.LongName;

            if (shortName != null)
            {
                m_shortLookup[shortName] = entry;
            }
            if (longName != null)
            {
                m_longLookup[longName] = entry;
            }
        }

        private CommandLineEntry FindEntry(String name, bool isShort)
        {
            IDictionary<String, CommandLineEntry> lookup = isShort ? m_shortLookup : m_longLookup;
            CommandLineEntry e;
            if (lookup.TryGetValue(name, out e))
            {
                return e;
            }

            return null;
        }
    }

    public abstract class CommandLineEntry
    {
        private String m_shortName;
        private String m_longName;

        protected CommandLineEntry(String shortName, String longName = null)
        {
            m_shortName = shortName;
            m_longName = longName;
        }

        public abstract void Parse(ArgsIterator iter);

        public String ShortName
        {
            get { return m_shortName; }
        }

        public String LongName
        {
            get { return m_longName; }
        }
    }

    public class ArgsIterator : IResettable
    {
        private String[] m_args;
        private int m_index;

        public ArgsIterator(String[] args)
        {
            m_args = args;
            Reset();
        }

        public void Reset()
        {
            m_index = -1;
        }

        public bool HasNext()
        {
            return m_index < m_args.Length - 1;
        }

        public String Next()
        {
            Debug.Assert(HasNext());
            return m_args[++m_index];
        }

        public String PeekNext()
        {
            Debug.Assert(HasNext());
            return m_args[m_index + 1];
        }
    }
}
