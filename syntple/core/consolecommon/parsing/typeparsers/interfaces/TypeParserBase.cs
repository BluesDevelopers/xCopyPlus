﻿namespace syntple.core.consolecommon.parsing.typeparsers.interfaces
{
    public abstract class TypeParserBase<T> : ITypeParser
    {
        public Type GetTypeToParse(params Type[] genericTypeParams)
        {
            Type _returnType = typeof(T);
            if(genericTypeParams.Count() > 0 && _returnType.IsGenericType)
            {
                _returnType = _returnType.MakeGenericType(genericTypeParams);
            }
            return _returnType;
        }
        public abstract object Parse(string toParse, Type typeToParse, ITypeParserContainer parserContainer);
        public virtual string[] GetAcceptedValues(Type typeToParse)
        {
            return new string[0];
        }
    }
}
