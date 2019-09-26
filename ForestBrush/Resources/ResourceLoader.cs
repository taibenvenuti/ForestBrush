using ColossalFramework.IO;
using ColossalFramework.Plugins;
using ColossalFramework.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;

namespace ForestBrush.Resources
{
    //Class by SamsamTS
    public class ResourceLoader
    {
        public static string ForestBrushNormal { get; } =  "ForestBrushNormal";
        public static string ForestBrushFocused { get; } =  "ForestBrushFocused";
        public static string ForestBrushHovered { get; } =  "ForestBrushHovered";
        public static string ForestBrushPressed { get; } =  "ForestBrushPressed";
        public static string OptionsDropbox { get; } =  "OptionsDropbox";
        public static string OptionsDropboxHovered { get; } =  "OptionsDropboxHovered";
        public static string OptionsDropboxPressed { get; } =  "OptionsDropboxPressed";
        public static string OptionsDropboxFocused { get; } =  "OptionsDropboxFocused";
        public static string SettingsDropbox { get; } =  "SettingsDropbox";
        public static string SettingsDropboxHovered { get; } =  "SettingsDropboxHovered";
        public static string SettingsDropboxPressed { get; } =  "SettingsDropboxPressed";
        public static string SettingsDropboxFocused { get; } =  "SettingsDropboxFocused";
        public static string EmptySprite { get; } =  "EmptySprite";
        public static string ListItemHover { get; } = "ListItemHover";
        public static string ListItemHighlight { get; } = "ListItemHighlight";
        public static string MenuPanel { get; } = "MenuPanel";
        public static string TextFieldPanelHovered { get; } = "TextFieldPanelHovered";
        public static string WhiteRect{ get; } = "WhiteRect";
        public static string TextFieldPanel { get; internal set; } = "TextFieldPanel";
        public static string IconPolicyForest{ get; } = "IconPolicyForest";
        public static string CheckBoxSpriteUnchecked{ get; } = "AchievementCheckedFalse";
        public static string CheckBoxSpriteChecked{ get; } = "AchievementCheckedTrue";
        public static string StylesDropboxListbox{ get; } = "StylesDropboxListbox";
        public static string CMStylesDropbox{ get; } = "CMStylesDropbox";
        public static string CMStylesDropboxHovered{ get; } = "CMStylesDropboxHovered";
        public static string DeleteLineButton{ get; } = "DeleteLineButton";
        public static string DeleteLineButtonHovered{ get; } = "DeleteLineButtonHovered";
        public static string DeleteLineButtonPressed{ get; } = "DeleteLineButtonPressed";
        public static string LevelBarBackground{ get; } = "LevelBarBackground";
        public static string LevelBarForeground{ get; } = "LevelBarForeground";
        public static string ButtonMenu{ get; } = "ButtonMenu";
        public static string ButtonMenuDisabled{ get; } = "ButtonMenuDisabled";
        public static string ButtonMenuHovered{ get; } = "ButtonMenuHovered";
        public static string ButtonMenuPressed{ get; } = "ButtonMenuPressed";
        public static string OptionsDropboxListbox{ get; } = "OptionsDropboxListbox";
        public static string OptionsDropboxListboxHovered{ get; } = "OptionsDropboxListboxHovered";
        public static string OptionsDropboxListboxPressed{ get; } = "OptionsDropboxListboxPressed";
        public static string OptionsDropboxListboxFocused { get; } = "OptionsDropboxListboxFocused";
        public static string PaintBrushNormal { get; } = "PaintBrushNormal";
        public static string PaintBrushHovered { get; } = "PaintBrushHovered";
        public static string PaintBrushPressed { get; } = "PaintBrushPressed";
        public static string PaintBrushFocused { get; } = "PaintBrushFocused";
        public static string MenuContainer { get; } = "MenuContainer";
        public static string GenericPanel { get; } = "GenericPanel";
        public static string SubcategoriesPanel { get; } = "SubcategoriesPanel";

        private static Shader shader;
        public static Shader Shader
        {
            get
            {
                if (shader == null)
                    shader = LoadCustomShaderFromBundle();
                return shader;
            }
        }

        private static UITextureAtlas atlas;
        public static UITextureAtlas Atlas
        {
            get
            {
                if (atlas == null)
                {
                    atlas = GetAtlas();
                }
                return atlas;
            }
            set
            {
                atlas = value;
            }
        }

        private static UITextureAtlas forestBrushAtlas;
        internal static UITextureAtlas ForestBrushAtlas
        {
            get
            {
                if (forestBrushAtlas == null)
                {
                    forestBrushAtlas = LoadAtlas();
                }
                return forestBrushAtlas;
            }
            set
            {
                forestBrushAtlas = value;
            }
        }

