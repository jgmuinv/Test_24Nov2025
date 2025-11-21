namespace Contratos.Login;

public sealed record LoginResponse(
    bool   Ok,
    string? Token,
    string? Usuario,
    string? Error
);