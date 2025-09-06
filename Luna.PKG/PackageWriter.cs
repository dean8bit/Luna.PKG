using System.Text;

namespace Luna.PKG;

public class PackageWriter
{
    public List<PackedAsset> Assets { get; } = [];

    public static void MakePkg(string outputPath, string assetPath)
    {

        PackageWriter pkg = new();
        string[] files = Directory.GetFiles(assetPath, "*.*", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            string path = file.Replace("\\", "/").Replace("//", "/");
            Console.WriteLine($"Adding {path}");
            pkg.AddAsset(path);
        }
        pkg.Write(outputPath);
    }

    public void Write(string path)
    {
        using BinaryWriter writer = new(File.Open(path, FileMode.Create));
        writer.Write(Encoding.ASCII.GetBytes("PKG"));
        writer.Write((long)0); //index to file table
        writer.Write((long)0); //size of file table

        foreach (PackedAsset asset in Assets)
        {
            byte[] data = LoadData(asset);
            asset.Size = data.Length;
            asset.Start = writer.BaseStream.Position;
            writer.Write(data);
        }

        int index = (int)writer.BaseStream.Position;
        foreach (PackedAsset asset in Assets)
        {
            writer.Write(asset.Path);
            writer.Write(asset.Start);
            writer.Write(asset.Size);
        }

        _ = writer.BaseStream.Seek(3, SeekOrigin.Begin);
        writer.Write((long)index);
        writer.Write((long)Assets.Count);
    }

    public void AddAsset(string path)
    {
        if (Assets.Any(a => a.Path == path))
        {
            return;
        }

        Assets.Add(new PackedAsset(path, 0, 0));
    }

    public byte[] LoadData(PackedAsset asset)
    {
        using BinaryReader reader = new(File.Open(asset.Path, FileMode.Open));
        return reader.ReadBytes((int)reader.BaseStream.Length);
    }
}
