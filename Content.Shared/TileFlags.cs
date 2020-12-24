namespace Content.Shared
{
    public enum TileFlags : ushort
    {
        // The tile is protected and cannot be written unless you are an admin and in admin mode.
        Protected = 0x0001
    }
}

