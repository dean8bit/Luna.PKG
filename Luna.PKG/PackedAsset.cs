namespace Luna.PKG;

public class PackedAsset(string path, Int64 start, Int32 size)
{
  public string Path { get; set; } = path;
  public Int64 Start { get; set; } = start;
  public Int32 Size { get; set; } = size;
}