﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;

namespace UOStudio.Tools.TextureAtlasGenerator.Ultima
{
    public sealed class Files
    {
        public delegate void FileSaveHandler();
        public static event FileSaveHandler FileSaveEvent;

        public static void FireFileSaveEvent()
        {
            FileSaveEvent?.Invoke();
        }

        /// <summary>
        /// Should loaded Data be cached
        /// </summary>
        public static bool CacheData { get; set; } = true;

        /// <summary>
        /// Should a Hash file be used to speed up loading
        /// </summary>
        public static bool UseHashFile { get; set; }

        /// <summary>
        /// Contains the path infos
        /// </summary>
        public static Dictionary<string, string> MulPath { get; set; }

        /// <summary>
        /// Gets a list of paths to the Client's data files.
        /// </summary>
        public static string Directory { get; private set; }

        /// <summary>
        /// Contains the rootDir (so relative values are possible for <see cref="MulPath"/>
        /// </summary>
        public static string RootDir { get; set; }

        private static readonly string[] _uoFiles = {
            "anim.idx",
            "anim.mul",
            "anim2.idx",
            "anim2.mul",
            "anim3.idx",
            "anim3.mul",
            "anim4.idx",
            "anim4.mul",
            "anim5.idx",
            "anim5.mul",
            "animdata.mul",
            "art.mul",
            "artidx.mul",
            "artlegacymul.uop",
            "body.def",
            "bodyconv.def",
            "client.exe",
            "cliloc.custom1",
            "cliloc.custom2",
            "cliloc.deu",
            "cliloc.enu",
            "equipconv.def",
            "facet00.mul",
            "facet01.mul",
            "facet02.mul",
            "facet03.mul",
            "facet04.mul",
            "facet05.mul",
            "fonts.mul",
            "gump.def",
            "gumpart.mul",
            "gumpidx.mul",
            "gumpartlegacymul.uop",
            "hues.mul",
            "light.mul",
            "lightidx.mul",
            "map0.mul",
            "map1.mul",
            "map2.mul",
            "map3.mul",
            "map4.mul",
            "map5.mul",
            "map0legacymul.uop",
            "map1legacymul.uop",
            "map2legacymul.uop",
            "map3legacymul.uop",
            "map4legacymul.uop",
            "map5legacymul.uop",
            "mapdif0.mul",
            "mapdif1.mul",
            "mapdif2.mul",
            "mapdif3.mul",
            "mapdif4.mul",
            "mapdifl0.mul",
            "mapdifl1.mul",
            "mapdifl2.mul",
            "mapdifl3.mul",
            "mapdifl4.mul",
            "mobtypes.txt",
            "multi.idx",
            "multi.mul",
            "multimap.rle",
            "radarcol.mul",
            "skillgrp.mul",
            "skills.idx",
            "skills.mul",
            "sound.def",
            "sound.mul",
            "soundidx.mul",
            "soundlegacymul.uop",
            "speech.mul",
            "stadif0.mul",
            "stadif1.mul",
            "stadif2.mul",
            "stadif3.mul",
            "stadif4.mul",
            "stadifi0.mul",
            "stadifi1.mul",
            "stadifi2.mul",
            "stadifi3.mul",
            "stadifi4.mul",
            "stadifl0.mul",
            "stadifl1.mul",
            "stadifl2.mul",
            "stadifl3.mul",
            "stadifl4.mul",
            "staidx0.mul",
            "staidx1.mul",
            "staidx2.mul",
            "staidx3.mul",
            "staidx4.mul",
            "staidx5.mul",
            "statics0.mul",
            "statics1.mul",
            "statics2.mul",
            "statics3.mul",
            "statics4.mul",
            "statics5.mul",
            "texidx.mul",
            "texmaps.mul",
            "tiledata.mul",
            "unifont.mul",
            "unifont1.mul",
            "unifont2.mul",
            "unifont3.mul",
            "unifont4.mul",
            "unifont5.mul",
            "unifont6.mul",
            "unifont7.mul",
            "unifont8.mul",
            "unifont9.mul",
            "unifont10.mul",
            "unifont11.mul",
            "unifont12.mul",
            "uotd.exe",
            "verdata.mul"
        };

        public static void Initialize(string uoPath)
        {
            Directory = uoPath;
            LoadMulPath();
        }

        /// <summary>
        /// Fills <see cref="MulPath"/> with <see cref="Files.Directory"/>
        /// </summary>
        public static void LoadMulPath()
        {
            MulPath = new Dictionary<string, string>();
            RootDir = Directory ?? string.Empty;

            foreach (var file in _uoFiles)
            {
                var filePath = Path.Combine(RootDir, file);

                MulPath[file] = File.Exists(filePath) ? file : string.Empty;
            }
        }

