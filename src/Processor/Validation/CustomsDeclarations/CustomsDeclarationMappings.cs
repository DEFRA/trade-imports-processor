namespace Defra.TradeImportsProcessor.Processor.Validation.CustomsDeclarations;

public static class CustomsDeclarationMappings
{
    public sealed record AuthorityDocumentCheck(string Name, string DocumentCode, string CheckCode);

    public static readonly List<AuthorityDocumentCheck> AuthorityDocumentChecks =
    [
        new("HMI-SMS", "N002", "H218"),
        new("HMI-GMS", "N002", "H220"),
        new("HMI-SMS", "C085", "H218"),
        new("HMI-GMS", "C085", "H220"),
        new("PHSI", "N851", "H219"),
        new("PHSI", "9115", "H219"),
        new("PHSI", "C085", "H219"),
        new("PHA-IUU", "C673", "H224"),
        new("PHA-IUU", "C641", "H224"),
        new("PHA-POAO", "N853", "H222"),
        new("PHA-FNAO", "N852", "H223"),
        new("PHA-FNAO", "C678", "H223"),
        new("APHA", "C640", "H221"),
    ];
}
