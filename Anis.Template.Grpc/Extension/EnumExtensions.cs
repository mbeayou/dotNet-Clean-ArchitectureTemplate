using Anis.Template.Domain.Enums;
using Anis.Template.Domain.Exceptions;
using Anis.Template.Grpc.Protos;
using System;
using GrpcCore = Grpc.Core;

namespace Anis.Template.Grpc.Extensions
{
    public static class EnumExtensions
    {
        public static GrpcCore.StatusCode ToRpcStatuseCode(this ExceptionStatusCode exceptionStatusCode)
            => exceptionStatusCode switch
            {
                ExceptionStatusCode.OK => GrpcCore.StatusCode.OK,
                ExceptionStatusCode.Cancelled => GrpcCore.StatusCode.Cancelled,
                ExceptionStatusCode.Unknown => GrpcCore.StatusCode.Unknown,
                ExceptionStatusCode.InvalidArgument => GrpcCore.StatusCode.InvalidArgument,
                ExceptionStatusCode.DeadlineExceeded => GrpcCore.StatusCode.DeadlineExceeded,
                ExceptionStatusCode.NotFound => GrpcCore.StatusCode.NotFound,
                ExceptionStatusCode.AlreadyExists => GrpcCore.StatusCode.AlreadyExists,
                ExceptionStatusCode.PermissionDenied => GrpcCore.StatusCode.PermissionDenied,
                ExceptionStatusCode.ResourceExhausted => GrpcCore.StatusCode.ResourceExhausted,
                ExceptionStatusCode.FailedPrecondition => GrpcCore.StatusCode.FailedPrecondition,
                ExceptionStatusCode.Aborted => GrpcCore.StatusCode.Aborted,
                ExceptionStatusCode.OutOfRange => GrpcCore.StatusCode.OutOfRange,
                ExceptionStatusCode.Unimplemented => GrpcCore.StatusCode.Unimplemented,
                ExceptionStatusCode.Internal => GrpcCore.StatusCode.Internal,
                ExceptionStatusCode.Unavailable => GrpcCore.StatusCode.Unavailable,
                ExceptionStatusCode.DataLoss => GrpcCore.StatusCode.DataLoss,
                ExceptionStatusCode.Unauthenticated => GrpcCore.StatusCode.Unauthenticated,
                _ => throw new NotImplementedException("StatusCode not implemented ."),
            };
    }
}
