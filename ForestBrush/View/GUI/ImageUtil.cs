using ColossalFramework;
using ColossalFramework.UI;
using UnityEngine;

namespace ForestBrush.GUI
{
    //Class by SamsamTS
    public class ImageUtils
    {
        private static PreviewRenderer m_previewRenderer;
        private static Texture2D focusedFilterTexture;

        public static bool CreateThumbnailAtlas(string name, PrefabInfo prefab)
        {
            if (name.IsNullOrWhiteSpace() || prefab == null) return false;
            if (prefab.m_Thumbnail == name) return true;

            if (m_previewRenderer == null)
            {
                m_previewRenderer = ForestBrush.Instance.gameObject.AddComponent<PreviewRenderer>();
                m_previewRenderer.size = new Vector2(109, 100) * 2f;
            }

            m_previewRenderer.cameraRotation = 210f;
            m_previewRenderer.zoom = 4f;

            bool rendered = false;

            TreeInfo treePrefab = prefab as TreeInfo;
            if (treePrefab != null)
            {
                m_previewRenderer.mesh = treePrefab.m_mesh;
                m_previewRenderer.material = treePrefab.m_material;

                if (m_previewRenderer.mesh != null)
                {
                    m_previewRenderer.Render();
                    rendered = true;
                }
            }

            if (rendered)
            {
                Texture2D texture = Resources.ResourceLoader.ConvertRenderTexture(m_previewRenderer.texture);
                texture.name = name;

                prefab.m_Thumbnail = name;
                prefab.m_Atlas = Resources.ResourceLoader.CreateTextureAtlas("ForestBrushThumbnails_" + name, new string[] { }, null);

                Resources.ResourceLoader.ResizeTexture(texture, 109, 100);
                Resources.ResourceLoader.AddTexturesInAtlas(prefab.m_Atlas, GenerateMissingThumbnailVariants(texture));

                Debug.Log("Generated thumbnails for: " + name);
            }
            else
            {
                prefab.m_Thumbnail = "ThumbnailBuildingDefault";
            }

            return rendered;
        }

        public static void AddThumbnailVariantsInAtlas(PrefabInfo prefab)
        {
            Texture2D texture = prefab.m_Atlas[prefab.m_Thumbnail].texture;
            prefab.m_Atlas = Resources.ResourceLoader.CreateTextureAtlas("ForestBrushThumbnails_" + prefab.m_Thumbnail, new string[] { }, null);

            Resources.ResourceLoader.AddTexturesInAtlas(prefab.m_Atlas, GenerateMissingThumbnailVariants(texture));

            Debug.Log("Generated thumbnails variants for: " + prefab.name);
        }
        

