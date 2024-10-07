## Project Structure
- Assets are grouped in object oriented style.
- Prefabs are central figures, all necessary assets should be at the same folder as created prefab or in specific subfolders.
- All assets which are common for more than one prefab are placed in !Common folder.
- Game always starts from Bootstrap scene. All \*system stuff\* are placed there.
## Asset Naming Convention
- NoSpaceInNames.
- Scripts, scenes and prefabs should not use prefixes.
- [AssetTypePrefix]\_[AssetName]\_[Descriptor]_[OptionalVariantLetterOrNumber]
- Table of common prefixes:
  
| Asset | Prefix |
| ------ | ------ |
| T | Texture |
| M | Material |
| S | Audio Clip |
| A | Animation Clip |
| SM | Static Mesh |

- If there no suffix in textures it means that it's albedo texture.