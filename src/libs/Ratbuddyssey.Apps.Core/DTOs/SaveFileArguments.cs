namespace Ratbuddyssey.DTOs;

public readonly record struct SaveFileArguments(
    string FileName,
    string Extension,
    string FilterName,
    Func<Task<byte[]>> BytesFunc);
