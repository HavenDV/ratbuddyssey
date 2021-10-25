namespace Ratbuddyssey.DTOs;

public readonly record struct OpenFileArguments(
    string FileName,
    string[] Extensions,
    string FilterName);
