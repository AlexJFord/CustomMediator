using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomMediator
{
    public static class RequestHandlerFactory
    {
        private static List<Type> _handlers = new List<Type>();

        public static void AddRequestHandler(Type requestHandlerType)
        {
            _handlers.Add(requestHandlerType);
        }

        public static object CreateRequestHandler(Type genericType)
        {
            var handlerType = _handlers.FirstOrDefault(x => x.GetInterfaces().Contains(genericType));
            
            return Activator.CreateInstance(handlerType);
        }
    }
}