using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CustomMediator
{
     public class Mediator
    {
        // Invoke commands with no return types.
        public void Send(IRequest request)
        {
            var    commandType = request.GetType();
            var    handlerType = typeof(IRequestHandler<>).MakeGenericType(commandType);
            object handler;

            try
            {
                handler = RequestHandlerFactory.CreateRequestHandler(handlerType);
            }
            catch
            {
                throw new Exception(string.Format("Handler for type {0} not found.", commandType));
            }

            var handle = handler.GetType().GetMethod("Handle");

            try
            {
                ValidateRequest(request);
                handle.Invoke(handler, new object[] {request});
            }
            catch (Exception exception)
            {
                HandleException(exception.InnerException ?? exception, request, handler.GetType());
                throw exception.InnerException ?? exception;
            }
        }

        // Invoke queries with return types.
        public TResponse Send<TResponse>(IRequest<TResponse> request)
        {
            var    commandType = request.GetType();
            var    handlerType = typeof(IRequestHandler<,>).MakeGenericType(commandType, typeof(TResponse));
            object handler;

            try
            {
                handler = RequestHandlerFactory.CreateRequestHandler(handlerType);
            }
            catch
            {
                throw new Exception(string.Format("Handler for type {0} not found.", commandType));
            }

            var handle = handler.GetType().GetMethod("Handle");

            try
            {
                ValidateRequest(request);
                var result = handle.Invoke(handler, new object[] {request});

                return (TResponse) result;
            }
            catch (Exception exception)
            {
                HandleException(exception.InnerException ?? exception, request, handler.GetType());
                throw exception.InnerException ?? exception;
            }
        }

        private void HandleException(Exception exception, object request, Type handlerType)
        {
            throw new NotImplementedException();
        }

        private void ValidateRequest(object request)
        {
            ValidationContext             vc      = new ValidationContext(request);
            ICollection<ValidationResult> results = new List<ValidationResult>();
            bool                          isValid = Validator.TryValidateObject(request, vc, results, true);

            if (!isValid)
            {
                throw new Exception(string.Join(" ", results.Select(x => x.ErrorMessage)));
            }
        }
    }


    public interface IRequestHandler<in TCommand, out TResponse> where TCommand : IRequest<TResponse>
    {
        TResponse Handle(TCommand request);
    }

    public interface IRequestHandler<in TCommand>
    {
        void Handle(TCommand request);
    }
    
    public interface IRequest<out TResponse>
    {
        
    }

    public interface IRequest
    {
    }
}