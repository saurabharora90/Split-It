using Split_It.Utils;

namespace Split_It.Model.Enum
{
    public enum ExpenseSplit
    {
        [Display("EQUALLY")]
        EQUALLY,
        [Display("UNEQUALLY")]
        UNEQUALLY,
        [Display("Shares")]
        SHARES,
        [Display("They owe the full amount")]
        THEY_OWE,
        [Display("You owe the full amount")]
        YOU_OWE
    }
}
