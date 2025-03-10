using System.Text;

namespace Luna.PKG;

public class PackageReader
{
  public List<PackedAsset> Assets { get; private set; } = [];
  private string _path;
  private BinaryReader _reader;

  public PackageReader(string path)
  {
    _path = path;
    _reader = new BinaryReader(File.Open(path, FileMode.Open));
    var magic = _reader.ReadBytes(3);
    if (Encoding.ASCII.GetString(magic) != "PKG") throw new Exception("Invalid package file");
    var index = _reader.ReadInt64(); //index to file table
    var size = _reader.ReadInt32(); //size of file table

    _reader.BaseStream.Seek(index, SeekOrigin.Begin);
    for (var i = 0; i < size; i++)
    {
      var asset = new PackedAsset(_reader.ReadString(), _reader.ReadInt64(), _reader.ReadInt32());
      Assets.Add(asset);
    }
  }

  public byte[] GetAsset(string path)
  {
    var asset = Assets.FirstOrDefault(a => a.Path.Equals(path, StringComparison.OrdinalIgnoreCase)) ?? throw new Exception($"Asset {path} not found in package {_path}");
    _reader.BaseStream.Seek(asset.Start, SeekOrigin.Begin);
    return _reader.ReadBytes(asset.Size);
  }

  ~PackageReader()
  {
    _reader?.Dispose();
  }
}