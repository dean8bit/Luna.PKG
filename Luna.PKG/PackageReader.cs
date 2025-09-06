using System.Text;

namespace Luna.PKG;

public class PackageReader
{
    public List<PackedAsset> Assets { get; private set; } = [];
    private readonly string _path;
    private readonly BinaryReader _reader;

    public PackageReader(string path)
    {
        _path = path;
        _reader = new BinaryReader(File.Open(path, FileMode.Open));
        byte[] magic = _reader.ReadBytes(3);
        if (Encoding.ASCII.GetString(magic) != "PKG")
        {
            throw new Exception("Invalid package file");
        }

        long index = _reader.ReadInt64(); //index to file table
        int size = _reader.ReadInt32(); //size of file table

        _ = _reader.BaseStream.Seek(index, SeekOrigin.Begin);
        for (int i = 0; i < size; i++)
        {
            PackedAsset asset = new(_reader.ReadString(), _reader.ReadInt64(), _reader.ReadInt32());
            Assets.Add(asset);
        }
    }

    public byte[] GetAsset(string path)
    {
        PackedAsset asset = Assets.FirstOrDefault(a => a.Path.Equals(path, StringComparison.OrdinalIgnoreCase)) ?? throw new Exception($"Asset {path} not found in package {_path}");
        _ = _reader.BaseStream.Seek(asset.Start, SeekOrigin.Begin);
        return _reader.ReadBytes(asset.Size);
    }

    ~PackageReader()
    {
        _reader?.Dispose();
    }
}