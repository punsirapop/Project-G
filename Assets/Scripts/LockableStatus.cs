// Use for lock FactorySO, FarmSO, and ContentChapterSO
public enum LockableStatus
{
    Lock,
    Unlockable,
    Unlock
}

// Use for displaying requirement for unlock things with LockableStatus
public struct UnlockRequirementData
{
    public bool IsSatisfy { get; private set; }
    public string Header { get; private set; }          // Data like: Money, Factory, Research Chapter
    public string Description { get; private set; }      // Data like: 999, Factory 1, Single-point Crossover

    public UnlockRequirementData(bool isSatisfy, string header, string description)
    {
        IsSatisfy = isSatisfy;
        Header = header;
        Description = description;
    }
}