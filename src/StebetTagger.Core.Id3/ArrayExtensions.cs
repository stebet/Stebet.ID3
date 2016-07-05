namespace StebetTagger.Core.Id3
{
    public static class ArrayExtensions
    {
        public static uint ReadUInt32(this byte[] array, int offset) => ((uint)array[offset] << 24) + ((uint)array[offset + 1] << 16) + ((uint)array[offset + 2] << 8) + array[offset + 3];
    }
}
