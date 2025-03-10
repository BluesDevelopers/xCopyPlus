﻿using syntple.core.consolecommon.helpers;
using syntple.core.consolecommon.parsing.typeparsers.interfaces;


namespace syntple.core.consolecommon.parsing.typeparsers
{
    public class TypeParserContainer : ITypeParserContainer
    {
        private List<ITypeParser> _typeParsers = new List<ITypeParser>();
        public IEnumerable<ITypeParser> TypeParsers => _typeParsers.AsReadOnly();

        #region Cstors
        public TypeParserContainer(bool overwriteDupes, TypeParserContainer parserContainer, IEnumerable<ITypeParser> typeParsers)
            : this(overwriteDupes, parserContainer.TypeParsers.Concat(typeParsers))
        {
        }
        public TypeParserContainer(bool overwriteDupes, TypeParserContainer parserContainer, params ITypeParser[] typeParsers)
            : this(overwriteDupes, parserContainer.TypeParsers.Concat(typeParsers))
        {
        }
        public TypeParserContainer(bool overwriteDupes, params ITypeParser[] typeParsers)
        {
            foreach (ITypeParser _tp in typeParsers) addTypeParser(_tp, overwriteDupes);
        }
        public TypeParserContainer(bool overwriteDupes, IEnumerable<ITypeParser> typeParsers)
        {
            foreach (ITypeParser _tp in typeParsers) addTypeParser(_tp, overwriteDupes);
        }
        #endregion

        #region Implementation
        public object Parse(string toParse, Type type)
        {
            if (type.IsInterface) throw new Exception("Cannot parse interfaces. Must use concrete types");
            return GetParser(type).Parse(toParse, type, this);
        }
        public string[] GetAcceptedValues(Type type)
        {
            return GetParser(type).GetAcceptedValues(type);
        }
        public ITypeParser GetParser(Type typeToParse)
        {
            var _comparer = new TypesByInheritanceLevelComparer();
            Type[] _genArgs = typeToParse.IsGenericType ? typeToParse.GetGenericArguments() : new Type[0];
            ITypeParser _parser =
                TypeParsers
                .Where(t => t.GetTypeToParse(_genArgs).IsAssignableFrom(typeToParse))
                .OrderBy(t => t.GetTypeToParse(_genArgs), _comparer)
                .LastOrDefault();
            if (_parser == null) throw new Exception($"Parsing type '{typeToParse.Name}' not handled");
            return _parser;
        }
        #endregion

        #region Helpers
        private void addTypeParser(ITypeParser typeParser, bool overwriteDupes)
        {
            ITypeParser _match = TypeParsers.FirstOrDefault(tp => tp.GetTypeToParse().Equals(typeParser.GetTypeToParse()));
            if(_match!=null)
            {
                if (overwriteDupes)
                {
                    _typeParsers.Remove(_match);
                }
                else throw new Exception($"Type parser of type '{typeParser.GetTypeToParse()}' already exists");
            }
            _typeParsers.Add(typeParser);
        }
        #endregion
    }
}
