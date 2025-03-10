using Luna.PKG;

namespace Luna.PKG.Tests;

[TestClass]
public sealed class PackageTests
{
    [TestMethod]
    public void TestMakePackageAndRead()
    {
        PackageWriter.MakePkg("assets.pkg", "Assets");
        Assert.IsTrue(File.Exists("assets.pkg"));
        var pkg = new PackageReader("assets.pkg");
        Assert.IsTrue(pkg.Assets.Count > 0);
        var txtFile = pkg.GetAsset("Assets/test.txt");
        Assert.IsNotNull(txtFile);
        var spriteFile = pkg.GetAsset("Assets/SprItes/sprite.png");
        Assert.IsNotNull(spriteFile);
        Assert.IsTrue(spriteFile.Length == 796);
        Assert.ThrowsException<Exception>(() => pkg.GetAsset("Assets/doesnotexist.txt"));
        Assert.ThrowsException<FileNotFoundException>(() => new PackageReader("doesnotexist.pkg"));
    }
}
