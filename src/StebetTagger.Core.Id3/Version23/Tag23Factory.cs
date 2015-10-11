namespace StebetTagger.Core.Id3
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;
    using System.Threading.Tasks;

    /// <summary>
    /// This class creates instances of ID3 frames based on the V2.4 specification from a byte array.
    /// </summary>
    public static class Tag23Factory
    {
        static AssemblyCatalog catalog = new AssemblyCatalog(System.Reflection.Assembly.GetExecutingAssembly());
        static CompositionContainer container = new CompositionContainer(catalog);

        public static async Task<Tag> FromStream(Stream stream, int tagLength)
        {
            Tag newTag = new Tag();

            int peek = stream.ReadByte();
            while (stream.Position < tagLength && peek != 0x00)
            {
                stream.Seek(-1, SeekOrigin.Current);
                Frame23Header frameHeader = await Frame23Header.FromStream(stream);

                Frame tag = container.GetExportedValueOrDefault<Frame>(frameHeader.Id);
                if (tag != null)
                {
                    var frameBytes = new byte[frameHeader.Size];
                    await stream.ReadAsync(frameBytes, 0, frameBytes.Length);
                    tag.FromBytes(frameBytes, frameHeader.Size, TagVersion.V23);
                    newTag.Tags.Add(tag);
                }
                else
                {
                    Debug.WriteLine("Unable to read tag {0}", frameHeader.Id);
                    stream.Seek(frameHeader.Size, SeekOrigin.Current);
                }

                peek = stream.ReadByte();
            }

            return newTag;
        }
    }
}