        public static UITextureAtlas LoadAtlas()
        {
            UITextureAtlas atlas;

            string[] spriteNames = new string[]
            {
                ForestBrushNormal,
                ForestBrushFocused,
                ForestBrushHovered,
                ForestBrushPressed,
                OptionsDropbox,
                OptionsDropboxHovered,
                OptionsDropboxPressed,
                OptionsDropboxFocused,
                SettingsDropbox,
                SettingsDropboxHovered,
                SettingsDropboxPressed,
                SettingsDropboxFocused,
                PaintBrushNormal,
                PaintBrushHovered,
                PaintBrushPressed,
                PaintBrushFocused
            };

            atlas = CreateTextureAtlas("ForestBrushAtlas", spriteNames, "ForestBrush.Resources.");

            UITextureAtlas defaultAtlas = Atlas;

            Texture2D[] textures = new Texture2D[]
            {
                defaultAtlas["ToolbarIconGroup6Normal"].texture,
                defaultAtlas["ToolbarIconGroup6Disabled"].texture,
                defaultAtlas["ToolbarIconGroup6Focused"].texture,
                defaultAtlas["ToolbarIconGroup6Hovered"].texture,
                defaultAtlas["ToolbarIconGroup6Pressed"].texture
            };

            AddTexturesInAtlas(atlas, textures);

            return atlas;
        }

        public static UITextureAtlas CreateTextureAtlas(string atlasName, string[] spriteNames, string assemblyPath)
        {
            int maxSize = 1024;
            Texture2D texture2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            Texture2D[] textures = new Texture2D[spriteNames.Length];
            Rect[] regions = new Rect[spriteNames.Length];

            for (int i = 0; i < spriteNames.Length; i++)
                textures[i] = LoadTextureFromAssembly(assemblyPath + spriteNames[i] + ".png");

            regions = texture2D.PackTextures(textures, 2, maxSize);

            UITextureAtlas textureAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            Material material = UnityEngine.Object.Instantiate<Material>(UIView.GetAView().defaultAtlas.material);
            material.mainTexture = texture2D;
            textureAtlas.material = material;
            textureAtlas.name = atlasName;

            for (int i = 0; i < spriteNames.Length; i++)
            {
                UITextureAtlas.SpriteInfo item = new UITextureAtlas.SpriteInfo
                {
                    name = spriteNames[i],
                    texture = textures[i],
                    region = regions[i],
                };

                textureAtlas.AddSprite(item);
            }

            return textureAtlas;
        }

        public static void AddTexturesInAtlas(UITextureAtlas atlas, Texture2D[] newTextures, bool locked = false)
        {
            Texture2D[] textures = new Texture2D[atlas.count + newTextures.Length];

            for (int i = 0; i < atlas.count; i++)
            {
                Texture2D texture2D = atlas.sprites[i].texture;

                if (locked)
                {
                    // Locked textures workaround
                    RenderTexture renderTexture = RenderTexture.GetTemporary(texture2D.width, texture2D.height, 0);
                    Graphics.Blit(texture2D, renderTexture);

                    RenderTexture active = RenderTexture.active;
                    texture2D = new Texture2D(renderTexture.width, renderTexture.height);
                    RenderTexture.active = renderTexture;
                    texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
                    texture2D.Apply();
                    RenderTexture.active = active;

                    RenderTexture.ReleaseTemporary(renderTexture);
                }

                textures[i] = texture2D;
                textures[i].name = atlas.sprites[i].name;
            }

            for (int i = 0; i < newTextures.Length; i++)
                textures[atlas.count + i] = newTextures[i];

            Rect[] regions = atlas.texture.PackTextures(textures, atlas.padding, 4096, false);

            atlas.sprites.Clear();

            for (int i = 0; i < textures.Length; i++)
            {
                UITextureAtlas.SpriteInfo spriteInfo = atlas[textures[i].name];
                atlas.sprites.Add(new UITextureAtlas.SpriteInfo
                {
                    texture = textures[i],
                    name = textures[i].name,
                    border = (spriteInfo != null) ? spriteInfo.border : new RectOffset(),
                    region = regions[i]
                });
            }

            atlas.RebuildIndexes();
        }

        public static UITextureAtlas GetAtlas()
        {
            return UserMod.IsGame ? UIView.GetAView().defaultAtlas : UIView.library?.Get<OptionsMainPanel>("OptionsPanel")?.GetComponent<UIPanel>()?.atlas;
        }

