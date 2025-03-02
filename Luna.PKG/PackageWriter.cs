using System.Text;

namespace Luna.PKG;

public class PackageWriter
{
  public List<PackedAsset> Assets { get; } = [];

  public static void MakePkg(string outputPath, string assetPath)
  {

    var pkg = new PackageWriter();
    var files = Directory.GetFiles(assetPath, "*.*", SearchOption.AllDirectories);
    foreach (var file in files)
    {
      var path = file.Replace("\\", "/").Replace("//", "/");
      Console.WriteLine($"Adding {path}");
      pkg.AddAsset(path);
    }
    pkg.Write(outputPath);
  }

  public void Write(string path)
  {
    using BinaryWriter writer = new(File.Open(path, FileMode.Create));
    writer.Write(Encoding.ASCII.GetBytes("PKG"));
    writer.Write((Int64)0); //index to file table
    writer.Write((Int64)0); //size of file table

    foreach (var asset in Assets)
    {
      var data = LoadData(asset);
      asset.Size = data.Length;
      asset.Start = writer.BaseStream.Position;
      writer.Write(data);
    }

    var index = (int)writer.BaseStream.Position;
    foreach (var asset in Assets)
    {
      writer.Write(asset.Path);
      writer.Write((Int64)asset.Start);
      writer.Write((Int32)asset.Size);
    }

    writer.BaseStream.Seek(3, SeekOrigin.Begin);
    writer.Write((Int64)index);
    writer.Write((Int64)Assets.Count);
  }

  public void AddAsset(string path)
  {
    if (Assets.Any(a => a.Path == path)) return;
    Assets.Add(new PackedAsset(path, 0, 0));
  }

  public byte[] LoadData(PackedAsset asset)
  {
    using BinaryReader reader = new(File.Open(asset.Path, FileMode.Open));
    return reader.ReadBytes((int)reader.BaseStream.Length);
  }
}