        public static void ScaleTexture(Texture2D tex, int width, int height)
        {
            tex.filterMode = FilterMode.Trilinear;
            var newPixels = new Color[width * height];
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    newPixels[y * width + x] = tex.GetPixelBilinear(((float)x) / width, ((float)y) / height);
                }
            }
            tex.Resize(width, height);
            tex.SetPixels(newPixels);
            tex.Apply();
        }

        public static void ScaleTexture2(Texture2D tex, int width, int height)
        {
            var newPixels = new Color[width * height];

            float ratio = ((float)width) / tex.width;
            if (tex.height * ratio > height)
            {
                ratio = ((float)height) / tex.height;
            }

            if (ratio > 1f) ratio = 1f;

            int newW = Mathf.RoundToInt(tex.width * ratio);
            int newH = Mathf.RoundToInt(tex.height * ratio);

            ScaleTexture(tex, newW, newH);
        }

        public static void CropTexture(Texture2D tex, int x, int y, int width, int height)
        {
            var newPixels = tex.GetPixels(x, y, width, height);
            tex.Resize(width, height);
            tex.SetPixels(newPixels);
            tex.Apply();
        }

        // Colorize the focused icon blue using the LUT texture
        // Use a border of 8 (256/32) to ensure we don't pick up neighboring patches
        private static Color32 ColorizeFocused(Color32 c)
        {
            if (focusedFilterTexture == null)
            {
                focusedFilterTexture = Resources.ResourceLoader.LoadTextureFromAssembly("ForestBrush.Resources.SelectFilter.png");
            }

            int b = c.b * 31 / 255;
            float u = ((8f + (float)c.r) / 271) / 32 + ((float)b / 32);
            float v = 1f - ((8f + (float)c.g) / 271);
            Color32 result = focusedFilterTexture.GetPixelBilinear(u, v);
            result.a = c.a;
            return result;
        }

        public static bool FixFocusedTexture(PrefabInfo prefab)
        {
            if (prefab == null || prefab.m_Atlas == null || prefab.m_Thumbnail.IsNullOrWhiteSpace()) return false;

            UITextureAtlas.SpriteInfo sprite = prefab.m_Atlas[prefab.m_Thumbnail + "Focused"];
            if (sprite != null)
            {
                Color32[] pixels = sprite.texture.GetPixels32();

                int count = 0;

                foreach (Color32 pixel in pixels)
                {
                    if (pixel.a > 127 && (pixel.r + pixel.g + pixel.b) > 0)
                    {
                        Color.RGBToHSV(pixel, out float h, out float s, out float v);

                        if (h < 0.66f || h > 0.68f || s < 0.98f)
                        {
                            return false;
                        }

                        if (++count > 32)
                        {
                            break;
                        }
                    }
                }

                if (count > 0)
                {
                    ImageUtils.FixFocusedTexture(prefab.m_Atlas[prefab.m_Thumbnail].texture, sprite.texture);
                    Color32[] colors = sprite.texture.GetPixels32();

                    prefab.m_Atlas.texture.SetPixels32((int)(sprite.region.x * prefab.m_Atlas.texture.width), (int)(sprite.region.y * prefab.m_Atlas.texture.height), sprite.texture.width, sprite.texture.height, colors);
                    prefab.m_Atlas.texture.Apply();

                    return true;
                }
            }

            return false;
        }

        public static void RefreshAtlas(UITextureAtlas atlas)
        {
            Texture2D[] textures = new Texture2D[atlas.sprites.Count];

            int i = 0;
            foreach (UITextureAtlas.SpriteInfo sprite in atlas.sprites)
            {
                textures[i++] = sprite.texture;
            }
            atlas.AddTextures(textures);
        }

        public static void FixFocusedTexture(Texture2D baseTexture, Texture2D focusedTexture)
        {
            var newPixels = new Color32[baseTexture.width * baseTexture.height];
            var pixels = baseTexture.GetPixels32();

            ApplyFilter(pixels, newPixels, ColorizeFocused);
            focusedTexture.SetPixels32(newPixels);
            focusedTexture.Apply(false);
        }

        // Our own version of this as the one in AssetImporterThumbnails has hardcoded dimensions
        // and generates ugly dark blue focused thumbnails.
        public static Texture2D[] GenerateMissingThumbnailVariants(Texture2D baseTexture)
        {
            var newPixels = new Color32[baseTexture.width * baseTexture.height];
            var pixels = baseTexture.GetPixels32();

            ApplyFilter(pixels, newPixels, ColorizeFocused);
            Texture2D focusedTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.ARGB32, false, false);
            focusedTexture.SetPixels32(newPixels);
            focusedTexture.Apply(false);
            focusedTexture.name = baseTexture.name + "Focused";

            ApplyFilter(pixels, newPixels, c => new Color32((byte)(128 + c.r / 2), (byte)(128 + c.g / 2), (byte)(128 + c.b / 2), c.a));
            Texture2D hoveredTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.ARGB32, false, false);
            hoveredTexture.SetPixels32(newPixels);
            hoveredTexture.Apply(false);
            hoveredTexture.name = baseTexture.name + "Hovered";

            ApplyFilter(pixels, newPixels, c => new Color32((byte)(192 + c.r / 4), (byte)(192 + c.g / 4), (byte)(192 + c.b / 4), c.a));
            Texture2D pressedTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.ARGB32, false, false);
            pressedTexture.SetPixels32(newPixels);
            pressedTexture.Apply(false);
            pressedTexture.name = baseTexture.name + "Pressed";

            ApplyFilter(pixels, newPixels, c => new Color32(0, 0, 0, c.a));
            Texture2D disabledTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.ARGB32, false, false);
            disabledTexture.SetPixels32(newPixels);
            disabledTexture.Apply(false);
            disabledTexture.name = baseTexture.name + "Disabled";

            return new Texture2D[]
            {
                baseTexture,
                focusedTexture,
                hoveredTexture,
                pressedTexture,
                disabledTexture
            };
        }

        delegate Color32 Filter(Color32 c);

        private static void ApplyFilter(Color32[] src, Color32[] dst, Filter filter)
        {
            for (int i = 0; i < src.Length; i++)
            {
                dst[i] = filter(src[i]);
            }
        }
    }
}
