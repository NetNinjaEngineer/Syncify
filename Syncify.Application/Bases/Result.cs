﻿using System.Net;

namespace Syncify.Application.Bases;

public class Result<TSuccess>
{
    public TSuccess Value { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string Message { get; }
    public List<string>? Errors { get; }
    public HttpStatusCode StatusCode { get; }

    private Result(TSuccess value, string? message = null)
    {
        Value = value;
        IsSuccess = true;
        Message = string.IsNullOrEmpty(message) ? string.Empty : message;
        StatusCode = HttpStatusCode.OK;
    }

    private Result(HttpStatusCode statusCode, string? failureMessage = null, List<string>? errors = null)
    {
        Value = default!;
        IsSuccess = false;
        Message = string.IsNullOrEmpty(failureMessage) ? string.Empty : failureMessage;
        Errors = errors?.Count > 0 ? errors : [];
        StatusCode = statusCode;
    }

    public static Result<TSuccess> Success(TSuccess value, string? successMessage = null) => new(value, successMessage);
    public static Result<TSuccess> Failure(HttpStatusCode statusCode, string error) => new(statusCode, error);
    public static Result<TSuccess> Failure(HttpStatusCode statusCode, string? message = null,
        List<string>? errors = null) => new(statusCode, message, errors);

    public Result<TNextSuccess> Bind<TNextSuccess>(Func<TSuccess, Result<TNextSuccess>> next)
        => IsSuccess ? next(Value) : Result<TNextSuccess>.Failure(StatusCode, Message, Errors);

    public async Task<Result<TNextSuccess>> BindAsync<TNextSuccess>(Func<TSuccess, Task<Result<TNextSuccess>>> next)
        => IsSuccess ? await next(Value) : Result<TNextSuccess>.Failure(StatusCode, Message, Errors);

    public Result<TNext> Map<TNext>(Func<TSuccess, TNext> mapper)
        => IsSuccess ? Result<TNext>.Success(mapper(Value)) : Result<TNext>.Failure(StatusCode, Message, Errors);
}