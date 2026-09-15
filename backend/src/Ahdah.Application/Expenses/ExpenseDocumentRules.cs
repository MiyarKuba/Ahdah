namespace Ahdah.Application.Expenses;

public static class ExpenseDocumentRules
{
    // A null boundary means the existing approval policy does not require a document.
    public static decimal? RequiredFromAmount(string? mode, decimal? threshold) => mode switch
    {
        "Always" => 0m,
        "Threshold" => threshold,
        _ => null
    };

    public static bool IsRequired(string? mode, decimal? threshold, decimal amount) =>
        RequiredFromAmount(mode, threshold) is { } boundary && amount >= boundary;
}
