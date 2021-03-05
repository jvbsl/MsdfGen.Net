namespace MsdfGen
{
    public enum MsdfMode : uint
    {
        Single,
        Pseudo,
        Multi,
        MultiAndTrue,
        Metrics
    }

    public enum Format : uint
    {
        Auto,
        Png,
        Bmp,
        Tiff,
        Text,
        TextFloat,
        Binary,
        BinaryFloat,
        BinaryFloatBe
    }
    public enum MsdfColoringStrategy : uint {
        Simple,
        InkTrap
    };
}