namespace Bimcer.TaskManagement.Service.Core.Exceptions;

public sealed class BusinessException(string message) : Exception(message);
