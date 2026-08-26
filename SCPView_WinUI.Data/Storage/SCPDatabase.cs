using Microsoft.Data.Sqlite;
using SCPView_WinUI.Data.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SCPView_WinUI.Data.Storage
{
    public class SCPDatabase : IDisposable
    {
        private readonly SqliteConnection _connection;
        private static readonly string DbPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "SCPView_WinUI",
            "scp_cache.db");

        public SCPDatabase()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DbPath)!);
            _connection = new SqliteConnection($"Data Source={DbPath}");
            _connection.Open();
            Initialize();
        }

        private void Initialize()
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = @"
                CREATE TABLE IF NOT EXISTS cache_items (
                    url TEXT PRIMARY KEY,
                    name TEXT,
                    safe_level TEXT,
                    special_measures TEXT,
                    contents TEXT,
                    collapsible_contents TEXT,
                    blockquote_contents TEXT,
                    image_urls TEXT,
                    tables TEXT,
                    cached_at TEXT NOT NULL
                )";
            cmd.ExecuteNonQuery();

            var cmd2 = _connection.CreateCommand();
            cmd2.CommandText = @"
                CREATE TABLE IF NOT EXISTS cache_lists (
                    url TEXT PRIMARY KEY,
                    data TEXT NOT NULL,
                    cached_at TEXT NOT NULL
                )";
            cmd2.ExecuteNonQuery();

            string[] alterColumns = {
                "ALTER TABLE cache_items ADD COLUMN page_type TEXT DEFAULT 'Standard'",
                "ALTER TABLE cache_items ADD COLUMN hub_links TEXT DEFAULT '[]'",
                "ALTER TABLE cache_items ADD COLUMN footnotes TEXT DEFAULT '[]'",
                "ALTER TABLE cache_items ADD COLUMN content_blocks TEXT DEFAULT '[]'",
                "ALTER TABLE cache_items ADD COLUMN sub_page_urls TEXT DEFAULT '[]'"
            };
            foreach (var sql in alterColumns)
            {
                try
                {
                    var alterCmd = _connection.CreateCommand();
                    alterCmd.CommandText = sql;
                    alterCmd.ExecuteNonQuery();
                }
                catch { }
            }
        }

        public SCPItem? Get(string url)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = @"
                SELECT name, safe_level, special_measures, contents,
                       collapsible_contents, blockquote_contents, image_urls, tables,
                       page_type, hub_links, footnotes, content_blocks, sub_page_urls
                FROM cache_items WHERE url = @url";
            cmd.Parameters.AddWithValue("@url", url);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new SCPItem
            {
                Name = reader.GetString(0),
                SafeLevel = reader.GetString(1),
                SpecialMeasures = reader.GetString(2),
                Contents = reader.GetString(3),
                CollapsibleContents = JsonSerializer.Deserialize<List<CollapsibleContent>>(reader.GetString(4)) ?? new(),
                BlockQuoteContents = JsonSerializer.Deserialize<List<BlockQuoteContent>>(reader.GetString(5)) ?? new(),
                ImageUrls = JsonSerializer.Deserialize<List<string>>(reader.GetString(6)) ?? new(),
                Tables = JsonSerializer.Deserialize<List<string>>(reader.GetString(7)) ?? new(),
                PageType = Enum.TryParse<SCPPageType>(reader.GetString(8), true, out var pt) ? pt : SCPPageType.Standard,
                HubLinks = JsonSerializer.Deserialize<List<SCPItemList>>(reader.GetString(9)) ?? new(),
                Footnotes = JsonSerializer.Deserialize<List<SCPFootnote>>(reader.GetString(10)) ?? new(),
                ContentBlocks = JsonSerializer.Deserialize<List<ContentBlock>>(reader.GetString(11)) ?? new(),
                SubPageUrls = JsonSerializer.Deserialize<List<string>>(reader.GetString(12)) ?? new()
            };
        }

        public void Set(string url, SCPItem item)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = @"
                INSERT OR REPLACE INTO cache_items
                    (url, name, safe_level, special_measures, contents,
                     collapsible_contents, blockquote_contents, image_urls, tables,
                     page_type, hub_links, footnotes, content_blocks, sub_page_urls, cached_at)
                VALUES
                    (@url, @name, @safe_level, @special_measures, @contents,
                     @collapsible_contents, @blockquote_contents, @image_urls, @tables,
                     @page_type, @hub_links, @footnotes, @content_blocks, @sub_page_urls, @cached_at)";

            cmd.Parameters.AddWithValue("@url", url);
            cmd.Parameters.AddWithValue("@name", item.Name);
            cmd.Parameters.AddWithValue("@safe_level", item.SafeLevel);
            cmd.Parameters.AddWithValue("@special_measures", item.SpecialMeasures);
            cmd.Parameters.AddWithValue("@contents", item.Contents);
            cmd.Parameters.AddWithValue("@collapsible_contents", JsonSerializer.Serialize(item.CollapsibleContents));
            cmd.Parameters.AddWithValue("@blockquote_contents", JsonSerializer.Serialize(item.BlockQuoteContents));
            cmd.Parameters.AddWithValue("@image_urls", JsonSerializer.Serialize(item.ImageUrls));
            cmd.Parameters.AddWithValue("@tables", JsonSerializer.Serialize(item.Tables));
            cmd.Parameters.AddWithValue("@page_type", item.PageType.ToString());
            cmd.Parameters.AddWithValue("@hub_links", JsonSerializer.Serialize(item.HubLinks));
            cmd.Parameters.AddWithValue("@footnotes", JsonSerializer.Serialize(item.Footnotes));
            cmd.Parameters.AddWithValue("@content_blocks", JsonSerializer.Serialize(item.ContentBlocks));
            cmd.Parameters.AddWithValue("@sub_page_urls", JsonSerializer.Serialize(item.SubPageUrls));
            cmd.Parameters.AddWithValue("@cached_at", DateTime.UtcNow.ToString("o"));

            cmd.ExecuteNonQuery();
        }

        public bool Exists(string url)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(1) FROM cache_items WHERE url = @url";
            cmd.Parameters.AddWithValue("@url", url);
            return (long)cmd.ExecuteScalar()! > 0;
        }

        public void Remove(string url)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = "DELETE FROM cache_items WHERE url = @url";
            cmd.Parameters.AddWithValue("@url", url);
            cmd.ExecuteNonQuery();
        }

        public void Clear()
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = "DELETE FROM cache_items";
            cmd.ExecuteNonQuery();
        }

        public int Count()
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(1) FROM cache_items";
            return (int)(long)cmd.ExecuteScalar()!;
        }

        public Dictionary<string, List<SCPItemList>>? GetList(string url)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = "SELECT data FROM cache_lists WHERE url = @url";
            cmd.Parameters.AddWithValue("@url", url);

            var result = cmd.ExecuteScalar();
            if (result == null) return null;

            return JsonSerializer.Deserialize<Dictionary<string, List<SCPItemList>>>(result.ToString()!);
        }

        public void SetList(string url, Dictionary<string, List<SCPItemList>> data)
        {
            var cmd = _connection.CreateCommand();
            cmd.CommandText = @"
                INSERT OR REPLACE INTO cache_lists (url, data, cached_at)
                VALUES (@url, @data, @cached_at)";
            cmd.Parameters.AddWithValue("@url", url);
            cmd.Parameters.AddWithValue("@data", JsonSerializer.Serialize(data));
            cmd.Parameters.AddWithValue("@cached_at", DateTime.UtcNow.ToString("o"));
            cmd.ExecuteNonQuery();
        }

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
