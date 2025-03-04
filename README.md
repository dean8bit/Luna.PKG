## Luna.PKG

An asset packer and reader. 

Packs an asset folder into a single file and maintains a file table appended on to the end.


### Example usage:


      PackageWriter.MakePkg("assets.pkg", "assets"); -- Writes contents of assets folder into assets.pkg
  
      var reader = PackageReader("assets.pkg"); -- Reads a package file
  
      byte[] sprite = reader.GetAsset("assets/sprites/sprite.png") -- Reads byte data for asset
      //THROWS - throw new Exception($"Asset {path} not found in package {_path}");

---

### Pack data file structure

[1] Header Data

- 3 bytes ("PKG", Magic string) 
  
- 8 bytes (Int64, Index of the file table)
  
- 8 bytes (Int64, Count of assets)


[...] File data table
  
- File contents appended as byte[]
  

[...] Asset Table
  
  - length prefixed string (individual asset path)
    
  - 8 bytes (Int64, index to the start of the asset data in the file data table)
    
  - 4 bytes (Int32, size of the data block for the asset data)

---

-- Used as part of a personal game engine. Extracted this module out as an MIT licensed tool.