        /// <summary>
        /// ReSets <see cref="MulPath"/> with given path
        /// </summary>
        /// <param name="path"></param>
        public static void SetMulPath(string path)
        {
            RootDir = path;

            foreach (var file in _uoFiles)
            {
                string filePath;

                // file was set
                if (!string.IsNullOrEmpty(MulPath[file]))
                {
                    // and was relative like "art.mul"
                    if (string.IsNullOrEmpty(Path.GetDirectoryName(MulPath[file])))
                    {
                        filePath = Path.Combine(RootDir, MulPath[file]);
                        if (File.Exists(filePath))
                        {
                            MulPath[file] = filePath;
                            continue;
                        }
                    }
                    else
                    {
                        // absolute dir
                        // ignore because someone might want custom path for individual file
                        continue;
                    }
                }

                // file was not set, or relative and non existent
                filePath = Path.Combine(RootDir, file);
                MulPath[file] = File.Exists(filePath) ? filePath : string.Empty;
            }
        }

        /// <summary>
        /// Sets <see cref="MulPath"/> key to path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="key"></param>
        public static void SetMulPath(string path, string key)
        {
            MulPath[key] = path;
        }

        /// <summary>
        ///     Looks up a given <paramref name="file" /> in <see cref="Files.MulPath" />
        /// </summary>
        /// <returns>
        ///     The absolute path to <paramref name="file" /> -or- <c>null</c> if <paramref name="file" /> was not found.
        /// </returns>
        public static string GetFilePath(string file)
        {
            if (MulPath.Count == 0)
            {
                return null;
            }

            var path = string.Empty;

            if (MulPath.ContainsKey(file.ToLower()))
            {
                path = MulPath[file.ToLower()];
            }

            if (string.IsNullOrEmpty(path))
            {
                return null;
            }

            if (string.IsNullOrEmpty(Path.GetDirectoryName(path)))
            {
                path = Path.Combine(RootDir, path);
            }

            return File.Exists(path) ? path : null;
        }

        internal static string GetFilePath(string format, params object[] args)
        {
            return GetFilePath(string.Format(format, args));
        }

        /// <summary>
        /// Compares given MD5 hash with hash of given file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="hash"></param>
        /// <returns></returns>
        public static bool CompareMD5(string file, string hash)
        {
            if (file == null)
            {
                return false;
            }

            var fileCheck = File.OpenRead(file);
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                var md5Hash = md5.ComputeHash(fileCheck);
                fileCheck.Close();
                var md5String = BitConverter.ToString(md5Hash).Replace("-", "").ToLower();
                return md5String == hash;
            }
        }

        /// <summary>
        /// Returns MD5 hash from given file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static byte[] GetMD5(string file)
        {
            if (file == null)
            {
                return null;
            }

            var fileCheck = File.OpenRead(file);
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                var md5Hash = md5.ComputeHash(fileCheck);
                fileCheck.Close();
                return md5Hash;
            }
        }

        /// <summary>
        /// Compares MD5 hash from given mul file with hash in responsible hash-file
        /// </summary>
        /// <param name="what"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool CompareHashFile(string what, string path)
        {
            var fileName = Path.Combine(path, $"UOFiddler{what}.hash");
            if (!File.Exists(fileName))
            {
                return false;
            }

            try
            {
                using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (fs.Length == 0)
                    {
                        return false; // If file is empty there is nothing to compare
                    }

                    using (var bin = new BinaryReader(fs))
                    {
                        var length = bin.ReadInt32();
                        var buffer = new byte[length];
                        bin.Read(buffer, 0, length);

                        var hashOld = BitConverter.ToString(buffer).Replace("-", "").ToLower();

                        return CompareMD5(GetFilePath($"{what}.mul"), hashOld);
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if map1.mul exists and sets <see cref="Ultima.Map"/>
        /// </summary>
        public static void CheckForNewMapSize()
        {
            if (GetFilePath("map1.mul") != null)
            {
                Map.Trammel = Map.Trammel.Width == 7168
                    ? new Map(1, 1, 7168, 4096)
                    : new Map(1, 1, 6144, 4096);
            }
            else
            {
                Map.Trammel = Map.Trammel.Width == 7168
                    ? new Map(0, 1, 7168, 4096)
                    : new Map(0, 1, 6144, 4096);
            }
        }
    }
}
