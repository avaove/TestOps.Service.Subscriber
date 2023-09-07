using Ceridian.Framework.Core.ErrorHandling;
using Ceridian.Framework.Core.ErrorHandling.Enums;
using Ceridian.Framework.Core.ErrorHandling.Models;
using Moq;


namespace TestOps.Service.Subscriber.Tests.Fakes
{
    internal class ApiStatusFactoryFake : IApiStatusFactory
    {
        readonly ApiStatusFactory implementation;

        public ApiStatusFactoryFake(string messagePrefix = "Message:")
        {
            Mock<IMessageUtils> messageUtilMock = new();
            messageUtilMock.Setup(l => l.GetMessage(It.IsAny<string>())).Returns<string>(s => $"{messagePrefix}{s}");
            implementation = new(messageUtilMock.Object);
        }

        public ApiStatus Create(
            string code, 
            string fieldName = null, 
            Severity severity = Severity.ERROR, 
            Dictionary<string, string>? additionalInfo = null)
        {
           return implementation.Create(code, fieldName, severity, additionalInfo);
        }
    }
}