        public static Texture2D LoadTextureFromAssembly(string path)
        {
            Texture2D texture2D = new Texture2D(2, 2, TextureFormat.ARGB32, false);
            try
            {
                using (Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path))
                {
                    byte[] array = new byte[manifestResourceStream.Length];
                    manifestResourceStream.Read(array, 0, array.Length);
                    texture2D.LoadImage(array);
                }
            }
            catch (System.Exception)
            {
                Debug.LogWarning("Failed to Load Texture from Assembly");
            }
            return texture2D;
        }

        private static string GetModPath()
        {
            foreach (var plugin in PluginManager.instance.GetPluginsInfo())
            {
                string path = Path.Combine(plugin.modPath, "ForestBrush.dll");
                if (File.Exists(path))
                    return plugin.modPath;
            }
            return null;
        }
        public static Dictionary<string, Texture2D> LoadBrushTextures()
        {
            Dictionary<string, Texture2D> dict = new Dictionary<string, Texture2D>();
            string path = GetModPath();
            string resourcesPath = Path.Combine(path, "Resources");
            foreach (var file in Directory.GetFiles(Path.Combine(resourcesPath, "Brushes")))
            {
                Texture2D tex = new Texture2D(1, 1);
                tex.LoadImage(File.ReadAllBytes(file));
                tex.Apply();
                string id = Path.GetFileNameWithoutExtension(file);
                if (!dict.ContainsKey(id))
                    dict.Add(id, tex);
            }
            string mapEditorPath = Path.Combine(DataLocation.addonsPath, "MapEditor");
            string brushesPath = Path.Combine(mapEditorPath, "Brushes");
            string customBrushesPath = Path.Combine(brushesPath, "ForestBrush");
            if (Directory.Exists(customBrushesPath))
            {
                foreach (var userBrush in Directory.GetFiles(customBrushesPath))
                {
                    Texture2D tex = new Texture2D(1, 1);
                    tex.LoadImage(File.ReadAllBytes(userBrush));
                    tex.Apply();
                    if (tex.width == 128 && tex.height == 128)
                    {
                        string id = Path.GetFileNameWithoutExtension(userBrush);
                        if (!dict.ContainsKey(id))
                            dict.Add(id, tex);
                    }
                }
            }
            return dict;
        }

        private static Shader LoadCustomShaderFromBundle()
        {
            string bundleName =
                Application.platform == RuntimePlatform.OSXPlayer
                ? "ForestBrush.Resources.forestbrushmac"
                : Application.platform == RuntimePlatform.LinuxPlayer
                ? "ForestBrush.Resources.forestbrushlinux"
                : "ForestBrush.Resources.forestbrush";
            byte[] bytes = ExtractResource(bundleName);
            AssetBundle shaderBundle = AssetBundle.LoadFromMemory(bytes);
            Shader shader = shaderBundle.LoadAsset<Shader>("Assets/Shader/ForestBrush.shader");
            shaderBundle.Unload(false);
            return shader;
        }

        public static byte[] ExtractResource(string filename)
        {
            Assembly a = Assembly.GetExecutingAssembly();
            using (Stream resFilestream = a.GetManifestResourceStream(filename))
            {
                if (resFilestream == null) return null;
                byte[] ba = new byte[resFilestream.Length];
                resFilestream.Read(ba, 0, ba.Length);
                return ba;
            }
        }

        public static Texture2D ConvertRenderTexture(RenderTexture renderTexture)
        {
            RenderTexture active = RenderTexture.active;
            Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height);
            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
            texture2D.Apply();
            RenderTexture.active = active;

            return texture2D;
        }

        public static void ResizeTexture(Texture2D texture, int width, int height)
        {
            RenderTexture active = RenderTexture.active;

            texture.filterMode = FilterMode.Trilinear;
            RenderTexture renderTexture = RenderTexture.GetTemporary(width, height);
            renderTexture.filterMode = FilterMode.Trilinear;

            RenderTexture.active = renderTexture;
            Graphics.Blit(texture, renderTexture);
            texture.Resize(width, height);
            texture.ReadPixels(new Rect(0, 0, width, width), 0, 0);
            texture.Apply();

            RenderTexture.active = active;
            RenderTexture.ReleaseTemporary(renderTexture);
        }

        public static void CopyTexture(Texture2D texture2D, Texture2D dest)
        {
            RenderTexture renderTexture = RenderTexture.GetTemporary(texture2D.width, texture2D.height, 0);
            Graphics.Blit(texture2D, renderTexture);

            RenderTexture active = RenderTexture.active;
            RenderTexture.active = renderTexture;
            dest.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
            dest.Apply();
            RenderTexture.active = active;

            RenderTexture.ReleaseTemporary(renderTexture);
        }
    }
}
